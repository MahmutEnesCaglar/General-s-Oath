using UnityEngine;
using TowerDefense.Core;

namespace TowerDefense.Enemy
{
    /// <summary>
    /// Düşman karakterlerin ana MonoBehaviour script'i
    /// Her düşman prefab'ına bu script eklenecek
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Enemy : MonoBehaviour
    {
        [Header("Düşman Tipi")]
        public string enemyType = "basic"; // basic, fast, armored, vs.

        [Header("İstatistikler")]
        public int maxHealth = 100;
        public int currentHealth;
        public int damage = 5;           // Üsse verdiği hasar
        public float moveSpeed = 2f;     // Hareket hızı
        public int moneyReward = 5;      // Öldürüldüğünde verdiği para
        public bool isFlying = false;    // Havada uçuyor mu?
        public bool isBoss = false;      // Boss mu?

        [Header("Hero Aggro System")]
        public float aggroRange = 3f;    // Hero'yu algılama menzili
        public int damageToHero = 5;     // Hero'ya verdiği hasar
        public float heroAttackCooldown = 1.5f; // Hero'ya saldırı cooldown
        private TowerDefense.Hero.Hero currentHeroTarget;
        private float heroAttackTimer = 0f;
        private bool isAttacking = false; // Saldırı animasyonu oynarken true
        private float attackAnimationDuration = 0.8f; // Attack animasyon süresi

        [Header("Collision Avoidance")]
        public float separationRadius = 0.05f;      // Diğer düşmanlardan ayrılma mesafesi
        public float separationForce = 2f;         // Ayrılma kuvveti
        public float heroAttackRadius = 4f;      // Hero etrafında sıralanma yarıçapı
        private Vector2 separationVelocity;        // Ayrılma hız vektörü
        private int heroAttackSlot = -1;           // Hero'nun etrafında hangi slot'ta olacak
        private static int nextAttackSlot = 0;     // Sonraki attack slot numarası

        [Header("Hareket")]
        [HideInInspector]
        public Transform[] waypoints;    // Takip edilecek yol noktaları (spawner tarafından atanır)
        private int currentWaypointIndex = 0;
        private bool hasReachedBase = false;

        [Header("Görsel")]
        private SpriteRenderer spriteRenderer;
        private Animator animator;
        private Vector3 lastPosition;  // Animasyon için hareket algılama

        [Header("Health Bar (Opsiyonel)")]
        public GameObject healthBarPrefab;
        private GameObject healthBarInstance;

        private void Awake()
        {
            // SpriteRenderer ve Animator'ı bul (parent veya child'larda)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            animator = GetComponentInChildren<Animator>();

            // Animator speed'i ayarla (animasyonun oynatılması için)
            if (animator != null)
            {
                animator.speed = 1f;  // Normal hız
            }

            // Rigidbody2D ayarları
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.isKinematic = true;        // Manuel hareket ama collision detection var
            rb.gravityScale = 0f;         // Yerçekimi yok
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Collision detection aktif

            // CircleCollider2D ayarları
            CircleCollider2D col = GetComponent<CircleCollider2D>();
            if (col == null)
            {
                col = gameObject.AddComponent<CircleCollider2D>();
            }
            col.radius = 0.3f;            // Düşman boyutuna göre ayarla
            col.isTrigger = false;        // Trigger DEĞİL, gerçek collision
        }

        private void Start()
        {
            currentHealth = maxHealth;

            // Miniature Army 2D: Animator'da parametre yok, direkt state çalacağız
            // İlk animasyon: idle
            if (animator != null)
            {
                animator.Play("idle");
            }

            // Waypoint'leri otomatik bul (eğer spawner atamamışsa)
            if (waypoints == null || waypoints.Length == 0)
            {
                Debug.LogWarning($"{gameObject.name}: Waypoint'ler atanmadı, otomatik aranıyor...");
                FindWaypoints();
            }

            // İzometrik sorting için (Y pozisyonuna göre sıralama)
            UpdateSortingOrder();

            // İlk pozisyonu kaydet (hareket algılama için)
            lastPosition = transform.position;
        }

        private void Update()
        {
            if (hasReachedBase) return;

            // Hero aggro check
            CheckForHero();

            // Hareket: Hero varsa ona git, yoksa waypoint'lere
            if (currentHeroTarget != null && !currentHeroTarget.isDead)
            {
                MoveTowardsHero();
            }
            else
            {
                // Hero'yu kaybettik, attack slot'u serbest bırak
                if (currentHeroTarget != null)
                {
                    heroAttackSlot = -1;
                }
                currentHeroTarget = null;
                MoveTowardsWaypoint();
            }

            // Sorting order'ı sürekli güncelle (izometrik için)
            UpdateSortingOrder();

            // Animator'ı güncelle (yürüme animasyonu)
            UpdateAnimator();
        }

        /// <summary>
        /// Hero'yu algılar ve hedef olarak ayarlar
        /// </summary>
        private void CheckForHero()
        {
            // Eğer zaten bir hero hedefimiz varsa tekrar arama
            if (currentHeroTarget != null && !currentHeroTarget.isDead)
            {
                float distanceToHero = Vector2.Distance(transform.position, currentHeroTarget.transform.position);
                if (distanceToHero > aggroRange * 1.5f) // Range dışına çıkarsa hedefi bırak
                {
                    currentHeroTarget = null;
                    heroAttackSlot = -1; // Attack slot'u serbest bırak
                }
                return;
            }

            // Hero ara
            TowerDefense.Hero.Hero hero = FindObjectOfType<TowerDefense.Hero.Hero>();
            if (hero != null && !hero.isDead)
            {
                float distanceToHero = Vector2.Distance(transform.position, hero.transform.position);
                if (distanceToHero <= aggroRange)
                {
                    currentHeroTarget = hero;
                }
            }
        }

        /// <summary>
        /// Hero'ya doğru hareket eder ve saldırır
        /// </summary>
        private void MoveTowardsHero()
        {
            if (currentHeroTarget == null) return;

            // Saldırı animasyonu oynarken hareket etme
            if (isAttacking) return;

            // Hero attack slot ata (eğer atanmamışsa)
            if (heroAttackSlot == -1)
            {
                heroAttackSlot = nextAttackSlot;
                nextAttackSlot = (nextAttackSlot + 1) % 8; // 8 slot (her 45 derece)
            }

            // Hero etrafında hedef pozisyon hesapla (circular formation)
            Vector2 heroPos = currentHeroTarget.transform.position;
            float angle = heroAttackSlot * 45f * Mathf.Deg2Rad; // 8 slot = 360/8 = 45 derece
            Vector2 targetPosition = heroPos + new Vector2(
                Mathf.Cos(angle) * heroAttackRadius,
                Mathf.Sin(angle) * heroAttackRadius
            );

            // Hedef pozisyona doğru hareket
            Vector2 directionToTarget = (targetPosition - (Vector2)transform.position).normalized;
            float distanceToTarget = Vector2.Distance(transform.position, targetPosition);

            // Separation force (diğer düşmanlardan ayrılma)
            Vector2 separation = CalculateSeparation();

            // Toplam hareket vektörü
            Vector2 movement = directionToTarget * moveSpeed + separation * separationForce;

            // Hedefe yeterince yakın mı? (saldırı mesafesinde)
            if (distanceToTarget > 0.3f)
            {
                // Hedefe doğru hareket et
                transform.position += (Vector3)movement * Time.deltaTime;
                FlipSprite(movement.x);
            }
            else
            {
                // Saldırı mesafesindesin - Hero'ya saldır
                AttackHero();
            }
        }

        /// <summary>
        /// Diğer düşmanlardan ayrılma kuvveti hesaplar (separation/avoidance)
        /// </summary>
        private Vector2 CalculateSeparation()
        {
            Vector2 separationVector = Vector2.zero;
            int nearbyEnemyCount = 0;

            // Yakındaki tüm düşmanları bul
            Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, separationRadius);

            foreach (Collider2D col in nearbyColliders)
            {
                // Kendisi değilse ve düşman ise
                if (col.gameObject != gameObject && col.GetComponent<Enemy>() != null)
                {
                    // Bu düşmandan uzaklaş
                    Vector2 awayFromEnemy = (Vector2)transform.position - (Vector2)col.transform.position;
                    float distance = awayFromEnemy.magnitude;

                    if (distance > 0.01f) // Sıfıra bölme hatası önleme
                    {
                        // Yakın olan düşmanlardan daha fazla uzaklaş
                        separationVector += awayFromEnemy.normalized / distance;
                        nearbyEnemyCount++;
                    }
                }
            }

            // Ortalama separation vektörü
            if (nearbyEnemyCount > 0)
            {
                separationVector /= nearbyEnemyCount;
            }

            return separationVector;
        }

        /// <summary>
        /// Hero'ya saldırır
        /// </summary>
        private void AttackHero()
        {
            if (currentHeroTarget == null) return;

            // Eğer zaten saldırı animasyonu oynanıyorsa bekle
            if (isAttacking) return;

            // Cooldown azalt
            if (heroAttackTimer > 0)
            {
                heroAttackTimer -= Time.deltaTime;
                return;
            }

            // ÖNEMLİ: Coroutine başlamadan önce flag'i set et
            // Aksi takdirde aynı frame'de UpdateAnimator() AnimState değiştirip Attack trigger'ı bozar
            isAttacking = true;

            // Saldırı başlat
            StartCoroutine(PerformAttack());
        }

        /// <summary>
        /// Saldırı animasyonunu oynatır ve hasar verir
        /// </summary>
        private System.Collections.IEnumerator PerformAttack()
        {
            // Saldırı animasyonunu çal (Miniature Army 2D)
            if (animator != null)
            {
                animator.Play("attack");
            }

            // Animasyonun yarısında hasar ver (daha gerçekçi)
            yield return new WaitForSeconds(attackAnimationDuration * 0.5f);

            // Hero'ya hasar ver
            if (currentHeroTarget != null && !currentHeroTarget.isDead)
            {
                currentHeroTarget.TakeDamage(damageToHero);
            }

            // Animasyonun bitmesini bekle
            yield return new WaitForSeconds(attackAnimationDuration * 0.5f);

            // Cooldown başlat
            heroAttackTimer = heroAttackCooldown;
            isAttacking = false;
        }

        /// <summary>
        /// Waypoint'lere doğru hareket eder
        /// </summary>
        private void MoveTowardsWaypoint()
        {
            if (waypoints == null || waypoints.Length == 0)
            {
                Debug.LogWarning($"{gameObject.name}: Waypoint bulunamadı!");
                return;
            }

            if (currentWaypointIndex >= waypoints.Length)
            {
                // Tüm waypoint'lere ulaştı - üsse ulaştı
                ReachBase();
                return;
            }

            // Hedef waypoint
            Transform targetWaypoint = waypoints[currentWaypointIndex];

            // Hedefe doğru hareket et
            Vector2 direction = (targetWaypoint.position - transform.position).normalized;

            // Separation force (diğer düşmanlardan ayrılma)
            Vector2 separation = CalculateSeparation();

            // Toplam hareket vektörü
            Vector2 movement = direction * moveSpeed + separation * separationForce;

            transform.position += (Vector3)movement * Time.deltaTime;

            // Hedefe yaklaştı mı?
            float distanceToWaypoint = Vector2.Distance(transform.position, targetWaypoint.position);
            if (distanceToWaypoint < 0.1f)
            {
                currentWaypointIndex++; // Sonraki waypoint'e geç
            }

            // Sprite'ı hareket yönüne göre çevir (opsiyonel)
            FlipSprite(movement.x);
        }

        /// <summary>
        /// Sprite'ı hareket yönüne göre çevirir
        /// NOT: Modular karakterler için tüm child SpriteRenderer'ları flip eder
        /// </summary>
        private void FlipSprite(float directionX)
        {
            // Modular karakterler için tüm SpriteRenderer'ları flip et
            SpriteRenderer[] allSprites = GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer sr in allSprites)
            {
                // Sprite varsayılan SOLA bakıyor
                if (directionX > 0)
                    sr.flipX = true;   // Sağa git = Flip et
                else if (directionX < 0)
                    sr.flipX = false;  // Sola git = Normal (flip yok)
            }
        }

        /// <summary>
        /// Animator'ı günceller (yürüme animasyonu)
        /// Miniature Army 2D için direkt state isimleri kullanılır:
        /// - idle: Duruyor
        /// - walk: Yürüyor
        /// </summary>
        private void UpdateAnimator()
        {
            if (animator == null) return;

            // ÖNEMLI: Saldırı animasyonu oynarken state değiştirme!
            if (isAttacking)
            {
                return; // Attack animasyonu devam etsin
            }

            // Gerçek hareket algılama: Bu frame'de ne kadar hareket etti?
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);
            bool isMoving = distanceMoved > 0.001f; // 0.001 unit'ten fazla hareket ettiyse yürüyor

            // Miniature Army 2D: walk veya idle
            // Play yerine CrossFade kullan (daha smooth geçişler)
            if (isMoving)
            {
                // Sadece walk animasyonu çalmıyorsa çal
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (!stateInfo.IsName("walk"))
                {
                    animator.CrossFade("walk", 0.1f);
                }
            }
            else
            {
                // Sadece idle animasyonu çalmıyorsa çal
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (!stateInfo.IsName("idle"))
                {
                    animator.CrossFade("idle", 0.1f);
                }
            }
        }

        /// <summary>
        /// Frame sonunda lastPosition'ı güncelle
        /// </summary>
        private void LateUpdate()
        {
            // Bir sonraki frame için pozisyonu kaydet
            lastPosition = transform.position;
        }

        /// <summary>
        /// İzometrik görünüm için Y pozisyonuna göre sorting yapar
        /// Alttaki karakterler (düşük Y) önde görünür
        /// Sorting mimarisi için SortingOrderConfig'e bakın
        /// </summary>
        private void UpdateSortingOrder()
        {
            // Modular karakterler için tüm SpriteRenderer'ların sorting order'ını güncelle
            SpriteRenderer[] allSprites = GetComponentsInChildren<SpriteRenderer>();
            int baseSortingOrder = SortingOrderConfig.GetCharacterSortingOrder(transform.position.y);

            foreach (SpriteRenderer sr in allSprites)
            {
                // Her child sprite kendi relative sorting'ini korur
                sr.sortingOrder = baseSortingOrder;
            }
        }

        /// <summary>
        /// Waypoint'leri otomatik bulur (Path parent objesi altında)
        /// </summary>
        private void FindWaypoints()
        {
            GameObject pathParent = GameObject.Find("EnemyPath");
            if (pathParent != null)
            {
                int childCount = pathParent.transform.childCount;
                waypoints = new Transform[childCount];

                for (int i = 0; i < childCount; i++)
                {
                    waypoints[i] = pathParent.transform.GetChild(i);
                }

                Debug.Log($"{gameObject.name}: {waypoints.Length} waypoint bulundu.");
            }
            else
            {
                Debug.LogError("EnemyPath objesi bulunamadı! Scene'de EnemyPath adlı bir GameObject oluşturun.");
            }
        }

        /// <summary>
        /// Düşman hasar alır
        /// </summary>
        public void TakeDamage(int damageAmount)
        {
            currentHealth -= damageAmount;

            // Hasar efekti (sprite'ı kırmızı yap)
            StartCoroutine(DamageFlash());

            // Hasar alma animasyonu (Miniature Army 2D)
            if (animator != null && currentHealth > 0)
            {
                animator.Play("hurt");
            }

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Hasar alınca sprite'ları kırmızı yapar
        /// </summary>
        private System.Collections.IEnumerator DamageFlash()
        {
            SpriteRenderer[] allSprites = GetComponentsInChildren<SpriteRenderer>();

            // Tüm sprite'ları kırmızı yap
            foreach (SpriteRenderer sr in allSprites)
            {
                sr.color = Color.red;
            }

            yield return new WaitForSeconds(0.1f);

            // Tüm sprite'ları beyaza döndür
            foreach (SpriteRenderer sr in allSprites)
            {
                sr.color = Color.white;
            }
        }

        /// <summary>
        /// Düşman ölür
        /// </summary>
        private void Die()
        {
            // GameManager'a para ver
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnEnemyKilled(moneyReward);
            }

            // Ölüm animasyonu çal (Miniature Army 2D)
            if (animator != null)
            {
                animator.Play("die");
            }

            // Ölüm efekti eklenebilir
            // Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

            // Düşmanı yok et (animasyon süresi kadar bekle)
            Destroy(gameObject, 1.5f);  // 1.5 saniye sonra yok et (animasyon için)
        }

        /// <summary>
        /// Düşman üsse ulaştı
        /// Her düşman için -1 can
        /// </summary>
        private void ReachBase()
        {
            if (hasReachedBase) return;

            hasReachedBase = true;

            // GameManager'a bildir (her düşman için -1 can)
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnEnemyReachedBase(1); // Parametre kullanılmıyor ama tutarlılık için 1 gönderiyoruz
            }

            // Düşmanı yok et
            Destroy(gameObject);
        }

        /// <summary>
        /// Düşman istatistiklerini EnemyType data'sından yükler
        /// </summary>
        public void InitializeFromEnemyType(EnemyType enemyTypeData)
        {
            enemyType = enemyTypeData.type;
            maxHealth = enemyTypeData.hp;
            currentHealth = maxHealth;
            damage = enemyTypeData.damage;
            damageToHero = enemyTypeData.damage; // Hero'ya da aynı hasar
            moveSpeed = enemyTypeData.speed * 0.1f; // Speed değerini Unity birimlerine çevir
            moneyReward = enemyTypeData.money;
            isFlying = enemyTypeData.isFlying;
            isBoss = enemyTypeData.isBoss;

            // Boss ise daha büyük göster ve aggro range artır
            if (isBoss)
            {
                transform.localScale = Vector3.one * 1.5f;
                aggroRange = 5f; // Boss daha uzaktan algılar
            }

            // Hero attack cooldown düşman hızına göre ayarla
            heroAttackCooldown = 1.5f / (moveSpeed / 2f); // Hızlı düşmanlar daha sık vurur
        }

        /// <summary>
        /// Gizmos ile waypoint yolunu göster (Editor'de)
        /// </summary>
        private void OnDrawGizmos()
        {
            if (waypoints == null || waypoints.Length == 0) return;

            Gizmos.color = Color.yellow;
            for (int i = 0; i < waypoints.Length - 1; i++)
            {
                if (waypoints[i] != null && waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }
            }
        }

        /// <summary>
        /// Gizmos ile collision/separation radius'u göster (Editor'de)
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            // Separation radius (mavi)
            Gizmos.color = new Color(0, 0, 1, 0.3f);
            Gizmos.DrawWireSphere(transform.position, separationRadius);

            // Hero attack radius (kırmızı) - sadece hero varsa
            if (currentHeroTarget != null)
            {
                Gizmos.color = new Color(1, 0, 0, 0.3f);
                Gizmos.DrawWireSphere(currentHeroTarget.transform.position, heroAttackRadius);

                // Attack slot pozisyonu (yeşil)
                if (heroAttackSlot >= 0)
                {
                    Vector2 heroPos = currentHeroTarget.transform.position;
                    float angle = heroAttackSlot * 45f * Mathf.Deg2Rad;
                    Vector2 slotPosition = heroPos + new Vector2(
                        Mathf.Cos(angle) * heroAttackRadius,
                        Mathf.Sin(angle) * heroAttackRadius
                    );

                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(slotPosition, 0.1f);
                    Gizmos.DrawLine(transform.position, slotPosition);
                }
            }
        }
    }
}
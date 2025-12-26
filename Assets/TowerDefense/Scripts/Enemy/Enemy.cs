using UnityEngine;
using TowerDefense.Core;

namespace TowerDefense.Enemy
{
    /// <summary>
    /// Düşman karakterlerin ana MonoBehaviour script'i
    /// Her düşman prefab'ına bu script eklenecek
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
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

        [Header("Hareket")]
        [HideInInspector]
        public Transform[] waypoints;    // Takip edilecek yol noktaları (spawner tarafından atanır)
        private int currentWaypointIndex = 0;
        private bool hasReachedBase = false;

        [Header("Görsel")]
        private SpriteRenderer spriteRenderer;
        private Animator animator;

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
            rb.isKinematic = true;        // Fizik simülasyonu yok, sadece hareket
            rb.gravityScale = 0f;         // Yerçekimi yok
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
                    Debug.Log($"{gameObject.name} aggro on Hero! Distance: {distanceToHero:F2}");
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

            // Hero'ya doğru hareket
            Vector2 direction = (currentHeroTarget.transform.position - transform.position).normalized;
            float distanceToHero = Vector2.Distance(transform.position, currentHeroTarget.transform.position);

            // Saldırı menzilinde mi? (0.8 unit yaklaş)
            if (distanceToHero > 0.8f)
            {
                // Hero'ya yaklaş
                transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
                FlipSprite(direction.x);
            }
            else
            {
                // Saldırı menzilindesiniz - Hero'ya saldır
                AttackHero();
            }
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
                Debug.Log($"{gameObject.name} attacks Hero for {damageToHero} damage!");
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
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

            // Hedefe yaklaştı mı?
            float distanceToWaypoint = Vector2.Distance(transform.position, targetWaypoint.position);
            if (distanceToWaypoint < 0.1f)
            {
                currentWaypointIndex++; // Sonraki waypoint'e geç
            }

            // Sprite'ı hareket yönüne göre çevir (opsiyonel)
            FlipSprite(direction.x);
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

            // Hareket halinde mi?
            bool isMoving = moveSpeed > 0.1f;

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
        /// </summary>
        private void ReachBase()
        {
            if (hasReachedBase) return;

            hasReachedBase = true;

            // GameManager'a hasar ver
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnEnemyReachedBase(damage);
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
    }
}
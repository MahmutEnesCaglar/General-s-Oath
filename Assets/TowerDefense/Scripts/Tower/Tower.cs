using UnityEngine;
using System.Collections.Generic;
using TowerDefense.Enemy;
using TowerDefense.Core; // GameManager için
using TowerDefense.UI; // <--- BU SATIRI EN ÜSTE EKLE

namespace TowerDefense.Tower
{
    public class Tower : MonoBehaviour
    {
        [Header("Temel Ayarlar")]
        public string towerName = "Kule";
        public float range = 5f;
        public float fireRate = 1f;
        public int damage = 10;
        
        [Header("UI Ayarları")]
        [Tooltip("Upgrade menüsünün kule üzerinde ne kadar yukarıda çıkacağını belirler.")]
        public float uiYOffset = 150f; // Varsayılan değer

        private float baseFireRate;
        private int baseDamage;
        private bool isEnraged = false; // Şu an öfkeli mi?

        [Header("İzometrik Ayar")]
        [Range(0.1f, 1f)]
        public float verticalRangeModifier = 0.6f;

        [Header("Hedefleme ve Liste")]
        public List<GameObject> enemiesInRange = new List<GameObject>();
        public GameObject currentTarget;
        protected float fireCooldown = 0f;

        [Header("Setup")]
        public GameObject projectilePrefab;
        protected RotatableTowerSprite rotatableVisual;

        // Kule inşa edildiğinde hangi BuildSpot üzerine kurulduğunu saklayalım
        [HideInInspector] public GameObject occupiedSpot;

        [Header("Upgrade Sistemi")]
        public int buildCost = 50; // İlk yerleştirme maliyeti
        public int currentLevel = 1;
        public int maxLevel = 3;
        public int totalSpent = 0; // Toplam harcanan para (Satış için)

        [Header("UI Referansı")]
        public GameObject upgradeCanvasPrefab; // Kuleye tıklayınca çıkacak buton prefabı
        private GameObject activeUpgradeUI;    // O an açık olan UI

        [System.Serializable]
        public struct LevelData
        {
            public int cost;
            public float range;
            public float fireRate;
            public int damage;
            
            // BURASI DEĞİŞTİ: Artık her level kendi tam paketini taşıyor (Resim + Scale + Offset)
            public List<DirectionalData> visualData; 
        }

        // Değişiklik kontrolü
        private float lastRange;
        private float lastModifier;

        public List<LevelData> levels;

        protected virtual void Start()
        {
            rotatableVisual = GetComponentInChildren<RotatableTowerSprite>();
            
            lastRange = range;
            lastModifier = verticalRangeModifier;

            rotatableVisual = GetComponentInChildren<RotatableTowerSprite>();
            CreateOvalRangeDetector();
            
            // ORİJİNAL DEĞERLERİ KAYDET (Rage bitince buna döneceğiz)
            baseFireRate = fireRate;
            baseDamage = damage;
        }

        protected virtual void Update()
        {
            // Dinamik güncelleme
            if (range != lastRange || verticalRangeModifier != lastModifier)
            {
                CreateOvalRangeDetector();
                lastRange = range;
                lastModifier = verticalRangeModifier;
            }

            fireCooldown -= Time.deltaTime;

            UpdateTarget();

            if (currentTarget != null)
            {
                if (rotatableVisual != null)
                    rotatableVisual.RotateTowards(currentTarget.transform.position);

                if (fireCooldown <= 0)
                {
                    Attack();
                    fireCooldown = 1f / fireRate;
                }
            }
        }

        // --- MERMİ ATMA ---
        protected virtual void Attack()
        {
            if (projectilePrefab != null && currentTarget != null)
            {
                Vector3 spawnPos = transform.position;
                if (rotatableVisual != null)
                {
                    Vector2 offset = rotatableVisual.GetCurrentFirePointOffset(rotatableVisual.currentSegmentIndex);
                    spawnPos = transform.position + (Vector3)offset;
                }

                GameObject projObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

                // 1. Tip: Normal Projectile
                Projectile p = projObj.GetComponent<Projectile>();
                if (p != null) { p.Setup(currentTarget, damage); return; }

                // 2. Tip: Arrow Projectile
                ArrowProjectile a = projObj.GetComponent<ArrowProjectile>();
                if (a != null) { a.Setup(currentTarget, damage); return; }

                // 3. Tip: Mortar Projectile
                MortarProjectile m = projObj.GetComponent<MortarProjectile>();
                if (m != null) 
                { 
                    // DÜZELTME BURADA YAPILDI:
                    // Eğer bu scripti "MortarTower" olmayan düz bir kulede kullanırsan
                    // varsayılan olarak 1.5f alan hasarı ile çalışsın diye değer girdik.
                    // (MortarTower zaten bu fonksiyonu override edip kendi değerini gönderiyor)
                    m.Setup(currentTarget, damage, 1.5f); 
                    return; 
                }
            }
        }

        // --- OVAL COLLIDER OLUŞTURUCU ---
        public void CreateOvalRangeDetector()
        {
            Transform oldDetector = transform.Find("RangeDetector");
            if (oldDetector != null) Destroy(oldDetector.gameObject);

            GameObject detector = new GameObject("RangeDetector");
            detector.transform.SetParent(this.transform);
            detector.transform.localPosition = Vector3.zero;
            detector.transform.localScale = Vector3.one; 
            detector.layer = gameObject.layer;

            PolygonCollider2D polyCol = detector.AddComponent<PolygonCollider2D>();
            polyCol.isTrigger = true;

            int pointCount = 36;
            Vector2[] points = new Vector2[pointCount];
            
            for (int i = 0; i < pointCount; i++)
            {
                float angle = (2 * Mathf.PI * i) / pointCount;
                float x = Mathf.Cos(angle) * range; 
                float y = Mathf.Sin(angle) * range * verticalRangeModifier;
                points[i] = new Vector2(x, y);
            }

            polyCol.points = points;

            RangeRelay relay = detector.AddComponent<RangeRelay>();
            relay.towerParent = this;
        }

        // --- FİZİK İLETİŞİMİ ---
        public void OnEnemyEnter(GameObject enemy)
        {
            if (!enemiesInRange.Contains(enemy)) enemiesInRange.Add(enemy);
        }

        public void OnEnemyExit(GameObject enemy)
        {
            if (enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Remove(enemy);
                if (currentTarget == enemy) currentTarget = null;
            }
        }

        protected virtual void UpdateTarget()
        {
            enemiesInRange.RemoveAll(item => item == null);

            if (currentTarget == null && enemiesInRange.Count > 0)
            {
                currentTarget = GetClosestEnemy();
            }
        }

        private GameObject GetClosestEnemy()
        {
            GameObject bestTarget = null;
            float closestDistance = Mathf.Infinity;

            foreach (GameObject enemy in enemiesInRange)
            {
                if (enemy == null) continue;

                float dist = GetIsometricDistance(transform.position, enemy.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    bestTarget = enemy;
                }
            }
            return bestTarget;
        }

        protected float GetIsometricDistance(Vector3 posA, Vector3 posB)
        {
            float diffX = posA.x - posB.x;
            float diffY = (posA.y - posB.y) / verticalRangeModifier;
            return Mathf.Sqrt(diffX * diffX + diffY * diffY);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(1, verticalRangeModifier, 1));
            Gizmos.DrawWireSphere(Vector3.zero, range);
            Gizmos.matrix = oldMatrix;
        }
        public void EnableRage(float damageMultiplier, float speedMultiplier)
        {
            if (isEnraged) return; // Zaten öfkeliyse tekrar yapma

            isEnraged = true;
            
            // Hızı ve hasarı artır
            damage = Mathf.RoundToInt(baseDamage * damageMultiplier);
            fireRate = baseFireRate * speedMultiplier;

            // Görsel efekt: Kuleyi kırmızı yap (Varsa SpriteRenderer'ı bul)
            var sprites = GetComponentsInChildren<SpriteRenderer>();
            foreach (var sprite in sprites)
            {
                sprite.color = Color.red; 
            }

            Debug.Log($"{towerName} ÖFKELENDİ! Hız: {fireRate}, Hasar: {damage}");
        }

        public void DisableRage()
        {
            if (!isEnraged) return;

            isEnraged = false;

            // Değerleri normale döndür
            damage = baseDamage;
            fireRate = baseFireRate;

            // Rengi normale döndür
            var sprites = GetComponentsInChildren<SpriteRenderer>();
            foreach (var sprite in sprites)
            {
                sprite.color = Color.white; 
            }

            Debug.Log($"{towerName} sakinleşti.");
        }
        
        void OnMouseDown()
        {
            // UI veya özel modlar (Bariyer vb.) açıksa tıklama
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
            
            ToggleUpgradeUI();
        }

        public void ToggleUpgradeUI()
        {
            // Zaten açıksa kapat
            if (activeUpgradeUI != null)
            {
                Destroy(activeUpgradeUI);
                return;
            }

            // Maksimum seviyedeysek açma
            if (currentLevel >= maxLevel)
            {
                Debug.Log("Kule maksimum seviyede!");
                return;
            }

            // UI oluştur (Kulenin tepesinde)
            if (upgradeCanvasPrefab != null)
            {
                // Diğer tüm açık UI'ları kapatmak istersen burada bir Event çağırabilirsin
                
                activeUpgradeUI = Instantiate(upgradeCanvasPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
                
                // UI içindeki scripti bul ve ayarla (Birazdan yazacağız: TowerUpgradeUI)
                TowerUpgradeUI uiScript = activeUpgradeUI.GetComponent<TowerUpgradeUI>();
                if (uiScript != null)
                {
                    // Sıradaki seviyenin verisini bul (Level 1 isek index 0, Level 2 verisini alacağız)
                    // Logic: currentLevel 1 ise, levels[0] bize Level 2 bilgilerini verir.
                    // Çünkü levels listesine sadece YÜKSELTMELERİ koyacağız.
                    
                    if (currentLevel - 1 < levels.Count)
                    {
                        LevelData nextLevel = levels[currentLevel - 1];
                        uiScript.Setup(this, nextLevel.cost);
                    }
                }
            }
        }

        public void Initialize(int cost)
        {
            totalSpent = cost;
        }

        public bool Upgrade()
        {
            int nextLevelIndex = currentLevel - 1;

            // Index ve Liste güvenliği kontrolü
            if (levels == null || nextLevelIndex >= levels.Count)
            {
                Debug.LogWarning("Yükseltilecek seviye verisi bulunamadı!");
                return false;
            }

            LevelData data = levels[nextLevelIndex];

            // Para Kontrolü
            if (MoneyManager.Instance != null && MoneyManager.Instance.currentMoney >= data.cost)
            {
                MoneyManager.Instance.SpendMoney(data.cost);

                currentLevel++;
                totalSpent += data.cost; // Harcanan parayı ekle

                // İstatistikleri güncelle
                this.range = data.range;
                this.fireRate = data.fireRate;
                this.damage = data.damage;

                // Base değerleri de güncelle (Rage sisteminin doğru çalışması için)
                baseDamage = data.damage;
                baseFireRate = data.fireRate;

                // --- GÖRSEL GÜNCELLEME ---
                // Child objedeki scripti bul
                RotatableTowerSprite rotator = GetComponentInChildren<RotatableTowerSprite>();
                if (rotator != null)
                {
                    // Yeni levelin indexini gönder (currentLevel 1 tabanlı, index 0 tabanlı)
                    // Level 1 -> Index 0
                    // Level 2 -> Index 1
                    rotator.SetLevel(currentLevel - 1);
                }

                Debug.Log($"Kule Level {currentLevel} oldu!");

                // Menüyü kapat
                if (activeUpgradeUI != null) Destroy(activeUpgradeUI);
                return true;
            }
            else
            {
                Debug.Log("Yetersiz Para!");
                return false;
            }
        }

        public void Sell()
        {
            int refundAmount = totalSpent / 2;
            if (MoneyManager.Instance != null)
            {
                MoneyManager.Instance.AddMoney(refundAmount);
            }

            // BuildSpot'u tekrar aktif et
            if (occupiedSpot != null)
            {
                Collider2D col = occupiedSpot.GetComponent<Collider2D>();
                if (col != null) col.enabled = true;
            }

            Destroy(gameObject);
        }
    }

    public class RangeRelay : MonoBehaviour
    {
        public Tower towerParent;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
                towerParent.OnEnemyEnter(other.gameObject);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
                towerParent.OnEnemyExit(other.gameObject);
        }
    }
}
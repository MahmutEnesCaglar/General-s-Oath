using UnityEngine;
using TowerDefense.UI; // BossHealthBar için

namespace TowerDefense.Enemy
{
    /// <summary>
    /// Boss Enemy - Evil Wizard 2 (Final Boss)
    /// HP: 500, Damage to Hero: 20, Damage to Barrier: 20
    /// Speed: 1.5, Reward: 50$
    ///
    /// Wave 10'daki tüm Elite'ler öldükten sonra Anka Simurg dialog'u gösterir,
    /// ardından bu Boss spawn'lanır.
    ///
    /// ASSETS: Evil Wizard 2 kullanılacak (kullanıcı sonra ayarlayacak)
    /// </summary>
    public class BossEnemy : BaseEnemy
    {
        [Header("Boss Özellikleri")]
        [Tooltip("Boss'un özel giriş efekti/animasyonu")]
        public GameObject spawnEffect;

        [Tooltip("Boss'un ölüm efekti")]
        public GameObject deathEffect;

        [Header("UI")]
        [Tooltip("Boss Health Bar Prefabı (BossHealthBar scripti içermeli)")]
        public GameObject bossHealthBarPrefab;
        private BossHealthBar healthBarScript;
        private GameObject healthBarInstance;

        private bool hasSpawnedMinions = false; // Gelecek için: minion spawn sistemi

        protected override void InitializeStats()
        {
            // Combat Stats
            maxHealth = 500;  // Çok yüksek HP
            currentHealth = maxHealth;
            damageToHero = 20;     // Çok yüksek hasar
            damageToBarrier = 20;
            damageToOthers = 20;
            moveSpeed = 1.5f;
            moneyReward = 50;

            // Attack Settings
            attackCooldown = 1.5f;
            aggroRange = 3f;  // Geniş aggro range

            // Properties
            isBoss = true;  // BOSS!
            isRanged = false;

            // Boss scale (daha büyük)
            transform.localScale = Vector3.one * 1.5f;

            Debug.Log($"<color=red>BossEnemy Initialized:</color> HP:{maxHealth} DMG:{damageToHero} SPD:{moveSpeed}");
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();

            // Boss spawn efekti
            if (spawnEffect != null)
            {
                Instantiate(spawnEffect, transform.position, Quaternion.identity);
            }

            // Health Bar Oluştur
            CreateHealthBar();

            Debug.Log($"<color=red>═══════════════════════════════════</color>");
            Debug.Log($"<color=red>BOSS SPAWNED: EVIL WIZARD!</color>");
            Debug.Log($"<color=red>HP: {currentHealth} | Damage: {damageToHero}</color>");
            Debug.Log($"<color=red>═══════════════════════════════════</color>");
        }

        private void CreateHealthBar()
        {
            if (bossHealthBarPrefab == null)
            {
                Debug.LogError("❌ BossHealthBar Prefab atanmamış! Inspector'dan atayın.");
                return;
            }

            // Hero.cs mantığı: Canvas aramıyoruz, direkt instantiate ediyoruz.
            // Prefab'ın kendi içinde Canvas'ı olması veya World Space olması bekleniyor.
            healthBarInstance = Instantiate(bossHealthBarPrefab);
            healthBarInstance.name = "BossHealthBar"; // İsim verelim ki karışmasın

            Debug.Log($"✅ Boss Health Bar oluşturuldu: {healthBarInstance.name}");

            healthBarScript = healthBarInstance.GetComponent<BossHealthBar>();
            
            if (healthBarScript != null)
            {
                healthBarScript.Initialize("Evil Wizard", maxHealth);
                Debug.Log("✅ Boss Health Bar scripti initialize edildi.");
            }
            else
            {
                // Script yoksa eklemeyi dene (Hero.cs gibi)
                Debug.LogWarning("⚠️ BossHealthBar scripti prefabda bulunamadı, runtime'da ekleniyor...");
                healthBarScript = healthBarInstance.AddComponent<BossHealthBar>();
                
                // Otomatik referans bulma (Hero.cs gibi)
                var fillTransform = healthBarInstance.transform.Find("Fill");
                if (fillTransform != null) healthBarScript.fillImage = fillTransform.GetComponent<UnityEngine.UI.Image>();
                
                var bgTransform = healthBarInstance.transform.Find("Background");
                if (bgTransform != null) healthBarScript.backgroundImage = bgTransform.GetComponent<UnityEngine.UI.Image>();
                
                var textTransform = healthBarInstance.transform.Find("HealthText");
                if (textTransform != null) healthBarScript.healthText = textTransform.GetComponent<TMPro.TextMeshProUGUI>();

                healthBarScript.Initialize("Evil Wizard", maxHealth);
            }
        }

        public override void TakeDamage(int damageAmount)
        {
            base.TakeDamage(damageAmount);
            
            // Health Bar Güncelle
            if (healthBarScript != null)
            {
                healthBarScript.UpdateHealthBar(currentHealth, maxHealth);
            }
        }

        protected override void OnDeath()
        {
            // Boss ölüm efekti
            if (deathEffect != null)
            {
                Instantiate(deathEffect, transform.position, Quaternion.identity);
            }

            // Health Bar'ı yok et
            if (healthBarInstance != null)
            {
                Destroy(healthBarInstance);
            }

            Debug.Log($"<color=yellow>═══════════════════════════════════</color>");
            Debug.Log($"<color=yellow>BOSS DEFEATED! EVIL WIZARD HAS FALLEN!</color>");
            Debug.Log($"<color=yellow>Victory! Wave Complete!</color>");
            Debug.Log($"<color=yellow>═══════════════════════════════════</color>");

            base.OnDeath();

            // Boss öldüğünde oyun kazanılır (WaveManager bunu dinler)
        }

        protected override void OnAttackPerformed()
        {
            base.OnAttackPerformed();
            // Boss saldırısı çok güçlü ve görsel efektli
            Debug.Log($"<color=red>BOSS ATTACK!</color> Dealing {damageToHero} damage!");
        }

        // Gelecek için: Boss özel yetenekleri
        // protected override void Update()
        // {
        //     base.Update();
        //
        //     // HP %50'nin altına düştüğünde minion spawn
        //     if (!hasSpawnedMinions && currentHealth <= maxHealth / 2)
        //     {
        //         SpawnMinions();
        //         hasSpawnedMinions = true;
        //     }
        // }
        //
        // private void SpawnMinions()
        // {
        //     // Boss HP %50'de 3-5 Fast Enemy spawn'lar
        //     Debug.Log("<color=red>BOSS SUMMONS REINFORCEMENTS!</color>");
        // }
    }
}

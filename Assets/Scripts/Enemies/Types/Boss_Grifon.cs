using UnityEngine;
using TowerDefense.UI; // BossHealthBar için

namespace TowerDefense.Enemy
{
    public class Boss_Grifon : BaseEnemyRefactored
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

        protected override void InitializeStats()
        {
            // Combat Stats
            maxHealth = 2000;
            currentHealth = maxHealth;
            damageToHero = 20;
            damageToBarrier = 20;
            damageFromHero = 15;
            damageFromTower = 5;
            moveSpeed = 1f;
            moneyReward = 50;

            // Attack Settings
            attackCooldown = 1.5f;
            aggroRange = 4f;
            meleReachOffset = 11f;
            maxPathDeviation = 50f;

            // Properties
            isBoss = true;
            isRanged = false;

            // Boss scale (daha büyük) - X eksenini negatif yaparak sprite yönünü düzelt
            transform.localScale = new Vector3(-1.5f, 1.5f, 1.5f);

            // Boss collider'ı büyüt (Tower detection için)
            CircleCollider2D col = GetComponent<CircleCollider2D>();
            if (col != null)
            {
                col.radius = 0.6f; // Boss daha büyük olduğu için radius artırıldı
            }

            Debug.Log($"<color=red>BossEnemy Initialized:</color> HP:{maxHealth} DMG:{damageToHero} SPD:{moveSpeed} Range:{meleReachOffset}");
        }

        public override void OnSpawn()
        {
            base.OnSpawn();

            // Boss spawn efekti
            if (spawnEffect != null)
            {
                Instantiate(spawnEffect, transform.position, Quaternion.identity);
            }

            // Health Bar Oluştur
            CreateHealthBar();

            // DEBUG: Tag ve Collider kontrolü
            Debug.Log($"<color=red>BOSS TAG CHECK: Tag={gameObject.tag}, Layer={LayerMask.LayerToName(gameObject.layer)}</color>");
            CircleCollider2D col = GetComponent<CircleCollider2D>();
            if (col != null)
            {
                Debug.Log($"<color=red>BOSS COLLIDER: Enabled={col.enabled}, IsTrigger={col.isTrigger}, Radius={col.radius}</color>");
            }
            else
            {
                Debug.LogError("<color=red>BOSS HAS NO COLLIDER!</color>");
            }

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

            // Health Bar Güncelle - healthComponent'ten güncel can değerini al
            if (healthBarScript != null && healthComponent != null)
            {
                healthBarScript.UpdateHealthBar(healthComponent.CurrentHealth, healthComponent.MaxHealth);
                Debug.Log($"<color=yellow>Boss Damaged!</color> Damage: {damageAmount}, Current HP: {healthComponent.CurrentHealth}/{healthComponent.MaxHealth}");
            }
        }

        public override void TakeDamageFromHero(int baseDamage)
        {
            base.TakeDamageFromHero(baseDamage);

            // Health Bar Güncelle
            if (healthBarScript != null && healthComponent != null)
            {
                healthBarScript.UpdateHealthBar(healthComponent.CurrentHealth, healthComponent.MaxHealth);
                Debug.Log($"<color=cyan>Boss Damaged by HERO!</color> Base: {baseDamage}, Actual: {damageFromHero}, Current HP: {healthComponent.CurrentHealth}/{healthComponent.MaxHealth}");
            }
        }

        public override void TakeDamageFromTower(int baseDamage)
        {
            base.TakeDamageFromTower(baseDamage);

            // Health Bar Güncelle
            if (healthBarScript != null && healthComponent != null)
            {
                healthBarScript.UpdateHealthBar(healthComponent.CurrentHealth, healthComponent.MaxHealth);
                Debug.Log($"<color=green>Boss Damaged by TOWER!</color> Base: {baseDamage}, Actual: {damageFromTower}, Current HP: {healthComponent.CurrentHealth}/{healthComponent.MaxHealth}");
            }
        }

        public override void OnDeath()
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

        public override void OnAttackPerformed()
        {
            base.OnAttackPerformed();
            // Boss saldırısı çok güçlü ve görsel efektli
            Debug.Log($"<color=red>BOSS ATTACK!</color> Dealing {damageToHero} damage!");
        }
    }
}

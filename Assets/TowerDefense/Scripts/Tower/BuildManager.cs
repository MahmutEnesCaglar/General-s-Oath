using UnityEngine;
using TowerDefense.Core;

namespace TowerDefense.Tower
{
    /// <summary>
    /// Kule inşaat sistemini yönetir
    /// Hangi kulenin inşa edileceğini seçer, UI ve klavye kısayollarını yönetir
    /// Hibrit sistem: Arkadaşın BuildManager + para kontrolü
    /// </summary>
    public class BuildManager : MonoBehaviour
    {
        // Singleton yapısı
        public static BuildManager Instance;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Sahnede birden fazla BuildManager var!");
                return;
            }
            Instance = this;
        }

        [Header("Kule Prefabları")]
        public GameObject cannonTowerPrefab;
        public GameObject archerTowerPrefab;
        public GameObject mortarTowerPrefab;

        [Header("Kule Maliyetleri")]
        public int cannonCost = 50;
        public int archerCost = 75;
        public int mortarCost = 100;

        // Şu an inşa etmek için seçili olan kule
        private GameObject towerToBuild;
        private int towerCost = 0;

        void Start()
        {
            // Oyun başlarken varsayılan olarak Cannon seçili olsun (Test için)
            SelectTower(cannonTowerPrefab, cannonCost);
            Debug.Log("[BuildManager] Başlangıç: Cannon Tower seçili");
        }

        /// <summary>
        /// Seçili kuleyi döndürür
        /// </summary>
        public GameObject GetTowerToBuild()
        {
            return towerToBuild;
        }

        /// <summary>
        /// Seçili kulenin maliyetini döndürür
        /// </summary>
        public int GetTowerCost()
        {
            return towerCost;
        }

        /// <summary>
        /// Kule seçer (UI butonları bu fonksiyonu çağıracak)
        /// </summary>
        public void SelectTower(GameObject tower, int cost)
        {
            towerToBuild = tower;
            towerCost = cost;
            Debug.Log($"[BuildManager] Seçili kule değişti: {tower.name}, Maliyet: {cost}");
        }

        /// <summary>
        /// Kule satın alınabilir mi kontrol eder
        /// </summary>
        public bool CanAffordTower()
        {
            if (GameManager.Instance == null)
                return true; // GameManager yoksa test için true döndür

            return GameManager.Instance.playerMoney >= towerCost;
        }

        /// <summary>
        /// Kuleyi satın alır (para düşer)
        /// </summary>
        public bool PurchaseTower()
        {
            if (!CanAffordTower())
            {
                Debug.LogWarning($"[BuildManager] Yetersiz para! Mevcut: {GameManager.Instance?.playerMoney}, Gerekli: {towerCost}");
                return false;
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.playerMoney -= towerCost;
                Debug.Log($"[BuildManager] Kule satın alındı! Kalan para: {GameManager.Instance.playerMoney}");
            }

            return true;
        }

        /// <summary>
        /// Klavye kısayolları (UI yapana kadar)
        /// 1: Cannon, 2: Archer, 3: Mortar
        /// </summary>
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SelectTower(cannonTowerPrefab, cannonCost);
                Debug.Log("[BuildManager] Cannon Tower seçildi (1)");
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SelectTower(archerTowerPrefab, archerCost);
                Debug.Log("[BuildManager] Archer Tower seçildi (2)");
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SelectTower(mortarTowerPrefab, mortarCost);
                Debug.Log("[BuildManager] Mortar Tower seçildi (3)");
            }
        }
    }
}

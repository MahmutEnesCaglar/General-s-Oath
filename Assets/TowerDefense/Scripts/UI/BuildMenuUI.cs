using UnityEngine;
using TMPro;
using UnityEngine.UI;
using TowerDefense.Tower;

namespace TowerDefense.UI
{
    /// <summary>
    /// Build Menu - BuildSpot'a tıklayınca gösterilir
    /// Tower prefablarından otomatik maliyet okur
    /// </summary>
    public class BuildMenuUI : MonoBehaviour
    {
        [Header("Tower Butonları")]
        public Button[] towerButtons; // 3 buton (Archer, Cannon, Mortar)
        public TextMeshProUGUI[] costTexts; // Her butonun altındaki maliyet
        
        [Header("Kontrol Butonları")]
        public Button closeButton; // Menüyü kapatma butonu
        
        private BuildManager buildManager;
        
        private void Start()
        {
            if (BuildManager.main != null)
            {
                Setup(BuildManager.main);
            }
        }

        public void Setup(BuildManager manager)
        {
            buildManager = manager;
            UpdateCostTexts();
            SetupButtons();
            
            // Kapat butonunu ayarla
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(Close);
            }
        }
        
        private void UpdateCostTexts()
        {
            if (buildManager == null || buildManager.towerPrefabs == null) return;
            
            for (int i = 0; i < buildManager.towerPrefabs.Length && i < costTexts.Length; i++)
            {
                if (costTexts[i] != null)
                {
                    int cost = 0;
                    
                    // 1. Öncelik: Prefab üzerindeki Tower scriptinden maliyeti oku
                    if (buildManager.towerPrefabs[i] != null)
                    {
                        TowerDefense.Tower.Tower tower = buildManager.towerPrefabs[i].GetComponent<TowerDefense.Tower.Tower>();
                        if (tower != null && tower.levels != null && tower.levels.Count > 0)
                        {
                            cost = tower.levels[0].cost;
                        }
                    }
                    
                    // 2. Öncelik: Eğer prefabdan okuyamadıysak BuildManager listesinden oku
                    if (cost == 0 && buildManager.towerCosts != null && i < buildManager.towerCosts.Length)
                    {
                        cost = buildManager.towerCosts[i];
                    }

                    costTexts[i].text = cost.ToString();
                }
            }
        }
        
        private void SetupButtons()
        {
            for (int i = 0; i < towerButtons.Length; i++)
            {
                int index = i; // Closure için
                if (towerButtons[i] != null)
                {
                    towerButtons[i].onClick.RemoveAllListeners();
                    towerButtons[i].onClick.AddListener(() => OnTowerButtonClicked(index));
                }
            }
        }
        
        private void OnTowerButtonClicked(int towerIndex)
        {
            if (buildManager != null)
            {
                buildManager.BuildTower(towerIndex);
            }
        }
        
        public void Close()
        {
            if (buildManager != null)
            {
                buildManager.CloseBuildMenu();
            }
        }
    }
}

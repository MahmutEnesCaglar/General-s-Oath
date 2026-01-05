using UnityEngine;
using TMPro;
using UnityEngine.UI;
using TowerDefense.Tower;
using System.Collections;

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
        private bool isInitialized = false; // Setup'ın yapılıp yapılmadığını takip et
        
        private void Start()
        {
            Debug.Log($"<color=cyan>[BuildMenuUI] Start çağrıldı. GameObject: {gameObject.name}</color>");
            Debug.Log($"<color=cyan>[BuildMenuUI] BuildManager.main: {(BuildManager.main != null ? BuildManager.main.gameObject.name : "NULL")}</color>");
            
            TrySetup();
        }
        
        private void OnEnable()
        {
            // Her aktif olduğunda Setup'ı kontrol et
            Debug.Log($"<color=orange>[BuildMenuUI] OnEnable çağrıldı. isInitialized: {isInitialized}</color>");
            
            // Eğer Setup yapılmamışsa veya BuildManager değiştiyse yeniden Setup yap
            if (!isInitialized || buildManager != BuildManager.main)
            {
                TrySetup();
            }
        }
        
        private void TrySetup()
        {
            if (BuildManager.main != null)
            {
                Setup(BuildManager.main);
                isInitialized = true;
            }
            else
            {
                Debug.LogWarning("<color=red>[BuildMenuUI] BuildManager.main NULL! Setup yapılamadı.</color>");
                // Biraz bekleyip tekrar dene
                StartCoroutine(DelayedSetup());
            }
        }
        
        private System.Collections.IEnumerator DelayedSetup()
        {
            yield return new WaitForSeconds(0.1f);
            Debug.Log($"<color=cyan>[BuildMenuUI] Delayed setup: BuildManager.main {(BuildManager.main != null ? "bulundu" : "hala NULL")}</color>");
            if (BuildManager.main != null)
            {
                Setup(BuildManager.main);
            }
        }

        public void Setup(BuildManager manager)
        {
            Debug.Log($"<color=green>[BuildMenuUI] Setup çağrıldı! Manager: {(manager != null ? manager.gameObject.name : "NULL")}</color>");
            buildManager = manager;
            UpdateCostTexts();
            SetupButtons();
            
            // Kapat butonunu ayarla
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(Close);
            }
            
            Debug.Log("<color=green>[BuildMenuUI] Setup tamamlandı!</color>");
        }
        
        private void UpdateCostTexts()
        {
            if (buildManager == null || buildManager.towerPrefabs == null) return;
            
            for (int i = 0; i < buildManager.towerPrefabs.Length && i < costTexts.Length; i++)
            {
                if (costTexts[i] != null)
                {
                    int cost = 0;
                    
                    // 1. Öncelik: Prefab üzerindeki Tower scriptinden buildCost'u oku
                    if (buildManager.towerPrefabs[i] != null)
                    {
                        TowerDefense.Tower.Tower tower = buildManager.towerPrefabs[i].GetComponent<TowerDefense.Tower.Tower>();
                        if (tower != null)
                        {
                            cost = tower.buildCost;
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

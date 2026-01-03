using UnityEngine;
using TMPro;
using UnityEngine.UI;
using TowerDefense.Tower;
using TowerDefense.Core;

namespace TowerDefense.UI
{
    public class UpgradeMenuUI : MonoBehaviour
    {
        [Header("UI Referansları")]
        public GameObject panelObj; // Panelin kendisi (Açıp kapatmak için)
        public Button upgradeButton;
        public Button sellButton;
        public Button closeButton;

        [Header("Text Alanları")]
        public TextMeshProUGUI upgradeCostText; // Yükseltme bedeli
        public TextMeshProUGUI sellValueText;   // Satış iade bedeli

        private TowerDefense.Tower.Tower selectedTower;

        void Start()
        {
            // Başlangıçta paneli gizle
            if (panelObj != null) panelObj.SetActive(false);

            // Buton eventlerini bağla
            if (upgradeButton != null) upgradeButton.onClick.AddListener(OnUpgradeClicked);
            if (sellButton != null) sellButton.onClick.AddListener(OnSellClicked);
            if (closeButton != null) closeButton.onClick.AddListener(CloseMenu);
        }

        void Update()
        {
            // YENİ: Menü açıksa ve bir kule seçiliyse pozisyonunu güncelle (Zoom/Pan yapınca kaymasın diye)
            if (panelObj != null && panelObj.activeSelf && selectedTower != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(selectedTower.transform.position);
                // Kulenin kendi belirlediği offset değerini kullan
                panelObj.transform.position = screenPos + new Vector3(0, selectedTower.uiYOffset, 0); 
            }
        }

        public void OpenMenu(TowerDefense.Tower.Tower tower)
        {
            // Eğer aynı kuleye tıklandıysa ve menü açıksa, kapat (Toggle)
            if (selectedTower == tower && panelObj != null && panelObj.activeSelf)
            {
                CloseMenu();
                return;
            }

            // Farklı bir kule seçildiyse, önce eskisinin menzilini kapat
            if (selectedTower != null && selectedTower != tower)
            {
                selectedTower.ToggleRangeVisual(false);
            }

            selectedTower = tower;
            if (selectedTower == null) return;

            // YENİ: Menzil çemberini göster
            selectedTower.ToggleRangeVisual(true);

            UpdateUI();
            
            if (panelObj != null) 
            {
                panelObj.SetActive(true);
                
                // Paneli kulenin olduğu yere (biraz yukarısına) taşı
                Vector3 screenPos = Camera.main.WorldToScreenPoint(selectedTower.transform.position);
                // Kulenin kendi belirlediği offset değerini kullan
                panelObj.transform.position = screenPos + new Vector3(0, selectedTower.uiYOffset, 0); 
                
                // Layout'u zorla güncelle (Bazen ilk açılışta görünmeme sorunu için)
                LayoutRebuilder.ForceRebuildLayoutImmediate(panelObj.GetComponent<RectTransform>());
            }
        }

        public void CloseMenu()
        {
            // YENİ: Menzil çemberini gizle
            if (selectedTower != null)
            {
                selectedTower.ToggleRangeVisual(false);
            }

            if (panelObj != null) panelObj.SetActive(false);
            selectedTower = null;
        }

        private void UpdateUI()
        {
            if (selectedTower == null) return;

            // --- SATIŞ DEĞERİ ---
            // Toplam harcanan paranın yarısı
            int sellValue = selectedTower.totalSpent / 2;
            if (sellValueText != null) sellValueText.text = sellValue.ToString();

            // --- YÜKSELTME DEĞERİ ---
            // Kulenin bir sonraki seviyesi var mı?
            int nextLevelIndex = selectedTower.currentLevel - 1; // currentLevel 1 ise, levels[0] level 2 verisidir.
            
            if (selectedTower.levels != null && nextLevelIndex < selectedTower.levels.Count)
            {
                int cost = selectedTower.levels[nextLevelIndex].cost;
                if (upgradeCostText != null) upgradeCostText.text = cost.ToString();
                
                if (upgradeButton != null) upgradeButton.interactable = true;
            }
            else
            {
                // Max level
                if (upgradeCostText != null) upgradeCostText.text = "MAX";
                if (upgradeButton != null) upgradeButton.interactable = false;
            }
        }

        private void OnUpgradeClicked()
        {
            if (selectedTower != null)
            {
                bool success = selectedTower.Upgrade();
                if (success)
                {
                    // Yükseltme başarılıysa menüyü kapat
                    CloseMenu();
                }
            }
        }

        private void OnSellClicked()
        {
            if (selectedTower != null)
            {
                selectedTower.Sell();
                CloseMenu();
            }
        }
    }
}

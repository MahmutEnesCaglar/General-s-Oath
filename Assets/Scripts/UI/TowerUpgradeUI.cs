using UnityEngine;
using TMPro; // TextMeshPro
using UnityEngine.UI;
using TowerDefense.Tower; // Tower scriptine erişim için

namespace TowerDefense.UI // <--- Namespace burası
{
    public class TowerUpgradeUI : MonoBehaviour
    {
        [Header("UI Elemanları")]
        public TextMeshProUGUI costText;
        public Button upgradeButton;
        
        private TowerDefense.Tower.Tower targetTower;

        // Bu fonksiyonu Tower.cs çağıracak
        public void Setup(TowerDefense.Tower.Tower tower, int cost)
        {
            targetTower = tower;
            
            if (costText != null)
                costText.text = cost.ToString(); 
            
            // Butona tıklayınca ne olacağını belirle
            if (upgradeButton != null)
            {
                upgradeButton.onClick.RemoveAllListeners(); // Eski eventleri temizle
                upgradeButton.onClick.AddListener(OnUpgradeClicked);
            }
        }

        void OnUpgradeClicked()
        {
            if (targetTower != null)
            {
                targetTower.Upgrade();
            }
        }
    }
}
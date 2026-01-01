using UnityEngine;
using UnityEngine.Tilemaps; // Artık buna ihtiyacımız pek yok ama hata vermesin diye kalsın
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TowerDefense.UI; // UpgradeMenuUI için

namespace TowerDefense.Tower 
{
    public class BuildManager : MonoBehaviour
    {
        public static BuildManager main; 

        private void Awake()
        {
            main = this;
        }

        [Header("Gerekli Bileşenler")]
        public GameObject buildMenuPanel; 
        public UpgradeMenuUI upgradeMenu; // YENİ: Upgrade menüsü referansı
        
        [Header("Kule Prefabları & Ayarlar")]
        public GameObject[] towerPrefabs;
        // UPDATED: Yeni ekonomi sistemine göre maliyetler
        // Tower 0: Archer (75), Tower 1: Cannon (60), Tower 2: Mortar (125)
        public int[] towerCosts = { 75, 60, 125 }; 
        
        // Seçilen kulenin kurulacağı tam pozisyon (Artık Grid koordinatı değil, Dünya pozisyonu)
        private Vector3 selectedBuildPosition;
        // Kule kurulduktan sonra o noktaya tekrar kurulmasın diye, tıklanan objeyi hafızada tutabiliriz
        private GameObject selectedSpotObj;

        void Start()
        {
            if(buildMenuPanel != null)
                buildMenuPanel.SetActive(false);
            
            if(upgradeMenu != null)
                upgradeMenu.CloseMenu();
        }

        void Update()
        {
            if (Mouse.current == null) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (Mouse.current.leftButton.wasPressedThisFrame) 
            {
                HandleClick();
            }
        }

        // HeroInput tarafından çağrılan fonksiyon
        public bool IsMouseOverBuildSpot()
        {
            if (Mouse.current == null) return false;

            // Mouse pozisyonunu al
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, Mathf.Abs(Camera.main.transform.position.z)));

            // 2D Raycast atıyoruz - Sadece "BuildSpot" tag'li objeleri arıyoruz
            // Vector2.zero, "tam bu noktada bir şey var mı" demektir.
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("BuildSpot"))
            {
                return true;
            }
            return false;
        }

        void HandleClick()
        {
            // Mouse pozisyonu
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, Mathf.Abs(Camera.main.transform.position.z)));

            // Tıklanan yerde ne var?
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null)
            {
                // 1. Durum: BuildSpot'a tıklandı
                if (hit.collider.CompareTag("BuildSpot"))
                {
                    // Upgrade menüsünü kapat
                    if (upgradeMenu != null) upgradeMenu.CloseMenu();

                    // Bulduk! Tıklanan objenin (Spot_1 vb.) tam merkezini alıyoruz
                    selectedSpotObj = hit.collider.gameObject;
                    selectedBuildPosition = hit.transform.position;
                    
                    // Menüyü aç
                    OpenBuildMenu(selectedBuildPosition);
                    return;
                }
                // 2. Durum: Kuleye tıklandı (Tag: "Tower" olmalı)
                else if (hit.collider.CompareTag("Tower"))
                {
                    // Build menüsünü kapat
                    CloseBuildMenu();

                    // Kule scriptini al
                    Tower towerScript = hit.collider.GetComponent<Tower>();
                    if (towerScript != null && upgradeMenu != null)
                    {
                        upgradeMenu.OpenMenu(towerScript);
                    }
                    return;
                }
            }
            
            // Boşluğa tıklandıysa hepsini kapat
            CloseBuildMenu();
            if (upgradeMenu != null) upgradeMenu.CloseMenu();
        }

        void OpenBuildMenu(Vector3 position)
        {
            if(buildMenuPanel != null)
            {
                buildMenuPanel.SetActive(true);
                
                // Menüyü, seçilen noktanın (Collider merkezinin) biraz üzerine koy
                Vector3 screenPos = Camera.main.WorldToScreenPoint(position);
                buildMenuPanel.transform.position = screenPos + new Vector3(0, 150, 0); 
            }
        }

        public void CloseBuildMenu()
        {
            if(buildMenuPanel != null)
                buildMenuPanel.SetActive(false);
        }

        public void BuildTower(int towerIndex)
        {
            // Index kontrolü
            if (towerIndex < 0 || towerIndex >= towerPrefabs.Length)
            {
                Debug.LogError($"[BuildManager] Geçersiz tower index: {towerIndex}");
                return;
            }

            // Kule maliyetini al (Önce prefabdan, yoksa listeden)
            int cost = 0;
            if (towerPrefabs[towerIndex] != null)
            {
                Tower tower = towerPrefabs[towerIndex].GetComponent<Tower>();
                if (tower != null && tower.levels != null && tower.levels.Count > 0)
                {
                    cost = tower.levels[0].cost;
                }
            }
            
            // Eğer prefabdan alınamadıysa manuel listeden al
            if (cost == 0 && towerIndex < towerCosts.Length)
            {
                cost = towerCosts[towerIndex];
            }

            // Para kontrolü - MoneyManager üzerinden
            if (MoneyManager.Instance != null)
            {
                if (MoneyManager.Instance.SpendMoney(cost))
                {
                    // Para başarıyla harcandı, kuleyi inşa et
                    Vector3 spawnPos = selectedBuildPosition;
                    spawnPos.z = 0;

                    GameObject towerObj = Instantiate(towerPrefabs[towerIndex], spawnPos, Quaternion.identity);
                    
                    // Kuleye başlangıç maliyetini bildir
                    Tower towerScript = towerObj.GetComponent<Tower>();
                    if (towerScript != null)
                    {
                        towerScript.Initialize(cost);
                        // Kuleye hangi spot üzerinde olduğunu bildir (Satılınca geri açmak için)
                        towerScript.occupiedSpot = selectedSpotObj;
                    }

                    Debug.Log($"<color=cyan>[BuildManager] Kule inşa edildi! Tip: {towerIndex}, Maliyet: {cost}</color>");

                    // Opsiyonel: Kule kurulduktan sonra o "BuildSpot"u kapatabiliriz
                    if (selectedSpotObj != null)
                    {
                        selectedSpotObj.GetComponent<Collider2D>().enabled = false;
                    }

                    CloseBuildMenu();
                }
                else
                {
                    // Yetersiz para
                    Debug.Log($"<color=yellow>[BuildManager] Yetersiz para! İhtiyaç: {cost}, Mevcut: {MoneyManager.Instance.currentMoney}</color>");
                    // İsteğe bağlı: UI'da bir mesaj gösterebilirsiniz
                }
            }
            else
            {
                Debug.LogWarning("<color=red>[BuildManager] MoneyManager bulunamadı! Kule bedava inşa ediliyor.</color>");

                // Fallback: MoneyManager yoksa yine de kuleyi inşa et
                Vector3 spawnPos = selectedBuildPosition;
                spawnPos.z = 0;
                Instantiate(towerPrefabs[towerIndex], spawnPos, Quaternion.identity);

                if (selectedSpotObj != null)
                {
                    selectedSpotObj.GetComponent<Collider2D>().enabled = false;
                }

                CloseBuildMenu();
            }
        }
    }
}
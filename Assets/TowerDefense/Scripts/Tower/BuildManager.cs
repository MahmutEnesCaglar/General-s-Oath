using UnityEngine;
using UnityEngine.Tilemaps; // Artık buna ihtiyacımız pek yok ama hata vermesin diye kalsın
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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
        
        [Header("Kule Prefabları & Ayarlar")]
        public GameObject[] towerPrefabs; 
        public int[] towerCosts = { 100, 50, 40 }; 
        
        // Seçilen kulenin kurulacağı tam pozisyon (Artık Grid koordinatı değil, Dünya pozisyonu)
        private Vector3 selectedBuildPosition;
        // Kule kurulduktan sonra o noktaya tekrar kurulmasın diye, tıklanan objeyi hafızada tutabiliriz
        private GameObject selectedSpotObj;

        void Start()
        {
            if(buildMenuPanel != null)
                buildMenuPanel.SetActive(false);
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

            if (hit.collider != null && hit.collider.CompareTag("BuildSpot"))
            {
                // Bulduk! Tıklanan objenin (Spot_1 vb.) tam merkezini alıyoruz
                selectedSpotObj = hit.collider.gameObject;
                selectedBuildPosition = hit.transform.position;
                
                // Menüyü aç
                OpenBuildMenu(selectedBuildPosition);
            }
            else
            {
                CloseBuildMenu();
            }
        }

        void OpenBuildMenu(Vector3 position)
        {
            if(buildMenuPanel != null)
            {
                buildMenuPanel.SetActive(true);
                
                // Menüyü, seçilen noktanın (Collider merkezinin) biraz üzerine koy
                Vector3 screenPos = Camera.main.WorldToScreenPoint(position);
                buildMenuPanel.transform.position = screenPos + new Vector3(-25, 150, 0); 
            }
        }

        public void CloseBuildMenu()
        {
            if(buildMenuPanel != null)
                buildMenuPanel.SetActive(false);
        }

        public void BuildTower(int towerIndex)
        {
            // Seçilen "Kutu"nun tam merkezine kuleyi koy
            Vector3 spawnPos = selectedBuildPosition;
            spawnPos.z = 0; // Derinlik ayarı
            
            Instantiate(towerPrefabs[towerIndex], spawnPos, Quaternion.identity);
            
            // Opsiyonel: Kule kurulduktan sonra o "BuildSpot"u kapatabiliriz ki üstüne bir daha kurulmasın
            // if (selectedSpotObj != null) selectedSpotObj.GetComponent<Collider2D>().enabled = false;

            CloseBuildMenu();
        }
    }
}
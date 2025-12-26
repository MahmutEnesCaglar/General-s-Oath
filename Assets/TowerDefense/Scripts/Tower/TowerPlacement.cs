using UnityEngine;
using TowerDefense.Core;

namespace TowerDefense.Tower
{
    /// <summary>
    /// Kule yerleştirme sistemini yönetir
    /// Mouse ile tıklayarak kule yerleştirmeyi sağlar
    /// </summary>
    public class TowerPlacement : MonoBehaviour
    {
        [Header("Kule Prefab'ları")]
        public GameObject groundTowerPrefab;
        public GameObject universalTowerPrefab;
        public GameObject aoeTowerPrefab;

        [Header("Yerleştirme Ayarları")]
        private string selectedTowerType = "";
        private GameObject towerPreview;        // Ghost/preview kule
        private bool isPlacementMode = false;

        [Header("Yerleştirme Kuralları")]
        public LayerMask placementLayer;        // Hangi layer'a yerleştirilebilir
        public float gridSize = 1f;             // Grid snap boyutu
        public bool useGridSnap = true;         // Grid'e yapış

        [Header("Görsel")]
        public Color validPlacementColor = new Color(0, 1, 0, 0.5f);    // Yeşil
        public Color invalidPlacementColor = new Color(1, 0, 0, 0.5f);  // Kırmızı

        private void Update()
        {
            // Check if hero input mode is active (hybrid system)
            if (TowerDefense.Hero.HeroInput.Instance != null &&
                TowerDefense.Hero.HeroInput.Instance.currentMode == TowerDefense.Hero.HeroInput.InputMode.HeroControl)
            {
                return; // Don't process tower placement input when in hero mode
            }

            if (!isPlacementMode) return;

            // Mouse pozisyonunu al
            Vector3 mouseWorldPos = GetMouseWorldPosition();

            // Grid snap
            if (useGridSnap)
            {
                mouseWorldPos = SnapToGrid(mouseWorldPos);
            }

            // Preview'i güncelle
            if (towerPreview != null)
            {
                towerPreview.transform.position = mouseWorldPos;

                // Yerleştirme geçerliliğini kontrol et
                bool canPlace = CanPlaceTower(mouseWorldPos);
                UpdatePreviewColor(canPlace);

                // Mouse tıklaması
                if (Input.GetMouseButtonDown(0)) // Sol tık
                {
                    if (canPlace)
                    {
                        PlaceTower(mouseWorldPos);
                    }
                }

                // İptal (sağ tık veya ESC)
                if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
                {
                    CancelPlacement();
                }
            }
        }

        /// <summary>
        /// Kule seçer ve yerleştirme modunu aktif eder
        /// </summary>
        public void SelectTower(string towerType)
        {
            selectedTowerType = towerType;

            // Eski preview'i temizle
            if (towerPreview != null)
            {
                Destroy(towerPreview);
            }

            // Yeni preview oluştur
            GameObject prefab = GetTowerPrefab(towerType);
            if (prefab != null)
            {
                towerPreview = Instantiate(prefab);
                towerPreview.name = "TowerPreview";

                // Tower script'ini devre dışı bırak (preview'de çalışmasın)
                Tower towerScript = towerPreview.GetComponent<Tower>();
                if (towerScript != null)
                    towerScript.enabled = false;

                // Collider'ı kaldır (preview çarpışmasın)
                Collider2D col = towerPreview.GetComponent<Collider2D>();
                if (col != null)
                    Destroy(col);

                isPlacementMode = true;

                Debug.Log($"{towerType} kulesi yerleştirme modu aktif. Sol tık: yerleştir, Sağ tık: iptal");
            }
        }

        /// <summary>
        /// Kuleyi yerleştirir
        /// </summary>
        private void PlaceTower(Vector3 position)
        {
            // Para kontrolü
            if (Core.GameManager.Instance != null)
            {
                bool purchased = Core.GameManager.Instance.PurchaseTower(selectedTowerType);

                if (!purchased)
                {
                    Debug.Log("Yetersiz para!");
                    return;
                }
            }

            // Kuleyi yerleştir
            GameObject prefab = GetTowerPrefab(selectedTowerType);
            GameObject tower = Instantiate(prefab, position, Quaternion.identity);
            tower.name = $"{selectedTowerType}_Tower";

            // Tower script'i ayarla
            Tower towerScript = tower.GetComponent<Tower>();
            if (towerScript != null)
            {
                towerScript.LoadStatsForLevel(1);
            }

            Debug.Log($"{selectedTowerType} kulesi yerleştirildi: {position}");

            // Yerleştirme modundan çık
            CancelPlacement();
        }

        /// <summary>
        /// Yerleştirme modunu iptal eder
        /// </summary>
        public void CancelPlacement()
        {
            if (towerPreview != null)
            {
                Destroy(towerPreview);
            }

            isPlacementMode = false;
            selectedTowerType = "";

            Debug.Log("Yerleştirme modu iptal edildi");
        }

        /// <summary>
        /// Belirtilen pozisyona kule yerleştirilebilir mi?
        /// </summary>
        private bool CanPlaceTower(Vector3 position)
        {
            // Burada çakışma kontrolü yapabilirsiniz
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.5f);

            foreach (var col in colliders)
            {
                // Başka bir kule var mı?
                if (col.GetComponent<Tower>() != null)
                    return false;

                // Düşman var mı?
                if (col.GetComponent<Enemy.Enemy>() != null)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Preview rengini günceller
        /// </summary>
        private void UpdatePreviewColor(bool canPlace)
        {
            if (towerPreview == null) return;

            SpriteRenderer renderer = towerPreview.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = canPlace ? validPlacementColor : invalidPlacementColor;
            }
        }

        /// <summary>
        /// Mouse'un dünya pozisyonunu alır
        /// </summary>
        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f; // Kamera mesafesi
            return Camera.main.ScreenToWorldPoint(mousePos);
        }

        /// <summary>
        /// Pozisyonu grid'e yapıştırır
        /// </summary>
        private Vector3 SnapToGrid(Vector3 position)
        {
            float x = Mathf.Round(position.x / gridSize) * gridSize;
            float y = Mathf.Round(position.y / gridSize) * gridSize;
            return new Vector3(x, y, position.z);
        }

        /// <summary>
        /// Kule prefab'ını döndürür
        /// </summary>
        private GameObject GetTowerPrefab(string towerType)
        {
            switch (towerType.ToLower())
            {
                case "ground":
                    return groundTowerPrefab;
                case "universal":
                    return universalTowerPrefab;
                case "aoe":
                    return aoeTowerPrefab;
                default:
                    Debug.LogError($"Bilinmeyen kule tipi: {towerType}");
                    return null;
            }
        }

        /// <summary>
        /// Yerleştirme modu aktif mi?
        /// </summary>
        public bool IsPlacementModeActive()
        {
            return isPlacementMode;
        }
    }
}

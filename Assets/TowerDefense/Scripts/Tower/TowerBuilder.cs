using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace TowerDefense.Tower
{
    /// <summary>
    /// Kule yerleştirme sistemi - Tilemap tabanlı
    /// Arkadaşın sistemi + preview ve para kontrolü eklendi
    /// </summary>
    public class TowerBuilder : MonoBehaviour
    {
        [Header("Ayarlar")]
        public Tilemap buildTilemap; // Inspector'dan 'BuildMap'i buraya sürükle
        public BuildManager buildManager; // Inspector'dan 'BuildManager'ı buraya sürükle

        [Header("Preview Sistemi")]
        public bool showPreview = true;
        public Color validPlacementColor = new Color(0, 1, 0, 0.5f);   // Yeşil
        public Color invalidPlacementColor = new Color(1, 0, 0, 0.5f); // Kırmızı
        public Color cannotAffordColor = new Color(1, 1, 0, 0.5f);     // Sarı

        // Hangi karede kule var, kaydını tutmak için sözlük (Dictionary)
        // Anahtar: Koordinat (Vector3Int), Değer: Kule Objesi
        private Dictionary<Vector3Int, GameObject> occupiedTiles = new Dictionary<Vector3Int, GameObject>();

        // Preview sistemi
        private GameObject previewTower;
        private SpriteRenderer previewRenderer;
        private Vector3Int lastCellPos;

        void Update()
        {
            // Preview güncelle
            if (showPreview)
            {
                UpdatePreview();
            }

            // Sol Tıklandığında kule yerleştir
            if (Input.GetMouseButtonDown(0))
            {
                PlaceTower();
            }

            // Sağ tık veya ESC - Preview'i temizle
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                ClearPreview();
            }
        }

        /// <summary>
        /// Preview tower'ı günceller (mouse'u takip eder)
        /// </summary>
        void UpdatePreview()
        {
            if (buildTilemap == null || buildManager == null) return;

            // Mouse pozisyonu al
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;

            // Grid cell pozisyonu al
            Vector3Int cellPos = buildTilemap.WorldToCell(mouseWorldPos);

            // Hücre değişti mi?
            if (cellPos != lastCellPos)
            {
                lastCellPos = cellPos;

                // Preview'i güncelle
                CreateOrUpdatePreview(cellPos);
            }
        }

        /// <summary>
        /// Preview tower oluşturur veya günceller
        /// </summary>
        void CreateOrUpdatePreview(Vector3Int cellPos)
        {
            // Seçili kule var mı?
            GameObject towerPrefab = buildManager.GetTowerToBuild();
            if (towerPrefab == null) return;

            // Preview yok ise oluştur
            if (previewTower == null)
            {
                previewTower = Instantiate(towerPrefab);
                previewTower.name = "PreviewTower";

                // Tower script'ini devre dışı bırak
                Tower towerScript = previewTower.GetComponent<Tower>();
                if (towerScript != null)
                    towerScript.enabled = false;

                // Collider'ları devre dışı bırak
                Collider2D[] colliders = previewTower.GetComponents<Collider2D>();
                foreach (var col in colliders)
                    col.enabled = false;

                // SpriteRenderer al
                previewRenderer = previewTower.GetComponent<SpriteRenderer>();
            }

            // Preview pozisyonunu güncelle
            Vector3 previewPos = buildTilemap.GetCellCenterWorld(cellPos);
            previewTower.transform.position = previewPos;

            // Renk feedback
            UpdatePreviewColor(cellPos);
        }

        /// <summary>
        /// Preview rengini günceller (yeşil/kırmızı/sarı)
        /// </summary>
        void UpdatePreviewColor(Vector3Int cellPos)
        {
            if (previewRenderer == null) return;

            // Yerleştirme kontrolü
            PlacementResult result = CanPlaceTowerAt(cellPos);

            switch (result)
            {
                case PlacementResult.Valid:
                    previewRenderer.color = validPlacementColor;
                    break;
                case PlacementResult.CannotAfford:
                    previewRenderer.color = cannotAffordColor;
                    break;
                default:
                    previewRenderer.color = invalidPlacementColor;
                    break;
            }
        }

        /// <summary>
        /// Kule yerleştirir
        /// </summary>
        void PlaceTower()
        {
            if (buildTilemap == null || buildManager == null) return;

            // 1. Mouse'un dünya pozisyonunu al
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;

            // 2. Grid üzerindeki hücreyi (Koordinatını) bul
            Vector3Int cellPos = buildTilemap.WorldToCell(mouseWorldPos);

            // 3. Yerleştirme kontrolü
            PlacementResult result = CanPlaceTowerAt(cellPos);

            if (result == PlacementResult.Valid)
            {
                // 4. Para kontrolü ve ödeme
                if (!buildManager.PurchaseTower())
                {
                    Debug.LogWarning("[TowerBuilder] Yetersiz para!");
                    return;
                }

                // 5. Kuleyi inşa et
                GameObject towerToBuild = buildManager.GetTowerToBuild();
                Vector3 buildPos = buildTilemap.GetCellCenterWorld(cellPos);

                GameObject newTower = Instantiate(towerToBuild, buildPos, Quaternion.identity);
                newTower.name = $"{towerToBuild.name}_({cellPos.x},{cellPos.y})";

                // 6. Kayıt defterine işle
                occupiedTiles.Add(cellPos, newTower);

                Debug.Log($"[TowerBuilder] Kule inşa edildi: {cellPos}, Kalan para: {Core.GameManager.Instance?.money}");
            }
            else
            {
                // Hata mesajları
                switch (result)
                {
                    case PlacementResult.NoTile:
                        Debug.Log("[TowerBuilder] Buraya inşaat yapılamaz! (BuildMap boyanmamış)");
                        break;
                    case PlacementResult.Occupied:
                        Debug.Log("[TowerBuilder] Burada zaten bir kule var!");
                        break;
                    case PlacementResult.CannotAfford:
                        Debug.LogWarning("[TowerBuilder] Yetersiz para!");
                        break;
                }
            }
        }

        /// <summary>
        /// Belirtilen hücreye kule yerleştirilebilir mi kontrol eder
        /// </summary>
        PlacementResult CanPlaceTowerAt(Vector3Int cellPos)
        {
            // 1. BuildMap'te tile var mı?
            if (!buildTilemap.HasTile(cellPos))
            {
                return PlacementResult.NoTile;
            }

            // 2. Orada zaten kule var mı?
            if (occupiedTiles.ContainsKey(cellPos))
            {
                return PlacementResult.Occupied;
            }

            // 3. Para yeterli mi?
            if (!buildManager.CanAffordTower())
            {
                return PlacementResult.CannotAfford;
            }

            return PlacementResult.Valid;
        }

        /// <summary>
        /// Preview'i temizler
        /// </summary>
        void ClearPreview()
        {
            if (previewTower != null)
            {
                Destroy(previewTower);
                previewTower = null;
                previewRenderer = null;
            }
        }

        /// <summary>
        /// Belirtilen pozisyondaki kuleyi siler (upgrade/satış için)
        /// </summary>
        public void RemoveTowerAt(Vector3Int cellPos)
        {
            if (occupiedTiles.ContainsKey(cellPos))
            {
                GameObject tower = occupiedTiles[cellPos];
                occupiedTiles.Remove(cellPos);
                Destroy(tower);
                Debug.Log($"[TowerBuilder] Kule silindi: {cellPos}");
            }
        }

        /// <summary>
        /// Belirtilen pozisyondaki kuleyi döndürür
        /// </summary>
        public GameObject GetTowerAt(Vector3Int cellPos)
        {
            if (occupiedTiles.ContainsKey(cellPos))
            {
                return occupiedTiles[cellPos];
            }
            return null;
        }

        void OnDestroy()
        {
            ClearPreview();
        }
    }

    /// <summary>
    /// Yerleştirme sonuç durumları
    /// </summary>
    public enum PlacementResult
    {
        Valid,          // Yerleştirilebilir
        NoTile,         // BuildMap'te tile yok
        Occupied,       // Zaten kule var
        CannotAfford    // Para yetmiyor
    }
}

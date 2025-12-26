using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TowerBuilder : MonoBehaviour
{
    [Header("Ayarlar")]
    public Tilemap buildTilemap; // Inspector'dan 'BuildMap'i buraya sürükle
    public BuildManager buildManager; // Inspector'dan 'GameMaster'ı buraya sürükle

    // Hangi karede kule var, kaydını tutmak için sözlük (Dictionary)
    // Anahtar: Koordinat (Vector3Int), Değer: Kule Objesi
    private Dictionary<Vector3Int, GameObject> occupiedTiles = new Dictionary<Vector3Int, GameObject>();

    void Update()
    {
        // Sol Tıklandığında
        if (Input.GetMouseButtonDown(0))
        {
            PlaceTower();
        }
    }

    void PlaceTower()
    {
        // 1. Mouse'un dünya pozisyonunu al
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        // 2. Bu pozisyonun Grid üzerindeki hücresini (Koordinatını) bul
        Vector3Int cellPos = buildTilemap.WorldToCell(mouseWorldPos);

        // 3. Kontrol ET: Tıklanan yerde 'BuildMap' tile'ı var mı?
        if (!buildTilemap.HasTile(cellPos))
        {
            Debug.Log("Buraya inşaat yapılamaz! (BuildMap boyanmamış)");
            return;
        }

        // 4. Kontrol ET: Orada zaten kule var mı?
        if (occupiedTiles.ContainsKey(cellPos))
        {
            Debug.Log("Burada zaten bir kule var!");
            return;
        }

        // 5. İnşa Et
        GameObject towerToBuild = buildManager.GetTowerToBuild(); // Seçili kuleyi al
        
        if (towerToBuild != null)
        {
            // Kuleyi tam karenin ortasına hizalı koymak için 'GetCellCenterWorld' kullanıyoruz
            Vector3 buildPos = buildTilemap.GetCellCenterWorld(cellPos);
            
            GameObject newTower = Instantiate(towerToBuild, buildPos, Quaternion.identity);

            // 6. Kayıt Defterine İşle
            occupiedTiles.Add(cellPos, newTower);
            
            Debug.Log("Kule inşa edildi: " + cellPos);
        }
    }
}
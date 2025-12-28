using UnityEngine;
using System.Collections.Generic;

namespace TowerDefense.Environment
{
    public class WallSpawner : MonoBehaviour
    {
        [Header("Ayarlar")]
        public GameObject barrierPrefab; // Az önce yaptığın Prefab'ı buraya at
        public Transform barrierParent;  // Hiyerarşi kirlenmesin diye boş bir obje açıp buraya atabilirsin

        [Header("Pozisyonlar")]
        // Bu listeyi elle doldurabilirsin veya başka bir scriptten (MapGenerator) doldurtabilirsin
        public List<Transform> wallSpawnPoints; 

        void Start()
        {
            SpawnWalls();
        }

        void SpawnWalls()
        {
            if (barrierPrefab == null || wallSpawnPoints == null)
            {
                Debug.LogError("Prefab veya Spawn Point listesi eksik!");
                return;
            }

            foreach (Transform spawnPoint in wallSpawnPoints)
            {
                // 1. Duvarı oluştur
                GameObject newWall = Instantiate(barrierPrefab, spawnPoint.position, Quaternion.identity);
                
                // Hiyerarşide düzenli dursun
                if(barrierParent != null) newWall.transform.SetParent(barrierParent);

                // 2. BarrierSpriteController'a eriş
                BarrierSpriteController controller = newWall.GetComponent<BarrierSpriteController>();

                if (controller != null)
                {
                    // BURASI ÖNEMLİ: Hangi şeklin (0-7) geleceğine karar verme kısmı.
                    // Şimdilik hepsini '0' yapıyoruz. 
                    // Eğer harita mantığın varsa (komşular dolu mu?), buraya bir hesaplama ekleyeceğiz.
                    int secilecekIndex = CalculateWallIndex(spawnPoint); 
                    controller.SetBarrierDirection(secilecekIndex);
                }
            }
        }

        // İLERİ DÜZEY: Duvarın şekline karar verme fonksiyonu
        int CalculateWallIndex(Transform currentPoint)
        {
            // Eğer komşu kontrolü (Auto Tiling) yapmıyorsak
            // Rastgele veya sabit bir değer dönebilirsin.
            // Örneğin: return Random.Range(0, 8); // Rastgele duvar şekli dener
            
            return 0; // Şimdilik varsayılan (0. indexteki sprite)
        }
    }
}
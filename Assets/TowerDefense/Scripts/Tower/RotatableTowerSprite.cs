using UnityEngine;
using System.Collections.Generic;
using TowerDefense.Tower; 

namespace TowerDefense.Tower
{
    [System.Serializable]
    public struct DirectionalData
    {
        public string directionName;    
        public Sprite sprite;           
        public Vector3 scale; 
        public Vector2 firePointOffset; // Mermi çıkış noktası
    }

    public class RotatableTowerSprite : MonoBehaviour
    {
        [Header("Yön Verileri (Sırayla Doldur!)")]
        // 0:Doğu, 1:KD, 2:Kuzey, 3:KB, 4:Batı, 5:GB, 6:Güney, 7:GD
        public List<DirectionalData> directionDataList = new List<DirectionalData>();

        private SpriteRenderer spriteRenderer;
        private Tower towerComponent; 

        // Tower.cs'nin okuyabilmesi için PUBLIC yapıldı
        [HideInInspector] public int currentSegmentIndex;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            towerComponent = GetComponent<Tower>(); 
        }

        void Update()
        {
            // Eğer Tower scripti varsa ve bir hedefi kilitlediyse
            if (towerComponent != null && towerComponent.currentTarget != null)
            {
                RotateTowards(towerComponent.currentTarget.transform.position);
            }
        }

        public void RotateTowards(Vector3 targetPosition)
        {
            if (directionDataList == null || directionDataList.Count == 0 || spriteRenderer == null)
                return;

            Vector3 direction = targetPosition - transform.position;
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            // 8 yönlü hesaplama (45 derecelik dilimler)
            float step = 360f / 8f; 
            int index = Mathf.FloorToInt((angle + (step / 2)) / step) % 8;

            currentSegmentIndex = index;

            // Listeden doğru sprite'ı al ve uygula
            if (currentSegmentIndex < directionDataList.Count)
            {
                DirectionalData currentData = directionDataList[currentSegmentIndex];
                
                if (currentData.sprite != null)
                    spriteRenderer.sprite = currentData.sprite;
                
                // Scale (örn: sola bakarken aynalamak için)
                // Eğer listede scale (0,0,0) ise default (1,1,1) kullan
                if(currentData.scale == Vector3.zero) 
                    transform.localScale = Vector3.one;
                else
                    transform.localScale = currentData.scale;
            }
        }

        /// <summary>
        /// DÜZELTİLEN KISIM: Parametre geri eklendi.
        /// Tower.cs içindeki çağrı ile uyumlu hale getirildi.
        /// </summary>
        public Vector2 GetCurrentFirePointOffset(int index)
        {
            if (directionDataList != null && index >= 0 && index < directionDataList.Count)
            {
                return directionDataList[index].firePointOffset;
            }

            return Vector2.zero;
        }
    }
}
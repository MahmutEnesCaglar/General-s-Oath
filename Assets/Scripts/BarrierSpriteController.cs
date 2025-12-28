using UnityEngine;
using System.Collections.Generic;

namespace TowerDefense.Environment 
{
    [System.Serializable]
    public struct BarrierDirectionData
    {
        public string directionName;    // Örn: "Kuzey", "GuneyBati"
        public Sprite sprite;           // O yöne ait resim
        public Vector2 colliderSize;    // O resim için Box Collider Size
        public Vector2 colliderOffset;  // O resim için Box Collider Offset
    }

    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class BarrierSpriteController : MonoBehaviour
    {
        [Header("Duvar Yön Verileri (8 Yön)")]
        // Inspector'dan 8 eleman oluşturup dolduracağız
        public List<BarrierDirectionData> barrierDataList = new List<BarrierDirectionData>();

        [Header("Test / Başlangıç Ayarı")]
        [Range(0, 7)] 
        public int currentDirectionIndex = 0; // Inspector'dan test etmek için slider

        private SpriteRenderer spriteRenderer;
        private BoxCollider2D boxCollider;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            boxCollider = GetComponent<BoxCollider2D>();
        }

        void Start()
        {
            // Oyun başladığında seçili olan yönü uygula
            SetBarrierDirection(currentDirectionIndex);
        }

        // Inspector'da değeri değiştirdiğinde anlık görmek için:
        void OnValidate()
        {
            if(spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if(boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();
            
            SetBarrierDirection(currentDirectionIndex);
        }

        /// <summary>
        /// Duvarın yönünü (resmini ve collider'ını) index'e göre değiştirir.
        /// 0:Doğu, 1:KD, 2:Kuzey... (Senin sırlamana göre)
        /// </summary>
        public void SetBarrierDirection(int index)
        {
            if (barrierDataList == null || barrierDataList.Count == 0) return;

            // Index güvenliği (Liste dışına çıkmayı engelle)
            if (index < 0 || index >= barrierDataList.Count) return;

            currentDirectionIndex = index;
            BarrierDirectionData data = barrierDataList[index];

            // 1. Resmi Güncelle
            if (data.sprite != null)
            {
                spriteRenderer.sprite = data.sprite;
            }

            // 2. Collider'ı Güncelle (En önemli kısım burası)
            if (boxCollider != null)
            {
                boxCollider.size = data.colliderSize;
                boxCollider.offset = data.colliderOffset;
            }
        }
    }
}
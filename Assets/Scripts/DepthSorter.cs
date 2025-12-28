using UnityEngine;

namespace TowerDefense.Core
{
    [ExecuteInEditMode] // Editörde de çalışsın ki sahnede düzenlerken görebil
    [RequireComponent(typeof(SpriteRenderer))]
    public class DepthSorter : MonoBehaviour
    {
        [Header("Ayarlar")]
        public int sortingOrderBase = 5000; // Başlangıç değeri (yüksek tutalım)
        public float offset = 0f; // Objenin "ayaklarının" bastığı yeri ince ayar yapmak için
        public bool runOnlyOnce = false; // Kuleler/Bariyerler için TRUE (hareket etmiyorlar), Düşmanlar için FALSE

        private SpriteRenderer spriteRenderer;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void LateUpdate()
        {
            if (spriteRenderer == null) return;

            // FORMÜL: Order = Base - (Y Pozisyonu * 100)
            // Y ne kadar azalırsa (aşağı inerse), Order o kadar artar (öne gelir).
            spriteRenderer.sortingOrder = sortingOrderBase - (int)((transform.position.y + offset) * 100);

            if (runOnlyOnce && Application.isPlaying)
            {
                Destroy(this); // Statik objeler bir kere hesaplasın, script kendini silsin (Performans)
            }
        }
    }
}
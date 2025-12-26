using UnityEngine;
using System.Collections.Generic;

namespace TowerDefense.Tower
{
    /// <summary>
    /// Yönlü sprite sistemi - 8 yönlü kule sprite'ları
    /// Arkadaşın sistemi - namespace eklendi
    /// </summary>
    [System.Serializable]
    public struct DirectionalData
    {
        public string directionName;
        public Sprite sprite;
        public Vector3 scale;
        public Vector2 firePointOffset;
    }

    /// <summary>
    /// Kulenin hedefe doğru dönmesini ve sprite'ını değiştirmesini sağlar
    /// 8 yönlü sprite sistemi (0°, 45°, 90°, 135°, 180°, 225°, 270°, 315°)
    /// </summary>
    public class RotatableTowerSprite : MonoBehaviour
    {
        [Header("Yön Verileri")]
        public List<DirectionalData> directionDataList = new List<DirectionalData>();

        private SpriteRenderer spriteRenderer;

        // Tower.cs'nin hangi yöne bakıldığını bilmesi için
        [HideInInspector] public int currentSegmentIndex;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Kulenin sprite'ını hedef pozisyona göre döndürür
        /// </summary>
        public void RotateTowards(Vector3 targetPosition)
        {
            if (directionDataList == null || directionDataList.Count == 0 || spriteRenderer == null)
                return;

            // Hedef açıyı hesapla
            Vector3 direction = targetPosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            // 8 yönden en yakınını seç (her 45 derece bir segment)
            currentSegmentIndex = Mathf.RoundToInt(angle / 45f) % 8;

            // Sprite ve scale değiştir
            DirectionalData currentData = directionDataList[currentSegmentIndex];
            spriteRenderer.sprite = currentData.sprite;
            transform.localScale = currentData.scale;
        }

        /// <summary>
        /// Mevcut yönün fire point offset'ini döndürür
        /// (Mermi spawn pozisyonu için)
        /// </summary>
        public Vector2 GetCurrentFirePointOffset(int index)
        {
            if (index >= 0 && index < directionDataList.Count)
                return directionDataList[index].firePointOffset;

            return Vector2.zero;
        }
    }
}

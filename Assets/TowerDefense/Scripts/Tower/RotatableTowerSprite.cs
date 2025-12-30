using UnityEngine;
using System.Collections.Generic;

namespace TowerDefense.Tower
{
    [System.Serializable]
    public struct DirectionalData
    {
        public string directionName;    
        public Sprite sprite;           
        public Vector3 scale; 
        public Vector2 firePointOffset; 
    }

    [RequireComponent(typeof(SpriteRenderer))]
    public class RotatableTowerSprite : MonoBehaviour
    {
        [Header("Aktif Yön Verileri")]
        // Bu liste oyun içinde sürekli değişecek (Level 1, Level 2 verisi buraya kopyalanacak)
        public List<DirectionalData> currentDirectionalData = new List<DirectionalData>();

        private SpriteRenderer spriteRenderer;
        private Tower towerComponent; 
        
        [HideInInspector] public int currentSegmentIndex;
        private Vector3 lastTargetPosition; 

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            towerComponent = GetComponentInParent<Tower>(); // DİKKAT: Tower parent'ta olduğu için InParent dedik
        }

        void Start()
        {
            lastTargetPosition = transform.position + Vector3.right;
        }

        void Update()
        {
            // Tower bir hedef bulduysa ona dön
            if (towerComponent != null && towerComponent.currentTarget != null)
            {
                RotateTowards(towerComponent.currentTarget.transform.position);
            }
        }

        public void RotateTowards(Vector3 targetPosition)
        {
            // Liste boşsa işlem yapma
            if (currentDirectionalData == null || currentDirectionalData.Count == 0 || spriteRenderer == null)
                return;

            lastTargetPosition = targetPosition;

            Vector3 direction = targetPosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            float step = 360f / 8f; 
            int index = Mathf.FloorToInt((angle + (step / 2)) / step) % 8;

            currentSegmentIndex = index;

            if (currentSegmentIndex < currentDirectionalData.Count)
            {
                DirectionalData currentData = currentDirectionalData[currentSegmentIndex];
                
                if (currentData.sprite != null)
                    spriteRenderer.sprite = currentData.sprite;
                
                if(currentData.scale == Vector3.zero) 
                    transform.localScale = Vector3.one;
                else
                    transform.localScale = currentData.scale;
            }
        }

        public Vector2 GetCurrentFirePointOffset(int index)
        {
            if (currentDirectionalData != null && index >= 0 && index < currentDirectionalData.Count)
            {
                return currentDirectionalData[index].firePointOffset;
            }
            return Vector2.zero;
        }

        // --- KRİTİK FONKSİYON: LİSTE DEĞİŞTİRME ---
        // Tower.cs upgrade olunca bu fonksiyonu çağırıp yeni levelin listesini verecek
        public void SetLevelVisuals(List<DirectionalData> newLevelData)
        {
            // Yeni listeyi kopyala
            currentDirectionalData = new List<DirectionalData>(newLevelData);

            // Görüntüyü hemen yenile
            RotateTowards(lastTargetPosition);
        }
    }
}
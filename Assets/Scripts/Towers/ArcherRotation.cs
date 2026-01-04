using UnityEngine;
using System.Collections.Generic;

namespace TowerDefense.Tower
{
    public class ArcherRotation : RotatableTowerSprite
    {
        public override void RotateTowards(Vector3 targetPosition)
        {
            // Liste boşsa işlem yapma
            if (currentDirectionalData == null || currentDirectionalData.Count == 0)
                return;

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr == null) return;

            Vector3 direction = targetPosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            // 4 Yönlü Mantık
            // 0: Right, 1: Up, 2: Left, 3: Down
            float step = 360f / 4f; 
            int index = Mathf.FloorToInt((angle + (step / 2)) / step) % 4;

            currentSegmentIndex = index;

            if (currentSegmentIndex < currentDirectionalData.Count)
            {
                DirectionalData currentData = currentDirectionalData[currentSegmentIndex];
                
                if (currentData.sprite != null)
                    sr.sprite = currentData.sprite;
                
                if(currentData.scale == Vector3.zero) 
                    transform.localScale = Vector3.one;
                else
                    transform.localScale = currentData.scale;
            }
        }
    }
}

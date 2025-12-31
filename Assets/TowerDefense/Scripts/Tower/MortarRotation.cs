using UnityEngine;
using System.Collections.Generic;

namespace TowerDefense.Tower
{
    public class MortarRotation : RotatableTowerSprite
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

            // 6 Yönlü Mantık (60 derece aralıklarla)
            // 0: Right (0-60), 1: Up-Right (60-120), 2: Up-Left (120-180), 
            // 3: Left (180-240), 4: Down-Left (240-300), 5: Down-Right (300-360)
            
            float step = 360f / 6f; 
            int index = Mathf.FloorToInt((angle + (step / 2)) / step) % 6;

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
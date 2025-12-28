using UnityEngine;
using System.Collections.Generic;

namespace TowerDefense.Environment
{
    public class PathSystem : MonoBehaviour
    {
        // Waypointleri otomatik alacağız, public yapıp sürüklemeye gerek yok
        private List<Transform> waypoints = new List<Transform>();

        void Awake()
        {
            // Bu obje (EnemyPath) altındaki tüm child objeleri (Waypoint1, Waypoint2...) listeye al
            foreach (Transform child in transform)
            {
                waypoints.Add(child);
            }
            Debug.Log($"PathSystem: {waypoints.Count} adet waypoint bulundu.");
        }

        /// <summary>
        /// Mouse pozisyonuna en yakın yol parçasını ve o parçanın yönünü bulur.
        /// </summary>
        public bool GetClosestPointOnPath(Vector2 mousePos, out Vector2 closestPoint, out Vector2 pathDirection)
        {
            closestPoint = Vector2.zero;
            pathDirection = Vector2.right; // Varsayılan yön
            
            float minDistanceSqr = float.MaxValue;
            bool found = false;

            // Tüm waypointler arasındaki çizgileri tek tek kontrol et
            // Örn: Waypoint1 -> Waypoint2, Waypoint2 -> Waypoint3
            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                Vector2 start = waypoints[i].position;
                Vector2 end = waypoints[i + 1].position;

                // Bu çizgi üzerindeki en yakın noktayı hesapla
                Vector2 pointOnSegment = GetClosestPointOnSegment(start, end, mousePos);
                
                // Mesafeyi ölç
                float distSqr = (mousePos - pointOnSegment).sqrMagnitude;
                
                // Eğer bu çizgi mouse'a daha yakınsa, bunu seç
                if (distSqr < minDistanceSqr)
                {
                    minDistanceSqr = distSqr;
                    closestPoint = pointOnSegment;
                    
                    // Yolun yönünü (vektörünü) kaydet
                    pathDirection = (end - start).normalized;
                    found = true;
                }
            }

            return found;
        }

        // Matematiksel Yardımcı: Bir doğru parçası üzerindeki en yakın noktayı bulur
        private Vector2 GetClosestPointOnSegment(Vector2 a, Vector2 b, Vector2 p)
        {
            Vector2 ap = p - a;
            Vector2 ab = b - a;
            float ab2 = ab.sqrMagnitude;
            if (ab2 == 0) return a;
            float t = Vector2.Dot(ap, ab) / ab2;
            
            // t'yi 0 ile 1 arasına sıkıştır (Çizginin dışına taşmasın)
            t = Mathf.Clamp01(t);
            return a + ab * t;
        }
        
        // Editörde yolu görmek için (Opsiyonel)
        void OnDrawGizmos()
        {
            if (transform.childCount > 0)
            {
                Gizmos.color = Color.yellow;
                for (int i = 0; i < transform.childCount - 1; i++)
                {
                    Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
                }
            }
        }
    }
}
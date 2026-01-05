using UnityEngine;
using UnityEngine.EventSystems;
using TowerDefense.Buildings;
using System.Collections; // <--- 1. BURASI EKLENDİ (IEnumerator için şart)

namespace TowerDefense.Environment
{
    public class BarrierPlacementManager : MonoBehaviour
    {
        public static BarrierPlacementManager Instance;
        
        [Header("Referanslar")]
        public PathSystem pathSystem;
        public PathSystem pathSystem2; // İkinci path (Kirin haritası için)
        public GameObject barrierPrefab;
        
        [Header("Ayarlar")]
        public float placementRange = 2.0f;
        public float angleCorrection = 90f;

        private GameObject ghostObject;
        private BarrierSpriteController ghostController;
        private SpriteRenderer ghostRenderer;
        
        // Property
        public bool IsActive => isPlacingMode;
        private bool isPlacingMode = false;

        void Awake()
        {
            Instance = this;
        }

        void Update()
        {
            if (!isPlacingMode) return;

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                CancelPlacement();
                return;
            }

            UpdateGhostPosition();

            if (Input.GetMouseButtonDown(0) && ghostObject.activeSelf && !EventSystem.current.IsPointerOverGameObject())
            {
                PlaceBarrier();
            }
        }

        public void StartPlacement()
        {
            Debug.Log("[BarrierPlacement] StartPlacement() çağrıldı!");
            
            // PathSystem kontrolü
            if (pathSystem == null)
            {
                Debug.LogError("❌ PathSystem NULL! BarrierPlacementManager Inspector'ına PathSystem objesini ata!");
                return;
            }
            
            if (barrierPrefab == null)
            {
                Debug.LogError("❌ BarrierPrefab NULL! Inspector'a Barrier prefab'ını ata!");
                return;
            }

            if (isPlacingMode) CancelPlacement();

            isPlacingMode = true;
            Debug.Log("[BarrierPlacement] Placement modu açıldı!");

            if (ghostObject == null)
            {
                Debug.Log("[BarrierPlacement] Ghost objesi oluşturuluyor...");
                ghostObject = Instantiate(barrierPrefab);
                ghostObject.name = "Ghost_Barrier";
                
                // Ghost objesinde ses çalmasını engelle
                Barrier barrierScript = ghostObject.GetComponentInChildren<Barrier>();
                if (barrierScript != null)
                {
                    barrierScript.isGhost = true;
                }

                ghostController = ghostObject.GetComponentInChildren<BarrierSpriteController>();
                ghostRenderer = ghostObject.GetComponentInChildren<SpriteRenderer>();

                var colliders = ghostObject.GetComponentsInChildren<Collider2D>();
                foreach (var col in colliders) col.enabled = false;

                if (ghostRenderer != null)
                {
                    Color c = ghostRenderer.color;
                    c.a = 0.5f;
                    ghostRenderer.color = c;
                }
                Debug.Log("[BarrierPlacement] Ghost objesi oluşturuldu!");
            }
            ghostObject.SetActive(false);
        }

        private void UpdateGhostPosition()
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            // İlk olarak path1'i dene
            bool foundOnPath1 = false;
            Vector2 closestPoint1 = Vector2.zero;
            Vector2 pathDirection1 = Vector2.right;
            float distance1 = float.MaxValue;
            
            if (pathSystem != null && pathSystem.GetClosestPointOnPath(mousePos, out closestPoint1, out pathDirection1))
            {
                distance1 = Vector2.Distance(mousePos, closestPoint1);
                if (distance1 <= placementRange)
                {
                    foundOnPath1 = true;
                }
            }
            
            // İkinci path'i dene (varsa)
            bool foundOnPath2 = false;
            Vector2 closestPoint2 = Vector2.zero;
            Vector2 pathDirection2 = Vector2.right;
            float distance2 = float.MaxValue;
            
            if (pathSystem2 != null && pathSystem2.GetClosestPointOnPath(mousePos, out closestPoint2, out pathDirection2))
            {
                distance2 = Vector2.Distance(mousePos, closestPoint2);
                if (distance2 <= placementRange)
                {
                    foundOnPath2 = true;
                }
            }
            
            // Hangisi daha yakınsa onu kullan
            Vector2 selectedPoint;
            Vector2 selectedDirection;
            
            if (foundOnPath1 && foundOnPath2)
            {
                // Her ikisi de menzilde, daha yakın olanı seç
                if (distance1 <= distance2)
                {
                    selectedPoint = closestPoint1;
                    selectedDirection = pathDirection1;
                }
                else
                {
                    selectedPoint = closestPoint2;
                    selectedDirection = pathDirection2;
                }
            }
            else if (foundOnPath1)
            {
                selectedPoint = closestPoint1;
                selectedDirection = pathDirection1;
            }
            else if (foundOnPath2)
            {
                selectedPoint = closestPoint2;
                selectedDirection = pathDirection2;
            }
            else
            {
                // Hiçbir path menzilde değil
                ghostObject.SetActive(false);
                return;
            }
            
            // Ghost'u konumlandır
            ghostObject.SetActive(true);
            ghostObject.transform.position = selectedPoint;

            float pathAngle = Mathf.Atan2(selectedDirection.y, selectedDirection.x) * Mathf.Rad2Deg;
            float barrierAngle = pathAngle + angleCorrection;
            int spriteIndex = GetDirectionIndex(barrierAngle);
            
            // Önce yönü ayarla
            if (ghostController != null)
            {
                ghostController.SetBarrierDirection(spriteIndex);
            }

            // Tüm sprite renderer'ları şeffaf yap
            SpriteRenderer[] allRenderers = ghostObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (var sr in allRenderers)
            {
                Color c = sr.color;
                c.a = 0.5f;
                sr.color = c;
            }
        }
        
        private void PlaceBarrier()
        {
            // Gerçek bariyeri koy
            GameObject realBarrier = Instantiate(barrierPrefab, ghostObject.transform.position, Quaternion.identity);
            
            BarrierSpriteController realController = realBarrier.GetComponentInChildren<BarrierSpriteController>();
            if (realController != null && ghostController != null)
            {
                realController.SetBarrierDirection(ghostController.currentDirectionIndex);
            }

            // --- 2. DEĞİŞİKLİK BURADA ---
            // CancelPlacement(); // <--- ESKİ KOD BUYDU (Hemen kapatıyordu)
            
            StartCoroutine(DisableAfterFrame()); // <--- YENİ KOD (Biraz bekleyip kapatacak)
        }

        // --- 3. YENİ FONKSİYON ---
        // Bu fonksiyon, mod kapatma işlemini bir kare (frame) sonraya atar.
        private IEnumerator DisableAfterFrame()
        {
            // Bu kare bitene kadar bekle.
            // Böylece HeroInput scripti çalıştığında IsActive hala "TRUE" dönecek.
            yield return null; 
            
            CancelPlacement();
        }

        public void CancelPlacement()
        {
            isPlacingMode = false;
            if (ghostObject != null) Destroy(ghostObject);
        }

        private int GetDirectionIndex(float angle)
        {
            angle = (angle % 360 + 360) % 360;
            int index = Mathf.RoundToInt(angle / 45f);
            if (index >= 8) index = 0;
            return index;
        }
    }
}
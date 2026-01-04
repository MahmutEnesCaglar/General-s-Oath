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
            // BuildManager çakışması varsa kapat (Opsiyonel ama önerilir)
            // if (BuildManager.main != null) BuildManager.main.DeselectNode();

            if (isPlacingMode) CancelPlacement();

            isPlacingMode = true;

            if (ghostObject == null)
            {
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
            }
            ghostObject.SetActive(false);
        }

        private void UpdateGhostPosition()
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            if (pathSystem.GetClosestPointOnPath(mousePos, out Vector2 closestPoint, out Vector2 pathDirection))
            {
                float distance = Vector2.Distance(mousePos, closestPoint);

                if (distance <= placementRange)
                {
                    ghostObject.SetActive(true);
                    ghostObject.transform.position = closestPoint;

                    float pathAngle = Mathf.Atan2(pathDirection.y, pathDirection.x) * Mathf.Rad2Deg;
                    float barrierAngle = pathAngle + angleCorrection;
                    int spriteIndex = GetDirectionIndex(barrierAngle);
                    
                    // Önce yönü ayarla
                    if (ghostController != null)
                    {
                        ghostController.SetBarrierDirection(spriteIndex);
                    }

                    // --- DÜZELTME BURADA ---
                    // Tek bir renderer yerine, child'lardaki hepsini bul ve şeffaf yap
                    SpriteRenderer[] allRenderers = ghostObject.GetComponentsInChildren<SpriteRenderer>();
                    foreach (var sr in allRenderers)
                    {
                        Color c = sr.color;
                        c.a = 0.5f; // %50 Şeffaflık
                        sr.color = c;
                    }
                    // -----------------------
                }
                else
                {
                    ghostObject.SetActive(false);
                }
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
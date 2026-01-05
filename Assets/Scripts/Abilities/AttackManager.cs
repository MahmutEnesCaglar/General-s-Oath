using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using TowerDefense.Abilities; // MeteorStrike scripti için

namespace TowerDefense.Core
{
    public class AttackManager : MonoBehaviour
    {
        public static AttackManager Instance;

        [Header("Referanslar")]
        public GameObject meteorPrefab;     // Meteor prefabı
        public GameObject targetIndicatorPrefab; // Yerde görünecek nişangah (Ghost)

        [Header("Ayarlar")]
        public LayerMask whatIsGround;      // Yeri algılamak için (Opsiyonel, yoksa direkt koordinat alırız)

        private GameObject ghostObject;
        private bool isAttackMode = false;
        
        // Property
        public bool IsActive => isAttackMode;

        void Awake()
        {
            // Her sahne için yeni bir AttackManager instance'ı oluştur
            if (Instance != null && Instance != this)
            {
                Debug.Log("[AttackManager] Eski instance temizleniyor, yeni instance oluşturuluyor.");
            }
            Instance = this;
            
            // Durumları sıfırla
            isAttackMode = false;
            if (ghostObject != null)
            {
                Destroy(ghostObject);
                ghostObject = null;
            }
            
            Debug.Log("[AttackManager] Instance oluşturuldu.");
        }

        void Update()
        {
            if (!isAttackMode) return;

            // İptal (Sağ tık veya ESC)
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                CancelAttack();
                return;
            }

            UpdateTargetIndicator();

            // Sol Tık ve UI üzerinde değilsek
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                PerformAttack();
            }
        }

        public void StartAttackMode()
        {
            // Eğer bariyer modu açıksa onu kapat (Çakışma önleme)
            if (TowerDefense.Environment.BarrierPlacementManager.Instance != null)
                TowerDefense.Environment.BarrierPlacementManager.Instance.CancelPlacement();

            if (isAttackMode) CancelAttack();

            isAttackMode = true;

            // Göstergeyi oluştur
            if (ghostObject == null && targetIndicatorPrefab != null)
            {
                ghostObject = Instantiate(targetIndicatorPrefab);
                ghostObject.name = "Target_Indicator";
                
                // Rengini kırmızımsı ve şeffaf yap
                SpriteRenderer sr = ghostObject.GetComponentInChildren<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = new Color(1f, 0f, 0f, 0.5f); // Yarı saydam kırmızı
                    sr.sortingOrder = 5000; // Her şeyin üzerinde görünsün
                }
            }
            if (ghostObject != null) ghostObject.SetActive(true);
        }

        private void UpdateTargetIndicator()
        {
            if (ghostObject == null) return;

            // Mouse pozisyonunu al
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0; // 2D düzlemdeyiz

            // Göstergeyi mouse'un ucuna taşı
            ghostObject.transform.position = mousePos;
        }

        private void PerformAttack()
        {
            Vector3 targetPos = ghostObject.transform.position;

            // Meteoru oluştur
            GameObject meteorObj = Instantiate(meteorPrefab);
            MeteorStrike meteorScript = meteorObj.GetComponent<MeteorStrike>();

            if (meteorScript != null)
            {
                meteorScript.Initialize(targetPos);
            }

            StartCoroutine(DisableAfterFrame());
        }

        private IEnumerator DisableAfterFrame()
        {
            yield return null;
            CancelAttack();
        }

        public void CancelAttack()
        {
            isAttackMode = false;
            if (ghostObject != null) Destroy(ghostObject);
        }
    }
}
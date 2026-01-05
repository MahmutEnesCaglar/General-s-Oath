using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic; // List kullanımı için gerekli
using TowerDefense.Core;
using TowerDefense.Hero;
using TowerDefense.Tower;

namespace TowerDefense.Hero
{
    public class HeroInput : MonoBehaviour
    {
        public static HeroInput Instance;

        [Header("References")]
        public Hero hero;
        // Eski TowerPlacement referansını sildik.

        private void Awake()
        {
            // Her sahne için yeni bir HeroInput instance'ı oluştur
            if (Instance != null && Instance != this)
            {
                Debug.Log("[HeroInput] Eski instance temizleniyor, yeni instance oluşturuluyor.");
            }
            Instance = this;
            
            // Hero referansını temizle (Start'ta yeniden bulunacak)
            hero = null;
            
            Debug.Log("[HeroInput] Instance oluşturuldu.");
        }

        private void Start()
        {
            Debug.Log("HeroInput initialized. Click on ground to move, Click on Build Spots to build.");
        }

        private void Update()
        {
            // Mouse takılı değilse çık
            if (Mouse.current == null) return;

            // Hero yoksa aramaya devam et
            if (hero == null) 
            {
                hero = FindAnyObjectByType<Hero>();
                // Bulunca konsola yazsın
                if(hero != null) Debug.Log("Hero bulundu ve bağlandı!");
            }

            // SOL TIK BASILDI MI?
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Debug.Log("Sol Tık Algılandı! İşlemler başlıyor..."); // BU YAZI ÇIKIYOR MU?

                HandleInput();
            }
             // Özel yetenek (Q)
            if (Keyboard.current[Key.Q].wasPressedThisFrame)
            {
                if (hero != null && hero.abilities != null)
                    hero.abilities.ActivateSpecialAbility();
            }

            // Bloklama (Sağ Tık veya B)
            bool blockPressed = Mouse.current.rightButton.isPressed || Keyboard.current[Key.B].isPressed;
            if (hero != null && hero.abilities != null)
            {
                hero.abilities.ActivateBlock(blockPressed);
            }
        }
        private void HandleInput()
        {
            // 1. UI KONTROLÜ
            if (IsPointerOverInteractiveUI()) 
            {
                Debug.Log("❌ Tıklama UI (Buton/Panel) tarafından engellendi.");
                return;
            }

            // --- GÜNCELLENEN KISIM: ÖZEL MOD KONTROLLERİ ---
            // Bariyer modu açık mı?
            bool isBarrierMode = TowerDefense.Environment.BarrierPlacementManager.Instance != null && 
                                 TowerDefense.Environment.BarrierPlacementManager.Instance.IsActive;
            
            // Saldırı (Meteor) modu açık mı?
            bool isAttackMode = TowerDefense.Core.AttackManager.Instance != null && 
                                TowerDefense.Core.AttackManager.Instance.IsActive;

            // Eğer herhangi bir özel mod aktifse Hero hareket etmemeli
            if (isBarrierMode || isAttackMode)
            {
                Debug.Log("⛔ Özel mod (Bariyer/Saldırı) aktif. Hero hareket etmeyecek.");
                // Return diyerek Hero hareket kodlarını ve BuildManager kontrolünü pas geçiyoruz.
                // Tıklamayı sadece ilgili Manager (Barrier veya Attack) yakalayacak.
                return; 
            }
            // ------------------------------------------------

            // 2. İNŞAAT ALANI KONTROLÜ
            float camDistance = Mathf.Abs(Camera.main.transform.position.z);
            Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, camDistance));

            // BuildManager kontrolü
            if (BuildManager.main != null)
            {
                if (BuildManager.main.IsMouseOverBuildSpot())
                {
                    Debug.Log($"🏗️ İnşaat Alanı Algılandı! (Koordinat: {worldPoint}) - Hero duruyor, Menü açılmalı.");
                    return; 
                }
            }

            // YENİ: Kule Kontrolü (Upgrade Menüsü için)
            // Eğer bir kuleye tıklanıyorsa Hero hareket etmemeli
            RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint, Vector2.zero);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider.CompareTag("Tower"))
                {
                    Debug.Log($"🏰 Kule Algılandı! (Koordinat: {worldPoint}) - Hero duruyor, Upgrade Menüsü açılmalı.");
                    
                    // Kule scriptine eriş ve menüyü aç
                    TowerDefense.Tower.Tower tower = hit.collider.GetComponent<TowerDefense.Tower.Tower>();
                    if (tower == null) tower = hit.collider.GetComponentInParent<TowerDefense.Tower.Tower>();
                    
                    if (tower != null)
                    {
                        // tower.ToggleUpgradeUI(); // ARTIK BURADAN ÇAĞIRMIYORUZ
                        // BuildManager zaten bu tıklamayı yakalayıp UpgradeMenuUI'ı açacak.
                        // Biz sadece Hero'nun hareket etmesini engelliyoruz.
                    }

                    return;
                }
            }
            
            // 3. HERO HAREKETİ (Mevcut kodunda devam eden kısım...)
             Debug.Log($"✅ Boş alana tıklandı. Hero şuraya gitmeli: {worldPoint}");
             HandleHeroMovement();
        }

        private void HandleHeroMovement()
        {
            if (hero == null || hero.isDead) return;

            // Mouse pozisyonunu al (Dinamik Mesafe Hesabı)
            float camDistance = Mathf.Abs(Camera.main.transform.position.z);
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            
            // Z eksenini tam sıfıra oturtuyoruz
            Vector3 targetPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, camDistance));
            targetPos.z = 0; 

            // Hero'ya git emri ver
            hero.SetDestination(targetPos);
        }

        // UI Tıklamasını algılayan yardımcı fonksiyon (Senin kodun aynısı)
        private bool IsPointerOverInteractiveUI()
        {
            if (EventSystem.current == null) return false;

            var pointerData = new PointerEventData(EventSystem.current)
            {
                // Both modunda Input.mousePosition daha garantidir
                position = Input.mousePosition 
            };

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            // Tıklanan tüm UI elemanlarını kontrol et
            foreach (var result in results)
            {
                // Engelleyen objenin adını konsola yazdır (SUÇLUYU BURADA GÖRECEĞİZ)
                // 
                // 
                // Debug.Log("🖱️ Mouse şu UI objesinin üzerinde: " + result.gameObject.name);

                // Eğer tıklanan şey bir Buton, Toggle, InputField veya Scrollbar ise...
                if (result.gameObject.GetComponent<UnityEngine.UI.Button>() != null ||
                    result.gameObject.GetComponent<UnityEngine.UI.Toggle>() != null ||
                    result.gameObject.GetComponent<UnityEngine.UI.Slider>() != null ||
                    result.gameObject.GetComponent<UnityEngine.UI.Scrollbar>() != null ||
                    result.gameObject.GetComponent<TMPro.TMP_InputField>() != null ||
                    result.gameObject.GetComponent<UnityEngine.UI.InputField>() != null) 
                {
                    Debug.Log($"⛔ TIKLAMA ENGELLENDİ! Engelleyen Buton/Araç: {result.gameObject.name}");
                    return true; 
                }

                // EĞER SADECE ARKA PLAN RESMİ İSE (Panel, Image vb.)
                // Genelde panellerin "Raycast Target"ı açık unutulur.
                // Eğer şeffaf bir panel yüzünden tıklayamıyorsan burası yakalayacak.
                if (result.gameObject.GetComponent<UnityEngine.UI.Image>() != null)
                {
                     // İpucu: Eğer oyunun oynanmasını engelleyen şey şeffaf bir panelse,
                     // o panelin Inspector'ındaki "Raycast Target" tikini kaldırmalısın.
                     Debug.Log($"⚠️ Dikkat: '{result.gameObject.name}' isimli obje tıklamanı kesiyor olabilir! Raycast Target açık mı?");
                     // Şimdilik Image'leri engelleyici saymıyoruz ki test edelim.
                     // Eğer gerçekten panel engelliyorsa burayı 'return true' yapabilirsin.
                }
            }

            return false; 
        }
    }
}
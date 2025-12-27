# General's Oath

**Tower Defense Oyunu** - Unity ile geliştirilmiştir.

![Game Screenshot](Screenshots/game_play.png)

---

## 🎮 Temel Özellikler

### Oyun Mekanikleri
- **3 Harita:** Grifon, Kirin, Ve Ejderha haritaları.
- **Wave Sistemi:** 10 normal dalga + 1 Final Boss.
- **Tower Defense:** 3 farklı kule tipi (Okçu, Topçu, Havan).
  - *Okçu & Havan:* 8 yönlü sprite rotasyon sistemi.
  - *Havan:* Alan hasarı (AOE) ve parabolik atış mekaniği.
- **Hero Sistemi:** Click-to-move kontrol, yetenek sistemi, can barı yönetimi.
- **Fizik Tabanlı Hedefleme:** İzometrik (Oval) menzil algılama ve Tag/Layer tabanlı düşman tespiti.

### Kontroller
| Komut | Aksiyon |
|-------|---------|
| **Sol Tık** | Hero hareketi / Kule yerleştirme / UI Etkileşimi |
| **T** | Hero/Tower modu geçişi |
| **Sağ Tık** | Hero bloklama / İptal |
| **WASD** | Kamera hareketi |
| **Mouse Scroll** | Zoom (Dinamik) |
| **ESC** | Pause Menüsü |

---

## 🛠️ Son Geliştirme Güncellemeleri (Dev Update)

### 🏗️ Kule & İnşaat Sistemi
- **BuildSpot Sistemi:** Tilemap tıklama sorunları giderildi. Artık kule yerleşimi için özel `Collider` alanları (BuildSpot) kullanılıyor.
- **Oval (İzometrik) Menzil:** `CircleCollider2D` yerine matematiksel olarak hesaplanan `PolygonCollider2D` ile tam izometrik (yumurta şeklinde) menzil algılama sistemi kuruldu.
- **Otomatik Menzil Görseli:** Inspector'dan menzil değiştirildiğinde collider ve gizmos otomatik güncelleniyor.

### 🏹 Projectile (Mermi) Fizikleri
- **Unified Tower Logic:** Tek bir `Tower.cs` üzerinden farklı mermi tipleri (Arrow, Mortar, Cannon) otomatik tanınıyor.
- **Mortar (Havan):** `Setup` fonksiyonu ile hedef konuma parabolik uçuş ve `OverlapCircleAll` ile alan hasarı (AOE) eklendi.
- **Arrow (Ok):** Hedefe kilitlenen ve rotasyonu hedefe göre dönen güdümlü uçuş sistemi.

### 🦸 Hero & Yetenekler
- **Input Conflict Fix:** "Kule yerine tıklayınca Hero'nun yürümesi" sorunu Raycast önceliklendirmesi ile çözüldü.

---

## 🏗️ Mimari & Dosya Yapısı

### Temel Sistemler
- **GameManager:** Oyun döngüsü, para ve can yönetimi.
- **AbilityManager:** Hero yeteneklerinin cooldown ve işlev kontrolü.
- **PauseManager:** New Input System uyumlu durdurma, ses ve sahne yönetimi.

### Kule Sistemi (`TowerDefense.Tower`)
- **Tower.cs:** Ana mantık. Hedef seçme, ateşleme, menzil çizimi.
- **MortarTower.cs:** Kuleye özel alan hasarı (Explosion Radius) parametreleri.
- **Projectile Scripts:** `ArrowProjectile`, `MortarProjectile`, `Projectile` (Base).
- **Visuals:** `RotatableTowerSprite` (8 yönlü sprite değiştirme), `ArcherRotation`.

### Düşman Sistemi (`TowerDefense.Enemy`)
- **Enemy.cs:** Can, hasar alma ve yok olma mantığı.
- **Movement:** Waypoint tabanlı hareket, Rigidbody2D (Kinematic) fizik yapısı.

---

## 📁 Proje Klasör Yapısı

```text
Assets/
├── TowerDefense/
│   ├── Scripts/
│   │   ├── Core/           # GameManager, AbilityManager, Config
│   │   ├── Enemy/          # Enemy, Spawner, AI
│   │   ├── Tower/          # Tower Logic, Projectiles, Rotation
│   │   └── Hero/           # HeroMovement, HeroHealth, Input
│   └── Prefabs/            # Hazır objeler (Towers, Enemies, Projectiles)
├── Scripts/                # UI ve Yardımcılar (PauseManager vb.)
└── Scenes/                 # MainMenu, Map_Grifon, Map_Kirin
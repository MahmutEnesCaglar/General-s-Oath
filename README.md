# General's Oath

**Tower Defense Oyunu** - Unity ile geliştirilmiştir.

![Grifon Map](Screenshots/game_play.png)

---

## 🎮 Temel Özellikler

### Oyun Mekanikleri
- **3 Harita:** Grifon, Kirin, Ejderha haritaları
- **10 Dalga Sistem:** Her harita 10 wave + 1 final boss
- **Tower Defense:** 3 farklı kule tipi (Ground, Universal, AOE)
- **Hero Sistemi:** Click-to-move kontrol, düşman aggro, saldırı/savunma yetenekleri
- **Can Sistemi:** 5 can, her düşman geçtiğinde -1 can, görsel can barı

### Kontroller
| Komut | Aksiyon |
|-------|---------|
| **Sol Tık** | Hero hareketi (Hero Mode) / Kule yerleştirme (Tower Mode) |
| **T** | Hero/Tower mode geçişi |
| **Q** | Hero özel yetenek |
| **Sağ Tık / B** | Hero blok |
| **WASD / Ok Tuşları** | Kamera hareketi |
| **Mouse Scroll** | Zoom (3-9 arası) |
| **ESC** | Pause menüsü |

### UI Sistemleri
- **Ana Menü:** Müzik kontrolü, scene yönetimi
- **Pause Menü:** Time scale kontrolü, ses ayarları, blur overlay
- **Dinamik Kamera:** Zoom'a göre hareket sınırları, smooth camera
- **Can Barı:** Sprite tabanlı görsel can göstergesi (5/5 → 0/5)

---

## 🏗️ Mimari

### Tower Defense Sistemi
- **GameManager:** Singleton pattern, oyun durumu, para/can yönetimi
- **EnemySpawner:** Wave bazlı düşman spawn sistemi
- **TowerPlacement:** Grid tabanlı kule yerleştirme
- **Waypoint System:** Düşman yol takibi

### Hero Sistemi
- **HeroInput:** Mod bazlı input yönetimi (UI raycast filtreleme)
- **HeroMovement:** Click-to-move navigasyon
- **HeroAbilities:** Özel yetenek, blok, saldırı mekanikleri
- **Enemy Aggro:** Menzil bazlı hero takibi ve saldırı

### Son Güncellemeler (2025-12-26)
- ✅ Hero hareket sistemi düzeltildi - Sadece interaktif UI elementleri hero hareketini blokluyor
- ✅ Can sistemi güncellendi - Toplam 5 can, her düşman için -1 can
- ✅ SpriteHealthBar GameManager ile entegre edildi - Otomatik güncelleme
- ✅ Game Over ekranı UIManager ile entegre edildi

---

## 📁 Proje Yapısı

```
Assets/
├── TowerDefense/
│   ├── Scripts/
│   │   ├── Core/          # GameManager, UIManager, Configurators
│   │   ├── Enemy/         # Enemy, EnemySpawner, EnemyType
│   │   ├── Tower/         # TowerBuilder, TowerPlacement, Projectile
│   │   └── Hero/          # Hero, HeroInput, HeroMovement, HeroAbilities
│   └── Prefabs/           # Tower, Enemy, Hero prefab'ları
├── Scripts/               # UI ve yardımcı scriptler
└── Scenes/                # MainMenu, GameScene, Map_Grifon, Map_Kirin, Map_Ejderha
```

---

## 🛠️ Teknik Detaylar

- **Unity Version:** 6.2+
- **Input System:** New Input System (Keyboard, Mouse)
- **Architecture:** Singleton GameManager, modüler tower/enemy sistemleri
- **Camera:** Orthographic 2D, dinamik zoom/pan
- **UI:** TextMeshPro, Canvas overlay, EventSystem
- **Audio:** MusicManager (DontDestroyOnLoad), PlayerPrefs ile ayar kalıcılığı

---
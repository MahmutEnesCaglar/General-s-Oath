# 🎮 UNITY TOWER DEFENSE - KURULUM REHBERİ

Bu rehber Unity'yi ilk kez kullanıyorsanız sıfırdan nasıl kurulum yapacağınızı gösterir.

---

## 📁 1. KLASÖR YAPISI - PREFABLARI NEREYE KOYACAKSINIZ

Unity'de şu klasör yapısını oluşturun:

```
Assets/
├── TowerDefense/
│   ├── Prefabs/
│   │   ├── Enemies/          ← Düşman prefab'ları buraya
│   │   ├── Towers/           ← Kule prefab'ları buraya
│   │   └── Projectiles/      ← Mermi prefab'ları buraya
│   ├── Sprites/
│   │   ├── Enemies/          ← Düşman sprite'ları buraya
│   │   ├── Towers/           ← Kule sprite'ları buraya
│   │   ├── Projectiles/      ← Mermi sprite'ları buraya
│   │   └── UI/               ← UI ikonları buraya
│   ├── Scripts/              ← Zaten var (script'leriniz burada)
│   └── Data/                 ← Zaten var
```

### Klasörleri Nasıl Oluşturursunuz?

1. **Project** penceresinde **Assets/TowerDefense** klasörüne sağ tıklayın
2. **Create > Folder** seçin
3. Klasör adını yazın (örn: "Prefabs")
4. Yukarıdaki yapıyı oluşturun

---

## 🎨 2. SPRITE'LARI UNITY'E AKTARMA

### Adım 1: Sprite'ları İndirin veya Oluşturun

İzometrik 2D tower defense için sprite'lara ihtiyacınız var:

**ÜCRETSİZ ASSET KAYNAKLARI:**
- [itch.io](https://itch.io/game-assets/free/tag-tower-defense) - Ücretsiz Tower Defense asset'leri
- [OpenGameArt](https://opengameart.org/) - Açık kaynak sprite'lar
- [Kenney.nl](https://kenney.nl/assets) - Ücretsiz game asset'leri

**ARAMANIZ GEREKENLER:**
- "isometric tower defense sprites"
- "2d tower defense enemies"
- "tower defense turrets sprites"

### Adım 2: Sprite'ları Unity'e Aktarın

1. Bilgisayarınızdan sprite dosyalarını (PNG, JPG) kopyalayın
2. Unity'de **Assets/TowerDefense/Sprites/** klasörüne **sürükleyip bırakın**
3. Unity otomatik olarak import edecek

### Adım 3: Sprite Import Ayarları

Her sprite'a tıklayın ve **Inspector** penceresinde:

```
Texture Type: Sprite (2D and UI)
Pixels Per Unit: 100 (varsayılan)
Filter Mode: Point (pixel art için) veya Bilinear (smooth için)
Compression: None (kalite) veya Normal Quality
```

**Apply** butonuna tıklayın.

---

## 🤖 3. DÜŞMAN PREFAB OLUŞTURMA

### Adım 1: İlk Düşman Prefab'ını Oluşturun

1. **Hierarchy** penceresinde sağ tıklayın
2. **Create Empty** seçin
3. İsim verin: `BasicEnemy`

### Adım 2: Sprite Ekleyin

1. `BasicEnemy` objesini seçin
2. **Inspector**'da **Add Component** butonuna tıklayın
3. **Sprite Renderer** yazıp seçin
4. **Sprite** alanına düşman sprite'ınızı sürükleyin

### Adım 3: Script Ekleyin

1. `BasicEnemy` seçiliyken **Add Component**
2. **Enemy** yazın (yazdığımız script)
3. Script eklenecek

### Adım 4: Rigidbody2D Ekleyin

1. **Add Component**
2. **Rigidbody2D** seçin
3. Inspector'da ayarları:
   - **Body Type: Kinematic**
   - **Gravity Scale: 0**

### Adım 5: Collider Ekleyin (Opsiyonel)

1. **Add Component**
2. **Circle Collider 2D** veya **Box Collider 2D** seçin
3. **Is Trigger: işaretli** yapın (kule hedeflemesi için)

### Adım 6: Enemy Script Ayarları

Inspector'da **Enemy** script'inde:

```
Enemy Type: basic
Max Health: 75
Damage: 5
Move Speed: 2
Money Reward: 5
Is Flying: false (işaretsiz)
Is Boss: false (işaretsiz)
```

### Adım 7: Prefab Olarak Kaydedin

1. **Hierarchy**'den `BasicEnemy` objesini seçin
2. **Project** penceresinde **Assets/TowerDefense/Prefabs/Enemies/** klasörüne **sürükleyin**
3. Prefab oluştu! Mavi küp ikonu göreceksiniz
4. **Hierarchy**'deki objeyi silebilirsiniz (prefab zaten kaydedildi)

### Adım 8: Diğer Düşmanları Oluşturun

Aynı adımları tekrarlayın:
- `FastEnemy` (hızlı düşman)
- `ArmoredEnemy` (zırhlı)
- `ArcherEnemy` (okçu)
- `FlyingEnemy` (uçan - Is Flying: true)
- `EliteEnemy` (elit)
- `MiniBoss` (mini boss)
- `GrifonBoss` (final boss)
- `KirinBoss` (final boss)
- `EjderhaBoss` (final boss)

Her birinin istatistiklerini `FinalBossConfigurator.cs` dosyasındaki değerlere göre ayarlayın.

---

## 🏰 4. KULE PREFAB OLUŞTURMA

### Adım 1: Ground Tower (Okçu Kulesi)

1. **Hierarchy** > **Create Empty** > `GroundTower`
2. **Add Component** > **Sprite Renderer** > Kule sprite'ı ekle
3. **Add Component** > **Tower** (script)
4. **Add Component** > **Circle Collider 2D**

### Adım 2: Tower Script Ayarları

Inspector'da:

```
Tower Type: ground
Current Level: 1
Can Target Air: false (işaretsiz)
Is AOE: false (işaretsiz)
```

### Adım 3: Projectile Prefab (Ok)

1. **Hierarchy** > **Create Empty** > `Arrow`
2. **Add Component** > **Sprite Renderer** > Ok sprite'ı ekle
3. **Add Component** > **Projectile** (script)
4. **Speed: 10**
5. Prefab olarak kaydet: **Assets/TowerDefense/Prefabs/Projectiles/**

### Adım 4: Tower'a Projectile Ata

1. `GroundTower` prefab'ını aç
2. **Tower** script'inde **Projectile Prefab** alanına `Arrow` prefab'ını sürükle

### Adım 5: Prefab Kaydet

**Assets/TowerDefense/Prefabs/Towers/** klasörüne sürükle

### Adım 6: Diğer Kuleleri Oluştur

- `UniversalTower` (Can Target Air: true)
- `AOETower` (Is AOE: true, AOE Radius: 2)

---

## 🗺️ 5. SCENE KURULUMU

### Adım 1: Yeni Scene Oluşturun

1. **File > New Scene** veya `Ctrl+N`
2. **2D Template** seçin
3. **File > Save As** > `Map_Grifon`

### Adım 2: GameManager Oluşturun

1. **Hierarchy** > **Create Empty** > `GameManager`
2. **Add Component** > **Game Manager** (script)
3. **Add Component** > **Wave Configurator**
4. **Add Component** > **Tower Configurator**
5. **Add Component** > **Final Boss Configurator**

Inspector'da bağlantıları yapın:
- **Wave Configurator** alanına Wave Configurator component'ini sürükle
- **Tower Configurator** alanına Tower Configurator component'ini sürükle
- **Boss Configurator** alanına Final Boss Configurator component'ini sürükle

### Adım 3: EnemySpawner Oluşturun

1. **Hierarchy** > **Create Empty** > `EnemySpawner`
2. **Add Component** > **Enemy Spawner** (script)

**Düşman prefab'larını atayın:**
- **Basic Enemy Prefab** alanına `BasicEnemy` prefab'ını sürükle
- **Fast Enemy Prefab** alanına `FastEnemy` prefab'ını sürükle
- ... tüm düşmanlar için tekrarla

### Adım 4: GameManager'a EnemySpawner'ı Bağlayın

1. `GameManager` objesini seç
2. **Game Manager** script'inde **Enemy Spawner** alanına `EnemySpawner` objesini sürükle

### Adım 5: Spawn Point Oluşturun

1. **Hierarchy** > **Create Empty** > `SpawnPoint`
2. Pozisyonunu ayarlayın (düşmanların çıkacağı yer)
3. Görsel olarak görmek için: **Add Component** > **Sprite Renderer** (küçük bir ikon)

### Adım 6: Enemy Path (Waypoints) Oluşturun

1. **Hierarchy** > **Create Empty** > `EnemyPath`
2. `EnemyPath` objesine sağ tıklayın > **Create Empty** > `Waypoint1`
3. Tekrar et: `Waypoint2`, `Waypoint3`, ... `Waypoint10`
4. Her waypoint'i harita boyunca yol oluşturacak şekilde yerleştirin

**NOT:** Waypoint'ler düşmanların takip edeceği yoldur. Son waypoint üs olacak.

### Adım 7: TowerPlacement Oluşturun

1. **Hierarchy** > **Create Empty** > `TowerPlacement`
2. **Add Component** > **Tower Placement** (script)

Kule prefab'larını atayın:
- **Ground Tower Prefab** > `GroundTower`
- **Universal Tower Prefab** > `UniversalTower`
- **AOE Tower Prefab** > `AOETower`

---

## 🎨 6. UI KURULUMU

### Adım 1: Canvas Oluşturun

1. **Hierarchy** > sağ tık > **UI > Canvas**
2. Otomatik oluşur: `Canvas` ve `EventSystem`

### Adım 2: Canvas Ayarları

Canvas seçiliyken Inspector'da:

```
Render Mode: Screen Space - Overlay
Canvas Scaler:
  - UI Scale Mode: Scale With Screen Size
  - Reference Resolution: 1920x1080
```

### Adım 3: Text Elemanları (Para, Can, Wave)

**Para Göstergesi:**

1. **Canvas**'a sağ tıklayın > **UI > Text - TextMeshPro**
   (İlk kez kullanıyorsanız "Import TMP Essentials" diyor, tıklayın)
2. İsim verin: `MoneyText`
3. Inspector'da:
   - **Text:** "Para: 100"
   - **Font Size:** 36
   - **Alignment:** Left-Top
4. **Rect Transform** ayarları:
   - **Anchors:** Top-Left
   - **Pos X:** 100, **Pos Y:** -50

Aynı şekilde oluşturun:
- `LivesText` (Can: 20)
- `WaveText` (Wave: 0/10)

### Adım 4: Start Wave Butonu

1. **Canvas** > sağ tık > **UI > Button - TextMeshPro**
2. İsim: `StartWaveButton`
3. Alt objesi `Text (TMP)` seçin > Text: "START WAVE"
4. Butonun **Rect Transform**:
   - **Anchors:** Bottom-Center
   - **Pos Y:** 100
   - **Width:** 200, **Height:** 60

### Adım 5: Kule Butonları

3 buton oluşturun:
- `GroundTowerButton` (Okçu Kulesi - 30 coin)
- `UniversalTowerButton` (Büyücü Kulesi - 50 coin)
- `AOETowerButton` (Bomba Kulesi - 40 coin)

Yan yana dizin (Alt-Right köşeye)

### Adım 6: UIManager Oluşturun

1. **Canvas**'a sağ tıklayın > **Create Empty** > `UIManager`
2. **Add Component** > **UI Manager** (script)

Inspector'da bağlantıları yapın:
- **Money Text** > `MoneyText` objesini sürükle
- **Lives Text** > `LivesText` objesini sürükle
- **Wave Text** > `WaveText` objesini sürükle
- **Start Wave Button** > `StartWaveButton` objesini sürükle
- **Ground Tower Button** > `GroundTowerButton` objesini sürükle
- **Universal Tower Button** > `UniversalTowerButton` objesini sürükle
- **AOE Tower Button** > `AOETowerButton` objesini sürükle

### Adım 7: Game Over ve Victory Panelleri (Opsiyonel)

1. **Canvas** > **UI > Panel** > `GameOverPanel`
2. Panel içine **UI > Text** > "GAME OVER"
3. **UI > Button** > "Restart"

Aynısını `VictoryPanel` için yapın.

UIManager'a bağlayın.

---

## ▶️ 7. OYUNU BAŞLATMA

### Adım 1: Harita Başlatın

1. **Hierarchy**'de **GameManager** seçin
2. **Add Component** > **Start Map Helper** adında yeni bir script ekleyin

`StartMapHelper.cs`:

```csharp
using UnityEngine;
using TowerDefense.Core;

public class StartMapHelper : MonoBehaviour
{
    void Start()
    {
        // İlk haritayı başlat (Grifon)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartMap(0);
        }
    }
}
```

Bu script oyun başladığında otomatik harita başlatır.

### Adım 2: Camera Ayarları

1. **Main Camera** seçin
2. **Projection:** Orthographic
3. **Size:** 10 (haritanıza göre ayarlayın)
4. Pozisyonu haritanın ortasına getirin

### Adım 3: Play'e Basın!

Unity Editor'de **üst ortadaki PLAY ▶️ butonuna** tıklayın.

**Konsol (Console)**'u açın: **Window > General > Console** (Ctrl+Shift+C)

Şunu görmelisiniz:
```
=== TOWER DEFENSE OYUNU BAŞLIYOR ===
✓ 3 harita yüklendi
=== Grifon'un Dağları BAŞLADI ===
```

### Adım 4: Wave Başlatın

1. **START WAVE** butonuna tıklayın (Game view'da)
2. Düşmanlar spawn olacak ve waypoint'leri takip edecek

### Adım 5: Kule Yerleştirin

1. **Okçu Kulesi** butonuna tıklayın
2. Mouse'u hareket ettirin (preview görünecek)
3. Yeşil olduğunda **sol tıklayın** (yerleştirir)
4. **Sağ tık** veya **ESC** (iptal)

---

## 🐛 8. SORUN GİDERME

### "NullReferenceException" Hatası

**Sebep:** Prefab veya obje atanmamış.

**Çözüm:**
1. Console'daki hatayı oku (hangi script, hangi satır)
2. O script'in Inspector'ındaki tüm alanları doldur

### Düşmanlar Spawn Olmuyor

**Kontrol:**
- `EnemySpawner` script'inde tüm prefab'lar atandı mı?
- `GameManager` > `Enemy Spawner` alanı dolu mu?
- `SpawnPoint` var mı?

### Düşmanlar Hareket Etmiyor

**Kontrol:**
- `EnemyPath` objesi var mı?
- Waypoint'ler `EnemyPath`'in child'ı mı?
- En az 2 waypoint var mı?

### Kuleler Saldırmıyor

**Kontrol:**
- `Tower` script'inde **Projectile Prefab** atandı mı?
- Düşmanlar kule menzilinde mi?
- `CanTargetAir` ayarı doğru mu? (uçan düşmanlar için)

### UI Çalışmıyor

**Kontrol:**
- `UIManager` script'inde tüm Text ve Button referansları atandı mı?
- `EventSystem` objesi scene'de var mı?
- Canvas ayarları doğru mu?

### Para Kazanılmıyor

**Kontrol:**
- `GameManager.Instance` null değil mi?
- `Enemy` script'inde `Die()` fonksiyonu çalışıyor mu?

---

## 🎯 9. SONRAKİ ADIMLAR

### Temel Oyun Çalıştığında:

1. **Animasyonlar Ekleyin**
   - Sprite sheet'lerle düşman yürüme animasyonu
   - Kule ateş etme animasyonu

2. **Ses Efektleri**
   - Kule saldırı sesi
   - Düşman ölüm sesi
   - UI buton sesi

3. **Particle Efektleri**
   - Düşman ölüm patlaması
   - Kule mermi izi
   - AOE patlama efekti

4. **Daha Fazla Harita**
   - Scene'leri kopyalayın
   - Farklı waypoint yolları oluşturun

5. **Ana Menü**
   - Yeni scene: `MainMenu`
   - Harita seçim ekranı
   - Ayarlar

---

## 📚 10. UNITY ÖĞRENİMİ

İlk kez Unity kullanıyorsanız:

**Önerilen Kaynaklar:**
- [Unity Learn](https://learn.unity.com/) - Resmi Unity eğitimleri (Türkçe alt yazılı)
- [Brackeys YouTube](https://www.youtube.com/@Brackeys) - Tower Defense tutorial'ları
- [Unity Manual](https://docs.unity3d.com/Manual/index.html) - Dokümantasyon

**Önemli Kavramlar:**
- **GameObject:** Oyundaki her şey (karakter, kule, buton)
- **Component:** GameObject'e eklenen özellikler (script, sprite, collider)
- **Prefab:** Yeniden kullanılabilir şablon
- **Scene:** Oyun seviyesi/menü
- **Inspector:** Obje özelliklerini düzenleme
- **Hierarchy:** Scene'deki tüm objeler
- **Project:** Tüm dosyalarınız

---

## ✅ ÖZET - YAPMALISINIZ:

1. ✅ Klasör yapısını oluşturun
2. ✅ Sprite'ları indirin ve import edin
3. ✅ Düşman prefab'ları oluşturun (10 adet)
4. ✅ Kule prefab'ları oluşturun (3 adet)
5. ✅ Mermi prefab'ları oluşturun (3 adet)
6. ✅ Scene'i kurun (GameManager, EnemySpawner, TowerPlacement)
7. ✅ EnemyPath waypoint'lerini oluşturun
8. ✅ UI Canvas'ı kurun (Text'ler, Butonlar)
9. ✅ Tüm referansları atayın (Inspector'da sürükle-bırak)
10. ✅ Play'e basın ve test edin!

---

**İyi Şanslar! 🎮**

Herhangi bir sorunla karşılaşırsanız, Console'daki hata mesajını okuyun veya bana sorun!

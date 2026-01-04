# 🎨 SORTING ORDER GUIDE

## Neden Dinamik Sorting Kullanıyoruz?

İzometrik oyunlarda **depth illüzyonu** yaratmak için karakterlerin Y pozisyonuna göre sıralaması gerekir.

### Örnek:
```
Karakter A (Y = 2.0)  ← Haritanın üstünde
Karakter B (Y = -1.0) ← Haritanın altında

Görsel olarak: B, A'nın ÖNÜNDEdir
Sorting: B'nin sorting order'ı daha YÜKSEK olmalı
```

---

## 📊 Sorting Order Mimarisi

```
0-9     Background      | Zemin, arka plan efektleri
10-19   Tilemap         | Harita, yollar (şu an: 10-13)
20-99   Characters      | Düşmanlar, kuleler - DİNAMİK
100+    UI Effects      | Can barları, damage text
```

### Characters Layer (20-99) - Dinamik Y-based

**Base Sorting:** 50 (ortada)
**Y Multiplier:** 10
**Formül:** `sorting = 50 + (-(int)(Y * 10))`

**Örnekler:**
- Y = +2.0 → sorting = 30 (arkada, ama görünür)
- Y = 0.0 → sorting = 50 (orta)
- Y = -2.0 → sorting = 70 (önde)

**Range:** 30-70 (her zaman tilemap üstünde, UI altında)

---

## 🔧 Nasıl Kullanılır?

### 1. Karakterler (Enemy, Tower)

**Otomatik:** Script'ler zaten dinamik sorting kullanıyor.

```csharp
// Enemy.cs ve Tower.cs içinde:
void UpdateSortingOrder()
{
    spriteRenderer.sortingOrder = SortingOrderConfig.GetCharacterSortingOrder(transform.position.y);
}
```

**Ne zaman güncellenir?**
- **Düşmanlar:** Her frame (hareket ettikleri için)
- **Kuleler:** Sadece Start()'ta (sabit durdukları için)

### 2. Projectile'lar (Ok, Büyü, Bomba)

**Sabit sorting:** 60 (karakterlerin biraz üstünde)

```csharp
// Projectile.cs Awake() içinde:
spriteRenderer.sortingOrder = SortingOrderConfig.PROJECTILE_BASE;
```

### 3. UI Effects (Health Bar, Damage Text)

**Gelecekte eklenecek:**
- Health Bar: sorting = 100
- Damage Text: sorting = 200

---

## 🎯 Config Dosyası

Tüm sabitler **`SortingOrderConfig.cs`** dosyasında:

```csharp
// Kullanım örnekleri:
int sorting = SortingOrderConfig.GetCharacterSortingOrder(yPosition);
int projectileSorting = SortingOrderConfig.PROJECTILE_BASE;
int healthBarSorting = SortingOrderConfig.HEALTH_BAR_BASE;
```

**Avantajlar:**
- ✅ Tek yerden merkezi kontrol
- ✅ Magic number yok
- ✅ Kolay bakım ve değişiklik
- ✅ Tüm proje için tutarlılık

---

## ⚙️ Ayarlar

### Y_POSITION_MULTIPLIER (şu an: 10)

**Ne yapar:** Y pozisyonunun sorting'e ne kadar etki edeceğini belirler.

**Düşük değer (5):**
- Depth etkisi az
- Karakterler üst üste bindiğinde az fark görünür

**Yüksek değer (20):**
- Depth etkisi çok
- Karakterler çok net ayrılır
- Ama sorting range hızla dolabilir

**Önerilen:** 10 (dengeli)

### CHARACTER_BASE_SORTING (şu an: 50)

**Ne yapar:** Karakterlerin ortalama sorting değeri.

**Neden 50?**
- Tilemap (10-19) üstünde
- UI Effects (100+) altında
- +/- 20 offset ile 30-70 arası range
- Güvenli aralık

---

## 🐛 Sorun Giderme

### Karakter görünmüyor!

**Sebep:** Sorting order negatif veya çok düşük.

**Çözüm:**
1. Prefab'da **Order in Layer: 50** olduğundan emin olun
2. `SortingOrderConfig.CHARACTER_BASE_SORTING` değerine bakın
3. Console'da sorting değerini debug edin:
   ```csharp
   Debug.Log($"Sorting: {spriteRenderer.sortingOrder}, Y: {transform.position.y}");
   ```

### Karakterler tilemap'in altında!

**Sebep:** Tilemap'in sorting'i çok yüksek.

**Çözüm:**
- Tilemap sorting: 10-13 arası tutun
- Karakter base: 50 olsun
- Minimum karakter sorting (30) bile tilemap'ten (13) yüksek

### Depth etkisi çalışmıyor!

**Sebep:** `UpdateSortingOrder()` çağrılmıyor.

**Çözüm:**
- Enemy.cs: `Update()` içinde çağrılıyor mu?
- Tower.cs: `Start()` içinde çağrılıyor mu?

---

## 📚 Profesyonel Referanslar

Bu sistem şu oyunlarda kullanılır:
- **Stardew Valley** (izometrik farming sim)
- **Hades** (isometric action)
- **Diablo** serisi (classic isometric RPG)
- **Clash of Clans** (tower defense)

**Avantajları:**
- ✅ Görsel depth illüzyonu
- ✅ Karakterler doğru sıralanır
- ✅ Performans maliyeti minimal
- ✅ Endüstri standardı

---

## ✨ Sonuç

**Dinamik Y-based sorting** profesyonel izometrik oyunlar için standart yöntemdir.

Kodunuz artık:
- ✅ Temiz ve organize (tek config dosyası)
- ✅ Profesyonelce yapılandırılmış
- ✅ Kolay bakım ve genişletme
- ✅ AAA oyun standartlarında

İyi oyunlar! 🎮

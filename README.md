# General's Oath - Tower Defense

## 🚀 Son Güncellemeler (v1.4)

Bu sürümde kamera kontrolleri, kullanıcı arayüzü optimizasyonları ve Unity 6.2 uyumluluğu üzerine odaklanıldı.

### 🎮 Kamera Zoom Sistemi
Oyun kamerası artık mouse scroll ile dinamik zoom desteğine sahip. Oyun alanını yakınlaştırıp uzaklaştırabilirsiniz.

* **Orthographic Projeksiyon:** Top-down perspektif için optimize edilmiş kamera sistemi.
* **Smooth Zoom:** 3-10 birim arası ayarlanabilir zoom seviyesi.
* **Cross-Platform Input:** Hem eski (Input Manager) hem yeni (Input System Package) Unity input sistemleriyle uyumlu.
* **HUD Sabitliği:** Canvas Screen Space - Overlay modunda çalışıyor, zoom yaparken butonlar ve UI elementleri ekranda sabit kalıyor.

### 🛡️ Kule Animasyon & Rotasyon Sistemi
Kuleler artık menzillerine giren düşmanları algılıyor ve gerçek zamanlı olarak hedefe yöneliyor.

* **Cannon (Top) Kulesi:** 8 yönlü (45 derecelik açılarla) tam rotasyon desteği. Düşman hareketine göre en yakın sprite'ı otomatik seçer.
* **Mortar (Havan) Kulesi:** 6 farklı sprite kullanarak 8 yönlü bakış açısını simüle eden özel yönlendirme algoritması.
* **Dinamik Ölçeklendirme (Scaling):** Her bakış açısı için sprite boyutları manuel olarak optimize edildi, böylece perspektif bozulmaları giderildi.
* **Gelişmiş Fizik Algılama:** `OnTrigger2D` ve `Rigidbody2D` optimizasyonları ile düşman takip sistemi daha kararlı hale getirildi.

---

## 🎯 Teknik Özellikler

### Kamera Sistemi
* **Projection:** Orthographic
* **Position:** (0, 0, -10)
* **Rotation:** (0, 0, 0) - Top-down görünüm
* **Zoom Range:** 3-10 units
* **Input Handling:** Dual-mode (Legacy + New Input System)

### UI Sistemi
* **Canvas Render Mode:** Screen Space - Overlay
* **Canvas Scaler:** Scale With Screen Size
* **Reference Resolution:** 1920x1080
* **Match Mode:** 0.5 (Width/Height balanced)

---
*Bu proje geliştirilmeye devam etmektedir. Bir sonraki aşama: Mermi ve Alan Hasarı sistemleri.*
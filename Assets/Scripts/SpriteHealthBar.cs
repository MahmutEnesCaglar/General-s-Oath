using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpriteHealthBar : MonoBehaviour
{
    [Header("Ayarlar")]
    public Image targetImage;
    public TMP_Text healthText;

    [Header("Can Görselleri")]
    public Sprite[] healthSprites; 

    [Header("Can Değerleri (Tamsayı)")]
    public int maxHealth = 5; // ARTIK TAM SAYI VE VARSAYILAN 5
    public int currentHealth; // ARTIK TAM SAYI

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthSprite();
    }

    // Test kodu kaldırıldı - Artık GameManager can sistemini yönetiyor

    // Hasar alma fonksiyonu artık int alıyor (örn: 1 hasar)
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        // Canı 0 ile 5 (maxHealth) arasına sıkıştır
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        UpdateHealthSprite();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthSprite();
    }

    // Public yaptık ki GameManager erişebilsin
    public void UpdateHealthSprite()
    {
        // --- Yazı Güncelleme ---
        if (healthText != null)
        {
            // Ekranda "5 / 5", "4 / 5" gibi görünecek
            healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        }

        if (targetImage == null || healthSprites.Length == 0) return;

        // --- Sprite Hesaplama ---
        // int'i int'e bölünce sonuç virgüllü çıkmaz (örn 4/5 = 0 olur).
        // Bu yüzden başına (float) koyarak virgüllü bölme yapıyoruz.
        float healthPercentage = (float)currentHealth / maxHealth;

        int spriteIndex = 0;

        // 4 Parça resim için yüzdelik dilimler
        if (healthPercentage >= 0.8f)      spriteIndex = 0; // 5 ve 4 Can (Full gibi)
        else if (healthPercentage >= 0.5f) spriteIndex = 1; // 3 Can
        else if (healthPercentage >= 0.2f) spriteIndex = 2; // 2 ve 1 Can
        else                               spriteIndex = 3; // 0 Can (Ölü)

        // Diziden dışarı taşmayı önle
        spriteIndex = Mathf.Clamp(spriteIndex, 0, healthSprites.Length - 1);
        targetImage.sprite = healthSprites[spriteIndex];
    }
}
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Can Barı Sprite'ları")]
    public Sprite Can5; // %100 - Full can
    public Sprite Can4; // %80
    public Sprite Can3; // %60
    public Sprite Can2; // %40
    public Sprite Can1; // %20
    public Sprite Can0; // %0 - Ölü
    
    [Header("Can Değerleri")]
    public int maxHealth = 5;
    public int currentHealth = 5;
    
    private Image healthImage;
    
    void Start()
    {
        healthImage = GetComponent<Image>();
        UpdateHealth();
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealth();
    }
    
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealth();
    }
    
    public void UpdateHealth()
    {
        if (healthImage == null) return;
        
        // Can sayısına göre sprite seç (TERS SIRALAMA)
        Sprite selectedSprite = null;
        
        switch (currentHealth)
        {
            case 5: selectedSprite = Can0; break; // %100 - Full
            case 4: selectedSprite = Can1; break; // %80
            case 3: selectedSprite = Can2; break; // %60
            case 2: selectedSprite = Can3; break; // %40
            case 1: selectedSprite = Can4; break; // %20
            default: selectedSprite = Can5; break; // %0 - Ölü
        }
        
        if (selectedSprite != null)
            healthImage.sprite = selectedSprite;
    }
}
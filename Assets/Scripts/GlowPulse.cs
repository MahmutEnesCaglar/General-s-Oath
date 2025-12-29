using UnityEngine;
using UnityEngine.UI;

public class PhoenixGlowEffect : MonoBehaviour
{
    [Header("Glow Ayarları")]
    public float pulseSpeed = 4f;
    public float minAlpha = 0.1f;
    public float maxAlpha = 1f;
    
    private Image glowImage;
    private Color baseColor = Color.white;
    
    void Start()
    {
        glowImage = GetComponent<Image>();
        baseColor = Color.white;
    }
    
    void Update()
    {
        // Sinüs dalgası ile alpha animasyonu
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
        
        baseColor.a = alpha;
        glowImage.color = baseColor;
    }
}
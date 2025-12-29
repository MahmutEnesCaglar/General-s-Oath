using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderFeather : MonoBehaviour
{
    [Header("Slider")]
    public Slider volumeSlider;
    public Image Ses;
    
    [Header("Tüy Sprite'ları (0% → 100%)")]
    public Sprite Ses_5;  // %0 - sönük
    public Sprite Ses_4;  // %20
    public Sprite Ses_3;  // %40
    public Sprite Ses_2;  // %60
    public Sprite Ses_1;  // %80
    public Sprite Ses_0; // %100 - ateşli
    
    void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            OnVolumeChanged(volumeSlider.value); // Başlangıç değeri
        }
    }
    
    void OnVolumeChanged(float value)
    {
        // Ses seviyesini ayarla (MainMenuController üzerinden)
        MainMenuController menuController = FindFirstObjectByType<MainMenuController>();
        if (menuController != null)
            menuController.SetVolume(value);
        
        // Tüy sprite'ını değiştir
        UpdateFeatherSprite(value);
    }
    
    void UpdateFeatherSprite(float value)
    {
        if (Ses == null) return;
        
        // %0-20-40-60-80-100 aralıklarına göre sprite seç
        Sprite selectedSprite = null;
        
        if (value <= 0.1f)
            selectedSprite = Ses_5;
        else if (value <= 0.3f)
            selectedSprite = Ses_4;
        else if (value <= 0.5f)
            selectedSprite = Ses_3;
        else if (value <= 0.7f)
            selectedSprite = Ses_2;
        else if (value <= 0.9f)
            selectedSprite = Ses_1;
        else
            selectedSprite = Ses_0;
        
        if (selectedSprite != null)
            Ses.sprite = selectedSprite;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class MusicSliderFeather : MonoBehaviour
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
    public Sprite Ses_0;  // %100 - ateşli
    
    void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            OnVolumeChanged(volumeSlider.value);
        }
    }
    
    void OnVolumeChanged(float value)
    {
        // Sadece müzik sesini ayarla
        MainMenuController menuController = FindFirstObjectByType<MainMenuController>();
        if (menuController != null)
        {
            menuController.SetMusicVolume(value);
        }
        
        UpdateFeatherSprite(value);
    }
    
    void UpdateFeatherSprite(float value)
    {
        if (Ses == null) return;
        
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
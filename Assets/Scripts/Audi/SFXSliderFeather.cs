using UnityEngine;
using UnityEngine.UI;

public class SFXSliderFeather : MonoBehaviour
{
    [Header("Slider")]
    public Slider volumeSlider;
    public Image Ses;
    
    [Header("Tüy Sprite'ları, Kırmızı ve beyaz")]
    public Sprite beyazTuy;  // %0
    public Sprite kirmiziTuy;  // %bir şey
    
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
        // SFXManager üzerinden güncelle (MainMenuController'a bağımlı kalmadan)
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.SetSFXVolume(value);
        }
        else
        {
            // Manager yoksa bile PlayerPrefs'e kaydet
            PlayerPrefs.SetFloat("SFXVolume", value);
            PlayerPrefs.Save();
        }
        
        UpdateFeatherSprite(value);
    }
    
    void UpdateFeatherSprite(float value)
    {
        if (Ses == null) return;
        
        Sprite selectedSprite = null;
        
        if (value == 0.0f)
            selectedSprite = beyazTuy;
        else
            selectedSprite = kirmiziTuy;
        
        if (selectedSprite != null)
            Ses.sprite = selectedSprite;
    }
}
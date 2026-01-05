using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }
    
    [Header("SFX Volume")]
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Eğer bu obje bir başkasının altındaysa, onu en tepeye taşı (Root yap)
            // Çünkü DontDestroyOnLoad sadece Root objelerde çalışır.
            transform.SetParent(null); 
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
        
        Debug.Log($"SFX Volume: {sfxVolume:F2}");
        
        // ← KENDİNİ ÇAĞIRMA! Recursive call silindi
    }
    
    /// <summary>
    /// Statik metod: Her yerden erişilebilir, Instance yoksa PlayerPrefs'ten okur.
    /// </summary>
    public static void PlaySound(AudioClip clip, AudioSource source)
    {
        if (clip == null || source == null) return;

        float volume = 1f;
        
        if (Instance != null)
        {
            volume = Instance.sfxVolume;
        }
        else
        {
            // Instance yoksa (örn. direkt GameScene'den başlandıysa) kayıtlı ayarı oku
            volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        }
        
        source.PlayOneShot(clip, volume);
    }

    public void PlaySFX(AudioClip clip, AudioSource source)
    {
        PlaySound(clip, source);
    }
}
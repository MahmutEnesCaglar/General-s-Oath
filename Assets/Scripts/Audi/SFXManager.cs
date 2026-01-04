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
    
    public void PlaySFX(AudioClip clip, AudioSource source)
    {
        if (clip != null && source != null)
        {
            source.PlayOneShot(clip, sfxVolume);
        }
    }
}
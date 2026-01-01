using UnityEngine;
using UnityEngine.EventSystems;

public class CloseButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Ses Efektleri")]
    public AudioClip hoverSound;
    public AudioClip clickSound;
    [Range(0f, 1f)]
    public float volume = 1f;
    
    private AudioSource audioSource;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null && audioSource != null && SFXManager.Instance != null)
            SFXManager.Instance.PlaySFX(hoverSound, audioSource);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null && audioSource != null && SFXManager.Instance != null)
            SFXManager.Instance.PlaySFX(clickSound, audioSource);
    }
}

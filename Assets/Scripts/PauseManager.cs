using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [Header("Pause UI")]
    public GameObject pausePanel;
    public Button pauseButton;
    
    [Header("Pause Button Görselleri")]
    public Sprite pauseNormalSprite;
    public Sprite pauseActiveSprite;
    
    [Header("Panel İçi Butonlar")]
    public Button homeButton;
    public Button resumeButton;
    
    [Header("Ses Ayarı")]
    public Slider volumeSlider;
    public TextMeshProUGUI volumeText;
    public Button muteButton;
    
    [Header("Ses İkonları - 4 Seviye")]
    public Sprite soundMuteSprite;      // 0% - Sessiz
    public Sprite soundLowSprite;       // 1-33% - 1 dalga
    public Sprite soundMediumSprite;    // 34-70% - 2 dalga
    public Sprite soundHighSprite;      // 71-100% - 3 dalga
    
    [Header("Kamera Kontrolü")]  // ← BU SATIRI EKLE
    public CameraZoom cameraZoom; // ← BU SATIRI EKLE

    private bool isPaused = false;
    private bool isMuted = false;
    private float volumeBeforeMute = 1f;
    private Image pauseButtonImage;
    private Image muteButtonImage;

    void Awake()
    {
        // Kaydedilmiş ses seviyesini yükle
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("MasterVolume");
            AudioListener.volume = savedVolume;
        }
    }

    void Start()
    {
        // Pause button image
        if (pauseButton != null)
            pauseButtonImage = pauseButton.GetComponent<Image>();
        
        // Mute button image
        if (muteButton != null)
            muteButtonImage = muteButton.GetComponent<Image>();
        
        // Panel'i başlangıçta gizle
        if (pausePanel != null)
            pausePanel.SetActive(false);
        
        // Buton event'lerini bağla
        if (pauseButton != null)
            pauseButton.onClick.AddListener(TogglePause);
        
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
        
        if (homeButton != null)
            homeButton.onClick.AddListener(ReturnToMainMenu);
        
        if (muteButton != null)
            muteButton.onClick.AddListener(ToggleMute);
        
        // Volume slider event
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(SetVolume);
            volumeSlider.value = AudioListener.volume;
        }
        
        // İlk ses seviyesini göster
        UpdateVolumeText(AudioListener.volume);
        UpdateSoundIcon(AudioListener.volume);
        
        Debug.Log("PauseManager başlatıldı");
    }

    void Update()
    {
        // M tuşu ile mute toggle (sadece pause açıkken) - Opsiyonel
        if (isPaused && Input.GetKeyDown(KeyCode.M))
        {
            ToggleMute();
        }
    }

    /// <summary>
    /// Pause durumunu aç/kapat (toggle)
    /// </summary>
    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    /// <summary>
    /// Oyunu duraklat
    /// </summary>
    void PauseGame()
    {
        isPaused = true;
        
        if (pausePanel != null)
            pausePanel.SetActive(true);
        
        Time.timeScale = 0f;
        
        if (pauseButtonImage != null && pauseActiveSprite != null)
            pauseButtonImage.sprite = pauseActiveSprite;
        
        // Tüm kamera kontrollerini kapat (zoom + hareket)
        if (cameraZoom != null)
            cameraZoom.SetCameraControlsEnabled(false);
        
        Debug.Log("Oyun duraklatıldı - Kamera kontrolleri kapatıldı");
    }

    /// <summary>
    /// Oyunu devam ettir (Resume)
    /// </summary>
    public void ResumeGame()
    {
        isPaused = false;
        
        if (pausePanel != null)
            pausePanel.SetActive(false);
        
        Time.timeScale = 1f;
        
        if (pauseButtonImage != null && pauseNormalSprite != null)
            pauseButtonImage.sprite = pauseNormalSprite;
        
        // Tüm kamera kontrollerini aç
        if (cameraZoom != null)
            cameraZoom.SetCameraControlsEnabled(true);
        
        Debug.Log("Oyun devam ediyor - Kamera kontrolleri açıldı");
    }

    /// <summary>
    /// Ses seviyesini ayarla
    /// </summary>
    public void SetVolume(float volume)
    {
        // Global ses (SFX için)
        AudioListener.volume = volume;
        
        // Müzik için
        MusicManager musicManager = FindFirstObjectByType<MusicManager>();
        if (musicManager != null)
        {
            musicManager.SetMusicVolume(volume);
        }
        
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
        
        UpdateVolumeText(volume);
        UpdateSoundIcon(volume);
        
        if (volume > 0.01f && isMuted)
        {
            isMuted = false;
            volumeBeforeMute = volume;
            Debug.Log("Mute otomatik kaldırıldı (slider hareket etti)");
        }
        
        if (volume < 0.01f && !isMuted)
        {
            isMuted = true;
        }
        
        Debug.Log($"Ses seviyesi: {volume:F2} ({Mathf.RoundToInt(volume * 100)}%)");
    }

    /// <summary>
    /// Ses seviyesi yazısını güncelle (handle içindeki sayı)
    /// </summary>
    void UpdateVolumeText(float volume)
    {
        if (volumeText != null)
        {
            int percentage = Mathf.RoundToInt(volume * 100);
            volumeText.text = percentage.ToString();
            
            // Renk değiştir
            if (percentage == 0)
                volumeText.color = Color.red;
            else if (percentage < 30)
                volumeText.color = Color.yellow;
            else
                volumeText.color = Color.white;
        }
    }

    /// <summary>
    /// Ses ikonunu ses seviyesine göre güncelle
    /// 0% = Sessiz (mute)
    /// 1-33% = Düşük (1 dalga)
    /// 34-70% = Orta (2 dalga)
    /// 71-100% = Yüksek (3 dalga)
    /// </summary>
    void UpdateSoundIcon(float volume)
    {
        if (muteButtonImage == null)
            return;

        int percentage = Mathf.RoundToInt(volume * 100);

        if (percentage == 0)
        {
            // Sessiz
            if (soundMuteSprite != null)
                muteButtonImage.sprite = soundMuteSprite;
            
            // Mute durumunda butonu kırmızımsı yap (opsiyonel)
            muteButtonImage.color = new Color(1f, 0.7f, 0.7f);
        }
        else if (percentage >= 1 && percentage <= 33)
        {
            // Düşük - 1 dalga
            if (soundLowSprite != null)
                muteButtonImage.sprite = soundLowSprite;
            
            muteButtonImage.color = Color.white;
        }
        else if (percentage >= 34 && percentage <= 70)
        {
            // Orta - 2 dalga
            if (soundMediumSprite != null)
                muteButtonImage.sprite = soundMediumSprite;
            
            muteButtonImage.color = Color.white;
        }
        else // 71-100
        {
            // Yüksek - 3 dalga
            if (soundHighSprite != null)
                muteButtonImage.sprite = soundHighSprite;
            
            muteButtonImage.color = Color.white;
        }
    }

    /// <summary>
    /// Sessiz (Mute) modunu aç/kapat - Toggle
    /// Bir kez tıkla: Ses 0'a gider
    /// İkinci tıkla: Eski ses seviyesine döner
    /// </summary>
    public void ToggleMute()
    {
        if (isMuted)
        {
            // Sesi AÇ - eski seviyeye dön
            isMuted = false;
            
            // Eğer önceki ses seviyesi 0 idiyse, varsayılan olarak %50'ye ayarla
            if (volumeBeforeMute < 0.01f)
                volumeBeforeMute = 0.5f;
            
            AudioListener.volume = volumeBeforeMute;
            
            if (volumeSlider != null)
                volumeSlider.value = volumeBeforeMute;
            
            Debug.Log($"Ses açıldı: {Mathf.RoundToInt(volumeBeforeMute * 100)}%");
        }
        else
        {
            // Sesi KAPAT - 0'a ayarla
            isMuted = true;
            
            // Mevcut ses seviyesini kaydet (0 değilse)
            if (AudioListener.volume > 0.01f)
                volumeBeforeMute = AudioListener.volume;
            else
                volumeBeforeMute = 0.5f; // Varsayılan
            
            AudioListener.volume = 0f;
            
            if (volumeSlider != null)
                volumeSlider.value = 0f;
            
            Debug.Log("Ses kapatıldı (mute)");
        }
        
        UpdateVolumeText(AudioListener.volume);
        UpdateSoundIcon(AudioListener.volume);
    }

    /// <summary>
    /// Ana menüye dön (Home butonu)
    /// </summary>
    public void ReturnToMainMenu()
    {
        // Oyun hızını normale çevir (yoksa MainMenu'de de pause kalır)
        Time.timeScale = 1f;
        
        Debug.Log("Ana menüye dönülüyor...");
        
        // MainMenu scene'ini yükle
        SceneManager.LoadScene(0); // veya "GameScene"

    }
}
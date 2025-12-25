using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    
    [Header("Müzik Klipleri")]
    public AudioClip mainMenuMusic;
    public AudioClip gameMusic;
    
    [Header("Ayarlar")]
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    
    private AudioSource audioSource;

    void Awake()
    {
        // Singleton - sadece bir instance olsun
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            audioSource = GetComponent<AudioSource>();
            
            // Kaydedilmiş ses seviyesini yükle
            if (PlayerPrefs.HasKey("MasterVolume"))
            {
                musicVolume = PlayerPrefs.GetFloat("MasterVolume");
                audioSource.volume = musicVolume;
            }
            
            // Scene değişikliklerini dinle
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            Debug.Log("MusicManager oluşturuldu");
        }
        else
        {
            // Başka bir MusicManager varsa yok et
            Debug.Log("Duplicate MusicManager silindi");
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // İlk scene için müziği başlat
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene yüklendi: {scene.name}");
        PlayMusicForScene(scene.name);
    }

    void PlayMusicForScene(string sceneName)
    {
        AudioClip clipToPlay = null;
        
        if (sceneName == "MainMenuSahne")
        {
            clipToPlay = mainMenuMusic;
            Debug.Log("MainMenu müziği seçildi");
        }
        else if (sceneName == "GameScene")
        {
            clipToPlay = gameMusic;
            Debug.Log("GameScene müziği seçildi");
        }
        
        // Müzik değiştir
        if (clipToPlay != null)
        {
            if (audioSource.clip != clipToPlay)
            {
                audioSource.clip = clipToPlay;
                audioSource.volume = musicVolume;
                audioSource.loop = true;
                audioSource.Play();
                Debug.Log($"Müzik çalıyor: {clipToPlay.name}");
            }
        }
        else
        {
            Debug.LogWarning($"Scene '{sceneName}' için müzik bulunamadı!");
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (audioSource != null)
        {
            audioSource.volume = volume;
            Debug.Log($"Müzik seviyesi: {volume:F2}");
        }
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}

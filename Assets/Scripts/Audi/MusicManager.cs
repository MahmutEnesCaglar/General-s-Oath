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
            
            // Eğer Canvas altındaysa DontDestroyOnLoad çalışmaz. Root'a taşıyoruz.
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            
            audioSource = GetComponent<AudioSource>();
            
            // Kaydedilmiş ses seviyesini yükle
            if (PlayerPrefs.HasKey("MusicVolume"))
            {
                musicVolume = PlayerPrefs.GetFloat("MusicVolume");
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
        
        // MainMenu veya WorldMap sahnelerinde ana menü müziği çalsın
        if (sceneName == "MainMenuSahne" || sceneName == "WorldMap")
        {
            clipToPlay = mainMenuMusic;
            Debug.Log("MainMenu/WorldMap müziği seçildi");
        }
        else if (sceneName == "Map_Grifon")
        {
            clipToPlay = gameMusic;
            Debug.Log("Map_Grifon oyun müziği seçildi");
        }
        
        // Müzik değiştir
        if (clipToPlay != null)
        {
            // Eğer müzik farklıysa VEYA şu an çalmıyorsa (örn. ilk açılışta)
            if (audioSource.clip != clipToPlay || !audioSource.isPlaying)
            {
                audioSource.clip = clipToPlay;
                audioSource.volume = musicVolume;
                audioSource.loop = true;
                
                // Eğer AudioSource disable edilmişse (kapalıysa) aç
                if (!audioSource.enabled) 
                {
                    audioSource.enabled = true;
                    Debug.Log("AudioSource kapalıydı, açıldı.");
                }

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

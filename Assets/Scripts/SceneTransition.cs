using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }
    
    [Header("Transition Ayarları")]
    public Animator cloudAnimator;
    public float transitionDuration = 1f;
    
    private bool isTransitioning = false;
    private bool shouldSlideOut = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Sahne yüklendiğinde dinle
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            // Kopya instance oluşursa hemen yok et
            Destroy(gameObject);
            return;
        }
    }
    
    void OnDestroy()
    {
        // EĞER bu obje asıl Instance ise aboneliği kaldır.
        // Yoksa (yani bu bir kopyaysa), sakın asıl Instance'ın aboneliğini bozma.
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"OnSceneLoaded tetiklendi: {scene.name}, shouldSlideOut: {shouldSlideOut}");
        
        if (shouldSlideOut)
        {
            shouldSlideOut = false;
            StartCoroutine(SlideOutAfterDelay());
        }
    }
    
    IEnumerator SlideOutAfterDelay()
    {
        // Sahne tam oturana kadar minik bir bekleme
        yield return new WaitForSecondsRealtime(0.2f);
        
        Debug.Log("3. SlideOut başlatılıyor...");
        if (cloudAnimator != null)
        {
            cloudAnimator.SetTrigger("SlideOut");
        }
        else
        {
            Debug.LogError("HATA: cloudAnimator referansı kayıp! Canvas yok olmuş olabilir.");
        }
        
        yield return new WaitForSecondsRealtime(transitionDuration);
        
        Debug.Log("4. Transition tamamlandı!");
        isTransitioning = false;
    }
    
    // ... LoadScene ve TransitionToScene fonksiyonların aynı kalabilir ...
    public void LoadScene(string sceneName)
    {
        if (!isTransitioning)
            StartCoroutine(TransitionToScene(sceneName));
    }

    public void LoadScene(int sceneIndex)
    {
        if (!isTransitioning)
            StartCoroutine(TransitionToScene(sceneIndex));
    }

    IEnumerator TransitionToScene(string sceneName)
    {
        isTransitioning = true;
        if (cloudAnimator != null) cloudAnimator.SetTrigger("SlideIn");
        yield return new WaitForSecondsRealtime(transitionDuration);
        
        shouldSlideOut = true;
        SceneManager.LoadScene(sceneName);
    }
    
    IEnumerator TransitionToScene(int sceneIndex)
    {
        isTransitioning = true;
        if (cloudAnimator != null) cloudAnimator.SetTrigger("SlideIn");
        yield return new WaitForSecondsRealtime(transitionDuration);
        
        shouldSlideOut = true;
        SceneManager.LoadScene(sceneIndex);
    }
}
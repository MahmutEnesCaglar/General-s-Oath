using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    [Header("Para Ayarları")]
    public int currentMoney = 1000;
    
    [Header("UI Referansları")]
    public TMP_Text moneyText;
    
    [Header("Ses Efektleri")]
    public AudioClip moneySpentSound;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    
    private AudioSource audioSource;
    
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = sfxVolume;
        
        UpdateMoneyUI();
    }
    
    void Update()
    {
        // Test: Space ile para azalt
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpendMoney(50);
        }
    }
    
    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateMoneyUI();
            PlayMoneySound();
            Debug.Log($"Para harcandı: -{amount}. Kalan: {currentMoney}");
            return true;
        }
        else
        {
            Debug.Log("Yetersiz para!");
            return false;
        }
    }
    
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyUI();
        Debug.Log($"Para kazanıldı: +{amount}. Toplam: {currentMoney}");
    }
    
    void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = currentMoney.ToString();
        }
    }
    
    void PlayMoneySound()
    {
        if (moneySpentSound != null && audioSource != null && SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySFX(moneySpentSound, audioSource);
        }
    }
}
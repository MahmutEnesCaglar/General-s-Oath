using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [Header("Para Ayarları")]
    public int currentMoney = 100;  // Başlangıç parası

    [Header("UI Referansları")]
    public TMP_Text moneyText;

    [Header("Ses Efektleri")]
    public AudioClip moneySpentSound;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

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
            int oldMoney = currentMoney;
            currentMoney -= amount;
            UpdateMoneyUI();
            PlayMoneySound();
            Debug.Log($"<color=red>[MoneyManager] Para harcandı: {oldMoney} - {amount} = {currentMoney}</color>");
            return true;
        }
        else
        {
            Debug.Log($"<color=yellow>[MoneyManager] Yetersiz para! İhtiyaç: {amount}, Mevcut: {currentMoney}</color>");
            return false;
        }
    }
    
    public void AddMoney(int amount)
    {
        int oldMoney = currentMoney;
        currentMoney += amount;
        UpdateMoneyUI();
        Debug.Log($"<color=green>[MoneyManager] Para kazanıldı: {oldMoney} + {amount} = {currentMoney}</color>");
    }
    
    public void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = currentMoney.ToString();
        }
    }
    
    void PlayMoneySound()
    {
        if (moneySpentSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(moneySpentSound, sfxVolume);
        }
    }
}
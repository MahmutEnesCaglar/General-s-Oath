using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    private void Awake()
    {
        // Her sahne için yeni bir MoneyManager instance'ı oluştur
        // (Çünkü UI elemanları her sahnede farklı)
        if (Instance != null && Instance != this)
        {
            Debug.Log("[MoneyManager] Eski instance temizleniyor, yeni instance oluşturuluyor.");
        }
        Instance = this;
        Debug.Log("[MoneyManager] Instance oluşturuldu.");
    }

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
        // AudioSource'u oluştur
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = sfxVolume;
        
        // UI referansını yeniden bul (eğer null ise)
        if (moneyText == null)
        {
            moneyText = GameObject.Find("MoneyText")?.GetComponent<TMP_Text>();
            if (moneyText == null)
            {
                Debug.LogWarning("[MoneyManager] MoneyText bulunamadı! UI'da 'MoneyText' isimli bir TMP_Text objesi olmalı.");
            }
        }
        
        // GameManager ile senkronize ol
        if (TowerDefense.Core.GameManager.Instance != null)
        {
            currentMoney = TowerDefense.Core.GameManager.Instance.playerMoney;
            Debug.Log($"[MoneyManager] GameManager ile senkronize edildi. Para: {currentMoney}");
        }
        
        UpdateMoneyUI();
        Debug.Log("[MoneyManager] Start tamamlandı.");
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
    
    public void UpdateMoneyUI()
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
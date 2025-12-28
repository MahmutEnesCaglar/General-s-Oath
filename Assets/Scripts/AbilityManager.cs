using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TowerDefense.Hero;
using TowerDefense.Tower;
using TowerDefense.Core;
using TowerDefense.Environment; // BarrierPlacementManager'a erişmek için bunu ekledik!

public class AbilityManager : MonoBehaviour 
{
    [System.Serializable]
    public class AbilityData {
        public string name;
        public Button button;
        public Image fillImage; 
        public float cooldown = 10f;
        public int maxUsage = 5;
        
        [HideInInspector] public int remainingUsage;
        [HideInInspector] public float currentCooldown;
        [HideInInspector] public bool isOnCooldown;
    }

    [Header("Yetenek Ayarları")]
    public AbilityData rage;
    public AbilityData heal;
    public AbilityData attack;
    public AbilityData barrier;

    [Header("Rage Ayarları")]
    public float rageDuration = 5f;
    public float rageDamageMult = 2.0f;
    public float rageSpeedMult = 1.5f;

    void Start() {
        InitAbility(rage);
        InitAbility(heal);
        InitAbility(attack);
        InitAbility(barrier);
    }

    void InitAbility(AbilityData data) {
        data.remainingUsage = data.maxUsage;
        if (data.fillImage != null) data.fillImage.fillAmount = 0; 
    }

    void Update() {
        HandleCooldown(rage);
        HandleCooldown(heal);
        HandleCooldown(attack);
        HandleCooldown(barrier);
    }

    void HandleCooldown(AbilityData data) {
        if (data.isOnCooldown) {
            data.currentCooldown -= Time.deltaTime;
            
            if (data.fillImage != null) {
                data.fillImage.fillAmount = (data.cooldown - data.currentCooldown) / data.cooldown;
            }

            if (data.currentCooldown <= 0) {
                data.isOnCooldown = false;
                if (data.fillImage != null) data.fillImage.fillAmount = 0; 
                if (data.remainingUsage > 0 && data.button != null) data.button.interactable = true;
            }
        }
    }

    private void ExecuteAbility(AbilityData data) {
        if (data.remainingUsage > 0 && !data.isOnCooldown) {
            data.remainingUsage--;
            data.isOnCooldown = true;
            data.currentCooldown = data.cooldown;
            
            if(data.button != null) data.button.interactable = false;
            if (data.fillImage != null) data.fillImage.fillAmount = 0;
            
            Debug.Log($"{data.name} Kullanıldı! Kalan hak: {data.remainingUsage}");
        }
    }

    // --- BUTON FONKSİYONLARI ---
    
    // 1. RAGE
    public void UseRage()
    {
        if (rage.isOnCooldown || rage.remainingUsage <= 0) return;

        StartCoroutine(RageRoutine());
        ExecuteAbility(rage);
    }

    private IEnumerator RageRoutine()
    {
        Tower[] allTowers = FindObjectsByType<Tower>(FindObjectsSortMode.None);

        if (allTowers.Length == 0)
        {
            Debug.Log("Sahnede hiç kule yok, Rage boşa gitti!");
            yield break;
        }

        Debug.Log($"<color=red>RAGE BAŞLADI! {allTowers.Length} kule güçlendi.</color>");

        foreach (Tower tower in allTowers)
        {
            tower.EnableRage(rageDamageMult, rageSpeedMult);
        }

        yield return new WaitForSeconds(rageDuration);

        foreach (Tower tower in allTowers)
        {
            if (tower != null) 
                tower.DisableRage();
        }
    }

    // 2. HEAL
    public void UseHeal()
    {
        if (heal.isOnCooldown || heal.remainingUsage <= 0) return;

        Hero heroScript = FindFirstObjectByType<Hero>(); 

        if (heroScript != null)
        {
            heroScript.HealPercentage(0.5f);
            ExecuteAbility(heal);
            Debug.Log("<color=green>HEAL BASILDI! Hero iyileştirildi.</color>");
        }
        else
        {
            Debug.LogError("Heal basıldı ama sahnede 'Hero' bulunamadı!");
        }
    }

    // 3. BARRIER (GÜNCELLENDİ)
    public void UseBarrier()
    {
        if (barrier.isOnCooldown || barrier.remainingUsage <= 0) return;

        // BarrierPlacementManager sahne üzerinde var mı kontrol et
        if (BarrierPlacementManager.Instance != null)
        {
            Debug.Log("<color=blue>BARİYER MODU BAŞLATILDI!</color>");
            
            // Yerleştirme modunu aç
            BarrierPlacementManager.Instance.StartPlacement();
            
            // Yeteneği harca ve cooldown başlat
            // (İstersen bunu sadece başarılı yerleştirme olursa düşecek şekilde ileride güncelleyebiliriz)
            ExecuteAbility(barrier);
        }
        else
        {
            Debug.LogError("BarrierPlacementManager sahnede bulunamadı! GameManager'a ekledin mi?");
        }
    }
    
    // AbilityManager.cs içindeki UseAttack fonksiyonu
    public void UseAttack() 
    {
        if (attack.isOnCooldown || attack.remainingUsage <= 0) return;

        if (AttackManager.Instance != null)
        {
            Debug.Log("<color=orange>METEOR SALDIRISI HAZIRLANIYOR!</color>");
            AttackManager.Instance.StartAttackMode();
            
            // Kullanım hakkını burada düşüyoruz
            ExecuteAbility(attack);
        }
        else
        {
            Debug.LogError("AttackManager bulunamadı!");
        }
    }
}
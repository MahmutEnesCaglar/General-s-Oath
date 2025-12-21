using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AbilityManager : MonoBehaviour 
{
    [System.Serializable]
    public class AbilityData {
        public string name;
        public Button button;
        public Image fillImage; // Yavaşça dolacak olan görsel
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

    void Start() {
        InitAbility(rage);
        InitAbility(heal);
        InitAbility(attack);
        InitAbility(barrier);
    }

    void InitAbility(AbilityData data) {
        data.remainingUsage = data.maxUsage;
        // Oyun başında cooldown barı boş (0) olsun
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
            
            // FORMÜL DEĞİŞTİ: (Toplam Süre - Kalan Süre) / Toplam Süre
            // Bu sayede 0'dan başlayıp 1'e doğru yavaşça artar.
            if (data.fillImage != null) {
                data.fillImage.fillAmount = (data.cooldown - data.currentCooldown) / data.cooldown;
            }

            if (data.currentCooldown <= 0) {
                data.isOnCooldown = false;
                // Cooldown bittiğinde barı sıfırla (veya 1'de bırak, tasarımına göre)
                // Eğer "dolunca parlasın/açılsın" istiyorsan burayı 1 yapabilirsin.
                if (data.fillImage != null) data.fillImage.fillAmount = 0; 
                
                if (data.remainingUsage > 0) data.button.interactable = true;
            }
        }
    }

    private void ExecuteAbility(AbilityData data) {
        if (data.remainingUsage > 0 && !data.isOnCooldown) {
            data.remainingUsage--;
            data.isOnCooldown = true;
            data.currentCooldown = data.cooldown;
            data.button.interactable = false;
            
            // Yetenek kullanıldığı an görseli sıfırla (0 yap)
            if (data.fillImage != null) data.fillImage.fillAmount = 0;
            
            Debug.Log($"{data.name} Kullanıldı!");
        }
    }

    // --- BUTONLARA BAĞLANACAK FONKSİYONLAR ---
    public void UseRage() => UseAbilityLogic(rage, "<color=red>RAGE AKTİF!</color>");
    public void UseHeal() => UseAbilityLogic(heal, "<color=green>HEAL KULLANILDI!</color>");
    public void UseAttack() => UseAbilityLogic(attack, "<color=orange>SALDIRI MODU!</color>");
    public void UseBarrier() => UseAbilityLogic(barrier, "<color=blue>BARİYER MODU!</color>");

    // Kod tekrarını önlemek için küçük bir yardımcı fonksiyon
    private void UseAbilityLogic(AbilityData data, string logMessage) {
        if (data.remainingUsage > 0 && !data.isOnCooldown) {
            ExecuteAbility(data);
            Debug.Log(logMessage);
        }
    }
}
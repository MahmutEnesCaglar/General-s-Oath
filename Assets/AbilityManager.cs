using UnityEngine;
using System.Collections;

public class AbilityManager : MonoBehaviour 
{
    [Header("Yetenek Ayarları")]
    public float rageDuration = 5f;
    public float healAmountPercent = 0.5f;

    // 1- RAGE (ÖFKE)
    public void UseRage() 
    {
        Debug.Log("<color=red>RAGE AKTİF!</color> Tüm kuleler güçlendi.");
        // İleride buraya kuleleri bulma kodu gelecek
    }

    // 2- HEAL (İYİLEŞTİRME)
    public void UseHeal() 
    {
        Debug.Log("<color=green>HEAL KULLANILDI!</color> Herolar iyileşiyor.");
        // İleride buraya heroları bulma kodu gelecek
    }

    // 3- ATTACK (ALAN HASARI)
    public void UseAttack() 
    {
        Debug.Log("<color=orange>SALDIRI MODU!</color> Bir bölge seçin.");
        // İleride buraya tıklanan yere patlama koyma kodu gelecek
    }

    // 4- BARRIER (BARİYER)
    public void UseBarrier() 
    {
        Debug.Log("<color=blue>BARİYER MODU!</color> Engel yerleştirin.");
        // İleride buraya bariyer koyma kodu gelecek
    }
}
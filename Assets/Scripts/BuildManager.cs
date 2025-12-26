using UnityEngine;

public class BuildManager : MonoBehaviour
{
    // Singleton yapısı (Her yerden erişebilmek için)
    public static BuildManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Sahnede birden fazla BuildManager var!");
            return;
        }
        instance = this;
    }

    [Header("Kule Prefabları")]
    public GameObject cannonTowerPrefab;
    public GameObject archerTowerPrefab;
    public GameObject mortarTowerPrefab;

    // Şu an inşa etmek için seçili olan kule
    private GameObject towerToBuild;

    void Start()
    {
        // Oyun başlarken varsayılan olarak Cannon seçili olsun (Test için)
        towerToBuild = cannonTowerPrefab;
    }

    public GameObject GetTowerToBuild()
    {
        return towerToBuild;
    }

    // UI butonları bu fonksiyonu çağıracak
    public void SelectTower(GameObject tower)
    {
        towerToBuild = tower;
    }

    // Basit bir test için klavye kısayolları (UI yapana kadar)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            towerToBuild = cannonTowerPrefab;
            Debug.Log("Cannon Seçildi");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            towerToBuild = archerTowerPrefab;
            Debug.Log("Archer Seçildi");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            towerToBuild = mortarTowerPrefab;
            Debug.Log("Mortar Seçildi");
        }
    }
}
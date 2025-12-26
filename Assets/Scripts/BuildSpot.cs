using UnityEngine;

public class BuildSpot : MonoBehaviour
{
    public Color hoverColor;    // Mouse üzerindeyken alacağı renk
    private Color startColor;   // Orijinal rengi
    private SpriteRenderer rend;

    [Header("Durum")]
    public GameObject currentTower; // Bu noktada şu an bir kule var mı?

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        startColor = rend.color;
    }

    // Mouse üzerine gelince
    void OnMouseEnter()
    {
        // Eğer zaten kule varsa veya mouse bir UI elemanının üzerindeyse renk değiştirme
        if (currentTower != null) return;

        rend.color = hoverColor;
    }

    // Mouse üzerinden gidince
    void OnMouseExit()
    {
        rend.color = startColor;
    }

    // Tıklayınca
    void OnMouseDown()
    {
        // 1. Zaten kule varsa inşa etme
        if (currentTower != null)
        {
            Debug.Log("Burada zaten bir kule var!");
            return;
        }

        // 2. Manager'dan seçili kuleyi al
        GameObject towerToBuild = BuildManager.instance.GetTowerToBuild();

        // 3. Kuleyi inşa et
        // Quaternion.identity = Dönmeden düz koy
        currentTower = Instantiate(towerToBuild, transform.position, Quaternion.identity);
        
        Debug.Log("Kule inşa edildi!");
    }
}
using UnityEngine;
using UnityEngine.UI;

public class SilhouetteGlow : MonoBehaviour
{
    public Image sourceImage; // PhoenixImage
    
    void Start()
    {
        Image glowImage = GetComponent<Image>();
        
        // Aynı sprite'ı kullan
        glowImage.sprite = sourceImage.sprite;
        
        // Shader ile beyazlat
        Material mat = new Material(Shader.Find("UI/Default"));
        mat.SetInt("_StencilComp", 8);
        mat.SetInt("_Stencil", 0);
        mat.SetInt("_StencilOp", 0);
        mat.SetInt("_StencilWriteMask", 255);
        mat.SetInt("_StencilReadMask", 255);
        mat.SetInt("_ColorMask", 15);
        
        glowImage.material = mat;
        
        // BEMBEYAZ - alpha hariç tüm pikselleri beyaz yap
        glowImage.color = Color.white;
        mat.SetColor("_Color", Color.white);
        
        // Maskeleme modu
        mat.SetFloat("_UseUIAlphaClip", 1);
    }
}
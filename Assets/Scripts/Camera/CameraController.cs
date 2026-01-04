using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float panSpeed = 20f;       // Klavye hızı
    public float dragSpeed = 30f;      // Mouse sürükleme hızı
    
    [Header("Zoom Ayarları (Orthographic)")]
    public float zoomSpeed = 2f;       // Zoom hızı
    public float minZoom = 5f;         // En yakın (Küçük değer = Yakın)
    public float maxZoom = 20f;        // En uzak (Büyük değer = Uzak)

    [Header("Harita Sınırları")]
    // Burası çok önemli! 0 kalırsa kilitlenirsiniz.
    public Vector2 minLimit = new Vector2(-50, -50); 
    public Vector2 maxLimit = new Vector2(50, 50);

    private Camera cam;
    private Vector3 dragOrigin;

    void Start()
    {
        cam = GetComponent<Camera>(); // Kamerayı otomatik tanı
    }

    void Update()
    {
        // --- 1. KLAVYE HAREKETİ (WASD) ---
        Vector3 pos = transform.position;

        if (Input.GetKey("w")) pos.z += panSpeed * Time.deltaTime;
        if (Input.GetKey("s")) pos.z -= panSpeed * Time.deltaTime;
        if (Input.GetKey("d")) pos.x += panSpeed * Time.deltaTime;
        if (Input.GetKey("a")) pos.x -= panSpeed * Time.deltaTime;

        // --- 2. MOUSE SÜRÜKLEME ---
        // Sağ tık (1) basılıyken sürükle
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (Input.GetMouseButton(1))
        {
            // Mouse ne kadar kaydı?
            Vector3 posMove = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            
            // Kamerayı ters yöne it (Sürükleme hissi için)
            // İzometrik açıda X ve Y koordinatları biraz farklı hissedilebilir
            Vector3 move = new Vector3(posMove.x * dragSpeed, 0, posMove.y * dragSpeed);
            
            // Kameranın kendi açısına göre hareket etmesi için (Global değil Local hareket)
            transform.Translate(-move, Space.World);  
            
            dragOrigin = Input.mousePosition;
            
            // Pozisyonu güncelle ki sınır kontrolüne girsin
            pos = transform.position; 
        }

        // --- 3. SINIRLAMA (CLAMP) ---
        // Kamerayı harita sınırları içinde tut
        pos.x = Mathf.Clamp(pos.x, minLimit.x, maxLimit.x);
        pos.z = Mathf.Clamp(pos.z, minLimit.y, maxLimit.y);
        
        transform.position = pos;

        // --- 4. ZOOM (ORTHOGRAPHIC SIZE) ---
        // İzometrik kamerada "Size" değeri değişince zoom olur
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cam.orthographicSize -= scroll * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }
}
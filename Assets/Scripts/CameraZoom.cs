using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [Header("Zoom Ayarları")]
    public float zoomHizi = 2f;
    public float enYakin = 3f;
    public float enUzak = 10f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        
        if (!cam.orthographic)
        {
            Debug.LogError("Kamera Orthographic modda değil! Inspector'dan değiştir.");
        }
    }

    void Update()
    {
        ZoomYap();
    }

    void ZoomYap()
    {
        float scrollMiktari = 0f;

        // ESKİ INPUT SİSTEMİ (Input Manager)
        #if ENABLE_LEGACY_INPUT_MANAGER
        scrollMiktari = Input.GetAxis("Mouse ScrollWheel");
        #endif

        // YENİ INPUT SİSTEMİ (Input System Package)
        #if ENABLE_INPUT_SYSTEM
        if (UnityEngine.InputSystem.Mouse.current != null)
        {
            scrollMiktari = UnityEngine.InputSystem.Mouse.current.scroll.ReadValue().y / 120f;
        }
        #endif

        if (scrollMiktari != 0f)
        {
            cam.orthographicSize -= scrollMiktari * zoomHizi;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, enYakin, enUzak);
        }
    }
}
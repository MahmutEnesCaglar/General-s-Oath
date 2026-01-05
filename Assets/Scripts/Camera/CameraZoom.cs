using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [Header("Zoom Ayarları")]
    public float zoomHizi = 25f;
    public float enYakin = 3f;
    public float enUzak = 8f;

    [Header("Hareket Ayarları")]
    public float hareketHizi = 10f;
    public bool hareketAktif = true;
    
    [Header("Dinamik Sınırlar")]
    public bool dinamikSinirKullan = true;
    public float maxZoomSize = 8f; // Tam zoom'da sınır = 0
    public float xFactor = 1.78f; // Max X için çarpan
    public float yFactor = 1.02f; // Max Y için çarpan
    
    [Header("Sabit Sınırlar (Dinamik kapalıysa)")]
    public bool sabitSinirKullan = true;
    public float minX = -20f;
    public float maxX = 20f;
    public float minY = -20f;
    public float maxY = 20f;

    private Camera cam;
    private bool zoomEnabled = true;
    private bool movementEnabled = true;
    
    // Hesaplanmış dinamik sınırlar
    private float currentMinX;
    private float currentMaxX;
    private float currentMinY;
    private float currentMaxY;

    void Start()
    {
        cam = GetComponent<Camera>();
        
        if (!cam.orthographic)
        {
            Debug.LogError("Kamera Orthographic modda değil!");
        }
        
        // İlk sınırları hesapla
        if (dinamikSinirKullan)
        {
            UpdateDynamicBounds();
        }
    }

    void Update()
    {
        if (zoomEnabled)
        {
            ZoomYap();
        }
        
        if (movementEnabled && hareketAktif)
        {
            HareketYap();
        }
    }

    void ZoomYap()
    {
        if (!zoomEnabled)
            return;

        float scrollMiktari = 0f;

        #if ENABLE_LEGACY_INPUT_MANAGER
        scrollMiktari = Input.GetAxis("Mouse ScrollWheel");
        #endif

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
            
            // Zoom değişince sınırları güncelle
            if (dinamikSinirKullan)
            {
                UpdateDynamicBounds();
            }
        }
    }

    /// <summary>
    /// Zoom seviyesine göre dinamik sınırları hesapla
    /// </summary>
    void UpdateDynamicBounds()
    {
        float currentSize = cam.orthographicSize;
        
        // Zoom seviyesi maxZoomSize'a (8) ulaştığında sınır 0 olmalı
        float delta = Mathf.Max(0, maxZoomSize - currentSize);
        
        // Sınırları hesapla
        currentMaxX = delta * xFactor;
        currentMaxY = delta * yFactor;
        currentMinX = -delta * xFactor;
        currentMinY = -delta * yFactor;
        
        // !!!!! ÖNEMLİ: Kamera pozisyonunu hemen sınırlar içine al !!!!!
        ClampCameraPosition();
    }

    /// <summary>
    /// Kamera pozisyonunu mevcut sınırlar içine al
    /// </summary>
    void ClampCameraPosition()
    {
        Vector3 pos = transform.position;
        
        // X ve Y'yi sınırlar içine clamp et
        pos.x = Mathf.Clamp(pos.x, currentMinX, currentMaxX);
        pos.y = Mathf.Clamp(pos.y, currentMinY, currentMaxY);
        
        // Pozisyonu güncelle (smooth değil, anında)
        transform.position = pos;
    }

    void HareketYap()
    {
        float horizontal = 0f;
        float vertical = 0f;

        #if ENABLE_LEGACY_INPUT_MANAGER
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        #endif

        #if ENABLE_INPUT_SYSTEM
        var keyboard = UnityEngine.InputSystem.Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                vertical = 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
                vertical = -1f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                horizontal = -1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                horizontal = 1f;
        }
        #endif

        if (horizontal != 0f || vertical != 0f)
        {
            Vector3 hareket = new Vector3(horizontal, vertical, 0f) * hareketHizi * Time.deltaTime;
            transform.position += hareket;

            // Sınırları uygula
            Vector3 yeniPozisyon = transform.position;
            
            if (dinamikSinirKullan)
            {
                // Dinamik sınırları kullan
                yeniPozisyon.x = Mathf.Clamp(yeniPozisyon.x, currentMinX, currentMaxX);
                yeniPozisyon.y = Mathf.Clamp(yeniPozisyon.y, currentMinY, currentMaxY);
            }
            else if (sabitSinirKullan)
            {
                // Sabit sınırları kullan
                yeniPozisyon.x = Mathf.Clamp(yeniPozisyon.x, minX, maxX);
                yeniPozisyon.y = Mathf.Clamp(yeniPozisyon.y, minY, maxY);
            }
            
            transform.position = yeniPozisyon;
        }
    }

    public void SetZoomEnabled(bool enabled)
    {
        zoomEnabled = enabled;
        Debug.Log($"Zoom {(enabled ? "açıldı" : "kapatıldı")}");
    }

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;
        Debug.Log($"Kamera hareketi {(enabled ? "açıldı" : "kapatıldı")}");
    }

    public void SetCameraControlsEnabled(bool enabled)
    {
        SetZoomEnabled(enabled);
        SetMovementEnabled(enabled);
    }
}
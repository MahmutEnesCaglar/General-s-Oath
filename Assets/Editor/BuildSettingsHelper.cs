using UnityEditor;
using UnityEngine;
using System.Linq;

/// <summary>
/// Build Settings'e scene'leri otomatik ekleyen helper script
/// Unity Editor'da çalışır
/// Unity açıldığında otomatik olarak scene'leri ekler
/// </summary>
[InitializeOnLoad]
public static class BuildSettingsHelper
{
    // Unity açıldığında otomatik çalışır
    static BuildSettingsHelper()
    {
        EditorApplication.delayCall += AddScenesToBuildSettings;
    }

    [MenuItem("Tools/Add Scenes to Build Settings")]
    public static void AddScenesToBuildSettings()
    {
        // Eklenecek scene'ler
        string[] scenePaths = new string[]
        {
            "Assets/Scenes/MainMenuSahne.unity",
            "Assets/Scenes/Map_Grifon.unity"
        };

        // Mevcut Build Settings'teki scene'leri al
        var existingScenes = EditorBuildSettings.scenes.ToList();

        int addedCount = 0;
        foreach (string scenePath in scenePaths)
        {
            // Scene dosyası var mı kontrol et
            if (!System.IO.File.Exists(scenePath))
            {
                Debug.LogWarning($"[BuildSettings] Scene bulunamadı: {scenePath}");
                continue;
            }

            // Scene zaten listede mi?
            bool alreadyExists = existingScenes.Any(scene => scene.path == scenePath);

            if (!alreadyExists)
            {
                // Yeni scene ekle
                var newScene = new EditorBuildSettingsScene(scenePath, true);
                existingScenes.Add(newScene);
                addedCount++;
                Debug.Log($"[BuildSettings] ✓ Eklendi: {scenePath}");
            }
            else
            {
                Debug.Log($"[BuildSettings] Zaten var: {scenePath}");
            }
        }

        // Build Settings'i güncelle
        if (addedCount > 0)
        {
            EditorBuildSettings.scenes = existingScenes.ToArray();
            Debug.Log($"[BuildSettings] {addedCount} scene Build Settings'e eklendi!");
        }
        else
        {
            Debug.Log("[BuildSettings] Tüm scene'ler zaten Build Settings'te mevcut.");
        }
    }
}

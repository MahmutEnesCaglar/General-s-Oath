using UnityEngine;
using UnityEditor;

public class MissingScriptFinder : EditorWindow
{
    [MenuItem("Tools/Find Missing Scripts (Deep Search)")]
    public static void FindMissingScriptsDeep()
    {
        Debug.Log("--- Derinlemesine Missing Script Taraması Başlıyor ---");
        int count = 0;

        // 1. Sahnedeki Objeleri Tara
        GameObject[] sceneObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject go in sceneObjects)
        {
            count += CheckGameObject(go, "SAHNE: " + GetPath(go.transform));
        }

        // 2. Projedeki Tüm Prefabları Tara
        string[] allPrefabs = AssetDatabase.FindAssets("t:Prefab");
        foreach (string guid in allPrefabs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab != null)
            {
                // Prefab'ın kendisini ve tüm çocuklarını kontrol et (Inactive olanlar dahil)
                Component[] components = prefab.GetComponentsInChildren<Component>(true);
                bool foundInPrefab = false;
                
                foreach (Component c in components)
                {
                    if (c == null)
                    {
                        Debug.LogWarning($"⚠️ Script Kayıp! PREFAB: <b>{path}</b>", prefab);
                        count++;
                        foundInPrefab = true;
                        // Bir prefabda bir tane bulsak yeter, aynı prefab için spam yapmayalım
                        break; 
                    }
                }
            }
        }

        if (count == 0)
        {
            Debug.Log("✅ Hiçbir eksik script bulunamadı. (Eğer hala uyarı alıyorsan, Unity'i kapatıp açmayı dene)");
        }
        else
        {
            Debug.Log($"⚠️ Toplam {count} adet eksik script bulundu. Konsoldaki uyarılara tıklayarak dosyalara gidebilirsin.");
        }
    }

    private static int CheckGameObject(GameObject go, string context)
    {
        int found = 0;
        Component[] components = go.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] == null)
            {
                Debug.LogWarning($"⚠️ Script Kayıp! {context}", go);
                found++;
            }
        }
        return found;
    }

    private static string GetPath(Transform transform)
    {
        if (transform == null) return "NULL";
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }
}

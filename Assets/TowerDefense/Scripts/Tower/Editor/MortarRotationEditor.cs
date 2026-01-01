using UnityEngine;
using UnityEditor;
using TowerDefense.Tower;

namespace TowerDefense.Editor
{
    [CustomEditor(typeof(MortarRotation))]
    public class MortarRotationEditor : UnityEditor.Editor
    {
        private int previewDirection = 0;

        public override void OnInspectorGUI()
        {
            MortarRotation script = (MortarRotation)target;
            
            // Varsayılan Inspector'ı çiz
            DrawDefaultInspector();
            
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("═══════════════════════════════════", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("🎮 PREVIEW KONTROLLERI (6 YÖN)", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("═══════════════════════════════════", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            // --- LEVEL PREVIEW ---
            if (script.allLevels != null && script.allLevels.Count > 0)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("📊 Level Seçimi", EditorStyles.boldLabel);
                
                EditorGUI.BeginChangeCheck();
                int newLevel = EditorGUILayout.IntSlider("Level Index", script.currentLevelIndex, 0, script.allLevels.Count - 1);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(script, "Change Level Preview");
                    script.SetLevel(newLevel);
                    EditorUtility.SetDirty(script);
                }
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.HelpBox("⚠️ Level listesi boş! Lütfen 'All Levels' listesine veri ekleyin.", MessageType.Warning);
            }
            
            EditorGUILayout.Space(10);
            
            // Sprite listesi durumu
            if (script.currentDirectionalData == null || script.currentDirectionalData.Count == 0)
            {
                EditorGUILayout.HelpBox("⚠️ Aktif Level verisi (currentDirectionalData) boş!", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox($"✅ Aktif Level: {script.currentLevelIndex} | {script.currentDirectionalData.Count}/6 yön tanımlı", MessageType.Info);
                
                EditorGUILayout.Space(10);
                
                // Direction Slider
                EditorGUI.BeginChangeCheck();
                int newDirection = EditorGUILayout.IntSlider("🧭 Preview Direction", previewDirection, 0, 5);
                if (EditorGUI.EndChangeCheck())
                {
                    previewDirection = newDirection;
                    
                    if (Application.isPlaying)
                    {
                        // Oyun modunda test pozisyonu hesapla
                        float angle = previewDirection * 60f;
                        float rad = angle * Mathf.Deg2Rad;
                        Vector3 testPos = script.transform.position + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * 5f;
                        script.RotateTowards(testPos);
                    }
                    else
                    {
                        // Edit modunda direkt sprite değiştir
                        if (previewDirection < script.currentDirectionalData.Count)
                        {
                            var spriteRenderer = script.GetComponent<SpriteRenderer>();
                            if (spriteRenderer != null)
                            {
                                Undo.RecordObject(spriteRenderer, "Change Sprite Preview");
                                var data = script.currentDirectionalData[previewDirection];
                                spriteRenderer.sprite = data.sprite;
                                script.transform.localScale = data.scale == Vector3.zero ? Vector3.one : data.scale;
                                EditorUtility.SetDirty(script);
                            }
                        }
                    }
                }
                
                // Direction ismi
                string[] directionNames = { 
                    "→ Right (0°)", 
                    "↗ Up-Right (60°)", 
                    "↖ Up-Left (120°)", 
                    "← Left (180°)", 
                    "↙ Down-Left (240°)",
                    "↘ Down-Right (300°)"
                };
                
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField($"Yön: {directionNames[previewDirection]}", EditorStyles.boldLabel);
                
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("⚡ Hızlı Yön Butonları:", EditorStyles.boldLabel);
                
                // Butonlar (2 satır halinde)
                // Satır 1
                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < 3; i++)
                {
                    string buttonLabel = directionNames[i].Split(' ')[0] + " " + i;
                    if (GUILayout.Button(buttonLabel, GUILayout.Height(30)))
                    {
                        SetPreview(script, i);
                    }
                }
                EditorGUILayout.EndHorizontal();

                // Satır 2
                EditorGUILayout.BeginHorizontal();
                for (int i = 3; i < 6; i++)
                {
                    string buttonLabel = directionNames[i].Split(' ')[0] + " " + i;
                    if (GUILayout.Button(buttonLabel, GUILayout.Height(30)))
                    {
                        SetPreview(script, i);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void SetPreview(MortarRotation script, int dirIndex)
        {
            previewDirection = dirIndex;
            
            if (Application.isPlaying)
            {
                float angle = dirIndex * 60f;
                float rad = angle * Mathf.Deg2Rad;
                Vector3 testPos = script.transform.position + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * 5f;
                script.RotateTowards(testPos);
            }
            else
            {
                if (dirIndex < script.currentDirectionalData.Count)
                {
                    var spriteRenderer = script.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        Undo.RecordObject(spriteRenderer, "Change Sprite Preview");
                        var data = script.currentDirectionalData[dirIndex];
                        spriteRenderer.sprite = data.sprite;
                        script.transform.localScale = data.scale == Vector3.zero ? Vector3.one : data.scale;
                        EditorUtility.SetDirty(script);
                    }
                }
            }
        }
    }
}

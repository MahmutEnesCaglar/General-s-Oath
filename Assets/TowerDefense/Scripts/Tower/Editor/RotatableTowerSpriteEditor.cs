using UnityEngine;
using UnityEditor;
using TowerDefense.Tower;

namespace TowerDefense.Editor
{
    [CustomEditor(typeof(RotatableTowerSprite))]
    public class RotatableTowerSpriteEditor : UnityEditor.Editor
    {
        private int previewDirection = 0;

        public override void OnInspectorGUI()
        {
            RotatableTowerSprite script = (RotatableTowerSprite)target;
            
            // Varsayılan Inspector'ı çiz
            DrawDefaultInspector();
            
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("═══════════════════════════════════", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("🎮 PREVIEW KONTROLLERI", EditorStyles.boldLabel);
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
                EditorGUILayout.HelpBox($"✅ Aktif Level: {script.currentLevelIndex} | {script.currentDirectionalData.Count}/8 yön tanımlı", MessageType.Info);
                
                EditorGUILayout.Space(10);
                
                // Direction Slider
                EditorGUI.BeginChangeCheck();
                int newDirection = EditorGUILayout.IntSlider("🧭 Preview Direction", previewDirection, 0, 7);
                if (EditorGUI.EndChangeCheck())
                {
                    previewDirection = newDirection;
                    
                    if (Application.isPlaying)
                    {
                        // Oyun modunda test pozisyonu hesapla
                        float angle = previewDirection * 45f;
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
                    "↘ Down-Right (45°)", 
                    "↓ Down (90°)", 
                    "↙ Down-Left (135°)", 
                    "← Left (180°)", 
                    "↖ Up-Left (225°)", 
                    "↑ Up (270°)", 
                    "↗ Up-Right (315°)" 
                };
                
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField($"Yön: {directionNames[previewDirection]}", EditorStyles.boldLabel);
                
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("⚡ Hızlı Yön Butonları:", EditorStyles.boldLabel);
                
                // İlk satır (0-3)
                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < 4; i++)
                {
                    string buttonLabel = directionNames[i].Split(' ')[0] + " " + i;
                    if (GUILayout.Button(buttonLabel, GUILayout.Height(30)))
                    {
                        previewDirection = i;
                        
                        if (Application.isPlaying)
                        {
                            float angle = i * 45f;
                            float rad = angle * Mathf.Deg2Rad;
                            Vector3 testPos = script.transform.position + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * 5f;
                            script.RotateTowards(testPos);
                        }
                        else if (i < script.currentDirectionalData.Count)
                        {
                            var spriteRenderer = script.GetComponent<SpriteRenderer>();
                            if (spriteRenderer != null)
                            {
                                var data = script.currentDirectionalData[i];
                                spriteRenderer.sprite = data.sprite;
                                script.transform.localScale = data.scale == Vector3.zero ? Vector3.one : data.scale;
                                EditorUtility.SetDirty(script);
                            }
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                // İkinci satır (4-7)
                EditorGUILayout.BeginHorizontal();
                for (int i = 4; i < 8; i++)
                {
                    string buttonLabel = directionNames[i].Split(' ')[0] + " " + i;
                    if (GUILayout.Button(buttonLabel, GUILayout.Height(30)))
                    {
                        previewDirection = i;
                        
                        if (Application.isPlaying)
                        {
                            float angle = i * 45f;
                            float rad = angle * Mathf.Deg2Rad;
                            Vector3 testPos = script.transform.position + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * 5f;
                            script.RotateTowards(testPos);
                        }
                        else if (i < script.currentDirectionalData.Count)
                        {
                            var spriteRenderer = script.GetComponent<SpriteRenderer>();
                            if (spriteRenderer != null)
                            {
                                var data = script.currentDirectionalData[i];
                                spriteRenderer.sprite = data.sprite;
                                script.transform.localScale = data.scale == Vector3.zero ? Vector3.one : data.scale;
                                EditorUtility.SetDirty(script);
                            }
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                // Eksik sprite kontrolü
                CheckForMissingSprites(script);
            }
            
            EditorGUILayout.Space(10);
            
            // Bilgi kutusu
            EditorGUILayout.HelpBox(
                "💡 İPUCU:\n" +
                "• currentDirectionalData listesine 8 yön için sprite ekleyin\n" +
                "• Preview butonları ile farklı yönleri test edebilirsiniz\n" +
                "• Scale ve FirePoint Offset değerleri her yön için farklı olabilir\n" +
                "• Level değiştiğinde Tower.cs SetLevelVisuals() ile listeyi güncelleyecek", 
                MessageType.None
            );
        }

        private void CheckForMissingSprites(RotatableTowerSprite script)
        {
            bool hasWarning = false;
            string warningMessage = "⚠️ UYARI: Eksik Sprite'lar:\n";
            
            if (script.currentDirectionalData.Count < 8)
            {
                hasWarning = true;
                warningMessage += $"• {script.currentDirectionalData.Count}/8 sprite tanımlı - 8 olmalı!\n";
            }
            else
            {
                int nullCount = 0;
                foreach (var data in script.currentDirectionalData)
                {
                    if (data.sprite == null) nullCount++;
                }
                if (nullCount > 0)
                {
                    hasWarning = true;
                    warningMessage += $"• {nullCount} sprite null durumda\n";
                }
            }
            
            if (hasWarning)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.HelpBox(warningMessage, MessageType.Warning);
            }
            else
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.HelpBox("✅ Tüm yönler için sprite'lar tam!", MessageType.Info);
            }
        }
    }
}

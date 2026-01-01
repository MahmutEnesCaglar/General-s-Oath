using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using System.Linq;

/// <summary>
/// Evil Wizard 2 için BossEnemy Animator Controller'ı otomatik oluşturur
/// Unity Editor menüsünden: Tools > Setup Evil Wizard Boss
/// </summary>
public class EvilWizardBossSetup : Editor
{
    [MenuItem("Tools/Setup Evil Wizard Boss")]
    public static void SetupEvilWizardBoss()
    {
        Debug.Log("Starting Evil Wizard Boss Setup...");

        // 1. Animator Controller oluştur
        string controllerPath = "Assets/Evil Wizard 2/Animations/EvilWizardBoss.controller";
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

        // 2. Animasyon dosyalarını yükle
        AnimationClip idleAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Evil Wizard 2/Animations/Idle.anim");
        AnimationClip runAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Evil Wizard 2/Animations/Run.anim");
        AnimationClip attack1Anim = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Evil Wizard 2/Animations/Attack1.anim");
        AnimationClip deathAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Evil Wizard 2/Animations/Death.anim");

        // Take hit animasyonu optional (asset pack'te eksik olabilir)
        AnimationClip takeHitAnim = idleAnim; // Fallback: idle kullan

        if (idleAnim == null || runAnim == null || attack1Anim == null || deathAnim == null)
        {
            Debug.LogError("Evil Wizard 2 temel animasyonları bulunamadı! Gerekli: Idle.anim, Run.anim, Attack1.anim, Death.anim");
            Debug.LogError($"Idle: {idleAnim != null}, Run: {runAnim != null}, Attack1: {attack1Anim != null}, Death: {deathAnim != null}");
            return;
        }

        Debug.Log("<color=cyan>Evil Wizard 2 animasyonları yüklendi. Hurt için Idle animasyonu kullanılacak.</color>");

        // 3. State'leri oluştur
        var rootStateMachine = controller.layers[0].stateMachine;

        // Idle State (Default)
        var idleState = rootStateMachine.AddState("idle");
        idleState.motion = idleAnim;
        rootStateMachine.defaultState = idleState;

        // Walk State (Run animasyonu kullanılacak)
        var walkState = rootStateMachine.AddState("walk");
        walkState.motion = runAnim;

        // Attack State
        var attackState = rootStateMachine.AddState("attack");
        attackState.motion = attack1Anim;

        // Hurt State
        var hurtState = rootStateMachine.AddState("hurt");
        hurtState.motion = takeHitAnim;

        // Die State
        var dieState = rootStateMachine.AddState("die");
        dieState.motion = deathAnim;

        // 4. Any State transition'ları ekle (BaseEnemy Play() metodları için)
        var anyState = rootStateMachine.AddAnyStateTransition(attackState);
        anyState.hasExitTime = false;
        anyState.duration = 0.1f;
        anyState.canTransitionToSelf = false;

        var anyStateToHurt = rootStateMachine.AddAnyStateTransition(hurtState);
        anyStateToHurt.hasExitTime = false;
        anyStateToHurt.duration = 0.1f;
        anyStateToHurt.canTransitionToSelf = false;

        var anyStateToDie = rootStateMachine.AddAnyStateTransition(dieState);
        anyStateToDie.hasExitTime = false;
        anyStateToDie.duration = 0.1f;
        anyStateToDie.canTransitionToSelf = false;

        // 6. Attack -> Idle transition (saldırı bitince idle'a dön)
        var attackToIdle = attackState.AddTransition(idleState);
        attackToIdle.hasExitTime = true;
        attackToIdle.exitTime = 0.9f;
        attackToIdle.duration = 0.1f;

        // 7. Hurt -> Idle transition (hasar animasyonu bitince idle'a dön)
        var hurtToIdle = hurtState.AddTransition(idleState);
        hurtToIdle.hasExitTime = true;
        hurtToIdle.exitTime = 0.9f;
        hurtToIdle.duration = 0.1f;

        // 8. Die state'i final yap (bir daha dönmesin)
        dieState.transitions = new AnimatorStateTransition[0];

        // Controller'ı kaydet
        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"<color=green>✓ Evil Wizard Boss Animator Controller oluşturuldu: {controllerPath}</color>");

        // 9. BossEnemy prefab'ını bul ve güncelle
        UpdateBossEnemyPrefab(controller);
    }

    private static void UpdateBossEnemyPrefab(AnimatorController controller)
    {
        string prefabPath = "Assets/TowerDefense/Prefabs/Enemies/BossEnemy.prefab";
        GameObject prefabInstance;

        // Mevcut prefab'ı kontrol et
        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (existingPrefab == null)
        {
            Debug.Log("<color=yellow>BossEnemy.prefab bulunamadı. Sıfırdan oluşturuluyor...</color>");

            // Yeni GameObject oluştur
            prefabInstance = new GameObject("BossEnemy");

            // BossEnemy script'ini ekle (Type.GetType kullanarak)
            System.Type bossType = System.Type.GetType("TowerDefense.Enemy.BossEnemy, Assembly-CSharp");
            if (bossType == null)
            {
                Debug.LogError("BossEnemy script bulunamadı! Assembly-CSharp'ta TowerDefense.Enemy.BossEnemy class'ı yok.");
                Object.DestroyImmediate(prefabInstance);
                return;
            }
            var bossScript = prefabInstance.AddComponent(bossType) as MonoBehaviour;

            // Visual child object oluştur
            GameObject visualObj = new GameObject("Visual");
            visualObj.transform.SetParent(prefabInstance.transform);
            visualObj.transform.localPosition = Vector3.zero;

            // SpriteRenderer ve Animator ekle
            SpriteRenderer sr = visualObj.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = "Default";
            sr.sortingOrder = 5;

            // İlk sprite'ı yükle (tek sprite olarak - animasyon controller halleder)
            Sprite[] idleSprites = AssetDatabase.LoadAllAssetsAtPath("Assets/Evil Wizard 2/Sprites/Idle.png")
                .OfType<Sprite>().ToArray();
            if (idleSprites.Length > 0)
            {
                sr.sprite = idleSprites[0];
            }

            Animator animator = visualObj.AddComponent<Animator>();
            animator.runtimeAnimatorController = controller;

            // Rigidbody2D ekle (Kinematic)
            Rigidbody2D rb = prefabInstance.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0;

            // BoxCollider2D ekle
            BoxCollider2D collider = prefabInstance.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.8f, 1.2f);
            collider.offset = new Vector2(0, 0.6f);

            // Prefab olarak kaydet
            PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
            Object.DestroyImmediate(prefabInstance);

            Debug.Log($"<color=green>✓ Yeni BossEnemy prefab oluşturuldu: {prefabPath}</color>");
        }
        else
        {
            // Mevcut prefab'ı düzenleme modunda aç
            prefabInstance = PrefabUtility.LoadPrefabContents(prefabPath);

            // Animator component kontrolü
            Animator animator = prefabInstance.GetComponentInChildren<Animator>();
            if (animator == null)
            {
                // Visual child object'i bul veya oluştur
                Transform visualChild = prefabInstance.transform.Find("Visual");
                if (visualChild == null)
                {
                    GameObject visualObj = new GameObject("Visual");
                    visualObj.transform.SetParent(prefabInstance.transform);
                    visualObj.transform.localPosition = Vector3.zero;
                    visualChild = visualObj.transform;

                    // SpriteRenderer ekle
                    SpriteRenderer sr = visualObj.AddComponent<SpriteRenderer>();
                    Sprite[] idleSprites = AssetDatabase.LoadAllAssetsAtPath("Assets/Evil Wizard 2/Sprites/Idle.png")
                        .OfType<Sprite>().ToArray();
                    if (idleSprites.Length > 0)
                    {
                        sr.sprite = idleSprites[0];
                    }
                }

                animator = visualChild.gameObject.AddComponent<Animator>();
            }

            // Animator Controller'ı ata
            animator.runtimeAnimatorController = controller;

            // Prefab'ı kaydet
            PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
            PrefabUtility.UnloadPrefabContents(prefabInstance);

            Debug.Log($"<color=green>✓ Mevcut BossEnemy prefab güncellendi!</color>");
        }

        Debug.Log($"<color=cyan>Evil Wizard Boss artık kullanıma hazır!</color>");
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace TowerDefense.Hero
{
    /// <summary>
    /// Manages hero input and mode switching between hero control and tower placement
    /// Prevents input conflicts by toggling modes with T key
    /// Uses Unity's new Input System
    /// </summary>
    public class HeroInput : MonoBehaviour
    {
        public static HeroInput Instance;

        public enum InputMode { HeroControl, TowerPlacement }
        public InputMode currentMode = InputMode.HeroControl;

        [Header("References")]
        public Hero hero;
        private TowerDefense.Tower.TowerPlacement towerPlacement;

        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Find tower placement system
            towerPlacement = FindObjectOfType<TowerDefense.Tower.TowerPlacement>();
        }

        private void Start()
        {
            Debug.Log("HeroInput initialized - HERO MODE active. Press T to toggle Tower Mode.");
        }

        private void Update()
        {
            // Safety check for input devices
            if (Keyboard.current == null || Mouse.current == null)
            {
                return;
            }

            // Find hero if not assigned (hero might spawn later)
            if (hero == null)
            {
                hero = FindObjectOfType<Hero>();
                if (hero != null)
                {
                    Debug.Log("✓ Hero found and assigned to HeroInput!");
                }
            }

            // Toggle tower mode with T key
            if (Keyboard.current[Key.T].wasPressedThisFrame)
            {
                ToggleTowerMode();
            }

            // Hero movement (only in hero mode)
            if (currentMode == InputMode.HeroControl && Mouse.current.leftButton.wasPressedThisFrame)
            {
                HandleHeroMovement();
            }

            // Special ability (Q key) - works in both modes
            if (Keyboard.current[Key.Q].wasPressedThisFrame)
            {
                if (hero != null && hero.abilities != null)
                {
                    hero.abilities.ActivateSpecialAbility();
                }
            }

            // Block (RMB or B key) - works in both modes
            bool blockPressed = Mouse.current.rightButton.isPressed || Keyboard.current[Key.B].isPressed;
            if (hero != null && hero.abilities != null)
            {
                hero.abilities.ActivateBlock(blockPressed);
            }
        }

        /// <summary>
        /// Handles hero click-to-move
        /// </summary>
        private void HandleHeroMovement()
        {
            // Ignore clicks on actual interactive UI elements (buttons, panels, etc.)
            if (EventSystem.current != null && IsPointerOverInteractiveUI())
            {
                Debug.Log("Clicked on interactive UI, ignoring hero movement");
                return;
            }

            Debug.Log("HandleHeroMovement called!");

            if (hero == null)
            {
                Debug.LogError("Hero is NULL!");
                return;
            }

            if (hero.isDead)
            {
                Debug.LogWarning("Hero is dead, cannot move!");
                return;
            }

            // Get mouse world position using new Input System
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 10f));
            mousePos.z = 0;

            Debug.Log($"Mouse clicked at: {mousePos}");

            // Set hero destination
            hero.SetDestination(mousePos);
            Debug.Log($"Hero destination set to: {mousePos}");
        }

        /// <summary>
        /// Toggles between hero control and tower placement modes
        /// </summary>
        private void ToggleTowerMode()
        {
            if (currentMode == InputMode.HeroControl)
            {
                // Switch to tower mode
                currentMode = InputMode.TowerPlacement;
                Debug.Log("TOWER MODE activated - Left click to place towers, T to return to Hero Mode");
            }
            else
            {
                // Switch to hero mode
                currentMode = InputMode.HeroControl;

                // Cancel any active tower placement
                if (towerPlacement != null)
                {
                    towerPlacement.CancelPlacement();
                }

                Debug.Log("HERO MODE activated - Left click to move hero, T to switch to Tower Mode");
            }
        }

        /// <summary>
        /// Public method to check if tower placement should be active
        /// </summary>
        public bool IsTowerPlacementMode()
        {
            return currentMode == InputMode.TowerPlacement;
        }

        /// <summary>
        /// Public method to check if hero control should be active
        /// </summary>
        public bool IsHeroControlMode()
        {
            return currentMode == InputMode.HeroControl;
        }

        /// <summary>
        /// Checks if the pointer is over an interactive UI element (buttons, panels)
        /// Returns false for non-interactive UI elements like background images
        /// </summary>
        private bool IsPointerOverInteractiveUI()
        {
            if (EventSystem.current == null) return false;

            // Get all UI elements under the pointer
            var pointerData = new UnityEngine.EventSystems.PointerEventData(EventSystem.current)
            {
                position = Mouse.current.position.ReadValue()
            };

            var results = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            // Check if any result is an actual interactive UI element
            foreach (var result in results)
            {
                // Check if it's a Button, Toggle, Slider, or other interactive component
                if (result.gameObject.GetComponent<UnityEngine.UI.Button>() != null ||
                    result.gameObject.GetComponent<UnityEngine.UI.Toggle>() != null ||
                    result.gameObject.GetComponent<UnityEngine.UI.Slider>() != null ||
                    result.gameObject.GetComponent<UnityEngine.UI.Scrollbar>() != null ||
                    result.gameObject.GetComponent<UnityEngine.UI.InputField>() != null ||
                    result.gameObject.GetComponent<TMPro.TMP_InputField>() != null)
                {
                    return true; // Found an interactive UI element
                }
            }

            return false; // No interactive UI found
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using Pilipala.GameState.Data;
using EventSystem;

namespace Pilipala.GameState.Input
{
    /// <summary>
    /// Handles input for game state management (pause, menu navigation, etc.)
    /// </summary>
    public class GameStateInputHandler : MonoBehaviour
    {
        [Header("Input Configuration")]
        [SerializeField] private bool enablePauseInput = true;
        [SerializeField] private Key pauseKey = Key.Escape;
        [SerializeField] private bool enableMenuInput = true;
        [SerializeField] private Key menuKey = Key.Escape;
        [SerializeField] private bool enableBackInput = true;
        [SerializeField] private Key backKey = Key.Backspace;
        
        [Header("Input Actions")]
        [SerializeField] private InputAction pauseAction;
        [SerializeField] private InputAction menuAction;
        [SerializeField] private InputAction backAction;
        
        private GameStateManager _gameStateManager;
        
        private void Awake()
        {
            _gameStateManager = GameStateManager.Instance;
            
            // Set up input actions if not assigned
            if (pauseAction == null)
            {
                pauseAction = new InputAction("Pause", InputActionType.Button, pauseKey.ToString());
            }
            
            if (menuAction == null)
            {
                menuAction = new InputAction("Menu", InputActionType.Button, menuKey.ToString());
            }
            
            if (backAction == null)
            {
                backAction = new InputAction("Back", InputActionType.Button, backKey.ToString());
            }
        }
        
        private void OnEnable()
        {
            if (enablePauseInput)
            {
                pauseAction.Enable();
                pauseAction.performed += OnPauseInput;
            }
            
            if (enableMenuInput)
            {
                menuAction.Enable();
                menuAction.performed += OnMenuInput;
            }
            
            if (enableBackInput)
            {
                backAction.Enable();
                backAction.performed += OnBackInput;
            }
        }
        
        private void OnDisable()
        {
            pauseAction?.Disable();
            menuAction?.Disable();
            backAction?.Disable();
        }
        
        private void OnDestroy()
        {
            pauseAction?.Dispose();
            menuAction?.Dispose();
            backAction?.Dispose();
        }
        
        /// <summary>
        /// Handle pause input
        /// </summary>
        private void OnPauseInput(InputAction.CallbackContext context)
        {
            if (!_gameStateManager.CanPerformAction("pause"))
            {
                return;
            }
            
            if (_gameStateManager.CurrentState == Data.GameState.Playing)
            {
                _gameStateManager.PauseGame();
                EventBusHelper.Trigger("GameState.PauseInput");
            }
            else if (_gameStateManager.CurrentState == Data.GameState.Paused)
            {
                _gameStateManager.ResumeGame();
                EventBusHelper.Trigger("GameState.ResumeInput");
            }
        }
        
        /// <summary>
        /// Handle menu input
        /// </summary>
        private void OnMenuInput(InputAction.CallbackContext context)
        {
            if (!_gameStateManager.CanPerformAction("input"))
            {
                return;
            }
            
            switch (_gameStateManager.CurrentState)
            {
                case Data.GameState.Playing:
                    _gameStateManager.ShowSettings();
                    break;
                case Data.GameState.Paused:
                    _gameStateManager.ResumeGame();
                    break;
                case Data.GameState.Settings:
                    _gameStateManager.GoToPreviousState();
                    break;
                case Data.GameState.LevelSelect:
                    _gameStateManager.GoToMainMenu();
                    break;
            }
            
            EventBusHelper.Trigger("GameState.MenuInput");
        }
        
        /// <summary>
        /// Handle back input
        /// </summary>
        private void OnBackInput(InputAction.CallbackContext context)
        {
            if (!_gameStateManager.CanPerformAction("input"))
            {
                return;
            }
            
            switch (_gameStateManager.CurrentState)
            {
                case Data.GameState.Settings:
                case Data.GameState.LevelSelect:
                    _gameStateManager.GoToPreviousState();
                    break;
                case Data.GameState.Paused:
                    _gameStateManager.GoToMainMenu();
                    break;
            }
            
            EventBusHelper.Trigger("GameState.BackInput");
        }
        
        /// <summary>
        /// Enable or disable pause input
        /// </summary>
        public void SetPauseInputEnabled(bool enabled)
        {
            enablePauseInput = enabled;
            if (enabled)
            {
                pauseAction?.Enable();
            }
            else
            {
                pauseAction?.Disable();
            }
        }
        
        /// <summary>
        /// Enable or disable menu input
        /// </summary>
        public void SetMenuInputEnabled(bool enabled)
        {
            enableMenuInput = enabled;
            if (enabled)
            {
                menuAction?.Enable();
            }
            else
            {
                menuAction?.Disable();
            }
        }
        
        /// <summary>
        /// Enable or disable back input
        /// </summary>
        public void SetBackInputEnabled(bool enabled)
        {
            enableBackInput = enabled;
            if (enabled)
            {
                backAction?.Enable();
            }
            else
            {
                backAction?.Disable();
            }
        }
        
        /// <summary>
        /// Get the current pause input state
        /// </summary>
        public bool IsPausePressed()
        {
            return pauseAction?.ReadValue<float>() > 0f;
        }
        
        /// <summary>
        /// Get the current menu input state
        /// </summary>
        public bool IsMenuPressed()
        {
            return menuAction?.ReadValue<float>() > 0f;
        }
        
        /// <summary>
        /// Get the current back input state
        /// </summary>
        public bool IsBackPressed()
        {
            return backAction?.ReadValue<float>() > 0f;
        }
    }
} 
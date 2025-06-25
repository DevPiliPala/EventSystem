using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Pilipala.GameState.Data;
using EventSystem;

namespace Pilipala.GameState
{
    /// <summary>
    /// Main game state manager that handles state transitions, scene management, and pause functionality
    /// </summary>
    [DefaultExecutionOrder(-200)]
    public class GameStateManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private GameStateDataSO stateData;
        [SerializeField] private bool showDebugInfo = true;
        [SerializeField] private bool autoInitialize = true;
        
        [Header("Scene Management")]
        [SerializeField] private string loadingSceneName = "Loading";
        [SerializeField] private float minimumLoadingTime = 1f;
        
        // State tracking
        private Data.GameState _currentState;
        private Data.GameState _previousState;
        private GameStateData _currentStateData;
        private bool _isTransitioning = false;
        private bool _isPaused = false;
        
        // Scene management
        private string _currentScene;
        private string _targetScene;
        private Coroutine _sceneTransitionCoroutine;
        private GameObject _loadingScreenInstance;
        
        // Properties
        public Data.GameState CurrentState => _currentState;
        public Data.GameState PreviousState => _previousState;
        public GameStateData CurrentStateData => _currentStateData;
        public bool IsTransitioning => _isTransitioning;
        public bool IsPaused => _isPaused;
        public string CurrentScene => _currentScene;
        
        // Singleton pattern
        private static GameStateManager _instance;
        public static GameStateManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<GameStateManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("GameStateManager");
                        _instance = go.AddComponent<GameStateManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        private void Initialize()
        {
            if (stateData == null)
            {
                Debug.LogError("[GameStateManager] No state data assigned!");
                return;
            }
            
            _currentScene = SceneManager.GetActiveScene().name;
            
            if (autoInitialize)
            {
                SetState(stateData.GetDefaultState());
            }
            
            if (showDebugInfo)
            {
                Debug.Log("[GameStateManager] Game State Manager initialized");
            }
        }
        
        private void Start()
        {
            // Subscribe to scene events
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }
        
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        
        /// <summary>
        /// Set the current game state
        /// </summary>
        public void SetState(Data.GameState newState)
        {
            if (_isTransitioning)
            {
                Debug.LogWarning("[GameStateManager] Cannot change state while transitioning");
                return;
            }
            
            if (_currentState == newState)
            {
                return;
            }
            
            _previousState = _currentState;
            _currentState = newState;
            _currentStateData = stateData.GetStateData(newState);
            
            if (showDebugInfo)
            {
                Debug.Log($"[GameStateManager] State changed from {_previousState} to {_currentState}");
            }
            
            // Apply state configuration
            ApplyStateConfiguration();
            
            // Trigger state change event
            EventBusHelper.Trigger("GameState.Changed", newState);
            EventBusHelper.Trigger("GameState.StateChanged");
            
            // Handle scene transition if needed
            if (!string.IsNullOrEmpty(_currentStateData.targetScene) && 
                _currentStateData.targetScene != _currentScene)
            {
                LoadScene(_currentStateData.targetScene);
            }
        }
        
        /// <summary>
        /// Apply the current state's configuration
        /// </summary>
        private void ApplyStateConfiguration()
        {
            if (_currentStateData == null) return;
            
            // Time scale
            if (_currentStateData.allowTimeScale)
            {
                Time.timeScale = _currentStateData.timeScale;
            }
            
            // Cursor
            Cursor.visible = _currentStateData.showCursor;
            Cursor.lockState = _currentStateData.cursorLockMode;
            
            // Trigger enter event
            if (!string.IsNullOrEmpty(_currentStateData.onEnterEvent))
            {
                EventBusHelper.Trigger(_currentStateData.onEnterEvent);
            }
        }
        
        /// <summary>
        /// Load a scene with optional loading screen
        /// </summary>
        public void LoadScene(string sceneName)
        {
            if (_isTransitioning)
            {
                Debug.LogWarning("[GameStateManager] Scene transition already in progress");
                return;
            }
            
            _targetScene = sceneName;
            _isTransitioning = true;
            
            if (showDebugInfo)
            {
                Debug.Log($"[GameStateManager] Loading scene: {sceneName}");
            }
            
            // Trigger scene loading event
            EventBusHelper.Trigger("GameState.SceneLoading", sceneName);
            
            // Start scene transition
            if (_sceneTransitionCoroutine != null)
            {
                StopCoroutine(_sceneTransitionCoroutine);
            }
            _sceneTransitionCoroutine = StartCoroutine(SceneTransitionCoroutine(sceneName));
        }
        
        /// <summary>
        /// Scene transition coroutine with loading screen
        /// </summary>
        private IEnumerator SceneTransitionCoroutine(string sceneName)
        {
            // Show loading screen if available
            if (!string.IsNullOrEmpty(loadingSceneName))
            {
                var loadingScene = SceneManager.GetSceneByName(loadingSceneName);
                if (loadingScene.isLoaded)
                {
                    SceneManager.SetActiveScene(loadingScene);
                }
                else
                {
                    SceneManager.LoadScene(loadingSceneName, LoadSceneMode.Additive);
                    yield return new WaitForSeconds(0.1f);
                }
            }
            
            // Load the target scene
            var loadOperation = SceneManager.LoadSceneAsync(sceneName);
            loadOperation.allowSceneActivation = false;
            
            // Wait for minimum loading time
            var startTime = Time.time;
            while (Time.time - startTime < minimumLoadingTime)
            {
                yield return null;
            }
            
            // Wait for scene to load
            while (loadOperation.progress < 0.9f)
            {
                yield return null;
            }
            
            // Activate the scene
            loadOperation.allowSceneActivation = true;
            
            // Wait for scene activation
            while (!loadOperation.isDone)
            {
                yield return null;
            }
            
            // Unload loading scene if it was loaded additively
            if (!string.IsNullOrEmpty(loadingSceneName))
            {
                var loadingScene = SceneManager.GetSceneByName(loadingSceneName);
                if (loadingScene.isLoaded)
                {
                    SceneManager.UnloadSceneAsync(loadingScene);
                }
            }
            
            _isTransitioning = false;
            _currentScene = sceneName;
            
            // Trigger scene loaded event
            EventBusHelper.Trigger("GameState.SceneLoaded", sceneName);
            
            if (showDebugInfo)
            {
                Debug.Log($"[GameStateManager] Scene loaded: {sceneName}");
            }
        }
        
        /// <summary>
        /// Pause the game
        /// </summary>
        public void PauseGame()
        {
            if (_isPaused) return;
            
            _isPaused = true;
            Time.timeScale = 0f;
            
            if (showDebugInfo)
            {
                Debug.Log("[GameStateManager] Game paused");
            }
            
            EventBusHelper.Trigger("GameState.Paused");
        }
        
        /// <summary>
        /// Resume the game
        /// </summary>
        public void ResumeGame()
        {
            if (!_isPaused) return;
            
            _isPaused = false;
            Time.timeScale = 1f;
            
            if (showDebugInfo)
            {
                Debug.Log("[GameStateManager] Game resumed");
            }
            
            EventBusHelper.Trigger("GameState.Resumed");
        }
        
        /// <summary>
        /// Toggle pause state
        /// </summary>
        public void TogglePause()
        {
            if (_isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
        
        /// <summary>
        /// Go to previous state
        /// </summary>
        public void GoToPreviousState()
        {
            if (_previousState != _currentState)
            {
                SetState(_previousState);
            }
        }
        
        /// <summary>
        /// Check if current state allows a specific action
        /// </summary>
        public bool CanPerformAction(string action)
        {
            if (_currentStateData == null) return false;
            
            return _currentStateData.allowedActions.Contains(action);
        }
        
        /// <summary>
        /// Handle scene loaded event
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _currentScene = scene.name;
        }
        
        /// <summary>
        /// Handle scene unloaded event
        /// </summary>
        private void OnSceneUnloaded(Scene scene)
        {
            // Clean up scene-specific resources if needed
        }
        
        // Convenience methods
        public void GoToMainMenu() => SetState(Data.GameState.MainMenu);
        public void StartGame() => SetState(Data.GameState.Playing);
        public void CompleteLevel() => SetState(Data.GameState.LevelCompleted);
        public void GameOver() => SetState(Data.GameState.GameOver);
        public void Victory() => SetState(Data.GameState.Victory);
        public void ShowSettings() => SetState(Data.GameState.Settings);
        public void ShowLevelSelect() => SetState(Data.GameState.LevelSelect);
    }
} 
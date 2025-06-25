using UnityEngine;
using UnityEditor;
using Pilipala.GameState;
using Pilipala.GameState.Data;
using EventSystem;

namespace Pilipala.GameState.Editor
{
    /// <summary>
    /// Editor window for monitoring and testing game states
    /// </summary>
    public class GameStateWindow : EditorWindow
    {
        private GameStateManager _gameStateManager;
        private Vector2 _scrollPosition;
        private bool _autoRefresh = true;
        private float _lastRefreshTime;
        
        [MenuItem("PiliPala/Game State/Game State Monitor")]
        public static void ShowWindow()
        {
            GetWindow<GameStateWindow>("Game State Monitor");
        }
        
        private void OnEnable()
        {
            EditorApplication.update += OnEditorUpdate;
        }
        
        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
        }
        
        private void OnEditorUpdate()
        {
            if (_autoRefresh && Time.realtimeSinceStartup - _lastRefreshTime > 0.1f)
            {
                Repaint();
                _lastRefreshTime = Time.realtimeSinceStartup;
            }
        }
        
        private void OnGUI()
        {
            _gameStateManager = GameStateManager.Instance;
            
            if (_gameStateManager == null)
            {
                EditorGUILayout.HelpBox("No GameStateManager found in scene. Create one to start monitoring.", MessageType.Info);
                
                if (GUILayout.Button("Create GameStateManager"))
                {
                    CreateGameStateManager();
                }
                
                return;
            }
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Game State Monitor", EditorStyles.boldLabel);
            _autoRefresh = EditorGUILayout.Toggle("Auto Refresh", _autoRefresh);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            
            // Current State Information
            EditorGUILayout.LabelField("Current State Information", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            EditorGUILayout.LabelField("Current State", _gameStateManager.CurrentState.ToString());
            EditorGUILayout.LabelField("Previous State", _gameStateManager.PreviousState.ToString());
            EditorGUILayout.LabelField("Current Scene", _gameStateManager.CurrentScene);
            EditorGUILayout.LabelField("Is Paused", _gameStateManager.IsPaused.ToString());
            EditorGUILayout.LabelField("Is Transitioning", _gameStateManager.IsTransitioning.ToString());
            
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            
            // Quick State Actions
            EditorGUILayout.LabelField("Quick State Actions", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Main Menu"))
            {
                _gameStateManager.GoToMainMenu();
            }
            if (GUILayout.Button("Start Game"))
            {
                _gameStateManager.StartGame();
            }
            if (GUILayout.Button("Pause"))
            {
                _gameStateManager.PauseGame();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Resume"))
            {
                _gameStateManager.ResumeGame();
            }
            if (GUILayout.Button("Level Complete"))
            {
                _gameStateManager.CompleteLevel();
            }
            if (GUILayout.Button("Game Over"))
            {
                _gameStateManager.GameOver();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Victory"))
            {
                _gameStateManager.Victory();
            }
            if (GUILayout.Button("Settings"))
            {
                _gameStateManager.ShowSettings();
            }
            if (GUILayout.Button("Level Select"))
            {
                _gameStateManager.ShowLevelSelect();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            
            // State Testing
            EditorGUILayout.LabelField("State Testing", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            // Dropdown for state selection
            Data.GameState selectedState = (Data.GameState)EditorGUILayout.EnumPopup("Select State", _gameStateManager.CurrentState);
            if (selectedState != _gameStateManager.CurrentState)
            {
                _gameStateManager.SetState(selectedState);
            }
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Previous State"))
            {
                _gameStateManager.GoToPreviousState();
            }
            if (GUILayout.Button("Toggle Pause"))
            {
                _gameStateManager.TogglePause();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            
            // Scene Management
            EditorGUILayout.LabelField("Scene Management", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Scene Name", GUILayout.Width(100));
            string sceneName = EditorGUILayout.TextField("MainMenu");
            if (GUILayout.Button("Load Scene", GUILayout.Width(100)))
            {
                _gameStateManager.LoadScene(sceneName);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            
            // Action Permissions
            EditorGUILayout.LabelField("Action Permissions", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            EditorGUILayout.LabelField("Can Input", _gameStateManager.CanPerformAction("input").ToString());
            EditorGUILayout.LabelField("Can Audio", _gameStateManager.CanPerformAction("audio").ToString());
            EditorGUILayout.LabelField("Can UI", _gameStateManager.CanPerformAction("ui").ToString());
            EditorGUILayout.LabelField("Can Scene", _gameStateManager.CanPerformAction("scene").ToString());
            EditorGUILayout.LabelField("Can Pause", _gameStateManager.CanPerformAction("pause").ToString());
            
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            
            // Event Testing
            EditorGUILayout.LabelField("Event Testing", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Trigger State Changed"))
            {
                EventBusHelper.Trigger("GameState.Changed", _gameStateManager.CurrentState);
            }
            if (GUILayout.Button("Trigger Pause"))
            {
                EventBusHelper.Trigger("GameState.Paused", true);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Trigger Resume"))
            {
                EventBusHelper.Trigger("GameState.Resumed");
            }
            if (GUILayout.Button("Trigger Scene Loading"))
            {
                EventBusHelper.Trigger("GameState.SceneLoading", "TestScene");
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.indentLevel--;
            
            EditorGUILayout.EndScrollView();
        }
        
        private void CreateGameStateManager()
        {
            GameObject go = new GameObject("GameStateManager");
            go.AddComponent<GameStateManager>();
            Selection.activeGameObject = go;
        }
    }
} 
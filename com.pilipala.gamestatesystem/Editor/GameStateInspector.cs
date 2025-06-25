using UnityEngine;
using UnityEditor;
using Pilipala.GameState;
using Pilipala.GameState.Data;

namespace Pilipala.GameState.Editor
{
    /// <summary>
    /// Custom inspector for GameStateManager with testing capabilities
    /// </summary>
    [CustomEditor(typeof(GameStateManager))]
    public class GameStateInspector : UnityEditor.Editor
    {
        private GameStateManager _gameStateManager;
        private bool _showDebugInfo = true;
        private bool _showStateTesting = true;
        private bool _showSceneTesting = true;
        
        private void OnEnable()
        {
            _gameStateManager = (GameStateManager)target;
        }
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            EditorGUILayout.Space();
            
            // Debug Information
            _showDebugInfo = EditorGUILayout.Foldout(_showDebugInfo, "Debug Information");
            if (_showDebugInfo)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.LabelField("Current State", _gameStateManager.CurrentState.ToString());
                EditorGUILayout.LabelField("Previous State", _gameStateManager.PreviousState.ToString());
                EditorGUILayout.LabelField("Current Scene", _gameStateManager.CurrentScene);
                EditorGUILayout.LabelField("Is Paused", _gameStateManager.IsPaused.ToString());
                EditorGUILayout.LabelField("Is Transitioning", _gameStateManager.IsTransitioning.ToString());
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space();
            
            // State Testing
            _showStateTesting = EditorGUILayout.Foldout(_showStateTesting, "State Testing");
            if (_showStateTesting)
            {
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
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Pause"))
                {
                    _gameStateManager.PauseGame();
                }
                if (GUILayout.Button("Resume"))
                {
                    _gameStateManager.ResumeGame();
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
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
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Level Select"))
                {
                    _gameStateManager.ShowLevelSelect();
                }
                if (GUILayout.Button("Previous State"))
                {
                    _gameStateManager.GoToPreviousState();
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space();
            
            // Scene Testing
            _showSceneTesting = EditorGUILayout.Foldout(_showSceneTesting, "Scene Testing");
            if (_showSceneTesting)
            {
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
            }
            
            // Repaint to update debug info
            if (GUI.changed)
            {
                Repaint();
            }
        }
    }
} 
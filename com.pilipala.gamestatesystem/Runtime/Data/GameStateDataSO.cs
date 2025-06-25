using UnityEngine;
using System.Collections.Generic;

namespace Pilipala.GameState.Data
{
    /// <summary>
    /// ScriptableObject containing all game state configurations
    /// </summary>
    [CreateAssetMenu(fileName = "GameStateData", menuName = "ScriptableObjects/GameState/GameStateData", order = 1)]
    public class GameStateDataSO : ScriptableObject
    {
        [Header("Game State Configurations")]
        [SerializeField] private List<GameStateData> stateConfigurations = new List<GameStateData>();
        
        [Header("Default Settings")]
        [SerializeField] private GameState defaultState = GameState.MainMenu;
        [SerializeField] private string defaultScene = "MainMenu";
        
        /// <summary>
        /// Get configuration for a specific game state
        /// </summary>
        public GameStateData GetStateData(GameState state)
        {
            foreach (var config in stateConfigurations)
            {
                if (config.state == state)
                {
                    return config;
                }
            }
            
            Debug.LogWarning($"No configuration found for state: {state}. Using default configuration.");
            return CreateDefaultStateData(state);
        }
        
        /// <summary>
        /// Get all state configurations
        /// </summary>
        public List<GameStateData> GetAllStateData()
        {
            return stateConfigurations;
        }
        
        /// <summary>
        /// Get the default state
        /// </summary>
        public GameState GetDefaultState()
        {
            return defaultState;
        }
        
        /// <summary>
        /// Get the default scene
        /// </summary>
        public string GetDefaultScene()
        {
            return defaultScene;
        }
        
        /// <summary>
        /// Create a default state data configuration
        /// </summary>
        private GameStateData CreateDefaultStateData(GameState state)
        {
            var defaultData = new GameStateData
            {
                state = state,
                allowTimeScale = true,
                timeScale = 1f,
                allowInput = true,
                allowAudio = true,
                allowUI = true,
                allowSceneTransitions = true,
                canBePaused = true,
                autoSave = false,
                showCursor = true,
                cursorLockMode = CursorLockMode.None,
                allowedActions = new List<string> { "input", "audio", "ui", "scene", "pause" }
            };
            
            // Set state-specific defaults
            switch (state)
            {
                case GameState.Loading:
                case GameState.LoadingScreen:
                    defaultData.allowTimeScale = false;
                    defaultData.timeScale = 0f;
                    defaultData.allowInput = false;
                    defaultData.allowUI = false;
                    defaultData.allowedActions = new List<string> { "audio" };
                    break;
                    
                case GameState.Paused:
                    defaultData.allowTimeScale = false;
                    defaultData.timeScale = 0f;
                    defaultData.allowInput = false;
                    defaultData.allowAudio = false;
                    defaultData.allowedActions = new List<string> { "ui", "pause" };
                    break;
                    
                case GameState.Cutscene:
                    defaultData.allowInput = false;
                    defaultData.canBePaused = false;
                    defaultData.allowedActions = new List<string> { "audio", "ui" };
                    break;
                    
                case GameState.GameOver:
                case GameState.Victory:
                    defaultData.allowTimeScale = false;
                    defaultData.timeScale = 0f;
                    defaultData.allowInput = false;
                    defaultData.allowedActions = new List<string> { "ui" };
                    break;
            }
            
            return defaultData;
        }
    }
} 
using UnityEngine;
using Pilipala.GameState.Data;
using EventSystem;

namespace Pilipala.GameState
{
    /// <summary>
    /// Helper class for easy game state management through events
    /// </summary>
    public static class GameStateHelper
    {
        /// <summary>
        /// Set the current game state
        /// </summary>
        public static void SetState(Data.GameState state)
        {
            EventBusHelper.Trigger("GameState.SetState", state);
        }
        
        /// <summary>
        /// Pause the game
        /// </summary>
        public static void PauseGame()
        {
            EventBusHelper.Trigger("GameState.Pause");
        }
        
        /// <summary>
        /// Resume the game
        /// </summary>
        public static void ResumeGame()
        {
            EventBusHelper.Trigger("GameState.Resume");
        }
        
        /// <summary>
        /// Toggle pause state
        /// </summary>
        public static void TogglePause()
        {
            EventBusHelper.Trigger("GameState.TogglePause");
        }
        
        /// <summary>
        /// Load a scene
        /// </summary>
        public static void LoadScene(string sceneName)
        {
            EventBusHelper.Trigger("GameState.LoadScene", sceneName);
        }
        
        /// <summary>
        /// Go to main menu
        /// </summary>
        public static void GoToMainMenu()
        {
            EventBusHelper.Trigger("GameState.GoToMainMenu");
        }
        
        /// <summary>
        /// Start the game
        /// </summary>
        public static void StartGame()
        {
            EventBusHelper.Trigger("GameState.StartGame");
        }
        
        /// <summary>
        /// Complete the current level
        /// </summary>
        public static void CompleteLevel()
        {
            EventBusHelper.Trigger("GameState.CompleteLevel");
        }
        
        /// <summary>
        /// Trigger game over
        /// </summary>
        public static void GameOver()
        {
            EventBusHelper.Trigger("GameState.GameOver");
        }
        
        /// <summary>
        /// Trigger victory
        /// </summary>
        public static void Victory()
        {
            EventBusHelper.Trigger("GameState.Victory");
        }
        
        /// <summary>
        /// Show settings menu
        /// </summary>
        public static void ShowSettings()
        {
            EventBusHelper.Trigger("GameState.ShowSettings");
        }
        
        /// <summary>
        /// Show level select
        /// </summary>
        public static void ShowLevelSelect()
        {
            EventBusHelper.Trigger("GameState.ShowLevelSelect");
        }
        
        /// <summary>
        /// Go to previous state
        /// </summary>
        public static void GoToPreviousState()
        {
            EventBusHelper.Trigger("GameState.GoToPreviousState");
        }
        
        /// <summary>
        /// Check if current state allows a specific action
        /// </summary>
        public static bool CanPerformAction(string action)
        {
            var manager = GameStateManager.Instance;
            return manager != null && manager.CanPerformAction(action);
        }
        
        /// <summary>
        /// Get current game state
        /// </summary>
        public static Data.GameState GetCurrentState()
        {
            var manager = GameStateManager.Instance;
            return manager != null ? manager.CurrentState : Data.GameState.Loading;
        }
        
        /// <summary>
        /// Check if game is paused
        /// </summary>
        public static bool IsPaused()
        {
            var manager = GameStateManager.Instance;
            return manager != null && manager.IsPaused;
        }
        
        /// <summary>
        /// Check if game is transitioning
        /// </summary>
        public static bool IsTransitioning()
        {
            var manager = GameStateManager.Instance;
            return manager != null && manager.IsTransitioning;
        }
    }
} 
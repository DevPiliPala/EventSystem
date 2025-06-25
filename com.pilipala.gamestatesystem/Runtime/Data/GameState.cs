using System;

namespace Pilipala.GameState.Data
{
    /// <summary>
    /// Defines all possible game states
    /// </summary>
    [Serializable]
    public enum GameState
    {
        /// <summary>
        /// Initial state when game is loading
        /// </summary>
        Loading,
        
        /// <summary>
        /// Main menu state
        /// </summary>
        MainMenu,
        
        /// <summary>
        /// Settings menu state
        /// </summary>
        Settings,
        
        /// <summary>
        /// Level selection state
        /// </summary>
        LevelSelect,
        
        /// <summary>
        /// Active gameplay state
        /// </summary>
        Playing,
        
        /// <summary>
        /// Game is paused
        /// </summary>
        Paused,
        
        /// <summary>
        /// Level completed state
        /// </summary>
        LevelCompleted,
        
        /// <summary>
        /// Game over state
        /// </summary>
        GameOver,
        
        /// <summary>
        /// Victory/win state
        /// </summary>
        Victory,
        
        /// <summary>
        /// Credits/ending state
        /// </summary>
        Credits,
        
        /// <summary>
        /// Loading screen between scenes
        /// </summary>
        LoadingScreen,
        
        /// <summary>
        /// Tutorial state
        /// </summary>
        Tutorial,
        
        /// <summary>
        /// Cutscene state
        /// </summary>
        Cutscene
    }
} 
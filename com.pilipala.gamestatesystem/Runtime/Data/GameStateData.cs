using UnityEngine;
using System;
using System.Collections.Generic;

namespace Pilipala.GameState.Data
{
    /// <summary>
    /// Configuration data for a specific game state
    /// </summary>
    [Serializable]
    public class GameStateData
    {
        [Header("State Configuration")]
        [Tooltip("The game state this data represents")]
        public GameState state;
        
        [Tooltip("Whether this state allows time to pass (false = time frozen)")]
        public bool allowTimeScale = true;
        
        [Tooltip("Time scale multiplier for this state (1 = normal, 0 = frozen)")]
        [Range(0f, 2f)]
        public float timeScale = 1f;
        
        [Tooltip("Whether this state allows input")]
        public bool allowInput = true;
        
        [Tooltip("Whether this state allows audio")]
        public bool allowAudio = true;
        
        [Tooltip("Whether this state allows UI interactions")]
        public bool allowUI = true;
        
        [Tooltip("Whether this state allows scene transitions")]
        public bool allowSceneTransitions = true;
        
        [Header("Scene Management")]
        [Tooltip("Scene name to load when entering this state (empty = keep current)")]
        public string targetScene = "";
        
        [Tooltip("Whether to unload previous scene when loading new one")]
        public bool unloadPreviousScene = true;
        
        [Tooltip("Loading screen to show during scene transition")]
        public GameObject loadingScreenPrefab;
        
        [Header("State Behavior")]
        [Tooltip("Whether this state can be paused")]
        public bool canBePaused = true;
        
        [Tooltip("Whether this state should auto-save")]
        public bool autoSave = false;
        
        [Tooltip("Whether this state should show cursor")]
        public bool showCursor = true;
        
        [Tooltip("Cursor lock mode for this state")]
        public CursorLockMode cursorLockMode = CursorLockMode.None;
        
        [Header("Allowed Actions")]
        [Tooltip("List of actions allowed in this state")]
        public List<string> allowedActions = new List<string>();
        
        [Header("Events")]
        [Tooltip("Event to trigger when entering this state")]
        public string onEnterEvent = "";
        
        [Tooltip("Event to trigger when exiting this state")]
        public string onExitEvent = "";
        
        [Tooltip("Event to trigger when this state is updated")]
        public string onUpdateEvent = "";
    }
} 
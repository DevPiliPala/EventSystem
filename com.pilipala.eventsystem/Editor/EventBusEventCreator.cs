using UnityEditor;
using UnityEngine;
using EventSystem;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace EventSystem.Editor {
    /// <summary>
    /// Editor window for creating and managing event bus events
    /// </summary>
    public class EventBusEventCreator : EditorWindow {
        private string _eventName = "";
        private EventDomain _selectedDomain = EventDomain.Core;
        private string _eventDescription = "";
        private bool _createInRuntime = true;
        private bool _createPersistent = true;
        private EventBusEventsAsset _eventsAsset;
        private Vector2 _scrollPosition;
        private string _payloadTypeName = "";
        private bool _isGenericEvent = false;
        private List<EventData> _existingEvents = new List<EventData>();
        private bool _showExistingEvents = true;

        // Collapsed state dictionaries
        private Dictionary<string, bool> _domainFoldouts = new Dictionary<string, bool>();
        private Dictionary<string, Dictionary<string, bool>> _categoryFoldouts = new Dictionary<string, Dictionary<string, bool>>();

        // Pastel color mapping for domains
        private static readonly Dictionary<EventDomain, Color> _domainColors = new Dictionary<EventDomain, Color> {
            { EventDomain.Gameplay, new Color(0.64f, 0.79f, 0.98f) },      // #A3C9F9
            { EventDomain.Input, new Color(0.71f, 0.92f, 0.84f) },         // #B5EAD7
            { EventDomain.Presentation, new Color(1.0f, 0.85f, 0.76f) },  // #FFDAC1
            { EventDomain.Core, new Color(1.0f, 0.72f, 0.7f) },           // #FFB7B2
            { EventDomain.Infrastructure, new Color(0.89f, 0.94f, 0.80f) },// #E2F0CB
            { EventDomain.Editor, new Color(0.78f, 0.81f, 0.92f) },       // #C7CEEA
            { EventDomain.Custom, new Color(1.0f, 0.98f, 0.80f) }         // #FFFACD
        };

        [MenuItem("PiliPala/Event Bus/Event Creator")]
        public static void ShowWindow() {
            GetWindow<EventBusEventCreator>("Event Creator");
        }

        private void OnEnable() {
            LoadOrCreateEventsAsset();
            RefreshExistingEvents();
        }

        // Helper to get best contrast text color (black or white)
        private Color GetContrastColor(Color bg)
        {
            float luminance = 0.299f * bg.r + 0.587f * bg.g + 0.114f * bg.b;
            return luminance > 0.6f ? Color.black : Color.white;
        }

        private void OnGUI() {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Event Bus Event Creator", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Create and manage events for the Event Bus system", EditorStyles.miniLabel);
            EditorGUILayout.Space();
            
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            
            // Create new event section
            EditorGUILayout.LabelField("Create New Event", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            _eventName = EditorGUILayout.TextField("Event Name", _eventName);
            _selectedDomain = (EventDomain)EditorGUILayout.EnumPopup("Event Domain", _selectedDomain);
            _eventDescription = EditorGUILayout.TextField("Event Description (optional)", _eventDescription);
            _isGenericEvent = EditorGUILayout.Toggle("Is Generic (Payload)", _isGenericEvent);
            if (_isGenericEvent) {
                _payloadTypeName = EditorGUILayout.TextField("Payload Type (e.g. string, int, MyType)", _payloadTypeName);
            } else {
                _payloadTypeName = "";
            }
            
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create Event"))
            {
                CreateEvent();
            }
            if (GUILayout.Button("Clear"))
            {
                _eventName = "";
                _eventDescription = "";
                _payloadTypeName = "";
                _isGenericEvent = false;
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            
            // Existing events section
            _showExistingEvents = EditorGUILayout.Foldout(_showExistingEvents, "Existing Events", true);
            if (_showExistingEvents)
            {
                EditorGUI.indentLevel++;
                
                if (_existingEvents.Count == 0)
                {
                    EditorGUILayout.HelpBox("No events found. Create some events to see them here.", MessageType.Info);
                }
                else
                {
                    foreach (var eventData in _existingEvents)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(eventData.eventName, GUILayout.Width(200));
                        EditorGUILayout.LabelField(eventData.domain.ToString(), GUILayout.Width(100));
                        
                        if (GUILayout.Button("Delete", GUILayout.Width(60)))
                        {
                            DeleteEvent(eventData);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                
                EditorGUILayout.Space();
                if (GUILayout.Button("Refresh Events"))
                {
                    RefreshExistingEvents();
                }
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndScrollView();
        }

        private void LoadOrCreateEventsAsset() {
            // Try to load existing asset
            _eventsAsset = Resources.Load<EventBusEventsAsset>("EventBusEvents");
            
            if (_eventsAsset == null) {
                // Create the Resources folder if it doesn't exist
                if (!Directory.Exists("Assets/Resources")) {
                    Directory.CreateDirectory("Assets/Resources");
                    AssetDatabase.Refresh();
                }
                
                // Create new asset
                _eventsAsset = ScriptableObject.CreateInstance<EventBusEventsAsset>();
                AssetDatabase.CreateAsset(_eventsAsset, "Assets/Resources/EventBusEvents.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
                Debug.Log("[EventBusEventCreator] Created new EventBusEvents asset at Assets/Resources/EventBusEvents.asset");
            }
        }

        private void CreateEvent() {
            if (string.IsNullOrEmpty(_eventName)) {
                EditorUtility.DisplayDialog("Error", "Please enter an event name.", "OK");
                return;
            }

            // Validate event name format
            if (!IsValidEventName(_eventName)) {
                EditorUtility.DisplayDialog("Error", 
                    "Event name should follow the format: domain.category.action\n" +
                    "Example: gameplay.player.died", "OK");
                return;
            }

            // Check for type conflict
            if (_eventsAsset != null && _eventsAsset.HasEvent(_eventName)) {
                var existing = _eventsAsset.PersistentEvents.Find(e => e.eventName == _eventName);
                bool existingIsGeneric = !string.IsNullOrEmpty(existing.eventDescription) && existing.eventDescription.Contains("Payload:");
                if (existingIsGeneric != _isGenericEvent) {
                    EditorUtility.DisplayDialog("Type Conflict", $"Event '{_eventName}' already exists with a different type (generic/non-generic).", "OK");
                    return;
                }
            }

            bool success = false;

            // Create persistent event
            if (_createPersistent && _eventsAsset != null) {
                var newEventData = new EventData {
                    eventName = _eventName,
                    domain = _selectedDomain,
                    subscriberCount = 0,
                    isActive = true,
                    lastTriggeredTime = System.DateTime.Now.ToString("HH:mm:ss"),
                    eventDescription = _isGenericEvent && !string.IsNullOrEmpty(_payloadTypeName) ? $"Payload: {_payloadTypeName}" : _eventDescription
                };
                
                _eventsAsset.AddEvent(newEventData);
                EditorUtility.SetDirty(_eventsAsset);
                AssetDatabase.SaveAssets();
                success = true;
                
                Debug.Log($"[EventBusEventCreator] Created persistent event: {_eventName} in domain: {_selectedDomain}");
            }

            // Create in runtime
            if (_createInRuntime && EventBus.IsAvailable) {
                EventBusHelper.CreateEvent(_eventName, _selectedDomain);
                success = true;
                
                Debug.Log($"[EventBusEventCreator] Created runtime event: {_eventName} in domain: {_selectedDomain}");
            } else if (_createInRuntime && !EventBus.IsAvailable) {
                EditorUtility.DisplayDialog("Warning", 
                    "EventBus is not available in the current scene.\n" +
                    "The event will be created when first triggered.", "OK");
            }

            if (success) {
                EditorUtility.DisplayDialog("Success", 
                    $"Event '{_eventName}' created successfully!", "OK");
                ClearFields();
            }
        }

        private void DeleteEvent(EventData eventData) {
            if (EditorUtility.DisplayDialog("Confirm Delete", 
                $"Are you sure you want to delete the event '{eventData.eventName}'?", 
                "Delete", "Cancel")) {
                if (_eventsAsset != null) {
                    _eventsAsset.RemoveEvent(eventData.eventName);
                    EditorUtility.SetDirty(_eventsAsset);
                    AssetDatabase.SaveAssets();
                    
                    Debug.Log($"[EventBusEventCreator] Deleted persistent event: {eventData.eventName}");
                }
                RefreshExistingEvents();
            }
        }

        private void RefreshExistingEvents() {
            _existingEvents = EventBusHelper.GetAllEvents();
        }

        private bool IsValidEventName(string name) {
            if (string.IsNullOrEmpty(name)) return false;
            
            var parts = name.Split('.');
            if (parts.Length < 2) return false;
            
            // Check if the first part matches a valid domain
            var domainString = parts[0].ToLower();
            var validDomains = new[] { "core", "editor", "gameplay", "infrastructure", "input", "presentation" };
            
            return System.Array.Exists(validDomains, domain => domain == domainString);
        }

        private void ClearFields() {
            _eventName = "";
            _eventDescription = "";
            _payloadTypeName = "";
            _isGenericEvent = false;
        }

        private string GetCategory(string eventName) {
            var parts = eventName.Split('.');
            return parts.Length > 1 ? parts[1] : "(none)";
        }
    }
}
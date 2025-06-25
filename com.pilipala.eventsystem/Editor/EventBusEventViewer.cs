using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using EventSystem;

namespace EventSystem.Editor {
    /// <summary>
    /// Editor window for viewing and monitoring Event Bus events
    /// </summary>
    public class EventBusEventViewer : EditorWindow {
        private Vector2 _scrollPosition;
        private EventDomain _selectedDomain = EventDomain.Core;
        private bool _showAllDomains = true;
        private bool _autoRefresh = true;
        private float _lastRefreshTime;
        private List<EventData> _allEvents = new List<EventData>();
        private Dictionary<EventDomain, List<EventData>> _eventsByDomain = new Dictionary<EventDomain, List<EventData>>();
        
        [MenuItem("PiliPala/Event Bus/Event Viewer")]
        public static void ShowWindow() {
            GetWindow<EventBusEventViewer>("Event Viewer");
        }
        
        private void OnEnable() {
            EditorApplication.update += OnEditorUpdate;
            RefreshEvents();
        }
        
        private void OnDisable() {
            EditorApplication.update -= OnEditorUpdate;
        }
        
        private void OnEditorUpdate() {
            if (_autoRefresh && Time.realtimeSinceStartup - _lastRefreshTime > 0.5f) {
                RefreshEvents();
                Repaint();
                _lastRefreshTime = Time.realtimeSinceStartup;
            }
        }
        
        private void OnGUI() {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Event Bus Event Viewer", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Monitor and inspect Event Bus events", EditorStyles.miniLabel);
            EditorGUILayout.Space();
            
            // Controls
            EditorGUILayout.BeginHorizontal();
            _autoRefresh = EditorGUILayout.Toggle("Auto Refresh", _autoRefresh);
            if (GUILayout.Button("Refresh Now")) {
                RefreshEvents();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // Domain filter
            EditorGUILayout.BeginHorizontal();
            _showAllDomains = EditorGUILayout.Toggle("Show All Domains", _showAllDomains);
            if (!_showAllDomains) {
                _selectedDomain = (EventDomain)EditorGUILayout.EnumPopup("Domain", _selectedDomain);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // Event statistics
            EditorGUILayout.LabelField("Event Statistics", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Total Events", _allEvents.Count.ToString());
            EditorGUILayout.LabelField("Active Events", _allEvents.Count(e => e.isActive).ToString());
            EditorGUILayout.LabelField("Total Subscribers", _allEvents.Sum(e => e.subscriberCount).ToString());
            EditorGUI.indentLevel--;
            
            EditorGUILayout.Space();
            
            // Events list
            EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            
            var eventsToShow = _showAllDomains ? _allEvents : _eventsByDomain.GetValueOrDefault(_selectedDomain, new List<EventData>());
            
            if (eventsToShow.Count == 0) {
                EditorGUILayout.HelpBox("No events found.", MessageType.Info);
            } else {
                foreach (var eventData in eventsToShow) {
                    DrawEventItem(eventData);
                }
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        private void DrawEventItem(EventData eventData) {
            EditorGUILayout.BeginVertical("box");
            
            // Event name and domain
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(eventData.eventName, EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"({eventData.domain})", EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();
            
            // Event details
            EditorGUI.indentLevel++;
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Status", GUILayout.Width(60));
            EditorGUILayout.LabelField(eventData.isActive ? "Active" : "Inactive", 
                eventData.isActive ? EditorStyles.boldLabel : EditorStyles.label);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Subscribers", GUILayout.Width(60));
            EditorGUILayout.LabelField(eventData.subscriberCount.ToString());
            EditorGUILayout.EndHorizontal();
            
            if (!string.IsNullOrEmpty(eventData.lastTriggeredTime)) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Last Triggered", GUILayout.Width(80));
                EditorGUILayout.LabelField(eventData.lastTriggeredTime);
                EditorGUILayout.EndHorizontal();
            }
            
            if (!string.IsNullOrEmpty(eventData.eventDescription)) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Description", GUILayout.Width(60));
                EditorGUILayout.LabelField(eventData.eventDescription, EditorStyles.wordWrappedLabel);
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUI.indentLevel--;
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }
        
        private void RefreshEvents() {
            _allEvents = EventBusHelper.GetAllEvents();
            
            // Group events by domain
            _eventsByDomain.Clear();
            foreach (var eventData in _allEvents) {
                if (!_eventsByDomain.ContainsKey(eventData.domain)) {
                    _eventsByDomain[eventData.domain] = new List<EventData>();
                }
                _eventsByDomain[eventData.domain].Add(eventData);
            }
        }
    }
}
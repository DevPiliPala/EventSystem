using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using EventSystem;

namespace EventSystem.Editor {
[CustomEditor(typeof(EventBusInspector))]
public class EventBusInspectorEditor : UnityEditor.Editor {
    private bool showEventList = true;
    private bool showSubscriberList = false;
    private string selectedEvent = "";
    private Vector2 scrollPosition;

    public override void OnInspectorGUI() {
        EventBusInspector inspector = (EventBusInspector)target;
        
        DrawDefaultInspector();
        
        GUILayout.Space(10);
        
        if (!EventSystem.EventBus.IsAvailable) {
            EditorGUILayout.HelpBox("EventBus is not available in the current scene.", MessageType.Warning);
            return;
        }
        
        // Event statistics
        var events = EventBusHelper.GetAllEvents();
        EditorGUILayout.LabelField($"Total Events: {events.Count}", EditorStyles.boldLabel);
        
        // Domain breakdown
        var domainStats = new Dictionary<EventDomain, int>();
        foreach (var eventData in events) {
            if (!domainStats.ContainsKey(eventData.domain)) {
                domainStats[eventData.domain] = 0;
            }
            domainStats[eventData.domain]++;
        }
        
        EditorGUILayout.LabelField("Events by Domain:", EditorStyles.boldLabel);
        foreach (var kvp in domainStats) {
            EditorGUILayout.LabelField($"  {kvp.Key}: {kvp.Value} events");
        }
        
        GUILayout.Space(10);
        
        // Event list
        showEventList = EditorGUILayout.Foldout(showEventList, "Event List");
        if (showEventList) {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));
            
            foreach (var eventData in events) {
                EditorGUILayout.BeginVertical("box");
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(eventData.eventName, EditorStyles.boldLabel);
                EditorGUILayout.LabelField(eventData.domain.ToString(), GUILayout.Width(80));
                EditorGUILayout.LabelField($"Subs: {eventData.subscriberCount}", GUILayout.Width(60));
                EditorGUILayout.LabelField(eventData.lastTriggeredTime, GUILayout.Width(100));
                EditorGUILayout.LabelField($"Type: {(string.IsNullOrEmpty(eventData.eventDescription) ? "Non-Generic" : eventData.eventDescription)}", GUILayout.Width(120));
                EditorGUILayout.EndHorizontal();
                
                if (GUILayout.Button("Show Subscribers")) {
                    selectedEvent = eventData.eventName;
                    showSubscriberList = true;
                }
                
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        // Subscriber list
        if (showSubscriberList && !string.IsNullOrEmpty(selectedEvent)) {
            GUILayout.Space(10);
            EditorGUILayout.LabelField($"Subscribers for: {selectedEvent}", EditorStyles.boldLabel);
            
            var subscribers = EventSystem.EventBus.Instance.GetSubscribersForEvent(selectedEvent);
            foreach (var subscriber in subscribers) {
                EditorGUILayout.BeginHorizontal("box");
                EditorGUILayout.LabelField(subscriber.subscriberName);
                EditorGUILayout.LabelField(subscriber.domain.ToString(), GUILayout.Width(80));
                EditorGUILayout.LabelField(subscriber.isActive ? "Active" : "Inactive", GUILayout.Width(60));
                EditorGUILayout.EndHorizontal();
            }
            
            if (GUILayout.Button("Close Subscribers")) {
                showSubscriberList = false;
                selectedEvent = "";
            }
        }
        
        // Test event triggering
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Test Event Triggering", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        inspector.TestEventName = EditorGUILayout.TextField("Event Name", inspector.TestEventName);
        if (GUILayout.Button("Trigger", GUILayout.Width(60))) {
            if (!string.IsNullOrEmpty(inspector.TestEventName)) {
                EventBusHelper.Trigger<string>(inspector.TestEventName, "test");
                inspector.AddRecentEvent(inspector.TestEventName);
                EditorUtility.SetDirty(inspector);
            }
        }
        EditorGUILayout.EndHorizontal();
        
        // Recent events
        var recentEvents = inspector.GetRecentEvents();
        if (recentEvents.Count > 0) {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Recent Events:", EditorStyles.boldLabel);
            foreach (var recentEvent in recentEvents) {
                EditorGUILayout.LabelField($"• {recentEvent}");
            }
        }
        
        var test = ((MonoBehaviour)inspector).GetComponent<EventSystem.Tests.EventBusTest>();
        if (test != null) {
            GUILayout.Space(10);
            GUILayout.Label("EventBus System Test Results", EditorStyles.boldLabel);
            var namesGeneric = test.GetTestEventNamesGeneric();
            var namesNonGeneric = test.GetTestEventNamesNonGeneric();
            var triggeredGeneric = test.GetTestTriggeredGeneric();
            var triggeredNonGeneric = test.GetTestTriggeredNonGeneric();
            var resultsGeneric = test.GetTestResultsGeneric();
            var resultsNonGeneric = test.GetTestResultsNonGeneric();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Event Name (Generic)", EditorStyles.boldLabel, GUILayout.Width(220));
            EditorGUILayout.LabelField("Generic<string>", EditorStyles.boldLabel, GUILayout.Width(100));
            EditorGUILayout.LabelField("Event Name (Non-Generic)", EditorStyles.boldLabel, GUILayout.Width(220));
            EditorGUILayout.LabelField("Non-Generic", EditorStyles.boldLabel, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
            for (int i = 0; i < namesGeneric.Length; i++) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(namesGeneric[i], GUILayout.Width(220));
                Color c1 = !triggeredGeneric[i] ? Color.yellow : (resultsGeneric[i] ? Color.green : Color.red);
                Rect r1 = GUILayoutUtility.GetRect(1, 20, GUILayout.Width(100));
                EditorGUI.DrawRect(r1, c1);
                GUIStyle style1 = new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Bold };
                style1.normal.textColor = Color.black;
                string status1 = !triggeredGeneric[i] ? "Not Triggered" : (resultsGeneric[i] ? "PASS" : "FAIL");
                EditorGUI.LabelField(new Rect(r1.x + 8, r1.y, r1.width - 8, r1.height), status1, style1);
                EditorGUILayout.LabelField(namesNonGeneric[i], GUILayout.Width(220));
                Color c2 = !triggeredNonGeneric[i] ? Color.yellow : (resultsNonGeneric[i] ? Color.green : Color.red);
                Rect r2 = GUILayoutUtility.GetRect(1, 20, GUILayout.Width(100));
                EditorGUI.DrawRect(r2, c2);
                GUIStyle style2 = new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Bold };
                style2.normal.textColor = Color.black;
                string status2 = !triggeredNonGeneric[i] ? "Not Triggered" : (resultsNonGeneric[i] ? "PASS" : "FAIL");
                EditorGUI.LabelField(new Rect(r2.x + 8, r2.y, r2.width - 8, r2.height), status2, style2);
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.Space(10);
        }
    }
}
}

[System.Serializable]
public class EventBusInspector : MonoBehaviour {
    [Header("Event Bus Inspector")]
    [SerializeField] private bool enableRuntimeMonitoring = true;
    [SerializeField] private bool logEventTriggers = false;
    [SerializeField] private string testEventName = "test.event";
    
    private List<string> recentEvents = new List<string>();
    private int maxRecentEvents = 10;
    
    // Public property for accessing testEventName
    public string TestEventName {
        get => testEventName;
        set => testEventName = value;
    }
    
    private void Start() {
        if (enableRuntimeMonitoring) {
            Debug.Log("[EventBusInspector] Event Bus monitoring enabled");
        }
    }
    
    private void Update() {
        if (enableRuntimeMonitoring && EventSystem.EventBus.IsAvailable) {
            // This could be expanded to show real-time event activity
            // For now, it just ensures the inspector stays active
        }
    }
    
    public void AddRecentEvent(string eventName) {
        recentEvents.Insert(0, $"{eventName} - {System.DateTime.Now:HH:mm:ss}");
        if (recentEvents.Count > maxRecentEvents) {
            recentEvents.RemoveAt(recentEvents.Count - 1);
        }
        
        // Use the logEventTriggers field to log events when enabled
        if (logEventTriggers) {
            Debug.Log($"[EventBusInspector] Event triggered: {eventName}");
        }
    }
    
    public List<string> GetRecentEvents() {
        return new List<string>(recentEvents);
    }
    
    // Method to clear recent events
    [ContextMenu("Clear Recent Events")]
    public void ClearRecentEvents() {
        recentEvents.Clear();
    }
    
    // Method to test event triggering from context menu
    [ContextMenu("Trigger Test Event")]
    public void TriggerTestEvent() {
        if (EventSystem.EventBus.IsAvailable && !string.IsNullOrEmpty(testEventName)) {
            EventBusHelper.Trigger<string>(testEventName, "test");
            AddRecentEvent(testEventName);
        }
    }
}
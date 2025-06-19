using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using EventSystem;

namespace EventSystem.Editor {
public class EventBusEventViewer : EditorWindow {
    private List<EventData> events;
    private List<SubscriberData> selectedEventSubscribers;
    private string selectedEventName = "";
    private Vector2 scrollPosition;
    private Vector2 subscriberScrollPosition;
    private bool showSubscribers = false;
    private bool autoRefresh = true;
    private double lastAutoRefreshTime = 0;
    private float refreshInterval = 1f; // Refresh every second

    // Collapsed state dictionaries
    private Dictionary<string, bool> domainFoldouts = new Dictionary<string, bool>();
    private Dictionary<string, Dictionary<string, bool>> categoryFoldouts = new Dictionary<string, Dictionary<string, bool>>();

    // Pastel color mapping for domains
    private static readonly Dictionary<EventDomain, Color> domainColors = new Dictionary<EventDomain, Color> {
        { EventDomain.Gameplay, new Color(0.64f, 0.79f, 0.98f) },      // #A3C9F9
        { EventDomain.Input, new Color(0.71f, 0.92f, 0.84f) },         // #B5EAD7
        { EventDomain.Presentation, new Color(1.0f, 0.85f, 0.76f) },  // #FFDAC1
        { EventDomain.Core, new Color(1.0f, 0.72f, 0.7f) },           // #FFB7B2
        { EventDomain.Infrastructure, new Color(0.89f, 0.94f, 0.80f) },// #E2F0CB
        { EventDomain.Editor, new Color(0.78f, 0.81f, 0.92f) },       // #C7CEEA
        { EventDomain.Custom, new Color(1.0f, 0.98f, 0.80f) }         // #FFFACD
    };

    // Helper to get best contrast text color (black or white)
    private Color GetContrastColor(Color bg)
    {
        // Perceived luminance formula
        float luminance = 0.299f * bg.r + 0.587f * bg.g + 0.114f * bg.b;
        return luminance > 0.6f ? Color.black : Color.white;
    }

    [MenuItem("Window/Event Bus/Event Viewer")]
    public static void ShowWindow() {
        GetWindow<EventBusEventViewer>("Event Bus Event Viewer");
    }

    private void OnEnable() {
        LoadEvents();
    }

    private void OnInspectorUpdate() {
        if (autoRefresh) {
            Repaint();
        }
    }

    private void OnGUI() {
        GUILayout.Label("Event Bus Events", EditorStyles.boldLabel);
        
        // Auto-refresh toggle
        autoRefresh = EditorGUILayout.Toggle("Auto Refresh", autoRefresh);
        
        // Manual refresh button
        if (GUILayout.Button("Refresh Events")) {
            LoadEvents();
        }
        
        // Auto-refresh logic (works in Edit and Play mode)
        if (autoRefresh) {
            double now = EditorApplication.timeSinceStartup;
            if (now - lastAutoRefreshTime > refreshInterval) {
                LoadEvents();
                lastAutoRefreshTime = now;
                Repaint();
            }
        }

        if (events == null || events.Count == 0) {
            GUILayout.Label("No events found. Make sure EventBus is running in the scene.");
            return;
        }

        // Group events by domain and category
        var grouped = events
            .GroupBy(e => e.domain)
            .ToDictionary(
                g => g.Key,
                g => g.GroupBy(e => GetCategory(e.eventName)).ToDictionary(
                    cg => cg.Key,
                    cg => cg.ToList()
                )
            );

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        foreach (var domainGroup in grouped) {
            var domain = domainGroup.Key;
            var domainName = domain.ToString();
            var domainColor = domainColors.ContainsKey(domain) ? domainColors[domain] : Color.white;
            var textColor = GetContrastColor(domainColor);
            if (!domainFoldouts.ContainsKey(domainName)) domainFoldouts[domainName] = true;

            // Draw colored domain header using EditorGUI.DrawRect and contrast text
            Rect headerRect = GUILayoutUtility.GetRect(1, 24, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(headerRect, domainColor);
            var prevColor = GUI.color;
            GUI.color = textColor;
            domainFoldouts[domainName] = EditorGUI.Foldout(
                new Rect(headerRect.x + 10, headerRect.y + 4, headerRect.width - 10, headerRect.height - 4),
                domainFoldouts[domainName],
                $"{domainName}",
                true,
                new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold, normal = { textColor = textColor }, onNormal = { textColor = textColor } }
            );
            GUI.color = prevColor;
            GUILayout.Space(2);

            if (domainFoldouts[domainName]) {
                if (!categoryFoldouts.ContainsKey(domainName)) categoryFoldouts[domainName] = new Dictionary<string, bool>();
                foreach (var categoryGroup in domainGroup.Value) {
                    var category = categoryGroup.Key;
                    if (!categoryFoldouts[domainName].ContainsKey(category)) categoryFoldouts[domainName][category] = true;
                    // Draw colored category header
                    Rect catRect = GUILayoutUtility.GetRect(1, 20, GUILayout.ExpandWidth(true));
                    EditorGUI.DrawRect(catRect, domainColor);
                    GUI.color = textColor;
                    categoryFoldouts[domainName][category] = EditorGUI.Foldout(
                        new Rect(catRect.x + 20, catRect.y + 2, catRect.width - 20, catRect.height - 2),
                        categoryFoldouts[domainName][category],
                        $"  {category}",
                        true,
                        new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Normal, normal = { textColor = textColor }, onNormal = { textColor = textColor } }
                    );
                    GUI.color = prevColor;
                    if (categoryFoldouts[domainName][category]) {
                        foreach (var eventData in categoryGroup.Value) {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label($"Event: {eventData.eventName}", GUILayout.Width(300));
                            GUILayout.Label($"Subscribers: {eventData.subscriberCount}", GUILayout.Width(100));
                            GUILayout.Label($"Last Triggered: {eventData.lastTriggeredTime}", GUILayout.Width(150));
                            GUILayout.Label($"Type: {(string.IsNullOrEmpty(eventData.eventDescription) ? "Non-Generic" : eventData.eventDescription)}", GUILayout.Width(120));
                            if (GUILayout.Button($"Show Subscribers ({eventData.subscriberCount})", GUILayout.Width(160))) {
                                selectedEventName = eventData.eventName;
                                LoadSubscribers(eventData.eventName);
                                showSubscribers = true;
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                }
            }
        }

        EditorGUILayout.EndScrollView();

        // Subscriber details panel
        if (showSubscribers && !string.IsNullOrEmpty(selectedEventName)) {
            GUILayout.Space(10);
            GUILayout.Label($"Subscribers for: {selectedEventName}", EditorStyles.boldLabel);
            
            if (selectedEventSubscribers != null && selectedEventSubscribers.Count > 0) {
                subscriberScrollPosition = EditorGUILayout.BeginScrollView(subscriberScrollPosition, GUILayout.Height(200));
                
                foreach (var subscriber in selectedEventSubscribers) {
                    GUILayout.BeginHorizontal("box");
                    GUILayout.Label($"Name: {subscriber.subscriberName}", GUILayout.Width(300));
                    GUILayout.Label($"Domain: {subscriber.domain}", GUILayout.Width(100));
                    GUILayout.Label($"Active: {subscriber.isActive}", GUILayout.Width(100));
                    GUILayout.EndHorizontal();
                }
                
                EditorGUILayout.EndScrollView();
            } else {
                GUILayout.Label("No subscribers found for this event.");
            }
            
            if (GUILayout.Button("Close Subscribers")) {
                showSubscribers = false;
                selectedEventName = "";
            }
        }
    }

    private void LoadEvents() {
        if (EventBus.IsPresentInScene) {
            events = EventBusHelper.GetAllEvents();
        } else {
            events = new List<EventData>();
        }
    }

    private void LoadSubscribers(string eventName) {
        if (EventBus.IsPresentInScene) {
            selectedEventSubscribers = EventBus.Instance.GetSubscribersForEvent(eventName);
        } else {
            selectedEventSubscribers = new List<SubscriberData>();
        }
    }

    private string GetCategory(string eventName) {
        var parts = eventName.Split('.');
        return parts.Length > 1 ? parts[1] : "(none)";
    }
}
}
using UnityEditor;
using UnityEngine;
using EventSystem;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace EventSystem.Editor {
public class EventBusEventCreator : EditorWindow {
    private string eventName = "";
    private EventDomain eventDomain = EventDomain.Core;
    private string eventDescription = "";
    private bool createInRuntime = true;
    private bool createPersistent = true;
    private EventBusEventsAsset eventsAsset;
    private Vector2 scrollPosition;
    private string payloadTypeName = "";
    private bool isGenericEvent = false;

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

    [MenuItem("Window/Event Bus/Event Creator")]
    public static void ShowWindow() {
        GetWindow<EventBusEventCreator>("Event Creator");
    }

    private void OnEnable() {
        LoadOrCreateEventsAsset();
    }

    // Helper to get best contrast text color (black or white)
    private Color GetContrastColor(Color bg)
    {
        float luminance = 0.299f * bg.r + 0.587f * bg.g + 0.114f * bg.b;
        return luminance > 0.6f ? Color.black : Color.white;
    }

    private void OnGUI() {
        GUILayout.Label("Create New Event", EditorStyles.boldLabel);
        
        // Event creation options
        EditorGUILayout.Space(5);
        createPersistent = EditorGUILayout.Toggle("Create Persistent Event", createPersistent);
        createInRuntime = EditorGUILayout.Toggle("Create in Runtime", createInRuntime);
        
        EditorGUILayout.Space(10);
        
        // Event details
        eventName = EditorGUILayout.TextField("Event Name", eventName);
        eventDomain = (EventDomain)EditorGUILayout.EnumPopup("Event Domain", eventDomain);
        eventDescription = EditorGUILayout.TextField("Event Description (optional)", eventDescription);
        isGenericEvent = EditorGUILayout.Toggle("Is Generic (Payload)", isGenericEvent);
        if (isGenericEvent) {
            payloadTypeName = EditorGUILayout.TextField("Payload Type (e.g. string, int, MyType)", payloadTypeName);
        } else {
            payloadTypeName = "";
        }

        EditorGUILayout.Space(10);
        
        // Show event naming convention help
        EditorGUILayout.HelpBox(
            "How to use the Event Creator:\n" +
            "1. Enter a new event name. The name must start with the domain as a prefix (e.g., 'gameplay.player.ondeath').\n" +
            "2. Select the domain from the dropdown. This should match the prefix in your event name.\n" +
            "3. (Optional) Add a description for your event.\n" +
            "\nNaming Convention Example:\n" +
            "  gameplay.player.ondeath\n" +
            "  input.keyboard.onpress\n" +
            "  presentation.ui.menuopened\n" +
            "\nThe domain prefix in the name (e.g., 'gameplay') must match the selected domain.",
            MessageType.Info);

        EditorGUILayout.Space(10);

        // Create button
        if (GUILayout.Button("Create Event")) {
            CreateEvent();
        }

        EditorGUILayout.Space(10);
        
        // Show persistent events grouped by domain and category
        if (eventsAsset != null) {
            EditorGUILayout.LabelField("Persistent Events:", EditorStyles.boldLabel);
            
            // Group events by domain and category
            var grouped = eventsAsset.PersistentEvents
                .GroupBy(e => e.domain)
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(e => GetCategory(e.eventName)).ToDictionary(
                        cg => cg.Key,
                        cg => cg.ToList()
                    )
                );

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));

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
                                EditorGUILayout.BeginHorizontal("box");
                                EditorGUILayout.LabelField(eventData.eventName, EditorStyles.boldLabel);
                                EditorGUILayout.LabelField($"Domain: {eventData.domain}", EditorStyles.miniLabel);
                                EditorGUILayout.LabelField($"Type: {(string.IsNullOrEmpty(eventData.eventDescription) ? "Non-Generic" : eventData.eventDescription)}", EditorStyles.miniLabel);
                                if (GUILayout.Button("Delete", GUILayout.Width(60), GUILayout.Height(30))) {
                                    if (EditorUtility.DisplayDialog("Delete Event", 
                                        $"Are you sure you want to delete the event '{eventData.eventName}'?", 
                                        "Delete", "Cancel")) {
                                        DeleteEvent(eventData.eventName);
                                    }
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                    }
                }
            }

            EditorGUILayout.EndScrollView();
            
            if (eventsAsset.PersistentEvents.Count > 0) {
                if (GUILayout.Button("Clear All Persistent Events")) {
                    if (EditorUtility.DisplayDialog("Clear All Events", 
                        "Are you sure you want to delete all persistent events?", 
                        "Clear All", "Cancel")) {
                        ClearAllPersistentEvents();
                    }
                }
            }
        }

        // Show current runtime events if EventBus is available
        if (EventBus.IsPresentInScene) {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Current Runtime Events in Domain:", EditorStyles.boldLabel);
            
            var eventsInDomain = EventBusHelper.GetEventsInDomain(eventDomain);
            if (eventsInDomain.Count > 0) {
                foreach (var eventName in eventsInDomain) {
                    EditorGUILayout.LabelField($"• {eventName}");
                }
            } else {
                EditorGUILayout.LabelField("No runtime events in this domain yet.");
            }
        } else {
            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("EventBus is not available in the current scene.", MessageType.Warning);
        }
    }

    private void LoadOrCreateEventsAsset() {
        // Try to load existing asset
        eventsAsset = Resources.Load<EventBusEventsAsset>("EventBusEvents");
        
        if (eventsAsset == null) {
            // Create the Resources folder if it doesn't exist
            if (!Directory.Exists("Assets/Resources")) {
                Directory.CreateDirectory("Assets/Resources");
                AssetDatabase.Refresh();
            }
            
            // Create new asset
            eventsAsset = ScriptableObject.CreateInstance<EventBusEventsAsset>();
            AssetDatabase.CreateAsset(eventsAsset, "Assets/Resources/EventBusEvents.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("[EventBusEventCreator] Created new EventBusEvents asset at Assets/Resources/EventBusEvents.asset");
        }
    }

    private void CreateEvent() {
        if (string.IsNullOrEmpty(eventName)) {
            EditorUtility.DisplayDialog("Error", "Event name cannot be empty.", "OK");
            return;
        }

        // Validate event name format
        if (!IsValidEventName(eventName)) {
            EditorUtility.DisplayDialog("Error", 
                "Event name should follow the format: domain.category.action\n" +
                "Example: gameplay.player.died", "OK");
            return;
        }

        // Check for type conflict
        if (eventsAsset != null && eventsAsset.HasEvent(eventName)) {
            var existing = eventsAsset.PersistentEvents.Find(e => e.eventName == eventName);
            bool existingIsGeneric = !string.IsNullOrEmpty(existing.eventDescription) && existing.eventDescription.Contains("Payload:");
            if (existingIsGeneric != isGenericEvent) {
                EditorUtility.DisplayDialog("Type Conflict", $"Event '{eventName}' already exists with a different type (generic/non-generic).", "OK");
                return;
            }
        }

        bool success = false;

        // Create persistent event
        if (createPersistent && eventsAsset != null) {
            var newEventData = new EventData {
                eventName = eventName,
                domain = eventDomain,
                subscriberCount = 0,
                isActive = true,
                lastTriggeredTime = System.DateTime.Now.ToString("HH:mm:ss"),
                eventDescription = isGenericEvent && !string.IsNullOrEmpty(payloadTypeName) ? $"Payload: {payloadTypeName}" : eventDescription
            };
            
            eventsAsset.AddEvent(newEventData);
            EditorUtility.SetDirty(eventsAsset);
            AssetDatabase.SaveAssets();
            success = true;
            
            Debug.Log($"[EventBusEventCreator] Created persistent event: {eventName} in domain: {eventDomain}");
        }

        // Create in runtime
        if (createInRuntime && EventBus.IsAvailable) {
            EventBusHelper.CreateEvent(eventName, eventDomain);
            success = true;
            
            Debug.Log($"[EventBusEventCreator] Created runtime event: {eventName} in domain: {eventDomain}");
        } else if (createInRuntime && !EventBus.IsAvailable) {
            EditorUtility.DisplayDialog("Warning", 
                "EventBus is not available in the current scene.\n" +
                "The event will be created when first triggered.", "OK");
        }

        if (success) {
            EditorUtility.DisplayDialog("Success", 
                $"Event '{eventName}' created successfully!", "OK");
            ClearFields();
        }
    }

    private void DeleteEvent(string eventName) {
        if (eventsAsset != null) {
            eventsAsset.RemoveEvent(eventName);
            EditorUtility.SetDirty(eventsAsset);
            AssetDatabase.SaveAssets();
            
            Debug.Log($"[EventBusEventCreator] Deleted persistent event: {eventName}");
        }
    }

    private void ClearAllPersistentEvents() {
        if (eventsAsset != null) {
            eventsAsset.ClearAllEvents();
            EditorUtility.SetDirty(eventsAsset);
            AssetDatabase.SaveAssets();
            
            Debug.Log("[EventBusEventCreator] Cleared all persistent events");
        }
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
        eventName = "";
        eventDescription = "";
        payloadTypeName = "";
        isGenericEvent = false;
    }

    private string GetCategory(string eventName) {
        var parts = eventName.Split('.');
        return parts.Length > 1 ? parts[1] : "(none)";
    }
}
}
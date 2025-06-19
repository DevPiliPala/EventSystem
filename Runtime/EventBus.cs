// filepath: /UnityEventBusEnhancement/UnityEventBusEnhancement/Runtime/EventBus.cs
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace EventSystem {

    [DefaultExecutionOrder(-100)]
    public class EventBus : MonoBehaviour {
        [Header("Event Bus Configuration")]
        [SerializeField] private bool showDebugInfo = true;
        [SerializeField] private bool logEventTriggers = false;

        private static EventBus _instance;
        private static bool _isDestroying = false;

        public static bool IsAvailable => _instance != null && !_isDestroying;

        public static EventBus Instance {
            get {
                if (_isDestroying) {
                    return null;
                }

                if (_instance == null) {
                    var eventBuses = FindObjectsByType<EventBus>(FindObjectsSortMode.None);
                    if (eventBuses.Length > 0) {
                        _instance = eventBuses[0];
                    } else {
                        GameObject go = new GameObject("EventBus");
                        _instance = go.AddComponent<EventBus>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        private Dictionary<string, UnityEvent> _events = new Dictionary<string, UnityEvent>();
        private Dictionary<string, EventDomain> _eventDomains = new Dictionary<string, EventDomain>();
        private Dictionary<string, List<SubscriberInfo>> _subscribers = new Dictionary<string, List<SubscriberInfo>>();
        private Dictionary<string, DateTime> _lastTriggerTimes = new Dictionary<string, DateTime>();

        // --- Type-safe generic event registry ---
        private Dictionary<string, Delegate> _genericEvents = new Dictionary<string, Delegate>();
        // Track event type: null = non-generic, otherwise typeof(T)
        private Dictionary<string, Type> _eventTypes = new Dictionary<string, Type>();

        // Property to expose active events for the editor
        public List<EventData> activeEvents {
            get {
                var events = new List<EventData>();
                foreach (var kvp in _events) {
                    var eventName = kvp.Key;
                    var domain = _eventDomains.ContainsKey(eventName) ? _eventDomains[eventName] : EventDomain.Core;
                    var subscriberCount = _subscribers.ContainsKey(eventName) ? _subscribers[eventName].Count(s => s.isActive) : 0;
                    var lastTriggered = _lastTriggerTimes.ContainsKey(eventName) ? _lastTriggerTimes[eventName].ToString("HH:mm:ss") : "Never";
                    
                    events.Add(new EventData {
                        eventName = eventName,
                        domain = domain,
                        subscriberCount = subscriberCount,
                        isActive = true,
                        lastTriggeredTime = lastTriggered
                    });
                }
                return events;
            }
        }

        private class SubscriberInfo {
            public string subscriberName;
            public EventDomain domain;
            public UnityAction action;
            public UnityAction<object> parameterizedAction;
            public bool isActive;
        }

        private void Awake() {
            if (_instance == null) {
                _instance = this;
                _isDestroying = false;
                DontDestroyOnLoad(gameObject);
                InitializeEventBus();
            } else if (_instance != this) {
                Destroy(gameObject);
            }
        }

        private void InitializeEventBus() {
            if (showDebugInfo) {
                Debug.Log("[EventBus] EventBus initialized successfully");
            }
            
            // Load persistent events from ScriptableObject
            LoadPersistentEvents();
        }
        
        private void LoadPersistentEvents() {
            var eventsAsset = Resources.Load<EventBusEventsAsset>("EventBusEvents");
            if (eventsAsset != null) {
                eventsAsset.RegisterAllEventsWithEventBus();
                if (showDebugInfo) {
                    Debug.Log($"[EventBus] Loaded {eventsAsset.PersistentEvents.Count} persistent events");
                }
            } else {
                if (showDebugInfo) {
                    Debug.Log("[EventBus] No EventBusEvents asset found in Resources folder");
                }
            }
        }

        public void Subscribe(string eventName, UnityAction action, EventDomain domain, string subscriberName = "") {
            if (string.IsNullOrEmpty(eventName) || action == null) {
                if (showDebugInfo) {
                    Debug.LogWarning("[EventBus] Attempted to subscribe with null or empty event name or action");
                }
                return;
            }
            // Type-safety enforcement
            if (_eventTypes.TryGetValue(eventName, out var existingType)) {
                if (existingType != null) {
                    Debug.LogError($"[EventBus] Cannot subscribe non-generic handler to event '{eventName}' which is already registered as a generic event with payload type {existingType.Name}.");
                    return;
                }
            } else {
                _eventTypes[eventName] = null;
            }

            if (!_events.ContainsKey(eventName)) {
                _events[eventName] = new UnityEvent();
                _eventDomains[eventName] = domain;
                _subscribers[eventName] = new List<SubscriberInfo>();
                if (showDebugInfo) {
                    Debug.Log($"[EventBus] Created new event: {eventName} in domain: {domain}");
                }
            }

            var subscriberInfo = new SubscriberInfo {
                subscriberName = string.IsNullOrEmpty(subscriberName) ? action.Target?.ToString() ?? "Unknown" : subscriberName,
                domain = domain,
                action = action,
                isActive = true
            };

            _subscribers[eventName].Add(subscriberInfo);
            _events[eventName].AddListener(action);
            
            if (showDebugInfo) {
                Debug.Log($"[EventBus] Subscribed {subscriberInfo.subscriberName} to {eventName}");
            }
        }

        public void Unsubscribe(string eventName, UnityAction action) {
            if (_isDestroying || !_events.ContainsKey(eventName)) {
                return;
            }
            // Type-safety enforcement
            if (_eventTypes.TryGetValue(eventName, out var existingType)) {
                if (existingType != null) {
                    Debug.LogError($"[EventBus] Cannot unsubscribe non-generic handler from event '{eventName}' which is registered as a generic event with payload type {existingType.Name}.");
                    return;
                }
            }

            _events[eventName].RemoveListener(action);
            var subscribers = _subscribers[eventName];
            var subscriberToRemove = subscribers.FirstOrDefault(s => s.action == action);
            if (subscriberToRemove != null) {
                subscribers.Remove(subscriberToRemove);
                if (showDebugInfo) {
                    Debug.Log($"[EventBus] Unsubscribed {subscriberToRemove.subscriberName} from {eventName}");
                }
            }
        }

        public void Trigger(string eventName) {
            // Type-safety enforcement
            if (_eventTypes.TryGetValue(eventName, out var existingType)) {
                if (existingType != null) {
                    Debug.LogError($"[EventBus] Cannot trigger non-generic event '{eventName}' which is registered as a generic event with payload type {existingType.Name}.");
                    return;
                }
            }

            if (!_events.ContainsKey(eventName)) {
                var domain = GetDomainFromEventName(eventName);
                _events[eventName] = new UnityEvent();
                _eventDomains[eventName] = domain;
                _subscribers[eventName] = new List<SubscriberInfo>();
                if (showDebugInfo) {
                    Debug.Log($"[EventBus] Auto-created event: {eventName} in domain: {domain}");
                }
            }

            _lastTriggerTimes[eventName] = DateTime.Now;
            
            if (logEventTriggers) {
                Debug.Log($"[EventBus] Triggering event: {eventName} at {DateTime.Now:HH:mm:ss}");
            }

            _events[eventName].Invoke();
        }

        /// <summary>
        /// Subscribe to a type-safe event with a payload.
        /// </summary>
        public void Subscribe<TPayload>(string eventName, Action<TPayload> handler)
        {
            if (string.IsNullOrEmpty(eventName) || handler == null)
                return;
            // Type-safety enforcement
            if (_eventTypes.TryGetValue(eventName, out var existingType))
            {
                if (existingType == null)
                {
                    Debug.LogError($"[EventBus] Cannot subscribe generic handler to event '{eventName}' which is already registered as a non-generic event.");
                    return;
                }
                if (existingType != typeof(TPayload))
                {
                    Debug.LogError($"[EventBus] Cannot subscribe generic handler to event '{eventName}' with payload type {typeof(TPayload).Name}, which is already registered with payload type {existingType.Name}.");
                    return;
                }
            }
            else
            {
                _eventTypes[eventName] = typeof(TPayload);
            }
            if (_genericEvents.TryGetValue(eventName, out var existing))
            {
                _genericEvents[eventName] = Delegate.Combine(existing, handler);
            }
            else
            {
                _genericEvents[eventName] = handler;
            }
            if (showDebugInfo)
            {
                Debug.Log($"[EventBus] (Generic) Subscribed to {eventName} with payload type {typeof(TPayload).Name}");
            }
        }

        /// <summary>
        /// Unsubscribe from a type-safe event with a payload.
        /// </summary>
        public void Unsubscribe<TPayload>(string eventName, Action<TPayload> handler)
        {
            if (string.IsNullOrEmpty(eventName) || handler == null)
                return;
            // Type-safety enforcement
            if (_eventTypes.TryGetValue(eventName, out var existingType))
            {
                if (existingType == null)
                {
                    Debug.LogError($"[EventBus] Cannot unsubscribe generic handler from event '{eventName}' which is registered as a non-generic event.");
                    return;
                }
                if (existingType != typeof(TPayload))
                {
                    Debug.LogError($"[EventBus] Cannot unsubscribe generic handler from event '{eventName}' with payload type {typeof(TPayload).Name}, which is already registered with payload type {existingType.Name}.");
                    return;
                }
            }
            if (_genericEvents.TryGetValue(eventName, out var existing))
            {
                var newDelegate = Delegate.Remove(existing, handler);
                if (newDelegate == null)
                    _genericEvents.Remove(eventName);
                else
                    _genericEvents[eventName] = newDelegate;
            }
        }

        /// <summary>
        /// Trigger a type-safe event with a payload. (Now the main Trigger<TPayload> method)
        /// </summary>
        public void Trigger<TPayload>(string eventName, TPayload payload)
        {
            // Type-safety enforcement
            if (_eventTypes.TryGetValue(eventName, out var existingType))
            {
                if (existingType == null)
                {
                    Debug.LogError($"[EventBus] Cannot trigger generic event '{eventName}' with payload type {typeof(TPayload).Name}, which is registered as a non-generic event.");
                    return;
                }
                if (existingType != typeof(TPayload))
                {
                    Debug.LogError($"[EventBus] Cannot trigger generic event '{eventName}' with payload type {typeof(TPayload).Name}, which is already registered with payload type {existingType.Name}.");
                    return;
                }
            }
            else
            {
                _eventTypes[eventName] = typeof(TPayload);
            }
            if (_genericEvents.TryGetValue(eventName, out var del))
            {
                if (del is Action<TPayload> action)
                {
                    if (logEventTriggers)
                        Debug.Log($"[EventBus] (Generic) Triggering {eventName} with payload: {payload}");
                    action.Invoke(payload);
                }
                else if (del != null)
                {
                    Debug.LogWarning($"[EventBus] (Generic) Event {eventName} has mismatched payload type: {typeof(TPayload).Name}");
                }
            }
            else if (showDebugInfo)
            {
                Debug.Log($"[EventBus] (Generic) No subscribers for {eventName} with payload type {typeof(TPayload).Name}");
            }
        }

        private EventDomain GetDomainFromEventName(string eventName) {
            if (string.IsNullOrEmpty(eventName))
                return EventDomain.Core;

            var parts = eventName.Split('.');
            if (parts.Length < 2)
                return EventDomain.Core;

            var domainString = parts[0];

            switch (domainString.ToLower()) {
                case "core": return EventDomain.Core;
                case "editor": return EventDomain.Editor;
                case "gameplay": return EventDomain.Gameplay;
                case "infrastructure": return EventDomain.Infrastructure;
                case "input": return EventDomain.Input;
                case "presentation": return EventDomain.Presentation;
                default: return EventDomain.Core;
            }
        }

        public bool HasEvent(string eventName) {
            return _events.ContainsKey(eventName);
        }

        public EventDomain GetEventDomain(string eventName) {
            return _eventDomains.ContainsKey(eventName) ? _eventDomains[eventName] : EventDomain.Core;
        }

        public List<string> GetEventsInDomain(EventDomain domain) {
            return _eventDomains.Where(kvp => kvp.Value == domain).Select(kvp => kvp.Key).ToList();
        }

        public void ClearAll() {
            _events.Clear();
            _eventDomains.Clear();
            _subscribers.Clear();
            _lastTriggerTimes.Clear();
            
            if (showDebugInfo) {
                Debug.Log("[EventBus] All events cleared");
            }
        }

        // Method to get subscriber information for debugging
        public List<SubscriberData> GetSubscribersForEvent(string eventName) {
            if (!_subscribers.ContainsKey(eventName)) {
                return new List<SubscriberData>();
            }

            return _subscribers[eventName].Select(s => new SubscriberData {
                subscriberName = s.subscriberName,
                domain = s.domain,
                subscribedToEvent = eventName,
                isActive = s.isActive
            }).ToList();
        }

        private void OnDestroy() {
            if (_instance == this) {
                _isDestroying = true;
                ClearAll();
                _instance = null;
            }
        }

        private void OnApplicationQuit() {
            _isDestroying = true;
        }

        public static bool IsPresentInScene {
            get {
#if UNITY_EDITOR
                // In edit mode, search all loaded scenes
                var all = UnityEngine.Object.FindObjectsByType<EventBus>(FindObjectsSortMode.None);
                return all != null && all.Length > 0;
#else
                return IsAvailable;
#endif
            }
        }

        public static EventBus FindInScene() {
#if UNITY_EDITOR
            var all = UnityEngine.Object.FindObjectsByType<EventBus>(FindObjectsSortMode.None);
            return all != null && all.Length > 0 ? all[0] : null;
#else
            return Instance;
#endif
        }
    }
}
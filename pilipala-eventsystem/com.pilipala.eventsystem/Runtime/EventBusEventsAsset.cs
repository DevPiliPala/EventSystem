using System;
using System.Collections.Generic;
using UnityEngine;

namespace EventSystem {
    [CreateAssetMenu(fileName = "EventBusEvents", menuName = "Event Bus/Event Bus Events Asset")]
    public class EventBusEventsAsset : ScriptableObject {
        [SerializeField] private List<EventData> persistentEvents = new List<EventData>();
        
        public List<EventData> PersistentEvents => persistentEvents;
        
        public void AddEvent(EventData eventData) {
            // Check if event already exists
            var existingEvent = persistentEvents.Find(e => e.eventName == eventData.eventName);
            if (existingEvent != null) {
                // Check for type conflict
                if (existingEvent.GetType() != eventData.GetType()) {
                    Debug.LogError($"[EventBusEventsAsset] Type conflict: Event '{eventData.eventName}' already exists with a different type.");
                    return;
                }
                Debug.LogWarning($"[EventBusEventsAsset] Event '{eventData.eventName}' already exists in persistent events.");
                return;
            }
            
            persistentEvents.Add(eventData);
            Debug.Log($"[EventBusEventsAsset] Added persistent event: {eventData.eventName}");
        }
        
        public void RemoveEvent(string eventName) {
            var eventToRemove = persistentEvents.Find(e => e.eventName == eventName);
            if (eventToRemove != null) {
                persistentEvents.Remove(eventToRemove);
                Debug.Log($"[EventBusEventsAsset] Removed persistent event: {eventName}");
            } else {
                Debug.LogWarning($"[EventBusEventsAsset] Event '{eventName}' not found in persistent events.");
            }
        }
        
        public bool HasEvent(string eventName) {
            return persistentEvents.Exists(e => e.eventName == eventName);
        }
        
        public void ClearAllEvents() {
            persistentEvents.Clear();
            Debug.Log("[EventBusEventsAsset] Cleared all persistent events.");
        }
        
        public List<EventData> GetEventsInDomain(EventDomain domain) {
            return persistentEvents.FindAll(e => e.domain == domain);
        }
        
        // Method to register all persistent events with the EventBus at runtime
        public void RegisterAllEventsWithEventBus() {
            if (!EventBus.IsAvailable) {
                Debug.LogWarning("[EventBusEventsAsset] EventBus is not available. Cannot register persistent events.");
                return;
            }
            
            foreach (var eventData in persistentEvents) {
                EventBusHelper.CreateEvent(eventData.eventName, eventData.domain);
                Debug.Log($"[EventBusEventsAsset] Registered persistent event: {eventData.eventName}");
            }
        }
    }
} 
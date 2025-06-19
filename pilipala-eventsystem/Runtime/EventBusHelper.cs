// filepath: /UnityEventBusEnhancement/UnityEventBusEnhancement/Runtime/EventBusHelper.cs
using System.Collections.Generic;

namespace EventSystem {
    public static class EventBusHelper {
        // --- Type-safe generic event helpers ---
        public static void Subscribe<T>(string eventName, System.Action<T> handler) {
            if (EventBus.IsAvailable) {
                EventBus.Instance.Subscribe<T>(eventName, handler);
            }
        }
        public static void Unsubscribe<T>(string eventName, System.Action<T> handler) {
            if (EventBus.IsAvailable) {
                EventBus.Instance.Unsubscribe<T>(eventName, handler);
            }
        }
        public static void Trigger<T>(string eventName, T payload) {
            if (EventBus.IsAvailable) {
                EventBus.Instance.Trigger<T>(eventName, payload);
            }
        }
        // --- Non-generic event helpers ---
        public static void Subscribe(string eventName, System.Action handler) {
            if (EventBus.IsAvailable) {
                EventBus.Instance.Subscribe(eventName, new UnityEngine.Events.UnityAction(handler), EventBusHelper.GetDomainFromEventName(eventName));
            }
        }
        public static void Unsubscribe(string eventName, System.Action handler) {
            if (EventBus.IsAvailable) {
                EventBus.Instance.Unsubscribe(eventName, new UnityEngine.Events.UnityAction(handler));
            }
        }
        public static void Trigger(string eventName) {
            if (EventBus.IsAvailable) {
                EventBus.Instance.Trigger(eventName);
            }
        }

        public static bool HasEvent(string eventName) {
            return EventBus.IsAvailable && EventBus.Instance.HasEvent(eventName);
        }

        public static List<string> GetEventsInDomain(EventDomain domain) {
            return EventBus.IsAvailable ? EventBus.Instance.GetEventsInDomain(domain) : new List<string>();
        }

        public static void CreateEvent(string eventName, EventDomain domain) {
            if (EventBus.IsAvailable) {
                EventBus.Instance.Subscribe(eventName, () => { }, domain);
            }
        }

        public static List<EventData> GetAllEvents() {
            if (EventBus.IsAvailable) {
                return EventBus.Instance.activeEvents;
            }
            return new List<EventData>();
        }

        private static EventDomain GetDomainFromEventName(string eventName) {
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
    }
}
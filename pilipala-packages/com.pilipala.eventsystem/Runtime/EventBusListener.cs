using System;
using System.Collections.Generic;
using UnityEngine;

namespace EventSystem {
    /// <summary>
    /// Base MonoBehaviour that tracks and auto-unsubscribes all generic event subscriptions on destroy.
    /// </summary>
    public abstract class EventBusListener : MonoBehaviour {
        private List<Action> _unsubscribeActions = new List<Action>();

        /// <summary>
        /// Subscribe to a type-safe event and auto-unsubscribe on destroy.
        /// </summary>
        protected void Subscribe<TPayload>(string eventName, Action<TPayload> handler) {
            EventBus.Instance.Subscribe<TPayload>(eventName, handler);
            _unsubscribeActions.Add(() => EventBus.Instance.Unsubscribe<TPayload>(eventName, handler));
        }

        protected virtual void OnDestroy() {
            foreach (var unsub in _unsubscribeActions) {
                unsub();
            }
            _unsubscribeActions.Clear();
        }
    }
}

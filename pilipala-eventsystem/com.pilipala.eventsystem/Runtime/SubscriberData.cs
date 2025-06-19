using System;

namespace EventSystem {
    [System.Serializable]
    public class SubscriberData {
        public string subscriberName;
        public EventDomain domain;
        public string subscribedToEvent;
        public bool isActive;
    }
}
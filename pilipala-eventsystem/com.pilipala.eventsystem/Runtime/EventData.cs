using System;

namespace EventSystem {
    [System.Serializable]
    public class EventData
    {
        public string eventName;
        public EventDomain domain;
        public int subscriberCount;
        public bool isActive;
        public string lastTriggeredTime;
        public string eventDescription;
    }
}


   
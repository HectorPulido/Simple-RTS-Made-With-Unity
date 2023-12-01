using UnityEngine;

namespace Unity.Services.Analytics.Internal
{
    public class Event
    {
        public Event(string name, int? version)
        {
            Name = name;
            Version = version;
            Parameters = new EventData();
        }

        public EventData Parameters { get; private set; }
        public string Name { get; private set; }
        public int? Version { get; private set; }
    }
}

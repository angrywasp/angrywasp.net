using System;
using System.Collections.Generic;

namespace AngryWasp.Net
{
    public static class TimedEventManager
    {
        private static Dictionary<string, TimedEvent> events = new Dictionary<string, TimedEvent>();

        public static void Add(string name, Action action, int frequency) => events.Add(name, TimedEvent.Create(action, frequency));

        public static void Stop(string name) => events[name].Stop();

         public static void Start(string name) => events[name].Start();
    }
}
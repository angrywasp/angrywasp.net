using System;
using System.Timers;

namespace AngryWasp.Net
{
    public class TimedEvent
    {
        private Timer timer;

        public static TimedEvent Create(Action action, int frequency)
        {
            TimedEvent te = new TimedEvent();
            te.timer = new Timer(frequency);

            te.timer.Elapsed += (s, e) =>
            {
                action.Invoke();
            };

            te.Start();

            return te;
        }

        public void Start() => timer.Start();

        public void Stop() => timer.Stop();
    }
}
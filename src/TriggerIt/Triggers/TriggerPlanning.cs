using System;
using System.Collections.Generic;
using System.Text;

namespace TriggerIt.Triggers
{
    public class TriggerPlanning
    {
        private TriggerPlanning() { }

        internal string LastEventName { get; set; }
        internal Func<int, bool> EverySeconds { get; set; }
        internal TimeSpan? MinUptime { get; set; }
        internal DateTime? MinRealDateTime { get; set; }
        internal Dictionary<string, Func<int, bool>> EventsCounts { get; } = new Dictionary<string, Func<int, bool>>();
        internal Dictionary<string, Func<int, bool>> EventsCountsSinceUptime { get; } = new Dictionary<string, Func<int, bool>>();
        internal TriggerPlanningPeriodicities Periodicity { get; set; } = TriggerPlanningPeriodicities.Default;

        public static TriggerPlanning If()
        {
            return new TriggerPlanning();
        }

        public TriggerPlanning UptimeIs(TimeSpan timeSpan)
        {
            this.EverySeconds = i => i == timeSpan.TotalSeconds;

            return this;
        }

        public TriggerPlanning UptimeIs(Func<int, bool> func)
        {
            this.EverySeconds = func;

            return this;
        }

        public TriggerPlanning EventCountIs(string eventName, int count)
        {
            this.EventsCounts.Add(eventName, i => i == count);

            return this;
        }

        public TriggerPlanning EventCountIs(string eventName, Func<int, bool> countFunc)
        {
            this.EventsCounts.Add(eventName, countFunc);

            return this;
        }

        public TriggerPlanning EventCountSinceUptimeIs(string eventName, int count)
        {
            this.EventsCountsSinceUptime.Add(eventName, i => i == count);

            return this;
        }

        public TriggerPlanning EventCountSinceUptimeIs(string eventName, Func<int, bool> countFunc)
        {
            this.EventsCountsSinceUptime.Add(eventName, countFunc);

            return this;
        }

        public TriggerPlanning LastEventIs(string name)
        {
            this.LastEventName = name;

            return this;
        }

        public TriggerPlanning RealTimeIsAfter(DateTime dateTime)
        {
            this.MinRealDateTime = dateTime;

            return this;
        }

        public TriggerPlanning UptimeIsAtLeast(TimeSpan minUptime)
        {
            this.MinUptime = minUptime;

            return this;
        }

        public TriggerPlanning WithPeriodicity(TriggerPlanningPeriodicities periodicity = TriggerPlanningPeriodicities.Default)
        {
            this.Periodicity = periodicity;

            return this;
        }
    }

    public enum TriggerPlanningPeriodicities
    {
        Default,
        OnceSinceZeroUptime,
        OnceEver
    }
}

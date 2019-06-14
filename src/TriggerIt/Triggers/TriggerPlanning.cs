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

        /// <summary>
        /// Execute when uptime is exactly the given value. In the default setting, TriggerIt ticks once per 5 seconds.
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public TriggerPlanning UptimeIs(TimeSpan timeSpan)
        {
            this.EverySeconds = i => i == timeSpan.TotalSeconds;

            return this;
        }

        /// <summary>
        /// Execute when uptime meets the given condition. This is useful for "execute every X seconds" scenario.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public TriggerPlanning UptimeIs(Func<int, bool> func)
        {
            this.EverySeconds = func;

            return this;
        }

        /// <summary>
        /// Execute when the given event was logged for Count times.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public TriggerPlanning EventCountIs(string eventName, int count)
        {
            this.EventsCounts.Add(eventName, i => i == count);

            return this;
        }

        /// <summary>
        /// Execute when the count of the given event meets the given condition.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="countFunc"></param>
        /// <returns></returns>
        public TriggerPlanning EventCountIs(string eventName, Func<int, bool> countFunc)
        {
            this.EventsCounts.Add(eventName, countFunc);

            return this;
        }

        /// <summary>
        /// Execute when the given event was logged for Count times since new Uptime.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public TriggerPlanning EventCountSinceUptimeIs(string eventName, int count)
        {
            this.EventsCountsSinceUptime.Add(eventName, i => i == count);

            return this;
        }

        /// <summary>
        /// Execute when the count of the given event since new Uptime meets the given condition.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="countFunc"></param>
        /// <returns></returns>
        public TriggerPlanning EventCountSinceUptimeIs(string eventName, Func<int, bool> countFunc)
        {
            this.EventsCountsSinceUptime.Add(eventName, countFunc);

            return this;
        }

        /// <summary>
        /// Execute when the last event is the given event.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TriggerPlanning LastEventIs(string name)
        {
            this.LastEventName = name;

            return this;
        }

        /// <summary>
        /// Execute when DateTime.Now is less than the given DateTime.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public TriggerPlanning RealTimeIsAfter(DateTime dateTime)
        {
            this.MinRealDateTime = dateTime;

            return this;
        }

        /// <summary>
        /// Execute when Uptime is less than the given value.
        /// </summary>
        /// <param name="minUptime"></param>
        /// <returns></returns>
        public TriggerPlanning UptimeIsAtLeast(TimeSpan minUptime)
        {
            this.MinUptime = minUptime;

            return this;
        }

        /// <summary>
        /// Execute with the given periodicity.
        /// </summary>
        /// <param name="periodicity"></param>
        /// <returns></returns>
        public TriggerPlanning WithPeriodicity(TriggerPlanningPeriodicities periodicity = TriggerPlanningPeriodicities.Default)
        {
            this.Periodicity = periodicity;

            return this;
        }
    }

    /// <summary>
    /// Trigger executing periodicity
    /// </summary>
    public enum TriggerPlanningPeriodicities
    {
        /// <summary>
        /// Default executing periodicity depends on planner settings. It allows to execute triggers multiple times.
        /// </summary>
        Default,

        /// <summary>
        /// Allow executing just once per new Uptime.
        /// </summary>
        OnceSinceZeroUptime,

        /// <summary>
        /// Allow executing just once ever.
        /// </summary>
        OnceEver
    }
}

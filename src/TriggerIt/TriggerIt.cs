using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TriggerIt.Persisting;
using TriggerIt.Triggers;

[assembly: InternalsVisibleTo("TriggerIt.Tests")]
namespace TriggerIt
{
    /// <summary>
    /// The main service class of TriggerIt.
    /// </summary>
    public class TriggerIt
    {
        public int TriggerCheckFireIntervalSeconds { get; set; } = 5;
        private int uptimeSeconds = 0;

        private List<(Triggers.TriggerPlanning plan, Trigger val)> triggers = new List<(Triggers.TriggerPlanning plan, Trigger val)>();

        private List<Event> inMemoryEvents = new List<Event>();
        private List<Event> inMemoryEventsSinceUptime = new List<Event>();

        private Dictionary<string, int> cachedEventStats = new Dictionary<string, int>();
        private Dictionary<string, int> cachedEventStatsSinceUptime = new Dictionary<string, int>();

        private List<ExecutedTrigger> executedTriggers = new List<ExecutedTrigger>();
        private List<ExecutedTrigger> executedTriggersSinceUptime = new List<ExecutedTrigger>();

        internal ITimer Timer { get; set; }
        internal IPersitor EventsPersitor { get; set; }

        internal Utils.IDateTimeService DateTimeService { get; set; } = new Utils.DateTimeService();

        /// <summary>
        /// 
        /// </summary>
        public TriggerIt(ITimer timer, IPersitor eventsPersitor)
        {
            this.EventsPersitor = eventsPersitor ?? throw new ArgumentNullException(nameof(eventsPersitor));
            this.Timer = timer ?? throw new ArgumentNullException(nameof(timer));
        }

        /// <summary>
        /// You have to call this before you use TriggerIt service.
        /// </summary>
        public async Task InitializeAsync()
        {
            await this.InitializeInternalAsync();
            this.StartTimerInternal();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal async Task InitializeInternalAsync()
        {
            this.inMemoryEvents = (await this.EventsPersitor.LoadEventsAsync()).ToList();
            this.executedTriggers = (await this.EventsPersitor.LoadExecutedTriggersAsync()).ToList();

            this.cachedEventStats = this.inMemoryEvents.GroupBy(i => i.Name).ToDictionary(i => i.Key, i => i.Count());
        }

        /// <summary>
        /// 
        /// </summary>
        internal void StartTimerInternal()
        {
            this.Timer.Start(TimeSpan.FromSeconds(TriggerCheckFireIntervalSeconds), async () =>
            {
                await this.OnTimerFired();
            });
        }

        /// <summary>
        /// Unregisters a trigger instance.
        /// </summary>
        /// <param name="triggerInstance"></param>
        public void UnregisterTrigger(string name)
        {
            this.triggers.RemoveAll(i => i.val.Name == name);
        }

        /// <summary>
        /// Registers a trigger instance.
        /// </summary>
        /// <param name="triggerInstance"></param>
        public void RegisterTrigger(Trigger triggerInstance, TriggerPlanning triggerPlanning)
        {
            if (triggerInstance == null)
            {
                throw new ArgumentNullException(nameof(triggerInstance));
            }

            this.triggers.Add((triggerPlanning, triggerInstance));
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task OnTimerFired()
        {
            this.uptimeSeconds += TriggerCheckFireIntervalSeconds;

            await this.TryFireTriggers();
        }

        private bool executingTriggers = false;

        /// <summary>
        /// 
        /// </summary>
        private async Task TryFireTriggers()
        {
            if (this.executingTriggers) return;
            this.executingTriggers = true;

            var triggersToPersist = new List<ExecutedTrigger>();

            foreach (var triggerKv in this.triggers.ToList())
            {
                var conditions = new List<(bool has, Func<bool> should)>();

                (bool has, Func<bool> should) lastEvent = (has: triggerKv.plan.LastEventName != null,
                                                        should: () => triggerKv.plan.LastEventName == this.inMemoryEventsSinceUptime.LastOrDefault()?.Name);
                conditions.Add(lastEvent);

                (bool has, Func<bool> should) minUptime = (has: triggerKv.plan.MinUptime != null,
                                                        should: () => triggerKv.plan.MinUptime.Value.TotalSeconds <= this.uptimeSeconds);
                conditions.Add(minUptime);

                (bool has, Func<bool> should) eventsCounts = (has: triggerKv.plan.EventsCounts.Any(),
                                                            should: () => this.cachedEventStats.Any() &&
                                                                        this.cachedEventStats.All(i => triggerKv.plan.EventsCounts.Any(y => y.Key == i.Key && y.Value(i.Value))));
                conditions.Add(eventsCounts);

                (bool has, Func<bool> should) eventsCountsSinceUptime = (has: triggerKv.plan.EventsCountsSinceUptime.Any(),
                                                            should: () => this.cachedEventStatsSinceUptime.Any() &&
                                                                        this.cachedEventStatsSinceUptime.All(i => triggerKv.plan.EventsCountsSinceUptime.Any(y => y.Key == i.Key && y.Value(i.Value))));
                conditions.Add(eventsCountsSinceUptime);

                (bool has, Func<bool> should) minRealDateTime = (has: triggerKv.plan.MinRealDateTime != null,
                                                                should: () => triggerKv.plan.MinRealDateTime.Value <= this.DateTimeService.Now);
                conditions.Add(minRealDateTime);

                (bool has, Func<bool> should) everySeconds = (has: triggerKv.plan.EverySeconds != null,
                                                            should: () => triggerKv.plan.EverySeconds(this.uptimeSeconds));
                conditions.Add(everySeconds);

                (bool has, Func<bool> should) periodicity = (has: triggerKv.plan.Periodicity != TriggerPlanningPeriodicities.Default,
                                                            should: () => ShouldRunOnTriggerPeridiocity(triggerKv.plan.Periodicity, triggerKv.val));
                conditions.Add(periodicity);

                if (conditions.Where(i => i.has).All(i => i.should()))
                {
                    await ExecuteTrigger(triggerKv.val);
                }
            }

            await this.EventsPersitor.PersistsExecutedTriggersAsync(triggersToPersist);

            this.executingTriggers = false;

            // Check if trigger should run based on periodicity
            bool ShouldRunOnTriggerPeridiocity(Triggers.TriggerPlanningPeriodicities periodicity, Trigger trigger)
            {
                if (periodicity == TriggerPlanningPeriodicities.OnceEver && this.executedTriggers.Any(i => i.Name == trigger.Name)) return false;
                if (periodicity == TriggerPlanningPeriodicities.OnceSinceZeroUptime && this.executedTriggersSinceUptime.Any(i => i.Name == trigger.Name)) return false;

                return true;
            }

            // Execute trigger with proper params
            async Task ExecuteTrigger(Trigger triggerBase)
            {
                await triggerBase.ExecuteAsync(new TriggerContext()
                {
                    LastEvent = this.inMemoryEvents.LastOrDefault()?.Name,
                    Uptime = TimeSpan.FromSeconds(this.uptimeSeconds),
                    TriggerTypeExecutedCount = this.executedTriggers.Count(i => i.Name == triggerBase.Name) + 1
                });

                var executedTrigger = new ExecutedTrigger(triggerBase.Name);
                this.executedTriggers.Add(executedTrigger);
                this.executedTriggersSinceUptime.Add(executedTrigger);

                triggersToPersist.Add(executedTrigger);
            }
        }

        /// <summary>
        /// Logs an event.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="scope"></param>
        public async Task LogEventAsync(string name)
        {
            var eventObj = new Event()
            {
                Name = name,
                CreatedAt = DateTime.Now
            };

            this.inMemoryEvents.Add(eventObj);
            this.inMemoryEventsSinceUptime.Add(eventObj);
            await this.EventsPersitor.PersistEventAsync(eventObj);

            this.UpdateEventStats(this.cachedEventStats, name);
            this.UpdateEventStats(this.cachedEventStatsSinceUptime, name);

            await this.TryFireTriggers();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dict"></param>
        private void UpdateEventStats(IDictionary<string, int> dict, string name)
        {
            if (!dict.TryGetValue(name, out var eventCount))
            {
                dict.Add(name, 1);
            }
            else
            {
                dict[name] = ++eventCount;
            }
        }
    }
}

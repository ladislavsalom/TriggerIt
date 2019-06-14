using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TriggerIt.Triggers;

namespace TriggerIt.Tests
{
    public static class Init
    {
        public static async Task<TriggerIt> TriggerItWithImmediateTimerAsync(bool history = false)
        {
            var s = new TriggerIt(new Mock.ImmediateTimer(100), (history ? (Persisting.IPersitor)new Mock.PersistorWithHistory() : new Mock.PersistorWithoutHistory()));

            s.DateTimeService = new Mock.DateTimeService();

            await s.InitializeInternalAsync();

            return s;
        }

        public static async Task<(TriggerIt triggerIt, Mock.ManualTimer timer)> TriggerItWithManualTimerAsync(bool history = false)
        {
            var timer = new Mock.ManualTimer();
            var s = new TriggerIt(timer, (history ? (Persisting.IPersitor)new Mock.PersistorWithHistory() : new Mock.PersistorWithoutHistory()));

            s.DateTimeService = new Mock.DateTimeService();

            await s.InitializeInternalAsync();

            return (s, timer);
        }

        public static void TimerAgain(TriggerIt triggerIt)
        {
            triggerIt.Timer = new Mock.ImmediateTimer(100);
        }

        public class HitTriggerTrigger : Triggers.Trigger
        {
            public override string Name => nameof(HitTriggerTrigger);

            public TriggerContext LastContext { get; set; }
            public int HitCount { get; set; }

            public bool WasHitOnce => this.HitCount == 1;
            public bool WasHitMoreThanOnce => this.HitCount > 1;
            public bool WasNotHit => this.HitCount == 0;
            public bool WasHitAtLeastOnce => this.HitCount > 0;

            public override Task ExecuteAsync(TriggerContext context)
            {
                this.LastContext = context;
                this.HitCount++;

                return Task.CompletedTask;
            }
        }

        public static HitTriggerTrigger HitTrigger()
        {
            return new HitTriggerTrigger();
        }

        public static void HitTriggerAgain(HitTriggerTrigger t)
        {
            t.LastContext = null;
            t.HitCount = 0;
        }
    }
}

using System;
using System.Threading.Tasks;
using TriggerIt.Triggers;
using Xunit;

namespace TriggerIt.Tests
{
    public class TriggersTests
    {
        [Fact]
        public async Task TriggersImmediatelyAfterEvent()
        {
            var service = await Init.TriggerItWithImmediateTimerAsync();
            var trigger = Init.HitTrigger();

            var eventName = "test";
            service.RegisterTrigger(trigger, TriggerPlanning.If().LastEventIs(eventName));

            await service.LogEventAsync(eventName);

            Assert.True(trigger.WasHitOnce);
            Assert.True(trigger.LastContext.LastEvent == eventName);
        }

        [Fact]
        public async Task RealTimeCheck()
        {
            var service = await Init.TriggerItWithImmediateTimerAsync();
            var trigger = Init.HitTrigger();

            var realTimeService = new Mock.DateTimeService();
            realTimeService.RequestedDateTime = DateTime.Now.AddMinutes(-10);

            var eventName = "test";
            service.RegisterTrigger(trigger, TriggerPlanning.If().LastEventIs(eventName).RealTimeIsAfter(realTimeService.Now).WithPeriodicity(TriggerPlanningPeriodicities.OnceEver));

            await service.LogEventAsync(eventName);

            service.StartTimerInternal();

            Assert.True(trigger.WasHitOnce);

            Init.TimerAgain(service);
            Init.HitTriggerAgain(trigger);

            realTimeService.RequestedDateTime = DateTime.Now.AddMinutes(10);
            service.StartTimerInternal();

            Assert.True(trigger.WasNotHit);
        }

        [Fact]
        public async Task TriggersAfterUptimeOnce()
        {
            (var service, var timer) = await Init.TriggerItWithManualTimerAsync();
            var trigger = Init.HitTrigger();

            service.RegisterTrigger(trigger, TriggerPlanning.If().UptimeIsAtLeast(TimeSpan.FromSeconds(service.TriggerCheckFireIntervalSeconds * 3)).WithPeriodicity(TriggerPlanningPeriodicities.OnceSinceZeroUptime)); // 10 = 3 ticks with default settings
            service.StartTimerInternal();
            Assert.True(trigger.WasNotHit);

            timer.Tick();
            Assert.True(trigger.WasNotHit);

            timer.Tick();
            Assert.True(trigger.WasNotHit);

            timer.Tick();
            Assert.True(trigger.WasHitOnce);

            timer.Tick();
            Assert.True(trigger.WasHitOnce);
        }

        [Fact]
        public async Task TriggersAfterUptime()
        {
            (var service, var timer) = await Init.TriggerItWithManualTimerAsync();
            var trigger = Init.HitTrigger();

            service.RegisterTrigger(trigger, TriggerPlanning.If().UptimeIsAtLeast(TimeSpan.FromSeconds(service.TriggerCheckFireIntervalSeconds * 3))
                .WithPeriodicity(TriggerPlanningPeriodicities.Default)); // 10 = 3 ticks with default settings

            service.StartTimerInternal();
            Assert.True(trigger.WasNotHit);

            timer.Tick();
            Assert.True(trigger.WasNotHit);

            timer.Tick();
            Assert.True(trigger.WasNotHit);

            timer.Tick();
            Assert.True(trigger.HitCount == 1);

            timer.Tick();
            Assert.True(trigger.HitCount == 2);

            timer.Tick();
            Assert.True(trigger.HitCount == 3);
        }

        [Fact]
        public async Task TriggersEveryTwoTicks()
        {
            (var service, var timer) = await Init.TriggerItWithManualTimerAsync();
            var trigger = Init.HitTrigger();

            service.RegisterTrigger(trigger, TriggerPlanning.If().UptimeIs(uptime => uptime % 2 == 0).WithPeriodicity(TriggerPlanningPeriodicities.Default));
            service.StartTimerInternal();
            Assert.True(trigger.WasNotHit);

            timer.Tick();
            Assert.True(trigger.WasNotHit);

            timer.Tick();
            Assert.True(trigger.WasHitOnce);

            timer.Tick();
            Assert.True(trigger.WasHitOnce);

            timer.Tick();
            Assert.True(trigger.HitCount == 2);

            timer.Tick();
            Assert.True(trigger.HitCount == 2);
        }

        [Fact]
        public async Task TriggerOnEventCounts()
        {
            var service = await Init.TriggerItWithImmediateTimerAsync();
            var trigger = Init.HitTrigger();

            var eventName = "test";
            service.RegisterTrigger(trigger, TriggerPlanning.If().EventCountIs(eventName, 3));
            service.StartTimerInternal();

            await service.LogEventAsync(eventName);
            Assert.True(trigger.WasNotHit);

            await service.LogEventAsync(eventName);
            Assert.True(trigger.WasNotHit);

            await service.LogEventAsync(eventName);
            Assert.True(trigger.WasHitOnce);

            await service.LogEventAsync(eventName);
            Assert.True(trigger.WasHitOnce);
        }
    }
}

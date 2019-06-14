using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggerIt.Tests.Mock
{
    public class PersistorWithHistory : Persisting.IPersitor
    {
        private List<Event> events = new List<Event>();
        private List<ExecutedTrigger> executedTriggers = new List<ExecutedTrigger>();

        public PersistorWithHistory()
        {
            var eventName = "test";
            this.events.Add(new Event() { CreatedAt = DateTime.Now.AddDays(-3), Name = eventName });
            this.events.Add(new Event() { CreatedAt = DateTime.Now.AddDays(-3), Name = eventName });
            this.events.Add(new Event() { CreatedAt = DateTime.Now.AddDays(-2), Name = eventName });
            this.events.Add(new Event() { CreatedAt = DateTime.Now.AddDays(-1), Name = eventName });

            this.executedTriggers.Add(new ExecutedTrigger() { ExecutedAt = DateTime.Now.AddDays(-1), Name = nameof(Init.HitTriggerTrigger) });
        }

        public Task<IEnumerable<Event>> LoadEventsAsync()
        {
            return Task.FromResult(this.events.AsEnumerable());
        }

        public Task<IEnumerable<ExecutedTrigger>> LoadExecutedTriggersAsync()
        {
            return Task.FromResult(this.executedTriggers.AsEnumerable());
        }

        public Task PersistEventAsync(Event e)
        {
            this.events.Add(e);

            return Task.CompletedTask;
        }

        public Task PersistsExecutedTriggersAsync(List<ExecutedTrigger> triggerNames)
        {
            this.executedTriggers.AddRange(triggerNames);

            return Task.CompletedTask;
        }
    }
}

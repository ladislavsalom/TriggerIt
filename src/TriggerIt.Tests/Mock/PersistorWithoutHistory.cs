using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TriggerIt.Tests.Mock
{
    public class PersistorWithoutHistory : Persisting.IPersitor
    {
        private List<Event> events = new List<Event>();
        private List<ExecutedTrigger> executedTriggers = new List<ExecutedTrigger>();

        public async Task<IEnumerable<Event>> LoadEventsAsync()
        {
            return await Task.FromResult(this.events);
        }

        public async Task<IEnumerable<ExecutedTrigger>> LoadExecutedTriggersAsync()
        {
            return await Task.FromResult(this.executedTriggers);
        }

        public async Task PersistEventAsync(Event e)
        {
            this.events.Add(e);

            await Task.CompletedTask;
        }

        public async Task PersistsExecutedTriggersAsync(List<ExecutedTrigger> triggerNames)
        {
            this.executedTriggers.AddRange(triggerNames);

            await Task.CompletedTask;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TriggerIt.Tests.Mock
{
    public class Persistor : Persisting.IEventsPersitor
    {
        private List<Event> events = new List<Event>();
        private List<string> executedTriggers = new List<string>();

        public async Task<IEnumerable<Event>> LoadEventsAsync()
        {
            return await Task.FromResult(this.events);
        }

        public async Task<IEnumerable<string>> LoadExecutedTriggersAsync()
        {
            return await Task.FromResult(this.executedTriggers);
        }

        public async Task PersistEventAsync(Event e)
        {
            this.events.Add(e);

            await Task.CompletedTask;
        }

        public async Task PersistsExecutedTriggersAsync(List<string> triggerNames)
        {
            this.executedTriggers.AddRange(triggerNames);

            await Task.CompletedTask;
        }
    }
}

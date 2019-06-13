using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TriggerIt.Persisting
{
    public interface IEventsPersitor
    {
        Task PersistEventAsync(Event e);
        Task<IEnumerable<Event>> LoadEventsAsync();

        Task PersistsExecutedTriggersAsync(List<string> triggerNames);
        Task<IEnumerable<string>> LoadExecutedTriggersAsync();
    }
}

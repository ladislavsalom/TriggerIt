using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TriggerIt.Persisting
{
    public interface IPersitor
    {
        Task PersistEventAsync(Event e);
        Task<IEnumerable<Event>> LoadEventsAsync();

        Task PersistsExecutedTriggersAsync(List<ExecutedTrigger> triggerNames);
        Task<IEnumerable<ExecutedTrigger>> LoadExecutedTriggersAsync();
    }
}

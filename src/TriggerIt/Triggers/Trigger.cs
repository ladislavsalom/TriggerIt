using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TriggerIt.Triggers
{
    public abstract class Trigger
    {
        public abstract string Name { get; }
        public abstract Task ExecuteAsync(TriggerContext context);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TriggerIt.Triggers
{
    public class TriggerContext
    {
        internal TriggerContext()
        {
        }

        public string LastEvent { get; internal set; }
        public TimeSpan Uptime { get; internal set; }
        public int TriggerTypeExecutedCount { get; internal set; }
    }
}

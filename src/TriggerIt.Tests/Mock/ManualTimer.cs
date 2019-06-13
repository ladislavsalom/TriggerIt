using System;
using System.Collections.Generic;
using System.Text;

namespace TriggerIt.Tests.Mock
{
    public class ManualTimer : ITimer
    {
        private Action tickAction;

        public void Destroy()
        {
        }

        public void Start(TimeSpan interval, Action tickAction)
        {
            this.tickAction = tickAction;
        }

        public void Tick()
        {
            this.tickAction();
        }
    }
}

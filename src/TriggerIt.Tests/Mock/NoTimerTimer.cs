using System;
using System.Collections.Generic;
using System.Text;

namespace TriggerIt.Tests.Mock
{
    class NoTimerTimer : ITimer
    {
        public void Destroy()
        {
        }

        public void Start(TimeSpan interval, Action tickAction)
        {
        }
    }
}

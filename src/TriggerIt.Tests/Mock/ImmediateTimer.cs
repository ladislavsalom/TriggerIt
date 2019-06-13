using System;
using System.Collections.Generic;
using System.Text;

namespace TriggerIt.Tests.Mock
{
    class ImmediateTimer : ITimer
    {
        private readonly int ticksCount;

        public ImmediateTimer(int ticksCount)
        {
            this.ticksCount = ticksCount;
        }

        public void Destroy()
        {
        }

        public void Start(TimeSpan interval, Action tickAction)
        {
            for (var i = 1; i <= this.ticksCount; i++)
            {
                tickAction();
            }
        }
    }
}

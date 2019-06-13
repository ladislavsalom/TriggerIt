using System;
using System.Collections.Generic;
using System.Text;

namespace TriggerIt
{
    public interface ITimer
    {
        void Start(TimeSpan interval, Action tickAction);
        void Destroy();
    }
}

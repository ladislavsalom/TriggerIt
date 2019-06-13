using System;
using System.Collections.Generic;
using System.Text;

namespace TriggerIt.Utils
{
    class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.Now;
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TriggerIt.Tests.Mock
{
    class DateTimeService : Utils.IDateTimeService
    {
        public DateTime RequestedDateTime { get; set; } = DateTime.Now;

        public DateTime Now => this.RequestedDateTime;
    }
}

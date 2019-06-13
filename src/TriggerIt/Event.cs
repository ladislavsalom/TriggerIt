using System;
using System.Collections.Generic;
using System.Text;

namespace TriggerIt
{
    public class Event
    {
        public Event()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string Name { get; internal set; }
        public DateTime CreatedAt { get; internal set; }
    }
}

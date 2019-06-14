using System;
using System.Collections.Generic;
using System.Text;

namespace TriggerIt
{
    public class ExecutedTrigger
    {
        public ExecutedTrigger() { }
        public ExecutedTrigger(string name) { this.Name = name; }

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public DateTime ExecutedAt { get; set; }
    }
}

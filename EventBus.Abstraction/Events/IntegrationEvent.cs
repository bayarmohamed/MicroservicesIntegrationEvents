using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Abstraction.Events
{
    public class IntegrationEvent
    {
        public DateTime Timespan { get; set; }
        public Guid EventId  { get; set; }

        public IntegrationEvent()
        {
            Timespan = DateTime.Now;
            EventId = Guid.NewGuid();
        }
    }
}

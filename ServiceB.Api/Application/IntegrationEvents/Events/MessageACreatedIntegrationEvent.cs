using EventBus.Abstraction.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceB.Api.Application.IntegrationEvents.Events
{
    public class MessageACreatedIntegrationEvent:IntegrationEvent
    {
        public string Message { get; set; }
    }
}

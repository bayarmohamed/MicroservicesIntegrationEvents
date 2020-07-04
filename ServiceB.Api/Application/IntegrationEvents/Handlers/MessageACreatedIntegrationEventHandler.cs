using EventBus.Abstraction.Events;
using ServiceB.Api.Application.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceB.Api.Application.IntegrationEvents.Handlers
{
    public class MessageACreatedIntegrationEventHandler : IIntegrationEventHandler<MessageACreatedIntegrationEvent>
    {
        public Task Handle(MessageACreatedIntegrationEvent @event)
        {
            return Task.CompletedTask;
            
        }
    }
}

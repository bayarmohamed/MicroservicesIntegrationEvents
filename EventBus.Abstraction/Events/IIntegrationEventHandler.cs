using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Abstraction.Events
{
    public interface IIntegrationEventHandler<in TEvent> where TEvent:IntegrationEvent
    {
        Task Handle(TEvent @event);
    }
}

using EventBus.Abstraction.Bus;
using MediatR;
using ServiceA.Api.Application.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceA.Api.Application.Commands
{
    public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, bool>
    {
        private readonly IEventBus _bus;
        public CreateMessageCommandHandler(IEventBus bus)
        {
            _bus = bus;
        }
        public Task<bool> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
        {
            _bus.Publish(new MessageACreatedIntegrationEvent { Message = request.Message });
            return Task.FromResult(true);
        }
    }
}

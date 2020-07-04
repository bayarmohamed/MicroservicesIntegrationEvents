using EventBus.Abstraction.Bus;
using EventBus.Abstraction.Events;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace RabbitMQBus.Infra
{
    public class RabbitMQBus : IEventBus
    {
        Dictionary<string, List<Type>> handlers;
        List<Type> eventTypes;
        IServiceScopeFactory _serviceScopeFactory;

        public RabbitMQBus(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            handlers = new Dictionary<string, List<Type>>();
            eventTypes = new List<Type>();
        }
        public void Publish<T>(T @event) where T : IntegrationEvent
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var eventName = @event.GetType().Name;
                    channel.QueueDeclare(eventName, false, false, false, null);

                    var message = JsonConvert.SerializeObject(@event);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish("", eventName, null, body);

                }
            }
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = typeof(T).Name;
            var handlerType = typeof(TH);

            if (!eventTypes.Contains(typeof(T)))
            {
                eventTypes.Add(typeof(T));
            }
            if (!handlers.ContainsKey(eventName))
            {
                handlers.Add(eventName, new List<Type>());
            }
            if (handlers[eventName].Any(s => s.GetType() == handlerType))
            {
                throw new ArgumentException($"Handler type { handlerType.Name } already is registred for {eventName}"
                    , nameof(handlerType));
            }
            handlers[eventName].Add(handlerType);

            StartBasicConsumer<T>();

        }

        private void StartBasicConsumer<T>() where T : IntegrationEvent
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                DispatchConsumersAsync = true
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            var eventName = typeof(T).Name;

            channel.QueueDeclare(eventName, false, false, false, null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += consumer_received;
            channel.BasicConsume(eventName, true, consumer);

        }

        private async Task consumer_received(object sender, BasicDeliverEventArgs @event)
        {
            var eventName = @event.RoutingKey;
            string message = Encoding.UTF8.GetString(@event.Body.ToArray());
            try
            {
                await ProcessEvent(eventName, message).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (handlers.ContainsKey(eventName))
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var subscriptions = handlers[eventName];
                    foreach (var subscription in subscriptions)
                    {
                        var handler = scope.ServiceProvider.GetService(subscription);
                        // handler = Activator.CreateInstance(subscription);
                        if (handler == null) continue;
                        var eventType = eventTypes.SingleOrDefault(t => t.Name == eventName);
                        var @event = JsonConvert.DeserializeObject(message, eventType);
                        var concretType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                        await (Task)concretType.GetMethod("Handle").Invoke(handler, new object[] { @event });
                    }
                }
            }
        }
    }
}

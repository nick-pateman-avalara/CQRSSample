using NServiceBus;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace CQRSAPI.Messages
{

    public class PeopleRabbitMQMessageTransport : IMessageTransport
    {

        private IEndpointInstance _endpoint;

        public static PeopleRabbitMQMessageTransport Instance { get; private set; }

        public static async Task InitialiseAsync(string connectionString)
        {
            if(Instance == null)
            {
                Instance = new PeopleRabbitMQMessageTransport();
                await Instance.CreateEndpointAsync(connectionString);

            }
        }

        private async Task CreateEndpointAsync(string connectionString)
        {
            EndpointConfiguration endpointConfiguration = new EndpointConfiguration("CQRSAPI.Messages.In");

            TransportExtensions<RabbitMQTransport> transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            transport.UseDirectRoutingTopology();
            transport.ConnectionString(connectionString);

            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UsePersistence<LearningPersistence>();
            endpointConfiguration.SendFailedMessagesTo("CQRSAPI.Messages.Error");

            _endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false); ;
        }

        public async Task PublishAsync(PersonEventMessage message)
        {
            SendOptions sendOptions = new SendOptions();
            sendOptions.SetDestination("CQRSAPI.Messages.Out");
            await _endpoint.Send(message, sendOptions).ConfigureAwait(false); ;
        }

    }
}

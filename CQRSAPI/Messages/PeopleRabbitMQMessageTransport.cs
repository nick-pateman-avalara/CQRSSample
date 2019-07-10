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
            EndpointConfiguration endpointConfiguration = new EndpointConfiguration("CqrsSample.CqrsApi");
            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            transport.UseConventionalRoutingTopology();
            transport.ConnectionString(connectionString);
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            _endpoint = await Endpoint.Start(endpointConfiguration);
        }

        public async Task SendLocalAsync(PersonMessage message)
        {
            await _endpoint.SendLocal(message);
        }

    }
}

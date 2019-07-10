using System.Threading.Tasks;
using CQRSAPI.Messages;
using NServiceBus;

namespace CQRSAPI.People.Messages
{

    public class PeopleRabbitMqMessageTransport : IMessageTransport
    {

        private IEndpointInstance _endpoint;

        public static PeopleRabbitMqMessageTransport Instance { get; private set; }

        public static async Task InitialiseAsync(string connectionString)
        {
            if(Instance == null)
            {
                Instance = new PeopleRabbitMqMessageTransport();
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

            _endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
        }

        public async Task SendAsync(PersonEventMessage message)
        {
            //I needed to manually create this Queue on RabbitMQ using the admin portal
            //Ideally we would use an exchange that publishes to multiple queues, the
            //topology would need to be changed from DirectRouting first.
            SendOptions sendOptions = new SendOptions();
            sendOptions.SetDestination("CQRSAPI.Messages.Out");
            await _endpoint.Send(message, sendOptions).ConfigureAwait(false);
        }

        public static async Task SendIfInitialisedAsync(PersonEventMessage message)
        {
            if (Instance != null)
            {
                await Instance.SendAsync(message);
            }
        }

    }
}

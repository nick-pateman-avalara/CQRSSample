using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NServiceBus;
using NServiceBus.Serialization;

namespace CQRSAPI.Messages
{

    public class RabbitMqMessageTransport : IMessageTransport
    {

        private readonly bool _enabled;
        private IEndpointInstance _endpoint;

        public RabbitMqMessageTransport(bool enabled)
        {
            _enabled = enabled;
        }

        private async Task CreateEndpointAsync(string connectionString)
        {
            EndpointConfiguration endpointConfiguration = new EndpointConfiguration(Assembly.GetExecutingAssembly().GetName().Name);

            TransportExtensions<RabbitMQTransport> transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            transport.UseDirectRoutingTopology();
            transport.ConnectionString(connectionString);
            transport.Transactions(TransportTransactionMode.None);

            endpointConfiguration.EnableInstallers();
            SerializationExtensions<NewtonsoftSerializer> serialiser = endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Converters =
                    {
                        new StringEnumConverter()
                    },
                NullValueHandling = NullValueHandling.Ignore
            };
            serialiser.Settings(settings);

            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            endpointConfiguration.SendOnly();

            _endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
        }

        public async Task SendAsync(MessageBase message)
        {
            if(_enabled)
            {
                SendOptions sendOptions = new SendOptions();
                sendOptions.SetDestination(Assembly.GetExecutingAssembly().GetName().Name);
                await _endpoint.Send(message, sendOptions).ConfigureAwait(false);
            }
        }

        public static async Task<RabbitMqMessageTransport> CreateAsync(IConfiguration configuration)
        {
            IConfigurationSection configSection = configuration.GetSection("NServiceBus");
            bool enabled = configSection.GetValue("Enabled", false);
            string connectionString = enabled ? configSection.GetValue("ConnectionString", string.Empty) : string.Empty;
            return (await CreateAsync(enabled, connectionString));
        }

        public static async Task<RabbitMqMessageTransport> CreateAsync(
            bool enabled,
            string connectionString = "")
        {
            RabbitMqMessageTransport instance = new RabbitMqMessageTransport(enabled);
            if (enabled)
            {
                await instance.CreateEndpointAsync(connectionString);
            }
            return (instance);
        }

    }
}

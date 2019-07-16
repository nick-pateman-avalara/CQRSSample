using System;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace CQRSAPI.Messages
{

    public class MessageTransportFactory
    {

        private readonly IConfiguration _configuration;

        public MessageTransportFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<T> CreateAsync<T>() where T : IMessageTransport
        {
            IConfigurationSection configSection = _configuration.GetSection("NServiceBus");
            bool enabled = configSection.GetValue("Enabled", false);
            object parameter;
            if (typeof(T) == typeof(RabbitMqMessageTransport))
            {
                parameter = enabled ? configSection.GetValue("ConnectionString", string.Empty) : string.Empty;
            }
            else
            {
                throw new NotSupportedException($"Type '{typeof(T).Name}' is not supported by MessageTransportFactory.");
            }

            return (await CreateAsync<T>(enabled, parameter));
        }

        public static async Task<T> CreateAsync<T>(
            bool enabled,
            object parameter = null) where T: IMessageTransport
        {
            IMessageTransport instance;
            if (typeof(T) == typeof(RabbitMqMessageTransport))
            {
                RabbitMqMessageTransport rabbitInstance = new RabbitMqMessageTransport(enabled);
                await rabbitInstance.InitialiseAsync(parameter);
                instance = rabbitInstance;
            }
            else
            {
                throw new NotSupportedException($"Type '{typeof(T).Name}' is not supported by MessageTransportFactory.");
            }
            return ((T)instance);
        }

    }

}

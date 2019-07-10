using System.Threading.Tasks;
using CQRSAPI.Messages;
using NServiceBus;

namespace CQRSAPI.MessageHandlers
{

    public class PersonMessageHandler : IHandleMessages<PersonMessage>
    {

        public Task Handle(PersonMessage message, IMessageHandlerContext context)
        {
            System.Diagnostics.Debug.WriteLine($"Received message - {message.ToString()}");
            return (Task.CompletedTask);
        }

    }

}

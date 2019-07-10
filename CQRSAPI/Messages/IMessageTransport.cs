using System.Threading.Tasks;
using CQRSAPI.People.Messages;

namespace CQRSAPI.Messages
{

    public interface IMessageTransport
    {

        Task SendAsync(PersonEventMessage message);

    }

}

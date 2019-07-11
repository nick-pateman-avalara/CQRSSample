using System.Threading.Tasks;
using CQRSAPI.Features.People.Messages;

namespace CQRSAPI.Messages
{

    public interface IMessageTransport
    {

        Task SendAsync(MessageBase message);

    }

}

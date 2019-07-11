using System.Threading.Tasks;

namespace CQRSAPI.Messages
{

    public interface IMessageTransport
    {

        Task SendAsync(MessageBase message);

    }

}

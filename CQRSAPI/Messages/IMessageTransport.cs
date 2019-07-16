using System.Threading.Tasks;

namespace CQRSAPI.Messages
{

    public interface IMessageTransport
    {

        Task InitialiseAsync(object parameter);

        Task SendAsync(MessageBase message);

    }

}

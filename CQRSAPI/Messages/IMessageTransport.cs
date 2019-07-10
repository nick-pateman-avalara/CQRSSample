using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRSAPI.Models;

namespace CQRSAPI.Messages
{

    public interface IMessageTransport
    {

        Task SendLocalAsync(PersonMessage message);

    }

}

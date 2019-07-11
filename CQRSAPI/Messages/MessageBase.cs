using NServiceBus;
using System;

namespace CQRSAPI.Messages
{
    public class MessageBase : IMessage
    {

        public virtual string FromFeature { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    }
}

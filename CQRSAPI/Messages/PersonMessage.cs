using NServiceBus;

namespace CQRSAPI.Messages
{

    public class PersonMessage : IMessage
    {

        public enum Operation
        {
            None = 0,
            CreatedPerson = 1,
            UpdatedPerson = 2,
            DeletedPerson = 3
        }

        public Operation Op { get; set; }

        public int Id { get; set; }

        public override string ToString()
        {
            return ($"[Op = {Op.ToString()}, Id = {Id}]");
        }
    }

}

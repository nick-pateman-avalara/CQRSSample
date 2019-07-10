using NServiceBus;

namespace CQRSAPI.People.Messages
{

    public class PersonEventMessage : IMessage
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

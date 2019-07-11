using CQRSAPI.Features.PeopleV2.Feature;
using CQRSAPI.Messages;

namespace CQRSAPI.Features.PeopleV2.Messages
{

    public class PersonEventMessage : MessageBase
    {

        public new string FromFeature { get; set; } = new PeopleFeature().Name;

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

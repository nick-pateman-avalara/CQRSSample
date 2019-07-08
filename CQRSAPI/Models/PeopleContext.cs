using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CQRSAPI.Models
{

    public class PeopleContext : DbContext
    {

        public DbSet<Person> People { get; set; }

        public PeopleContext(DbContextOptions<PeopleContext> options) :
            base(options)
        { }

        public bool PersonExists(int id)
        {
            return (People.Any(e => e.PersonId == id));
        }

    }

}

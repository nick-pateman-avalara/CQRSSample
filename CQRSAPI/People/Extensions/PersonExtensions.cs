using System.Linq;
using System.Reflection;
using CQRSAPI.People.Models;

namespace CQRSAPI.People.Extensions
{

    public static class PersonExtensions
    {

        public static bool IsEqual(
            this Person person,
            Person compare,
            params string[] omit)
        {
            PropertyInfo[] properties = person.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo propertyInfo in properties)
            {
                if(omit == null || omit.Length == 0 || !omit.Contains(propertyInfo.Name))
                {
                    object value1 = propertyInfo.GetValue(person, null);
                    object value2 = propertyInfo.GetValue(compare, null);
                    if (!value1.Equals(value2)) return (false);
                }
            }
            return (true);
        }

    }

}

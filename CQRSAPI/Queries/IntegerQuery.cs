using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRSAPI.Queries
{

    public class IntegerQuery
    {

        public enum ComparisonOperator
        {
            None = 0,
            Equals = 1,
            LessThan = 2,
            GreaterThan = 3
        }

        public int Value { get; set; }

        public ComparisonOperator Operator { get; set; }

        public IntegerQuery(string query)
        {
            switch (query.Substring(0, 2).ToLower())
            {
                case "eq":
                {
                    Operator = ComparisonOperator.Equals;
                    break;
                }
                case "lt":
                {
                    Operator = ComparisonOperator.LessThan;
                    break;
                }
                case "gt":
                {
                    Operator = ComparisonOperator.GreaterThan;
                    break;
                }
            }

            string valueString = query.Substring(Operator != ComparisonOperator.None ? 2 : 0);
            if (int.TryParse(valueString, out var valueInt))
            {
                Value = valueInt;
                Operator = ComparisonOperator.Equals;
            }
        }

    }

}

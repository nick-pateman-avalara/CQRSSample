using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace CQRSAPI.Helpers
{

    public class QueryHelpers
    {

        public static List<KeyValuePair<string, string>> ExtractQueryParamsFromRequest(
            HttpRequest request,
            params string[] exclude)
        {
            string queryString = request.QueryString.ToString();
            if (!string.IsNullOrEmpty((queryString)))
            {
                List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();
                string[] valuePairs = request.QueryString.ToString().Substring(1).Split('&');
                foreach (string pair in valuePairs)
                {
                    string[] valueParts = pair.Split('=');
                    parameters.Add((new KeyValuePair<string, string>(valueParts[0], valueParts[1])));
                }
                return (parameters);
            }
            else
            {
                return (null);
            }
        }

        public static string Generate<T>(
            List<KeyValuePair<string, string>> queryParams,
            out Dictionary<string, object> outParams)
        {
            outParams = new Dictionary<string, object>();

            if (queryParams == null || queryParams.Count == 0) return (string.Empty);

            Dictionary<string, int> counters = new Dictionary<string, int>();

            Type inputType = typeof(T);
            StringBuilder stringBuilder = new StringBuilder("WHERE ");
            foreach (KeyValuePair<string, string> param in queryParams)
            {
                switch (param.Key.ToLower())
                {
                    case "op":
                    {
                        if (IsValidOp(param.Value))
                        {
                            stringBuilder.Append($"{param.Value.ToUpper()} ");
                        }

                        break;
                    }
                    default:
                    {
                        PropertyInfo propInfo = inputType.GetProperty(param.Key, BindingFlags.Instance | BindingFlags.Public);
                        if (propInfo != null)
                        {
                            string[] valueParts = param.Value.Split('.');
                            if (valueParts.Length == 2)
                            {
                                string op = string.Empty;
                                switch (valueParts[0])
                                {
                                    case "eq":
                                    {
                                        op = "=";
                                        break;
                                    }
                                    case "lt":
                                    {
                                        op = "<";
                                        break;
                                    }
                                    case "gt":
                                    {
                                        op = ">";
                                        break;
                                    }
                                    case "like":
                                    {
                                        op = "LIKE";
                                        break;
                                    }
                                }

                                int counter = IncrementCounter(
                                    propInfo.Name,
                                    counters);
                                outParams.Add($"{propInfo.Name}{counter}", $"{valueParts[1]}");
                                stringBuilder.Append($"{propInfo.Name} {op} @{propInfo.Name}{counter}");
                            }
                        }
                        break;
                    }
                }
                stringBuilder.Append(' ');
            }
            return (stringBuilder.ToString());
        }

        private static int IncrementCounter(
            string name,
            Dictionary<string, int> counters)
        {
            if (counters.ContainsKey(name))
            {
                int curValue = counters[name];
                counters.Remove(name);
                curValue += 1;
                counters.Add(name, curValue);
                return (curValue);
            }
            else
            {
                counters.Add(name, 1);
                return (1);
            }
        }

        private static  bool IsValidOp(string op)
        {
            return (op.ToLower() == "and" ||
                    op.ToLower() == "or");
        }

    }

}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace CQRSAPI.Helpers
{

    public class QueryHelpers
    {

        public static NameValueCollection GetQueryFromRequest(
            HttpRequest request,
            params string[] exclude)
        {
            NameValueCollection queryParameters = HttpUtility.ParseQueryString(request.QueryString.ToString());
            foreach (string exc in exclude)
            {
                queryParameters.Remove(exc);
            }
            return (queryParameters);
        }

        public static string Generate<T>(NameValueCollection query)
        {
            if (query.Count == 0) return (string.Empty);

            int processed = 0;
            Type inputType = typeof(T);
            StringBuilder stringBuilder = new StringBuilder("WHERE ");
            foreach (string property in query.Keys)
            {
                PropertyInfo propInfo = inputType.GetProperty(property, BindingFlags.Instance | BindingFlags.Public);
                if (propInfo != null)
                {
                    if (processed > 0)
                    {
                        //Can we get any control over this from a query string?
                        stringBuilder.Append("AND ");
                    }

                    bool needQuotes = propInfo.PropertyType == typeof(string);
                    string value = query[property];
                    string[] valueParts = value.Split('.');
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

                        stringBuilder.Append(needQuotes
                            ? $"{propInfo.Name} {op} '{valueParts[1]}'"
                            : $"{propInfo.Name} {op} {valueParts[1]}");
                    }
                    else
                    {
                        stringBuilder.Append(needQuotes
                            ? $"{propInfo.Name} = '{valueParts[0]}'"
                            : $"{propInfo.Name} = {valueParts[0]}");
                    }

                    processed += 1;
                }
            }
            stringBuilder.Append(' ');
            return (stringBuilder.ToString());
        }

    }

}

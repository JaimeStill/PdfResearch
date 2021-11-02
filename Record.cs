using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PdfResearch
{
    public class Record
    {
        public string Name { get; set; }

        public IEnumerable<RecordProp> Properties { get; set; }

        public static Record Generate<T>(T data, string name = "ssn") => new Record
        {
            Name = name,
            Properties = GenerateProps(data)
        };

        static IEnumerable<RecordProp> GenerateProps<T>(T data)
        {
            var type = data.GetType();
            var properties = type.GetProperties();
            var props = new List<RecordProp>();

            return properties.Select(prop => new RecordProp
            {
                Key = prop.Name,
                Map = $"{type.Name}.{prop.Name}",
                Value = GetStringValue(prop, data)
            });
        }

        static string GetStringValue<T>(PropertyInfo prop, T data)
        {
            switch (prop.PropertyType.Name)
            {
                case "DateTime":
                    return ((DateTime)prop.GetValue(data)).ToString("MM-dd-yyyy");
                case "Boolean":
                    return (bool)prop.GetValue(data) ? "Yes" : "No";
                default:
                    return prop.GetValue(data).ToString();
            }
        }
    }
}
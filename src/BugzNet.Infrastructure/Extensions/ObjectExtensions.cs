using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BugzNet.Infrastructure.Extensions
{
    public static class ObjectExtensions
    {
        public static string SerializeToDepth(this object obj, int maxDepth)
        {
            using (var strWriter = new StringWriter())
            {
                using (var jsonWriter = new MaxDepthJsonTextWriter(strWriter))
                {
                    Func<bool> include = () => jsonWriter.CurrentDepth <= maxDepth;
                    var resolver = new ConditionalContractResolver(include);
                    var serializer = new JsonSerializer { ContractResolver = resolver };
                    serializer.Serialize(jsonWriter, obj);
                }
                return strWriter.ToString();
            }
        }

        public static string SerializeWithUpperCaseValues(this object obj)
        {
            using (TextWriter textWriter = new StringWriter())
            using (JsonWriter jsonWriter = new UpperCaseValueJsonWriter(textWriter))
            {
                JsonSerializer ser = new JsonSerializer();
                ser.Serialize(jsonWriter, obj);

                return textWriter.ToString();
            }
        }

        public class UpperCaseValueJsonWriter : JsonTextWriter
        {
            public UpperCaseValueJsonWriter(TextWriter textWriter)
                : base(textWriter)
            {
            }

            public override void WriteValue(string value)
            {
                if (value != null)
                {
                    value = value.ToUpper();
                }

                base.WriteValue(value);
            }
        }

        private class MaxDepthJsonTextWriter : JsonTextWriter
        {
            public MaxDepthJsonTextWriter(TextWriter textWriter) : base(textWriter) { }

            public int CurrentDepth { get; private set; }

            public override void WriteStartObject()
            {
                CurrentDepth++;
                base.WriteStartObject();
            }

            public override void WriteEndObject()
            {
                CurrentDepth--;
                base.WriteEndObject();
            }
        }

        private class ConditionalContractResolver : DefaultContractResolver
        {
            private readonly Func<bool> _includeProperty;

            public ConditionalContractResolver(Func<bool> includeProperty)
            {
                _includeProperty = includeProperty;
            }

            protected override JsonProperty CreateProperty(
                MemberInfo member, MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);
                var shouldSerialize = property.ShouldSerialize;
                property.ShouldSerialize = obj => _includeProperty() &&
                                                  (shouldSerialize == null ||
                                                   shouldSerialize(obj));
                return property;
            }
        }
    }
}

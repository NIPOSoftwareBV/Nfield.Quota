using System;
using Newtonsoft.Json;

namespace Nfield.Quota.Persistence
{
    /// <summary>
    ///     JSON converter for <see cref="Guid"/>.
    /// </summary>
    class GuidJsonConverter : JsonConverter
    {
        /// <summary>
        ///     Gets a value indicating whether this <see cref="Newtonsoft.Json.JsonConverter"/> can read JSON.
        /// </summary>
        /// <value><see langword="true"/> if this <see cref="Newtonsoft.Json.JsonConverter"/> can read JSON; otherwise, <see langword="false"/>.
        /// </value>
        public override bool CanRead { get { return true; } }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Newtonsoft.Json.JsonConverter"/> can write JSON.
        /// </summary>
        /// <value><see langword="true"/> if this <see cref="Newtonsoft.Json.JsonConverter"/> can write JSON; otherwise, <see langword="false"/>.
        /// </value>
        public override bool CanWrite { get { return true; } }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">
        /// Kind of the object.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this instance can convert the specified object type; otherwise,
        /// .
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(Guid)) || objectType.IsAssignableFrom(typeof(Guid?));
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="Newtonsoft.Json.JsonWriter"/> to write to.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="serializer">
        /// The calling serializer.
        /// </param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteValue(default(string));
            }
            else if (value is Guid)
            {
                var guid = (Guid)value;
                writer.WriteValue(guid.ToString("N"));
            }
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">
        /// The <see cref="Newtonsoft.Json.JsonReader"/> to read from.
        /// </param>
        /// <param name="objectType">
        /// Kind of the object.
        /// </param>
        /// <param name="existingValue">
        /// The existing value of object being read.
        /// </param>
        /// <param name="serializer">
        /// The calling serializer.
        /// </param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            var str = reader.Value as string;
            return str != null ? Guid.Parse(str) : default(Guid);
        }
    }
}

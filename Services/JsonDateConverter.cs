using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TimeSnapBackend_MySql.Services
{
    public class JsonDateConverter : JsonConverter<DateTime>
    {
        private readonly string[] dateFormats = { "yyyy-MM-dd", "MM/dd/yyyy", "MM-dd-yyyy" };

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (DateTime.TryParseExact(reader.GetString(), dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date;
            }
            throw new JsonException("Invalid date format.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
        }
    
}
}

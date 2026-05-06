using Newtonsoft.Json;
using System;
using System.Globalization;

namespace ReFilter.Converters
{
    public class TimeOnlyNullableConverter : JsonConverter<TimeOnly?>
    {
        private const string TimeFormat = "HH:mm:ss.FFFFFFF";

        public override TimeOnly? ReadJson(JsonReader reader, Type objectType, TimeOnly? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Handle null JSON
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonToken.String)
            {
                var str = (string)reader.Value!;
                if (string.IsNullOrEmpty(str))
                {
                    return null;
                }

                // Parse exact with invariant culture
                return TimeOnly.ParseExact(str, TimeFormat, CultureInfo.InvariantCulture);
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, TimeOnly? value, JsonSerializer serializer)
        {
            if (value.HasValue)
            {
                writer.WriteValue(value.Value.ToString(TimeFormat, CultureInfo.InvariantCulture));
            }
            else
            {
                writer.WriteNull();
            }
        }
    }
}

﻿using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ReFilter.Converters
{
    public class DateOnlyConverter : JsonConverter<DateOnly>
    {
        private const string DateFormat = "yyyy-MM-dd";

        public override DateOnly ReadJson(JsonReader reader, Type objectType, DateOnly existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value.GetType() == typeof(string))
            {
                Regex dateOnlyRegex = new(@"\d{4}-\d{1,2}-\d{1,2}");
                Match match = dateOnlyRegex.Match((string)reader.Value);

                if (match.Success && match.Groups.Count == 1)
                {
                    return DateOnly.ParseExact(match.Groups[0].Value, DateFormat, CultureInfo.InvariantCulture);
                }
            }

            if (reader.Value.GetType() == typeof(DateTime))
            {
                return DateOnly.FromDateTime((DateTime)reader.Value);
            }

            return DateOnly.ParseExact((string)reader.Value, DateFormat, CultureInfo.InvariantCulture);
        }

        public override void WriteJson(JsonWriter writer, DateOnly value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
        }
    }
}

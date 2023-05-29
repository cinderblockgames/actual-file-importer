using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

// https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to?pivots=dotnet-7-0

namespace AFI.Wrapper;

public class JsonDateOnlyAttribute : JsonConverterAttribute
{
   public JsonDateOnlyAttribute()
      : base(typeof(JsonDateOnly))
   {
   }
}

public class JsonDateOnly : JsonConverter<DateTime>
{
   public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
      DateTime.ParseExact(reader.GetString()!, "yyyy-MM-dd", CultureInfo.InvariantCulture);

   public override void Write(Utf8JsonWriter writer, DateTime dateTimeValue, JsonSerializerOptions options) =>
      writer.WriteStringValue(dateTimeValue.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
}
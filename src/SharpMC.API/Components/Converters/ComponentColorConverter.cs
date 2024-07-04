using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharpMC.API.Components.Converters;

public class ComponentColorConverter : JsonConverter<IComponentColor>
{
    public override IComponentColor? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, IComponentColor value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Color);
    }
}
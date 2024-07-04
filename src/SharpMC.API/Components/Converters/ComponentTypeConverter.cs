using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using SharpMC.API.Components.Enums;

namespace SharpMC.API.Components.Converters;

public class ComponentTypeConverter : JsonConverter<ComponentType>
{
    public override ComponentType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException("Deserializing ComponentType is not supported.");
    }

    public override void Write(Utf8JsonWriter writer, ComponentType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString().ToLower());
    }
}
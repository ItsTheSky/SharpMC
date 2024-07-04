using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using SharpMC.API.Components.Enums;
using SharpMC.API.Components.Types;

namespace SharpMC.API.Components.Converters;

public class ComponentConverter : JsonConverter<Component>
{
    public override Component? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException("Deserializing Component is not supported.");
    }

    public override void Write(Utf8JsonWriter writer, Component component, JsonSerializerOptions options)
    {
        var typeClass = component.ComponentType switch
        {
            ComponentType.Text => typeof(TextComponent),
            ComponentType.Translatable => null,
            ComponentType.Score => null,
            ComponentType.Selector => null,
            ComponentType.Keybind => null,
            ComponentType.Nbt => null,
            _ => throw new ArgumentException($"Unknown component type: {component.ComponentType}", nameof(component))
        };
        
        if (typeClass == null)
            throw new ArgumentException($"Cannot serialize component type: {component.ComponentType}", nameof(component));
        
        // Component pre-serialization
        if (component.Children != null! && component.Children.Count == 0)
            component.Children = null!;
        
        // copy options
        var newOptions = new JsonSerializerOptions(options)
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        JsonSerializer.Serialize(writer, component, typeClass, newOptions);
    }
}
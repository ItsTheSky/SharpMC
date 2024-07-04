using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SharpMC.API.Components.Enums;

namespace SharpMC.API.Components.Types;

/// <summary>
/// Text (= literal text) component.
/// </summary>
public class TextComponent : Component
{
    
    [JsonPropertyName("text")]
    public string Text { get; set; }
    public TextComponent(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Given text cannot be null or empty", nameof(text));
        
        Text = text;
    }

    [JsonIgnore]
    public override ComponentType ComponentType => ComponentType.Text;
    public override List<Component> Children { get; set; } = new();
}
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SharpMC.API.Components.Converters;
using SharpMC.API.Components.Enums;

namespace SharpMC.API.Components;

/// <summary>
/// Represent a (display) component.
/// </summary>
[Serializable, JsonConverter(typeof(ComponentConverter))]
public abstract class Component
{
    
    [JsonPropertyName("type"), JsonConverter(typeof(ComponentTypeConverter))]
    public abstract ComponentType ComponentType { get; }
    
    [JsonPropertyName("extra"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public abstract List<Component> Children { get; set; }
    
    [JsonPropertyName("clickEvent")]
    public IComponentClickEvent? ClickEvent { get; set; }

    #region Styles

    [JsonPropertyName("bold")]
    public bool Bold { get; set; } = false;
    [JsonPropertyName("italic")]
    public bool Italic { get; set; } = false;
    [JsonPropertyName("underlined")]
    public bool Underlined { get; set; } = false;
    [JsonPropertyName("strikethrough")]
    public bool Strikethrough { get; set; } = false;
    [JsonPropertyName("obfuscated")]
    public bool Obfuscated { get; set; } = false;
    
    [JsonPropertyName("color"), JsonConverter(typeof(ComponentColorConverter))]
    public IComponentColor? Color { get; set; }
    [JsonPropertyName("font")]
    public string? Font { get; set; }
    [JsonPropertyName("insertion")]
    public string? Insertion { get; set; }

    #endregion
}
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using SharpMC.API.Components.Enums;
using SharpMC.API.Components.Types;

namespace SharpMC.API.Components;

public static class ChatHelper
{
    public static string EncodeChatMessage(string message)
    {
        return EncodeComponent(new TextComponent(message));
    }

    public static string EncodeComponent(Component component)
    {
        return JsonSerializer.Serialize(component);
    }
        
    public static Component Combine(params Component[] components)
    {
        if (components.Length == 0)
            throw new System.ArgumentException("Cannot combine zero components", nameof(components));
        if (components.Length == 1)
            return components[0];
        
        var first = components[0];
        var children = new List<Component>();
        for (var i = 1; i < components.Length; i++)
            children.Add(components[i]);
        first.Children = children;
        
        return first;
    }
    
    public static Component Join(this Component separator, params Component[] components)
    {
        if (components.Length == 0)
            throw new System.ArgumentException("Cannot join zero components", nameof(components));
        if (components.Length == 1)
            return components[0];
        
        var mainComponent = components[0];
        for (var i = 1; i < components.Length; i++)
        {
            mainComponent.Children.Add(separator);
            mainComponent.Children.Add(components[i]);
        }
        
        return mainComponent;
    }
    
    public static Component Append(this Component component, Component child)
    {
        component.Children.Add(child);
        return component;
    }
}
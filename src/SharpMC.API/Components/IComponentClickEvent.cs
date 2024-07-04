using System;
using SharpMC.API.Components.Enums;

namespace SharpMC.API.Components;

public interface IComponentClickEvent
{

    #region Creators

    public static IComponentClickEvent CreateOpenUrl(string url)
    {
        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            throw new ArgumentException("URL is not well formed.", nameof(url));
        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            throw new ArgumentException("URL is not http or https.", nameof(url));
        
        return new ComponentClickEvent(ComponentClickActionType.OpenUrl, url);
    }
    
    public static IComponentClickEvent CreateRunCommand(string command)
    {
        return new ComponentClickEvent(ComponentClickActionType.RunCommand, command);
    }
    
    public static IComponentClickEvent CreateSuggestCommand(string command)
    {
        return new ComponentClickEvent(ComponentClickActionType.SuggestCommand, command);
    }
    
    public static IComponentClickEvent CreateChangePage(int page)
    {
        if (page < 0)
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than or equal to 0.");
        
        return new ComponentClickEvent(ComponentClickActionType.ChangePage, page + "");
    }
    
    public static IComponentClickEvent CreateCopyToClipboard(string text)
    {
        return new ComponentClickEvent(ComponentClickActionType.CopyToClipboard, text);
    }

    #endregion
    
    public ComponentClickActionType Action { get; }
    public string Value { get; }
 
    private class ComponentClickEvent : IComponentClickEvent
    {
        public ComponentClickActionType Action { get; }
        public string Value { get; }

        internal ComponentClickEvent(ComponentClickActionType action, string value)
        {
            Action = action;
            Value = value;
        }
    }
}
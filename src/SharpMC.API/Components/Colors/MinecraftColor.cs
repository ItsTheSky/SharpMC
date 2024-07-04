using System;
using SharpMC.API.Enums;

namespace SharpMC.API.Components.Colors;

public record MinecraftColor(string Name, char Code) : IComponentColor
{ 

    public static MinecraftColor
        Black = new("black", '0'),
        DarkBlue = new("dark_blue", '1'),
        DarkGreen = new("dark_green", '2'),
        DarkAqua = new("dark_aqua", '3'),
        DarkRed = new("dark_red", '4'),
        DarkPurple = new("dark_purple", '5'),
        Gold = new("gold", '6'),
        Gray = new("gray", '7'),
        DarkGray = new("dark_gray", '8'),
        Blue = new("blue", '9'),
        Green = new("green", 'a'),
        Aqua = new("aqua", 'b'),
        Red = new("red", 'c'),
        Pink = new("light_purple", 'd'),
        Yellow = new("yellow", 'e'),
        White = new("white", 'f');

    public string Color => Name;

    public static MinecraftColor FromChatColor(ChatColor color)
    {
        if (color == ChatColor.Black) return Black;
        if (color == ChatColor.DarkBlue) return DarkBlue;
        if (color == ChatColor.DarkGreen) return DarkGreen;
        if (color == ChatColor.DarkAqua) return DarkAqua;
        if (color == ChatColor.DarkRed) return DarkRed;
        if (color == ChatColor.DarkPurple) return DarkPurple;
        if (color == ChatColor.Gold) return Gold;
        if (color == ChatColor.Gray) return Gray;
        if (color == ChatColor.DarkGray) return DarkGray;
        // if (color == ChatColor.Blue) return Blue; todo: add Blue
        if (color == ChatColor.Green) return Green;
        if (color == ChatColor.Aqua) return Aqua;
        if (color == ChatColor.Red) return Red;
        if (color == ChatColor.Pink) return Pink;
        if (color == ChatColor.Yellow) return Yellow;
        if (color == ChatColor.White) return White;
        throw new ArgumentOutOfRangeException(nameof(color), color, null);
    }
}
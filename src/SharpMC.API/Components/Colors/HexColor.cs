using System.Drawing;

namespace SharpMC.API.Components.Colors;

public record HexColor(Color SysColor) : IComponentColor
{
    public string Color => $"#{SysColor.R:X2}{SysColor.G:X2}{SysColor.B:X2}";
}
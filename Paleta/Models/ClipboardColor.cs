using Paleta.Converters;
using System;
using Windows.UI;

namespace Paleta.Models
{
    public class ClipboardColor
    {
        public String Name { get; set; }
        public String HEX { get; set; }
        public Color Color { get; set; }

        public ClipboardColor(String name, Color color)
        {
            this.Name = name;
            this.Color = color;
            this.HEX = ColorConverters.ColorToHex(color);
        }
    }
}
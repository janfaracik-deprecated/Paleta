using Palette.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Palette.Converters
{

    class ColorConverters
    {

        public static Color HexToRGB(string HexCode, Boolean IsTransparent = false)
        {
            HexCode = HexCode.Replace("#", string.Empty);

            byte r = (byte)(Convert.ToUInt32(HexCode.Substring(0, 2), 16));
            byte g = (byte)(Convert.ToUInt32(HexCode.Substring(2, 2), 16));
            byte b = (byte)(Convert.ToUInt32(HexCode.Substring(4, 2), 16));

            if (IsTransparent)
            {
                Color myBrush = new Color { R = r, G = g, B = b };
                return myBrush;
            }
            else
            {
                Color myBrush = new Color { R = r, G = g, B = b, A = 255 };
                return myBrush;
            }
        }

        public static Color HSLToRGB(double h, double sl, double l)
        {
            double v;
            double r, g, b;

            r = l;   // default to gray
            g = l;
            b = l;
            v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);
            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0;
                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }

            Color rgb = new Color()
            {
                A = 255,
                R = Convert.ToByte(r * 255.0f),
                G = Convert.ToByte(g * 255.0f),
                B = Convert.ToByte(b * 255.0f)
            };

            return rgb;
        }

        public static void RGBToHSL(Color color, out double h, out double s, out double l)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;
            double v;
            double m;
            double vm;
            double r2, g2, b2;

            h = 0; // default to black
            s = 0;
            l = 0;
            v = Math.Max(r, g);
            v = Math.Max(v, b);
            m = Math.Min(r, g);
            m = Math.Min(m, b);
            l = (m + v) / 2.0;
            if (l <= 0.0)
            {
                return;
            }
            vm = v - m;
            s = vm;
            if (s > 0.0)
            {
                s /= (l <= 0.5) ? (v + m) : (2.0 - v - m);
            }
            else
            {
                return;
            }
            r2 = (v - r) / vm;
            g2 = (v - g) / vm;
            b2 = (v - b) / vm;
            if (r == v)
            {
                h = (g == m ? 5.0 + b2 : 1.0 - g2);
            }
            else if (g == v)
            {
                h = (b == m ? 1.0 + r2 : 3.0 - b2);
            }
            else
            {
                h = (r == m ? 3.0 + g2 : 5.0 - r2);
            }
            h /= 6.0;
        }

        public static void RGBToHSL(double r, double g, double b, out double h, out double s, out double l)
        {
            r /= 255.0;
            g /= 255.0;
            b /= 255.0;
            double v;
            double m;
            double vm;
            double r2, g2, b2;

            h = 0; // default to black
            s = 0;
            l = 0;
            v = Math.Max(r, g);
            v = Math.Max(v, b);
            m = Math.Min(r, g);
            m = Math.Min(m, b);
            l = (m + v) / 2.0;
            if (l <= 0.0)
            {
                return;
            }
            vm = v - m;
            s = vm;
            if (s > 0.0)
            {
                s /= (l <= 0.5) ? (v + m) : (2.0 - v - m);
            }
            else
            {
                return;
            }
            r2 = (v - r) / vm;
            g2 = (v - g) / vm;
            b2 = (v - b) / vm;
            if (r == v)
            {
                h = (g == m ? 5.0 + b2 : 1.0 - g2);
            }
            else if (g == v)
            {
                h = (b == m ? 1.0 + r2 : 3.0 - b2);
            }
            else
            {
                h = (r == m ? 3.0 + g2 : 5.0 - r2);
            }
            h /= 6.0;
        }

        public static String ColorToHex(Color color)
        {
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        public static String ColorToRGB(Color color)
        {
            return "rgb(" + color.R + ", " + color.G + ", " + color.B + ");";
        }

        public static String ColorToHSL(Color color)
        {
            RGBToHSL(color, out double h, out double s, out double l);
            return "hsl(" + h + ", " + s + ", " + l + ");";
        }

        public static String ColorToARGB(Color color)
        {
            return "argb(255, " + color.R + ", " + color.G + ", " + color.B + ");";
        }

        public static String ColorToRGBA(Color color)
        {
            return "rgba(" + color.R + ", " + color.G + ", " + color.B + ", 255);";
        }

        public static String ColorToSwift(Color color)
        {
            double newR = color.R / 255;
            double newG = color.G / 255;
            double newB = color.B / 255;
            return "UIColor(red:" + newR + ", green:" + newG + ", blue:" + newB + ", alpha:1)";
        }

        public static String ColorToObjectiveC(Color color)
        {
            double newR = color.R / 255;
            double newG = color.G / 255;
            double newB = color.B / 255;
            return "[UIColor colorWithRed:" + newR + " green:" + newG + " blue:" + newB + " alpha:1];";
        }

        public static String ColorToXamarin(Color color)
        {
            double newR = color.R / 255;
            double newG = color.G / 255;
            double newB = color.B / 255;
            return "new UIColor(red:" + newR + "f, green:" + newG + "f, blue:" + newB + "f, alpha:1)";
        }

        public static String ColorToWindowsUI(Color color)
        {
            return "Color.FromArgb(255, " + color.R + ", " + color.G + ", " + color.B + ");";
        }

        // Gradients

        public static String ColorsToLinearGradient(ColorCollectionItem colorCollectionItem)
        {
            List<ColorItem> temp = colorCollectionItem.Colors.OrderBy(x => x.Offset).ToList();
            double rotation = colorCollectionItem.Rotation;
            String colors = "";
            foreach (ColorItem colorItem in temp)
            {
                if (colors != "")
                {
                    colors += ", ";
                }
                colors += ColorToHex(colorItem.Color) + " " + Math.Floor(colorItem.Offset * 100) + "%";
            }
            return "linear-gradient(" + rotation + "deg, " + colors + ");";
        }

        public static String ColorsToRadialGradient(ColorCollectionItem colorCollectionItem)
        {
            List<ColorItem> temp = colorCollectionItem.Colors.OrderBy(x => x.Offset).ToList();
            double rotation = colorCollectionItem.Rotation;
            String colors = "";
            foreach (ColorItem colorItem in temp)
            {
                if (colors != "")
                {
                    colors += ", ";
                }
                colors += ColorToHex(colorItem.Color) + " " + Math.Floor(colorItem.Offset * 100) + "%";
            }
            return "radial-gradient(" + rotation + "deg, " + colors + ");";
        }

    }

    public class ConvertToColor
    {

        private static void AssignIfNull<T>(ref T target, T value)
        {
            if (target == null) target = value;
        }

        public static Color? GenerateColorFromString(String value)
        {
            Color? color = null;
            AssignIfNull(ref color, HexToColor(value));
            AssignIfNull(ref color, ShortenedHexToColor(value));
            AssignIfNull(ref color, RGBToColor(value));
            AssignIfNull(ref color, ARGBToColor(value));
            AssignIfNull(ref color, RGBAToColor(value));
            return color;
        }

        public static Color? HexToColor(String hex)
        {
            try
            {
                hex = hex.Replace("#", string.Empty);
                byte r = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
                byte g = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
                byte b = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
                return new Color { R = r, G = g, B = b, A = 255 };
            }
            catch
            {
                return null;
            }
        }

        public static Color? ShortenedHexToColor(String hex)
        {
            try
            {
                byte r = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
                byte g = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
                byte b = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
                return new Color { R = r, G = g, B = b, A = 255 };
            }
            catch
            {
                return null;
            }
        }

        public static Color? RGBToColor(String rgb)
        {
            rgb = rgb.ToLower().Replace(" ", "");
            Regex regex = new Regex("rgb *\\( *([0-9]+), *([0-9]+), *([0-9]+) *\\)");
            Match match = regex.Match(rgb);
            if (match.Success)
            {
                return new Color { R = Convert.ToByte(match.Groups[1].Value), G = Convert.ToByte(match.Groups[2].Value), B = Convert.ToByte(match.Groups[3].Value), A = 255 };
            }
            else
            {
                return null;
            }
        }

        public static Color? ARGBToColor(String argb)
        {
            argb = argb.ToLower().Replace(" ", "");
            Regex regex = new Regex("argb *\\( *([0-9]+), *([0-9]+), *([0-9]+), *([0-9]+) *\\)");
            Match match = regex.Match(argb);
            if (match.Success)
            {
                return new Color { R = Convert.ToByte(match.Groups[2].Value), G = Convert.ToByte(match.Groups[3].Value), B = Convert.ToByte(match.Groups[4].Value), A = 255 };
            }
            else
            {
                return null;
            }
        }

        public static Color? RGBAToColor(String rgba)
        {
            rgba = rgba.ToLower().Replace(" ", "");
            Regex regex = new Regex("rgba *\\( *([0-9]+), *([0-9]+), *([0-9]+), *([0-9]+) *\\)");
            Match match = regex.Match(rgba);
            if (match.Success)
            {
                return new Color { R = Convert.ToByte(match.Groups[1].Value), G = Convert.ToByte(match.Groups[2].Value), B = Convert.ToByte(match.Groups[3].Value), A = 255 };
            }
            else
            {
                return null;
            }
        }

    }

    internal sealed class HueToBrushConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {

            double h = (double)value / 360.0f;
            double sl = 0.5;
            double l = 0.5;

            double v;
            double r, g, b;

            r = l;   // default to gray
            g = l;
            b = l;
            v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);
            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0;
                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }

            Color rgb = new Color()
            {
                A = 255,
                R = System.Convert.ToByte(r * 255.0f),
                G = System.Convert.ToByte(g * 255.0f),
                B = System.Convert.ToByte(b * 255.0f)
            };

            return new SolidColorBrush(rgb);

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class SaturationToBrushConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return 1 - (double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
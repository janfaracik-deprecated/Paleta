using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Paleta.Converters
{
    public class Clip
    {
        public static bool GetToBounds(DependencyObject depObj)
        {
            return (bool)depObj.GetValue(ToBoundsProperty);
        }

        public static void SetToBounds(DependencyObject depObj, bool clipToBounds)
        {
            depObj.SetValue(ToBoundsProperty, clipToBounds);
        }

        /// <summary>
        /// Identifies the ToBounds Dependency Property.
        /// <summary>
        public static readonly DependencyProperty ToBoundsProperty =
            DependencyProperty.RegisterAttached("ToBounds", typeof(bool),
            typeof(Clip), new PropertyMetadata(false, OnToBoundsPropertyChanged));

        private static void OnToBoundsPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = d as FrameworkElement;
            if (fe != null)
            {
                ClipToBounds(fe);

                // whenever the element which this property is attached to is loaded
                // or re-sizes, we need to update its clipping geometry
                fe.Loaded += new RoutedEventHandler(fe_Loaded);
                fe.SizeChanged += new SizeChangedEventHandler(fe_SizeChanged);
            }
        }

        /// <summary>
        /// Creates a rectangular clipping geometry which matches the geometry of the
        /// passed element
        /// </summary>
        private static void ClipToBounds(FrameworkElement fe)
        {
            if (GetToBounds(fe))
            {
                fe.Clip = new RectangleGeometry()
                {
                    Rect = new Rect(0, 0, fe.ActualWidth, fe.ActualHeight)
                };
            }
            else
            {
                fe.Clip = null;
            }
        }

        static void fe_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ClipToBounds(sender as FrameworkElement);
        }

        static void fe_Loaded(object sender, RoutedEventArgs e)
        {
            ClipToBounds(sender as FrameworkElement);
        }
    }

    class Converters
    {

        public static SolidColorBrush BrushConverter(string HexCode, Boolean IsTransparent = false)
        {
            try
            {
                HexCode = HexCode.Replace("#", string.Empty);

                byte r = (byte)(Convert.ToUInt32(HexCode.Substring(0, 2), 16));
                byte g = (byte)(Convert.ToUInt32(HexCode.Substring(2, 2), 16));
                byte b = (byte)(Convert.ToUInt32(HexCode.Substring(4, 2), 16));

                if (IsTransparent)
                {
                    SolidColorBrush myBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(0, r, g, b));
                    return myBrush;
                }
                else
                {
                    SolidColorBrush myBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, r, g, b));
                    return myBrush;
                }
            }
            catch
            {
                return null;
            }
        }

        public static String BrushConverterToHex(Windows.UI.Color myColor)
        {

            return "#" + myColor.R.ToString("X2") + myColor.G.ToString("X2") + myColor.B.ToString("X2");

        }

    }

    // Color Converters

    internal sealed class ColorToRGBConverter : IValueConverter
    {

        Color currentColor;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            currentColor = (Color)value;

            switch (System.Convert.ToChar(parameter))
            {
                case 'R':
                    return System.Convert.ToDouble(currentColor.R);
                case 'G':
                    return System.Convert.ToDouble(currentColor.G);
                case 'B':
                    return System.Convert.ToDouble(currentColor.B);
                default:
                    return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            byte passedInt = System.Convert.ToByte(value);
            switch (System.Convert.ToChar(parameter))
            {
                case 'R':
                    return new Color { R = passedInt, G = currentColor.G, B = currentColor.B };
                case 'G':
                    return new Color { R = currentColor.R, G = passedInt, B = currentColor.B };
                case 'B':
                    return new Color { R = currentColor.R, G = currentColor.G, B = passedInt };
                default:
                    return new Color { R = currentColor.R, G = currentColor.G, B = currentColor.B };
            }
        }
    }

    internal sealed class ColorToRGBStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Color currentColor = (Color)value;
            return ColorConverters.ColorToRGB(currentColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class ArrayToHSLConverter : IValueConverter
    {
        int[] currentArray;
        double[] currentHSL = { 0, 0, 0 };

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            currentArray = (int[])value;

            ColorConverters.RGBToHSL(currentArray[0], currentArray[1], currentArray[2], out double _h, out double _s, out double _l);

            //  currentHSL = new double[] { _h, _s, _l };

            switch (System.Convert.ToChar(parameter))
            {
                case 'H':
                    //h *= 360.0f;
                    // _h = Math.Round(_h, 0);
                    return _h;
                case 'S':
                    //  _s = Math.Round(_s, 2);
                    return _s;
                case 'L':
                    //  _l = Math.Round(_l, 2);
                    return _l;
                default:
                    return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            double currentValue = System.Convert.ToDouble(value);

            Debug.WriteLine(currentArray[0]);
            Debug.WriteLine(currentArray[1]);
            Debug.WriteLine(currentArray[2]);

            ColorConverters.RGBToHSL(currentArray[0], currentArray[1], currentArray[2], out double _h, out double _s, out double _l);

            switch (System.Convert.ToChar(parameter))
            {
                case 'H':
                    _h = currentValue;
                    break;
                case 'S':
                    _s = currentValue;
                    break;
                case 'L':
                    _l = currentValue;
                    break;
            }

            Debug.WriteLine("val " + parameter);
            Debug.WriteLine("H " + _h);
            Debug.WriteLine("S " + _s);
            Debug.WriteLine("L " + _l);

            Color color = ColorConverters.HSLToRGB(_h, _s, _l);

            return new int[] { color.R, color.G, color.B };
        }
    }

    internal sealed class DoubleToClampedDegreesString : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double returnedObject = (double)value;
            return ((int)Math.Floor(returnedObject)).ToString() + "°";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class ColorToHexConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Color color = (Color)value;
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            string HexCode = value.ToString();
            try
            {
                HexCode = HexCode.Replace("#", string.Empty);

                byte r = (byte)(System.Convert.ToUInt32(HexCode.Substring(0, 2), 16));
                byte g = (byte)(System.Convert.ToUInt32(HexCode.Substring(2, 2), 16));
                byte b = (byte)(System.Convert.ToUInt32(HexCode.Substring(4, 2), 16));
                return Color.FromArgb(255, r, g, b);
            }
            catch
            {
                return Color.FromArgb(255, 0, 0, 0);
            }
        }
    }

    // Other

    internal sealed class HexToEnabledConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {

            String returnedValue = (String)value;

            //Debug.WriteLine("'" + returnedValue + "'");

            if (returnedValue == "")
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class IntToGradientTrackbarConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int returnedValue = (int)value;

            if (returnedValue == 1)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class IntToVisibilityConverter : IValueConverter
    {

        public bool IsInverted { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {

            int returnedValue = (int)value;

            if (returnedValue == 0)
            {
                if (this.IsInverted)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
            else
            {
                if (this.IsInverted)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class SelectedItemToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int returnedValue = (int)value;
            return (returnedValue != -1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class AngleToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int returnedValue = (int)Math.Floor((double)value);
            int param = System.Convert.ToInt16(parameter);

            int startRange = param - 45;
            int endRange = param + 45;

            if (param == 0)
            {
                startRange = 314;
                if (returnedValue >= startRange)
                {
                    double difference = Math.Abs(returnedValue - 360);
                    return 1 - (difference / 100);
                }
                if (returnedValue <= endRange)
                {
                    double difference = Math.Abs(returnedValue - param);
                    return 1 - (difference / 100);
                }
            }

            if (returnedValue >= startRange && returnedValue <= endRange)
            {
                double difference = Math.Abs(returnedValue - param);
                return 1 - (difference / 100);
            }

            return 0.55;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class TextToVisibilityConverter : IValueConverter
    {

        public bool IsInverted { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {

            string returnedValue = (string)value;

            if (IsInverted)
            {
                if (String.IsNullOrWhiteSpace(returnedValue))
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
            else
            {
                if (String.IsNullOrWhiteSpace(returnedValue))
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

    }

}
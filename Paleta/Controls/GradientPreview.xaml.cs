using Paleta.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Paleta.Controls
{

    internal sealed class IntToVisibilityConverter : IValueConverter
    {
        public bool IsInverted { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int returnedValue = (int)value;
            if (IsInverted)
            {
                return (returnedValue == 1) ? Visibility.Collapsed : Visibility.Visible;

            }
            else
            {
                return (returnedValue == 1) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class ObservableToGradientStopsCollection : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                ObservableCollection<ColorItem> colors = (ObservableCollection<ColorItem>)value;

                GradientStopCollection GradientItems = new GradientStopCollection();

                foreach (ColorItem colorItem in colors)
                {
                    GradientItems.Add(new GradientStop { Color = colorItem.Color, Offset = colorItem.Offset });
                }

                return GradientItems;
            }
            catch
            {
                Debug.WriteLine("GradientPreview had an error converting");
                GradientStopCollection GradientItems = new GradientStopCollection();
                GradientItems.Add(new GradientStop { Color = Colors.Red, Offset = 0 });
                GradientItems.Add(new GradientStop { Color = Colors.Blue, Offset = 1 });
                return GradientItems;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public sealed partial class GradientPreview : UserControl
    {

        #region Properties

        public bool IsRounded
        {
            get
            {
                return (bool)GetValue(isRoundedProperty);
            }
            set
            {
                SetValue(isRoundedProperty, value);
                UpdateCorners();
            }
        }
        public static readonly DependencyProperty isRoundedProperty = DependencyProperty.Register("IsRounded", typeof(bool), typeof(GradientPreview), null);

        #endregion

        public GradientPreview()
        {
            this.InitializeComponent();
            UpdateCorners();
        }

        private void UserControl_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            UpdateCorners();
        }

        public void UpdateCorners()
        {
            if (IsRounded)
            {
                rectangleGradient.RadiusX = ActualWidth / 2;
                rectangleGradient.RadiusY = ActualHeight / 2;
                rectangleSolid.RadiusX = ActualWidth / 2;
                rectangleSolid.RadiusY = ActualHeight / 2;
            }
            else
            {
                rectangleGradient.RadiusX = 0;
                rectangleGradient.RadiusY = 0;
                rectangleSolid.RadiusX = 0;
                rectangleSolid.RadiusY = 0;
            }
        }

    }
}

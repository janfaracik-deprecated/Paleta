using System.Diagnostics;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Shared.Helpers
{
    public class TitleBarHelper
    {

        //ColorTypes for class

        enum ColorTypes { Custom, Automatic };
        ColorTypes ColorType = ColorTypes.Automatic;

        //Track theme changes through this object

        FrameworkElement frameworkElement;

        //Gets Current TitleBar

        ApplicationViewTitleBar formattableTitleBar = ApplicationView.GetForCurrentView().TitleBar;
        public CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;

        public TitleBarHelper(FrameworkElement elementToTrack)
        {

            //Sets TitleBar colours

            formattableTitleBar.ButtonBackgroundColor = Colors.Transparent;
            formattableTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            //Extends view into TitleBar

            coreTitleBar.ExtendViewIntoTitleBar = true;

            //Track theme changes through sent object

            frameworkElement = elementToTrack;
            frameworkElement.ActualThemeChanged += FrameworkElement_ActualThemeChanged;

        }

        private void FrameworkElement_ActualThemeChanged(FrameworkElement sender, object args)
        {
            if (ColorType == ColorTypes.Automatic)
            {
                SetToAutomaticTitleBar();
            }
        }

        public void SetToAutomaticTitleBar()
        {
            if (frameworkElement.ActualTheme == ElementTheme.Light)
            {
                SetToLightTitleBar(true);
            }
            else
            {
                SetToDarkTitleBar(true);
            }

            ColorType = ColorTypes.Automatic;
        }

        public void SetToLightTitleBar(bool suppressColorTypeChange = false)
        {
            SetTitleBarColor(Colors.Black, suppressColorTypeChange);
        }

        public void SetToDarkTitleBar(bool suppressColorTypeChange = false)
        {
            SetTitleBarColor(Colors.White, suppressColorTypeChange);
        }

        public void SetTitleBarColor(Color color, bool suppressColorTypeChange = false)
        {
            formattableTitleBar.ButtonForegroundColor = color;
            formattableTitleBar.ButtonHoverForegroundColor = color;
            formattableTitleBar.ButtonPressedForegroundColor = color;
            formattableTitleBar.InactiveForegroundColor = color;
            formattableTitleBar.ButtonHoverBackgroundColor = Color.FromArgb(20, color.R, color.G, color.B);
            formattableTitleBar.ButtonPressedBackgroundColor = Color.FromArgb(40, color.R, color.G, color.B);

            if (suppressColorTypeChange == false)
            {
                ColorType = ColorTypes.Custom;
            }
        }

    }
}

using Palette.Helpers;
using Palette.Models;
using Palette.ViewModels;
using Shared.Helpers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Palette.Views
{
    public sealed partial class NewWindowView : UserControl
    {

        TitleBarHelper titleBarHelper;
        Color averageColor = Colors.White;

        public NewWindowView()
        {
            this.InitializeComponent();
            titleBarHelper = new TitleBarHelper(gridSlide);
        }

        private void UserControl_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                ColorCollectionItem colorCollectionItem = (ColorCollectionItem)DataContext;

                // Set the title of the window
                var appView = ApplicationView.GetForCurrentView();
                appView.Title = colorCollectionItem.Name;

                exportView.DataContext = new ExportViewModel(colorCollectionItem);

                // Set the theme depending on the brightness of the color
                averageColor = Helpers.ColorHelper.GetAverageColor(colorCollectionItem.Colors);
                SetAverageColor();
            }
        }

        public void SetAverageColor()
        {
            if (averageColor.R + averageColor.G + averageColor.B < 382)
            {
                titleBarHelper.SetToDarkTitleBar();
                gridColorTitle.RequestedTheme = ElementTheme.Dark;
            }
            else
            {
                titleBarHelper.SetToLightTitleBar();
                gridColorTitle.RequestedTheme = ElementTheme.Light;
            }
        }

        private void UserControl_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            AnimationHelper.ChangeObjectOpacity(gridTitleBar, 0, 1);
            AnimationHelper.ChangeObjectOpacity(rectangleDarken, 0, 1);
            AnimationHelper.ChangeObjectOpacity(gridColorTitle, 1, 0);
            AnimationHelper.ChangeObjectTranslateY(gridSlide, 0, -ActualHeight);
            titleBarHelper.SetToAutomaticTitleBar();
        }

        private void UserControl_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            AnimationHelper.ChangeObjectOpacity(gridTitleBar, 1, 0);
            AnimationHelper.ChangeObjectOpacity(rectangleDarken, 1, 0);
            AnimationHelper.ChangeObjectOpacity(gridColorTitle, 0, 1);
            AnimationHelper.ChangeObjectTranslateY(gridSlide, -ActualHeight, 0);
            SetAverageColor();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            gridSlide.Height = ActualHeight * 2;
        }
    }
}
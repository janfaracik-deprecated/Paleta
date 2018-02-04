using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Paleta.Controls
{
    public sealed partial class ExportItemControl : UserControl
    {
        public ExportItemControl()
        {
            this.InitializeComponent();
            if (ActualWidth >= 500)
            {
                gridSmall.Visibility = Visibility.Collapsed;
                stackPanelLarge.Visibility = Visibility.Visible;
            }
            else
            {
                gridSmall.Visibility = Visibility.Visible;
                stackPanelLarge.Visibility = Visibility.Collapsed;
            }
        }

        public void AnimateIn()
        {
            Animate.Begin();
        }

        private void UserControl_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            if (ActualWidth >= 500)
            {
                gridSmall.Visibility = Visibility.Collapsed;
                stackPanelLarge.Visibility = Visibility.Visible;
            }
            else
            {
                gridSmall.Visibility = Visibility.Visible;
                stackPanelLarge.Visibility = Visibility.Collapsed;
            }
        }
    }
}
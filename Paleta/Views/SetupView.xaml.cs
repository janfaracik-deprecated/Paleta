using Shared.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Paleta.Views
{
    public sealed partial class SetupView : UserControl
    {
        public SetupView()
        {
            this.InitializeComponent();
        }

        private void buttonBegin_Click(object sender, RoutedEventArgs e)
        {
            SettingsHelper.UpdateSetting("SetupComplete", true, false);
            AnimationHelper.FadeObjectVisibility(this, 1, 0, Visibility.Collapsed, 200);
        }
    }
}
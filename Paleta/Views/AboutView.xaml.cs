using System;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;

namespace Palette.Views
{
    public sealed partial class AboutView : UserControl
    {
        public AboutView()
        {
            this.InitializeComponent();

            PackageId packageId = Package.Current.Id;
            PackageVersion version = packageId.Version;

            textBlockTitle.Text = Package.Current.DisplayName;
            textBlockVersion.Text = string.Format("{0}.{1}", version.Major, version.Minor);
        }

        private async void buttonAcknowledgements_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("http://janfaracik.me/Paleta/Acknowledgements"));
        }

        private async void buttonPrivacyPolicy_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("http://janfaracik.me/Paleta/PrivacyPolicy"));
        }
    }
}
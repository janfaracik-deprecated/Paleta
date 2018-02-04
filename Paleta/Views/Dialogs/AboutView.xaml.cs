using System;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Paleta.Views.Dialogs
{
    public sealed partial class AboutView : UserControl
    {
        public AboutView()
        {
            this.InitializeComponent();

            PackageId packageId = Package.Current.Id;
            PackageVersion version = packageId.Version;

            textBlockTitle.Text = Package.Current.DisplayName;

            if (version.Revision == 0)
            {
                textBlockVersion.Text = string.Format("{0}.{1}", version.Major, version.Minor);
            }
            else
            {
                textBlockVersion.Text = string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Revision);
            }

        }

        private async void buttonAcknowledgements_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("http://janfaracik.me/Paleta/Acknowledgements"));
        }

        private async void buttonPrivacyPolicy_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("http://janfaracik.me/Paleta/PrivacyPolicy"));
        }
    }
}
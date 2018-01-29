using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Shared.Helpers
{
    public class WindowHelper
    {

        #region Set Minimum Window Size

        public static void SetMinimumWindowSize(Size size)
        {
            ApplicationView.GetForCurrentView().SetPreferredMinSize(size);
        }

        public static void SetMinimumWindowSize(int width, int height)
        {
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(width, height));
        }

        #endregion

    }
}

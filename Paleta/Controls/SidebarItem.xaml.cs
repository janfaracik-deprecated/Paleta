using Shared.Helpers;
using Windows.UI.Xaml.Controls;

namespace Paleta.Controls
{
    public sealed partial class SidebarItem : UserControl
    {
        public SidebarItem()
        {
            this.InitializeComponent();
        }

        private void Grid_DragEnter(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            AnimationHelper.ChangeObjectOpacity(rectangleDrag, 0, 0.1);
        }

        private void Grid_DragLeave(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            AnimationHelper.ChangeObjectOpacity(rectangleDrag, 0.1, 0);
        }

        private void Grid_Drop(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            AnimationHelper.ChangeObjectOpacity(rectangleDrag, 0.1, 0);
        }
    }
}
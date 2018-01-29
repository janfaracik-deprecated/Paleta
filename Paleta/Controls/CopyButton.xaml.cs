using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Palette.Controls
{
    public sealed partial class CopyButton : UserControl
    {

        public new static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(CopyButton), new PropertyMetadata(null));

        public new object Content
        {
            get
            {
                return GetValue(ContentProperty);
            }
            set
            {
                if (value != null)
                {
                    SetValue(ContentProperty, value);
                }
            }
        }

        public CopyButton()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Animate.Begin();
        }

    }
}
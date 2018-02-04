using Shared.Helpers;
using Windows.UI;

namespace Paleta.Models
{
    public class ColorItem : BindableBase
    {
        private Color _color;
        public Color Color
        {
            get => _color;
            set
            {
                if (_color == value)
                {
                    return;
                }
                _color = value;
                OnPropertyChanged();
            }
        }

        private double _offset;
        public double Offset
        {
            get => _offset;
            set
            {
                if (_offset == value)
                {
                    return;
                }
                _offset = value;
                OnPropertyChanged();
            }
        }
    }
}
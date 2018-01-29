using Shared.Helpers;
using System;
using System.Collections.ObjectModel;

namespace Palette.Models
{
    public class VersionItem : BindableBase
    {

        public ObservableCollection<ColorItem> Colors { get; set; } = new ObservableCollection<ColorItem>();

        private String _name = "";
        public String Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name == value)
                {
                    return;
                }
                _name = value;
                OnPropertyChanged();
            }
        }

        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                if (_selectedIndex == value)
                {
                    return;
                }
                _selectedIndex = value;
                OnPropertyChanged();
                OnPropertyChanged("SelectedItem");
            }
        }

        public ColorItem SelectedItem
        {
            get
            {
                if (SelectedIndex == -1 || Colors.Count == 0)
                {
                    return null;
                }
                return Colors?[_selectedIndex];
            }
        }

        private double _rotation;
        public double Rotation
        {
            get => _rotation;
            set
            {
                _rotation = Math.Floor(value);
                OnPropertyChanged();
            }
        }

        public VersionItem()
        {
            SelectedIndex = 0;
        }

    }
}
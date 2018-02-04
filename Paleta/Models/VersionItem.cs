using Shared.Helpers;
using System;
using System.Collections.ObjectModel;

namespace Paleta.Models
{
    public class VersionItem : BindableBase
    {

        private ObservableCollection<ColorItem> _colors = new ObservableCollection<ColorItem>();
        public ObservableCollection<ColorItem> Colors
        {
            get => _colors;
            set
            {
                _colors = value;
                OnPropertyChanged();
            }
        }

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
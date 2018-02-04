using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using Paleta.Converters;
using Paleta.Helpers;
using Shared.Helpers;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Paleta.Models
{
    public class PaletteItem : BindableBase
    {

        private ObservableCollection<ColorCollectionItem> _colors = new ObservableCollection<ColorCollectionItem>();
        public ObservableCollection<ColorCollectionItem> Colors
        {
            get => _colors;
            set
            {
                _colors = value;
                OnPropertyChanged();
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                {
                    return;
                }
                _name = value;
                Messenger.Default.Send("Save");
                OnPropertyChanged();
            }
        }

        private int _selectedIndex;
        [JsonIgnore]
        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
                OnPropertyChanged("SelectedItem");
            }
        }

        public void ForceSelectedIndex(int newIndex)
        {
            _selectedIndex = newIndex;
            OnPropertyChanged("SelectedIndex");
            OnPropertyChanged("SelectedItem");
        }

        [JsonIgnore]
        public ColorCollectionItem SelectedItem
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

        public PaletteItem()
        {
            _selectedIndex = -1;
            Colors.CollectionChanged += ListColour_CollectionChanged;
            Messenger.Default.Register<String>(this, (action) => ReceiveMessage(action));
        }

        public PaletteItem(string name) : base()
        {
            _name = name;
        }

        #region Public

        public void DeleteSelectedColor()
        {
            DeleteColor(SelectedItem);
        }

        public void DeleteColor(ColorCollectionItem ColorCollectionItem)
        {
            int currentIndex = Colors.IndexOf(ColorCollectionItem);

            Colors.Remove(ColorCollectionItem);

            if (currentIndex == Colors.Count)
            {
                if (Colors.Count != 0)
                {
                    SelectedIndex = Colors.Count - 1;
                }
            }
            else
            {
                SelectedIndex = currentIndex;
            }
        }

        #endregion

        #region Private

        private void ReceiveMessage(String action)
        {
            switch (action)
            {
                case "DeleteSelectedColor":
                    DeleteSelectedColor();
                    break;
            }
        }

        private void ListColour_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Messenger.Default.Send("Save");
        }

        public void DuplicateColour(ColorCollectionItem colorColletionItem)
        {
            Colors.Add(Helpers.ColorHelper.DuplicateColor(colorColletionItem));
            SelectedIndex = Colors.Count - 1;
        }

        #region Add Color

        private ICommand _addColorCommand;

        [JsonIgnore]
        public ICommand AddColorCommand
        {
            get
            {
                if (_addColorCommand == null)
                {
                    _addColorCommand = new RelayCommand(AddColor);
                }
                return _addColorCommand;
            }
        }

        public void AddColor()
        {
            Colors.Add(new ColorCollectionItem("", Windows.UI.Colors.White));
            SelectedIndex = Colors.Count - 1;
        }

        #endregion

        #region Sort Colors

        private ICommand _sortColorByAlphabeticalAscCommand;

        [JsonIgnore]
        public ICommand SortColorByAlphabeticalAscCommand
        {
            get
            {
                if (_sortColorByAlphabeticalAscCommand == null)
                {
                    _sortColorByAlphabeticalAscCommand = new RelayCommand(SortColorByAlphabeticalAsc);
                }
                return _sortColorByAlphabeticalAscCommand;
            }
        }

        public void SortColorByAlphabeticalAsc()
        {
            if (Colors.Count > 0)
            {
                Colors.Sort();
                ForceSelectedIndex(0);
            }
        }

        private ICommand _sortColorByAlphabeticalDescCommand;

        [JsonIgnore]
        public ICommand SortColorByAlphabeticalDescCommand
        {
            get
            {
                if (_sortColorByAlphabeticalDescCommand == null)
                {
                    _sortColorByAlphabeticalDescCommand = new RelayCommand(SortColorByAlphabeticalDesc);
                }
                return _sortColorByAlphabeticalDescCommand;
            }
        }

        public void SortColorByAlphabeticalDesc()
        {
            if (Colors.Count > 0)
            {
                Colors.SortInverse();
                ForceSelectedIndex(0);
            }
        }

        #endregion

        #region Import

        private ICommand _importColorsCommand;

        [JsonIgnore]
        public ICommand ImportColorsCommand
        {
            get
            {
                if (_importColorsCommand == null)
                {
                    _importColorsCommand = new RelayCommand(ImportColors);
                }
                return _importColorsCommand;
            }
        }

        public async void ImportColors()
        {
            TextBox textBox = new TextBox()
            {
                Style = (Style)Application.Current.Resources["TextBoxContentDialog"],
                AcceptsReturn = true,
                PlaceholderText = "#25489E" + Environment.NewLine + "rgb(37, 72, 158)" + Environment.NewLine + "argb(255, 37, 72, 158)",
                IsSpellCheckEnabled = false
            };

            ScrollViewer.SetVerticalScrollBarVisibility(textBox, ScrollBarVisibility.Auto);

            var dialog = new ContentDialog()
            {
                Content = textBox,
                Title = "Import Colors",
                CloseButtonText = "Close Dialog",
                PrimaryButtonText = "Import",
                IsPrimaryButtonEnabled = true
            };

            //Show Dialog

            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                using (StringReader reader = new StringReader(textBox.Text))
                {
                    string line = string.Empty;
                    do
                    {
                        line = reader.ReadLine();
                        if (line != null)
                        {
                            Color? color = ConvertToColor.GenerateColorFromString(line);

                            if (color != null)
                            {
                                ColorCollectionItem colorCollectionItem = new ColorCollectionItem()
                                {
                                    Name = Helpers.ColorHelper.GetNearestName(color.Value)
                                };
                                colorCollectionItem.Colors.Add(new ColorItem() { Color = color.Value });
                                Colors.Add(colorCollectionItem);
                            }
                        }

                    } while (line != null);
                }
            }
        }

        #endregion

        #endregion

    }
}
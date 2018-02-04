using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using Newtonsoft.Json;
using Paleta.Converters;
using Paleta.Helpers;
using Paleta.Views;
using Shared.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Paleta.Models
{
    public class ColorCollectionItem : INotifyPropertyChanged, IComparable
    {

        private DispatcherTimer VersionsTimer = new DispatcherTimer();

        private ObservableCollection<ColorItem> _colors = new ObservableCollection<ColorItem>();
        public ObservableCollection<ColorItem> Colors
        {
            get => _colors;
            set
            {
                _colors = value;
                Colors.CollectionChanged += Colors_CollectionChanged;
                foreach (ColorItem colorItem in Colors)
                {
                    colorItem.PropertyChanged += (s, e2) =>
                    {
                        OnPropertyChanged("Colors");
                    };
                }
                OnPropertyChanged();
            }
        }

        private ObservableCollection<VersionItem> _versions = new ObservableCollection<VersionItem>();
        public ObservableCollection<VersionItem> Versions
        {
            get => _versions;
            set
            {
                _versions = value;
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

        private int _selectedIndex = -1;
        [JsonIgnore]
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

        public void ForceSelectedIndex(int newIndex)
        {
            _selectedIndex = newIndex;
            OnPropertyChanged("SelectedIndex");
            OnPropertyChanged("SelectedItem");
        }

        [JsonIgnore]
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
                if (value == _rotation)
                {
                    return;
                }
                _rotation = Math.Floor(value);
                OnPropertyChanged();
            }
        }

        public ColorCollectionItem()
        {
            VersionsTimer.Tick += VersionsTimer_Tick;
            VersionsTimer.Interval = TimeSpan.FromMilliseconds(800);
            SelectedIndex = 0;
        }

        public ColorCollectionItem(String name, Windows.UI.Color color)
        {
            ColorItem colorItem = new ColorItem() { Color = color, Offset = 0 };
            Colors.Add(colorItem);
            colorItem.PropertyChanged += (s, e2) =>
            {
                OnPropertyChanged("Colors");
            };
            Colors.CollectionChanged += Colors_CollectionChanged;
            SelectedIndex = 0;
            Name = name;
            VersionsTimer.Tick += VersionsTimer_Tick;
            VersionsTimer.Interval = TimeSpan.FromMilliseconds(800);
        }

        public int CompareTo(object o)
        {
            ColorCollectionItem a = this;
            ColorCollectionItem b = (ColorCollectionItem)o;
            return string.Compare(a.Name, b.Name);
        }

        private void Colors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Colors");
            foreach (ColorItem colorItem in Colors)
            {
                colorItem.PropertyChanged += (s, e2) =>
                {
                    OnPropertyChanged("Colors");
                };
            }
        }

        public String GetDefaultExport()
        {
            if (Colors.Count == 1)
            {
                return ColorConverters.ColorToHex(Colors[0].Color);
            }
            else
            {
                return "Gradient";
            }
        }

        #region Open in New Window

        private ICommand _openColorInNewWindowCommand;

        [JsonIgnore]
        public ICommand OpenColorInNewWindowCommand
        {
            get
            {
                if (_openColorInNewWindowCommand == null)
                {
                    _openColorInNewWindowCommand = new RelayCommand<ColorCollectionItem>(c => OpenColorInNewWindowAsync());
                }
                return _openColorInNewWindowCommand;
            }
        }

        public async void OpenColorInNewWindowAsync()
        {
            try
            {
                CoreApplicationView newView = CoreApplication.CreateNewView();
                int newViewId = 0;

                ColorCollectionItem newColorCollectionItem = new ColorCollectionItem { Name = this.Name, Rotation = this.Rotation };

                newColorCollectionItem.Colors.Clear();

                foreach (ColorItem colorItem in Colors)
                {
                    newColorCollectionItem.Colors.Add(new ColorItem { Color = colorItem.Color, Offset = colorItem.Offset });
                }

                await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    NewWindowView newWindowView = new NewWindowView
                    {
                        DataContext = newColorCollectionItem
                    };
                    Window.Current.Content = newWindowView;
                    Window.Current.Activate();
                    newViewId = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().Id;
                });

                ViewModePreferences compactOptions = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
                compactOptions.CustomSize = new Windows.Foundation.Size(270, 270);

                await ApplicationViewSwitcher.TryShowAsViewModeAsync(newViewId, ApplicationViewMode.CompactOverlay, compactOptions);
            }
            catch
            {
                Debug.WriteLine("Failed to create new view");
            }
        }

        #endregion

        #region Use Average Color from Image

        private ICommand _useAverageColorFromImageCommand;

        [JsonIgnore]
        public ICommand UseAverageColorFromImageCommand
        {
            get
            {
                if (_useAverageColorFromImageCommand == null)
                {
                    _useAverageColorFromImageCommand = new RelayCommand<ColorCollectionItem>(c => UseAverageColorFromImage());
                }
                return _useAverageColorFromImageCommand;
            }
        }

        public async void UseAverageColorFromImage()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker()
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {

                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                var bitmapImage = new Windows.UI.Xaml.Media.Imaging.BitmapImage()
                {
                    DecodePixelWidth = 30
                };
                await bitmapImage.SetSourceAsync(stream);

                var decoder = await BitmapDecoder.CreateAsync(stream);

                BitmapBounds bbounds = new BitmapBounds()
                {
                    Width = 10,
                    Height = 10,
                    X = 0,
                    Y = 0
                };

                var myTransform = new BitmapTransform { Bounds = bbounds, ScaledHeight = 10, ScaledWidth = 10 };

                var pixels = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Rgba8,
                    BitmapAlphaMode.Ignore,
                    myTransform,
                    ExifOrientationMode.IgnoreExifOrientation,
                    ColorManagementMode.ColorManageToSRgb);

                var bytes = pixels.DetachPixelData();
                var myDominantColor = Windows.UI.Color.FromArgb(255, bytes[0], bytes[1], bytes[2]);

                SelectedItem.Color = myDominantColor;
            }
        }

        #endregion

        #region Versions

        private String PrintObservable(ObservableCollection<ColorItem> colors)
        {
            String toReturn = "";
            foreach (ColorItem colorItem in colors)
            {
                if (toReturn.Length != 0)
                {
                    toReturn += Environment.NewLine;
                }
                toReturn += colorItem.Color + " " + colorItem.Offset;
            }

            return toReturn;
        }

        public void GenerateVersion()
        {

            VersionItem versionItem = new VersionItem
            {
                Name = this.Name,
                Date = DateTime.Now,
                Rotation = this.Rotation
            };

            versionItem.Colors.Clear();

            foreach (var colorItem in Colors)
            {
                versionItem.Colors.Add(new ColorItem { Color = colorItem.Color, Offset = colorItem.Offset });
            }

            // Check if the version already exists

            VersionItem versionTemp = Versions.FirstOrDefault(v => v.Name == this.Name && v.Rotation == this.Rotation && PrintObservable(v.Colors) == PrintObservable(Colors));
            if (versionTemp != null)
            {
                return;
            }

            if (Versions.Count == 15)
            {
                Debug.WriteLine("We're removing a version!");
                Versions.RemoveAt(0);
            }

            Debug.WriteLine("We're generating a version!");
            Versions.Add(versionItem);
            Debug.WriteLine("Version count: " + Versions.Count);
        }

        public void RestoreVersion(VersionItem versionItem)
        {
            Name = versionItem.Name;
            Rotation = versionItem.Rotation;
            Colors.Clear();
            foreach (var colorItem in versionItem.Colors)
            {
                Colors.Add(new ColorItem { Color = colorItem.Color, Offset = colorItem.Offset });
            }

            ForceSelectedIndex(0);
        }

        private void VersionsTimer_Tick(object sender, object e)
        {
            GenerateVersion();
            Messenger.Default.Send("Save");
            VersionsTimer.Stop();
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (!(propertyName == "SelectedIndex" || propertyName == "SelectedItem"))
            {
                VersionsTimer.Start();
            }
        }

    }
}
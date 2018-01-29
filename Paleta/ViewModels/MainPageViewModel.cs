using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Palette.Models;
using Shared.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using Palette.Views;
using System.Collections.Generic;

namespace Palette.ViewModels
{
    public class MainPageViewModel : BindableBase
    {

        private DispatcherTimer SaveTimer = new DispatcherTimer();
        private static readonly StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
        private static readonly string fileName = "data.txt";

        public ObservableCollection<PaletteItem> Palettes { get; set; } = new ObservableCollection<PaletteItem>();

        private int _selectedIndex = -1;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                SettingsHelper.UpdateSetting("PaletteIndex", value, true);
                _selectedIndex = value;
                OnPropertyChanged();
                OnPropertyChanged("SelectedItem");
            }
        }

        public PaletteItem SelectedItem
        {
            get
            {
                if (SelectedIndex == -1 || Palettes.Count == 0)
                {
                    Debug.WriteLine("Returning null");
                    return null;
                }
                return Palettes?[_selectedIndex];
            }
        }

        #region Public

        public MainPageViewModel()
        {
            LoadPalettes();
            SaveTimer.Tick += SaveTimer_Tick;
            SaveTimer.Interval = TimeSpan.FromMilliseconds(400);
            Messenger.Default.Register<String>(this, (action) => ReceiveMessage(action));
        }

        public async void LoadPalettes()
        {
            try
            {
                StorageFile file = await roamingFolder.GetFileAsync(fileName);
                string json = await FileIO.ReadTextAsync(file);
                Debug.WriteLine("");
                Debug.WriteLine("The File Path is: " + file.Path);
                Debug.WriteLine("");
                Palettes = JsonConvert.DeserializeObject<ObservableCollection<PaletteItem>>(json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            if (Palettes == null)
            {
                Palettes = new ObservableCollection<PaletteItem>();
                Palettes.Add(new PaletteItem { Name = "Default" });
            }
        }

        #endregion

        #region Save

        public async void Save()
        {
            Debug.WriteLine("Saving " + DateTime.Now);
            string json = JsonConvert.SerializeObject(Palettes, Formatting.Indented);
            StorageFile file = await roamingFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, json);
        }

        private void SaveTimer_Tick(object sender, object e)
        {
            Save();
            SaveTimer.Stop();
        }

        private void ReceiveMessage(String action)
        {
            switch (action)
            {
                case "Save":
                    SaveTimer.Start();
                    break;
            }
        }

        #endregion

        #region New Palette

        public void NewPalette()
        {
            Palettes.Add(new PaletteItem());
            SelectedIndex = Palettes.Count - 1;
        }

        #endregion

        public async void ImportPalettes()
        {
            List<PaletteItem> temp = new List<PaletteItem>();

            TextBox textBox = new TextBox
            {
                Style = (Style)Application.Current.Resources["TextBoxContentDialog"],
                AcceptsReturn = true,
                PlaceholderText = "Paste your JSON here",
                IsSpellCheckEnabled = false
            };

            ScrollViewer.SetVerticalScrollBarVisibility(textBox, ScrollBarVisibility.Auto);

            var dialog = new ContentDialog()
            {
                Content = textBox,
                Title = "Import Palettes",
                CloseButtonText = "Close Dialog",
                PrimaryButtonText = "Import",
                IsPrimaryButtonEnabled = true
            };

            //Show Dialog

            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    string json = textBox.Text;
                    temp = JsonConvert.DeserializeObject<List<PaletteItem>>(json);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                if (temp != null)
                {
                    foreach (var paletteItem in temp)
                    {
                        Palettes.Add(paletteItem);
                    }
                }
            }

        }

        public async void ExportPalettes()
        {
            TextBox textBox = new TextBox
            {
                AcceptsReturn = true,
                Text = JsonConvert.SerializeObject(Palettes, Formatting.Indented),
                IsReadOnly = true,
                Style = (Style)Application.Current.Resources["TextBoxContentDialog"],
                IsSpellCheckEnabled = false
            };

            ScrollViewer.SetVerticalScrollBarVisibility(textBox, ScrollBarVisibility.Auto);

            var dialog = new ContentDialog()
            {
                Content = textBox,
                Title = "Export Palettes",
                CloseButtonText = "Close Dialog"
            };

            //Show Dialog

            await dialog.ShowAsync();
        }

        #region Delete Palette

        private ICommand _deletePaletteCommand;

        public ICommand DeletePaletteCommand
        {
            get
            {
                if (_deletePaletteCommand == null)
                {
                    _deletePaletteCommand = new RelayCommand<PaletteItem>(p => DeletePalette(p));
                }
                return _deletePaletteCommand;
            }
        }

        public async void DeletePalette(PaletteItem paletteItem)
        {
            var dialog = new ContentDialog()
            {
                Title = "Are you sure you want to delete this palette?"
            };

            // Add Buttons
            dialog.PrimaryButtonText = "Yes";
            dialog.PrimaryButtonClick += delegate
            {

                int currentIndex = Palettes.IndexOf(paletteItem);

                Palettes.Remove(paletteItem);

                if (currentIndex == Palettes.Count)
                {
                    if (Palettes.Count != 0)
                    {
                        SelectedIndex = Palettes.Count - 1;
                    }
                }
                else
                {
                    SelectedIndex = currentIndex;
                }

                SaveTimer.Start();

            };

            dialog.SecondaryButtonText = "No";
            dialog.SecondaryButtonClick += delegate
            {
                //   btn.Content = "Result: Cancel";
            };

            // Show Dialog
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.None)
            {
                //   btn.Content = "Result: NONE";
            }
        }

        #endregion

        #region Copy

        public void CopyRGB()
        {
            //  ColorItem ci = Palettes[SelectedIndex].Colors[Palettes[SelectedIndex].SelectedIndex];
            //  String str = String.Format("rgb({0}, {1}, {2})", ci.R, ci.G, ci.B);
            //    DataPackage dataPackage = new DataPackage();
            //   dataPackage.SetText(str);
            //   dataPackage.RequestedOperation = DataPackageOperation.Copy;
            //   Clipboard.SetContent(dataPackage);
            //  NotificationHelper.ShowNotification(str + " copied to clipboard.");
        }

        #endregion

        #region Share

        private ICommand _sharePaletteCommand;

        public ICommand SharePaletteCommand
        {
            get
            {
                if (_sharePaletteCommand == null)
                {
                    _sharePaletteCommand = new RelayCommand<PaletteItem>(p => SharePalette(p));
                }
                return _sharePaletteCommand;
            }
        }

        public void SharePalette(PaletteItem paletteItem)
        {
            DataTransferManager.ShowShareUI();
            DataTransferManager.GetForCurrentView().DataRequested += (sender, args) =>
            {
                String shareText = "";

                //foreach (ColorItem ci in paletteItem.Colors)
                //{

                //  // String ColorItemName = ci.Name;

                //    if (String.IsNullOrWhiteSpace(ColorItemName))
                //    {
                //        ColorItemName = "Untitled";
                //    }

                //    if (shareText == "")
                //    {
                //       // shareText = ColorItemName + ": " + ci.Hex;
                //    }
                //    else
                //    {
                //      //  shareText = shareText + Environment.NewLine + ColorItemName + ": " + ci.Hex;
                //    }
                //}

                args.Request.Data.Properties.Title = paletteItem.Name;
                args.Request.Data.SetText(shareText);
            };
        }

        public void Share()
        {
            DataTransferManager.ShowShareUI();
            DataTransferManager.GetForCurrentView().DataRequested += ShareColour_DataRequested;
        }

        private void ShareColour_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            //  ColorItem ci = Palettes[SelectedIndex].Colors[Palettes[SelectedIndex].SelectedIndex];

            //  args.Request.Data.Properties.Title = ci.Name;
            // args.Request.Data.SetText(ci.Hex);
        }

        #endregion

        #region Overflow

        public async void WhatsNew()
        {

            //Create Content

            WhatsNewView whatsNewView = new WhatsNewView();

            var dialog = new ContentDialog()
            {
                Content = whatsNewView,
                Title = "What's New",
                CloseButtonText = "Close Dialog"
            };

            //Show Dialog

            await dialog.ShowAsync();

        }

        public async void About()
        {

            //Create Content

            AboutView aboutView = new AboutView();

            var dialog = new ContentDialog()
            {
                Content = aboutView,
                CloseButtonText = "Close Dialog"
            };

            //Show Dialog

            await dialog.ShowAsync();

        }

        #endregion

    }
}
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Paleta.Models;
using Shared.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Paleta.Views.Dialogs;

namespace Paleta.ViewModels
{
    public class MainPageViewModel : BindableBase
    {

        private DispatcherTimer SaveTimer = new DispatcherTimer();
        private static readonly StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
        private static readonly string fileName = "data.txt";

        private ObservableCollection<PaletteItem> _palettes = new ObservableCollection<PaletteItem>();
        public ObservableCollection<PaletteItem> Palettes
        {
            get => _palettes;
            set
            {
                _palettes = value;
                OnPropertyChanged();
            }
        }

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

                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
                DataContractJsonSerializer ser = new DataContractJsonSerializer(Palettes.GetType());
                Palettes = ser.ReadObject(ms) as ObservableCollection<PaletteItem>;
                ms.Close();

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

            MemoryStream ms = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ObservableCollection<PaletteItem>));
            ser.WriteObject(ms, Palettes);
            byte[] json = ms.ToArray();
            ms.Close();
            string jsonString = Encoding.UTF8.GetString(json, 0, json.Length);

            StorageFile file = await roamingFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, jsonString);
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

        private ICommand _newPaletteCommand;

        public ICommand NewPaletteCommand
        {
            get
            {
                if (_newPaletteCommand == null)
                {
                    _newPaletteCommand = new RelayCommand(NewPalette);
                }
                return _newPaletteCommand;
            }
        }

        public void NewPalette()
        {
            Palettes.Add(new PaletteItem());
            SelectedIndex = Palettes.Count - 1;
            SaveTimer.Start();
        }

        #endregion

        public async void ImportPalettes()
        {

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

                List<PaletteItem> temp = new List<PaletteItem>();

                try
                {
                    string json = textBox.Text;
                    MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(temp.GetType());
                    temp = ser.ReadObject(ms) as List<PaletteItem>;
                    ms.Close();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                foreach (var paletteItem in temp)
                {
                    Palettes.Add(paletteItem);
                }

            }

        }

        public async void ExportPalettes()
        {
            TextBox textBox = new TextBox
            {
                AcceptsReturn = true,
                IsReadOnly = true,
                Style = (Style)Application.Current.Resources["TextBoxContentDialog"],
                IsSpellCheckEnabled = false
            };

            // Convert Palettes to JSON
            MemoryStream ms = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ObservableCollection<PaletteItem>));
            ser.WriteObject(ms, Palettes);
            byte[] json = ms.ToArray();
            ms.Close();
            string jsonString = Encoding.UTF8.GetString(json, 0, json.Length);
            textBox.Text = jsonString;

            ScrollViewer.SetVerticalScrollBarVisibility(textBox, ScrollBarVisibility.Auto);

            var dialog = new ContentDialog()
            {
                Content = textBox,
                Title = "Export Palettes",
                CloseButtonText = "Close Dialog"
            };

            // Show Dialog
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

        #region Overflow Menu

        public async void Settings()
        {
            // Create Content
            var dialog = new ContentDialog()
            {
                Content = new SettingsView(),
                Title = "Settings",
                CloseButtonText = "Close Dialog"
            };

            // Show Dialog
            await dialog.ShowAsync();
        }

        public async void WhatsNew()
        {
            // Create Content
            var dialog = new ContentDialog()
            {
                Content = new WhatsNewView(),
                Title = "What's New",
                CloseButtonText = "Close Dialog"
            };

            // Show Dialog
            await dialog.ShowAsync();
        }

        public async void About()
        {
            // Create Content
            var dialog = new ContentDialog()
            {
                Content = new AboutView(),
                CloseButtonText = "Close Dialog"
            };

            // Show Dialog
            await dialog.ShowAsync();
        }

        #endregion

    }
}
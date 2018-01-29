using GalaSoft.MvvmLight.Messaging;
using Palette.Controls;
using Palette.Converters;
using Palette.Models;
using Palette.ViewModels;
using Shared.Helpers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Palette
{
    public sealed partial class MainPage : Page
    {

        MainPageViewModel ViewModel { get; set; } = new MainPageViewModel();
        String currentClipboardValue = "";
        bool isFocused = true;

        public MainPage()
        {
            this.InitializeComponent();

            // Setup
            try
            {
                if ((bool)SettingsHelper.GetSetting("SetupComplete", false) == false)
                {
                    setupView.Visibility = Visibility.Visible;
                }
            }
            catch
            {
                SettingsHelper.UpdateSetting("SetupComplete", false, false);
                setupView.Visibility = Visibility.Visible;
            }

            // Set window properties, adjust title bar and track theme changes
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(800, 500));
            TitleBarHelper titleBarHelper = new TitleBarHelper(textBlockPalettes);
            titleBarHelper.SetToAutomaticTitleBar();

            // Events
            Messenger.Default.Register<Tuple<String, ColorCollectionItem>>(this, (action) => ReceiveVersionsViewModel(action));
            Window.Current.Activated += Current_Activated;
            Clipboard.ContentChanged += Clipboard_ContentChanged;

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            int index = 0;

            if (ViewModel.Palettes.Count != 0)
            {

                if (SettingsHelper.GetSetting("PaletteIndex", true) != null)
                {
                    index = (int)SettingsHelper.GetSetting("PaletteIndex", true);
                }

                if (index < ViewModel.Palettes.Count)
                {
                    ViewModel.SelectedIndex = index;
                }
                else
                {
                    ViewModel.SelectedIndex = 0;
                }

            }
        }

        #region Drag and Drop

        // Move color to another palette

        private void sidebarItem_DragOver(object sender, DragEventArgs e)
        {
            e.DragUIOverride.Caption = "Move to Palette";

            if (e.DataView.Contains(StandardDataFormats.Text))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
            }
        }

        private void sidebarItem_Drop(object sender, DragEventArgs e)
        {
            PaletteItem paletteItem = (PaletteItem)((SidebarItem)sender).DataContext;

            Debug.WriteLine(paletteItem.Name);

            if (e.DataView.Contains(StandardDataFormats.Text))
            {
                e.Data.Properties.TryGetValue("item", out object ci);
                if (ci != null)
                {
                    ColorCollectionItem colorCollectionItem = Helpers.ColorHelper.DuplicateColor((ColorCollectionItem)ci);
                    ViewModel.SelectedItem.Colors.Remove((ColorCollectionItem)ci);
                    paletteItem.Colors.Add(colorCollectionItem);
                }
            }
        }

        // Duplicate color by dragging it to the new button

        private void buttonAddColor_DragOver(object sender, DragEventArgs e)
        {
            e.DragUIOverride.Caption = "Duplicate";

            if (e.DataView.Contains(StandardDataFormats.Text))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
            }
        }

        private void buttonAddColor_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.Text))
            {
                e.Data.Properties.TryGetValue("item", out object ci);
                ViewModel.SelectedItem.DuplicateColour((ColorCollectionItem)ci);
            }
        }

        private void listViewItems_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            ColorCollectionItem ci = (ColorCollectionItem)e.Items.FirstOrDefault();
            e.Data.SetText(ci.GetDefaultExport());
            e.Data.Properties.Add("item", ci);
            e.Data.RequestedOperation = DataPackageOperation.Copy | DataPackageOperation.Move;
        }

        #endregion

        #region New Palette

        private async void buttonNewPalette_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NewPalette();
            // Scroll to bottom of ListView ScrollViewer on item add
            await Task.Delay(20);
            var scrollViewer = ControlHelper.GetScrollViewer(listViewPalettes);
            scrollViewer.ChangeView(null, scrollViewer.ScrollableHeight, null);
            textBoxPaletteName.Focus(FocusState.Keyboard);
        }

        #endregion

        #region Messaging

        private async void ReceiveVersionsViewModel(Tuple<String, ColorCollectionItem> tuple)
        {
            switch (tuple.Item1)
            {
                case "Versions":
                    // Delay the setting of the DataContext so that the GridView has time to update
                    versionsView.AnimateIn();
                    versionsView.DataContext = null;
                    await Task.Delay(10);
                    versionsView.DataContext = tuple.Item2;
                    break;
                case "Export":
                    ExportViewModel exportViewModel = new ExportViewModel(tuple.Item2);
                    exportView.DataContext = exportViewModel;
                    exportView.AnimateIn();
                    break;
            }
        }

        #endregion

        #region Colors ListView Interactions

        private void listViewPalettes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PaletteItem paletteItem = listViewPalettes.SelectedItem as PaletteItem;

            if (paletteItem != null && paletteItem.Colors.Count != 0)
            {
                paletteItem.SelectedIndex = 0;
                colorView.AnimateIn();
            }

            AnimateInColors.Begin();
        }

        private void listViewColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            colorView.AnimateIn();
        }

        private void Grid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            // Open the selected color item in a new window on double tap
            ViewModel.SelectedItem.SelectedItem.OpenColorInNewWindowAsync();
        }

        #endregion

        #region Clipboard Integration

        private async void Clipboard_ContentChanged(object sender, object e)
        {
            if (isFocused)
            {
                DataPackageView dataPackageView = Clipboard.GetContent();
                if (dataPackageView.Contains(StandardDataFormats.Text))
                {
                    string text = await dataPackageView.GetTextAsync();
                    currentClipboardValue = text;
                }
            }
        }

        private async void Current_Activated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState != CoreWindowActivationState.Deactivated)
            {
                isFocused = true;

                if (ViewModel.SelectedItem != null)
                {
                    DataPackageView dataPackageView = Clipboard.GetContent();
                    if (dataPackageView.Contains(StandardDataFormats.Text))
                    {
                        string text = await dataPackageView.GetTextAsync();

                        // Return if length is more than 20 - it's pretty evident at this point that a color string hasn't been copied
                        if (text.Length > 20)
                        {
                            return;
                        }

                        // Try and create a color from the string
                        Color? color = ConvertToColor.GenerateColorFromString(text);

                        // If the color is null or the text is already the currentClipboardValue then return
                        if (color == null || text == currentClipboardValue)
                        {
                            return;
                        }

                        // Show notification
                        buttonImportFromClipboard.DataContext = new ClipboardColor(Helpers.ColorHelper.GetNearestName(color.Value), color.Value);
                        ShowClipboardNotification();
                        currentClipboardValue = text;
                    }
                }

            }
            else
            {
                isFocused = false;
            }
        }

        private void ShowClipboardNotification()
        {
            buttonImportFromClipboard.Visibility = Visibility.Visible;
            AnimationHelper.ChangeObjectTranslateY(buttonImportFromClipboard, 100, 0);
            AnimationHelper.ChangeObjectOpacity(buttonImportFromClipboard, 0, 1);
        }

        private void HideClipboardNotification()
        {
            AnimationHelper.ChangeObjectTranslateY(buttonImportFromClipboard, 0, 100);
            AnimationHelper.FadeObjectVisibility(buttonImportFromClipboard, 1, 0, Visibility.Collapsed);
        }

        private async void buttonImportFromClipboard_Click(object sender, RoutedEventArgs e)
        {
            ClipboardColor clipboardColor = (ClipboardColor)buttonImportFromClipboard.DataContext;

            ViewModel.SelectedItem.Colors.Add(new ColorCollectionItem(clipboardColor.Name, clipboardColor.Color));
            await Task.Delay(20);
            var scrollViewer = ControlHelper.GetScrollViewer(listViewColors);
            scrollViewer.ChangeView(null, scrollViewer.ScrollableHeight, null);
            HideClipboardNotification();
            ViewModel.SelectedItem.SelectedIndex = ViewModel.SelectedItem.Colors.Count - 1;
        }

        private void buttonCloseClipboard_Click(object sender, RoutedEventArgs e)
        {
            HideClipboardNotification();
        }

        #endregion

        #region Other

        private async void buttonAddColor_Click(object sender, RoutedEventArgs e)
        {
            // Scroll to bottom of ListView ScrollViewer on item add
            await Task.Delay(20);
            var scrollViewer = ControlHelper.GetScrollViewer(listViewColors);
            scrollViewer.ChangeView(null, scrollViewer.ScrollableHeight, null);
        }

        private void buttonDeletePalette_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DeletePalette(ViewModel.SelectedItem);
        }

        #endregion

    }
}
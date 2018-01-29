using Palette.Models;
using Shared.Helpers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Animations;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;
using Palette.Converters;

namespace Palette.Views
{
    public sealed partial class ColorView : UserControl
    {

        public ColorCollectionItem ViewModel { get; set; }

        public ColorView()
        {
            this.InitializeComponent();

            // Update Bindings on DataContext change
            this.DataContextChanged += (s, e) =>
            {
                ViewModel = DataContext as ColorCollectionItem;

                if (ViewModel == null)
                {
                    return;
                }

                ViewModel.ForceSelectedIndex(0);

                Bindings.Update();

                gradientTrackBar.Opacity = 1;
                gradientTrackBar.IsHitTestVisible = true;


                if (ViewModel.Colors.Count == 1)
                {
                    gridRadial.Opacity = 0;
                    gridRadial.IsHitTestVisible = false;
                }

                colorSpectrum.Color = ViewModel.SelectedItem.Color;

                ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            };

        }

        #region Public

        /// <summary> 
        /// Animates in the control
        /// </summary>
        public void AnimateIn()
        {
            AnimateInContainer.Begin();
            textBoxColorName.Focus(FocusState.Keyboard);
            textBoxColorName.SelectionStart = textBoxColorName.Text.Length;
        }

        #endregion

        #region Color Binding

        // Manual binding due to ColorPicker bug

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                colorSpectrum.Color = ViewModel.SelectedItem.Color;
            }
        }

        #endregion

        #region Private

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Apply shadow to Preview pane
            await gridBlur.Blur(value: 30, duration: 0, delay: 0).StartAsync();

            // Hide "Open Color in New Window" button if the feature is not supported
            if (!ApplicationView.GetForCurrentView().IsViewModeSupported(ApplicationViewMode.CompactOverlay))
            {
                buttonOpenInNewWindow.Visibility = Visibility.Collapsed;
            }
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (colorSpectrum == null)
            {
                return;
            }

            gridHEX.Visibility = Visibility.Collapsed;
            gridRGB.Visibility = Visibility.Collapsed;
            gridColorPicker.Visibility = Visibility.Collapsed;

            switch (((RadioButton)sender).Name)
            {
                case "radioButtonHex":
                    gridHEX.Visibility = Visibility.Visible;
                    break;
                case "radioButtonRGB":
                    gridRGB.Visibility = Visibility.Visible;
                    break;
                case "radioButtonHSV":

                    break;
                case "radioButtonWheel":
                    gridColorPicker.Visibility = Visibility.Visible;
                    colorSpectrum.Color = ViewModel.SelectedItem.Color;
                    break;
            }

            AnimationHelper.ChangeObjectOpacity(gridEditControlsInner, 0, 1, 200);
            AnimationHelper.ChangeObjectTranslateY(gridEditControlsInner, 40, 0, 200);

        }

        private void buttonAddToTrackBar_Click(object sender, RoutedEventArgs e)
        {
            gradientTrackBar.Add();
            AnimationHelper.ChangeObjectOpacity(buttonShowRadial, 0, 1);
        }

        private void buttonShowRadial_Click(object sender, RoutedEventArgs e)
        {
            if (gridRadial.Opacity == 1)
            {
                AnimationHelper.ChangeObjectOpacity(gridRadial, 1, 0);
                gridRadial.IsHitTestVisible = false;
                AnimationHelper.ChangeObjectOpacity(gradientTrackBar, 0, 1);
                gradientTrackBar.IsHitTestVisible = true;
            }

            if (gridRadial.Opacity == 0)
            {
                AnimationHelper.ChangeObjectOpacity(gridRadial, 0, 1);
                gridRadial.IsHitTestVisible = true;
                AnimationHelper.ChangeObjectOpacity(gradientTrackBar, 1, 0);
                gradientTrackBar.IsHitTestVisible = false;
            }
        }

        private void buttonSetAngle_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            ViewModel.Rotation = System.Convert.ToDouble(button.Tag);
        }

        private void buttonVersions_Click(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send(new Tuple<String, ColorCollectionItem>("Versions", DataContext as ColorCollectionItem));
        }

        private void buttonExport_Click(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send(new Tuple<String, ColorCollectionItem>("Export", DataContext as ColorCollectionItem));
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            // Adjusts gradient preview shadow according to the scrollviewer's vertical offset
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            gridBlurInner.Height = Math.Max(0, 200 - (scrollViewer.VerticalOffset));
        }

        private void gridEditControlsInner_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Animate edit control background size
            AnimationHelper.ChangeObjectHeight(borderPivotBG, borderPivotBG.ActualHeight, gridEditControlsInner.ActualHeight);
            AnimationHelper.ChangeObjectHeight(borderPivotBorder, borderPivotBorder.ActualHeight, gridEditControlsInner.ActualHeight);
        }

        #region Delete

        private void buttonConfirmDelete_Click(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send("DeleteSelectedColor");
            flyoutDelete.Hide();
        }

        private void buttonCancelDelete_Click(object sender, RoutedEventArgs e)
        {
            flyoutDelete.Hide();
        }

        #endregion

        #endregion

        private void colorSpectrum_ColorChanged(Windows.UI.Xaml.Controls.Primitives.ColorSpectrum sender, ColorChangedEventArgs args)
        {
            try
            {
                ViewModel.SelectedItem.Color = sender.Color;
            }
            catch
            {

            }
        }

        #region ComboBoxes

        private void comboBoxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (comboBoxType.SelectedIndex)
            {
                case 0:
                    colorSpectrum.Shape = ColorSpectrumShape.Box;
                    break;
                case 1:
                    colorSpectrum.Shape = ColorSpectrumShape.Ring;
                    break;
            }
        }

        private void comboBoxView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (comboBoxView.SelectedIndex)
            {
                case 0:
                    colorSpectrum.Components = ColorSpectrumComponents.HueSaturation;
                    break;
                case 1:
                    colorSpectrum.Components = ColorSpectrumComponents.HueValue;
                    break;
                case 2:
                    colorSpectrum.Components = ColorSpectrumComponents.SaturationHue;
                    break;
                case 3:
                    colorSpectrum.Components = ColorSpectrumComponents.SaturationValue;
                    break;
                case 4:
                    colorSpectrum.Components = ColorSpectrumComponents.ValueSaturation;
                    break;
                case 5:
                    colorSpectrum.Components = ColorSpectrumComponents.ValueHue;
                    break;
            }
        }

        #endregion

        private void Copy(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            ColorItem colorItem = ViewModel.SelectedItem;
            string textToCopy = "";

            switch (button.Name)
            {
                case "buttonCopyHex":
                      textToCopy = ColorConverters.ColorToHex(colorItem.Color);
                    break;
                case "buttonCopyRGB":
                     textToCopy = ColorConverters.ColorToRGB(colorItem.Color);
                    break;
                case "buttonCopyHSL":
                    textToCopy = ColorConverters.ColorToHSL(colorItem.Color);
                    break;
            }

            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(textToCopy);
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            Clipboard.SetContent(dataPackage);
        }

    }
}
using Paleta.Controls;
using Paleta.Models;
using Paleta.ViewModels;
using Shared.Helpers;
using System;
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Paleta.Views
{
    public sealed partial class ExportView : UserControl
    {

        public ExportView()
        {
            this.InitializeComponent();
        }

        #region Public

        /// <summary> 
        /// Animates in the control
        /// </summary>
        public void AnimateIn()
        {
            AnimationHelper.FadeObjectVisibility(this, 0, 1, Visibility.Visible);
            AnimationHelper.ChangeObjectTranslateY(gridTitleBar, 40, 0);
            AnimationHelper.ChangeObjectTranslateY(gridSearch, 80, 0);
            AnimationHelper.ChangeObjectTranslateY(gridLists, 120, 0);
            textBoxSearch.Focus(FocusState.Keyboard);
        }

        /// <summary> 
        /// Animates out the control
        /// </summary>
        public void AnimateOut()
        {
            AnimationHelper.ChangeObjectTranslateY(gridTitleBar, 0, 40);
            AnimationHelper.ChangeObjectTranslateY(gridSearch, 0, 80);
            AnimationHelper.ChangeObjectTranslateY(gridLists, 0, 120);
            AnimationHelper.FadeObjectVisibility(this, 1, 0, Visibility.Collapsed);
        }

        #endregion

        #region Private

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            AnimateOut();
        }

        private void textBoxSearch_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Down)
            {
                if (String.IsNullOrEmpty(textBoxSearch.Text))
                {
                    listViewItems.Focus(FocusState.Keyboard);
                }
                else
                {
                    listViewSearch.Focus(FocusState.Keyboard);
                }
            }
        }

        private void listViewItems_ItemClick(object sender, ItemClickEventArgs e)
        {

            ListView listView = sender as ListView;
            ExportItem exportItem = e.ClickedItem as ExportItem;

            if (listView.ContainerFromItem(exportItem) is ListViewItem container)
            {
                var templateRoot = (FrameworkElement)container.ContentTemplateRoot;
                ExportItemControl exportItemControl = templateRoot.FindName("exportItemControl") as ExportItemControl;
                exportItemControl.AnimateIn();
            }

            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(exportItem.Value);
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            Clipboard.SetContent(dataPackage);

            textBoxSearch.Focus(FocusState.Keyboard);

        }

        #endregion

    }
}
using Palette.Models;
using Shared.Helpers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Palette.Views
{
    public sealed partial class VersionsView : UserControl
    {

        const int itemwidth = 510;
        VersionItem selectedVersion = null;

        public VersionsView()
        {
            this.InitializeComponent();
        }

        #region Public

        /// <summary> 
        /// Animates in the control
        /// </summary>
        public void AnimateIn()
        {
            //Debug.WriteLine("Animating in");
            AnimationHelper.FadeObjectVisibility(this, 0, 1, Visibility.Visible);

            stackPanelColorAnimationInner.RenderTransform.SetValue(CompositeTransform.TranslateYProperty, 0);
            scrollViewerExportItems.RenderTransform.SetValue(CompositeTransform.TranslateXProperty, 0);
            scrollViewerExportItems2.RenderTransform.SetValue(CompositeTransform.TranslateXProperty, 0);

            stackPanelColorAnimation.Opacity = 0;
            scrollViewerExportItems2.Opacity = 0;

            AnimationHelper.ChangeObjectTranslateY(gridTitleBar, 40, 0);
            AnimationHelper.ChangeObjectTranslateY(scrollViewerExportItems, 120, 0, 300);
            AnimationHelper.ChangeObjectTranslateY(scrollViewerExportItems2, 120, 0, 300);
            AnimationHelper.ChangeObjectOpacity(scrollViewerExportItems2, 0, 1, 250, 200);
        }

        /// <summary> 
        /// Animates out the control
        /// </summary>
        public void AnimateOut()
        {
            //Debug.WriteLine("Animating out");
            AnimationHelper.ChangeObjectTranslateY(gridTitleBar, 0, 40);
            AnimationHelper.ChangeObjectTranslateY(scrollViewerExportItems, 0, 80);
            AnimationHelper.ChangeObjectOpacity(scrollViewerExportItems2, 1, 0, 50);
            AnimationHelper.ChangeObjectTranslateY(stackPanelColorAnimationInner, 0, 80);
            AnimationHelper.FadeObjectVisibility(this, 1, 0, Visibility.Collapsed);
        }

        #endregion

        #region Private

        private void scrollViewerExportItems_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            //Debug.WriteLine("View Changed");
            // Checks if the scrollviewer is still scrolling, if not, then continue

            try
            {
                if (!e.IsIntermediate)
                {

                    // Gets the horizontal offset of the scrollviewer

                    double targetNumber = scrollViewerExportItems.HorizontalOffset;

                    // Create an array based on multiples of 334 (the ExportControl width + margin)

                    int[] array = new int[100];

                    for (int i = 0; i < 100; i++)
                    {
                        array[i] = itemwidth * i;
                    }

                    // Find the nearest value in the array to the horizontal offset of the scrollviewer and scroll to that position

                    var nearest = array.OrderBy(v => Math.Abs(v - targetNumber)).First();

                    scrollViewerExportItems.ChangeView(nearest, null, null);

                }

                buttonRestore.IsEnabled = (scrollViewerExportItems.HorizontalOffset <= scrollViewerExportItems.ScrollableWidth - itemwidth);

                scrollViewerExportItems2.ChangeView(scrollViewerExportItems.HorizontalOffset, null, null, true);

                ScaleCards(gridViewExportItems);
                ScaleCards(gridViewExportItems2);
            }
            catch
            {
                Debug.WriteLine("Error in viewchanged");
            }
        }

        private void ScaleCards(GridView gridView)
        {
            //Debug.WriteLine("Scale cards");
            try
            {
                double midWayPoint = ActualWidth / 2;

                foreach (var item in gridView.Items)
                {

                    var listviewitem = item as VersionItem;

                    if (gridView.ContainerFromItem(listviewitem) is GridViewItem container)
                    {

                        var templateRoot = (FrameworkElement)container.ContentTemplateRoot;

                        if (templateRoot != null)
                        {

                            try
                            {
                                var transform = templateRoot.TransformToVisual(gridContainer);
                                var positionInScrollViewer = transform.TransformPoint(new Point(0, 0));

                                double difference = midWayPoint - (positionInScrollViewer.X + ((itemwidth - 30) / 2));
                                double differenceAbs = Math.Abs(midWayPoint - (positionInScrollViewer.X + ((itemwidth - 30) / 2)));
                                double divideBy = Math.Pow(1 - (differenceAbs / 20000), 3);

                                if (templateRoot.Opacity != 0)
                                {
                                    templateRoot.Opacity = Math.Max(Math.Pow(1 - (differenceAbs / 2000), 3), 0.1);
                                }

                                templateRoot.RenderTransform.SetValue(CompositeTransform.ScaleXProperty, divideBy);
                                templateRoot.RenderTransform.SetValue(CompositeTransform.ScaleYProperty, divideBy);

                                templateRoot.IsHitTestVisible = templateRoot.Opacity > 0.95;

                                if (templateRoot.Opacity > 0.95)
                                {
                                    selectedVersion = listviewitem;
                                }
                            }
                            catch
                            {

                            }

                        }

                    }

                }
            }
            catch
            {
                Debug.WriteLine("Error in scale cards");
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Debug.WriteLine("size changed event");
            double xPadding = (ActualWidth - itemwidth) / 2;
            stackPanelExportItems.Padding = new Thickness(xPadding, 0, xPadding, 0);
            stackPanelExportItems2.Padding = new Thickness(xPadding, 0, xPadding, 0);
            //Debug.WriteLine("size changed event over");
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine("Cancel");
            AnimateOut();
        }

        private async void buttonRestore_Click(object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine("Restore");
            try
            {
                //Get item in centre of screen
                stackPanelColorAnimation.DataContext = selectedVersion;
                ColorCollectionItem colorCollectionItem = DataContext as ColorCollectionItem;
                VersionItem versionItem = selectedVersion;
                versionItem.Date = DateTime.Now;

                {
                    if (gridViewExportItems.ContainerFromItem(versionItem) is GridViewItem container)
                    {
                        var templateRoot = (FrameworkElement)container.ContentTemplateRoot;
                        if (templateRoot != null)
                        {
                            templateRoot.Opacity = 0;
                        }
                    }
                }
                {
                    if (gridViewExportItems2.ContainerFromItem(versionItem) is GridViewItem container)
                    {
                        var templateRoot = (FrameworkElement)container.ContentTemplateRoot;
                        if (templateRoot != null)
                        {
                            templateRoot.Opacity = 0;
                        }
                    }
                }

                // Animate card
                stackPanelColorAnimation.Opacity = 1;
                IncreaseCardSize.Begin();
                AnimationHelper.ChangeObjectOpacity(scrollViewerExportItems, 1, 0.4);
                await Task.Delay(400);

                // Scroll to end
                AnimationHelper.ChangeObjectTranslateX(scrollViewerExportItems, 0, -510);
                AnimationHelper.ChangeObjectTranslateX(scrollViewerExportItems2, 0, -510);
                scrollViewerExportItems.ChangeView(scrollViewerExportItems.ScrollableWidth, null, null);
                await Task.Delay(300);

                // Decrease card size
                DecreaseCardSize.Begin();
                AnimationHelper.ChangeObjectOpacity(scrollViewerExportItems, 0.4, 1);

                // Animate out
                await Task.Delay(1000);
                colorCollectionItem.RestoreVersion(versionItem);
                AnimateOut();

                await Task.Delay(400);
                colorCollectionItem.Versions.Remove(versionItem);
            }
            catch
            {
                Debug.WriteLine("Error in restoring");
            }
        }

        private async void gridViewExportItems_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Debug.WriteLine("Gridviewexportitems size changed event");
            // Delay the scaling of the cards so that the position can be updated in time
            try
            {
                scrollViewerExportItems.ChangeView(scrollViewerExportItems.ScrollableWidth, null, null, true);
                scrollViewerExportItems2.ChangeView(scrollViewerExportItems2.ScrollableWidth, null, null, true);
                await Task.Delay(10);
                ScaleCards(gridViewExportItems);
                ScaleCards(gridViewExportItems2);
            }
            catch
            {
                Debug.WriteLine("Error in versions size changed");
            }
            //Debug.WriteLine("Gridviewexportitems size changed event over");
        }

        #endregion

    }
}
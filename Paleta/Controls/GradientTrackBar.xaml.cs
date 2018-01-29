using Palette.Models;
using Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Palette.Controls
{
    public sealed partial class GradientTrackBar : UserControl
    {

        ColorCollectionItem colorCollectionItem;

        const double THUMB_WIDTH = 15;
        const double THUMB_HALF_WIDTH = THUMB_WIDTH / 2;

        public GradientTrackBar()
        {
            this.InitializeComponent();
        }

        private void UserControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {

            if (DataContext == null)
            {
                Debug.WriteLine("GradientTrackBar DataContext is null");
                return;
            }

            colorCollectionItem = (ColorCollectionItem)DataContext;

            colorCollectionItem.Colors.CollectionChanged += (s, e) =>
                {
                    ContainerCanvas.Children.Clear();
                    for (int i = 0; i < colorCollectionItem.Colors.Count(); i++)
                    {
                        if (i == 0)
                        {
                            SetSelectedThumb(AddThumb(colorCollectionItem.Colors[i]));
                        }
                        else
                        {
                            AddThumb(colorCollectionItem.Colors[i]);
                        }
                    }
                };

            ContainerCanvas.Children.Clear();
            for (int i = 0; i < colorCollectionItem.Colors.Count(); i++)
            {
                if (i == 0)
                {
                    SetSelectedThumb(AddThumb(colorCollectionItem.Colors[i]));
                }
                else
                {
                    AddThumb(colorCollectionItem.Colors[i]);
                }
            }

        }

        public static Rect GetElementRect(FrameworkElement element)
        {
            GeneralTransform buttonTransform = element.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }

        private void Thumb_RightTappedAsync(object sender, RightTappedRoutedEventArgs e)
        {
            var menu = new MenuFlyout();

            MenuFlyoutItem menu2 = new MenuFlyoutItem
            {
                Text = "Delete Color in Gradient"
            };
            menu2.Click += Menu2_Click;
            menu2.DataContext = ((FrameworkElement)sender).DataContext;
            menu.Items.Add(menu2);
            menu.ShowAt((FrameworkElement)sender);
        }

        private void Menu2_Click(object sender, RoutedEventArgs e)
        {
            ColorItem colorItem = (ColorItem)((FrameworkElement)sender).DataContext;
            Delete(colorItem);
        }

        private void Delete(ColorItem colorItem)
        {
            if (colorCollectionItem.Colors.Count == 1)
            {
                return;
            }

            colorCollectionItem.Colors.Remove(colorItem);

            ContainerCanvas.Children.Remove(GetThumbByColorItem(colorItem));

            colorCollectionItem.ForceSelectedIndex(0);

            SetSelectedThumb(GetThumbByColorItem(colorCollectionItem.Colors[0]));
        }

        private Thumb GetThumbByColorItem(ColorItem colorItem)
        {
            foreach (Thumb thumb in ContainerCanvas.Children)
            {
                if (thumb.DataContext == colorItem)
                {
                    return thumb;
                }
            }
            return null;
        }

        private void Thumb_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SetSelectedThumb((Thumb)sender);
        }

        private void ContainerCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //foreach (Thumb thumb in ContainerCanvas.Children)
            //{
            //    ColorItem colorItem = thumb.DataContext as ColorItem;
            //    Canvas.SetLeft(thumb, GetNewThumbPosition(colorItem.Offset));
            //}
        }

        private void SetSelectedThumb(ColorItem colorItem)
        {
            colorCollectionItem.SelectedIndex = colorCollectionItem.Colors.IndexOf(colorItem);

            foreach (Thumb thumb in ContainerCanvas.Children)
            {
                if (thumb.DataContext == colorItem)
                {
                    gridSelected.Margin = new Thickness(Canvas.GetLeft(thumb) + 19, 0, 0, 27);
                    return;
                }
            }
        }

        private void SetSelectedThumb(Thumb thumb)
        {
            ColorItem colorItem = thumb.DataContext as ColorItem;
            colorCollectionItem.SelectedIndex = colorCollectionItem.Colors.IndexOf(colorItem);
            gridSelected.Margin = new Thickness(Canvas.GetLeft(thumb) + 19, 0, 0, 27);
        }

        private double GetNewThumbPosition(double offset)
        {
            return (offset * (ContainerCanvas.ActualWidth - THUMB_WIDTH)) + THUMB_HALF_WIDTH;
        }

        private double DragThumb(Thumb thumb, double min, double max, double offset)
        {
            var currentPos = Canvas.GetLeft(thumb);
            var nextPos = currentPos + offset;

            nextPos = Math.Max(min, nextPos);
            nextPos = Math.Min(max, nextPos);

            return nextPos;
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = sender as Thumb;
            SetSelectedThumb(thumb);
            ColorItem colorItem = thumb.DataContext as ColorItem;
            var max = DragThumb(thumb, THUMB_HALF_WIDTH, ContainerCanvas.ActualWidth - THUMB_HALF_WIDTH, e.HorizontalChange);
            Canvas.SetLeft((Thumb)sender, max);
            colorItem.Offset = (Canvas.GetLeft(thumb) - THUMB_HALF_WIDTH) / (ContainerCanvas.ActualWidth - THUMB_WIDTH);

            gridToolTip.Margin = new Thickness(Canvas.GetLeft(thumb) + 3, 0, 0, 40);

            if (gridToolTip.Opacity == 0)
            {
                AnimationHelper.ChangeObjectOpacity(gridToolTip, 0, 1);
                AnimationHelper.ChangeObjectTranslateY(gridToolTip, 0, -11);
            }

            gridSelected.Margin = new Thickness(Canvas.GetLeft(thumb) + 19, 0, 0, 27);

            textblockPosition.Text = (Math.Round(colorItem.Offset * 100, 1)) + "%";

        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (gridToolTip.Opacity != 0)
            {
                AnimationHelper.ChangeObjectOpacity(gridToolTip, 1, 0, 160);
                AnimationHelper.ChangeObjectTranslateY(gridToolTip, -11, 0);
            }
        }

        private void Border_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            double thumbPosition = e.GetPosition(ContainerCanvas).X;
            double itemPostion = thumbPosition / (ContainerCanvas.ActualWidth - THUMB_WIDTH);

            // Add new color to collection
            ColorItem colorItem = new ColorItem { Color = Windows.UI.Colors.White, Offset = itemPostion };
            colorCollectionItem.Colors.Add(colorItem);
            SetSelectedThumb(colorItem);
        }

        public void Add()
        {
            if (colorCollectionItem.Colors.Count == 1)
            {
                // If only one color is present, set its offset to 0 and add another one at offset 1
                colorCollectionItem.Colors[0].Offset = 0;
                ColorItem colorItem = new ColorItem { Color = Windows.UI.Colors.White, Offset = 1 };
                colorCollectionItem.Colors.Add(colorItem);
                SetSelectedThumb(colorItem);
            }
            else
            {
                // Else create an item in the center
                ColorItem colorItem = new ColorItem { Color = Windows.UI.Colors.White, Offset = 0.5 };
                colorCollectionItem.Colors.Add(colorItem);
                SetSelectedThumb(colorItem);
            }
        }

        private Thumb AddThumb(ColorItem colorItem, double manualPosition = -1)
        {
            Thumb thumb = new Thumb();
            thumb.Tapped += Thumb_Tapped;
            thumb.DragDelta += Thumb_DragDelta;
            thumb.DragCompleted += Thumb_DragCompleted;
            thumb.RightTapped += Thumb_RightTappedAsync;
            thumb.DataContext = colorItem;
            ContainerCanvas.Children.Add(thumb);

            if (manualPosition == -1)
            {
                Canvas.SetLeft(thumb, GetNewThumbPosition(colorItem.Offset));
            }
            else
            {
                Canvas.SetLeft(thumb, manualPosition);
            }

            return thumb;
        }

    }
}

using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Shared.Helpers
{
    public static class AnimationHelper
    {

        #region Size

        public static void ChangeObjectWidth(DependencyObject o, double oldWidth, double newWidth, double duration = 250, double delay = 0)
        {
            SineEase easingFunction = new SineEase();
            easingFunction.EasingMode = EasingMode.EaseInOut;

            DoubleAnimation widthAnimation = new DoubleAnimation
            {
                From = oldWidth,
                To = newWidth,
                Duration = TimeSpan.FromMilliseconds(duration),
                EnableDependentAnimation = true,
                EasingFunction = easingFunction,
                BeginTime = TimeSpan.FromMilliseconds(delay)
            };

            Storyboard.SetTargetProperty(widthAnimation, "(FrameworkElement.Width)");
            Storyboard.SetTarget(widthAnimation, o);

            Storyboard s = new Storyboard();
            s.Children.Add(widthAnimation);
            s.Begin();
        }

        public static void ChangeObjectHeight(DependencyObject o, double oldHeight, double newHeight, double duration = 250, double delay = 0, EasingFunctionBase easing = null)
        {
            SineEase defaultEasing = new SineEase();
            defaultEasing.EasingMode = EasingMode.EaseInOut;

            DoubleAnimation heightAnimation = new DoubleAnimation
            {
                From = oldHeight,
                To = newHeight,
                Duration = TimeSpan.FromMilliseconds(duration),
                EnableDependentAnimation = true,
                EasingFunction = defaultEasing,
                BeginTime = TimeSpan.FromMilliseconds(delay)
            };

            if (easing != null)
            {
                heightAnimation.EasingFunction = easing;
            }

            Storyboard.SetTargetProperty(heightAnimation, "(FrameworkElement.Height)");
            Storyboard.SetTarget(heightAnimation, o);

            Storyboard s = new Storyboard();
            s.Children.Add(heightAnimation);
            s.Begin();
        }

        public static void ChangeObjectSize(DependencyObject o, double oldSize, double newSize, double duration = 250, double delay = 0)
        {
            SineEase easingFunction = new SineEase();
            easingFunction.EasingMode = EasingMode.EaseInOut;

            DoubleAnimation heightAnimation = new DoubleAnimation
            {
                From = oldSize,
                To = newSize,
                Duration = TimeSpan.FromMilliseconds(duration),
                EnableDependentAnimation = true,
                EasingFunction = easingFunction,
                BeginTime = TimeSpan.FromMilliseconds(delay)
            };

            Storyboard.SetTargetProperty(heightAnimation, "(FrameworkElement.Height)");
            Storyboard.SetTarget(heightAnimation, o);

            Storyboard s = new Storyboard();
            s.Children.Add(heightAnimation);
            s.Begin();
        }


        #endregion

        #region Scale
        
        public static void ChangeObjectScaleX(FrameworkElement o, double oldScale, double newScale, double duration = 250, double delay = 0)
        {
            o.RenderTransform = new CompositeTransform();

            SineEase easingFunction = new SineEase();
            easingFunction.EasingMode = EasingMode.EaseInOut;

            DoubleAnimation scaleAnimation = new DoubleAnimation
            {
                From = oldScale,
                To = newScale,
                Duration = TimeSpan.FromMilliseconds(duration),
                EnableDependentAnimation = true,
                EasingFunction = easingFunction,
                BeginTime = TimeSpan.FromMilliseconds(delay)
            };

            Storyboard.SetTargetProperty(scaleAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTarget(scaleAnimation, o);

            Storyboard s = new Storyboard();
            s.Children.Add(scaleAnimation);
            s.Begin();
        }

        public static void ChangeObjectScaleY(FrameworkElement o, double oldScale, double newScale, double duration = 250, double delay = 0)
        {
            o.RenderTransform = new CompositeTransform();

            SineEase easingFunction = new SineEase
            {
                EasingMode = EasingMode.EaseInOut
            };

            DoubleAnimation scaleAnimation = new DoubleAnimation
            {
                From = oldScale,
                To = newScale,
                Duration = TimeSpan.FromMilliseconds(duration),
                EnableDependentAnimation = true,
                EasingFunction = easingFunction,
                BeginTime = TimeSpan.FromMilliseconds(delay)
            };

            Storyboard.SetTargetProperty(scaleAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            Storyboard.SetTarget(scaleAnimation, o);

            Storyboard s = new Storyboard();
            s.Children.Add(scaleAnimation);
            s.Begin();
        }

        public static void ChangeObjectScale(FrameworkElement o, double newScale, double duration = 250, double delay = 0)
        {
            o.RenderTransform = new CompositeTransform();
            o.RenderTransformOrigin = new Point(0.5, 0.5);

            SineEase easingFunction = new SineEase
             {
                 EasingMode = EasingMode.EaseInOut
             };

             DoubleAnimation scaleAnimation = new DoubleAnimation
             {
                 To = newScale,
                 Duration = TimeSpan.FromMilliseconds(duration),
                 EnableDependentAnimation = true,
                 EasingFunction = easingFunction,
                 BeginTime = TimeSpan.FromMilliseconds(delay),
                 FillBehavior = FillBehavior.Stop
             };

             DoubleAnimation scaleAnimation2 = new DoubleAnimation
             {
                 To = newScale,
                 Duration = TimeSpan.FromMilliseconds(duration),
                 EnableDependentAnimation = true,
                 EasingFunction = easingFunction,
                 BeginTime = TimeSpan.FromMilliseconds(delay),
                 FillBehavior = FillBehavior.Stop
             };

             Storyboard.SetTargetProperty(scaleAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
             Storyboard.SetTarget(scaleAnimation, o);

             Storyboard.SetTargetProperty(scaleAnimation2, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
             Storyboard.SetTarget(scaleAnimation2, o);

             Storyboard s = new Storyboard();
             s.Children.Add(scaleAnimation);
             s.Children.Add(scaleAnimation2);
             s.Begin();
        }

        #endregion

        #region Translate

        public static void ChangeObjectTranslateX(FrameworkElement o, double oldX, double newX, double duration = 250, double delay = 0)
        {

            o.RenderTransform = new CompositeTransform();

            o.RenderTransform.SetValue(CompositeTransform.TranslateXProperty, oldX);

            SineEase easingFunction = new SineEase();
            easingFunction.EasingMode = EasingMode.EaseOut;

            DoubleAnimation heightAnimation = new DoubleAnimation
            {
                From = oldX,
                To = newX,
                Duration = TimeSpan.FromMilliseconds(duration),
                EnableDependentAnimation = true,
                EasingFunction = easingFunction,
                BeginTime = TimeSpan.FromMilliseconds(delay)
            };

            Storyboard.SetTargetProperty(heightAnimation, "(UIElement.RenderTransform).(CompositeTransform.TranslateX)");
            Storyboard.SetTarget(heightAnimation, o);

            Storyboard s = new Storyboard();
            s.Children.Add(heightAnimation);
            s.Begin();
        }

        public static void ChangeObjectTranslateY(FrameworkElement o, double oldY, double newY, double duration = 250, double delay = 0)
        {

            o.RenderTransform = new CompositeTransform();

            o.RenderTransform.SetValue(CompositeTransform.TranslateYProperty, oldY);

            SineEase easingFunction = new SineEase();
            easingFunction.EasingMode = EasingMode.EaseOut;

            DoubleAnimation heightAnimation = new DoubleAnimation
            {
                From = oldY,
                To = newY,
                Duration = TimeSpan.FromMilliseconds(duration),
                EnableDependentAnimation = true,
                EasingFunction = easingFunction,
                BeginTime = TimeSpan.FromMilliseconds(delay)
            };

            Storyboard.SetTargetProperty(heightAnimation, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");
            Storyboard.SetTarget(heightAnimation, o);

            Storyboard s = new Storyboard();
            s.Children.Add(heightAnimation);
            s.Begin();
        }

        #endregion

        #region Opacity

        /// <summary> 
        /// Smoothly animates a FrameworkElement's opacity according to the given paramaters
        /// </summary>
        public static void ChangeObjectOpacity(FrameworkElement o, double oldOpacity, double newOpacity, double duration = 250, double delay = 0)
        {
            SineEase easingFunction = new SineEase();
            easingFunction.EasingMode = EasingMode.EaseInOut;

            o.Opacity = oldOpacity;

            DoubleAnimation heightAnimation = new DoubleAnimation
            {
                From = oldOpacity,
                To = newOpacity,
                EasingFunction = easingFunction,
                Duration = TimeSpan.FromMilliseconds(duration),
                BeginTime = TimeSpan.FromMilliseconds(delay)
            };

            Storyboard.SetTargetProperty(heightAnimation, "(FrameworkElement.Opacity)");
            Storyboard.SetTarget(heightAnimation, o);

            Storyboard s = new Storyboard();
            s.Children.Add(heightAnimation);
            s.Begin();
        }

        public static void FadeObjectVisibility(DependencyObject o, double oldOpacity, double newOpacity, Visibility visibility, double duration = 250, double delay = 0)
        {
            SineEase easingFunction = new SineEase();
            easingFunction.EasingMode = EasingMode.EaseInOut;

            DoubleAnimation heightAnimation = new DoubleAnimation
            {
                From = oldOpacity,
                To = newOpacity,
                EasingFunction = easingFunction,
                Duration = TimeSpan.FromMilliseconds(duration),
                BeginTime = TimeSpan.FromMilliseconds(delay)
            };

            UIElement uiElement = o as UIElement;

            if (visibility == Visibility.Visible)
            {
                uiElement.Visibility = Visibility.Visible;
            }
            else
            {
                heightAnimation.Completed += delegate { uiElement.Visibility = Visibility.Collapsed; };
            }

            Storyboard.SetTargetProperty(heightAnimation, "(FrameworkElement.Opacity)");
            Storyboard.SetTarget(heightAnimation, o);

            Storyboard s = new Storyboard();
            s.Children.Add(heightAnimation);
            s.Begin();
        }

        #endregion

    }
}

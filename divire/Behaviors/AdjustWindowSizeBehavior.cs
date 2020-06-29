//
//  divire
//
//  Copyright (C) 2020 Aru Nanika
//
//  This program is released under the MIT License.
//  https://opensource.org/licenses/MIT
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace divire.Behaviors
{
    public class AdjustWindowSizeBehavior
    {
        //================================//
        //==    Attached Properties     ==//
        //================================//

        public static readonly DependencyProperty IsAttachedProperty
            = DependencyProperty.RegisterAttached(  "IsAttached",
                                                    typeof(bool),
                                                    typeof(AdjustWindowSizeBehavior),
                                                    new PropertyMetadata(false, new PropertyChangedCallback(OnIsAttachedPropertyChanged))
                                                    );

        public static readonly DependencyProperty WindowTopProperty
            = DependencyProperty.RegisterAttached(  "WindowTop",
                                                    typeof(double),
                                                    typeof(AdjustWindowSizeBehavior),
                                                    new PropertyMetadata(0.0, new PropertyChangedCallback(OnWindowTopPropertyChanged))
                                                    );

        public static readonly DependencyProperty WindowLeftProperty
            = DependencyProperty.RegisterAttached(  "WindowLeft",
                                                    typeof(double),
                                                    typeof(AdjustWindowSizeBehavior),
                                                    new PropertyMetadata(0.0, new PropertyChangedCallback(OnWindowLeftPropertyChanged))
                                                    );

        public static readonly DependencyProperty WindowHeightProperty
            = DependencyProperty.RegisterAttached(  "WindowHeight",
                                                    typeof(double),
                                                    typeof(AdjustWindowSizeBehavior),
                                                    new PropertyMetadata(300.0, new PropertyChangedCallback(OnWindowHeightPropertyChanged))
                                                    );

        public static readonly DependencyProperty WindowWidthProperty
            = DependencyProperty.RegisterAttached(  "WindowWidth",
                                                    typeof(double),
                                                    typeof(AdjustWindowSizeBehavior),
                                                    new PropertyMetadata(300.0, new PropertyChangedCallback(OnWindowWidthPropertyChanged))
                                                    );

        public static readonly DependencyProperty LastWindowTopProperty
            = DependencyProperty.RegisterAttached(  "LastWindowTop",
                                                    typeof(double),
                                                    typeof(AdjustWindowSizeBehavior),
                                                    new PropertyMetadata(0.0)
                                                    );

        public static readonly DependencyProperty LastWindowLeftProperty
            = DependencyProperty.RegisterAttached(  "LastWindowLeft",
                                                    typeof(double),
                                                    typeof(AdjustWindowSizeBehavior),
                                                    new PropertyMetadata(0.0)
                                                    );

        public static readonly DependencyProperty LastWindowHeightProperty
            = DependencyProperty.RegisterAttached(  "LastWindowHeight",
                                                    typeof(double),
                                                    typeof(AdjustWindowSizeBehavior),
                                                    new PropertyMetadata(300.0)
                                                    );

        public static readonly DependencyProperty LastWindowWidthProperty
            = DependencyProperty.RegisterAttached(  "LastWindowWidth",
                                                    typeof(double),
                                                    typeof(AdjustWindowSizeBehavior),
                                                    new PropertyMetadata(300.0)
                                                    );

        //================================//
        //==    Methods (Static)        ==//
        //================================//

        public static bool GetIsAttached(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsAttachedProperty);
        }

        public static void SetIsAttached(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAttachedProperty, value);
        }

        public static double GetWindowTop(DependencyObject obj)
        {
            return (double)obj.GetValue(WindowTopProperty);
        }

        public static void SetWindowTop(DependencyObject obj, double value)
        {
            obj.SetValue(WindowTopProperty, value);
        }

        public static double GetWindowLeft(DependencyObject obj)
        {
            return (double)obj.GetValue(WindowLeftProperty);
        }

        public static void SetWindowLeft(DependencyObject obj, double value)
        {
            obj.SetValue(WindowLeftProperty, value);
        }

        public static double GetWindowHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(WindowHeightProperty);
        }

        public static void SetWindowHeight(DependencyObject obj, double value)
        {
            obj.SetValue(WindowHeightProperty, value);
        }

        public static double GetWindowWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(WindowWidthProperty);
        }

        public static void SetWindowWidth(DependencyObject obj, double value)
        {
            obj.SetValue(WindowWidthProperty, value);
        }

        public static double GetLastWindowTop(DependencyObject obj)
        {
            return (double)obj.GetValue(LastWindowTopProperty);
        }

        public static void SetLastWindowTop(DependencyObject obj, double value)
        {
            obj.SetValue(LastWindowTopProperty, value);
        }

        public static double GetLastWindowLeft(DependencyObject obj)
        {
            return (double)obj.GetValue(LastWindowLeftProperty);
        }

        public static void SetLastWindowLeft(DependencyObject obj, double value)
        {
            obj.SetValue(LastWindowLeftProperty, value);
        }

        public static double GetLastWindowHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(LastWindowHeightProperty);
        }

        public static void SetLastWindowHeight(DependencyObject obj, double value)
        {
            obj.SetValue(LastWindowHeightProperty, value);
        }

        public static double GetLastWindowWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(LastWindowWidthProperty);
        }

        public static void SetLastWindowWidth(DependencyObject obj, double value)
        {
            obj.SetValue(LastWindowWidthProperty, value);
        }

        private static void OnIsAttachedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var window = obj as Window;

            if (null != window)
            {
                if (GetIsAttached(window))
                {
                    window.Closing += OnClosing;
                }
                else
                {
                    window.Closing -= OnClosing;
                }
            }
        }

        private static void OnWindowTopPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var window = obj as Window;

            if (null != window)
            {
                window.Top = GetWindowTop(obj);
            }
        }

        private static void OnWindowLeftPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var window = obj as Window;

            if (null != window)
            {
                window.Left = GetWindowLeft(obj);
            }
        }

        private static void OnWindowHeightPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var window = obj as Window;

            if (null != window)
            {
                window.Height = GetWindowHeight(obj);
            }
        }

        private static void OnWindowWidthPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var window = obj as Window;

            if (null != window)
            {
                window.Width = GetWindowWidth(obj);
            }
        }

        private static void OnClosing(object sender, EventArgs e)
        {
            var window = sender as Window;

            SetLastWindowTop(window, window.Top);
            SetLastWindowLeft(window, window.Left);
            SetLastWindowHeight(window, window.ActualHeight);
            SetLastWindowWidth(window, window.ActualWidth);
        }
    }
}

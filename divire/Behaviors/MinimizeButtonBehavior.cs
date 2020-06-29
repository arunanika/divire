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
using System.Windows.Controls;

namespace divire.Behaviors
{
    public class MinimizeButtonBehavior
    {
        //================================//
        //==    Attached Properties     ==//
        //================================//

        public static readonly DependencyProperty IsAttachedProperty
            = DependencyProperty.RegisterAttached(  "IsAttached",
                                                    typeof(bool),
                                                    typeof(MinimizeButtonBehavior),
                                                    new PropertyMetadata(false, new PropertyChangedCallback(OnIsAttachedPropertyChanged))
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

        private static void OnIsAttachedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var button = obj as Button;

            if (null != button)
            {
                if (GetIsAttached(button))
                {
                    button.Click += OnClick;
                }
                else
                {
                    button.Click -= OnClick;
                }
            }
        }

        private static void OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var window = Window.GetWindow(button);

            window.WindowStyle = WindowStyle.SingleBorderWindow;
            window.WindowState = WindowState.Minimized;
        }
    }
}

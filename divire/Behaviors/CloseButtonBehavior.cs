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
using System.Windows.Input;

namespace divire.Behaviors
{
    public class CloseButtonBehavior
    {
        //================================//
        //==    Attached Properties     ==//
        //================================//

        public static readonly DependencyProperty IsAttachedProperty
            = DependencyProperty.RegisterAttached(  "IsAttached",
                                                    typeof(bool),
                                                    typeof(CloseButtonBehavior),
                                                    new PropertyMetadata(false, new PropertyChangedCallback(OnIsAttachedPropertyChanged))
                                                    );

        public static readonly DependencyProperty OnClosingCommandProperty
            = DependencyProperty.RegisterAttached(  "OnClosingCommand",
                                                    typeof(ICommand),
                                                    typeof(CloseButtonBehavior),
                                                    new UIPropertyMetadata(null)
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

        public static ICommand GetOnClosingCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(OnClosingCommandProperty);
        }

        public static void SetOnClosingCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(OnClosingCommandProperty, value);
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

            GetOnClosingCommand(button)?.Execute(null);

            Window.GetWindow(button).Close();
        }
    }
}

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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace divire.Behaviors
{
    public class DestinationSelectButtonBehavior
    {
        //================================//
        //==    Attached Properties     ==//
        //================================//

        public static readonly DependencyProperty IsAttachedProperty
            = DependencyProperty.RegisterAttached(  "IsAttached",
                                                    typeof(bool),
                                                    typeof(DestinationSelectButtonBehavior),
                                                    new PropertyMetadata(false, new PropertyChangedCallback(OnIsAttachedPropertyChanged))
                                                    );

        public static readonly DependencyProperty DestinationProperty
            = DependencyProperty.RegisterAttached(  "Destination",
                                                    typeof(string),
                                                    typeof(DestinationSelectButtonBehavior),
                                                    new PropertyMetadata(string.Empty)
                                                    );

        public static readonly DependencyProperty DemoFileNameProperty
            = DependencyProperty.RegisterAttached(  "DemoFileName",
                                                    typeof(string),
                                                    typeof(DestinationSelectButtonBehavior),
                                                    new PropertyMetadata(string.Empty)
                                                    );

        public static readonly DependencyProperty FilterProperty
            = DependencyProperty.RegisterAttached(  "Filter",
                                                    typeof(string),
                                                    typeof(DestinationSelectButtonBehavior),
                                                    new PropertyMetadata(string.Empty)
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

        public static string GetDestination(DependencyObject obj)
        {
            return (string)obj.GetValue(DestinationProperty);
        }

        public static void SetDestination(DependencyObject obj, string value)
        {
            obj.SetValue(DestinationProperty, value);
        }

        public static string GetDemoFileName(DependencyObject obj)
        {
            return (string)obj.GetValue(DemoFileNameProperty);
        }

        public static void SetDemoFileName(DependencyObject obj, string value)
        {
            obj.SetValue(DemoFileNameProperty, value);
        }

        public static string GetFilter(DependencyObject obj)
        {
            return (string)obj.GetValue(FilterProperty);
        }

        public static void SetFilter(DependencyObject obj, string value)
        {
            obj.SetValue(FilterProperty, value);
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

            var saveFileDialog = new SaveFileDialog();

            var lastDestination = GetDestination(button);
            if (Directory.Exists(lastDestination))
            {
                saveFileDialog.InitialDirectory = lastDestination;
            }
            saveFileDialog.Title = "Select Destination";
            saveFileDialog.DefaultExt = "*";
            saveFileDialog.FileName = GetDemoFileName(button);
            saveFileDialog.Filter = GetFilter(button);

            if (saveFileDialog.ShowDialog() == true)
            {
                var destination = Path.GetDirectoryName(saveFileDialog.FileName);
                SetDestination(button, destination);
            }
        }

    }
}

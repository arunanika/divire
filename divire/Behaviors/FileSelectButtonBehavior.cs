//
//  divire
//
//  Copyright (C) 2020 Aru Nanika
//
//  This program is released under the MIT License.
//  https://opensource.org/licenses/MIT
//
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
using Microsoft.Win32;

namespace divire.Behaviors
{
    public class FileSelectButtonBehavior
    {
        //================================//
        //==    Attached Properties     ==//
        //================================//

        public static readonly DependencyProperty IsAttachedProperty
            = DependencyProperty.RegisterAttached(  "IsAttached",
                                                    typeof(bool),
                                                    typeof(FileSelectButtonBehavior),
                                                    new PropertyMetadata(false, new PropertyChangedCallback(OnIsAttachedPropertyChanged))
                                                    );

        public static readonly DependencyProperty DefaultExtProperty
            = DependencyProperty.RegisterAttached(  "DefaultExt",
                                                    typeof(string),
                                                    typeof(FileSelectButtonBehavior),
                                                    new PropertyMetadata(string.Empty)
                                                    );

        public static readonly DependencyProperty FileFilterProperty
            = DependencyProperty.RegisterAttached(  "FileFilter",
                                                    typeof(string),
                                                    typeof(FileSelectButtonBehavior),
                                                    new PropertyMetadata(string.Empty)
                                                    );

        public static readonly DependencyProperty FilePathProperty
            = DependencyProperty.RegisterAttached(  "FilePath",
                                                    typeof(string),
                                                    typeof(FileSelectButtonBehavior),
                                                    new PropertyMetadata(string.Empty)
                                                    );

        public static readonly DependencyProperty TitleProperty
            = DependencyProperty.RegisterAttached(  "Title",
                                                    typeof(string),
                                                    typeof(FileSelectButtonBehavior),
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

        public static string GetDefaultExt(DependencyObject obj)
        {
            return (string)obj.GetValue(DefaultExtProperty);
        }

        public static void SetDefaultExt(DependencyObject obj, string value)
        {
            obj.SetValue(DefaultExtProperty, value);
        }

        public static string GetFileFilter(DependencyObject obj)
        {
            return (string)obj.GetValue(FileFilterProperty);
        }

        public static void SetFileFilter(DependencyObject obj, string value)
        {
            obj.SetValue(FileFilterProperty, value);
        }

        public static string GetFilePath(DependencyObject obj)
        {
            return (string)obj.GetValue(FilePathProperty);
        }

        public static void SetFilePath(DependencyObject obj, string value)
        {
            obj.SetValue(FilePathProperty, value);
        }

        public static string GetTitle(DependencyObject obj)
        {
            return (string)obj.GetValue(TitleProperty);
        }

        public static void SetTitle(DependencyObject obj, string value)
        {
            obj.SetValue(TitleProperty, value);
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

            var openFileDialog = new OpenFileDialog();

            openFileDialog.DefaultExt = GetDefaultExt(button);
            openFileDialog.Filter = GetFileFilter(button);
            openFileDialog.Title = GetTitle(button);

            if (openFileDialog.ShowDialog() == true)
            {
                SetFilePath(button, openFileDialog.FileName);
            }
        }
    }
}

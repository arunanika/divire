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
    public class FileDropEnabledBehavior
    {
        //================================//
        //==    Attached Properties     ==//
        //================================//

        public static readonly DependencyProperty IsAttachedProperty
            = DependencyProperty.RegisterAttached(  "IsAttached",
                                                    typeof(bool),
                                                    typeof(FileDropEnabledBehavior),
                                                    new PropertyMetadata(false, new PropertyChangedCallback(OnIsAttachedPropertyChanged))
                                                    );

        public static readonly DependencyProperty DroppedHeadItemProperty
            = DependencyProperty.RegisterAttached(  "DroppedHeadItem",
                                                    typeof(string),
                                                    typeof(FileDropEnabledBehavior),
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

        public static string GetDroppedHeadItem(DependencyObject obj)
        {
            return (string)obj.GetValue(DroppedHeadItemProperty);
        }

        public static void SetDroppedHeadItem(DependencyObject obj, string value)
        {
            obj.SetValue(DroppedHeadItemProperty, value);
        }

        private static void OnIsAttachedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var element = obj as UIElement;

            if (null != element)
            {
                if (GetIsAttached(element))
                {
                    element.PreviewDragOver += OnPreviewDragOver;
                }
                else
                {
                    element.PreviewDragOver -= OnPreviewDragOver;
                }
            }
        }

        private static void OnPreviewDragOver(object sender, DragEventArgs e)
        {
            var element = sender as UIElement;
            Window.GetWindow(element).Activate();

            var isFileDrop = e.Data.GetDataPresent(DataFormats.FileDrop);

            if (!isFileDrop)
            {
                return;
            }

            var data = (string[])e.Data.GetData(DataFormats.FileDrop);

            SetDroppedHeadItem(element, data[0]);
        }
    }
}

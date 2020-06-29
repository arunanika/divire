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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace divire.Behaviors
{
    public class RestrictedTextBoxBehavior
    {
        //================================//
        //==    Attached Properties     ==//
        //================================//

        public static readonly DependencyProperty IsAttachedProperty
            = DependencyProperty.RegisterAttached(  "IsAttached",
                                                    typeof(bool),
                                                    typeof(RestrictedTextBoxBehavior),
                                                    new PropertyMetadata(false, new PropertyChangedCallback(OnIsAttachedPropertyChanged))
                                                    );

        public static readonly DependencyProperty PermittedExpressionProperty
            = DependencyProperty.RegisterAttached(  "PermittedExpression",
                                                    typeof(string),
                                                    typeof(RestrictedTextBoxBehavior),
                                                    new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnPermittedExpressionPropertyChanged))
                                                    );

        //========================================//
        //==    Attached Properties (private)   ==//
        //========================================//

        private static readonly DependencyProperty PermittedRegexProperty
            = DependencyProperty.RegisterAttached(  "PermittedRegex",
                                                    typeof(Regex),
                                                    typeof(RestrictedTextBoxBehavior),
                                                    new UIPropertyMetadata(new Regex(@"^[\x00-\x7F]+$"))
                                                    );

        private static readonly DependencyProperty LastTextProperty
            = DependencyProperty.RegisterAttached(  "LastText",
                                                    typeof(string),
                                                    typeof(RestrictedTextBoxBehavior),
                                                    new UIPropertyMetadata(string.Empty)
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

        public static string GetPermittedExpression(DependencyObject obj)
        {
            return (string)obj.GetValue(PermittedExpressionProperty);
        }

        public static void SetPermittedExpression(DependencyObject obj, string value)
        {
            obj.SetValue(PermittedExpressionProperty, value);
        }

        private static Regex GetPermittedRegex(DependencyObject obj)
        {
            return (Regex)obj.GetValue(PermittedRegexProperty);
        }

        private static void SetPermittedRegex(DependencyObject obj, Regex value)
        {
            obj.SetValue(PermittedRegexProperty, value);
        }

        private static string GetLastText(DependencyObject obj)
        {
            return (string)obj.GetValue(LastTextProperty);
        }

        private static void SetLastText(DependencyObject obj, string value)
        {
            obj.SetValue(LastTextProperty, value);
        }

        private static void OnIsAttachedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var textBox = obj as TextBox;

            if (null != textBox)
            {
                if (GetIsAttached(textBox))
                {
                    textBox.PreviewTextInput += PreviewTextInput;
                    textBox.TextChanged += TextChanged;
                }
                else
                {
                    textBox.PreviewTextInput -= PreviewTextInput;
                    textBox.TextChanged += TextChanged;
                }
            }
        }

        private static void OnPermittedExpressionPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var regex = new Regex((string)e.NewValue);
            SetPermittedRegex(obj, regex);
        }

        private static void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            var format = GetPermittedRegex(textBox);

            if (null != format)
            {
                var input = e.Text;

                e.Handled = !format.IsMatch(input);
            }
        }

        private static void TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            var format = GetPermittedRegex(textBox);

            var newText = textBox.Text;
            var lastText = GetLastText(textBox);

            if (newText == lastText)
            {
                return;
            }

            if (format.IsMatch(newText))
            {
                SetLastText(textBox, newText);
            }
            else
            {
                textBox.Text = lastText;
            }
        }
    }
}

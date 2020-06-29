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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace divire.Controls
{
    /// <summary>
    /// TextBox for input numerical value.
    /// </summary>
    public class NumericTextBox : TextBox
    {
        //================================//
        //==    Static Resources        ==//
        //================================//

        private static readonly Regex FormatDecimal = new Regex(@"^\d+$");
        private static readonly Regex FormatHexadecimal = new Regex(@"^[a-fA-F0-9]+$");

        //================================//
        //==    Dependency Properties   ==//
        //================================//

        public static readonly DependencyProperty ValueProperty
            = DependencyProperty.Register(  "Value",
                                            typeof(int),
                                            typeof(NumericTextBox),
                                            new PropertyMetadata(0, new PropertyChangedCallback(OnValuePropertyChanged))
                                            );

        public static readonly DependencyProperty MaxProperty
            = DependencyProperty.Register(  "Max",
                                            typeof(int),
                                            typeof(NumericTextBox),
                                            new PropertyMetadata(int.MaxValue)
                                            );

        public static readonly DependencyProperty MinProperty
            = DependencyProperty.Register(  "Min",
                                            typeof(int),
                                            typeof(NumericTextBox),
                                            new PropertyMetadata(int.MinValue)
                                            );

        public static readonly DependencyProperty IsHexadecimalProperty
            = DependencyProperty.Register(  "IsHexadecimal",
                                            typeof(bool),
                                            typeof(NumericTextBox),
                                            new PropertyMetadata(false, new PropertyChangedCallback(OnIsHexadecimalPropertyChanged))
                                            );

        public static readonly DependencyProperty ZerosPaddingProperty
            = DependencyProperty.Register(  "ZerosPadding",
                                            typeof(int),
                                            typeof(NumericTextBox),
                                            new PropertyMetadata(0, new PropertyChangedCallback(OnZerosPaddingPropertyChanged))
                                            );

        //================================//
        //==    Fields                  ==//
        //================================//

        private Regex format;
        private string representation;
        private string representationWithPadding;

        //================================//
        //==    Constructor             ==//
        //================================//

        public NumericTextBox()
        {
            format = FormatDecimal;
            representation = "D";
            representationWithPadding = "D";
        }

        //================================//
        //==    Properties              ==//
        //================================//

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public int Max
        {
            get { return (int)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        public int Min
        {
            get { return (int)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        public bool IsHexadecimal
        {
            get { return (bool)GetValue(IsHexadecimalProperty); }
            set { SetValue(IsHexadecimalProperty, value); }
        }

        public int ZerosPadding
        {
            get { return (int)GetValue(ZerosPaddingProperty); }
            set { SetValue(ZerosPaddingProperty, value); }
        }

        //================================//
        //==    Methods (Static)        ==//
        //================================//

        private static void OnValuePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var numericTextBox = obj as NumericTextBox;
            numericTextBox.ResetDisplayWithNumericalValue(numericTextBox.Value);
        }

        private static void OnIsHexadecimalPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var numericTextBox = obj as NumericTextBox;
            var isHex = Convert.ToBoolean(e.NewValue);
            numericTextBox.format = (isHex) ? FormatHexadecimal : FormatDecimal;

            numericTextBox.SetupRepresentation();
        }

        private static void OnZerosPaddingPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var numericTextBox = obj as NumericTextBox;
            numericTextBox.SetupRepresentation();
        }

        //================================//
        //==    Methods (Instance)      ==//
        //================================//

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            var input = e.Text;

            e.Handled = !format.IsMatch(input);

            base.OnPreviewTextInput(e);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            ResetDisplayWithNumericalValue(Value);
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            if (!format.IsMatch(Text))
            {
                ResetDisplayWithNumericalValue(Min);
                Value = Min;
            }
            else
            {
                long numericalValue = Min;

                if (IsHexadecimal)
                {
                    long.TryParse(Text, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out numericalValue);
                }
                else
                {
                    long.TryParse(Text, out numericalValue);
                }

                var validated = numericalValue;

                if (Min > numericalValue)
                {
                    validated = Min;
                }
                else if (Max < numericalValue)
                {
                    validated = Max;
                }

                if (numericalValue != validated)
                {
                    ResetDisplayWithNumericalValue((int)validated);
                }
                else
                {
                    var newValue = (int)validated;
                    if (newValue != Value)
                    {
                        Value = newValue;
                    }
                }
            }

            base.OnTextChanged(e);
        }

        private void ResetDisplayWithNumericalValue(int numericalValue)
        {
            var format = (IsFocused) ? representation : representationWithPadding;

            var lastCaretIndex = CaretIndex;

            Text = numericalValue.ToString(format).ToUpper();

            if (IsFocused)
            {
                CaretIndex = lastCaretIndex;
            }
        }

        private void SetupRepresentation()
        {
            var letter = IsHexadecimal ? "X" : "D";
            representation = letter;
            representationWithPadding = letter + ZerosPadding.ToString();
        }
    }
}

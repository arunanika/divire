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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using divire.Models;

namespace divire.Controls
{
    /// <summary>
    /// ColorSelector.xaml の相互作用ロジック
    /// </summary>
    public partial class ColorSelector : UserControl
    {
        //================================//
        //==    Dependency Properties   ==//
        //================================//

        public static readonly DependencyProperty ColorSelectedCommandProperty
            = DependencyProperty.Register(  "ColorSelectedCommand",
                                            typeof(ICommand),
                                            typeof(ColorSelector),
                                            new UIPropertyMetadata(null)
                                            );

        public static readonly DependencyProperty IsOpenProperty
            = DependencyProperty.Register(  "IsOpen",
                                            typeof(bool),
                                            typeof(ColorSelector),
                                            new PropertyMetadata(false, new PropertyChangedCallback(OnIsOpenValuePropertyChanged))
                                            );

        public static readonly DependencyProperty ColorValueProperty
            = DependencyProperty.Register(  "ColorValue",
                                            typeof(int),
                                            typeof(ColorSelector),
                                            new PropertyMetadata(0, new PropertyChangedCallback(OnColorValuePropertyChanged))
                                            );

        public static readonly DependencyProperty HueDisplayProperty
            = DependencyProperty.Register(  "HueDisplay",
                                            typeof(int),
                                            typeof(ColorSelector),
                                            new PropertyMetadata(0, new PropertyChangedCallback(OnHueDisplayPropertyChanged))
                                            );

        public static readonly DependencyProperty RealHueProperty
            = DependencyProperty.Register(  "RealHue",
                                            typeof(double),
                                            typeof(ColorSelector),
                                            new PropertyMetadata(0.0, new PropertyChangedCallback(OnRealHuePropertyChanged))
                                            );

        public static readonly DependencyProperty SaturationDisplayProperty
            = DependencyProperty.Register(  "SaturationDisplay",
                                            typeof(int),
                                            typeof(ColorSelector),
                                            new PropertyMetadata(0, new PropertyChangedCallback(OnSaturationDisplayPropertyChanged))
                                            );


        public static readonly DependencyProperty RealSaturationProperty
            = DependencyProperty.Register(  "RealSaturation",
                                            typeof(double),
                                            typeof(ColorSelector),
                                            new PropertyMetadata(0.0, new PropertyChangedCallback(OnRealSaturationPropertyChanged))
                                            );

        public static readonly DependencyProperty BrightnessDisplayProperty
            = DependencyProperty.Register(  "BrightnessDisplay",
                                            typeof(int),
                                            typeof(ColorSelector),
                                            new PropertyMetadata(0, new PropertyChangedCallback(OnBrightnessDisplayPropertyChanged))
                                            );

        public static readonly DependencyProperty RealBrightnessProperty
            = DependencyProperty.Register(  "RealBrightness",
                                            typeof(double),
                                            typeof(ColorSelector),
                                            new PropertyMetadata(0.0, new PropertyChangedCallback(OnRealBrightnessPropertyChanged))
                                            );

        public static readonly DependencyProperty SaturationMaxColorProperty
            = DependencyProperty.Register(  "SaturationMaxColor",
                                            typeof(int),
                                            typeof(ColorSelector),
                                            new PropertyMetadata(0)
                                            );

        //================================//
        //==    Constructor             ==//
        //================================//

        public ColorSelector()
        {
            InitializeComponent();
        }

        //================================//
        //==    Properties              ==//
        //================================//

        public ICommand ColorSelectedCommand
        {
            get { return (ICommand)GetValue(ColorSelectedCommandProperty); }
            set { SetValue(ColorSelectedCommandProperty, value); }
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public int ColorValue
        {
            get { return (int)GetValue(ColorValueProperty); }
            set { SetValue(ColorValueProperty, value); }
        }

        public int HueDisplay
        {
            get { return (int)GetValue(HueDisplayProperty); }
            set { SetValue(HueDisplayProperty, value); }
        }

        public double RealHue
        {
            get { return (double)GetValue(RealHueProperty); }
            set { SetValue(RealHueProperty, value); }
        }

        public int SaturationDisplay
        {
            get { return (int)GetValue(SaturationDisplayProperty); }
            set { SetValue(SaturationDisplayProperty, value); }
        }

        public double RealSaturation
        {
            get { return (double)GetValue(RealSaturationProperty); }
            set { SetValue(RealSaturationProperty, value); }
        }

        public int BrightnessDisplay
        {
            get { return (int)GetValue(BrightnessDisplayProperty); }
            set { SetValue(BrightnessDisplayProperty, value); }
        }

        public double RealBrightness
        {
            get { return (double)GetValue(RealBrightnessProperty); }
            set { SetValue(RealBrightnessProperty, value); }
        }

        public int SaturationMaxColor
        {
            get { return (int)GetValue(SaturationMaxColorProperty); }
            set { SetValue(SaturationMaxColorProperty, value); }
        }

        //================================//
        //==    Methods (Static)        ==//
        //================================//

        private static void OnIsOpenValuePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var selector = obj as ColorSelector;
            var oldState = Convert.ToBoolean(e.OldValue);
            var newState = Convert.ToBoolean(e.NewValue);

            if( (!newState) && oldState)
            {
                selector.ColorSelectedCommand?.Execute(null);
            }
        }

        private static void OnColorValuePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var colorSelector = obj as ColorSelector;
            var newRgb = Convert.ToInt32(e.NewValue);

            if (colorSelector.RgbTextBox.IsFocused)
            {
                colorSelector.UpdateRealHSV();
            }
        }

        private static void OnRealHuePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var colorSelector = obj as ColorSelector;
            colorSelector.UpdateHueDisplay();

            if (colorSelector.IsOpen)
            {
                colorSelector.UpdateRgbValueFromRealHSV();
            }

            colorSelector.SaturationMaxColor = GetRgbFromHsv(colorSelector.RealHue, 100, 80);
        }

        private static void OnHueDisplayPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var colorSelector = obj as ColorSelector;

            if (colorSelector.HueTextBox.IsFocused)
            {
                var updatedHue = Convert.ToInt32(e.NewValue);
                colorSelector.RealHue = updatedHue;
            }
        }

        private static void OnRealSaturationPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var colorSelector = obj as ColorSelector;
            colorSelector.UpdateSaturationDisplay();

            if (colorSelector.IsOpen)
            {
                colorSelector.UpdateRgbValueFromRealHSV();
            }
        }

        private static void OnSaturationDisplayPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var colorSelector = obj as ColorSelector;

            if (colorSelector.SaturationTextBox.IsFocused)
            {
                var updatedSaturation = Convert.ToInt32(e.NewValue);
                colorSelector.RealSaturation = updatedSaturation;
            }
        }

        private static void OnRealBrightnessPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var colorSelector = obj as ColorSelector;
            colorSelector.UpdateBrightnessDisplay();

            if (colorSelector.IsOpen)
            {
                colorSelector.UpdateRgbValueFromRealHSV();
            }
        }

        private static void OnBrightnessDisplayPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var colorSelector = obj as ColorSelector;

            if (colorSelector.BrightnessTextBox.IsFocused)
            {
                var updatedBrightness = Convert.ToInt32(e.NewValue);
                colorSelector.RealBrightness = updatedBrightness;
            }
        }

        private static int Round(double value)
        {
            return (int)Math.Round(value, 0, MidpointRounding.AwayFromZero);
        }

        private static int GetRgbFromHsv(double hue, double saturation, double brightness)
        {
            var saturationRatio = saturation / 100.0;
            var brightnessRatio = brightness / 100.0;
            var rgb = ColorCalculation.GetRgbValueFromHsv(hue, saturationRatio, brightnessRatio);
            return rgb;
        }

        private static Tuple<double, double, double> GetRealHsvFromRgb(int rgbValue)
        {
            var hsv = ColorCalculation.GetHSVFromRgbValue(rgbValue);

            return new Tuple<double, double, double>(hsv.Item1, (hsv.Item2 * 100.0), (hsv.Item3 * 100.0));
        }

        //================================//
        //==    Methods (Instance)      ==//
        //================================//

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateRealHSV();

            IsOpen = !IsOpen;
        }

        private void UpdateRgbValueFromRealHSV()
        {
            if (!RgbTextBox.IsFocused)
            {
                ColorValue = GetRgbFromHsv(RealHue, RealSaturation, RealBrightness);
            }
        }

        private void UpdateRealHSV()
        {
            var lastRgb = GetRgbFromHsv(RealHue, RealSaturation, BrightnessDisplay);

            if (lastRgb != ColorValue)
            {
                var realHsv = GetRealHsvFromRgb(ColorValue);
                RealHue = realHsv.Item1;
                RealSaturation = realHsv.Item2;
                RealBrightness = realHsv.Item3;
            }
        }

        private void UpdateHueDisplay()
        {
            var roundedHue = Round(RealHue);

            if (roundedHue != HueDisplay)
            {
                HueDisplay = roundedHue;
            }
        }

        private void UpdateSaturationDisplay()
        {
            var roundedSaturation = Round(RealSaturation);

            if (roundedSaturation != SaturationDisplay)
            {
                SaturationDisplay = roundedSaturation;
            }
        }

        private void UpdateBrightnessDisplay()
        {
            var roundedBrightness = Round(RealBrightness);

            if (roundedBrightness != BrightnessDisplay)
            {
                BrightnessDisplay = roundedBrightness;
            }
        }
    }
}

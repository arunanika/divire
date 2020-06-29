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

namespace divire.Controls
{
    /// <summary>
    /// NumericUpDown.xaml の相互作用ロジック
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        //================================//
        //==    Dependency Properties   ==//
        //================================//

        public static readonly DependencyProperty ValueProperty
            = DependencyProperty.Register(  "Value",
                                            typeof(int),
                                            typeof(NumericUpDown),
                                            new PropertyMetadata(0)
                                            );

        public static readonly DependencyProperty MaxProperty
            = DependencyProperty.Register(  "Max",
                                            typeof(int),
                                            typeof(NumericUpDown),
                                            new PropertyMetadata(int.MaxValue)
                                            );

        public static readonly DependencyProperty MinProperty
            = DependencyProperty.Register(  "Min",
                                            typeof(int),
                                            typeof(NumericUpDown),
                                            new PropertyMetadata(int.MinValue)
                                            );

        //================================//
        //==    Constructor             ==//
        //================================//

        public NumericUpDown()
        {
            InitializeComponent();
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

        //================================//
        //==    Methods                 ==//
        //================================//

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            numericTextBox.Text = (Value + 1).ToString();
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            numericTextBox.Text = (Value - 1).ToString();
        }
    }
}

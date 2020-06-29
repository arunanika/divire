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
using System.Windows.Interop;

namespace divire.Controls
{
    using ROWTUPLE = Tuple<object, object, object, object, object>;

    public class DisplayDataGrid : DataGrid
    {
        //================================//
        //==    Static Resources        ==//
        //================================//

        private static readonly int WmExitSizeMove = 0x232;
        private static readonly double GridSpacing = 2.0;
        private static readonly double CellElementsSpacing = 1.0;
        private static readonly double ColumnOffset = 16.0;
        private static readonly double MinimumCellHeight = 6.0;
        private static readonly List<ROWTUPLE> EmptySource = new List<ROWTUPLE>();

        //================================//
        //==    Dependency Properties   ==//
        //================================//

        public static readonly DependencyProperty PrimitiveSourceProperty
            = DependencyProperty.Register(  "PrimitiveSource",
                                            typeof(IEnumerable<object>),
                                            typeof(DisplayDataGrid),
                                            new PropertyMetadata(null, new PropertyChangedCallback(OnPrimitiveSourcePropertyChanged))
                                            );

        public static readonly DependencyProperty CellElementsWidthProperty
            = DependencyProperty.Register(  "CellElementsWidth",
                                            typeof(double),
                                            typeof(DisplayDataGrid),
                                            new PropertyMetadata(12.0)
                                            );

        public static readonly DependencyProperty CellElementsHeightProperty
            = DependencyProperty.Register(  "CellElementsHeight",
                                            typeof(double),
                                            typeof(DisplayDataGrid),
                                            new PropertyMetadata(6.0)
                                            );

        public static readonly DependencyProperty PaddingObjectProperty
            = DependencyProperty.Register(  "PaddingObject",
                                            typeof(object),
                                            typeof(DisplayDataGrid),
                                            new PropertyMetadata(-1)
                                            );

        //================================//
        //==    Fields                  ==//
        //================================//

        private bool hasWindowHelperSet;

        //================================//
        //==    Constructor             ==//
        //================================//

        public DisplayDataGrid()
        {
            hasWindowHelperSet = false;

            Loaded += DisplayDataGrid_Loaded;
            
            SizeChanged += DisplayDataGrid_SizeChanged;
        }

        //================================//
        //==    Properties              ==//
        //================================//

        public IEnumerable<object> PrimitiveSource
        {
            get { return (IEnumerable<object>)GetValue(PrimitiveSourceProperty); }
            set { SetValue(PrimitiveSourceProperty, value); }
        }

        public double CellElementsWidth
        {
            get { return (double)GetValue(CellElementsWidthProperty); }
            set { SetValue(CellElementsWidthProperty, value); }
        }

        public double CellElementsHeight
        {
            get { return (double)GetValue(CellElementsHeightProperty); }
            set { SetValue(CellElementsHeightProperty, value); }
        }

        public object PaddingObject
        {
            get { return (object)GetValue(PaddingObjectProperty); }
            set { SetValue(PaddingObjectProperty, value); }
        }

        //================================//
        //==    Methods (Static)        ==//
        //================================//

        private static void OnPrimitiveSourcePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var grid = obj as DisplayDataGrid;

            grid.UpdateElementsDisplay();
        }

        private static IEnumerable<Tuple<object, object, object, object, object>> GetArrangedSource(IEnumerable<object> source, int countOfRowItems, object paddingItem)
        {
            if ((0 >= countOfRowItems) || (5 < countOfRowItems))
            {
                return null;
            }

            var elementsCount = source.Count();
            var rowsCount = (elementsCount + (countOfRowItems - 1)) / countOfRowItems;
            var c = countOfRowItems;
            var p = paddingItem;

            var list = new List<ROWTUPLE>(Enumerable.Range(0, rowsCount - 1)
                .Select(e => new ROWTUPLE(  source.ElementAt(e * c),
                                            ((2 > c) ? p : source.ElementAt(e * c + 1)),
                                            ((3 > c) ? p : source.ElementAt(e * c + 2)),
                                            ((4 > c) ? p : source.ElementAt(e * c + 3)),
                                            ((5 > c) ? p : source.ElementAt(e * c + 4))
                                            )));

            var lastIndex = rowsCount - 1;
            var lastRowItem = new ROWTUPLE(
                source.ElementAt(lastIndex * c),
                (((lastIndex * c + 1) >= elementsCount) ? p : source.ElementAt(lastIndex * c + 1)),
                (((lastIndex * c + 2) >= elementsCount) ? p : source.ElementAt(lastIndex * c + 2)),
                (((lastIndex * c + 3) >= elementsCount) ? p : source.ElementAt(lastIndex * c + 3)),
                (((lastIndex * c + 4) >= elementsCount) ? p : source.ElementAt(lastIndex * c + 4))
                );
            list.Add(lastRowItem);

            return list;
        }

        //================================//
        //==    Methods (Instance)      ==//
        //================================//

        private void DisplayDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            DependencyObject dependencyObject = this;

            while (null != dependencyObject)
            {
                var parent = LogicalTreeHelper.GetParent(dependencyObject);

                if (parent is TabItem)
                {
                    var tabItem = parent as TabItem;

                    tabItem.GotFocus += TabItem_GotFocus;
                }

                if (parent is Window)
                {
                    var window = parent as Window;

                    window.Activated += Window_Activated;

                    break;
                }

                dependencyObject = parent;
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            UpdateElementsDisplay();
        }

        private void TabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            UpdateElementsDisplay();
        }

        private void DisplayDataGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var helper = new WindowInteropHelper(Window.GetWindow(this));

            if (!hasWindowHelperSet)
            {
                hasWindowHelperSet = true;

                if (null != helper.Handle)
                {
                    var source = HwndSource.FromHwnd(helper.Handle);
                    if (null != source)
                    {
                        source.AddHook(HwndMessageHook);
                    }
                }
            }
        }

        private IntPtr HwndMessageHook(IntPtr wnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if(WmExitSizeMove == msg)
            {
                hasWindowHelperSet = false;
                UpdateElementsDisplay();
            }

            return IntPtr.Zero;
        }

        private void UpdateElementsDisplay()
        {
            var primitive = PrimitiveSource?.Cast<object>();
            if (null == primitive)
            {
                return;
            }

            var elementsCount = primitive.Count();

            if (0 >= ActualHeight)
            {
                return;
            }

            if (0 > elementsCount)
            {
                return;
            }

            if (0 == elementsCount)
            {
                ItemsSource = EmptySource;
                return;
            }

            var rowsCount = elementsCount;
            var columnsCount = 1;
            double calculatedHeight = (ActualHeight - (GridSpacing * (rowsCount + 1))) / rowsCount;

            for (; columnsCount <= 5; columnsCount++)
            {
                rowsCount = (elementsCount + (columnsCount - 1)) / columnsCount;
                calculatedHeight = (ActualHeight - (GridSpacing * (rowsCount + 1))) / rowsCount;

                if ((MinimumCellHeight <= calculatedHeight) || (columnsCount == 5))
                {
                    ItemsSource = GetArrangedSource(primitive, columnsCount, PaddingObject);
                    break;
                }
            }

            CellElementsWidth = (ActualWidth - ColumnOffset) / columnsCount - CellElementsSpacing;
            CellElementsHeight = calculatedHeight;
        }
    }
}

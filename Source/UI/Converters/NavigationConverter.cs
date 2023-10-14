using FRED.UI.Controls;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace FRED.UI.Converters {
    public class NavigationConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if(value is ScrollViewer) {
                ScrollViewer scroller = (ScrollViewer) value;

                if (scroller != null) {
                    NavigationItem header = (NavigationItem) scroller.Parent;

                    if(header != null) {
                        double height = 0;

                        if(!(header.Height is double.NaN)) {
                            height = header.Height;
                        }

                        if (!(header.ActualHeight is double.NaN)) {
                            height = header.ActualHeight;
                        }
                    }
                }
            }
           
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new Exception("Only OneWay supported!");
        }
    }
}

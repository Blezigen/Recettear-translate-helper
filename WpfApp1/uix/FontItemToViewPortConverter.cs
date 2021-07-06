using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WpfApp1.uix
{
    public class FontItemToViewPortConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fontData = value as FontData;
            if (fontData== null)
                return null;

            return  new Rect(fontData.HorizontalOffset,
                fontData.VerticalOffset - fontData.VerticalOffsetCorrection,
                fontData.BoxWidth, 48);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

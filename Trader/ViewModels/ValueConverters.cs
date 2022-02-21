using System;
using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Markup;
using Tinkoff.InvestApi.V1;

namespace Trader.ViewModels
{
    public class DecToValueConverter : MarkupExtension, IValueConverter
    {
        private static DecToValueConverter _converter = null;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal input = (decimal)value;
            if (input < 0) return Brushes.Coral;
            else if (input == 0) return Brushes.Gray;
            else return Brushes.LightGreen;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_converter == null)
                _converter = new DecToValueConverter();
            return _converter;
        }
    }

    public class StringToValueConverter : MarkupExtension, IValueConverter
    {
        private static StringToValueConverter _converter = null;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal input = decimal.Parse((string)value);
            if (input < 0) return Brushes.Coral;
            else if (input == 0) return Brushes.LightGray;
            else return Brushes.LightGreen;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_converter == null)
                _converter = new StringToValueConverter();
            return _converter;
        }
    }

    public class CurrencyToValueConverter : MarkupExtension, IValueConverter
    {
        private static CurrencyToValueConverter _converter = null;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string input = (string)value;
            if (input == "rub") return Brushes.LightBlue;
            else if (input == "usd") return Brushes.LightGoldenrodYellow;
            else return Brushes.Gray;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_converter == null)
                _converter = new CurrencyToValueConverter();
            return _converter;
        }
    }
}

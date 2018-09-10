using System;
using System.Globalization;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;

namespace XDemo.UI.Markup.Converters
{
    /// <summary>
    /// get the first error messages from validation result
    /// </summary>
    public class FirstValidationErrorConverter : IValueConverter
    {
        public static FirstValidationErrorConverter Instance = new FirstValidationErrorConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var messages = value as IEnumerable<string>;
            return messages?.FirstOrDefault();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

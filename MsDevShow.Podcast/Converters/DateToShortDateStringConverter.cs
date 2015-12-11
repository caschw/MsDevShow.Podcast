using System;
using Windows.UI.Xaml.Data;

namespace MsDevShow.Podcast.Converters
{
    public class DateToShortDateStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var date = (DateTime) value;
            return date.Date.ToString(new System.Globalization.DateTimeFormatInfo().ShortDatePattern);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}

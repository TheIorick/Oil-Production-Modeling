using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Task3_10.Models;

namespace Task3_10.Converters
{
    public class StatusToBrushConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is OilRigStatus status)
            {
                return status switch
                {
                    OilRigStatus.Operational => new SolidColorBrush(Colors.Green),
                    OilRigStatus.Damaged => new SolidColorBrush(Colors.Red),
                    OilRigStatus.Inactive => new SolidColorBrush(Colors.Gray),
                    _ => new SolidColorBrush(Colors.Black)
                };
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
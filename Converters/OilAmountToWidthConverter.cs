using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Task3_10.Converters
{
    public class OilAmountToWidthConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double amount && parameter is double capacity && capacity > 0)
            {
                // Вычисляем процент заполнения
                double percentage = Math.Min(amount / capacity, 1.0);
                
                // Предполагаем, что максимальная ширина элемента - 100
                return percentage * 100;
            }
            return 0;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
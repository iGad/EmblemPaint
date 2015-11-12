using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EmblemPaint.Kernel
{
    /// <summary>
    /// Конвертер из double в Int
    /// </summary>
    [ValueConversion(typeof(int), typeof(double))]
    public class DoubleToIntValueConverter : IValueConverter
    {
        /// <summary>
        /// Преобразует из Int в Double
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(value, culture);            
        }

        /// <summary>
        /// Преобразует из double в int
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;
            return System.Convert.ChangeType(value, typeof(int), culture);
        }
    }
}

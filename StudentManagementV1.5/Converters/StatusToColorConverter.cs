using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace StudentManagementV1._5.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value?.ToString() ?? string.Empty;
            
            return status.ToLower() switch
            {
                "draft" => new SolidColorBrush(Color.FromRgb(158, 158, 158)),        // Gray
                "published" => new SolidColorBrush(Color.FromRgb(3, 169, 244)),       // Blue
                "closed" => new SolidColorBrush(Color.FromRgb(96, 125, 139)),         // Blue Gray
                "submitted" => new SolidColorBrush(Color.FromRgb(255, 152, 0)),       // Orange
                "graded" => new SolidColorBrush(Color.FromRgb(76, 175, 80)),          // Green
                "rejected" => new SolidColorBrush(Color.FromRgb(244, 67, 54)),        // Red
                "not submitted" => new SolidColorBrush(Color.FromRgb(158, 158, 158)), // Gray
                "upcoming" => new SolidColorBrush(Color.FromRgb(3, 169, 244)),        // Blue
                "due soon" => new SolidColorBrush(Color.FromRgb(255, 152, 0)),        // Orange
                "overdue" => new SolidColorBrush(Color.FromRgb(244, 67, 54)),         // Red
                _ => new SolidColorBrush(Color.FromRgb(97, 97, 97))                   // Dark Gray (default)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

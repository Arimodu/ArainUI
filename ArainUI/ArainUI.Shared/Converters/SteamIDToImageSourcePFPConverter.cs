using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace ArainUI.Converters
{
    public class SteamIDToImageSourcePFPConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!Uri.TryCreate($"https://new.scoresaber.com/api/static/avatars/{value}.jpg", UriKind.RelativeOrAbsolute, out Uri uri)) return null;
            return new BitmapImage(uri);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}

using ScreamViewLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Windows.Forms.Design.AxImporter;

namespace ScreamControl
{
    internal class Header
    {
        private double _scaleFactor = 1.0;

        private System.Windows.Controls.Label _title = null!;
        private System.Windows.Controls.Label _option1 = null!;
        private System.Windows.Controls.Label _option2 = null!;
        private System.Windows.Controls.Label _option3 = null!;

        private ScreamOffConfig _config;

        public Header(double scaleFactor, ScreamOffConfig config)
        {
            _scaleFactor = scaleFactor;
            _config = config;
        }

        public void CreateOnCanvas(Canvas canvas)
        {
            _title = CreateTextElement(0.2, 0.0, 0.6, 0.1, _config.Title, 150, canvas, _scaleFactor);
            _option1 = CreateTextElement(0.1, 0.1, 0.2, 0.1, _config.Option1, 96, canvas, _scaleFactor);
            _option2 = CreateTextElement(0.4, 0.1, 0.2, 0.1, _config.Option2, 96, canvas, _scaleFactor);
            _option3 = CreateTextElement(0.7, 0.1, 0.2, 0.1, _config.Option3, 96, canvas, _scaleFactor);

            foreach (var image in _config.Images)
            {
                CreateImageElement(image.Left, image.Top, image, canvas, _scaleFactor);
            }
        }

        private static System.Windows.Controls.Label CreateTextElement(double xp, double yp, double wp, double hp, ScreamOffConfig.TextOption info, double fontSize, Canvas canvas, double scale)
        {
            var elem = new System.Windows.Controls.Label();
            canvas.Children.Add(elem);
            elem.Content = info.Text;
            elem.Foreground = new SolidColorBrush(info.TextColor);
            elem.FontSize = (fontSize + info.FontAdjust) * scale;
            elem.Width = (int)(canvas.ActualWidth * wp);
            elem.Height = (int)(canvas.ActualHeight * hp);
            elem.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            elem.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            Canvas.SetLeft(elem, (int)(canvas.ActualWidth * xp));
            Canvas.SetTop(elem, (int)(canvas.ActualHeight * yp));

            if (info.Location == ScreamOffConfig.FontLocation.Assets)
            {
                string fontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
                var fontFamily = new System.Windows.Media.FontFamily(new Uri($"file:///{fontPath}/"), $"./#{info.Font}");
                elem.FontFamily = fontFamily;
            }
            else
            {
                elem.FontFamily = new System.Windows.Media.FontFamily(info.Font);
            }
            return elem;
        }

        private static System.Windows.Controls.Image CreateImageElement(double xp, double yp, ScreamOffConfig.ImageOption info, Canvas canvas, double scale)
        {
            var elem = new System.Windows.Controls.Image();
            BitmapImage bitmap = new();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(info.File, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            elem.Source = bitmap;
            RenderOptions.SetBitmapScalingMode(elem, BitmapScalingMode.HighQuality);
            canvas.Children.Add(elem);

            elem.Width = (int)(bitmap.PixelWidth * scale);
            elem.Height = (int)(bitmap.PixelHeight * scale);
            Canvas.SetLeft(elem, (int)(canvas.ActualWidth * xp));
            Canvas.SetTop(elem, (int)(canvas.ActualHeight * yp));
            return elem;
        }
    }
}

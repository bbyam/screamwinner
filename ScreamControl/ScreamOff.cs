using ScreamView;
using ScreamViewLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;

namespace ScreamControl
{
    internal class ScreamOff : IAnimation
    {
        private ScreamOffConfig _config;
        private Meter _meter = null!;
        private Meter _meter2 = null!;
        private Meter _meter3 = null!;
        private IAudioSampler _audioSampler;
        private ScreamViewLib.ScreamView _screamView;

        private double _scaleFactor = 1.0;

        private bool _audioRunning = false;

        private System.Windows.Controls.Label _title = null!;
        private System.Windows.Controls.Label _option1 = null!;
        private System.Windows.Controls.Label _option2 = null!;
        private System.Windows.Controls.Label _option3 = null!;

        private Color OutlineColor = Color.FromArgb(255, 200, 200, 200);
        private Color BackgroundColor = Color.FromArgb(255, 60, 60, 60);
        private Color SamplingColor = Color.FromArgb(255, 200, 180, 0);
        private Color SamplingMaxlineColor = Color.FromArgb(255, 255, 255, 0);
        private Color SamplingMaxBGColor = Color.FromArgb(255, 100, 90, 0);

        private Color WinnerColor = Color.FromArgb(255, 0, 255, 0);
        private Color LoserColor = Color.FromArgb(255, 255, 0, 0);

        const double MeterRestValue = 0.01;

        public ScreamOff(IAudioSampler audioSampler, ScreamViewLib.ScreamView screamView, ScreamOffConfig config)
        {
            _config = config;
            _audioSampler = audioSampler;

            _screamView = screamView;

            _scaleFactor = _screamView.Width / 3840.0; // Base scale factor on a 3840x2160p screen

            _meter = new(0.2, 0.75, MeterRestValue, .1, .95);
            _meter.AddToCanvas(_screamView);
            _meter2 = new(0.2, 0.75, MeterRestValue, .4, .95);
            _meter2.AddToCanvas(_screamView);
            _meter3 = new(0.2, 0.75, MeterRestValue, .7, .95);
            _meter3.AddToCanvas(_screamView);

            SetInitialColors();

            _title = CreateTextElement(0.2, 0.0, 0.6, 0.1, config.Title, 150, _screamView, _scaleFactor);
            _option1 = CreateTextElement(0.1, 0.1, 0.2, 0.1, config.Option1, 96, _screamView, _scaleFactor);
            _option2 = CreateTextElement(0.4, 0.1, 0.2, 0.1, config.Option2, 96, _screamView, _scaleFactor);
            _option3 = CreateTextElement(0.7, 0.1, 0.2, 0.1, config.Option3, 96, _screamView, _scaleFactor);

            foreach (var image in config.Images)
            {
                CreateImageElement(image.Left, image.Top, image, _screamView, _scaleFactor);
            }
        }

        private void SetInitialColors()
        {
            _meter.SetColor(SamplingColor, OutlineColor, BackgroundColor, SamplingMaxlineColor, SamplingMaxBGColor);
            _meter2.SetColor(SamplingColor, OutlineColor, BackgroundColor, SamplingMaxlineColor, SamplingMaxBGColor);
            _meter3.SetColor(SamplingColor, OutlineColor, BackgroundColor, SamplingMaxlineColor, SamplingMaxBGColor);
        }

        public static System.Windows.Controls.Label CreateTextElement(double xp, double yp, double wp, double hp, ScreamOffConfig.TextOption info, double fontSize, Canvas canvas, double scale)
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

        public static System.Windows.Controls.Image CreateImageElement(double xp, double yp, ScreamOffConfig.ImageOption info, Canvas canvas, double scale)
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


        int _meterAnimateCount = 0;
        double _nextMeterValue = 0.0;
        double _prevMeterValue = 0.0;
        public void OnAnimate()
        {
            if (_audioRunning)
            {
                // This assumes OnAnimate is called 60 times per second
                if (++_meterAnimateCount >= 4)
                {
                    _meter.MeterValue = _nextMeterValue;
                    _prevMeterValue = _nextMeterValue;
                    _nextMeterValue = _audioSampler.GetMaxPeak();
                    _meterAnimateCount = 0;
                }
                else
                {
                    // Smoothly animate towards the next meter value
                    var delta = _nextMeterValue - _prevMeterValue;
                    var step = delta * (_meterAnimateCount / 4.0);
                    _meter.MeterValue = _prevMeterValue + step;
                }
            }



            // TEMPORARY: Testing multiple meters
            if (_audioRunning)
            {
                var nextValue = _meter2.MeterValue + 0.009;
                if (nextValue > 0.8)
                    nextValue = 0.0;
                _meter2.MeterValue = nextValue;
            }
            if (_audioRunning)
            {
                var nextValue = _meter3.MeterValue + 0.008;
                if (nextValue > 0.6)
                    nextValue = 0.0;
                _meter3.MeterValue = nextValue;
            }
        }

        public void Start()
        {
            _audioSampler.Start();
            _audioRunning = true;

            SetInitialColors();

            _meter.BeginMaxValue();
            _meter2.BeginMaxValue();
            _meter3.BeginMaxValue();
        }

        public void Stop()
        {
            _audioSampler.Stop();
            _audioRunning = false;

            _meter.MeterValue = _meter.MaxValue;
            _meter3.MeterValue = _meter3.MaxValue;
            _meter2.MeterValue = _meter2.MaxValue;
            var meter1Win = _meter.MaxValue >= _meter2.MaxValue && _meter.MaxValue >= _meter3.MaxValue;
            var meter2Win = _meter2.MaxValue >= _meter.MaxValue && _meter2.MaxValue >= _meter3.MaxValue;
            var meter3Win = _meter3.MaxValue >= _meter.MaxValue && _meter3.MaxValue >= _meter2.MaxValue;
            var meter1FinalColor = meter1Win ? WinnerColor : LoserColor;
            var meter2FinalColor = meter2Win ? WinnerColor : LoserColor;
            var meter3FinalColor = meter3Win ? WinnerColor : LoserColor;
            _meter.SetColor(meter1FinalColor, OutlineColor, BackgroundColor, meter1FinalColor, meter1FinalColor);
            _meter2.SetColor(meter2FinalColor, OutlineColor, BackgroundColor, meter2FinalColor, meter2FinalColor);
            _meter3.SetColor(meter3FinalColor, OutlineColor, BackgroundColor, meter3FinalColor, meter3FinalColor);

            _meter.EndMaxValue();
            _meter2.EndMaxValue();
            _meter3.EndMaxValue();
        }
    }
}

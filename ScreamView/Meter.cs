using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Color = System.Windows.Media.Color;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;
using Brushes = System.Windows.Media.Brushes;

namespace ScreamView
{
    public class Meter
    {
        public Meter(double widthPercent, double heightPercent, double initialValue, double locationX, double locationY)
        {
            WidthPercent = widthPercent;
            HeightPercent = heightPercent;
            MeterValue = initialValue;
            LocationX = locationX;
            LocationY = locationY;

            _fillRect.Fill = new SolidColorBrush(Color.FromArgb(50, 220, 220, 220));
            _fillRect.Stroke = Brushes.Transparent;
            _fillRect.StrokeThickness = 0;

            _outlineRect.Fill = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
            _outlineRect.Stroke = Brushes.Gray;
            _outlineRect.StrokeThickness = OutlineWidth;

            _maxValueRect.Fill = Brushes.White;
            _maxValueRect.Stroke = Brushes.Transparent;
            _maxValueRect.StrokeThickness = 0;

            _maxValueBGRect.Fill = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100));
            _maxValueBGRect.Stroke = Brushes.Transparent;
            _maxValueBGRect.StrokeThickness = 0;
        }

        public double WidthPercent
        {
            get { return _widthPercent; }
            set
            {
                if (value < 0.0 || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(WidthPercent), "WidthPercent must be between 0.0 and 1.0");
                }
                _widthPercent = value;
                GenerateRect();
            }
        }

        public double HeightPercent
        {
            get { return _heightPercent; }
            set
            {
                if (value < 0.0 || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(HeightPercent), "HeightPercent must be between 0.0 and 1.0");
                }
                _heightPercent = value;
                GenerateRect();
            }
        }

        public double MeterValue
        {
            get { return _meterValue; }
            set
            {
                if (value < 0.0 || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(MeterValue), "MeterValue must be between 0.0 and 1.0");
                }
                _meterValue = value;
                if (_maxValueActive && _meterValue > _maxValue)
                {
                    _maxValue = _meterValue;
                }
                GenerateRect();
            }
        }

        public double MaxValue
        {
            get { return _maxValue; }
            private set { }
        }

        public double LocationX
        {
            get { return _locationX; }
            set
            {
                if (value < 0.0 || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(MeterValue), "LocationX must be between 0.0 and 1.0");
                }
                _locationX = value;
                GenerateRect();
            }
        }

        public double LocationY
        {
            get { return _locationY; }
            set
            {
                if (value < 0.0 || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(MeterValue), "LocationY must be between 0.0 and 1.0");
                }
                _locationY = value;
                GenerateRect();
            }
        }

        public void SetColor(Color fill, Color outline, Color bg, Color maxline, Color maxbox)
        {
            _fillRect.Fill = new SolidColorBrush(Color.FromArgb(255, fill.R, fill.G, fill.B));

            _outlineRect.Fill = new SolidColorBrush(Color.FromArgb(255, bg.R, bg.G, bg.B));
            _outlineRect.Stroke = new SolidColorBrush(Color.FromArgb(255, outline.R, outline.G, outline.B));

            _maxValueRect.Fill = new SolidColorBrush(Color.FromArgb(255, maxline.R, maxline.G, maxline.B));

            _maxValueBGRect.Fill = new SolidColorBrush(Color.FromArgb(255, maxbox.R, maxbox.G, maxbox.B));
        }

        public void BeginMaxValue()
        {
            if (!_maxValueActive)
            {
                _maxValueActive = true;
                _maxValue = MeterValue;
                if (_canvas != null)
                {
                    _canvas.Children.Add(_maxValueRect);
                    _canvas.Children.Add(_maxValueBGRect);
                    Canvas.SetZIndex(_maxValueRect, Canvas.GetZIndex(_fillRect) + 1);
                    Canvas.SetZIndex(_maxValueBGRect, Canvas.GetZIndex(_fillRect) - 1);
                    GenerateRect();
                }
            }
        }

        public void EndMaxValue()
        {
            if (_maxValueActive)
            {
                _maxValueActive = false;
                if (_canvas != null)
                {
                    _canvas.Children.Remove(_maxValueRect);
                    _canvas.Children.Remove(_maxValueBGRect);
                }
                _maxValue = 0.0;
            }
        }

        const int OutlineWidth = 4;

        private double _widthPercent = 0.1;
        private double _heightPercent = 0.5;
        private double _locationX = 0.0;
        private double _locationY = 0.0;

        private double _meterValue = 0.0;
        private double _maxValue = 0.0;
        private bool _maxValueActive = false;

        private System.Windows.Shapes.Rectangle _fillRect = new();
        private System.Windows.Shapes.Rectangle _outlineRect = new();
        private System.Windows.Shapes.Rectangle _maxValueRect = new();
        private System.Windows.Shapes.Rectangle _maxValueBGRect = new();
        private Canvas? _canvas = null;

        public void AddToCanvas(Canvas canvas)
        {
            if (_canvas != null)
                RemoveFromCanvas();

            _canvas = canvas;
            _canvas.Children.Add(_outlineRect);
            _canvas.Children.Add(_fillRect);
            Canvas.SetZIndex(_outlineRect, -1);
            GenerateRect();
        }

        public void RemoveFromCanvas()
        {
            if (_canvas == null)
                return;

            _canvas.Children.Remove(_fillRect);
            _canvas.Children.Remove(_outlineRect);
            _canvas.Children.Remove(_maxValueRect);
            _canvas.Children.Remove(_maxValueBGRect);
            _canvas = null;
        }

        private void GenerateRect()
        {
            if (_canvas == null)
                return;

            _outlineRect.Width = (int)_canvas.ActualWidth * WidthPercent;
            _outlineRect.Height = (int)_canvas.ActualHeight * HeightPercent;

            _fillRect.Width = (int)_outlineRect.Width - (OutlineWidth * 2);
            _fillRect.Height = (int)((_outlineRect.Height - (OutlineWidth * 2)) * MeterValue);

            _maxValueRect.Width = _fillRect.Width;
            _maxValueRect.Height = OutlineWidth;

            _maxValueBGRect.Width = (int)_outlineRect.Width - (OutlineWidth * 2);
            _maxValueBGRect.Height = (int)((_outlineRect.Height - (OutlineWidth * 2)) * _maxValue);

            var locXFill = (int)(_canvas.ActualWidth * LocationX + OutlineWidth);
            var locYFill = (int)(_canvas.ActualHeight * LocationY) - (int)_fillRect.Height - OutlineWidth;
            Canvas.SetLeft(_fillRect, locXFill);
            Canvas.SetTop(_fillRect, locYFill);

            var locXOutline = (int)(_canvas.ActualWidth * LocationX);
            var locYOutline = (int)_canvas.ActualHeight * LocationY - (int)_outlineRect.Height;
            Canvas.SetLeft(_outlineRect, locXOutline);
            Canvas.SetTop(_outlineRect, locYOutline);

            if (_maxValueActive)
            {
                var locYMaxValue = (int)(_canvas.ActualHeight * LocationY) - (int)((_outlineRect.Height - (OutlineWidth * 2)) * _maxValue) - OutlineWidth;
                Canvas.SetLeft(_maxValueRect, locXFill);
                Canvas.SetTop(_maxValueRect, locYMaxValue);

                var locYMaxValueBG = (int)(_canvas.ActualHeight * LocationY) - (int)_maxValueBGRect.Height - OutlineWidth;
                Canvas.SetLeft(_maxValueBGRect, locXFill);
                Canvas.SetTop(_maxValueBGRect, locYMaxValueBG);
            }
        }
    }
}

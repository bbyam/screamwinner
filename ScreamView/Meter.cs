using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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

            // TODO: Temporary
            _rect.Fill = System.Windows.Media.Brushes.Green;
            _rect.Stroke = System.Windows.Media.Brushes.DarkGreen;
            _rect.StrokeThickness = 4;
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
                GenerateRect();
            }
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

        private double _widthPercent = 0.1;
        private double _heightPercent = 0.5;
        private double _meterValue = 0.0;
        private double _locationX = 0.0;
        private double _locationY = 0.0;

        private System.Windows.Shapes.Rectangle _rect = new();
        private Canvas? _canvas = null;

        public void AddToCanvas(Canvas canvas)
        {
            if (_canvas != null)
                RemoveFromCanvas();

            _canvas = canvas;
            _canvas.Children.Add(_rect);
            GenerateRect();
        }

        public void RemoveFromCanvas()
        {
            if (_canvas == null)
                return;

            _canvas.Children.Remove(_rect);
            _canvas = null;
        }

        private void GenerateRect()
        {
            if (_canvas == null)
                return;

            _rect.Width = (int)_canvas.ActualWidth * WidthPercent;

            _rect.Height = (int)_canvas.ActualHeight * HeightPercent * MeterValue;

            var locX = (int)_canvas.ActualWidth * LocationX;
            var locY = (int)(_canvas.ActualHeight * LocationY) - (int)_rect.Height;
            Canvas.SetLeft(_rect, locX);
            Canvas.SetTop(_rect, locY);
        }
    }
}

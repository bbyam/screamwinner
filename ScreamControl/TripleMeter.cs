using ScreamView;
using ScreamViewLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Color = System.Windows.Media.Color;

namespace ScreamControl
{
    internal class TripleMeter
    {
        private Canvas _canvas = null!;
        private Meter _meter = null!;
        private Meter _meter2 = null!;
        private Meter _meter3 = null!;

        private Color OutlineColor = Color.FromArgb(255, 200, 200, 200);
        private Color BackgroundColor = Color.FromArgb(255, 60, 60, 60);
        private Color SamplingColor = Color.FromArgb(255, 200, 180, 0);
        private Color SamplingMaxlineColor = Color.FromArgb(255, 255, 255, 0);
        private Color SamplingMaxBGColor = Color.FromArgb(255, 100, 90, 0);

        private Color WinnerColor = Color.FromArgb(255, 0, 255, 0);
        private Color LoserColor = Color.FromArgb(255, 255, 0, 0);

        const double MeterRestValue = 0.01;


        public TripleMeter()
        {
        }

        public void CreateOnCanvas(Canvas canvas)
        {
            _canvas = canvas;
            _meter = new(0.2, 0.75, MeterRestValue, .1, .95);
            _meter.AddToCanvas(_canvas);
            _meter2 = new(0.2, 0.75, MeterRestValue, .4, .95);
            _meter2.AddToCanvas(_canvas);
            _meter3 = new(0.2, 0.75, MeterRestValue, .7, .95);
            _meter3.AddToCanvas(_canvas);

            SetInitialColors();
        }

        public void ReceiveValue(double value)
        {
            _meter.MeterValue = value;

            var nextValue = _meter2.MeterValue + 0.009;
            if (nextValue > 0.8)
                nextValue = 0.0;
            _meter2.MeterValue = nextValue;

            nextValue = _meter3.MeterValue + 0.008;
            if (nextValue > 0.6)
                nextValue = 0.0;
            _meter3.MeterValue = nextValue;
        }

        public void Start()
        {
            SetInitialColors();

            _meter.BeginMaxValue();
            _meter2.BeginMaxValue();
            _meter3.BeginMaxValue();
        }

        public void Stop()
        {

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

        private void SetInitialColors()
        {
            _meter.SetColor(SamplingColor, OutlineColor, BackgroundColor, SamplingMaxlineColor, SamplingMaxBGColor);
            _meter2.SetColor(SamplingColor, OutlineColor, BackgroundColor, SamplingMaxlineColor, SamplingMaxBGColor);
            _meter3.SetColor(SamplingColor, OutlineColor, BackgroundColor, SamplingMaxlineColor, SamplingMaxBGColor);
        }
    }
}

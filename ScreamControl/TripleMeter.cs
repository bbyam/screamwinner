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
        private Meter[] _meters = new Meter[3];

        private Color OutlineColor = Color.FromArgb(255, 200, 200, 200);
        private Color BackgroundColor = Color.FromArgb(255, 60, 60, 60);
        private Color SamplingColor = Color.FromArgb(255, 200, 180, 0);
        private Color SamplingMaxlineColor = Color.FromArgb(255, 255, 255, 0);
        private Color SamplingMaxBGColor = Color.FromArgb(255, 100, 90, 0);

        private Color WinnerColor = Color.FromArgb(255, 0, 255, 0);
        private Color LoserColor = Color.FromArgb(255, 255, 0, 0);

        private int _targetMeter = 0;

        const double MeterRestValue = 0.01;

        public TripleMeter()
        {
        }

        public void CreateOnCanvas(Canvas canvas)
        {
            _canvas = canvas;
            _meters[0] = new(0.2, 0.75, MeterRestValue, .1, .95);
            _meters[0].AddToCanvas(_canvas);
            _meters[1] = new(0.2, 0.75, MeterRestValue, .4, .95);
            _meters[1].AddToCanvas(_canvas);
            _meters[2] = new(0.2, 0.75, MeterRestValue, .7, .95);
            _meters[2].AddToCanvas(_canvas);

            SetInitialColors();
        }

        public void ReceiveValue(double value)
        {
            _meters[_targetMeter].MeterValue = value;
        }

        public void Start(int targetMeter)
        {
            _targetMeter = targetMeter;

            _meters[_targetMeter].BeginMaxValue();
        }

        public int DetermineWinner()
        {
            _meters[0].MeterValue = _meters[0].MaxValue;
            _meters[2].MeterValue = _meters[2].MaxValue;
            _meters[1].MeterValue = _meters[1].MaxValue;
            var meter1Win = _meters[0].MaxValue >= _meters[1].MaxValue && _meters[0].MaxValue >= _meters[2].MaxValue;
            var meter2Win = !meter1Win && _meters[1].MaxValue >= _meters[0].MaxValue && _meters[1].MaxValue >= _meters[2].MaxValue;
            var meter3Win = !meter1Win && !meter2Win && _meters[2].MaxValue >= _meters[0].MaxValue && _meters[2].MaxValue >= _meters[1].MaxValue;
            var meter1FinalColor = meter1Win ? WinnerColor : LoserColor;
            var meter2FinalColor = meter2Win ? WinnerColor : LoserColor;
            var meter3FinalColor = meter3Win ? WinnerColor : LoserColor;
            _meters[0].SetColor(meter1FinalColor, OutlineColor, BackgroundColor, meter1FinalColor, meter1FinalColor);
            _meters[1].SetColor(meter2FinalColor, OutlineColor, BackgroundColor, meter2FinalColor, meter2FinalColor);
            _meters[2].SetColor(meter3FinalColor, OutlineColor, BackgroundColor, meter3FinalColor, meter3FinalColor);

            _meters[0].EndMaxValue();
            _meters[1].EndMaxValue();
            _meters[2].EndMaxValue();

            if (meter1Win)
                return 0;
            if (meter2Win)
                return 1;
            return 2;
        }

        private void SetInitialColors()
        {
            _meters[0].SetColor(SamplingColor, OutlineColor, BackgroundColor, SamplingMaxlineColor, SamplingMaxBGColor);
            _meters[1].SetColor(SamplingColor, OutlineColor, BackgroundColor, SamplingMaxlineColor, SamplingMaxBGColor);
            _meters[2].SetColor(SamplingColor, OutlineColor, BackgroundColor, SamplingMaxlineColor, SamplingMaxBGColor);
        }
    }
}

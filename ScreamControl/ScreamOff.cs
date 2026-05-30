using ScreamView;
using ScreamViewLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ScreamControl
{
    internal class ScreamOff : IAnimation
    {
        private ScreamOffConfig _config;
        private IAudioSampler _audioSampler;
        private ScreamViewLib.ScreamView _screamView;
        private readonly double _scaleFactor = 1.0;

        private Header _header = null!;

        private TripleMeter _meters = null!;

        private List<IAnimation> _effects = new();

        private System.Windows.Shapes.Rectangle _blackCover = new();

        private bool _audioRunning = false;

        public ScreamOff(IAudioSampler audioSampler, ScreamViewLib.ScreamView screamView, ScreamOffConfig config)
        {
            _config = config;
            _audioSampler = audioSampler;

            _screamView = screamView;

            _meters = new();
            _meters.CreateOnCanvas(_screamView);

            _scaleFactor = _screamView.Width / 3840.0; // Base scale factor on a 3840x2160p screen

            _header = new(_scaleFactor, _config);
            _header.CreateOnCanvas(_screamView);

            _blackCover.Fill = new SolidColorBrush(Colors.Black);
            _blackCover.Stroke = System.Windows.Media.Brushes.Transparent;
            _blackCover.StrokeThickness = 0;
            _screamView.Children.Add(_blackCover);
            _blackCover.Width = _screamView.ActualWidth;
            _blackCover.Height = _screamView.ActualHeight;
            Canvas.SetLeft(_blackCover, 0);
            Canvas.SetTop(_blackCover, 0);
            Canvas.SetZIndex(_blackCover, 10000);
        }

        public void ReceiveEvent(ScreamEvents screamEvent)
        {
            switch (screamEvent)
            {
                case ScreamEvents.Begin:
                    if (_blackCover.Opacity > 0)
                    {
                        _showing = true;
                    }
                    _scream1Sampled = false;
                    _scream2Sampled = false;
                    _scream3Sampled = false;
                    _meterFade = false;
                    _finish = false;
                    _finishDelay = 0;
                    break;

                case ScreamEvents.StartScream1:
                    _audioSampler.Start();
                    _audioRunning = true;
                    _meters.Start(0);
                    _scream1Sampled = true;
                    break;

                case ScreamEvents.StartScream2:
                    _audioSampler.Start();
                    _audioRunning = true;
                    _meters.Start(1);
                    _scream2Sampled = true;
                    break;

                case ScreamEvents.StartScream3:
                    _audioSampler.Start();
                    _audioRunning = true;
                    _meters.Start(2);
                    _scream3Sampled = true;
                    break;

                case ScreamEvents.EndScream:
                    _meterFadeValue = Math.Min(_nextMeterValue, _prevMeterValue);
                    _meterFade = true;
                    _audioRunning = false;
                    _audioSampler.Stop();
                    _meterAnimateCount = 0;
                    _nextMeterValue = 0.0;
                    _prevMeterValue = 0.0;
                    break;

                default:
                    break;
            }

            foreach (var effect in _effects)
            {
                effect.ReceiveEvent(screamEvent);
            }
        }

        int _meterAnimateCount = 0;
        double _nextMeterValue = 0.0;
        double _prevMeterValue = 0.0;
        bool _showing = false;
        bool _meterFade = false;
        double _meterFadeValue = 0.0;

        bool _scream1Sampled = false;
        bool _scream2Sampled = false;
        bool _scream3Sampled = false;
        bool _finish = false;
        int _finishDelay = 0;

        public bool IsDead()
        {
            return false;
        }

        public void OnAnimate()
        {
            if (_showing)
            {
                var newOpacity = _blackCover.Opacity - 0.016667;
                if (newOpacity <= 0)
                {
                    newOpacity = 0;
                    _showing = false;
                }
                _blackCover.Opacity = newOpacity;
            }

            if (_audioRunning)
            {
                // This assumes OnAnimate is called 60 times per second
                if (++_meterAnimateCount >= 4)
                {
                    _meters.ReceiveValue(_nextMeterValue);
                    _prevMeterValue = _nextMeterValue;
                    _nextMeterValue = _audioSampler.GetMaxPeak();
                    _meterAnimateCount = 0;
                }
                else
                {
                    // Smoothly animate towards the next meter value
                    var delta = _nextMeterValue - _prevMeterValue;
                    var step = delta * (_meterAnimateCount / 4.0);
                    var newValue = _prevMeterValue + step;
                    _meters.ReceiveValue(newValue);
                }
            }
            else if (_meterFade)
            {
                _meterFadeValue -= 0.02;
                if (_meterFadeValue <= 0)
                {
                    _meterFadeValue = 0;
                    _meterFade = false;
                    if (_scream1Sampled && _scream2Sampled && _scream3Sampled)
                    {
                        _finish = true;
                        _finishDelay = 0;
                    }
                }
                _meters.ReceiveValue(_meterFadeValue);
            }

            if (_finish)
            {
                if (++_finishDelay >= 30)
                {
                    _finish = false;
                    var winner = _meters.DetermineWinner();
                    var winnerOffet = -1 + winner;
                    _effects.Add(new ConfettiBurst(_screamView, 0.5 + (0.3 * winnerOffet), 0.5, _scaleFactor));
                }
            }

            foreach (var effect in _effects)
            {
                effect.OnAnimate();
            }
            _effects.RemoveAll(e => e.IsDead());
        }
    }
}

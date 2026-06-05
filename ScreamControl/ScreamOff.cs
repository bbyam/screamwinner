using ScreamView;
using ScreamViewLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
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
                    _activeSlot = 0;
                    _audioSampler.Start();
                    _audioRunning = true;
                    _meters.Start(0);
                    _scream1Sampled = true;
                    break;

                case ScreamEvents.StartScream2:
                    _activeSlot = 1;
                    _audioSampler.Start();
                    _audioRunning = true;
                    _meters.Start(1);
                    _scream2Sampled = true;
                    break;

                case ScreamEvents.StartScream3:
                    _activeSlot = 2;
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
                    _nextMeterValue = CheatTheWinner();
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
                    if (_config.WinSound != null)
                    {
                        using (SoundPlayer player = new SoundPlayer(_config.WinSound))
                        {
                            player.Play();
                        }
                    }
                }
            }

            foreach (var effect in _effects)
            {
                effect.OnAnimate();
            }
            _effects.RemoveAll(e => e.IsDead());
        }

        private int _activeSlot = 0;
        private double _winnerSample = 0.0;
        private double _loserSample = 0.0;
        private double _cheatMultiplier = 1.0;
        public double CheatTheWinner()
        {
            var currSample = _audioSampler.GetMaxPeak();
            // Don't let any bar reach 100
            if (currSample >= 0.9)
            {
                var factor = (currSample - 0.9) * 10;
                currSample = 0.9 + 0.05 * factor;
            }

            if (_config.WinnerBias == -1)
                return currSample;

            // If the cheated winner is after this slot, reduce all samples to 70% and record the highest value seen as loserSample
            if (_activeSlot < _config.WinnerBias)
            {
                currSample *= 0.7;
                _loserSample = Math.Max(currSample, _loserSample);
            }
            // If this is the cheated winner slot, multiply the sample by the cheat multiplier
            // Record the highest value as winnerSample
            // Slowly increase the cheat multiplier as long as the winnerSample is below the loserSample, up to a maximum of 1.43 (so that a 70% sample can be boosted to 100%)
            if (_activeSlot == _config.WinnerBias)
            {
                currSample *= _cheatMultiplier;
                if (currSample > 1.0)
                    currSample = 1.0;
                _winnerSample = Math.Max(currSample, _winnerSample);
                if (_winnerSample < _loserSample)
                {
                    _cheatMultiplier = Math.Min(_cheatMultiplier + 0.0179, 1.43);
                }
            }
            // If this slot is after the cheated winner, keep it below the winner
            if (_activeSlot > _config.WinnerBias)
            {
                currSample *= (_winnerSample * 0.95);
            }

            return currSample;
        }
    }
}

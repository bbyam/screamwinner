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

namespace ScreamControl
{
    internal class ScreamOff : IAnimation
    {
        private ScreamOffConfig _config;
        private IAudioSampler _audioSampler;
        private ScreamViewLib.ScreamView _screamView;

        private Header _header = null!;

        private TripleMeter _meters = null!;

        private bool _audioRunning = false;

        public ScreamOff(IAudioSampler audioSampler, ScreamViewLib.ScreamView screamView, ScreamOffConfig config)
        {
            _config = config;
            _audioSampler = audioSampler;

            _screamView = screamView;

            _meters = new();
            _meters.CreateOnCanvas(_screamView);

            var scaleFactor = _screamView.Width / 3840.0; // Base scale factor on a 3840x2160p screen

            _header = new(scaleFactor, _config);
            _header.CreateOnCanvas(_screamView);
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
        }

        public void Start()
        {
            _audioSampler.Start();
            _audioRunning = true;

            _meters.Start();
        }

        public void Stop()
        {
            _audioSampler.Stop();
            _audioRunning = false;

            _meters.Stop();
        }
    }
}

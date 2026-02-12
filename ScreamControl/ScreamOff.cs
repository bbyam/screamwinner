using ScreamView;
using ScreamViewLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreamControl
{
    internal class ScreamOff : IAnimation
    {
        private Meter _meter = null!;
        private IAudioSampler _audioSampler;
        private ScreamViewLib.ScreamView _screamView;

        private bool _audioRunning = false;

        public ScreamOff(IAudioSampler audioSampler, ScreamViewLib.ScreamView screamView)
        {
            _audioSampler = audioSampler;
            _screamView = screamView;

            _meter = new(0.2, 0.8, 0.8, .1, .9);
            _meter.AddToCanvas(_screamView);
        }

        public void OnAnimate()
        {
            // TEMPORARY: Simulate audio input values for testing
            if (_audioRunning)
            {
                var nextValue = _meter.MeterValue + 0.01;
                if (nextValue > 1.0)
                    nextValue = 0.0;
                _meter.MeterValue = nextValue;
            }
        }

        public void Start()
        {
            _audioSampler.Start();
            _audioRunning = true;
        }

        public void Stop()
        {
            _audioSampler.Stop();
            _audioRunning = false;
        }
    }
}

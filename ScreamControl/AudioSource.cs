using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace ScreamControl
{
    internal class AudioSource : IAudioSampler
    {
        private double _maxPeak = 0.0;
        private bool _pendingReset = false;
        private WaveInEvent _waveIn = new();

        public AudioSource()
        {
            _waveIn.WaveFormat = new WaveFormat(44100, 16, 1);
            _waveIn.BufferMilliseconds = 30;
            _waveIn.DataAvailable += OnDataAvailable;
        }

        private void OnDataAvailable(object? sender, WaveInEventArgs e)
        {
            if (_pendingReset)
            {
                _maxPeak = 0.0;
                _pendingReset = false;
            }

            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                var sample = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index]);
                var sample32 = Math.Abs(sample / 32768f);
                if (sample32 > _maxPeak)
                    _maxPeak = sample32;
            }
        }

        public void Start()
        {
            _maxPeak = 0.0;
            _pendingReset = true;
            _waveIn.StartRecording();
        }

        public void Stop()
        {
            _waveIn.StopRecording();
            _maxPeak = 0.0;
            _pendingReset = true;
        }

        public void Dispose()
        {
            _waveIn.Dispose();
        }

        public void ResetPeak()
        {
            _maxPeak = 0.0;
        }

        public double GetMaxPeak()
        {
            // Flag max peak to reset on the next sample update
            _pendingReset = true;

            return _maxPeak;
        }
    }
}

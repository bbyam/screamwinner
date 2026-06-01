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
        private readonly double _multiplier;
        private double _maxPeak = 0.0;
        private bool _pendingReset = false;
        private bool _firstSample = true;
        private WaveInEvent _waveIn = new();

        public AudioSource(ScreamOffConfig config)
        {
            _waveIn.WaveFormat = new WaveFormat(44100, 16, 1);
            _waveIn.BufferMilliseconds = 30;
            _waveIn.DataAvailable += OnDataAvailable;

            int index = -1; // Default

            if (!string.IsNullOrEmpty(config.DeviceName))
            {
                index = GetDeviceNumberFromName(config.DeviceName);
                if (index == -1)
                    MessageBox.Show($"{config.DeviceName} not found. Using default microphone.", "Desired Microphone Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            _waveIn.DeviceNumber = index;
            _multiplier = config.MicMultiplier;
        }

        private void OnDataAvailable(object? sender, WaveInEventArgs e)
        {
            if (_pendingReset)
            {
                _maxPeak = 0.0;
                _pendingReset = false;
            }

            // Discard any noise from the first sample
            if (_firstSample)
            {
                _firstSample = false;
                return;
            }

            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                var sample = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index]);
                var sample32 = Math.Abs(sample / 32768f);
                if (sample32 > _maxPeak)
                    _maxPeak = sample32;
            }
        }

        private int GetDeviceNumberFromName(string name)
        {
            for (int i = 0; i < WaveInEvent.DeviceCount; i++)
            {
                var deviceInfo = WaveInEvent.GetCapabilities(i);
                if (deviceInfo.ProductName.Contains(name, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            return -1;
        }

        public void Start()
        {
            _maxPeak = 0.0;
            _pendingReset = true;
            _firstSample = true;
            _waveIn.StartRecording();
        }

        public void Stop()
        {
            _waveIn.StopRecording();
            _maxPeak = 0.0;
            _pendingReset = true;
            _firstSample = true;
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

            return Math.Clamp(_maxPeak * _multiplier, 0, 1);
        }
    }
}

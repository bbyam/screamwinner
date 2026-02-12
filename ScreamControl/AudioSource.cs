using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreamControl
{
    internal class AudioSource
    {
        private double _maxPeak = 0.0;

        public void Start()
        {

        }

        public void Stop()
        {

        }

        public void ResetPeak()
        {
            _maxPeak = 0.0;
        }

        public double GetMaxPeak()
        {
            // Return the current max peak and reset it to 0 for the next measurement period
            var currentPeak = _maxPeak;
            _maxPeak = 0.0;

            return currentPeak;
        }
    }
}

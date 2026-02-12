using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreamControl
{
    internal interface IAudioSampler
    {
        void Start();
        void Stop();
        void ResetPeak();
        double GetMaxPeak();
    }
}

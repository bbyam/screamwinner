using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace ScreamControl
{
    internal class MicrophoneInput
    {
        private async Task<double> GetMaxVolume()
        {
            WaveInEvent waveIn = new();

            double max = 0;

            // Setting the data available to capture the max volume level from the microphone input
            waveIn.DataAvailable += (s, e) => {
                float max = 0;
                for (int index = 0; index < e.BytesRecorded; index += 2)
                {
                    short sample = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index]);
                    float sample32 = Math.Abs(sample / 32768f);
                    if (sample32 > max) max = sample32;
                }
            };

            waveIn.StartRecording();
            await Task.Delay(60000); // 60 seconds
            waveIn.StopRecording();

            return max;
        }
    }
}

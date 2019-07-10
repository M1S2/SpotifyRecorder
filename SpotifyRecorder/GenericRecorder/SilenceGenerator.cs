using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCore;

namespace SpotifyRecorder.GenericRecorder
{
    //see: https://pastebin.com/9MYM3yFN
    public class SilenceGenerator : IWaveSource
    {
        public int Read(byte[] buffer, int offset, int count)
        {
            Array.Clear(buffer, offset, count);
            return count;
        }

        private readonly WaveFormat _waveFormat = new WaveFormat(44100, 16, 2);
        public WaveFormat WaveFormat
        {
            get { return _waveFormat; }
        }

        public long Position
        {
            get { return -1; }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public long Length
        {
            get { return -1; }
        }

        public bool CanSeek
        {
            get { return false; }
        }

        public void Dispose()
        {
            //do nothing
        }
    }
}

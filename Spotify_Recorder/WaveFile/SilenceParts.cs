using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify_Recorder
{
    public class SilenceParts
    {
        public double Original_Start_ms;
        public double New_Start_ms;
        public double Original_End_ms;

        public double Original_Length_ms
        {
            get
            {
                return (Original_End_ms - Original_Start_ms);
            }
        }

        public SilenceParts(double original_start_ms, double new_start_ms, double original_end_ms)
        {
            Original_Start_ms = original_start_ms;
            New_Start_ms = new_start_ms;
            Original_End_ms = original_end_ms;
        }
    }
}

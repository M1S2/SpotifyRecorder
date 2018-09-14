﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace Spotify_Recorder
{
    public static class AssemblyBuildDetails
    {
        /// <summary>
        /// Get the time of the last build of the assembly.
        /// Use this like: System.Reflection.Assembly.GetExecutingAssembly().GetLinkerTime()
        /// </summary>
        /// <param name="assembly">assembly</param>
        /// <param name="target"></param>
        /// <returns>last build time</returns>
        /// see: https://stackoverflow.com/questions/1600962/displaying-the-build-date?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa
        public static DateTime GetLinkerTime(this Assembly assembly, TimeZoneInfo target = null)
        {
            var filePath = assembly.Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;

            var buffer = new byte[2048];

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                stream.Read(buffer, 0, 2048);

            var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

            var tz = target ?? TimeZoneInfo.Local;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

            return localTime;
        }

    }
}

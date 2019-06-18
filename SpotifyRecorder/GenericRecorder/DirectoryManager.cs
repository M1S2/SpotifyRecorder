using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SpotifyRecorder.GenericRecorder
{
    public class DirectoryManager
    {
        /// <summary>
        /// This function deletes all empty folders in the given path.
        /// </summary>
        /// <param name="startFolder">The folder to start from.</param>
        /// see: https://stackoverflow.com/questions/2811509/c-sharp-remove-all-empty-subdirectories
        public static void DeleteEmptyFolders(string startFolder)
        {
            if(!Directory.Exists(startFolder)) { return; }

            foreach (string directory in Directory.GetDirectories(startFolder))
            {
                DeleteEmptyFolders(directory);
                if (Directory.GetFiles(directory).Length == 0 && Directory.GetDirectories(directory).Length == 0)
                {
                    Directory.Delete(directory, false);
                }
            }
        }
        
    }
}

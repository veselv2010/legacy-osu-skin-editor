using System;
using System.Diagnostics;

namespace SkinEditor
{
    class OsuPath
    {
        public static string GetOsuPath()
        {
            try
            {
                Process osuProcess = Process.GetProcessesByName("osu!")[0];
                string fullPath = osuProcess.MainModule.FileName.Replace("osu!.exe", "");

                return fullPath;
            }
            catch(Exception)
            {
                return null;
            }
        }
    }
}

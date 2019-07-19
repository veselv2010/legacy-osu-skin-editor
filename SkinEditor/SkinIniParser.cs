using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
namespace SkinEditor
{
    class SkinIniParser
    {
        public static Brush CurrentColour;
        public static bool IsVersionValid;
        private static readonly List<string> AllowedVersions = new List<string>{ "1.0", "1", "2.0", "2", "2.1", "2.2", "2.3", "2.4", "2.5", "latest", "User" };
        private static string SkinIniPathPrivate;
        //static List<string> SkinGeneralList = new List<string>();
        static string[] SkinGeneral;
        int SliderStyle; // 1 - peppysliders
                         // 2 - mmsliders
                         // 3 - toonsliders
                         // 4 - legacyOpenGL-only sliders

        //найти пример skin.ini со всеми параметрами
        public static string SkinIniInit(string skinpath)
        {
            SkinIniPathPrivate = skinpath + "\\skin.ini";
            SkinGeneral = SkinIniGeneral();
            return SkinIniPathPrivate;
        }

        private static string[] SkinIniGeneral() => File.ReadAllLines(SkinIniPathPrivate);

        public static string GetPropertyBySenderName(string sendername)
        {
            string temp = "";
            foreach(string elem in SkinGeneral)
            {
                if (elem.Contains(sendername + ":"))
                {
                    int IndexOfRemove = elem.IndexOf(":") + 2;
                    temp = elem.Remove(0, IndexOfRemove);
                    break;
                }
            }
            return temp;
        }

        public static string VersionValidator(string versiontext)
        {
            IsVersionValid = false;
            if (versiontext == "")
                return "enter something";

            if (!AllowedVersions.Contains(versiontext))
                return "version number is not valid";

            IsVersionValid = true;
            return "version number is ok!";
        }

        public static string[] GetToolTipLines(object sender, string[] ToolTipLines)
        {
            int i = 0;
            int j = 0;
            string[] ToReturn = new string[20]; //рандомное число

            var Sender = sender as FrameworkElement;

            foreach (string elem in ToolTipLines)
            {
                if (elem.Contains(Sender.Name))
                {
                    int IndexOfSenderName = i;
                     break;
                }
                i++;
            }
            while (ToolTipLines[i] != "")
            {
                ToReturn[j] = ToolTipLines[i];
                i++;
                j++;
            }

             return ToReturn;
        }
    }
}

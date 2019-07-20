using System.Collections.Generic;
using System.IO;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
namespace SkinEditor
{
    class SkinIniParser
    {
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
            int NeededPropertyIndex = 0;
            int ToolTipLineIndex = 0;
            string[] ToReturn = new string[20]; //рандомное число

            var Sender = sender as FrameworkElement;

            foreach (string elem in ToolTipLines)
            {
                if (elem.Contains(Sender.Name))
                {
                    int IndexOfSenderName = NeededPropertyIndex;
                     break;
                }
                NeededPropertyIndex++;
            }
            while (ToolTipLines[NeededPropertyIndex] != "")
            {
                ToReturn[ToolTipLineIndex] = ToolTipLines[NeededPropertyIndex];
                NeededPropertyIndex++;
                ToolTipLineIndex++;
            }
            return ToReturn;
        }
        public static string[] SkinIniToExport()
        {
            string[] ToExport = new string[MainWindow.SkinIniPropertiesObj.Count + 20];
            int Counter = 1;
            ToExport[0] = "[General]";
            foreach (object elem in MainWindow.SkinIniPropertiesObj)
            {
                Type ElemType = elem.GetType();
                if (ElemType.Name == "TextBox")
                {
                    var TempTextBox = elem as TextBox;
                    ToExport[Counter] = $"{TempTextBox.Name}: {TempTextBox.Text}";
                    Counter++;
                }
                if(ElemType.Name == "CheckBox")
                {
                    var TempCheckBox = elem as CheckBox;
                    ToExport[Counter] = $"{TempCheckBox.Name}: {TempCheckBox.IsChecked.Value}";
                    Counter++;
                }
                if (ElemType.Name == "Frame")
                {
                    ToExport[Counter] = "[Colours]";
                    Counter++;
                    break;
                }
            }
            for (int i = Counter - 2; i < MainWindow.SkinIniPropertiesObj.Count; i++)
            {
                var TempFrame = MainWindow.SkinIniPropertiesObj[i] as Frame;

                byte[] Colours = GetRgbColours(TempFrame.Background);

                byte r = Colours[0];
                byte g = Colours[1];
                byte b = Colours[2];

                ToExport[Counter] = $"{TempFrame.Name}: {r},{g},{b}";

                Counter++;
            }
            return ToExport;
        }
        #region rgbworker
        public static byte[] GetRgbColours(Brush Background)
        {
            byte R, G, B;
            byte[] ToReturn = new byte[3];
            string Colours = Background.ToString();
            R = Convert.ToByte(Colours[3].ToString()
                 + Colours[4].ToString(), 16);

            G = Convert.ToByte(Colours[5].ToString()
                             + Colours[6].ToString(), 16);

            B = Convert.ToByte(Colours[7].ToString()
                             + Colours[8].ToString(), 16);

            ToReturn[0] = R;
            ToReturn[1] = G;
            ToReturn[2] = B;
            return ToReturn;
        }

        public static byte[] GetRgbColours(string Background)
        {
            byte R, G, B;
            byte[] ToReturn = new byte[3];
            R = Convert.ToByte(Background[3].ToString()
                 + Background[4].ToString(), 16);

            G = Convert.ToByte(Background[5].ToString()
                             + Background[6].ToString(), 16);

            B = Convert.ToByte(Background[7].ToString()
                             + Background[8].ToString(), 16);

            ToReturn[0] = R;
            ToReturn[1] = G;
            ToReturn[2] = B;
            return ToReturn;
        }

        public static byte GetSingleChannel(string Colour)
        {
            if(Colour != "" && Colour.Length <= 3 && int.Parse(Colour) < 256)
                return Convert.ToByte(Colour);
            else
                return 0;
        }
        #endregion
    }
}

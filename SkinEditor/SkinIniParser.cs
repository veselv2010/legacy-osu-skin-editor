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
        private static string SkinIniPathPrivate;
        //static List<string> SkinGeneralList = new List<string>();
        static string[] SkinGeneral;
        string Name;
        string Author;
        string Version;
        bool SliderBallFlip;
        bool CursorRotate;
        bool CursorTrailRotate;
        bool CursorExpand;
        bool CursorCentre;
        bool LayeredHitSounds;
        int SliderBallFrames;
        bool SpinnerFadePlayfield;
        bool SpinnerNoBlink;
        bool ComboBurstRandom;
        bool AllowSliderBallTint;
        int HitCircleOverlayAboveNumer;
        int AnimationFramerate;
        string CustomComboBurstSounds;
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
                if (elem.Contains(sendername))
                {
                    int IndexOfRemove = elem.IndexOf(":") + 2;
                    temp = elem.Remove(0, IndexOfRemove);
                    break;
                }
            }
            return temp;
        }
    }
}

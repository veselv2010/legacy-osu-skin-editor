﻿//почистить код
using System;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;

namespace SkinEditor
{
    class SkinWorker
    {
        public static string DefaultSkinAbsPath = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("SkinEditor.exe", "Default");
        public static string EmptyPngPath = "Resources/empty.png";//
        public static string[] SkinPngElemArray; //todo
        public static string[] SkinJpegElemArray; //возможно, это потребуется для файлэксплорера
        public static string[] SkinJpgElemArray; //пока не уничтожать

        public static string[] ExistingSkin; // элементы в выбранном скине 

        public static string[] ExistingSkins; //названия папок от скинов в Skins
        public static void GetAllFiles(string path)
        {
            SkinPngElemArray  = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);
            SkinJpegElemArray = Directory.GetFiles(path, "*.jpeg", SearchOption.AllDirectories);
            SkinJpgElemArray  = Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories);
        }
        
        public static void GetSelectedSkinFiles(string path) => ExistingSkin = Directory.GetFiles(path, "*.png", SearchOption.TopDirectoryOnly);
        public static void GetSkinNames(string path) => ExistingSkins = Directory.GetDirectories(path);

        #region чтобы не засирать мейн
        public static BitmapImage GetImageBySenderName(string skinpath, Image sender) //все методы ниже - велосипеды собственного производства
        {
            string SkinElem;
            if (sender.Name.Contains("_Copy"))
            {
                string temp = sender.Name.Remove(sender.Name.IndexOf("_Copy"));
                SkinElem = SkinWorker.Name2FileNameConv(skinpath + "\\" + temp + ".png", false);
            }
            else
            {
                SkinElem = SkinWorker.Name2FileNameConv(skinpath + "\\" + sender.Name + ".png", false);
            }

            BitmapImage NewImage;
            try
            {
                 NewImage = new BitmapImage(new Uri(skinpath + "\\" + SkinElem));
            }
            catch (FileNotFoundException)
            {
                string uri = DefaultSkinAbsPath + "\\" + SkinElem; //уничтожить
                NewImage = new BitmapImage(new Uri(uri));
            }
            return NewImage;
        }

        public static BitmapImage GetImageByDirectPath(string path)//лень писать new Bitmapimage(new Uri())
        {
            try
            {
                return new BitmapImage(new Uri(path));
            }
            catch
            {
                return null;
            }
        }
        #endregion

        public static string Name2FileNameConv(string ImageSource, bool Is2x) //юзать это, если из куска говна нужно сделать имя файла c .png 
        {                                                                     // затестить парсинг через регекс
            int RemovableSymb = ImageSource.Length - 2;                       //есть подозрение, что это работает быстрее, чем способ с перестановкой элементов в массиве

            for (int i = ImageSource.Length - 4; i > 0; i--)
            {
                RemovableSymb--;
                if ('/' == ImageSource[i] 
                || '\\' == ImageSource[i])
                    break;
            }
            return ImageSource.Remove(0, RemovableSymb).Replace("_", "-"); 
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public static System.Windows.Media.ImageSource ImageSourceFromBitmap(System.Drawing.Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(handle,
                                                                                    IntPtr.Zero,
                                                                                    Int32Rect.Empty,
                                                                                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(handle);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SkinEditor
{
    /// <summary>
    /// Логика взаимодействия для ColourPicker.xaml
    /// </summary>
    public partial class ColourPicker : Window
    {
        private static string BackgroundColour;
        public ColourPicker(object sender)
        {
            InitializeComponent();
            BackgroundColour = SenderBackground(sender);
            ColourParser();
        }

        private void ColourParser()
        {
            string TempColour = BackgroundColour.Replace("#", "0x").Remove(8);
            //Int16 ToByteR = (Convert.ToInt16(TempColour[2]) + Convert.ToInt16(TempColour[3]));
            string ToByteB = $"{TempColour[4]}{TempColour[5]}";
            var rectinfo = RectangleColour.Fill.GetValue(Rectangle.FillProperty);
            int ColourInt32 = (Int16)TempColour[2] * (Int32)16 + (Int16)TempColour[3]; //?????????????
        }

        private string SenderBackground(object sender) => ((Frame)sender).Background.ToString();
    }
}

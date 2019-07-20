using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Text.RegularExpressions;
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
        private readonly Regex Numbers = new Regex("[^0-9]+");
        private static object Sender;
        private static Brush BackgroundColour;
        public ColourPicker(object sender)
        {
            InitializeComponent();
            BackgroundColour = SenderBackground(sender);
            Sender = sender;
            GetCurrentColor();
        }

        private void GetCurrentColor()
        {
            byte[] Colours = SkinIniParser.GetRgbColours(BackgroundColour);

            TextBoxColourR.Text = Colours[0].ToString();
            TextBoxColourG.Text = Colours[1].ToString();
            TextBoxColourB.Text = Colours[2].ToString();
            TextBoxColourHEX.Text = BackgroundColour.ToString().Remove(1, 2);
            RectangleColour.Fill = new SolidColorBrush(Color.FromRgb(Colours[0], Colours[1], Colours[2]));
        }

        private Brush SenderBackground(object sender) => ((Frame)sender).Background;

        private void TextBoxColour_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).Name.Contains("HEX"))
            {
                if (TextBoxColourHEX.Text.Length == 7)
                {
                    try
                    {
                        string HexColour = TextBoxColourHEX.Text.Insert(1, "FF");
                        byte[] Colours = SkinIniParser.GetRgbColours(HexColour);

                        TextBoxColourR.Text = Colours[0].ToString();
                        TextBoxColourG.Text = Colours[1].ToString();                       
                        TextBoxColourB.Text = Colours[2].ToString();

                        RectangleColour.Fill = new SolidColorBrush(Color.FromRgb(Colours[0], Colours[1], Colours[2]));
                        return;
                    }
                    catch (FormatException)
                    {
                        return;
                    }
                }
                return;
            }

            byte R, G, B;
            var Sender = sender as TextBox;
            if (Sender.Text != "" && int.Parse(Sender.Text) > 256)
            {
                Sender.Background = Brushes.Red;
                return;
            }
            Sender.Background = Brushes.White;

            R = SkinIniParser.GetSingleChannel(TextBoxColourR.Text);
            G = SkinIniParser.GetSingleChannel(TextBoxColourG.Text);
            B = SkinIniParser.GetSingleChannel(TextBoxColourB.Text);

            RectangleColour.Fill = new SolidColorBrush(Color.FromRgb(R, G, B));

            TextBoxColourHEX.Text = RectangleColour.Fill.ToString().Remove(1, 2);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var CurrentFrame = Sender as Frame;
            CurrentFrame.Background = RectangleColour.Fill;
        }

        private void TextBox_NumberInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Numbers.IsMatch(e.Text);
        }
    }
}

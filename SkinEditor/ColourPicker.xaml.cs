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
        private static object Sender;
        private static string BackgroundColour;
        private static byte R;
        private static byte G;
        private static byte B;
        public ColourPicker(object sender)
        {
            InitializeComponent();
            BackgroundColour = SenderBackground(sender);
            Sender = sender;
            GetCurrentColor();
        }

        private void GetCurrentColor()
        {
            R = Convert.ToByte($"{BackgroundColour[3]}" + $"{BackgroundColour[4]}", 16);
            G = Convert.ToByte($"{BackgroundColour[5]}" + $"{BackgroundColour[6]}", 16);
            B = Convert.ToByte($"{BackgroundColour[7]}" + $"{BackgroundColour[8]}", 16);
            TextBoxColourR.Text = R.ToString();
            TextBoxColourG.Text = G.ToString();
            TextBoxColourB.Text = B.ToString();
            TextBoxColourHEX.Text = BackgroundColour.Remove(1, 2);
            RectangleColour.Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
        }

        private string SenderBackground(object sender) => ((Frame)sender).Background.ToString();

        private void TextBoxColour_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).Name.Contains("HEX"))
            {
                if (TextBoxColourHEX.Text.Length == 7)
                {
                    try
                    {
                        R = Convert.ToByte($"{TextBoxColourHEX.Text[1]}" + $"{TextBoxColourHEX.Text[2]}", 16);
                        TextBoxColourR.Text = R.ToString();
                        G = Convert.ToByte($"{TextBoxColourHEX.Text[3]}" + $"{TextBoxColourHEX.Text[4]}", 16);
                        TextBoxColourG.Text = G.ToString();
                        B = Convert.ToByte($"{TextBoxColourHEX.Text[5]}" + $"{TextBoxColourHEX.Text[6]}", 16);
                        TextBoxColourB.Text = B.ToString();
                        RectangleColour.Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
                        return;
                    }
                    catch (FormatException)
                    {
                        return;
                    }
                }
                return;
            }

            var Sender = sender as TextBox;
            if (Sender.Text != "" && int.Parse(Sender.Text) > 256)
            {
                Sender.Background = Brushes.Red;
                return;
            }
            Sender.Background = Brushes.White;

            if (TextBoxColourR.Text != "" && int.Parse(TextBoxColourR.Text) < 256)
                R = Convert.ToByte(TextBoxColourR.Text);
            if (TextBoxColourG.Text != "" && int.Parse(TextBoxColourG.Text) < 256)
                G = Convert.ToByte(TextBoxColourG.Text);
            if (TextBoxColourB.Text != "" && int.Parse(TextBoxColourB.Text) < 256)
                B = Convert.ToByte(TextBoxColourB.Text);

            RectangleColour.Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
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
    }
}

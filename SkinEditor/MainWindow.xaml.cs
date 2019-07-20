using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace SkinEditor
{
    //передавать в метод GetImageBySenderName-чето-там данные о размере объекта в гриде, чтобы в дальнейшем скейлить его в методе до необходимых размеров;
    //проблема в том, что каждый объект ведет себя по-своему
    public partial class MainWindow : Window
    {
        private readonly Regex Numbers = new Regex("[^0-9]+");
        private readonly string[] ToolTipLines = File.ReadAllLines("Resources/ToolTips.txt"); //единоразовое считывание
        private int LastItem = 0; //уничтожить
        private byte R, G, B;
        private int ProgressBarFilesMax = 0;
        //public static System.Windows.Threading.DispatcherTimer Animtimer = new System.Windows.Threading.DispatcherTimer(); анимация кнопки skinscan до первого нажатия
        public static List<Image> OnScreenImages = new List<Image>(); //лист со всеми (не со всеми) объектами Image, предназначенными для экспорта
        public static List<string> OnScreenImagesNames = new List<string>(); //это нужно уничтожить в дальнейшем (используется для поиска элементов в основном листе)
        public static List<Image> GridScoreImages = new List<Image>(); //лист с Image из GridScore, нужен для "удобной" работы с ним (не перебирая ~50 элементов в _Copy)(_Score)
        public static List<object> SkinIniPropertiesObj = new List<object>();
        public MainWindow()
        {
            InitializeComponent();
            ComboBoxExistingSkin.Visibility = Visibility.Hidden;
            ProgressBarExport.Maximum = ProgressBarFilesMax;
            ComboBoxExistingSkin.Items.Add(SkinWorker.DefaultSkinAbsPath);
            ComboBoxRankingSymbol.SelectedIndex = 0;
            ranking_a.Visibility = Visibility.Hidden;
            ranking_b.Visibility = Visibility.Hidden;
            ranking_c.Visibility = Visibility.Hidden;
            ranking_d.Visibility = Visibility.Hidden;
            ranking_x.Visibility = Visibility.Hidden;
            ranking_s.Visibility = Visibility.Hidden;
            ComboBoxRankingSymbol.Items.Add("ranking_x"); //если создавать это из самой студии, то нельзя обращаться к созданным элементам
            ComboBoxRankingSymbol.Items.Add("ranking_s");
            ComboBoxRankingSymbol.Items.Add("ranking_a");
            ComboBoxRankingSymbol.Items.Add("ranking_b");
            ComboBoxRankingSymbol.Items.Add("ranking_c");
            ComboBoxRankingSymbol.Items.Add("ranking_d");
        }

        private void ButtonOsuPath_Click(object sender, RoutedEventArgs e)
        {
            string temp = OsuPath.GetOsuPath();

            if (temp == null)
                MessageBox.Show("osu isnt running", "osu path");
            else
                TextBoxOsuPath.Text = temp;
        }

        private void ButtonSkinScan_Click(object sender, RoutedEventArgs e)
        {
            string temp = TextBoxOsuPath.Text + "Skins";
            if (!(TextBoxOsuPath.Text.Contains("osu!/") || TextBoxOsuPath.Text.Contains("osu!\\")))
            {
                TextBoxOsuPath.Background = Brushes.Red;
                MessageBox.Show("Present","Exception"); //отсылка к самой крутой игре в мире
                return;
            }
            TextBoxOsuPath.Background = Brushes.Green;

            SkinWorker.GetAllFiles(temp);
            SkinWorker.GetSkinNames(temp);

            ProgressBarExport.Value = 0;
            ProgressBarExport.Maximum = SkinWorker.ExistingSkins.Length; 

            ComboBoxExistingSkin.Items.Clear(); //чтобы не заполняло дубликатами
            ComboBoxExistingSkin.Items.Add(SkinWorker.DefaultSkinAbsPath);

            foreach (string elem in SkinWorker.ExistingSkins)
            {
                ComboBoxExistingSkin.Items.Add(elem);
                ProgressBarExport.Value++;
            }
            ComboBoxExistingSkin.Visibility = Visibility.Visible;
            ComboBoxExistingSkin.SelectedIndex = 0;
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var ClickedImage = sender as Image; 
            string SenderSkinImage = SkinWorker.Name2FileNameConv(ClickedImage.Source.ToString(), false);
            LabelStatus.Content = $"{ClickedImage.Name} clicked!";
            //сделать свой файл эксплорер

            //temporary
            OpenFileDialog myDialog = new OpenFileDialog
            {
                Filter = $"ElementName ({SenderSkinImage}|{SenderSkinImage}",
                CheckFileExists = true,
                Multiselect = false
            };
            if (myDialog.ShowDialog() == true)
            {
                
                ClickedImage.Source = SkinWorker.GetImageByDirectPath(myDialog.FileName);
               
                SetCursor();//костыль
            }
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e) //для экспорта файлов -over нужно проводить по ним мышкой
        {                                                              //в идеале надо захардкодить -овер элементы, их не так много
            var HoveredImage = sender as Image;
            ImageSource CurrentHoveredImageSource = HoveredImage.Source;
            LabelStatus.Content = $"{HoveredImage.Name} hovered :)";
            HoveredImage.Source = SkinWorker.GetImageByDirectPath((HoveredImage.Source.ToString().Replace(".png", "-over.png")));
            if(HoveredImage.Source == null)
            {
                HoveredImage.Source = CurrentHoveredImageSource;
            }
            else
            {
                Image tempimage = new Image {
                    Name = HoveredImage.Name + "_over", //заменить в дальнейшем на что-то более человеческое
                    Source = HoveredImage.Source};

                if (OnScreenImages.Contains(tempimage))
                    return;
                else
                    OnScreenImages.Add(tempimage); 
            }
            HoveredImage.Opacity = 0.8;
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            var UnHoveredImage = sender as Image;
            LabelStatus.Content = $"{UnHoveredImage.Name} unhovered :(";
            try //костыль
            { 
                UnHoveredImage.Source = SkinWorker.GetImageByDirectPath(UnHoveredImage.Source.ToString().Replace("-over.png", ".png"));
            }
            catch
            {

            }
            UnHoveredImage.Opacity = 1;
        }

        private void ButtonSkinExport_Click(object sender, RoutedEventArgs e)
        {
            ProgressBarExport.Value = 0;
            File.WriteAllLines("export\\skin.ini", SkinIniParser.SkinIniToExport());
            Directory.CreateDirectory("export");
            foreach (Image elem in OnScreenImages)
            {
                File.Copy(elem.Source.ToString().Remove(0, 8), "export\\" + SkinWorker.Name2FileNameConv(elem.Source.ToString(), false), true);
                ZipExporter.AddFileToZip("exportzip.osk", elem.Source.ToString().Remove(0, 8)); //it just works
                ProgressBarExport.Value++; //factory new
            }
            ZipExporter.AddFileToZip("exportzip.osk", "export\\skin.ini");
            Process.Start("export\\");
        }

        private void Image_SourceUpdated(object sender, DataTransferEventArgs e)
        {
           //юзлесс ивент
        }

        private void ComboBoxExistingSkin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxExistingSkin.SelectedValue != null)
            {
                string path = ComboBoxExistingSkin.SelectedValue as string;
                foreach (Image elem in OnScreenImages)
                {
                    elem.Source = SkinWorker.GetImageBySenderName(path, elem);
                }

                ButtonSkinIni_Click(sender, e);
                SetCursor();
            }
        }

        private void Image_Initialized(object sender, EventArgs e)
        {
            var InitImage = sender as Image;
            if (!InitImage.Name.Contains("_Copy")&& !InitImage.Name.Contains("_Score"))
            {
                OnScreenImages.Add(InitImage);
                OnScreenImagesNames.Add(InitImage.Name);
                ProgressBarFilesMax++; //factory new
                return;
            }

            if (InitImage.Name.Contains("_Score"))
            {
                GridScoreImages.Add(InitImage);
            }
        }

        private void TextBoxSkinIni_Initialized(object sender, EventArgs e)
        {
            SkinIniPropertiesObj.Add(sender);
        }

        private void Image_Loaded(object sender, RoutedEventArgs e) // добавление кода сюда приведет к увеличению времени затупа в переходах между экранами
        {

        }

        private void DebugButtonPathToSkinFile_Click(object sender, RoutedEventArgs e)
        {
            string path = DebugTextBoxPath1.Text;
            DebugTextBox.Text = SkinWorker.Name2FileNameConv(path, RadioButtonFileSize2.IsChecked.Value);
        }

        private void DebugButtonShowSources_Click(object sender, RoutedEventArgs e)
        {
            DebugTextBox.Text = "";
            foreach(Image elem in OnScreenImages)
            {
                DebugTextBox.Text += elem.Source.ToString() + "\n";
            }
        }

        private void DebugOpenFileByPath_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(DebugTextBoxPath2.Text);
        }

        private void ButtonSkinIni_Click(object sender, RoutedEventArgs e)
        {
            string SkinIniPath = SkinIniParser.SkinIniInit(ComboBoxExistingSkin.SelectedValue as string);
            foreach (object elem in SkinIniPropertiesObj)
            {
                Type temp = elem.GetType();
                if (temp.Name == "TextBox")
                    ((TextBox)elem).Text = SkinIniParser.GetPropertyBySenderName(((TextBox)elem).Name);
                else if(temp.Name == "CheckBox")
                {
                    string tempstring = SkinIniParser.GetPropertyBySenderName(((CheckBox)elem).Name);
                    if (tempstring != "")
                    {
                        int tempf = int.Parse(tempstring);
                        ((CheckBox)elem).IsChecked = Convert.ToBoolean(tempf);
                    }
                    else
                    {
                        ((CheckBox)elem).IsChecked = Convert.ToBoolean(0); //костыль
                        ((CheckBox)elem).Background = Brushes.Yellow;
                        ((CheckBox)elem).ToolTip = "skin.ini does not contain this line";
                    }
                }
                else
                {
                    string colours = SkinIniParser.GetPropertyBySenderName(((Frame)elem).Name);
                    var TempFrame = elem as Frame;
                    if (colours == "")
                    {
                        TempFrame.ToolTip = "no value to work with";
                        TempFrame.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                        TempFrame.Background.Opacity = 0.01;
                    }
                    else
                    {
                        string[] RgbColours = colours.Split(',');
                        byte r = byte.Parse(RgbColours[0]);
                        byte g = byte.Parse(RgbColours[1]);
                        byte b = byte.Parse(RgbColours[2]);
                        TempFrame.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
                        TempFrame.Background.Opacity = 1;
                        TempFrame.ToolTip = "everything is ok!";
                    }
                }

            }
            Directory.CreateDirectory("export");
            File.Copy(SkinIniPath, "export\\" + "skin.ini", true); //сделать балдежное окно с галочками при экспорте этого файла
        }

        private void RadioButtonGameOver_Checked(object sender, RoutedEventArgs e)
        {
            pause_overlay.Visibility = Visibility.Hidden;
            fail_background.Visibility = Visibility.Visible;
            pause_continue.Visibility = Visibility.Hidden;
        }

        private void RadioButtonPause_Checked(object sender, RoutedEventArgs e)
        {
            pause_overlay.Visibility = Visibility.Visible;
            fail_background.Visibility = Visibility.Hidden;
            pause_continue.Visibility = Visibility.Visible;
        }


        private void ComboBoxRankingSymbol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LabelStatus.Content = $"{ComboBoxRankingSymbol.SelectedValue} visibility changed!";
            int OnScreenIndex = OnScreenImagesNames.IndexOf(ComboBoxRankingSymbol.SelectedValue as string);
            if (LastItem != 0)
            {
                OnScreenImages[LastItem].Visibility = Visibility.Hidden;
                OnScreenImages[OnScreenIndex].Visibility = Visibility.Visible;
                LastItem = OnScreenIndex;
                return;
            }
            if (OnScreenIndex != -1 && LastItem == 0)
            {
                OnScreenImages[OnScreenIndex].Visibility = Visibility.Visible;
                LastItem = OnScreenIndex;
                return;
            }
        }

        private void TextBoxRankingScore_TextChanged(object sender, TextChangedEventArgs e) //фича из разряда юзлесс
        {                                                                                   //потом надо это разгрузить, написание цифр в ранкедскоре не s m o o t h (в дебаге)
                                                                                        //затестить регекс
            char[] tempchar = new char[8];
            for (int j = 0; j < 8; j++)
            {
                try //дорого быстро эффективно
                {
                    tempchar[j] = TextBoxRankingScore.Text[j];
                }
                catch (IndexOutOfRangeException)
                {
                    tempchar[j] = ' ';
                }
            }
            
            if (ComboBoxExistingSkin.SelectedValue != null)
            {
                int ElementIndex = 0;
                foreach (char elem in tempchar)
                {
                    if (elem == ' ')
                    {
                        GridScoreImages[ElementIndex].Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        Binding tempbind = new Binding();
                        tempbind.ElementName = $"score_{elem}";
                        tempbind.Path = new PropertyPath("Source");
                        tempbind.Mode = BindingMode.TwoWay;

                        GridScoreImages[ElementIndex].SetBinding(Image.SourceProperty, tempbind);
                        GridScoreImages[ElementIndex].Visibility = Visibility.Visible;
                        ElementIndex++;
                    }
                }
            }
        }

        private void DebugButtonScorebarBackSizes_Click(object sender, RoutedEventArgs e) //выкупить изменение размера рендера пикчи в пикселях
        {
            DebugTextBox.Text = $"RenderSize.Height = {OnScreenImages[OnScreenImagesNames.IndexOf("scorebar_bg")].RenderSize.Height}\n" +
                                $"RenderSize.Width = {OnScreenImages[OnScreenImagesNames.IndexOf("scorebar_bg")].RenderSize.Width}\n" +
                                $"Width = {OnScreenImages[OnScreenImagesNames.IndexOf("scorebar_bg")].Width}\n" +
                                $"Height = {OnScreenImages[OnScreenImagesNames.IndexOf("scorebar_bg")].Height}\n";
            OnScreenImages[OnScreenImagesNames.IndexOf("scorebar_bg")].Height = 31; //сделать временный костыль в виде проверки разрешения пикчи перед ее установкой (scorebar-back)
        }

        private void DebugButtonGridScoreCapacity_Click(object sender, RoutedEventArgs e)
        {
            DebugTextBox.Text = GridScore.Children.Capacity.ToString();
        }

        private void DebugCheckBoxStatusShow_Checked(object sender, RoutedEventArgs e)
        {
            WindowSkinEditor.Height = 690; 
        }

        private void DebugCheckBoxStatusShow_Unchecked(object sender, RoutedEventArgs e)
        {
            WindowSkinEditor.Height = 662; //в идеале нужно отключать обновление юи на статус
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            cursor.Height = 80 * SliderSizeMul.Value;
            cursor.Width = 80 * SliderSizeMul.Value;
            cursormiddle.Height = 40 * SliderSizeMul.Value;
            cursormiddle.Width = 40 * SliderSizeMul.Value;
        }

        private void Version_TextChanged(object sender, TextChangedEventArgs e)
        {
            Version.ToolTip = SkinIniParser.VersionValidator(Version.Text);
            if (!SkinIniParser.IsVersionValid)
                Version.Background = Brushes.Red;
            else
                Version.Background = Brushes.Green;
        }

        private void SkinIniProperty_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBoxToolTips.Text = "";
            string[] ToToolTip = SkinIniParser.GetToolTipLines(sender, ToolTipLines);
            foreach(string elem in ToToolTip)
            {
                TextBoxToolTips.Text += elem + "\n";
            }
            Type SenderType = sender.GetType();
            if (SenderType.Name == "TextBox")
            {
                LabelToolTipSkinIniCurrentProp.Content = $"Current: {((TextBox)sender).Text}";
                return;
            }
            if (SenderType.Name == "CheckBox")
            {
                LabelToolTipSkinIniCurrentProp.Content = $"Current: {((CheckBox)sender).IsChecked.ToString()}";
                return;
            }
            if(SenderType.Name == "Frame")
            {
                LabelToolTipSkinIniCurrentProp.Content = $"Current: {((Frame)sender).Background}";
                return;
            }
        }

        private void ColourFrame_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ColourPicker picker = new ColourPicker(sender);
            picker.ShowDialog();
        }

        private void TextBoxOsuPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBoxOsuPath.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        }

        private void TextBox_NumberInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Numbers.IsMatch(e.Text);
        }

        private void DebugTextBoxColour_TextChanged(object sender, TextChangedEventArgs e) //не стал раскладывать на три разных ивента 
        {
            var Sender = sender as TextBox;
            if(Sender.Text != "" && int.Parse(Sender.Text) > 256)
            {
                Sender.Background = Brushes.Red;
                return;
            }
            Sender.Background = Brushes.White;

            R = SkinIniParser.GetSingleChannel(DebugTextBoxColourR.Text);
            G = SkinIniParser.GetSingleChannel(DebugTextBoxColourG.Text);
            B = SkinIniParser.GetSingleChannel(DebugTextBoxColourB.Text);

            DebugRectangle1.Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
        }

        private void SetCursor()
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(ComboBoxExistingSkin.SelectedValue + "/cursor.png");
            System.Windows.Forms.Cursor cursor = Png2CursorConverter.CreateCursor(bitmap);

            SafeFileHandle panHandle = new SafeFileHandle(cursor.Handle, false);
            GridCursor.Cursor = System.Windows.Interop.CursorInteropHelper.Create(panHandle);
            // Png2CursorConverter.CreateCursor(new System.Drawing.Bitmap(SkinWorker.DefaultSkinAbsPath + "/cursor.png"), 5, 5);
        }
    }
}

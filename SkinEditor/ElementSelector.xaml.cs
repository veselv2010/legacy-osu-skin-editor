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
    /// Логика взаимодействия для ElementSelector.xaml
    /// </summary>
    public partial class ElementSelector : Window
    {
        public ElementSelector()
        {
            InitializeComponent();
        }

        private void FrameElemViewer_Loaded(object sender, RoutedEventArgs e)
        {
           /* var temp = 0;
            foreach (string elem in SkinWorker.SkinPngElemArray)
            {
                ElementSelectorHelper i = new ElementSelectorHelper(elem, SkinWorker.Name2FileNameConv(elem, false));
            }*/
        }

        private void GridImages_Loaded(object sender, RoutedEventArgs e)
        {
            /*GridImages.ShowGridLines = true;
            Image temp = new Image();
            temp.Source = SkinWorker.GetImageByDirectPath(@"C:\Users\vesel\source\repos\SkinEditor\SkinEditor\bin\Debug\Default\cursor.png");
            List<Image> newList = new List<Image>();
            int j = 1;
            foreach(string elem in SkinWorker.SkinPngElemArray)
            {
                Image i = new Image();
                i.Name = elem.Replace(@"C:\osu!\Skins", "").Replace("-", "_").Replace(@"\", "_").Replace(".png", "").Replace("@", "").Replace(".PNG", "");
                i.Source = SkinWorker.GetImageByDirectPath(elem);
                GridImages.Children[j] = i;
                j++;
            } */
        }
    }
}

using Esri.ArcGISRuntime.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArcGISApp1.Controls.OtherTool
{
    /// <summary>
    /// MapViewTitleCsy.xaml 的交互逻辑
    /// </summary>
    public partial class MapViewTitleCsy : System.Windows.Controls.UserControl
    {
        Window w;
        MapView myMapView;
        System.Windows.Controls.TextBox t1;

        public System.Drawing.Color resultColor = System.Drawing.Color.Black;
        public System.Drawing.Color resultBgColor = System.Drawing.Color.FromArgb(0);

        double bgOpacity = 1.0;

        public MapViewTitleCsy()
        {
            InitializeComponent();
        }

        private void myUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            w = Window.GetWindow(this);
            myMapView = (MapView)w.FindName("myMapView");
            t1 = (System.Windows.Controls.TextBox)w.FindName("myMapTitleTxt");
        }

        private void fontSize_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9.-]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void fontColor_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog ColorForm = new ColorDialog();
            if (ColorForm.ShowDialog() == DialogResult.OK)
            {
                resultColor = ColorForm.Color;
                fontColor.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(resultColor.A, resultColor.R, resultColor.G, resultColor.B));
            }
        }

        private void fontOpacity_Checked(object sender, RoutedEventArgs e)
        {
            bgColor.IsEnabled = false;
            bgOpacity = 0;
        }

        private void fontOpacity_Unchecked(object sender, RoutedEventArgs e)
        {
            bgColor.IsEnabled = true;
            bgOpacity = 1;
        }

        private void sureBtn_Click(object sender, RoutedEventArgs e)
        {
            if (fontSize.Text != "")
            {
                t1.FontSize = Convert.ToInt32(fontSize.Text.Replace(" ", ""));
            }
            switch(fontWeight.SelectedIndex)
            {
                case 0:
                    t1.FontWeight = FontWeights.Normal;
                    break;
                case 1:
                    t1.FontWeight = FontWeights.Thin;
                    break;
                case 2:
                    t1.FontWeight = FontWeights.Bold;
                    break;
            }
            t1.Foreground = new SolidColorBrush(Color.FromArgb(resultColor.A, resultColor.R, resultColor.G, resultColor.B));
            if (fontOpacity.IsChecked == true)
            {
                t1.Background = new SolidColorBrush(Color.FromArgb(0,0,0,0));
            }
            else
            {
                t1.Background = new SolidColorBrush(Color.FromArgb(resultBgColor.A, resultBgColor.R, resultBgColor.G, resultBgColor.B));
            }
            Thumb thumb = (Thumb)w.FindName("mapTitle");
            thumb.Visibility = Visibility.Collapsed;
        }

        private void bgColor_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog ColorForm = new ColorDialog();
            if (ColorForm.ShowDialog() == DialogResult.OK)
            {
                resultBgColor = ColorForm.Color;
                bgColor.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(resultBgColor.A, resultBgColor.R, resultBgColor.G, resultBgColor.B));
            }
        }

        private void AddMapName_Click(object sender, RoutedEventArgs e)
        {
            Thumb thumb = (Thumb)w.FindName("mapTitle");
            if ((string)AddMapName.Content == "添加图名")
            {
                t1.Visibility = Visibility.Visible;
                thumb.Visibility = Visibility.Visible;
                AddMapName.Content = "隐藏图名";
            }
            else
            {
                t1.Visibility = Visibility.Collapsed;
                thumb.Visibility = Visibility.Collapsed;
                AddMapName.Content = "添加图名";
            }
        }
    }
}

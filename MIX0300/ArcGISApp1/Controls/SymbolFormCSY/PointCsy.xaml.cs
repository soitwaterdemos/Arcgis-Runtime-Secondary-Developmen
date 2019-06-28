using Esri.ArcGISRuntime.Symbology;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArcGISApp1.Controls.SymbolFormCSY
{
    /// <summary>
    /// PointCsy.xaml 的交互逻辑
    /// </summary>
    public partial class PointCsy : System.Windows.Controls.UserControl
    {
        // 点符号
        public static SimpleMarkerSymbol simplePointSymbol = new SimpleMarkerSymbol()
        {
            Color = Color.Green,
            Size = 6,
            Style = SimpleMarkerSymbolStyle.Circle
        };

        // 渲染器（原TOC代码）
        public static SimpleRenderer ptSymbol = new SimpleRenderer()
        {
            Symbol = simplePointSymbol
        };
        // 当前窗口
        Window w;
        // 符号类型
        public enumMarkerType MarkerType;
        // 颜色
        public Color resultColor = Color.Green;
        // 图片地址(本地)
        public Uri ImgUri;

        public PointCsy()
        {
            InitializeComponent();
        }

        private void csyPoint_Loaded(object sender, RoutedEventArgs e)
        {
            // 当前窗口
            w = Window.GetWindow(this);
        }

        private void myPointComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (myPointComboBox.SelectedIndex == 0)
            {
                myPointGroupBoxVector.IsEnabled = true;
                myPointGroupBoxPicture.IsEnabled = false;
            }
            else if (myPointComboBox.SelectedIndex == 1)
            {
                myPointGroupBoxVector.IsEnabled = false;
                myPointGroupBoxPicture.IsEnabled = true;
            }
        }

        private void myPointSymbolSize_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9.-]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void myPointSymbolColorChoose_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog ColorForm = new ColorDialog();
            if (ColorForm.ShowDialog() == DialogResult.OK)
            {
                resultColor = ColorForm.Color;
                myPointSymbolColorChoose.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(resultColor.A, resultColor.R, resultColor.G, resultColor.B));
                simplePointSymbol.Color = resultColor;
            }
        }

        private void myPointSymbolStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (myPointSymbolStyle.SelectedIndex)
            {
                case 0: simplePointSymbol.Style = SimpleMarkerSymbolStyle.Circle; break;
                case 1: simplePointSymbol.Style = SimpleMarkerSymbolStyle.Cross; break;
                case 2: simplePointSymbol.Style = SimpleMarkerSymbolStyle.Diamond; break;
                case 3: simplePointSymbol.Style = SimpleMarkerSymbolStyle.Square; break;
                case 4: simplePointSymbol.Style = SimpleMarkerSymbolStyle.Triangle; break;
                case 5: simplePointSymbol.Style = SimpleMarkerSymbolStyle.X; break;
               default: simplePointSymbol.Style = SimpleMarkerSymbolStyle.Circle; break;
            }
        }

        private void inputPictureAsPointSymbol_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofDlg = new OpenFileDialog();
            ofDlg.Title = "选择图像文件";
            ofDlg.Filter = "Bitmap图像（*.bmp）|*.bmp|Png图像（*.png） |*.png|Jpeg图像（*.jpg）|*.jpg|Tif图像（*.tif）|*.tif|所有文件（*.*）|*.*";
            if (ofDlg.ShowDialog() == DialogResult.OK)
            {
                myPointPictureBox.Source = new BitmapImage(new Uri(ofDlg.FileName));
                string path = System.IO.Path.GetFullPath(ofDlg.FileName);
                ImgUri = new Uri(path);
            }
        }

        private void myPointSymbolSure_Click(object sender, RoutedEventArgs e)
        {
            if (myPointComboBox.SelectedIndex == 0)
            {
                MarkerType = enumMarkerType.SimpleMarker;
                if (myPointSymbolSize.Text != "")
                {
                    simplePointSymbol.Size = Convert.ToInt32(myPointSymbolSize.Text);
                }             
            }
            else if (myPointComboBox.SelectedIndex == 1)
            {
                MarkerType = enumMarkerType.PictureMarker;
                PictureMarkerSymbol pm = new PictureMarkerSymbol(ImgUri);
                pm.Width = 20;
                pm.Height = 20;
                ptSymbol.Symbol = pm;
            }
            else if (myPointComboBox.SelectedIndex == 2)
            {
                MarkerType = enumMarkerType.Text;
            }
            w.Close();
        }

        private void myPointSymbolCancel_Click(object sender, RoutedEventArgs e)
        {
            w.Close();
        }
    }
}

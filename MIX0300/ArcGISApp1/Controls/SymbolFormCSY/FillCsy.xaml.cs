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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using Esri.ArcGISRuntime.Symbology;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;

namespace ArcGISApp1.Controls.SymbolFormCSY
{

    /// <summary>
    /// FillCsy.xaml 的交互逻辑
    /// </summary>
    public partial class FillCsy : System.Windows.Controls.UserControl
    {
        //// 1105测试
        //public static SimpleRenderer fs = new SimpleRenderer();
        //public static SimpleFillSymbol fs2 = new SimpleFillSymbol()
        //{
        //    Outline = new SimpleLineSymbol()
        //    {
        //        Style = SimpleLineSymbolStyle.Solid,
        //        Width = 4,
        //        Color = Color.Red
        //    },
        //    Style = SimpleFillSymbolStyle.Solid,
        //    Color = Color.Red
        //};

        // 当前窗口
        Window w;
        // 渲染器（原TOC代码）
        public static SimpleRenderer fillSymbol = new SimpleRenderer()
        {
            Symbol = new SimpleLineSymbol()
            {
                Style = SimpleLineSymbolStyle.Solid,
                Width = 4,
                Color = Color.Green
            }
        };

        // 边框
        public static SimpleLineSymbol simpleLineSymbol = new SimpleLineSymbol()
        {
            Style = SimpleLineSymbolStyle.Solid,
            Width = 4,
            Color = Color.Green
        };
        // 图片地址(本地)
        public Uri ImgUri;
        // 面符号-矢量
        public static SimpleFillSymbol simpleFillSymbol = new SimpleFillSymbol()
        {
            Outline = new SimpleLineSymbol()
            {
                Style = SimpleLineSymbolStyle.Solid,
                Width = 4,
                Color = Color.Red
            },
            Style = SimpleFillSymbolStyle.Solid,
            Color = Color.Red
        };
        // 面符号类型
        public enumMarkerType MarkerType;
        // 颜色
        public Color resultColor = Color.Red;
        // 边框颜色
        public Color outlineColor = Color.Gray;
        // 图片
        public PictureFillSymbol pictureFillSymbol;

        public FillCsy()
        {
            InitializeComponent();
        }

        private void myFillSymbolComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (myFillSymbolComboBox.SelectedIndex == 0)
            {
                myFillSymGroupBoxVector.IsEnabled = true;
                myFillSymGroupBoxPicture.IsEnabled = false;
            }
            else if (myFillSymbolComboBox.SelectedIndex == 1)
            {
                myFillSymGroupBoxVector.IsEnabled = false;
                myFillSymGroupBoxPicture.IsEnabled = true;
            }
        }

        private void myVectorColor_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog ColorForm = new ColorDialog();
            if (ColorForm.ShowDialog() == DialogResult.OK)
            {
                resultColor = ColorForm.Color;
                myVectorColor.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(resultColor.A, resultColor.R, resultColor.G, resultColor.B));
            }
        }

        private void myVectorStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (myVectorStyle.SelectedIndex)
            {
                case 0: simpleFillSymbol.Style = SimpleFillSymbolStyle.Solid; break;
                case 1: simpleFillSymbol.Style = SimpleFillSymbolStyle.Horizontal; break;
                case 2: simpleFillSymbol.Style = SimpleFillSymbolStyle.Vertical; break;
                case 3: simpleFillSymbol.Style = SimpleFillSymbolStyle.Cross; break;
                case 4: simpleFillSymbol.Style = SimpleFillSymbolStyle.DiagonalCross; break;
                case 5: simpleFillSymbol.Style = SimpleFillSymbolStyle.BackwardDiagonal; break;
                case 6: simpleFillSymbol.Style = SimpleFillSymbolStyle.ForwardDiagonal; break;
                default: simpleFillSymbol.Style = SimpleFillSymbolStyle.Null; break;
            }
        }

        private void myInputImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofDlg = new OpenFileDialog();
            ofDlg.Title = "选择图像文件";
            ofDlg.Filter = "Jpeg图像（*.jpg）|*.jpg|Png图像（*.png） |*.png|Bitmap图像（*.bmp）|*.bmp|Tif图像（*.tif）|*.tif|所有文件（*.*）|*.*";
            if (ofDlg.ShowDialog() == DialogResult.OK)
            {
                myPictureBox.Source = new BitmapImage(new Uri(ofDlg.FileName));
                string path = System.IO.Path.GetFullPath(ofDlg.FileName);
                ImgUri = new Uri(path);
            }
        }

        private void myLineColor_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog ColorForm = new ColorDialog();
            if (ColorForm.ShowDialog() == DialogResult.OK)
            {
                outlineColor = ColorForm.Color;
                myLineColor.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(resultColor.A, resultColor.R, resultColor.G, resultColor.B));
            }
        }

        private void myLineSize_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9.-]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void myLineStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (myLineStyle.SelectedIndex)
            {
                case 0: simpleLineSymbol.Style = SimpleLineSymbolStyle.Solid; break;
                case 1: simpleLineSymbol.Style = SimpleLineSymbolStyle.Dash; break;
                case 2: simpleLineSymbol.Style = SimpleLineSymbolStyle.Dot; break;
                case 3: simpleLineSymbol.Style = SimpleLineSymbolStyle.DashDot; break;
                case 4: simpleLineSymbol.Style = SimpleLineSymbolStyle.DashDotDot; break;
                default: simpleLineSymbol.Style = SimpleLineSymbolStyle.Null; break;
            }
        }

        // 确定按钮——符号渲染 / 窗口关闭
        private void myFillSymSureBtn_Click(object sender, RoutedEventArgs e)
        {           
            if (myFillSymbolComboBox.SelectedIndex == 0)
            {
                MarkerType = enumMarkerType.SimpleMarker;
                simpleFillSymbol.Color = resultColor;
                simpleLineSymbol.Color = outlineColor;
                simpleFillSymbol.Outline = simpleLineSymbol;
                fillSymbol.Symbol = simpleFillSymbol;
            }
            else if (myFillSymbolComboBox.SelectedIndex == 1)
            {
                MarkerType = enumMarkerType.PictureMarker;
                if (ImgUri == null)
                {                   
                    w.Close();
                    return;
                }
                pictureFillSymbol = new PictureFillSymbol(ImgUri);
                pictureFillSymbol.Outline = simpleLineSymbol;
                fillSymbol.Symbol = pictureFillSymbol;
            }
            w.Close();
        }

        private void csyFill_Loaded(object sender, RoutedEventArgs e)
        {
            // 当前窗口
            w = Window.GetWindow(this);
        }

        private void myFillSymCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            w.Close();
        }
    }
}

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
    /// LineCsy.xaml 的交互逻辑
    /// </summary>
    public partial class LineCsy : System.Windows.Controls.UserControl
    {
        public LineCsy()
        {
            InitializeComponent();
        }

        // 渲染器（原TOC）
        public static SimpleRenderer simpleRenderer = new SimpleRenderer();
        // 当前窗口
        Window w;
        // 颜色
        public Color resultColor = System.Drawing.Color.Green;
        // 线宽
        public int symbolSize;
        // 线符号
        public static SimpleLineSymbol simpleLineSymbol = new SimpleLineSymbol()
        {
            Style = SimpleLineSymbolStyle.Solid,
            Width = 6, 
            Color = System.Drawing.Color.Green
        };


        private void csyLine_Loaded(object sender, RoutedEventArgs e)
        {
            // 当前窗口
            w = Window.GetWindow(this);
        }

        private void myLineSymbolStyleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (myLineSymbolStyleComboBox.SelectedIndex)
            {
                case 0: simpleLineSymbol.Style = SimpleLineSymbolStyle.Solid; break;
                case 1: simpleLineSymbol.Style = SimpleLineSymbolStyle.Dash; break;
                case 2: simpleLineSymbol.Style = SimpleLineSymbolStyle.Dot; break;
                case 3: simpleLineSymbol.Style = SimpleLineSymbolStyle.DashDot; break;
                case 4: simpleLineSymbol.Style = SimpleLineSymbolStyle.DashDotDot; break;
               default: simpleLineSymbol.Style = SimpleLineSymbolStyle.Null; break;
            }
        }

        private void myLineSymbolColorBox_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog ColorForm = new ColorDialog();
            if (ColorForm.ShowDialog() == DialogResult.OK)
            {
                resultColor = ColorForm.Color;
                myLineSymbolColorBox.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(resultColor.A, resultColor.R, resultColor.G, resultColor.B));
                simpleLineSymbol.Color = resultColor;
            }
        }

        private void myLineSymbolSureBtn_Click(object sender, RoutedEventArgs e)
        {
            if (myLineSymbolSizeTxtBox.Text != "")
            {
                simpleLineSymbol.Width = Convert.ToInt32(myLineSymbolSizeTxtBox.Text);
            }
            simpleRenderer.Symbol = simpleLineSymbol;
            w.Close();
        }

        private void myLineSymbolCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            w.Close();
        }

        private void myLineSymbolSizeTxtBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9.-]+");
            e.Handled = re.IsMatch(e.Text);
        }
    }
}

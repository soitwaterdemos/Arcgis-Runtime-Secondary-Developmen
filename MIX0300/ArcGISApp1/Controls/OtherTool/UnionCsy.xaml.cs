using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArcGISApp1.Controls.OtherTool
{
    /// <summary>
    /// UnionCsy.xaml 的交互逻辑
    /// </summary>
    public partial class UnionCsy : UserControl
    {
        Window w;
        MapView myMapView;
        public static FeatureLayer layer;
        Graphic graphic;
        Esri.ArcGISRuntime.Geometry.Geometry resultGeometry;
        // 三种Symbol
        public static SimpleLineSymbol simpleLineSymbol;
        public static SimpleFillSymbol simpleFillSymbol = new SimpleFillSymbol()
        {
            Outline = new SimpleLineSymbol()
            {
                Style = SimpleLineSymbolStyle.Solid,
                Width = 4,
                Color = System.Drawing.Color.Red
            },
            Style = SimpleFillSymbolStyle.Solid,
            Color = System.Drawing.Color.Red
        };
        public static SimpleMarkerSymbol simplePointSymbol = new SimpleMarkerSymbol()
        {
            Color = System.Drawing.Color.Green,
            Size = 6,
            Style = SimpleMarkerSymbolStyle.Circle
        };
        // 判断要素类型
        int featureStyle = 0;

        public UnionCsy()
        {
            InitializeComponent();
        }

        private async void unionBtn_Click(object sender, RoutedEventArgs e)
        {
            FeatureQueryResult r = await layer.GetSelectedFeaturesAsync();
            IEnumerator<Feature> resultFeatures = r.GetEnumerator();
            List<Feature> features = new List<Feature>();
            while (resultFeatures.MoveNext())
            {
                features.Add(resultFeatures.Current);
            }


            for (int i = 0; i < features.Count - 1;i++)
            {
                resultGeometry = GeometryEngine.Union(features[i].Geometry, features[i+1].Geometry.Extent);
            }
            try
            {
                if (featureStyle == 1)
                {
                    graphic = new Graphic(resultGeometry, simplePointSymbol);
                }
                else if (featureStyle == 2)
                {
                    graphic = new Graphic(resultGeometry, simpleLineSymbol);
                }
                else if (featureStyle == 3)
                {
                    graphic = new Graphic(resultGeometry, simpleFillSymbol);
                }
                MainWindow.graphicsOverlay.Graphics.Add(graphic);
            }
            catch
            {

            }
        }

        private void myUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            w = Window.GetWindow(this);
            myMapView = (MapView)w.FindName("myMapView");
        }

        private void symbolBtn_Click(object sender, RoutedEventArgs e)
        {
            // 先关闭所有的工具条
            Canvas userControlsLayoutCsy = (Canvas)w.FindName("userControlsLayoutCsy");
            List<StackPanel> tools = ArcGISApp1.Utils.GetChildNode.GetChildObjects<StackPanel>(userControlsLayoutCsy);
            for (int i = 0; i < tools.Count(); i++)
            {
                tools[i].Visibility = Visibility.Collapsed;
            }
            
            // 符号渲染窗口
            if (layer.FeatureTable.GeometryType == Esri.ArcGISRuntime.Geometry.GeometryType.Point)
            {
                featureStyle = 1;
                var window = new Window();
                Controls.SymbolFormCSY.PointCsy pointForm = new Controls.SymbolFormCSY.PointCsy();
                window.Content = pointForm;
                window.SizeToContent = SizeToContent.WidthAndHeight;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Title = "点符号渲染工具";
                window.Closed += windowClosed_Point;
                window.Show();
            }
            else if (layer.FeatureTable.GeometryType == Esri.ArcGISRuntime.Geometry.GeometryType.Polyline)
            {
                featureStyle = 2;
                var window = new Window();
                Controls.SymbolFormCSY.LineCsy lineForm = new Controls.SymbolFormCSY.LineCsy();
                window.Content = lineForm;
                window.SizeToContent = SizeToContent.WidthAndHeight;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Title = "线符号渲染工具";
                window.Closed += windowClosed_Line;
                window.Show();
            }
            else if (layer.FeatureTable.GeometryType == Esri.ArcGISRuntime.Geometry.GeometryType.Polygon)
            {
                featureStyle = 3;
                var window = new Window();
                Controls.SymbolFormCSY.FillCsy fillForm = new Controls.SymbolFormCSY.FillCsy();
                window.Content = fillForm;
                window.SizeToContent = SizeToContent.WidthAndHeight;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Title = "面符号渲染工具";
                window.Closed += windowClosed_Polygon;
                window.Show();
            }
        }

        private void windowClosed_Point(object sender, EventArgs e)
        {
            featureStyle = 1;
            simplePointSymbol = SymbolFormCSY.PointCsy.simplePointSymbol;
        }

        private void windowClosed_Line(object sender, EventArgs e)
        {
            featureStyle = 2;
            simpleLineSymbol = SymbolFormCSY.LineCsy.simpleLineSymbol;
        }

        private void windowClosed_Polygon(object sender, EventArgs e)
        {
            featureStyle = 3;
            simpleFillSymbol = SymbolFormCSY.FillCsy.simpleFillSymbol;
        }
    }
}

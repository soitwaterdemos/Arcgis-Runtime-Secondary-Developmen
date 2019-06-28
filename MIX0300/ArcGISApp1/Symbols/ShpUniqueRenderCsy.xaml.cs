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

namespace ArcGISApp1.Symbols
{
    /// <summary>
    /// ShpUniqueRenderCsy.xaml 的交互逻辑
    /// </summary>
    public partial class ShpUniqueRenderCsy : UserControl
    {
        Window w;
        MapView myMapView;
        public static FeatureLayer layer;
        public static List<string> fieldSource;
        public static FeatureTable featureTable;
        string chooseField = "";
        // 默认分组为2
        int sort = 2;

        // 颜色
        System.Drawing.Color color1 = System.Drawing.Color.FromArgb(150,193,253,187); // 红色
        System.Drawing.Color color2 = System.Drawing.Color.FromArgb(150,151,251,141); // 黄色
        System.Drawing.Color color3 = System.Drawing.Color.FromArgb(150,97,249,81); // 绿色
        System.Drawing.Color color4 = System.Drawing.Color.FromArgb(150,22,222,12); // 蓝色
        System.Drawing.Color color5 = System.Drawing.Color.FromArgb(150,17,175,9); // 紫色
        List<System.Drawing.Color> colorList = new List<System.Drawing.Color>();
        // 符号
        SimpleMarkerSymbol defaultPoint = new SimpleMarkerSymbol(){Color = System.Drawing.Color.Gray, Size = 6, Style = SimpleMarkerSymbolStyle.Circle};
        //SimpleMarkerSymbol p1;
        //SimpleMarkerSymbol p2;
        //SimpleMarkerSymbol p3;
        //SimpleMarkerSymbol p4;
        SimpleLineSymbol defaultLine = new SimpleLineSymbol(){Style = SimpleLineSymbolStyle.Solid,Width = 2,Color = System.Drawing.Color.Gray};
        //SimpleLineSymbol l1;
        //SimpleLineSymbol l2;
        //SimpleLineSymbol l3;
        //SimpleLineSymbol l4;
        SimpleFillSymbol defaultFill = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.FromArgb(150, 137, 104, 205), new SimpleLineSymbol() { Style = SimpleLineSymbolStyle.Solid, Width = 2, Color = System.Drawing.Color.Gray });
        //SimpleFillSymbol f1;
        //SimpleFillSymbol f2;
        //SimpleFillSymbol f3;
        //SimpleFillSymbol f4;

        public ShpUniqueRenderCsy()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            w = Window.GetWindow(this);
            myMapView = (MapView)w.FindName("myMapView");

            // 颜色
            colorList.Add(color1);
            colorList.Add(color2);
            colorList.Add(color3);
            colorList.Add(color4);
            colorList.Add(color5);

            //// 符号
            //p1 = new SimpleMarkerSymbol() { Color = color1, Size = 4, Style = SimpleMarkerSymbolStyle.Circle };
            //p2 = new SimpleMarkerSymbol() { Color = color2, Size = 4, Style = SimpleMarkerSymbolStyle.Circle };
            //p3 = new SimpleMarkerSymbol() { Color = color3, Size = 4, Style = SimpleMarkerSymbolStyle.Circle };
            //p4 = new SimpleMarkerSymbol() { Color = color4, Size = 4, Style = SimpleMarkerSymbolStyle.Circle };

            //l1 = new SimpleLineSymbol() { Style = SimpleLineSymbolStyle.Solid, Width = 1, Color = color1 };
            //l2 = new SimpleLineSymbol() { Style = SimpleLineSymbolStyle.Solid, Width = 1, Color = color2 };
            //l3 = new SimpleLineSymbol() { Style = SimpleLineSymbolStyle.Solid, Width = 1, Color = color3 };
            //l4 = new SimpleLineSymbol() { Style = SimpleLineSymbolStyle.Solid, Width = 1, Color = color4 };

            //f1 = new SimpleFillSymbol() { Style = SimpleFillSymbolStyle.Solid, Color = color1, Outline = defaultLine };
            //f2 = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, color2, defaultLine);
            //f3 = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, color3, defaultLine);
            //f4 = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, color4, defaultLine);
        }

        private void fieldComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            chooseField = fieldSource[fieldComboBox.SelectedIndex];
            renderBtn.IsEnabled = true;
        }

        private async void renderBtn_Click(object sender, RoutedEventArgs e)
        {
            // 随机
            Random rd = new Random();

            UniqueValueRenderer regionRenderer = new UniqueValueRenderer();  
            // 需要找的field的字段名
            regionRenderer.FieldNames.Add(chooseField);
            // 获取值
            QueryParameters query = new QueryParameters();
            query.WhereClause = string.Format("upper(FID) >= 0");
            FeatureQueryResult queryResult = await featureTable.QueryFeaturesAsync(query);
            IEnumerator<Feature> resultFeatures = queryResult.GetEnumerator();
            List<Object> featureValue = new List<Object>();
            try
            {
                while (resultFeatures.MoveNext())
                {
                    featureValue.Add((resultFeatures.Current).GetAttributeValue(chooseField));
                }
                featureValue = featureValue.Distinct().ToList();
            }
            catch
            {
                return;
            }
            // 将值分组
            int count = featureValue.Count;

            if (layer.FeatureTable.GeometryType == Esri.ArcGISRuntime.Geometry.GeometryType.Point)
            {
                for (int i = 0; i < count;i++)
                {
                    int num = rd.Next(0, sort);
                    System.Drawing.Color tempColor = colorList[num];
                    SimpleMarkerSymbol tempPointSymbol = new SimpleMarkerSymbol() { Color = tempColor, Size = 6, Style = SimpleMarkerSymbolStyle.Circle };
                    regionRenderer.UniqueValues.Add(new UniqueValue("null", "null", tempPointSymbol, featureValue[i]));
                }
                regionRenderer.DefaultSymbol = defaultPoint;
                regionRenderer.DefaultLabel = "zero";
            }
            else if (layer.FeatureTable.GeometryType == Esri.ArcGISRuntime.Geometry.GeometryType.Polyline)
            {
                for (int i = 0; i < count; i++)
                {
                    int num = rd.Next(0, sort);
                    System.Drawing.Color tempColor = colorList[num];
                    SimpleLineSymbol tempLineSymbol = new SimpleLineSymbol() { Style = SimpleLineSymbolStyle.Solid, Width = 5, Color = tempColor};
                    regionRenderer.UniqueValues.Add(new UniqueValue("null", "null", tempLineSymbol, featureValue[i]));
                }
                regionRenderer.DefaultSymbol = defaultLine;
                regionRenderer.DefaultLabel = "zero";
            }
            else if (layer.FeatureTable.GeometryType == Esri.ArcGISRuntime.Geometry.GeometryType.Polygon)
            {
                SimpleLineSymbol outLineSymbol = new SimpleLineSymbol() { Style = SimpleLineSymbolStyle.Solid, Width = 3, Color = System.Drawing.Color.Gray };
                for (int i = 0; i < count; i++)
                {
                    int num = rd.Next(0, sort);
                    System.Drawing.Color tempColor = colorList[num];
                    SimpleFillSymbol tempFillSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, tempColor, outLineSymbol);
                    regionRenderer.UniqueValues.Add(new UniqueValue("null", "null", tempFillSymbol, featureValue[i]));
                }
                regionRenderer.DefaultSymbol = defaultFill;
                regionRenderer.DefaultLabel = "zero";
            }
            layer.Renderer = regionRenderer;
         
        }

        private void levelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sort = levelComboBox.SelectedIndex + 2;
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            fieldComboBox.ItemsSource = fieldSource;
            fieldComboBox.IsEnabled = true;
        }

        private void hideBtn_Click(object sender, RoutedEventArgs e)
        {
            StackPanel s = (StackPanel)w.FindName("SHPuniqueRenderCsy");
            s.Visibility = Visibility.Collapsed;
        }
    }
}

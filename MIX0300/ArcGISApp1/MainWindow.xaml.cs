using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MahApps.Metro.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Rasters;
using Esri.ArcGISRuntime.Symbology;
using MahApps.Metro.Controls.Dialogs;
using System.IO;
using System.Drawing;
using Esri.ArcGISRuntime.Geometry;

namespace ArcGISApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // geodatabase地址
        public static Geodatabase myDatbase;
        // geodatabaase 属性表
        public static IReadOnlyList<GeodatabaseFeatureTable> myTables;
        // 获取本地服务
        public static Esri.ArcGISRuntime.LocalServices.LocalServer _localServer = Esri.ArcGISRuntime.LocalServices.LocalServer.Instance;
        // 绘制层
        public static GraphicsOverlay graphicsOverlay = new GraphicsOverlay();
        // 全部图层
        public static List<Layer> myMapLayersList = new List<Layer>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 绘制层
            myMapView.GraphicsOverlays.Add(graphicsOverlay);
            // 鹰眼的底图
            myMapView_Eagle.Map = new Map(Basemap.CreateTopographic());
            // 主地图的底图(因为一次地图错误——重复载入底图导致进程中断 才添加的)
            myMapView.Map = new Map(Basemap.CreateTopographic());
            // 底图的列表
            ChangeBaseMap_list.ItemsSource = new List<ListBoxItem>()
            {
                new ListBoxItem(){ Content = "DarkGrayCanvasVector"},
                new ListBoxItem(){ Content = "Imagery"},
                new ListBoxItem(){ Content = "ImageryWithLabels"},
                new ListBoxItem(){ Content = "TerrainWithLabels"},
                new ListBoxItem(){ Content = "LightGrayCanvas"},
                new ListBoxItem(){ Content = "Topographic"},
                new ListBoxItem(){ Content = "NationalGeographic"},
                new ListBoxItem(){ Content = "NavigationVector"},
                new ListBoxItem(){ Content = "Oceans"},
                new ListBoxItem(){ Content = "Streets"},
            };

            // 初始化底图2
            myMapView2.Map = new Map(Basemap.CreateLightGrayCanvas());
        }
               
        // 数据/导入shapefile文件; 
        private async void AddSHP_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofdlg = new System.Windows.Forms.OpenFileDialog();
            ofdlg.Multiselect = true;
            ofdlg.Filter = "Shapefile文件(*.shp)|*.shp";
            ofdlg.Title = "打开Shapefile文件";
            if (ofdlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                for (int i = 0;i < ofdlg.FileNames.Length;i++)
                {                 
                    String fPath = ofdlg.FileNames.GetValue(i).ToString();
                    // 新建图层
                    ShapefileFeatureTable myShapefile = await ShapefileFeatureTable.OpenAsync(fPath);
                    FeatureLayer newFeaturelayer = new FeatureLayer(myShapefile)
                    {
                        SelectionColor = System.Drawing.Color.Cyan,
                        SelectionWidth = 5
                    };
                    Esri.ArcGISRuntime.UI.Controls.MapView mapView = (Esri.ArcGISRuntime.UI.Controls.MapView)this.FindName("myMapView");
                    if (mapView != null)
                    {
                        myMapLayersList.Add(newFeaturelayer);
                        mapView.Map.OperationalLayers.Add(newFeaturelayer);
                        await mapView.SetViewpointGeometryAsync(newFeaturelayer.FullExtent);
                        // 获取文件名
                        String layerName = System.IO.Path.GetFileName(fPath);
                        // 新建checkbox并入栈
                        myTableOfContent.addLayersList(layerName);                     
                    }
                }
            }
        }

        // 实验二：显示ArcGIS Online地图 (废弃)
        private async void AddArcGISOnlineMap_Click(object sender, RoutedEventArgs e)
        {
            // 找到myMapView控件
            Esri.ArcGISRuntime.UI.Controls.MapView mapView = (Esri.ArcGISRuntime.UI.Controls.MapView)this.FindName("myMapView");
            if (mapView.Map != null)
            {
                mapView.Map = null;
                Uri uri = new Uri(@"https://www.arcgis.com/home/webmap/viewer.html?webmap=2cc6d640d36d465ca32f1a20767d7d39");
                mapView.Map = await Map.LoadFromUriAsync(uri);
            }
        }

        // 鹰眼
        private void myMapView_ViewpointChanged(object sender, EventArgs e)
        {
            // 声明鹰眼地图的覆盖层边框
            Esri.ArcGISRuntime.Geometry.Geometry eagleViewEnv = null;
            // 每次主地图的焦点改变, 都会清空鹰眼地图的覆盖层
            myMapView_Eagle.GraphicsOverlays.Clear();
            // 获取主地图的四至
            Esri.ArcGISRuntime.Geometry.Polygon vExtent = myMapView.VisibleArea;
            // 鹰眼地图的覆盖层边框等于主地图四至
            eagleViewEnv = vExtent.Extent;
            // 鹰眼地图的覆盖层边框为"红色"
            System.Drawing.Color lineColor = System.Drawing.Color.FromName("Red");
            // 鹰眼地图的覆盖层边框样式
            Esri.ArcGISRuntime.Symbology.SimpleLineSymbol lineSymbol = new Esri.ArcGISRuntime.Symbology.SimpleLineSymbol(Esri.ArcGISRuntime.Symbology.SimpleLineSymbolStyle.Dash, lineColor, 2.0);
            System.Drawing.Color fillColor = System.Drawing.Color.FromArgb(0, 255, 255, 255);
            Esri.ArcGISRuntime.Symbology.SimpleFillSymbol polySymbol = new Esri.ArcGISRuntime.Symbology.SimpleFillSymbol(Esri.ArcGISRuntime.Symbology.SimpleFillSymbolStyle.Solid, fillColor, lineSymbol);
            var graphicOverlay = new Esri.ArcGISRuntime.UI.GraphicsOverlay();
            // 几何图层
            var envGraphic = new Esri.ArcGISRuntime.UI.Graphic(eagleViewEnv, polySymbol);
            // 覆盖层
            graphicOverlay.Graphics.Add(envGraphic);
            // 覆盖层添加到鹰眼地图
            myMapView_Eagle.GraphicsOverlays.Add(graphicOverlay);
        }      

        // 改变底图
        private void ChangeBaseMap_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Map myMap = myMapView.Map;
            switch (ChangeBaseMap_list.SelectedIndex)
            {
                case 0:
                    myMap.Basemap = Basemap.CreateDarkGrayCanvasVector();
                    myMapView_Eagle.Map.Basemap = Basemap.CreateDarkGrayCanvasVector();
                    break;
                case 1:
                    myMap.Basemap = Basemap.CreateImagery();
                    myMapView_Eagle.Map.Basemap = Basemap.CreateImagery();
                    break;
                case 2:
                    myMap.Basemap = Basemap.CreateImageryWithLabels();
                    myMapView_Eagle.Map.Basemap = Basemap.CreateImageryWithLabels();
                    break;
                case 3:
                    myMap.Basemap = Basemap.CreateTerrainWithLabels();
                    myMapView_Eagle.Map.Basemap = Basemap.CreateTerrainWithLabels();
                    break;
                case 4:
                    myMap.Basemap = Basemap.CreateLightGrayCanvas();
                    myMapView_Eagle.Map.Basemap = Basemap.CreateLightGrayCanvas();
                    break;
                case 5:
                    myMap.Basemap = Basemap.CreateTopographic();
                    myMapView_Eagle.Map.Basemap = Basemap.CreateTopographic();
                    break;
                case 6:
                    myMap.Basemap = Basemap.CreateNationalGeographic();
                    myMapView_Eagle.Map.Basemap = Basemap.CreateNationalGeographic();
                    break;
                case 7:
                    myMap.Basemap = Basemap.CreateNavigationVector();
                    myMapView_Eagle.Map.Basemap = Basemap.CreateNavigationVector();
                    break;
                case 8:
                    myMap.Basemap = Basemap.CreateOceans();
                    myMapView_Eagle.Map.Basemap = Basemap.CreateOceans();
                    break;
                case 9:
                    myMap.Basemap = Basemap.CreateStreets();
                    myMapView_Eagle.Map.Basemap = Basemap.CreateStreets();
                    break;
            }
        }

        // 添加栅格
        private async void AddRaster_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofdlg = new System.Windows.Forms.OpenFileDialog();
            ofdlg.Multiselect = true;
            ofdlg.Filter = "Raster文件|*.tiff;*.tif;*.jpeg;*.png;";
            ofdlg.Title = "打开Raster文件";
            if (ofdlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                for (int i = 0; i < ofdlg.FileNames.Length; i++)
                {
                    String fPath = ofdlg.FileNames.GetValue(i).ToString();
                    // 读取栅格文件
                    Raster myRasterFile = new Raster(fPath);
                    // 创建栅格图层
                    RasterLayer myRasterLayer = new RasterLayer(myRasterFile);
                    // 显示到窗口
                    myMapLayersList.Add(myRasterLayer);
                    // 加入到业务图层列表
                    myMapView.Map.OperationalLayers.Add(myRasterLayer);
                    // 等待图层加载完成
                    await myRasterLayer.LoadAsync();
                    // 设置四至，缩放至图层(这个才是正确的)
                    await myMapView.SetViewpointGeometryAsync(myRasterLayer.FullExtent);
                    // 获取文件名
                    String layerName = System.IO.Path.GetFileName(fPath);
                    // 加入到checkbox列表
                    myTableOfContent.addLayersList(layerName);
                }
            }
        }

        // 测试用方法
        private async void myTestBtn_Click(object sender, RoutedEventArgs e)
        {

            //IReadOnlyList<LegendInfo> i = await myMapView.Map.OperationalLayers[0].GetLegendInfosAsync();
            //myTest.Text = "///" + i[0].Name;

            //若Name改变，说明图层顺序修改成功
            //myTest.Text = myMapView.Map.OperationalLayers[0].Name + "";

            //var window = new Window();//Windows窗体
            //Controls.DrawAndEdit mytest = new Controls.DrawAndEdit();  //UserControl写的界面  
            //window.Content = mytest;
            //window.SizeToContent = SizeToContent.WidthAndHeight;
            //window.WindowStartupLocation = WindowStartupLocation.Manual;
            //window.Left = 600;
            //window.Top = 50;
            //window.Title = "1234";
            //window.Show();

            //List<StackPanel> mis = Utils.GetChildNode.GetChildObjects<StackPanel>(userControlsLayoutCsy);
            //myTest.Text = "shu: " + mis.Count();
            //closeAllTools();

            //userControlsLayoutCsy.Visibility = Visibility.Hidden;
            //myDrawAndEdit.Visibility = Visibility.Visible;

            // Create a new unique value renderer.
            FeatureLayer statesLayer = (FeatureLayer)myMapView.Map.OperationalLayers[0];
            UniqueValueRenderer regionRenderer = new UniqueValueRenderer();

            // Add the "SUB_REGION" field to the renderer.
            regionRenderer.FieldNames.Add("FID");

            // Define a line symbol to use for the region fill symbols.
            SimpleLineSymbol stateOutlineSymbol = new SimpleLineSymbol(
                SimpleLineSymbolStyle.Solid, System.Drawing.Color.White, 0.7);

            // Define distinct fill symbols for a few regions (use the same outline symbol).
            SimpleFillSymbol pacificFillSymbol = new SimpleFillSymbol(
                SimpleFillSymbolStyle.Solid, System.Drawing.Color.Blue, stateOutlineSymbol);
            SimpleFillSymbol mountainFillSymbol = new SimpleFillSymbol(
                SimpleFillSymbolStyle.Solid, System.Drawing.Color.LawnGreen, stateOutlineSymbol);
            SimpleFillSymbol westSouthCentralFillSymbol = new SimpleFillSymbol(
                SimpleFillSymbolStyle.Solid, System.Drawing.Color.SandyBrown, stateOutlineSymbol);

            // Add values to the renderer: define the label, description, symbol, and attribute value for each.
            regionRenderer.UniqueValues.Add(
                new UniqueValue("Pacific", "Pacific Region", pacificFillSymbol, 1));
            regionRenderer.UniqueValues.Add(
                new UniqueValue("Mountain", "Rocky Mountain Region", mountainFillSymbol, 2));
            regionRenderer.UniqueValues.Add(
                new UniqueValue("West South Central", "West South Central Region", westSouthCentralFillSymbol, 3));

            // Set the default region fill symbol for regions not explicitly defined in the renderer.
            SimpleFillSymbol defaultFillSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Cross, System.Drawing.Color.Gray, null);
            regionRenderer.DefaultSymbol = defaultFillSymbol;
            regionRenderer.DefaultLabel = "Other";

            // Apply the unique value renderer to the states layer.
            statesLayer.Renderer = regionRenderer;

            // Add created layer to the map.
            //myMapView.Map.OperationalLayers.Add(statesLayer);

            //// Assign the map to the MapView.
            //myMapView.Map = myMap;
            await myMapView.SetViewpointCenterAsync(new MapPoint(-10846309.950860, 4683272.219411, SpatialReferences.WebMercator), 20000000);

        }

        // 测试用方法
        private async void myTestBtn2_Click(object sender, RoutedEventArgs e)
        {
            //myLegend.GeoView = null;
            

            // Help regarding the Json syntax for defining the LabelDefinition.FromJson syntax can be found here:
            // https://developers.arcgis.com/web-map-specification/objects/labelingInfo/
            // This particular JSON string will have the following characteristics:
            string redLabelJson =
             @"{
                    ""labelExpressionInfo"":{""expression"":""$feature.NAME + ' (' + left($feature.PARTY,1) + ')\\nDistrict' + $feature.CDFIPS""},
                    ""labelPlacement"":""esriServerPolygonPlacementAlwaysHorizontal"",
                    ""where"":""PARTY = 'Republican'"",
                    ""symbol"":
                        { 
                            ""angle"":0,
                            ""backgroundColor"":[0,0,0,0],
                            ""borderLineColor"":[0,0,0,0],
                            ""borderLineSize"":0,
                            ""color"":[255,0,0,255],
                            ""font"":
                                {
                                    ""decoration"":""none"",
                                    ""size"":10,
                                    ""style"":""normal"",
                                    ""weight"":""normal""
                                },
                            ""haloColor"":[255,255,255,255],
                            ""haloSize"":2,
                            ""horizontalAlignment"":""center"",
                            ""kerning"":false,
                            ""type"":""esriTS"",
                            ""verticalAlignment"":""middle"",
                            ""xoffset"":0,
                            ""yoffset"":0
                        }
               }";

            string blueLabelJson =
                @"{
                    ""labelExpressionInfo"":{""expression"":""$feature.NAME + ' (' + left($feature.PARTY,1) + ')\\nDistrict' + $feature.CDFIPS""},
                    ""labelPlacement"":""esriServerPolygonPlacementAlwaysHorizontal"",
                    ""where"":""PARTY = 'Democrat'"",
                    ""symbol"":
                        { 
                            ""angle"":0,
                            ""backgroundColor"":[0,0,0,0],
                            ""borderLineColor"":[0,0,0,0],
                            ""borderLineSize"":0,
                            ""color"":[0,0,255,255],
                            ""font"":
                                {
                                    ""decoration"":""none"",
                                    ""size"":10,
                                    ""style"":""normal"",
                                    ""weight"":""normal""
                                },
                            ""haloColor"":[255,255,255,255],
                            ""haloSize"":2,
                            ""horizontalAlignment"":""center"",
                            ""kerning"":false,
                            ""type"":""esriTS"",
                            ""verticalAlignment"":""middle"",
                            ""xoffset"":0,
                            ""yoffset"":0
                        }
               }";

            string s3 = @"{
  ""labelExpressionInfo"": 
  {
                ""expression"": ""return $feature.FID;""
  },
  ""labelPlacement"": ""esriServerLinePlacementBelowAlong"",
  ""symbol"": 
  {
                ""color"": [255,0,255,123],
    ""font"": { ""size"": 16 },
    ""type"": ""esriTS""
  }
}";
            
            // Create a label definition from the JSON string. 
            LabelDefinition redLabelDefinition = LabelDefinition.FromJson(redLabelJson);
            LabelDefinition blueLabelDefinition = LabelDefinition.FromJson(blueLabelJson);
            LabelDefinition l3 = LabelDefinition.FromJson(s3);

            // Add the label definition to the feature layer's label definition collection.
            FeatureLayer a = (FeatureLayer)myMapView.Map.OperationalLayers[0];
            a.LabelDefinitions.Add(redLabelDefinition);
            a.LabelDefinitions.Add(blueLabelDefinition);
            a.LabelDefinitions.Add(l3);

            // Enable the visibility of labels to be seen.
            a.LabelsEnabled = true;

        }

        private async void myTestBtn3_Click(object sender, RoutedEventArgs e)
        {
            FeatureLayer a = (FeatureLayer)myMapView.Map.OperationalLayers[0];
            FeatureQueryResult r = await a.GetSelectedFeaturesAsync();
            IEnumerator<Feature> resultFeatures = r.GetEnumerator();
            List<Feature> features = new List<Feature>();
            while (resultFeatures.MoveNext())
            {
                features.Add(resultFeatures.Current);
            }
            myTest.Text = features.Count + " 个";
            Esri.ArcGISRuntime.Geometry.Geometry resultGeometry = GeometryEngine.Union(features[0].Geometry, features[1].Geometry);
            myTest.Text = resultGeometry.IsEmpty + "";
            // 渲染
            SimpleLineSymbol simpleLineSymbol = new SimpleLineSymbol()
            {
                Style = SimpleLineSymbolStyle.Solid,
                Width = 4,
                Color = System.Drawing.Color.Green
            };
            Graphic graphic = new Graphic(resultGeometry, simpleLineSymbol);
            graphicsOverlay.Graphics.Add(graphic);
        }

        // 打开草图编辑工具
        private void btn_DrawAndEdit_Click(object sender, RoutedEventArgs e)
        {
            if (myDrawAndEdit.Visibility != Visibility.Visible)
            {
                closeAllTools();
                myDrawAndEdit.Visibility = Visibility.Visible;
                btn_DrawAndEdit.Header = "隐藏草图编辑工具";
            }
            else
            {
                myDrawAndEdit.Visibility = Visibility.Collapsed;
                btn_DrawAndEdit.Header = "草图编辑工具";
            }
        }

        // 打开测量工具（ArcGIS）
        private void btn_MeasureArcGIS_Click(object sender, RoutedEventArgs e)
        {
            if (myMeasureArcGIS.Visibility != Visibility.Visible)
            {
                closeAllTools();
                myMeasureArcGIS.Visibility = Visibility.Visible;
                btn_MeasureArcGIS.Header = "隐藏测量工具(Toolkit)";
            }
            else
            {
                myMeasureArcGIS.Visibility = Visibility.Collapsed;
                btn_MeasureArcGIS.Header = "测量工具(Toolkit)";
            }
        }

        // 使所有工具条不可见
        public void closeAllTools()
        {
            List<StackPanel> tools = Utils.GetChildNode.GetChildObjects<StackPanel>(userControlsLayoutCsy);
            for (int i = 0; i < tools.Count();i++)
            {
                tools[i].Visibility = Visibility.Collapsed;
            }
        }

        private void btn_Measure_Click(object sender, RoutedEventArgs e)
        {
            if (myMeasure.Visibility == Visibility.Collapsed)
            {
                closeAllTools();
                myMeasure.Visibility = Visibility.Visible;
                btn_Measure.Header = "隐藏测量工具(ArcGIS)";
                this.ShowMessageAsync("提示", "该功能已过时，推荐使用\"编辑工具\"——\"测量工具(Toolkit)\"");
            }
            else if (myMeasure.Visibility == Visibility.Visible)
            {
                myMeasure.Visibility = Visibility.Collapsed;
                btn_Measure.Header = "测量工具(ArcGIS)";
            }
        }

        private void fillHydrology_Click(object sender, RoutedEventArgs e)
        {
            if (myHydroFill.Visibility == Visibility.Collapsed)
            {
                closeAllTools();
                myHydroFill.Visibility = Visibility.Visible;
                fillHydrology.Header = "隐藏填洼";
            }
            else if (myHydroFill.Visibility == Visibility.Visible)
            {
                myHydroFill.Visibility = Visibility.Collapsed;
                fillHydrology.Header = "填洼";
            }
        }

        private void flowDirHydrology_Click(object sender, RoutedEventArgs e)
        {
            if (myHydroFlowDir.Visibility == Visibility.Collapsed)
            {
                closeAllTools();
                myHydroFlowDir.Visibility = Visibility.Visible;
                flowDirHydrology.Header = "隐藏流向";
            }
            else if (myHydroFlowDir.Visibility == Visibility.Visible)
            {
                myHydroFlowDir.Visibility = Visibility.Collapsed;
                flowDirHydrology.Header = "流向";
            }
        }

        private void flowCountHydrology_Click(object sender, RoutedEventArgs e)
        {
            if (myHydroFlowCount.Visibility == Visibility.Collapsed)
            {
                closeAllTools();
                myHydroFlowCount.Visibility = Visibility.Visible;
                flowCountHydrology.Header = "隐藏流量";
            }
            else if (myHydroFlowCount.Visibility == Visibility.Visible)
            {
                myHydroFlowCount.Visibility = Visibility.Collapsed;
                flowCountHydrology.Header = "流量";
            }
        }

        private void conFucHydrology_Click(object sender, RoutedEventArgs e)
        {
            if (myHydroConFuc.Visibility == Visibility.Collapsed)
            {
                closeAllTools();
                myHydroConFuc.Visibility = Visibility.Visible;
                conFucHydrology.Header = "隐藏Con";
            }
            else if (myHydroConFuc.Visibility == Visibility.Visible)
            {
                myHydroConFuc.Visibility = Visibility.Collapsed;
                conFucHydrology.Header = "Con";
            }
        }

        private void stream2FeatureHydrology_Click(object sender, RoutedEventArgs e)
        {
            if (myHydroStream2Featrue.Visibility == Visibility.Collapsed)
            {
                closeAllTools();
                myHydroStream2Featrue.Visibility = Visibility.Visible;
                stream2FeatureHydrology.Header = "隐藏栅格河网矢量化";
            }
            else if (myHydroStream2Featrue.Visibility == Visibility.Visible)
            {
                myHydroStream2Featrue.Visibility = Visibility.Collapsed;
                stream2FeatureHydrology.Header = "栅格河网矢量化";
            }
        }
        
        private void waterShedHydrology_Click(object sender, RoutedEventArgs e)
        {
            if (myHydroWaterShed.Visibility == Visibility.Collapsed)
            {
                closeAllTools();
                myHydroWaterShed.Visibility = Visibility.Visible;
                waterShedHydrology.Header = "隐藏分水岭";
            }
            else if (myHydroWaterShed.Visibility == Visibility.Visible)
            {
                myHydroWaterShed.Visibility = Visibility.Collapsed;
                waterShedHydrology.Header = "分水岭";
            }
        }

        private void Raster2Polygon_Click(object sender, RoutedEventArgs e)
        {
            if (myDataRaster2Polygon.Visibility == Visibility.Collapsed)
            {
                closeAllTools();
                myDataRaster2Polygon.Visibility = Visibility.Visible;
                Raster2Polygon.Header = "隐藏栅格转面";
            }
            else if (myDataRaster2Polygon.Visibility == Visibility.Visible)
            {
                myDataRaster2Polygon.Visibility = Visibility.Collapsed;
                Raster2Polygon.Header = "栅格转面";
            }
        }

        private void btn_RasterClip_Click(object sender, RoutedEventArgs e)
        {
            if (myOtherRasterClip.Visibility == Visibility.Collapsed)
            {
                closeAllTools();
                myOtherRasterClip.Visibility = Visibility.Visible;
                btn_RasterClip.Header = "隐藏栅格裁剪";
            }
            else if (myOtherRasterClip.Visibility == Visibility.Visible)
            {
                myOtherRasterClip.Visibility = Visibility.Collapsed;
                btn_RasterClip.Header = "栅格裁剪";
            }
        }

        private void streamLinkHydrology_Click(object sender, RoutedEventArgs e)
        {
            if (myHydroStreamLink.Visibility == Visibility.Collapsed)
            {
                closeAllTools();
                myHydroStreamLink.Visibility = Visibility.Visible;
                streamLinkHydrology.Header = "隐藏河网连接";
            }
            else if (myHydroStreamLink.Visibility == Visibility.Visible)
            {
                myHydroStreamLink.Visibility = Visibility.Collapsed;
                streamLinkHydrology.Header = "河网连接";
            }
        }

        private void streamLevelHydrology_Click(object sender, RoutedEventArgs e)
        {
            if (myHydroStreamLevel.Visibility == Visibility.Collapsed)
            {
                closeAllTools();
                myHydroStreamLevel.Visibility = Visibility.Visible;
                streamLevelHydrology.Header = "隐藏河网分级";
            }
            else if (myHydroStreamLevel.Visibility == Visibility.Visible)
            {
                myHydroStreamLevel.Visibility = Visibility.Collapsed;
                streamLevelHydrology.Header = "河网分级";
            }
        }

        private void Feature2Raster_Click(object sender, RoutedEventArgs e)
        {
            if (myDataFeature2Raster.Visibility == Visibility.Collapsed)
            {
                closeAllTools();
                myDataFeature2Raster.Visibility = Visibility.Visible;
                Feature2Raster.Header = "隐藏要素转栅格";
            }
            else if (myDataFeature2Raster.Visibility == Visibility.Visible)
            {
                myDataFeature2Raster.Visibility = Visibility.Collapsed;
                Feature2Raster.Header = "要素转栅格";
            }
        }

        private void DBF2Excel_Click(object sender, RoutedEventArgs e)
        {
            if (myDataDBF2Excel.Visibility == Visibility.Collapsed)
            {
                closeAllTools();
                myDataDBF2Excel.Visibility = Visibility.Visible;
                DBF2Excel.Header = "隐藏表转Excel";
            }
            else if (myDataDBF2Excel.Visibility == Visibility.Visible)
            {
                myDataDBF2Excel.Visibility = Visibility.Collapsed;
                DBF2Excel.Header = "表转Excel";
            }
        }

        private void zonalStatisticsHydrology_Click(object sender, RoutedEventArgs e)
        {
            if (myHydroZonalStatistics.Visibility == Visibility.Collapsed)
            {
                closeAllTools();
                myHydroZonalStatistics.Visibility = Visibility.Visible;
                zonalStatisticsHydrology.Header = "隐藏分区统计";
            }
            else if (myHydroZonalStatistics.Visibility == Visibility.Visible)
            {
                myHydroZonalStatistics.Visibility = Visibility.Collapsed;
                zonalStatisticsHydrology.Header = "分区统计";
            }
        }

        private async void ExportImage_Click(object sender, RoutedEventArgs e)
        {
            var img = await myMapView.ExportImageAsync();
            var pngImg = await img.GetEncodedBufferAsync();

            string savePath = "";
            System.Windows.Forms.SaveFileDialog fbd = new System.Windows.Forms.SaveFileDialog()
            {
                Title = "保存为",
                FileName = "myMapView.jpg",
                Filter = "JPG文件|*.jpg;"
            };
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                savePath = fbd.FileName;               
            }

            Bitmap bt = new Bitmap(pngImg); 
            bt.Save(savePath); 
            bt.Dispose();
        }

        private void ExportImageMix_Click(object sender, RoutedEventArgs e)
        {
            string savePath = "";
            System.Windows.Forms.SaveFileDialog fbd = new System.Windows.Forms.SaveFileDialog()
            {
                Title = "保存为",
                FileName = "myMapView.jpg",
                Filter = "JPG文件|*.jpg;"
            };
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (fbd.FileName != "")
                {
                    savePath = fbd.FileName;

                    System.Windows.Point p = myMapViewGrid.PointToScreen(new System.Windows.Point(0, 0));
                    System.Windows.Point pHelp = helpMargin.PointToScreen(new System.Windows.Point(0, 0));
                    int w = (int)(pHelp.X - p.X);
                    int h = (int)(pHelp.Y - p.Y);

                    System.Drawing.Rectangle rc = new System.Drawing.Rectangle((int)p.X, (int)p.Y, w, h);
                    Bitmap bitmap = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(rc.X, rc.Y, 0, 0, rc.Size, CopyPixelOperation.SourceCopy);
                    }
                    Bitmap bt = bitmap;
                    bt.Save(savePath);
                    bt.Dispose();
                }              
            }         
            // 第二种方法
            //ArcGISApp1.Utils.SnapShot.GenerateImage(ArcGISApp1.Utils.SnapShot.RenderVisaulToBitmap(myMapView, 100, 100), @"C:\Users\CSY\Desktop\contour\as1.jpg");
        }

        private void openCompass_Click(object sender, RoutedEventArgs e)
        {
            if (myCompass.Visibility == Visibility.Collapsed)
            {
                closeAllTools();
                myCompass.Visibility = Visibility.Visible;
                openCompass.Header = "隐藏指南针";
            }
            else
            {
                myCompass.Visibility = Visibility.Collapsed;
                openCompass.Header = "指南针";
            }
        }

        private void openScaleLine_Click(object sender, RoutedEventArgs e)
        {
            if (myScaleLine.Visibility == Visibility.Collapsed)
            {
                closeAllTools();
                myScaleLine.Visibility = Visibility.Visible;
                openScaleLine.Header = "隐藏比例尺";
            }
            else
            {
                myScaleLine.Visibility = Visibility.Collapsed;
                openScaleLine.Header = "比例尺";
            }
        }

        private void openLegend_Click(object sender, RoutedEventArgs e)
        {
            if (myLegend.Visibility == Visibility.Collapsed)
            {
                closeAllTools();
                myLegend.Visibility = Visibility.Visible;
                openLegend.Header = "隐藏图例";
            }
            else
            {
                myLegend.Visibility = Visibility.Collapsed;
                openLegend.Header = "重载图例";
            }
            myLegend.GeoView = null;
            myLegend.GeoView = myMapView;
        }

        private void openLayerLegend_Click(object sender, RoutedEventArgs e)
        {
            if (myLayerLegend.Visibility == Visibility.Collapsed)
            {
                closeAllTools();
                myLayerLegend.Visibility = Visibility.Visible;
                openLayerLegend.Header = "隐藏图层图例";
                myLayerLegendItems.ItemsSource = null;
                myLayerLegendItems.ItemsSource = myMapView.Map.OperationalLayers;
            }
            else
            {
                myLayerLegend.Visibility = Visibility.Collapsed;
                openLayerLegend.Header = "重载图层图例";
            }
        }

        private void openGrid_Click(object sender, RoutedEventArgs e)
        {
            if (myOtherGrid.Visibility == Visibility.Collapsed)
            {
                closeAllTools();
                myOtherGrid.Visibility = Visibility.Visible;
                openGrid.Header = "隐藏格网";
            }
            else
            {
                myOtherGrid.Visibility = Visibility.Collapsed;
                openGrid.Header = "格网";
            }
        }

        private void btn_GeometryCalcuEditCky_Click(object sender, RoutedEventArgs e)
        {
            if (cykGeometryCalcuEdit.Visibility == Visibility.Collapsed)
            {
                closeAllTools();
                cykGeometryCalcuEdit.Visibility = Visibility.Visible;
                btn_GeometryCalcuEditCky.Header = "几何操作与要素编辑";
            }
            else
            {
                cykGeometryCalcuEdit.Visibility = Visibility.Visible;
                btn_GeometryCalcuEditCky.Header = "几何操作与要素编辑";
            }
        }

        //// 几何操作工具需要的(废弃)
        //private void myMapView_Loaded(object sender, RoutedEventArgs e)
        //{

        //}

        // 图名
        #region
        private void mapTitle_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            SolidColorBrush myBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 100, 183, 134));
            mapTitle.Background = (System.Windows.Media.Brush)myBrush;
        }

        private void mapTitle_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            mapTitle.Background = System.Windows.Media.Brushes.Gray;
        }

        private void mapTitle_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Canvas.SetLeft(mapTitle, Canvas.GetLeft(mapTitle) + e.HorizontalChange);
            Canvas.SetTop(mapTitle, Canvas.GetTop(mapTitle) + e.VerticalChange);
            Canvas.SetLeft(myMapTitleTxt, Canvas.GetLeft(myMapTitleTxt) + e.HorizontalChange);
            Canvas.SetTop(myMapTitleTxt, Canvas.GetTop(myMapTitleTxt) + e.VerticalChange);
        }
        

        private void label1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Label lbl = (Label)sender;
            DragDrop.DoDragDrop(lbl, lbl.Content, DragDropEffects.Copy);
        }

        private void openTitle_Click(object sender, RoutedEventArgs e)
        {
            if (MapTitleTool.Visibility == Visibility.Collapsed)
            {
                MapTitleTool.Visibility = Visibility.Visible;
                openTitle.Header = "隐藏图名工具";
            }
            else
            {
                MapTitleTool.Visibility = Visibility.Collapsed;
                openTitle.Header = "图名";
            }
        }
        #endregion

        private void AddPy_Click(object sender, RoutedEventArgs e)
        {
            if (AddPythonTool.Visibility == Visibility.Collapsed)
            {
                AddPythonTool.Visibility = Visibility.Visible;
                AddPy.Header = "隐藏使用外部Python脚本";
            }
            else
            {
                AddPythonTool.Visibility = Visibility.Collapsed;
                AddPy.Header = "使用外部Python脚本";
            }
        }

        // 主窗体关闭事件
        private async void myApp_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Boolean _shutdown = false;
            e.Cancel = !_shutdown;
            if (_shutdown) return;
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "退出",
                NegativeButtonText = "取消",
                AnimateShow = true,
                AnimateHide = false
            };
            var result = await this.ShowMessageAsync("退出?",
                "确定要退出系统吗?",
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            _shutdown = result == MessageDialogResult.Affirmative;
            if (_shutdown)
                Application.Current.Shutdown();        
        }

        private void myHelp_Click(object sender, RoutedEventArgs e)
        {
            string detailHelpMessage = @"[1] 请确保已安装本系统所需的软件环境：

框架：.NET Framework 4.6.1及以上；
DirectX：DirectX 11 及以上；
Python：2.7版本；
Visual Studio：2017社区版及以上；
ArcGIS Desktop：10.5版本（含所有扩展许可）；
ArcGIS Runtime SDK for WPF：100.3版本；
ArcGIS Runtime Local Server Utility：100.0版本；

[2] 系统功能分布
功能分布在三个模块：界面上方的菜单栏，界面左侧的导航栏，以及图层右键菜单；

[3] 更多帮助信息请查看本系统的技术文档，感谢您的使用！

--------------------------------------------------------------------------
作者：***
学号：************
日期：20181126

";
            this.ShowMessageAsync("帮助", detailHelpMessage);
        }

        private async void AddGeodatabase_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openDlg = new System.Windows.Forms.OpenFileDialog();
            openDlg.Filter = "GeoDatabase File(*.geodatabase)|*.geodatabase";
            openDlg.Title = "Open GeoDatabase File";
            if (openDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                myMapView2.Visibility = Visibility.Visible;
                myMapView.Visibility = Visibility.Collapsed;


                string path = openDlg.FileName;
                try
                {
                    myTest.Text = "Geodatabase数据加载中";
                    myDatbase = await Geodatabase.OpenAsync(path);//打开数据库                     
                    myTables = myDatbase.GeodatabaseFeatureTables;//获取所有表对象                     
                    //遍历所有表对象 
                    foreach (GeodatabaseFeatureTable t in myTables)
                    {
                        await t.LoadAsync();//加载数据                       
                        FeatureLayer ly = new FeatureLayer(t);//构造要素图层   
                        await myMapView2.SetViewpointGeometryAsync(ly.FullExtent);
                        myMapView2.Map.OperationalLayers.Add(ly); //添加图层到地图控件中 
                    }
                    AddGeodatabase.IsEnabled = false;//打开菜单失效 
                    myTest.Text = "";
                    // CloseGDB.IsEnabled = true;//关闭菜单有效 
                }
                catch (Exception ex)
                {
                    myTest.Text = "";
                    MessageBox.Show(ex.ToString(), "Error");
                }              
            }
        }

        private void AddArcGISonline_Click(object sender, RoutedEventArgs e)
        {
            if (ArcgisOnlineMapPanel.Visibility == Visibility.Collapsed)
            {
                ArcgisOnlineMapPanel.Visibility = Visibility.Visible;
            } else
            {
                ArcgisOnlineMapPanel.Visibility = Visibility.Collapsed;
            }
        }
    }   
}

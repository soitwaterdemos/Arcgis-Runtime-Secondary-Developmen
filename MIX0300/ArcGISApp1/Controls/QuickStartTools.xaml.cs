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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArcGISApp1.Controls
{
    /// <summary>
    /// QuickStartTools.xaml 的交互逻辑
    /// </summary>
    public partial class QuickStartTools : UserControl
    {
        Window w;
        MapView myMapView;
        Rectangle dragSelectRectangle;
        TextBlock t1;
        System.Windows.Controls.Grid myMapViewGrid;
        MapViewAutomationPeer mapViewAutomationPeer;

        public QuickStartTools()
        {
            InitializeComponent();
        }

        private void eagleEyeCsy_Click(object sender, RoutedEventArgs e)
        {
            MapView myMapView_Eagle = (MapView)w.FindName("myMapView_Eagle");
            myMapView_Eagle.Visibility = myMapView_Eagle.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            eagleEyeCsy.ToolTip = ((string)eagleEyeCsy.ToolTip == "打开鹰眼地图") ? "关闭鹰眼地图" : "打开鹰眼地图";
        }

        private void myUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            w = Window.GetWindow(this);
            myMapView = (MapView)w.FindName("myMapView");
            dragSelectRectangle = (Rectangle)w.FindName("dragSelectRectangle");
            myMapViewGrid = (System.Windows.Controls.Grid)w.FindName("myMapViewGrid");
            t1 = (TextBlock)w.FindName("myTest");
            mapViewAutomationPeer = new MapViewAutomationPeer(myMapView);
        }

        private async void openGpCsy_Click(object sender, RoutedEventArgs e)
        {
            openGpCsy.ToolTip = "LocalServer已启动";

            // 启动localserver
            try
            {
                await MainWindow._localServer.StartAsync();
            }
            catch (Exception ex1)
            {
                MessageBox.Show("localserver启动错误：\n" + ex1.Message);
            }
            ((TextBlock)w.FindName("myTest")).Text = "localserver启动成功";
            ((MenuItem)w.FindName("fillHydrology")).IsEnabled = true;
            ((MenuItem)w.FindName("flowDirHydrology")).IsEnabled = true;
            ((MenuItem)w.FindName("flowCountHydrology")).IsEnabled = true;
            ((MenuItem)w.FindName("conFucHydrology")).IsEnabled = true;
            ((MenuItem)w.FindName("stream2FeatureHydrology")).IsEnabled = true;
            ((MenuItem)w.FindName("waterShedHydrology")).IsEnabled = true;
            ((MenuItem)w.FindName("Raster2Polygon")).IsEnabled = true;
            ((MenuItem)w.FindName("btn_RasterClip")).IsEnabled = true;
            ((MenuItem)w.FindName("streamLinkHydrology")).IsEnabled = true;
            ((MenuItem)w.FindName("streamLevelHydrology")).IsEnabled = true;
            ((MenuItem)w.FindName("Feature2Raster")).IsEnabled = true;
            ((MenuItem)w.FindName("DBF2Excel")).IsEnabled = true;
            ((MenuItem)w.FindName("zonalStatisticsHydrology")).IsEnabled = true;
        }

        private void dragSelectCsy_Click(object sender, RoutedEventArgs e)
        {
            if ((string)dragSelectCsy.ToolTip == "启动要素多选")
            {
                myMapView.GeoViewTapped += MainWindow_Click;
                dragSelectCsy.ToolTip = "关闭要素多选";
                //myMapView.GeoViewTapped -= myMapViewGetAttributeMutilValue;
                //myMapView.GeoViewTapped += myMapViewGetAttributeMutilValue;
                t1.Text = "在MapView视图中,左键单击确认包围盒端点1,移动鼠标后再左键单击确认端点2并完成包围盒; 再次点击本按钮可关闭\"要素多选\"";
            }
            else
            {
                dragSelectCsy.ToolTip = "启动要素多选";
                myMapView.GeoViewTapped -= MainWindow_Click;
            }
            
        }
        
        private MapPoint _downPoint;
        //private Point _downPointP;
        private int count = 0;
        private async void MainWindow_Click(object sender, Esri.ArcGISRuntime.UI.Controls.GeoViewInputEventArgs e)
        {
            if (count == 0)
            {
                count = 1;
                _downPoint = e.Location;
                //_downPointP = e.Position;
                //dragSelectRectangle.Margin = new Thickness(0, 0, 0, 0);
                //dragSelectRectangle.Width = 0;
                //dragSelectRectangle.Height = 0;
            }
            else
            {
                count = 0;
                var _endpoint = e.Location;

                try
                {
                    double tolerance = 0.0001;
                    double mapTolerance = tolerance /** myMapView.UnitsPerPixel*/;
                    MapPoint geometry = new MapPoint((_downPoint.X + _endpoint.X) / 2, (_downPoint.Y + _endpoint.Y) / 2, myMapView.SpatialReference);
                    if (myMapView.IsWrapAroundEnabled)
                    {
                        geometry = (MapPoint)GeometryEngine.NormalizeCentralMeridian(geometry);
                    }
                    Envelope selectionEnvelope = new Envelope(geometry, Math.Abs(_downPoint.X - _endpoint.X), Math.Abs(_downPoint.Y - _endpoint.Y)/*geometry.X - mapTolerance, geometry.Y - mapTolerance, geometry.X + mapTolerance, geometry.Y + mapTolerance,*/ /*myMapView.Map.SpatialReference*/);
                    QueryParameters queryParams = new QueryParameters()
                    {
                        Geometry = selectionEnvelope
                    };                   

                    FeatureLayer tempLayer = (FeatureLayer)(myMapView.Map.OperationalLayers[0]);
                    FeatureQueryResult fr = await tempLayer.SelectFeaturesAsync(queryParams, Esri.ArcGISRuntime.Mapping.SelectionMode.New);
                    IEnumerator<Feature> frr = fr.GetEnumerator();
                    List<Feature> features = new List<Feature>();
                    while (frr.MoveNext())
                    {
                        features.Add(frr.Current);
                    }

                    // 查看属性
                    Esri.ArcGISRuntime.Data.FeatureTable tempTable = (Esri.ArcGISRuntime.Data.FeatureTable)tempLayer.FeatureTable;
                    long row = tempTable.NumberOfFeatures;
                    int col = tempTable.Fields.Count;
                    List<String> fieldNames = new List<string>();
                    for (int i = 0; i < col; i++)
                    {
                        fieldNames.Add(tempTable.Fields[i] + "");
                    }

                    StackPanel stackPanel = new StackPanel();
                    WrapPanel[] wrapPanels = new WrapPanel[row];

                    // 字段名
                    WrapPanel wrapPanelField = new WrapPanel()
                    {
                        Margin = new Thickness()
                        {
                            Left = 10,
                            Top = 1,
                            Right = 10,
                            Bottom = 1
                        }
                    };
                    for (int i = 0; i < col; i++)
                    {
                        Button button = new Button()
                        {
                            Content = fieldNames[i],
                            ToolTip = fieldNames[i],
                            Width = 60,
                            Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 100, 183, 134))
                        };
                        wrapPanelField.Children.Add(button);
                    }
                    stackPanel.Children.Add(wrapPanelField);

                    // 记录
                    for (int i = 0; i < row; i++)
                    {
                        wrapPanels[i] = new WrapPanel()
                        {
                            Margin = new Thickness()
                            {
                                Left = 10,
                                Top = 1,
                                Right = 10,
                                Bottom = 1
                            }
                        };
                        for (int j = 0; j < col; j++)
                        {
                            Button button = new Button()
                            {
                                Width = 60,
                                Content = features[i].GetAttributeValue(fieldNames[j]),
                                ToolTip = features[i].GetAttributeValue(fieldNames[j])
                            };
                            wrapPanels[i].Children.Add(button);
                        }
                        stackPanel.Children.Add(wrapPanels[i]);
                    }

                    ScrollViewer scrollViewer = new ScrollViewer();
                    scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                    scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                    scrollViewer.Content = stackPanel;

                    var window = new Window();
                    window.Content = scrollViewer;
                    window.SizeToContent = SizeToContent.WidthAndHeight;
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    window.MaxHeight = 700;
                    window.MaxWidth = 1000;

                    window.Title = "要素多选属性表";
                    window.Show();
                }
                catch
                {
                    t1.Text = "要素多选发生错误";
                }                           
            }           
        }

        // 清楚选择要素
        private void cleanSelectCsy_Click(object sender, RoutedEventArgs e)
        {
            int len = myMapView.Map.OperationalLayers.Count;
            for (int i = 0; i < len;i++)
            {
                FeatureLayer featureLayer =  (FeatureLayer)myMapView.Map.OperationalLayers[i];
                featureLayer.ClearSelection();

                //GraphicsOverlay g = (GraphicsOverlay)myMapView.GraphicsOverlays[i];
                //myMapView.GraphicsOverlays.
            }
        }

        // 清除所有绘制图层
        private void cleanGraphicCsy_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < myMapView.GraphicsOverlays.Count; i++)
            {
                myMapView.GraphicsOverlays[i].Opacity = 0;
            }
        }

        // 缩进
        private void zoomInCsy_Click(object sender, RoutedEventArgs e)
        {
            int x = (int)myMapView.Width / 2;
            int y = (int)myMapView.Height / 2;
            System.Windows.Point point = new System.Windows.Point(x,y);
            mapViewAutomationPeer.ZoomOutAnimated(point);
        }

        // 放大
        private void zoomOutCsy_Click(object sender, RoutedEventArgs e)
        {
            int x = (int)myMapView.Width / 2;
            int y = (int)myMapView.Height / 2;
            System.Windows.Point point = new System.Windows.Point(x, y);
            mapViewAutomationPeer.ZoomInAnimated(point);
        }

        private void clearToolsCsy_Click(object sender, RoutedEventArgs e)
        {
            Canvas userControlsLayoutCsy = (Canvas)w.FindName("userControlsLayoutCsy");
            List<StackPanel> tools = ArcGISApp1.Utils.GetChildNode.GetChildObjects<StackPanel>(userControlsLayoutCsy);
            for (int i = 0; i < tools.Count(); i++)
            {
                tools[i].Visibility = Visibility.Collapsed;
            }
        }

        private void changeMapCsy_Click(object sender, RoutedEventArgs e)
        {
            MapView myMapView2 = (MapView)w.FindName("myMapView2");
            if (myMapView.Visibility == Visibility.Visible)
            {
                myMapView.Visibility = Visibility.Collapsed;
                myMapView2.Visibility = Visibility.Visible;
            }
            else
            {
                myMapView.Visibility = Visibility.Visible;
                myMapView2.Visibility = Visibility.Collapsed;
            }
        }
    }
}

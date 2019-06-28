using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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
    /// TableOfContent.xaml 的交互逻辑
    /// </summary>
    public partial class TableOfContent : UserControl
    {
        // 符号与渲染器的当前图层
        public static FeatureLayer currentMyMapViewLayer;
        // 主窗体
        Window mainWindow;
        // 主地图
        MapView myMapView;
        // 全部ckeckbox控件
        public List<CheckBox> check = new List<CheckBox>();
        // 图层控制中checkbox即图层的索引
        public int layerIndex = 0;
        // 计数变量
        private int i = 1;

        public TableOfContent()
        {
            InitializeComponent();
        }

        // 清空工具条
        public void closeAll()
        {
            Canvas userControlsLayoutCsy = (Canvas)mainWindow.FindName("userControlsLayoutCsy");
            List<StackPanel> tools = ArcGISApp1.Utils.GetChildNode.GetChildObjects<StackPanel>(userControlsLayoutCsy);
            for (int i = 0; i < tools.Count(); i++)
            {
                tools[i].Visibility = Visibility.Collapsed;
            }

        }

        // 用户控件加载完成后
        private void myUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            mainWindow = Window.GetWindow(this);
            myMapView = (MapView)mainWindow.FindName("myMapView");
        }

        // 添加图层管理checkbox控件
        public void addLayersList (String layerName)
        {
            CheckBox cb = new CheckBox()
            {
                Margin = new Thickness()
                {
                    Left = 0,
                    Top = 5,
                    Right = 0,
                    Bottom = 5
                }
            };
            // 图层名字
            cb.Content = layerName;
            cb.ToolTip = cb.Content;
            //图层名字, 即图层索引,名字可能不能以数字开头
            cb.Name = "myLayersCkeckbox" + layerIndex;
            // 默认选中状态
            cb.IsChecked = true;
            // 禁用聚焦
            cb.Focusable = false;
            // 因为Checked事件只对true有反应, 所以改成了Click
            cb.Checked += new RoutedEventHandler(Checked_Layers_CheckBox);
            cb.Unchecked += new RoutedEventHandler(UnChecked_Layers_CheckBox);

            #region 右键菜单事件
            // 右键删除功能
            ContextMenu cm = new ContextMenu()
            {
                FontSize = 10
            };
            MenuItem mi_delete = new MenuItem();
            mi_delete.Header = "删除";
            mi_delete.Click += Deleted_Layers_CheckBox;
            cm.Items.Add(mi_delete);
            //// 隐藏功能
            //MenuItem mi_Hide = new MenuItem();
            //mi_Hide.Header = "隐藏";
            //mi_Hide.Click += Hide_Layers_CheckBox;
            //cm.Items.Add(mi_Hide);
            // 上移功能
            MenuItem mi_Up = new MenuItem();
            mi_Up.Header = "上移";
            mi_Up.Click += Up_Layers_CheckBox;
            cm.Items.Add(mi_Up);
            // 下移功能
            MenuItem mi_Down = new MenuItem();
            mi_Down.Header = "下移";
            mi_Down.Click += Down_Layers_CheckBox;
            cm.Items.Add(mi_Down);

            // 文件后缀
            string fileType = layerName.Substring(layerName.Length - 3);
            
            // 缩放至功能
            MenuItem mi_extend = new MenuItem();
            mi_extend.Header = "缩放至";
            if ((fileType == "shp" || fileType == "SHP" )|| (fileType == "tif" || fileType == "iff" || fileType == "TIF" || fileType == "IFF") )
            {
                mi_extend.Click += Extend_Layers_CheckBox;
                cm.Items.Add(mi_extend);
            }

            // 分割线
            Separator separator0 = new Separator();
            cm.Items.Add(separator0);
        
            // 符号渲染功能
            MenuItem mi_Symbol = new MenuItem();
            mi_Symbol.Header = "符号渲染";
            //string fileType = layerName.Substring(layerName.Length - 3);
            if (fileType == "shp" || fileType == "SHP")
            {
                mi_Symbol.Click += ChangeSymbol_Layers_CheckBox;
                cm.Items.Add(mi_Symbol);
            }

            // shp唯一值渲染
            MenuItem mi_shpUnique = new MenuItem();
            mi_shpUnique.Header = "唯一值渲染";
            fileType = layerName.Substring(layerName.Length - 3);
            if (fileType == "shp" || fileType == "SHP")
            {
                mi_shpUnique.Click += SHPuniqueRender_Layers_CheckBox;
                cm.Items.Add(mi_shpUnique);
            }

            // 栅格渲染
            MenuItem mi_RasterRGB = new MenuItem();
            mi_RasterRGB.Header = "栅格渲染";
            fileType = layerName.Substring(layerName.Length - 3);
            if (fileType == "TIF" || fileType == "tif")
            {
                mi_RasterRGB.Click += OpenRasterRGB_Layers_CheckBox;
                cm.Items.Add(mi_RasterRGB);
            }

            // 分割线
            if (fileType == "shp" || fileType == "SHP")
            {
                Separator separator1 = new Separator();
                cm.Items.Add(separator1);
            }
                

            // 打开属性表Popup功能
            MenuItem mi_FeatureTable = new MenuItem();
            mi_FeatureTable.Header = "要素点选(Popup)";
            // shapefile文件才支持打开属性表        
            fileType = layerName.Substring(layerName.Length - 3);
            if ( fileType == "shp" || fileType == "SHP")
            {
                mi_FeatureTable.Click += OpenFeatureTable_Layers_CheckBox;
                cm.Items.Add(mi_FeatureTable);
            }
            

            // 打开属性表功能
            MenuItem mi_FeatureTableWindow = new MenuItem();
            mi_FeatureTableWindow.Header = "查看属性表";
            // shapefile文件才支持打开属性表          
            fileType = layerName.Substring(layerName.Length - 3);
            if (fileType == "shp" || fileType == "SHP")
            {
                mi_FeatureTableWindow.Click += OpenFeatureTableWindow_Layers_CheckBox;
                cm.Items.Add(mi_FeatureTableWindow);
            }

            // 属性查询功能
            MenuItem mi_AttrSearch = new MenuItem();
            mi_AttrSearch.Header = "属性查询";
            // shapefile文件才支持打开属性表          
            fileType = layerName.Substring(layerName.Length - 3);
            if (fileType == "shp" || fileType == "SHP")
            {
                mi_AttrSearch.Click += OpenAttrSelect_Layers_CheckBox;
                cm.Items.Add(mi_AttrSearch);
            }

            // 分割线
            if (fileType == "shp" || fileType == "SHP")
            {
                Separator separator2 = new Separator();
                cm.Items.Add(separator2);
            }

            // 要素合并
            MenuItem mi_MergeSHP = new MenuItem();
            mi_MergeSHP.Header = "要素合并";
            fileType = layerName.Substring(layerName.Length - 3);
            if (fileType == "shp" || fileType == "SHP")
            {
                mi_MergeSHP.Click += MergeSHP_Layers_CheckBox;
                cm.Items.Add(mi_MergeSHP);
            }

            // 添加标注
            MenuItem mi_LabelDefinition = new MenuItem();
            mi_LabelDefinition.Header = "添加标注";
            fileType = layerName.Substring(layerName.Length - 3);
            if (fileType == "shp" || fileType == "SHP")
            {
                mi_LabelDefinition.Click += LabelDefinition_Layers_CheckBox;
                cm.Items.Add(mi_LabelDefinition);
            }

            // 要素编辑
            MenuItem mi_FeatureEdit = new MenuItem();
            mi_FeatureEdit.Header = "要素编辑";
            if (fileType == "shp" || fileType == "SHP")
            {
                //mi_FeatureEdit.Click += FeatureEdit_Layers_CheckBox;
                //cm.Items.Add(mi_FeatureEdit);
            }

            cm.Name = "myLayersCtxtMenu" + layerIndex + "";
            cb.ContextMenu = cm;
            check.Add(cb);

            #endregion
            
            // 在图层控制添加checkbox（方向）
            myLayerList.Items.Insert(0, cb);
            // 图层索引自加——放在最后
            layerIndex++;
        }
        
        // 当前需Popup的图层（有且最多一个）
        FeatureLayer tempPopupLayer;
        private void OpenFeatureTable_Layers_CheckBox(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            ContextMenu cm = menuItem.Parent as ContextMenu;
            int index = int.Parse(cm.Name.Substring(16));
            tempPopupLayer = (FeatureLayer)myMapView.Map.OperationalLayers[index];

            TextBlock myTest = (TextBlock)mainWindow.FindName("myTest");
            myTest.Text = "提示: 图层右键菜单可关闭\"要素点选(Popup)\"";

            if ((string)menuItem.Header == "要素点选(Popup)")
            {
                // 必要的检查，防止事件重复注册
                myMapView.GeoViewTapped -= myMapViewGetAttributeValue;
                myMapView.GeoViewTapped += myMapViewGetAttributeValue;
                menuItem.Header = "关闭要素点选(Popup)";               
            }
            else
            {
                myMapView.GeoViewTapped -= myMapViewGetAttributeValue;
                menuItem.Header = "要素点选(Popup)";
            }
        }
        
        // 点选Popup属性
        private async void myMapViewGetAttributeValue(object sender, Esri.ArcGISRuntime.UI.Controls.GeoViewInputEventArgs e)
        {
            IdentifyLayerResult myShapeFileResult = await myMapView.IdentifyLayerAsync(tempPopupLayer, e.Position, 20, false);
            try
            {
                Feature tempGeoElement = (Feature)myShapeFileResult.GeoElements[0];
                // 遍历全部属性
                List<ArcGISApp1.Utils.ShapefileAttribution> attrList = new List<ArcGISApp1.Utils.ShapefileAttribution>();
                foreach (KeyValuePair<string, object> keyValuePair in tempGeoElement.Attributes)
                {
                    ArcGISApp1.Utils.ShapefileAttribution temp = new ArcGISApp1.Utils.ShapefileAttribution(keyValuePair.Key, (keyValuePair.Value).ToString());                   
                    attrList.Add(temp);
                }
                Popup myPopup = (Popup)mainWindow.FindName("myPopup");
                DataGrid myDataGrid = (DataGrid)mainWindow.FindName("myDataGrid");
                myPopup.IsOpen = false;
                myPopup.IsOpen = true;
                myDataGrid.AutoGenerateColumns = false;
                myDataGrid.ItemsSource = attrList;
            }
            catch
            {
                TextBlock myTest = (TextBlock)mainWindow.FindName("myTest");
                myTest.Text = "当前无要素被选中。";
            }

            // 要素选择高亮
            try
            {
                double tolerance = 0.0000001;              
                double mapTolerance = tolerance * myMapView.UnitsPerPixel;
                MapPoint geometry = e.Location;
                
                if (myMapView.IsWrapAroundEnabled)
                {
                    geometry = (MapPoint)GeometryEngine.NormalizeCentralMeridian(geometry);
                }
                Envelope selectionEnvelope = new Envelope(geometry.X - mapTolerance, geometry.Y - mapTolerance, geometry.X + mapTolerance, geometry.Y + mapTolerance, myMapView.Map.SpatialReference);              
                QueryParameters queryParams = new QueryParameters
                {
                    Geometry = selectionEnvelope
                    //Geometry = geometry
                };
                FeatureQueryResult queryResult =  await tempPopupLayer.SelectFeaturesAsync(queryParams, Esri.ArcGISRuntime.Mapping.SelectionMode.New);
             
                //IEnumerator<Feature> resultFeatures = queryResult.GetEnumerator();
                //List<Feature> features = new List<Feature>();
                //while (resultFeatures.MoveNext())
                //{
                //    features.Add(resultFeatures.Current);
                //}
                //MessageBox.Show(geometry.X + "\n" + geometry.Y + "\n" + features.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show("要素选择错误: ", ex.ToString());
            }
        }

        // 打开属性查询
        private async void OpenAttrSelect_Layers_CheckBox(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            ContextMenu cm = menuItem.Parent as ContextMenu;
            int index = int.Parse(cm.Name.Substring(16));

            StackPanel stackPanel = (StackPanel)mainWindow.FindName("myOtherAttrSearch");
            // 关闭其余工具条
            Canvas userControlsLayoutCsy = (Canvas)mainWindow.FindName("userControlsLayoutCsy");
            List<StackPanel> tools = ArcGISApp1.Utils.GetChildNode.GetChildObjects<StackPanel>(userControlsLayoutCsy);
            for (int i = 0; i < tools.Count(); i++)
            {
                tools[i].Visibility = Visibility.Collapsed;
            }
            stackPanel.Visibility = Visibility.Visible;

            if ((string)menuItem.Header == "属性查询")
            {
                TextBlock myTest = (TextBlock)mainWindow.FindName("myTest");
                myTest.Text = "提示: 图层右键菜单可关闭\"属性查询\"";

                ArcGISApp1.Controls.OtherTool.AttrSearchCsy.index = index;              
                stackPanel.Visibility = Visibility.Visible;
                menuItem.Header = "关闭属性查询";
            }
            else
            {
                stackPanel.Visibility = Visibility.Collapsed;
                menuItem.Header = "属性查询";
            }

        }

        // 打开属性表
        private async void OpenFeatureTableWindow_Layers_CheckBox(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            ContextMenu cm = menuItem.Parent as ContextMenu;
            int index = int.Parse(cm.Name.Substring(16));
            FeatureLayer tempLayer = (FeatureLayer)myMapView.Map.OperationalLayers[index];
            Esri.ArcGISRuntime.Data.FeatureTable tempTable = tempLayer.FeatureTable;

            QueryParameters query = new QueryParameters();
            query.WhereClause = string.Format("upper(FID) >= 0");
            FeatureQueryResult queryResult = await tempTable.QueryFeaturesAsync(query);
            IEnumerator<Feature> resultFeatures = queryResult.GetEnumerator();
            List<Feature> features = new List<Feature>();
            while (resultFeatures.MoveNext())
            {
                features.Add(resultFeatures.Current);
            }

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
            for (int i = 0; i< col; i++)
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
            for (int i = 0;i < row;i++)
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
                for (int j = 0;j < col;j++)
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

            window.Title = check[index].Content + "属性表";
            window.Show();
        }

        // 符号渲染
        private void ChangeSymbol_Layers_CheckBox(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            // 获取父控件
            ContextMenu cm = menuItem.Parent as ContextMenu;
            //cm.Visibility = Visibility.Hidden;
            int index = int.Parse(cm.Name.Substring(16));
            currentMyMapViewLayer = (FeatureLayer)myMapView.Map.OperationalLayers[index]; // 需要检查是否是矢量图层
            if (currentMyMapViewLayer.FeatureTable.GeometryType == Esri.ArcGISRuntime.Geometry.GeometryType.Point)
            {
                var window = new Window();
                Controls.SymbolFormCSY.PointCsy pointForm = new Controls.SymbolFormCSY.PointCsy();
                window.Content = pointForm;
                window.SizeToContent = SizeToContent.WidthAndHeight;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Title = "点符号渲染工具";
                window.Closed += windowClosed_Point;
                window.Show();
            }
            else if (currentMyMapViewLayer.FeatureTable.GeometryType == Esri.ArcGISRuntime.Geometry.GeometryType.Polyline)
            {
                var window = new Window();
                Controls.SymbolFormCSY.LineCsy lineForm = new Controls.SymbolFormCSY.LineCsy();
                window.Content = lineForm;
                window.SizeToContent = SizeToContent.WidthAndHeight;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Title = "线符号渲染工具";
                window.Closed += windowClosed_Line;
                window.Show();
            }
            else if (currentMyMapViewLayer.FeatureTable.GeometryType == Esri.ArcGISRuntime.Geometry.GeometryType.Polygon)
            {
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

        // 唯一值渲染
        private void SHPuniqueRender_Layers_CheckBox(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            ContextMenu cm = menuItem.Parent as ContextMenu;
            int index = int.Parse(cm.Name.Substring(16));

            // 获取字段
            FeatureLayer tempLayer = (FeatureLayer)myMapView.Map.OperationalLayers[index];
            Esri.ArcGISRuntime.Data.FeatureTable tempTable = tempLayer.FeatureTable;
            List<string> fieldNames = new List<string>();
            for (int i = 0; i < tempTable.Fields.Count; i++)
            {
                fieldNames.Add(tempTable.Fields[i] + "");
            }
            ArcGISApp1.Symbols.ShpUniqueRenderCsy.fieldSource = fieldNames;
            ArcGISApp1.Symbols.ShpUniqueRenderCsy.featureTable = tempTable;
            ArcGISApp1.Symbols.ShpUniqueRenderCsy.layer = tempLayer;

            StackPanel s = (StackPanel)mainWindow.FindName("SHPuniqueRenderCsy");
            // 关闭其余工具条
            Canvas userControlsLayoutCsy = (Canvas)mainWindow.FindName("userControlsLayoutCsy");
            List<StackPanel> tools = ArcGISApp1.Utils.GetChildNode.GetChildObjects<StackPanel>(userControlsLayoutCsy);
            for (int i = 0; i < tools.Count(); i++)
            {
                tools[i].Visibility = Visibility.Collapsed;
            }
            if (s.Visibility != Visibility.Visible)
            {
                s.Visibility = Visibility.Visible;
            }
        }

        // 栅格渲染
        private void OpenRasterRGB_Layers_CheckBox(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            ContextMenu cm = menuItem.Parent as ContextMenu;
            int index = int.Parse(cm.Name.Substring(16));
            StackPanel s = (StackPanel)mainWindow.FindName("RasterRGBRenderCsy");
            s.Visibility = Visibility.Visible;
            ArcGISApp1.Symbols.RasterRGBRenderCsy._rasterLayer = (RasterLayer)myMapView.Map.OperationalLayers[index];
        }

        // 要素合并
        private void MergeSHP_Layers_CheckBox(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            ContextMenu cm = menuItem.Parent as ContextMenu;
            int index = int.Parse(cm.Name.Substring(16));
            StackPanel s = (StackPanel)mainWindow.FindName("UnionSHPTool");
            // 关闭其余工具条
            Canvas userControlsLayoutCsy = (Canvas)mainWindow.FindName("userControlsLayoutCsy");
            List<StackPanel> tools = ArcGISApp1.Utils.GetChildNode.GetChildObjects<StackPanel>(userControlsLayoutCsy);
            for (int i = 0; i < tools.Count(); i++)
            {
                tools[i].Visibility = Visibility.Collapsed;
            }
            s.Visibility = Visibility.Visible;
            ArcGISApp1.Controls.OtherTool.UnionCsy.layer = (FeatureLayer)myMapView.Map.OperationalLayers[index];
        }

        // 添加标注
        private void LabelDefinition_Layers_CheckBox(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            ContextMenu cm = menuItem.Parent as ContextMenu;
            int index = int.Parse(cm.Name.Substring(16));

            // 获取字段
            FeatureLayer tempLayer = (FeatureLayer)myMapView.Map.OperationalLayers[index];
            Esri.ArcGISRuntime.Data.FeatureTable tempTable = tempLayer.FeatureTable;
            List<string> fieldNames = new List<string>();
            for (int i = 0; i < tempTable.Fields.Count; i++)
            {
                fieldNames.Add(tempTable.Fields[i] + "");
            }
            ArcGISApp1.Controls.OtherTool.LabelDefinedCsy.fieldSource = fieldNames;
            ArcGISApp1.Controls.OtherTool.LabelDefinedCsy.index = index;

            StackPanel s = (StackPanel)mainWindow.FindName("AddLabelTool");
            // 关闭其余工具条
            Canvas userControlsLayoutCsy = (Canvas)mainWindow.FindName("userControlsLayoutCsy");
            List<StackPanel> tools = ArcGISApp1.Utils.GetChildNode.GetChildObjects<StackPanel>(userControlsLayoutCsy);
            for (int i = 0; i < tools.Count(); i++)
            {
                tools[i].Visibility = Visibility.Collapsed;
            }
            s.Visibility = Visibility.Visible;
        }

        // 要素编辑
        private void FeatureEdit_Layers_CheckBox(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            ContextMenu cm = menuItem.Parent as ContextMenu;
            int index = int.Parse(cm.Name.Substring(16));

            // 获取字段
            //FeatureLayer tempLayer = (FeatureLayer)myMapView.Map.OperationalLayers[index];
            //Esri.ArcGISRuntime.Data.FeatureTable tempTable = tempLayer.FeatureTable;
            //List<string> fieldNames = new List<string>();
            //for (int i = 0; i < tempTable.Fields.Count; i++)
            //{
            //    fieldNames.Add(tempTable.Fields[i] + "");
            //}
            //ArcGISApp1.Controls.OtherTool.LabelDefinedCsy.fieldSource = fieldNames;
            ArcGISApp1.Controls.OtherTool.FeatureEditCsy.layerIndex = index;

            StackPanel s = (StackPanel)mainWindow.FindName("FeatureEdit");
            // 关闭其余工具条
            Canvas userControlsLayoutCsy = (Canvas)mainWindow.FindName("userControlsLayoutCsy");
            List<StackPanel> tools = ArcGISApp1.Utils.GetChildNode.GetChildObjects<StackPanel>(userControlsLayoutCsy);
            for (int i = 0; i < tools.Count(); i++)
            {
                tools[i].Visibility = Visibility.Collapsed;
            }
            s.Visibility = Visibility.Visible;
        }

        private void windowClosed_Point(object sender, EventArgs e)
        {
            Controls.TableOfContent.currentMyMapViewLayer.Renderer = SymbolFormCSY.PointCsy.ptSymbol;
        }

        private void windowClosed_Line(object sender, EventArgs e)
        {
            Controls.TableOfContent.currentMyMapViewLayer.Renderer = SymbolFormCSY.LineCsy.simpleRenderer;
        }

        private void windowClosed_Polygon(object sender, EventArgs e)
        {
            Controls.TableOfContent.currentMyMapViewLayer.Renderer = SymbolFormCSY.FillCsy.fillSymbol;
        }

        // 隐藏
        private void Hide_Layers_CheckBox(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            ContextMenu cm = menuItem.Parent as ContextMenu;
            int index = int.Parse(cm.Name.Substring(16));

            CheckBox cb = (CheckBox)myLayerList.Items[index];
            cb.IsChecked = false;
            myMapView.Map.OperationalLayers[index].Opacity = 0;
        }

        // checkbox的unchecked事件
        private void UnChecked_Layers_CheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            int index = int.Parse(cb.Name.Substring(16));
            cb.IsChecked = false;
            myMapView.Map.OperationalLayers[index].Opacity = 0;
        }

        // 缩放至
        private async void Extend_Layers_CheckBox(object sender,RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            ContextMenu cm = menuItem.Parent as ContextMenu;
            int index = int.Parse(cm.Name.Substring(16));
            await myMapView.SetViewpointGeometryAsync(MainWindow.myMapLayersList[index].FullExtent);
        }

        // checkbox的checked事件
        private void Checked_Layers_CheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            int index = int.Parse(cb.Name.Substring(16));
            cb.IsChecked = true;
            myMapView.Map.OperationalLayers[index].Opacity = 1;
            myMapView.Map.InitialViewpoint = new Viewpoint(MainWindow.myMapLayersList[index].FullExtent);
        }

        // 删除checkbox与图层
        private void Deleted_Layers_CheckBox(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            ContextMenu cm = menuItem.Parent as ContextMenu;
            int index = int.Parse(cm.Name.Substring(16));
            myLayerList.Items.Remove(check[index]);
            myMapView.Map.OperationalLayers[index].Opacity = 0;
        }

        // 上移checkbox与图层
        // 难点在于 check 与  myMapView.Map.OperationalLayers 的移动; 而 myLayersList 只要找到该 checkbox 对象后交换即可。应该是 myLayersList 是主导
        private void Up_Layers_CheckBox(object sender, RoutedEventArgs e)
        {
            if (myLayerList.Items.Count == 0)
                return;
            // 重新渲染 myLayersList——使用 SelectIndex
            int selectedIndex = myLayerList.SelectedIndex;
            int index1 = 0;
            int index2 = 0;
            if (selectedIndex == 0)
            {
                CheckBox tempCheckBox = (CheckBox)myLayerList.Items[0];
                index1 = int.Parse(tempCheckBox.Name.Substring(16));
                CheckBox tempCheckBox2 = (CheckBox)myLayerList.Items[myLayerList.Items.Count-1];
                index2 = int.Parse(tempCheckBox2.Name.Substring(16));
                myLayerList.Items.RemoveAt(0);
                // myLayerList.Items.Count 不需要减1， 因为上一句已经 remove 了一个元素， 总长度已经减去1了
                myLayerList.Items.Insert(myLayerList.Items.Count, tempCheckBox);
            }
            else
            {
                CheckBox tempCheckBox = (CheckBox)myLayerList.Items[selectedIndex - 1];
                index1 = int.Parse(tempCheckBox.Name.Substring(16));
                CheckBox tempCheckBox2 = (CheckBox)myLayerList.Items[selectedIndex];
                index2 = int.Parse(tempCheckBox2.Name.Substring(16));
                myLayerList.Items.RemoveAt(selectedIndex - 1);
                myLayerList.Items.Insert(selectedIndex, tempCheckBox);
            }
            exchangeCheckBoxAndOperationalLayer(index1, index2);
        }     

        // 下移checkbox与图层
        private void Down_Layers_CheckBox(object sender, RoutedEventArgs e)
        {
            int myLayersListLength = myLayerList.Items.Count;
            if (myLayersListLength == 0)
                return;
            // 重新渲染 myLayersList——使用 SelectIndex
            int selectedIndex = myLayerList.SelectedIndex;
            int index1 = 0;
            int index2 = 0;
            if (selectedIndex == myLayersListLength - 1)
            {
                CheckBox tempCheckBox = (CheckBox)myLayerList.Items[myLayersListLength - 1];
                index1 = int.Parse(tempCheckBox.Name.Substring(16));
                CheckBox tempCheckBox2 = (CheckBox)myLayerList.Items[0];
                index2 = int.Parse(tempCheckBox2.Name.Substring(16));
                myLayerList.Items.RemoveAt(myLayersListLength - 1);
                // myLayerList.Items.Count 不需要减1， 因为上一句已经 remove 了一个元素， 总长度已经减去1了
                myLayerList.Items.Insert(0, tempCheckBox);
            }
            else
            {
                CheckBox tempCheckBox = (CheckBox)myLayerList.Items[selectedIndex];
                index1 = int.Parse(tempCheckBox.Name.Substring(16));
                CheckBox tempCheckBox2 = (CheckBox)myLayerList.Items[selectedIndex + 1];
                index2 = int.Parse(tempCheckBox2.Name.Substring(16));
                myLayerList.Items.RemoveAt(selectedIndex);
                myLayerList.Items.Insert(selectedIndex + 1, tempCheckBox);
            }
            exchangeCheckBoxAndOperationalLayer(index1, index2);
        }

        // 交换checkbox以及对应的业务图层
        private void exchangeCheckBoxAndOperationalLayer(int index1, int index2)
        {
            // 交换业务图层
            myMapView.Map.OperationalLayers.Move(index1, index2);
            // 交换 checkbox
            CheckBox cb = new CheckBox();
            cb = check[index1];
            check[index1] = check[index2];
            check[index2] = cb;
            // 交换 checkbox 名字
            string tempName = "";
            tempName = check[index1].Name;
            check[index1].Name = check[index2].Name;
            check[index2].Name = tempName;
        }

        // 图层拖拽
        private void LBoxSort_OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(myLayerList);
                HitTestResult result = VisualTreeHelper.HitTest(myLayerList, pos);
                if (result == null)
                {
                    return;
                }
                var listBoxItem = Utils.FindVisualParent<ListBoxItem>(result.VisualHit);
                if (listBoxItem == null || listBoxItem.Content != myLayerList.SelectedItem)
                {
                    return;
                }
                DataObject dataObj = new DataObject(listBoxItem.Content as CheckBox);
                DragDrop.DoDragDrop(myLayerList, dataObj, DragDropEffects.Move);
            }
        }

        // 图层释放
        private void LBoxSort_OnDrop(object sender, DragEventArgs e)
        {
            var pos = e.GetPosition(myLayerList);
            var result = VisualTreeHelper.HitTest(myLayerList, pos);
            if (result == null)
            {
                return;
            }
            //查找元数据
            var sourcePerson = e.Data.GetData(typeof(CheckBox)) as CheckBox;
            if (sourcePerson == null)
            {
                return;
            }
            //查找目标数据
            var listBoxItem = Utils.FindVisualParent<ListBoxItem>(result.VisualHit);
            if (listBoxItem == null)
            {
                return;
            }
            var targetPerson = listBoxItem.Content as CheckBox;
            if (ReferenceEquals(targetPerson, sourcePerson))
            {
                return;
            }
            myLayerList.Items.Remove(sourcePerson);
            myLayerList.Items.Insert(myLayerList.Items.IndexOf(targetPerson), sourcePerson);

            int index1 = int.Parse(targetPerson.Name.Substring(16));
            int index2 = int.Parse(sourcePerson.Name.Substring(16));
            exchangeCheckBoxAndOperationalLayer(index1, index2);
            // 拖拽后目标checkbox的opacity复原
            targetPerson.Opacity = 1;
        }

        // 图层拖拽中悬浮
        CheckBox __targetPerson;
        private void LBoxSort_PreviewDragOver(object sender, DragEventArgs e)
        {
            var pos = e.GetPosition(myLayerList);
            var result = VisualTreeHelper.HitTest(myLayerList, pos);
            if (result == null)
            {
                return;
            }
            //查找目标数据
            var listBoxItem = Utils.FindVisualParent<ListBoxItem>(result.VisualHit);
            if (listBoxItem == null)
            {
                return;
            }
            __targetPerson = listBoxItem.Content as CheckBox;
            __targetPerson.Opacity = 0.4;
        }

        // 图层拖拽中悬浮后离开
        private void LBoxSort_PreviewDragLeave(object sender, DragEventArgs e)
        {
            __targetPerson.Opacity = 1;
        }

        private void test4_Click(object sender, RoutedEventArgs e)
        {
        }
    }


    internal static class Utils
    {
        //根据子元素查找父元素
        public static T FindVisualParent<T>(DependencyObject obj) where T : class
        {
            while (obj != null)
            {
                if (obj is T)
                    return obj as T;

                obj = VisualTreeHelper.GetParent(obj);
            }
            return null;
        }
    }
}

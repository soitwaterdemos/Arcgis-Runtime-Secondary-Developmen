using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;

namespace ArcGISApp1.Controls.OtherTool
{
    public enum OperateType
    {
        None,
        DrawPoint, DrawPolyline, DrawPolygon,
        EditVertex, Cal_Buffer,
        Cal_Clip, Cal_Cut, Cal_Union,
        Cal_Simplify, Cal_Jiaodian, Cal_Gene, Cal_Buff
    }

    /// <summary>
    /// GeometryCalcuCyk.xaml 的交互逻辑
    /// </summary>
    public partial class GeometryCalcuCyk : UserControl
    {
        Window w;
        MapView myMapView;
        TextBlock t1;

        public static OperateType operation = OperateType.None;          //操作类型 
        private SimpleMarkerSymbol pointSymbol; //点符号         
        private SimpleLineSymbol lineSymbol;    //线符号         
        private SimpleFillSymbol fillSymbol;    //填充符号         
        private SimpleMarkerSymbol vertexSymbol;//顶点的符号样式        
        private GraphicsOverlay graphicsLayer;  //图形层         
        private GraphicsOverlay selVertexLayer; //顶点图形层         
        private Esri.ArcGISRuntime.Geometry.PointCollection pointCollection; //绘图时的鼠标点集         
        private Graphic curSelGraphic;//当前选中图形对象         
        private MapPoint orgPoint;//鼠标移动时的位移计算原点         
        private int selGracphicIndex;//选中的图形的索引         
        private int selPointIndex;//选中的顶点的索引         
        private List<Graphic> listOfClipGraphics;   //剪切操作时选择的图形 

        // 图层
        FeatureLayer featureLayer;
        // 属性表
        Esri.ArcGISRuntime.Data.FeatureTable table;
        // 要素选择
        IdentifyLayerResult myShapeFileResult;

        public GeometryCalcuCyk()
        {
            InitializeComponent();
            init();
        }

        private void init()
        {
            operation = OperateType.None;
            pointSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, Color.FromArgb(0, 0, 0), 8.0);
            lineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, Color.Black, 2.0);
            fillSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, Color.FromArgb(125, 255, 0, 0), new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, Color.FromArgb(0, 0, 0), 2.0));
            vertexSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Square, Color.FromArgb(0, 0, 255), 8.0);
            curSelGraphic = null;
            orgPoint = null;
            selGracphicIndex = -1;
            selPointIndex = -1;
            listOfClipGraphics = new List<Graphic>();
        }

        private void DrawPointMenuItem_Click(object sender, RoutedEventArgs e)
        {
            operation = OperateType.DrawPoint;
        }

        private void DrawPolylineMenuItem_Click(object sender, RoutedEventArgs e)
        {
            operation = OperateType.DrawPolyline;
            if (pointCollection != null)
            {
                pointCollection.Clear();
            }
            else
            {
                pointCollection = new Esri.ArcGISRuntime.Geometry.PointCollection(myMapView.Map.SpatialReference);
            }      
        }

        private void DrawPolygonMenuItem_Click(object sender, RoutedEventArgs e)
        {
            operation = OperateType.DrawPolygon;
            if (pointCollection != null)
            {
                pointCollection.Clear();
            }
            else
            {
                pointCollection = new Esri.ArcGISRuntime.Geometry.PointCollection(myMapView.Map.SpatialReference);
            }        
        }

        private void ClearAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            graphicsLayer.Graphics.Clear();
            pointCollection.Clear();
        }

        // 编辑绘制图层
        private void EditVertexMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (curSelGraphic != null)//检查当前是否有选择图形 
            {
                operation = OperateType.EditVertex;
                if (curSelGraphic.Geometry.GeometryType == GeometryType.Point)//所选图形为点 
                {
                    selVertexLayer.Graphics.Clear();//清空顶点图层 
                    MapPoint pt = (MapPoint)curSelGraphic.Geometry;
                    Graphic pg = new Graphic(pt, vertexSymbol);//创建新的点图形 
                    selVertexLayer.Graphics.Add(pg);
                }
                else if (curSelGraphic.Geometry.GeometryType == GeometryType.Polyline)//所选图形为线 
                {
                    if (pointCollection != null)
                    {
                        pointCollection.Clear();//清空点集 
                    }
                    else
                    {
                        pointCollection = new Esri.ArcGISRuntime.Geometry.PointCollection(myMapView.Map.SpatialReference);
                    }
                    Esri.ArcGISRuntime.Geometry.Polyline ln = (Esri.ArcGISRuntime.Geometry.Polyline)curSelGraphic.Geometry;
                    pointCollection.AddPoints(ln.Parts[0].Points);//将线的所有顶点加入点集 
                    selVertexLayer.Graphics.Clear();
                    for (int i = 0; i < pointCollection.Count; i++)//将所有点以顶点图形样式显示 
                    {
                        MapPoint pt = pointCollection[i];
                        Graphic pg = new Graphic(pt, vertexSymbol);
                        selVertexLayer.Graphics.Add(pg);
                    }
                }
                else if (curSelGraphic.Geometry.GeometryType == GeometryType.Polygon)//所选图形为多边形 
                {
                    if (pointCollection != null)
                    {
                        pointCollection.Clear();
                    }
                    else
                    {
                        pointCollection = new Esri.ArcGISRuntime.Geometry.PointCollection(myMapView.Map.SpatialReference);
                    }
                    Esri.ArcGISRuntime.Geometry.Polygon pg = (Esri.ArcGISRuntime.Geometry.Polygon)curSelGraphic.Geometry;
                    pointCollection.AddPoints(pg.Parts[0].Points);
                    selVertexLayer.Graphics.Clear();
                    for (int i = 0; i < pointCollection.Count; i++)
                    {
                        MapPoint pt = pointCollection[i];
                        Graphic gg = new Graphic(pt, vertexSymbol);
                        selVertexLayer.Graphics.Add(gg);
                    }
                }
                EditVertexMenuItem.IsEnabled = false;
                UneditVertexMenuItem.IsEnabled = true;
            }
        }

        private void UneditVertexMenuItem_Click(object sender, RoutedEventArgs e)
        {
            operation = OperateType.None;
            selVertexLayer.Graphics.Clear();//清空顶点图层 
            UneditVertexMenuItem.IsEnabled = false; 
            if (curSelGraphic != null)
                EditVertexMenuItem.IsEnabled = true; 
        }

        // 裁剪
        private void ClipMenuItem_Click(object sender, RoutedEventArgs e)
        {
            operation = OperateType.Cal_Clip;
            graphicsLayer.ClearSelection();
            listOfClipGraphics.Clear();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            w = Window.GetWindow(this);
            myMapView = (MapView)w.FindName("myMapView");
            t1 = (TextBlock)w.FindName("myTest");

            // 几何操作工具需要的
            graphicsLayer = new GraphicsOverlay();
            myMapView.GraphicsOverlays.Add(graphicsLayer);
            graphicsLayer.SelectionColor = Color.FromArgb(255, 255, 0);

            selVertexLayer = new GraphicsOverlay();
            myMapView.GraphicsOverlays.Add(selVertexLayer);

            EditVertexMenuItem.IsEnabled = false;
            UneditVertexMenuItem.IsEnabled = false;

            // myMapView的鼠标事件
            myMapView.MouseLeftButtonDown -= MyMapView_MouseLeftButtonDown;
            myMapView.MouseDoubleClick -= MyMapView_MouseDoubleClick;
            myMapView.MouseRightButtonDown -= MyMapView_MouseRightButtonDown;
            myMapView.MouseRightButtonUp -= MyMapView_MouseRightButtonUp;

            myMapView.MouseLeftButtonDown += MyMapView_MouseLeftButtonDown;
            myMapView.MouseDoubleClick += MyMapView_MouseDoubleClick;
            myMapView.MouseRightButtonDown += MyMapView_MouseRightButtonDown;
            myMapView.MouseRightButtonUp += MyMapView_MouseRightButtonUp;

            edit2SHP.ToolTip = "1.先选择需要编辑的要素;\n2.再选择所绘制的Grapgic;\n3. 最后点击\"编辑\"按钮, 用Grapgic替换所选要素;";
        }

        //计算点之前的距离
        public double GetDistanceBetweenPoints(MapPoint p1, MapPoint p2)
        {
            double dis = 0.0; dis = Math.Sqrt((p1.X - p2.X) * (p1.Y - p2.Y));
            return dis;
        }         
        //判断包含关系 
        public bool IsEnvelopeContains(Envelope env,MapPoint pt)
        {
            if (pt.X < env.XMax && pt.X > env.XMin && pt.Y < env.YMax && pt.Y > env.YMin)
                return true;
            else
                return false;
        }
        
        // myMapView 事件
        private async void MyMapView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IInputElement ie = (IInputElement)(sender);
            MapPoint loc = myMapView.ScreenToLocation(e.GetPosition(ie));
            switch (operation)
            {
                case OperateType.DrawPoint: //画点 
                    Graphic pt = new Graphic(loc, pointSymbol);
                    graphicsLayer.Graphics.Add(pt);
                    break;
                case OperateType.DrawPolyline://画线 
                    pointCollection.Add(loc);
                    if (pointCollection.Count >= 2)
                    {
                        if (pointCollection.Count > 2)
                        {
                            Graphic g = graphicsLayer.Graphics[graphicsLayer.Graphics.Count - 1];
                            PolylineBuilder lb = new PolylineBuilder(pointCollection);
                            g.Geometry = lb.ToGeometry();
                        }
                        else
                        {
                            Esri.ArcGISRuntime.Geometry.Polyline l = new Esri.ArcGISRuntime.Geometry.Polyline(pointCollection);
                            Graphic lg = new Graphic(l, lineSymbol);
                            graphicsLayer.Graphics.Add(lg);
                        }
                    }
                    break;
                case OperateType.DrawPolygon://画多边形 
                    pointCollection.Add(loc);
                    if (pointCollection.Count >= 3)
                    {
                        if (pointCollection.Count > 3)
                        {
                            Graphic g = graphicsLayer.Graphics[graphicsLayer.Graphics.Count - 1];
                            PolygonBuilder pb = new PolygonBuilder(pointCollection);
                            g.Geometry = pb.ToGeometry();
                        }
                        else
                        {
                            Esri.ArcGISRuntime.Geometry.Polygon p = new Esri.ArcGISRuntime.Geometry.Polygon(pointCollection);
                            Graphic pg = new Graphic(p, fillSymbol);
                            graphicsLayer.Graphics.Add(pg);
                        }
                    }
                    break;
                case OperateType.None://缺省状态                   
                    graphicsLayer.ClearSelection();
                    IdentifyGraphicsOverlayResult result = await myMapView.IdentifyGraphicsOverlayAsync(graphicsLayer, e.GetPosition(ie), 5, false);
                    //选择图形元素 
                    if (result.Graphics.Count < 1)
                    {
                        curSelGraphic = null;
                        EditVertexMenuItem.IsEnabled = false;
                        UneditVertexMenuItem.IsEnabled = false;
                        return;
                    }
                    curSelGraphic = result.Graphics.First();
                    curSelGraphic.IsSelected = true;
                    EditVertexMenuItem.IsEnabled = true;
                    break;
                case OperateType.Cal_Clip: //选择图形 
                    IdentifyGraphicsOverlayResult gResult = await myMapView.IdentifyGraphicsOverlayAsync(graphicsLayer, e.GetPosition(ie), 5, false);
                    if (gResult.Graphics.Count < 1)
                    {
                        return;
                    }
                    Graphic selGraphic = gResult.Graphics.First();
                    selGraphic.IsSelected = true; listOfClipGraphics.Add(selGraphic); //记录所选图形                    
                    if (listOfClipGraphics.Count == 2) //图形数目为2时，进行剪切计算 
                    {
                        Graphic g1 = listOfClipGraphics[0];
                        Graphic g2 = listOfClipGraphics[1];
                        if (g1.Geometry.GeometryType != GeometryType.Polygon || g2.Geometry.GeometryType != GeometryType.Polygon) //如果所选图形不是多边形，则退出 
                        {
                            MessageBox.Show("请选择两个多边形图形！");
                            listOfClipGraphics.Clear();
                            graphicsLayer.ClearSelection();
                            return;
                        }
                        Esri.ArcGISRuntime.Geometry.Geometry resultGeometry = GeometryEngine.Clip(g1.Geometry, g2.Geometry.Extent); //执行剪切操作 
                        if (resultGeometry != null) //处理结果 
                        {
                            graphicsLayer.Graphics.Remove(g1); //从图形层中移除原图形 
                            graphicsLayer.Graphics.Remove(g2);
                            Graphic clipedGraphic = new Graphic(resultGeometry, fillSymbol); //利 用剪切结果构建新的图形 
                            graphicsLayer.Graphics.Add(clipedGraphic); operation = OperateType.None;
                        }
                        listOfClipGraphics.Clear();//清空图形选择集合 
                        graphicsLayer.ClearSelection();//清空图形层所选                       
                    }
                    break;
                case OperateType.Cal_Union: // 联合
                    IdentifyGraphicsOverlayResult gResultUnion = await myMapView.IdentifyGraphicsOverlayAsync(graphicsLayer, e.GetPosition(ie), 5, false);
                    if (gResultUnion.Graphics.Count < 1)
                    {
                        return;
                    }
                    Graphic selGraphicUnion = gResultUnion.Graphics.First();
                    selGraphicUnion.IsSelected = true;
                    listOfClipGraphics.Add(selGraphicUnion); //记录所选图形                    
                    if (listOfClipGraphics.Count == 2) //图形数目为2时，进行剪切计算 
                    {
                        Graphic g1 = listOfClipGraphics[0];
                        Graphic g2 = listOfClipGraphics[1];
                        if (g1.Geometry.GeometryType != GeometryType.Polygon || g2.Geometry.GeometryType != GeometryType.Polygon) //如果所选图形不是多边形，则退出 
                        {
                            MessageBox.Show("请选择两个多边形图形！");
                            listOfClipGraphics.Clear();
                            graphicsLayer.ClearSelection();
                            return;
                        }
                        Esri.ArcGISRuntime.Geometry.Geometry resultGeometry = GeometryEngine.Union(g1.Geometry, g2.Geometry.Extent); //执行剪切操作 
                        if (resultGeometry != null) //处理结果 
                        {
                            graphicsLayer.Graphics.Remove(g1); //从图形层中移除原图形 
                            graphicsLayer.Graphics.Remove(g2);
                            Graphic clipedGraphic = new Graphic(resultGeometry, fillSymbol); //利 用剪切结果构建新的图形 
                            graphicsLayer.Graphics.Add(clipedGraphic);
                            operation = OperateType.None;
                        }
                        listOfClipGraphics.Clear();//清空图形选择集合 
                        graphicsLayer.ClearSelection();//清空图形层所选                    
                    }
                    break;
                case OperateType.Cal_Cut: // 剪切
                    IdentifyGraphicsOverlayResult gResult_Cut = await myMapView.IdentifyGraphicsOverlayAsync(graphicsLayer, e.GetPosition(ie), 5, false);
                    if (gResult_Cut.Graphics.Count < 1)
                    {
                        return;
                    }
                    Graphic selGraphic_Cut = gResult_Cut.Graphics.First();
                    selGraphic_Cut.IsSelected = true;
                    listOfClipGraphics.Add(selGraphic_Cut); //记录所选图形                    
                    if (listOfClipGraphics.Count == 2) //图形数目为1时，进行剪切计算 
                    {
                        Graphic g1 = listOfClipGraphics[0];
                        Graphic g2 = listOfClipGraphics[1];
                        if (g1.Geometry.GeometryType != GeometryType.Polygon || g2.Geometry.GeometryType != GeometryType.Polyline) //如果所选图形不是多边形，则退出 
                        {
                            MessageBox.Show("请先选择一个面要素后再选择一个线要素.");
                            listOfClipGraphics.Clear();
                            graphicsLayer.ClearSelection();
                            return;
                        }
                        Esri.ArcGISRuntime.Geometry.Polyline polyLine = (Esri.ArcGISRuntime.Geometry.Polyline)g2.Geometry;
                        Esri.ArcGISRuntime.Geometry.Geometry[] resultGeometry = GeometryEngine.Cut(g1.Geometry, polyLine); //执行剪切操作 
                        if (resultGeometry != null) //处理结果 
                        {
                            graphicsLayer.Graphics.Remove(g1);
                            for (int z = 0; z < resultGeometry.Length;z++)
                            {
                                Graphic clipedGraphic = new Graphic(resultGeometry[z], fillSymbol); //利 用剪切结果构建新的图形 
                                graphicsLayer.Graphics.Add(clipedGraphic);
                            }
                            operation = OperateType.None;
                        }
                        listOfClipGraphics.Clear();//清空图形选择集合 
                        graphicsLayer.ClearSelection();//清空图形层所选                    
                    }
                    break;
                case OperateType.Cal_Simplify: // 拓扑纠正
                    IdentifyGraphicsOverlayResult gResult_Simplify = await myMapView.IdentifyGraphicsOverlayAsync(graphicsLayer, e.GetPosition(ie), 5, false);
                    if (gResult_Simplify.Graphics.Count < 1)
                    {
                        return;
                    }
                    Graphic selGraphic_Simplify = gResult_Simplify.Graphics.First();
                    selGraphic_Simplify.IsSelected = true;
                    listOfClipGraphics.Add(selGraphic_Simplify); //记录所选图形                    
                    if (listOfClipGraphics.Count == 1) //图形数目为1时，进行剪切计算 
                    {
                        Graphic g1 = listOfClipGraphics[0];
                        if (g1.Geometry.GeometryType == GeometryType.Point) //如果所选图形不是多边形，则退出 
                        {
                            MessageBox.Show("请先选择一个面要素或线要素.");
                            listOfClipGraphics.Clear();
                            graphicsLayer.ClearSelection();
                            return;
                        }
                        Esri.ArcGISRuntime.Geometry.Geometry resultGeometry = GeometryEngine.Simplify(g1.Geometry); //执行剪切操作 
                        if (resultGeometry != null) //处理结果 
                        {
                            graphicsLayer.Graphics.Remove(g1); //从图形层中移除原图形 
                            Graphic clipedGraphic = new Graphic(resultGeometry, fillSymbol); //利 用剪切结果构建新的图形 
                            graphicsLayer.Graphics.Add(clipedGraphic);
                            operation = OperateType.None;
                        }
                        listOfClipGraphics.Clear();//清空图形选择集合 
                        graphicsLayer.ClearSelection();//清空图形层所选                    
                    }
                    break;
                case OperateType.Cal_Gene: // 简化
                    IdentifyGraphicsOverlayResult gResult_Gene = await myMapView.IdentifyGraphicsOverlayAsync(graphicsLayer, e.GetPosition(ie), 5, false);
                    if (gResult_Gene.Graphics.Count < 1)
                    {
                        return;
                    }
                    Graphic selGraphic_Gene = gResult_Gene.Graphics.First();
                    selGraphic_Gene.IsSelected = true;
                    listOfClipGraphics.Add(selGraphic_Gene); //记录所选图形                    
                    if (listOfClipGraphics.Count == 1) //图形数目为1时
                    {
                        Graphic g1 = listOfClipGraphics[0];
                        if (g1.Geometry.GeometryType == GeometryType.Point) //如果所选图形是点，则退出 
                        {
                            MessageBox.Show("请先选择一个面要素或线要素.");
                            listOfClipGraphics.Clear();
                            graphicsLayer.ClearSelection();
                            return;
                        }
                        Esri.ArcGISRuntime.Geometry.Geometry resultGeometry = GeometryEngine.Generalize(g1.Geometry,1000000.0,true); //执行剪切操作 
                        if (resultGeometry != null) //处理结果 
                        {
                            MessageBox.Show(resultGeometry.ToJson() + "\n" + resultGeometry.GeometryType);
                            graphicsLayer.Graphics.Remove(g1); //从图形层中移除原图形 
                            Graphic clipedGraphic = new Graphic(resultGeometry, fillSymbol); //利 用剪切结果构建新的图形 
                            graphicsLayer.Graphics.Add(clipedGraphic);
                            operation = OperateType.None;
                        }
                        listOfClipGraphics.Clear();//清空图形选择集合 
                        graphicsLayer.ClearSelection();//清空图形层所选                    
                    }
                    break;
                case OperateType.Cal_Buff: // 缓冲
                    IdentifyGraphicsOverlayResult gResult_Buff = await myMapView.IdentifyGraphicsOverlayAsync(graphicsLayer, e.GetPosition(ie), 5, false);
                    if (gResult_Buff.Graphics.Count < 1)
                    {
                        return;
                    }
                    Graphic selGraphic_Buff = gResult_Buff.Graphics.First();
                    selGraphic_Buff.IsSelected = true;
                    listOfClipGraphics.Add(selGraphic_Buff); //记录所选图形                    
                    if (listOfClipGraphics.Count == 1) //图形数目为1时，进行剪切计算 
                    {
                        Graphic g1 = listOfClipGraphics[0];
                        Esri.ArcGISRuntime.Geometry.Geometry resultGeometry = GeometryEngine.Buffer(g1.Geometry, 1000000.0); //执行剪切操作 
                        if (resultGeometry != null) //处理结果 
                        {
                            graphicsLayer.Graphics.Remove(g1); //从图形层中移除原图形 
                            Graphic clipedGraphic = new Graphic(resultGeometry, new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, Color.FromArgb(125, 255, 250, 0), new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, Color.FromArgb(0, 0, 0), 4.0))); //利 用剪切结果构建新的图形 
                            graphicsLayer.Graphics.Add(clipedGraphic);
                            operation = OperateType.None;
                        }
                        listOfClipGraphics.Clear();//清空图形选择集合 
                        graphicsLayer.ClearSelection();//清空图形层所选                    
                    }
                    break;
                case OperateType.Cal_Jiaodian: // 交点
                    IdentifyGraphicsOverlayResult gResult_Jiaodian = await myMapView.IdentifyGraphicsOverlayAsync(graphicsLayer, e.GetPosition(ie), 5, false);
                    if (gResult_Jiaodian.Graphics.Count < 1)
                    {
                        return;
                    }
                    Graphic selGraphic_Jiaodian = gResult_Jiaodian.Graphics.First();
                    selGraphic_Jiaodian.IsSelected = true;
                    listOfClipGraphics.Add(selGraphic_Jiaodian); //记录所选图形                    
                    if (listOfClipGraphics.Count == 2) //图形数目为1时，进行剪切计算 
                    {
                        Graphic g1 = listOfClipGraphics[0];
                        Graphic g2 = listOfClipGraphics[1];
                        IReadOnlyList<Geometry> resultGeometry = GeometryEngine.Intersections(g1.Geometry, g2.Geometry); //执行剪切操作 
                        if (resultGeometry != null) //处理结果 
                        {
                            Graphic clipedGraphic = new Graphic(resultGeometry[0], pointSymbol); //利 用剪切结果构建新的图形 
                            graphicsLayer.Graphics.Add(clipedGraphic);
                            operation = OperateType.None;
                        }
                        listOfClipGraphics.Clear();//清空图形选择集合 
                        graphicsLayer.ClearSelection();//清空图形层所选                    
                    }
                    break;
            }
        }
        private void MyMapView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (operation != OperateType.EditVertex) //当非处于顶点编辑状态时，鼠标双击表示结束 图形绘制
            {
                operation = OperateType.None;
                if (pointCollection != null)
                {
                    pointCollection.Clear();
                }
            }
        }
        private async void MyMapView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            IInputElement ie = (IInputElement)(sender); MapPoint loc = myMapView.ScreenToLocation(e.GetPosition(ie));
            bool isCaptured = false; if (operation == OperateType.None)//非绘图状态下，右键按下捕捉图形元素 
            {
                if (e.RightButton == MouseButtonState.Pressed && curSelGraphic != null)
                {
                    if (curSelGraphic.Geometry.GeometryType == GeometryType.Point)
                    {
                        MapPoint p = (MapPoint)curSelGraphic.Geometry;
                        if (GetDistanceBetweenPoints(p, loc) < 10)
                            isCaptured = true;
                    }
                    else
                    {
                        Envelope env = curSelGraphic.Geometry.Extent; if (IsEnvelopeContains(env, loc))
                            isCaptured = true;
                    }
                }
            }
            else if (operation == OperateType.EditVertex)//顶点编辑状态下，捕捉图形及顶点 
            {
                IReadOnlyList<IdentifyGraphicsOverlayResult> gResults = await myMapView.IdentifyGraphicsOverlaysAsync(e.GetPosition(ie), 5, false, 1);
                if (gResults.Count < 2) return; Graphic objGraphic1 = gResults[0].Graphics.First();
                Graphic objGraphic2 = gResults[1].Graphics.First();
                Graphic idGraphic = null;
                if (objGraphic1.GraphicsOverlay == graphicsLayer)//图形1位于图形层         
                {
                    selGracphicIndex = graphicsLayer.Graphics.IndexOf(objGraphic1);//获取其索引 位置            
                    idGraphic = objGraphic1;
                    selPointIndex = selVertexLayer.Graphics.IndexOf(objGraphic2);//获取相应顶点 所在顶点图层的索引 


                }
                else if (objGraphic2.GraphicsOverlay == graphicsLayer)//图形2位于图形层 
                {
                    selGracphicIndex = graphicsLayer.Graphics.IndexOf(objGraphic2); idGraphic = objGraphic2;
                    selPointIndex = selVertexLayer.Graphics.IndexOf(objGraphic1);
                }
                if (selGracphicIndex < 0 || selPointIndex < 0) return;
                isCaptured = true;
            }
            if (isCaptured)//如果捕捉到元素,设置光标样式
            {
                myMapView.Cursor = Cursors.SizeAll; orgPoint = loc; //标记移动图形时的起点 
            }
            else { myMapView.Cursor = Cursors.Arrow; orgPoint = null; }
        }
        private void MyMapView_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            myMapView.Cursor = Cursors.Arrow;//恢复光标样式 
            if (curSelGraphic == null || orgPoint == null)
                return;             //计算位移 
            IInputElement ie = (IInputElement)(sender);
            MapPoint loc = myMapView.ScreenToLocation(e.GetPosition(ie));
            double deltaX = loc.X - orgPoint.X;
            double deltaY = loc.Y - orgPoint.Y;
            if (operation == OperateType.None)//非绘制状态或顶点编辑状态    
            {
                if (curSelGraphic.Geometry.GeometryType == GeometryType.Point)//当前选择的图形 为点，重新构造点         
                {
                    MapPointBuilder pb = new MapPointBuilder(loc);
                    curSelGraphic.Geometry = pb.ToGeometry();
                }
                else if (curSelGraphic.Geometry.GeometryType == GeometryType.Polyline)//当前选 择的图形为线，重新计算所有点 


                {
                    Esri.ArcGISRuntime.Geometry.Polyline ln = (Esri.ArcGISRuntime.Geometry.Polyline)curSelGraphic.Geometry; pointCollection.Clear();
                    for (int i = 0; i < ln.Parts[0].Points.Count; i++)
                    {
                        double X = ln.Parts[0].Points[i].X + deltaX; double Y = ln.Parts[0].Points[i].Y + deltaY;
                        MapPoint Pt = new MapPoint(X, Y); pointCollection.Add(Pt);
                    }
                    PolylineBuilder lb = new PolylineBuilder(pointCollection);
                    curSelGraphic.Geometry = lb.ToGeometry();
                }
                else if (curSelGraphic.Geometry.GeometryType == GeometryType.Polygon)//当前选 择图形为多边形，重新计算所有点         
                {
                    Esri.ArcGISRuntime.Geometry.Polygon poly = (Esri.ArcGISRuntime.Geometry.Polygon)curSelGraphic.Geometry;
                    pointCollection.Clear();
                    for (int i = 0; i < poly.Parts[0].Points.Count; i++)
                    {
                        double X = poly.Parts[0].Points[i].X + deltaX; double Y = poly.Parts[0].Points[i].Y + deltaY;
                        MapPoint Pt = new MapPoint(X, Y); pointCollection.Add(Pt);
                    }
                    PolygonBuilder pb = new PolygonBuilder(pointCollection);
                    curSelGraphic.Geometry = pb.ToGeometry();
                }
                }
                else if (operation == OperateType.EditVertex)//处于顶点编辑状态 
                {
                    if (selGracphicIndex >= 0)
                    {
                        Graphic g = graphicsLayer.Graphics.ElementAt(selGracphicIndex);//找到所选 图形 
                        if (g.Geometry.GeometryType == GeometryType.Point)//图形为点 
                        {
                            MapPointBuilder mpb = new MapPointBuilder(loc);
                            g.Geometry = mpb.ToGeometry();
                        }
                        else if (g.Geometry.GeometryType == GeometryType.Polyline && selPointIndex >= 0)//图形为线，顶点已捕捉 


                        {
                            Esri.ArcGISRuntime.Geometry.Polyline pln = (Esri.ArcGISRuntime.Geometry.Polyline)g.Geometry;
                            pointCollection.Clear(); pointCollection.AddPoints(pln.Parts[0].Points); pointCollection.SetPoint(selPointIndex, loc.X, loc.Y);
                            PolylineBuilder plb = new PolylineBuilder(pointCollection); g.Geometry = plb.ToGeometry();
                        }
                        else if (g.Geometry.GeometryType == GeometryType.Polygon && selPointIndex >= 0)//图形为多边形，顶点已捕捉 
                        {
                            Esri.ArcGISRuntime.Geometry.Polygon plg = (Esri.ArcGISRuntime.Geometry.Polygon)g.Geometry; pointCollection.Clear();
                            pointCollection.AddPoints(plg.Parts[0].Points); pointCollection.SetPoint(selPointIndex, loc.X, loc.Y);
                            PolygonBuilder pgb = new PolygonBuilder(pointCollection); g.Geometry = pgb.ToGeometry();
                        }
                        if (selPointIndex >= 0)
                        {//移动相应的顶点到当前位置 
                            Graphic vtGraphic = selVertexLayer.Graphics.ElementAt(selPointIndex);
                            MapPointBuilder tgPoint = new MapPointBuilder(loc); vtGraphic.Geometry = tgPoint.ToGeometry();
                        }
                    }
                }
            }

        #region 几何操作的调用
        // 联合
        private void UnionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            operation = OperateType.Cal_Union;
            graphicsLayer.ClearSelection();
            listOfClipGraphics.Clear();
        }

        private void exitTool_Click(object sender, RoutedEventArgs e)
        {
            myMapView.MouseLeftButtonDown -= MyMapView_MouseLeftButtonDown;
            myMapView.MouseDoubleClick -= MyMapView_MouseDoubleClick;
            myMapView.MouseRightButtonDown -= MyMapView_MouseRightButtonDown;
            myMapView.MouseRightButtonUp -= MyMapView_MouseRightButtonUp;

            StackPanel stackPanel = (StackPanel)w.FindName("cykGeometryCalcuEdit");
            stackPanel.Visibility = Visibility.Collapsed;
        }

        // 剪切
        private void CutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            operation = OperateType.Cal_Cut;
            graphicsLayer.ClearSelection();
            listOfClipGraphics.Clear();
        }

        // 拓扑纠正
        private void SimplifyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            operation = OperateType.Cal_Simplify;
            graphicsLayer.ClearSelection();
            listOfClipGraphics.Clear();
        }

        // 交点
        private void IntersectionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            operation = OperateType.Cal_Jiaodian;
            graphicsLayer.ClearSelection();
            listOfClipGraphics.Clear();
        }

        // 简化
        private void GeneralizeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            operation = OperateType.Cal_Gene;
            graphicsLayer.ClearSelection();
            listOfClipGraphics.Clear();
        }

        // 缓冲区
        private void BufferMenuItem_Click(object sender, RoutedEventArgs e)
        {
            operation = OperateType.Cal_Buff;
            graphicsLayer.ClearSelection();
            listOfClipGraphics.Clear();
        }

        #endregion

        // 点击 第二个Menu 
        // 将绘制的要素添加到shp文件中
        private void add2SHPStart_Click(object sender, RoutedEventArgs e)
        {
            layersComboBox.Items.Clear();

            TableOfContent t = (TableOfContent)w.FindName("myTableOfContent");
            int tempInt = 0;
            String tempString = "";
            string fileType = "";

            for (int i = 0; i < /*ArcGISApp1.Controls.TableOfContent*/t.myLayerList.Items.Count; i++)
            {
                CheckBox cb = (CheckBox)t.myLayerList.Items[i];

                tempInt = int.Parse(cb.Name.Substring(16));
                tempString = (cb.Content + "");
                fileType = tempString.Substring(tempString.Length - 3);

                if (cb.IsChecked == true && (fileType == "shp" || fileType == "SHP"))
                {
                    intList.Add(int.Parse(cb.Name.Substring(16)));
                    layersComboBox.Items.Add(cb.Content);
                }
            }

            layersComboBox.IsEnabled = true;
        }

        public List<int> intList = new List<int>();
        
        // 添加要素
        private async void add2SHP_Click(object sender, RoutedEventArgs e)
        {
            if (table.CanAdd() && layersComboBox.Items.Count >= 1 && curSelGraphic != null && layersComboBox.SelectedIndex >= 0)
            {
                // 图层
                featureLayer = (FeatureLayer)(myMapView.Map.OperationalLayers[layersComboBox.SelectedIndex]);
                // 属性表
                table = featureLayer.FeatureTable;

                QueryParameters query = new QueryParameters();
                query.WhereClause = string.Format("upper(FID) = \"0\"");
                FeatureQueryResult queryResult = await table.QueryFeaturesAsync(query);
                IEnumerator<Feature> resultFeatures = queryResult.GetEnumerator();
                List<Feature> features = new List<Feature>();
                while (resultFeatures.MoveNext())
                {
                    features.Add(resultFeatures.Current);
                }

                Feature tempGeoElement = features[0];
               
                Feature addFeature = table.CreateFeature(features[0].Attributes, curSelGraphic.Geometry);
                await table.AddFeatureAsync(addFeature);
                t1.Text = "要素保存成功!";
            }
        }

        // 删除要素
        private async void delete2SHP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Feature tempGeoElement = (Feature)myShapeFileResult.GeoElements[0];
                await table.DeleteFeatureAsync(tempGeoElement);
            }
            catch
            {

            }
        }

        // 编辑要素
        private async void edit2SHP_Click(object sender, RoutedEventArgs e)
        {
            if (table.CanEditGeometry())
            {
                try
                {
                    Feature changedFeature = (Feature)myShapeFileResult.GeoElements[0];
                    if (curSelGraphic != null)
                    {
                        Feature newFeature = table.CreateFeature(changedFeature.Attributes, curSelGraphic.Geometry);
                        changedFeature.Geometry = curSelGraphic.Geometry;
                        await table.UpdateFeatureAsync(changedFeature);
                        
                    }

                }
                catch (Exception e2)
                {
                    MessageBox.Show(e2.Message);
                }
            }
        }

        private void choose2SHP_Click(object sender, RoutedEventArgs e)
        {
            add2SHP.IsEnabled = true;
            delete2SHP.IsEnabled = true;
            edit2SHP.IsEnabled = true;

            myMapView.GeoViewTapped -= myMapViewGetAttributeValue;
            myMapView.GeoViewTapped += myMapViewGetAttributeValue;
        }

        // 点选
        private async void myMapViewGetAttributeValue(object sender, Esri.ArcGISRuntime.UI.Controls.GeoViewInputEventArgs e)
        {
            StackPanel stackPanel = (StackPanel)w.FindName("cykGeometryCalcuEdit");
            if (layersComboBox.SelectedIndex >= 0 && stackPanel.Visibility == Visibility.Visible)
            {
                myShapeFileResult = await myMapView.IdentifyLayerAsync(featureLayer, e.Position, 15, false);

                //if(myShapeFileResult != null)  MessageBox.Show("" + 12 + " " + myShapeFileResult);

                // 要素选择高亮
                try
                {
                    string numberFID = ((Feature)(myShapeFileResult.GeoElements[0])).GetAttributeValue("FID") + "";
                    QueryParameters queryParams = new QueryParameters
                    {
                        //WhereClause = "upper(FID) = " + numberFID
                        Geometry = e.Location
                    };
                    FeatureQueryResult queryResult = await featureLayer.SelectFeaturesAsync(queryParams, Esri.ArcGISRuntime.Mapping.SelectionMode.New);

                    IEnumerator<Feature> resultFeatures = queryResult.GetEnumerator();
                    List<Feature> features = new List<Feature>();
                    while (resultFeatures.MoveNext())
                    {
                        features.Add(resultFeatures.Current);
                    }

                    // 每一次"选择"都需要点击一次"选择"按钮
                    myMapView.GeoViewTapped -= myMapViewGetAttributeValue;
                    //t1.Text = numberFID + " ID;" + features.Count + "个;  " + e.Location.X + ", " + e.Location.Y + "  //  投影" + myMapView.Map.SpatialReference + "," + myMapView.UnitsPerPixel;
                }
                catch (Exception ex)
                {

                }               
                //t1.Text += featureLayer.Description + table.DisplayName + "...";
            }
        }

        private void layersComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            featureLayer = (FeatureLayer)(myMapView.Map.OperationalLayers[layersComboBox.SelectedIndex]);
            // 属性表
            table = featureLayer.FeatureTable;

            if (layersComboBox.SelectedIndex >= 0)
            {               
                choose2SHP.IsEnabled = true;
            }
        }
    }
}

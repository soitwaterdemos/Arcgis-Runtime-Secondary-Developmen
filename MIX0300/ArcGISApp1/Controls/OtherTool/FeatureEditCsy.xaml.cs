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
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;

namespace ArcGISApp1.Controls.OtherTool
{
    
    /// <summary>
    /// FeatureEditCsy.xaml 的交互逻辑
    /// </summary>
    public partial class FeatureEditCsy : UserControl
    {
        Window w;
        MapView myMapView;
        public static int layerIndex = 0;

        public OperateType operation = ArcGISApp1.Controls.OtherTool.GeometryCalcuCyk.operation;          //操作类型 
        private SimpleMarkerSymbol pointSymbol; //点符号         
        private SimpleLineSymbol lineSymbol;    //线符号         
        private SimpleFillSymbol fillSymbol;    //填充符号         
        private SimpleMarkerSymbol vertexSymbol;//顶点的符号样式        
        private GraphicsOverlay graphicsLayer;  //图形层         
        private GraphicsOverlay selVertexLayer; //顶点图形层         
        private Esri.ArcGISRuntime.Geometry.PointCollection pointCollection; //绘图时的 鼠标点集         
        private Graphic curSelGraphic;//当前选中图形对象         
        private MapPoint orgPoint;//鼠标移动时的位移计算原点         
        private int selGracphicIndex;//选中的图形的索引         
        private int selPointIndex;//选中的顶点的索引         
        private List<Graphic> listOfClipGraphics;   //剪切操作时选择的图形 

        public FeatureEditCsy()
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

            // 画点
            drawPoint.Checked += (sender, e) =>
            {
                operation = OperateType.DrawPoint;
            };

            // 画线
            drawLine.Checked += (sender, e) =>
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
            };

            // 画面
            drawPoly.Checked += (sender, e) =>
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
            };
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            w = Window.GetWindow(this);
            myMapView = (MapView)w.FindName("myMapView");

            // 几何操作工具需要的
            graphicsLayer = new GraphicsOverlay();
            myMapView.GraphicsOverlays.Add(graphicsLayer);
            graphicsLayer.SelectionColor = Color.FromArgb(255, 255, 0);

            selVertexLayer = new GraphicsOverlay();
            myMapView.GraphicsOverlays.Add(selVertexLayer);

            // myMapView的鼠标事件
            myMapView.MouseLeftButtonDown -= MyMapView_MouseLeftButtonDown;
            myMapView.MouseDoubleClick -= MyMapView_MouseDoubleClick;
            myMapView.MouseRightButtonDown -= MyMapView_MouseRightButtonDown;
            myMapView.MouseRightButtonUp -= MyMapView_MouseRightButtonUp;

            myMapView.MouseLeftButtonDown += MyMapView_MouseLeftButtonDown;
            myMapView.MouseDoubleClick += MyMapView_MouseDoubleClick;
            myMapView.MouseRightButtonDown += MyMapView_MouseRightButtonDown;
            myMapView.MouseRightButtonUp += MyMapView_MouseRightButtonUp;
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void chooseBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {

        }       

        private void hideBtn_Click(object sender, RoutedEventArgs e)
        {
            StackPanel s = (StackPanel)w.FindName("FeatureEdit");
            s.Visibility = Visibility.Collapsed;
        }

        private void EditVertexMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (curSelGraphic != null)//检查当前是否有选择图形 
            {
                operation = OperateType.EditVertex;
                if (curSelGraphic.Geometry.GeometryType == GeometryType.Point)//所选图形为点 
                {
                    selVertexLayer.Graphics.Clear();//清空顶点图层 
                    MapPoint pt = (MapPoint)curSelGraphic.Geometry; Graphic pg = new Graphic(pt, vertexSymbol);//创建新的点图形 
                    selVertexLayer.Graphics.Add(pg);
                }
                else if (curSelGraphic.Geometry.GeometryType == GeometryType.Polyline)//所选图 形为线 
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

        //计算点之前的距离
        public double GetDistanceBetweenPoints(MapPoint p1, MapPoint p2)
        {
            double dis = 0.0; dis = Math.Sqrt((p1.X - p2.X) * (p1.Y - p2.Y));
            return dis;
        }
        //判断包含关系 
        public bool IsEnvelopeContains(Envelope env, MapPoint pt)
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
                    pointCollection.Add(loc); if (pointCollection.Count >= 2)
                    {
                        if (pointCollection.Count > 2)
                        {
                            Graphic g = graphicsLayer.Graphics[graphicsLayer.Graphics.Count - 1];
                            PolylineBuilder lb = new PolylineBuilder(pointCollection); g.Geometry = lb.ToGeometry();
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
                    pointCollection.Add(loc); if (pointCollection.Count >= 3)
                    {
                        if (pointCollection.Count > 3)
                        {
                            Graphic g = graphicsLayer.Graphics[graphicsLayer.Graphics.Count - 1];
                            PolygonBuilder pb = new PolygonBuilder(pointCollection); g.Geometry = pb.ToGeometry();
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
    }
}

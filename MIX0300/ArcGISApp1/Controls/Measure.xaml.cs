using Esri.ArcGISRuntime.Data;
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
using System.Windows.Controls;
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
    /// Measure.xaml 的交互逻辑
    /// </summary>
    public partial class Measure : UserControl
    {
        //
        Window w;
        //
        MapView myMapView;
        //
        GraphicsOverlay measureOverlay = MainWindow.graphicsOverlay;
        //
        List<MapPoint> mapPoints = new List<MapPoint>();
        //
        List<double> lengthList = new List<double>();
        //
        List<double> areaList = new List<double>();

        public Measure()
        {
            InitializeComponent();
        }

        private async void myMeasureLine_Click(object sender, RoutedEventArgs e)
        {
            // 找到鼠标点击位置的坐标
            myMapView.GeoViewTapped += MyMapViewOnGeoViewTapped_Line;
            myMapView.GeoViewTapped -= MyMapViewOnGeoViewTapped_Area;
            myMeasureLine.IsEnabled = false;
            try
            {
                // 0表示point
                SketchCreationMode creationMode = SketchCreationMode.Polyline;
                Esri.ArcGISRuntime.Geometry.Geometry geometry = await myMapView.SketchEditor.StartAsync(creationMode, true);

                // Create and add a graphic from the geometry the user drew
                Graphic graphic = CreateGraphic(geometry);
                measureOverlay.Graphics.Add(graphic);               
            }
            catch (TaskCanceledException)
            {
                // Ignore ... let the user cancel drawing
            }
            catch (Exception ex)
            {
                // Report exceptions
                MessageBox.Show("Error drawing graphic shape: " + ex.Message);
            }

        }

        private void myStart_Click(object sender, RoutedEventArgs e)
        {  
            myStart.IsEnabled = false;
            myMeasureLine.IsEnabled = true;
            myMeasureArea.IsEnabled = true;
            myMeasureFeature.IsEnabled = true;
            myMeasureReset.IsEnabled = true;
        }

        private void MyMapViewOnGeoViewTapped_Line(object sender, GeoViewInputEventArgs geoViewInputEventArgs)
        {
            // Get the tapped point, projected to WGS84.
            MapPoint destination = (MapPoint)GeometryEngine.Project(geoViewInputEventArgs.Location, SpatialReferences.Wgs84);
            mapPoints.Add(destination);
            int len = mapPoints.Count();
            Esri.ArcGISRuntime.Geometry.PointCollection polylinePoints;
            Esri.ArcGISRuntime.Geometry.Geometry pathGeometry;
            Esri.ArcGISRuntime.Geometry.Polyline routeLine;
            if (len > 1)
            {
                polylinePoints = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84)
                {
                    mapPoints[len-2],
                    destination
                };

                routeLine = new Esri.ArcGISRuntime.Geometry.Polyline(polylinePoints);
                pathGeometry = GeometryEngine.DensifyGeodetic(routeLine, 1, LinearUnits.Kilometers, GeodeticCurveType.Geodesic);

                // 这是测地线的长度
                //double distance = GeometryEngine.LengthGeodetic(pathGeometry, LinearUnits.Kilometers, GeodeticCurveType.Geodesic);
                double distance = GeometryEngine.Length(pathGeometry);
                //double distance = GeometryEngine.Length(routeLine);
                lengthList.Add(distance);
                myMeasureResult.Text += "\n" + distance + " (地图默认长度单位)";
            }                     
        }

        // 创建图形
        private Graphic CreateGraphic(Esri.ArcGISRuntime.Geometry.Geometry geometry)
        {
            // Create a graphic to display the specified geometry
            Symbol symbol = null;
            switch (geometry.GeometryType)
            {
                // Symbolize with a fill symbol
                case GeometryType.Envelope:
                case GeometryType.Polygon:
                    {
                        symbol = new SimpleFillSymbol()
                        {
                            Color = System.Drawing.Color.FromArgb(251,214,206),
                            Style = SimpleFillSymbolStyle.Solid
                        };
                        break;
                    }
                // Symbolize with a line symbol
                case GeometryType.Polyline:
                    {
                        symbol = new SimpleLineSymbol()
                        {
                            Color = System.Drawing.Color.FromArgb(251, 214, 206),
                            Style = SimpleLineSymbolStyle.Solid,
                            Width = 5d
                        };
                        break;
                    }
                // Symbolize with a marker symbol
                case GeometryType.Point:
                case GeometryType.Multipoint:
                    {

                        symbol = new SimpleMarkerSymbol()
                        {
                            Color = System.Drawing.Color.FromArgb(251, 214, 206),
                            Style = SimpleMarkerSymbolStyle.Circle,
                            Size = 15d
                        };
                        break;
                    }
            }

            // pass back a new graphic with the appropriate symbol
            return new Graphic(geometry, symbol);
        }

        private async Task<Graphic> GetGraphicAsync()
        {
            // Wait for the user to click a location on the map
            Esri.ArcGISRuntime.Geometry.Geometry mapPoint = await myMapView.SketchEditor.StartAsync(SketchCreationMode.Point, false);

            // Convert the map point to a screen point
            Point screenCoordinate = myMapView.LocationToScreen((MapPoint)mapPoint);

            // Identify graphics in the graphics overlay using the point
            IReadOnlyList<IdentifyGraphicsOverlayResult> results = await myMapView.IdentifyGraphicsOverlaysAsync(screenCoordinate, 2, false);

            // If results were found, get the first graphic
            Graphic graphic = null;
            IdentifyGraphicsOverlayResult idResult = results.FirstOrDefault();
            if (idResult != null && idResult.Graphics.Count > 0)
            {
                graphic = idResult.Graphics.FirstOrDefault();
            }

            // Return the graphic (or null if none were found)
            return graphic;
        }

        private void myUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            w = Window.GetWindow(this);
            myMapView = (MapView)w.FindName("myMapView");
        }

        private async void myMeasureArea_Click(object sender, RoutedEventArgs e)
        {
            // 找到鼠标点击位置的坐标
            myMapView.GeoViewTapped += MyMapViewOnGeoViewTapped_Area;
            myMapView.GeoViewTapped -= MyMapViewOnGeoViewTapped_Line;
            myMeasureArea.IsEnabled = false;
            try
            {
                // 0表示point
                SketchCreationMode creationMode = SketchCreationMode.Polygon;
                Esri.ArcGISRuntime.Geometry.Geometry geometry = await myMapView.SketchEditor.StartAsync(creationMode, true);

                // Create and add a graphic from the geometry the user drew
                Graphic graphic = CreateGraphic(geometry);
                measureOverlay.Graphics.Add(graphic);
            }
            catch (TaskCanceledException)
            {
                // Ignore ... let the user cancel drawing
            }
            catch (Exception ex)
            {
                // Report exceptions
                MessageBox.Show("Error drawing graphic shape: " + ex.Message);
            }
        }

        private void MyMapViewOnGeoViewTapped_Area(object sender, GeoViewInputEventArgs geoViewInputEventArgs)
        {
            // Get the tapped point, projected to WGS84.
            MapPoint destination = (MapPoint)GeometryEngine.Project(geoViewInputEventArgs.Location, SpatialReferences.Wgs84);
            // 点集（mapPoints） ——记得清空
            mapPoints.Add(destination);
            int len = mapPoints.Count();
            Esri.ArcGISRuntime.Geometry.PointCollection polygonPoints;
            Esri.ArcGISRuntime.Geometry.Geometry areaGeometry;
            Esri.ArcGISRuntime.Geometry.Polygon routeArea;
            if (len > 2)
            {
                polygonPoints = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84)
                {
                    mapPoints[len-3],
                    mapPoints[len-2],
                    destination
                };

                routeArea = new Esri.ArcGISRuntime.Geometry.Polygon(polygonPoints);
                //pathGeometry = GeometryEngine.DensifyGeodetic(routeArea, 1, LinearUnits.Kilometers, GeodeticCurveType.Geodesic);

                // 这是测地线的长度
                //double distance = GeometryEngine.LengthGeodetic(pathGeometry, LinearUnits.Kilometers, GeodeticCurveType.Geodesic);
                double area = GeometryEngine.Area(routeArea);
                if (areaList.Count() != 0)
                {
                    area += areaList[areaList.Count() - 1];
                }
                areaList.Add(area);
                myMeasureResult.Text += "\n" + area + " (地图默认面积单位)";
            }
        }
    }
}

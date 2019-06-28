using Esri.ArcGISRuntime.Mapping;
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
using ArcGISApp1;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.UI.Controls;
using Esri.ArcGISRuntime.Data;
using Point = System.Windows.Point;

namespace ArcGISApp1.Controls
{
    /// <summary>
    /// DrawAndEdit.xaml 的交互逻辑
    /// </summary>
    public partial class DrawAndEdit : UserControl
    {
        // 成员变量
        GraphicsOverlay _sketchOverlay = MainWindow.graphicsOverlay;
        MapView myMapView;
        // 填充
        //public static SimpleFillSymbol drawSimpleFillSymbol = new SimpleFillSymbol()
        //{
        //    Outline = new SimpleLineSymbol()
        //    {
        //        Style = SimpleLineSymbolStyle.Solid,
        //        Width = 4,
        //        Color = System.Drawing.Color.Red
        //    },
        //    Style = SimpleFillSymbolStyle.Solid,
        //    Color = System.Drawing.Color.Red
        //};
        public static SimpleFillSymbol drawSimpleFillSymbol = Controls.SymbolFormCSY.FillCsy.simpleFillSymbol;
        public static SimpleLineSymbol drawLineSymbol = Controls.SymbolFormCSY.LineCsy.simpleLineSymbol;
        public static SimpleMarkerSymbol drawPointSymbol = Controls.SymbolFormCSY.PointCsy.simplePointSymbol;
        public DrawAndEdit()
        {
            InitializeComponent();
            // 初始化函数
            Initialize();
        }

     
        private void Initialize()
        {
            
        }

        private async void EditButtonClick(object sender, RoutedEventArgs e)
        {
            Window w = Window.GetWindow(this);
            myMapView = (MapView)w.FindName("myMapView");
            try
            {
                // Allow the user to select a graphic
                Graphic editGraphic = await GetGraphicAsync();
                if (editGraphic == null) { return; }

                // Let the user make changes to the graphic's geometry, await the result (updated geometry)
                Esri.ArcGISRuntime.Geometry.Geometry newGeometry = await myMapView.SketchEditor.StartAsync(editGraphic.Geometry);

                // Display the updated geometry in the graphic
                editGraphic.Geometry = newGeometry;
            }
            catch (TaskCanceledException)
            {
                // Ignore ... let the user cancel editing
            }
            catch (Exception ex)
            {
                // Report exceptions
                MessageBox.Show("绘制控件发生错误 : " + ex.Message);
            }
        }

        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            // Remove all graphics from the graphics overlay
            //GraphicsOverlay _sketchOverlay = MainWindow.graphicsOverlay;
            _sketchOverlay.Graphics.Clear();
            // Disable buttons that require graphics
            ClearButton.IsEnabled = false;
            EditButton.IsEnabled = false;
        }


        private async void DrawButtonClick(object sender, RoutedEventArgs e)
        {
            
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            
            //MainWindow t = new MainWindow();
            //GraphicsOverlay _sketchOverlay = MainWindow.graphicsOverlay;
            try
            {
                // Let the user draw on the map view using the chosen sketch mode
                SketchCreationMode creationMode = (SketchCreationMode)SketchModeComboBox.SelectedItem;
                Esri.ArcGISRuntime.Geometry.Geometry geometry = await myMapView.SketchEditor.StartAsync(creationMode, true);

                // Create and add a graphic from the geometry the user drew
                Graphic graphic = CreateGraphic(geometry);
                _sketchOverlay.Graphics.Add(graphic);
                
                // Enable/disable the clear and edit buttons according to whether or not graphics exist in the overlay
                ClearButton.IsEnabled = _sketchOverlay.Graphics.Count > 0;
                EditButton.IsEnabled = _sketchOverlay.Graphics.Count > 0;
            }
            catch (TaskCanceledException)
            {
                // Ignore ... let the user cancel drawing
            }
            catch (Exception ex)
            {
                // Report exceptions
                MessageBox.Show("绘制控件发生错误 : " + ex.Message);
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
                        symbol = drawSimpleFillSymbol;
                        break;
                    }
                // Symbolize with a line symbol
                case GeometryType.Polyline:
                    {
                        symbol = drawLineSymbol;
                        break;
                    }
                // Symbolize with a marker symbol
                case GeometryType.Point:
                case GeometryType.Multipoint:
                    {

                        symbol = drawPointSymbol;
                        break;
                    }
            }

            // pass back a new graphic with the appropriate symbol
            return new Graphic(geometry, symbol);
        }

        private async Task<Graphic> GetGraphicAsync()
        {
            Window w = Window.GetWindow(this);
            myMapView = (MapView)w.FindName("myMapView");
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

        private void drawAndEditCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Window w = Window.GetWindow(this);
            myMapView = (MapView)w.FindName("myMapView");
            ////////////////////////////////////////////////////////////////////////////////////////////////
            SketchModeComboBox.ItemsSource = System.Enum.GetValues(typeof(SketchCreationMode));
            SketchModeComboBox.SelectedIndex = 0;
            //SketchEditConfiguration config = myMapView.SketchEditor.EditConfiguration;
            //config.AllowVertexEditing = true;
            //config.ResizeMode = SketchResizeMode.Uniform;
            //config.AllowMove = true;
            // Set the sketch editor configuration to allow vertex editing, resizing, and moving
            SketchEditConfiguration config = myMapView.SketchEditor.EditConfiguration;
            config.AllowVertexEditing = true;
            config.ResizeMode = SketchResizeMode.Uniform;
            config.AllowMove = true;

            // Set the sketch editor as the page's data context
            DataContext = myMapView.SketchEditor;
        }

        private void drawAndEditCheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            wrapPanelCsy.IsEnabled = true;
            Window w = Window.GetWindow(this);
            myMapView = (MapView)w.FindName("myMapView");
            ////////////////////////////////////////////////////////////////////////////////////////////////
            SketchModeComboBox.ItemsSource = System.Enum.GetValues(typeof(SketchCreationMode));
            SketchModeComboBox.SelectedIndex = 0;
            //SketchEditConfiguration config = myMapView.SketchEditor.EditConfiguration;
            //config.AllowVertexEditing = true;
            //config.ResizeMode = SketchResizeMode.Uniform;
            //config.AllowMove = true;
            // Set the sketch editor configuration to allow vertex editing, resizing, and moving
            SketchEditConfiguration config = myMapView.SketchEditor.EditConfiguration;
            config.AllowVertexEditing = true;
            config.ResizeMode = SketchResizeMode.Uniform;
            config.AllowMove = true;

            // Set the sketch editor as the page's data context
            DataContext = myMapView.SketchEditor;
        }

        private void drawAndEditCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            wrapPanelCsy.IsEnabled = false;
        }

        private void StyleButton_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StyleButton.SelectedIndex == 0)
            {
                var window = new Window();
                Controls.SymbolFormCSY.PointCsy fillForm = new Controls.SymbolFormCSY.PointCsy();
                window.Content = fillForm;
                window.SizeToContent = SizeToContent.WidthAndHeight;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Title = "点符号渲染工具";
                //window.Closed += windowClosed_Polygon;
                window.Show();

            }
            else if (StyleButton.SelectedIndex == 1)
            {
                var window = new Window();
                Controls.SymbolFormCSY.LineCsy fillForm = new Controls.SymbolFormCSY.LineCsy();
                window.Content = fillForm;
                window.SizeToContent = SizeToContent.WidthAndHeight;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Title = "线符号渲染工具";
                //window.Closed += windowClosed_Polygon;
                window.Show();
            }
            else if (StyleButton.SelectedIndex == 2)
            {
                var window = new Window();
                Controls.SymbolFormCSY.FillCsy fillForm = new Controls.SymbolFormCSY.FillCsy();
                window.Content = fillForm;
                window.SizeToContent = SizeToContent.WidthAndHeight;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Title = "面符号渲染工具";    
                //window.Closed += windowClosed_Polygon;
                window.Show();
            }
        }

        //private void windowClosed_Polygon(object sender, EventArgs e)
        //{
        //    _sketchOverlay.Renderer = SymbolFormCSY.FillCsy.fillSymbol;
        //}
    }
}

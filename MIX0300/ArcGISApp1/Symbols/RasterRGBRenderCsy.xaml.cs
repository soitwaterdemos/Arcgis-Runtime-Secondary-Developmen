using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Rasters;
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
    /// RasterRGBRenderCsy.xaml 的交互逻辑
    /// </summary>
    public partial class RasterRGBRenderCsy : UserControl
    {
        
        public static RasterLayer _rasterLayer;
        Window w;
        MapView myMapView;

        public RasterRGBRenderCsy()
        {
            InitializeComponent();
            Initialize();
        }

        private async void Initialize()
        {
            //// Create a map with a streets basemap.
            //Map map = new Map(Basemap.CreateStreets());

            //// Get the file name for the local raster dataset.
            //string filepath = GetRasterPath();

            //// Load the raster file
            //Raster rasterFile = new Raster(filepath);

            //// Create a new raster layer to show the image.
            //_rasterLayer = new RasterLayer(rasterFile);

            //// Once the layer is loaded, enable the button to apply a new renderer.
            //await _rasterLayer.LoadAsync();

            //// Create a viewpoint with the raster's full extent.
            //Viewpoint fullRasterExtent = new Viewpoint(_rasterLayer.FullExtent);

            //// Set the initial viewpoint for the map.
            //map.InitialViewpoint = fullRasterExtent;

            //// Add the layer to the map.
            //map.OperationalLayers.Add(_rasterLayer);

            //// Add the map to the map view.
            //MyMapView.Map = map;

            // Add available stretch types to the combo box.
            StretchTypeComboBox.Items.Add("Min Max");
            StretchTypeComboBox.Items.Add("Percent Clip");
            StretchTypeComboBox.Items.Add("Standard Deviation");

            // Select "Min Max" as the stretch type.
            StretchTypeComboBox.SelectedIndex = 0;

            // Create a range of values from 0-255.
            List<int> minMaxValues = Enumerable.Range(0, 256).ToList();

            // Fill the min and max red combo boxes with the range and set default values.
            MinRedComboBox.ItemsSource = minMaxValues;
            MinRedComboBox.SelectedValue = 0;
            MaxRedComboBox.ItemsSource = minMaxValues;
            MaxRedComboBox.SelectedValue = 255;

            // Fill the min and max green combo boxes with the range and set default values.
            MinGreenComboBox.ItemsSource = minMaxValues;
            MinGreenComboBox.SelectedValue = 0;
            MaxGreenComboBox.ItemsSource = minMaxValues;
            MaxGreenComboBox.SelectedValue = 255;

            // Fill the min and max blue combo boxes with the range and set default values.
            MinBlueComboBox.ItemsSource = minMaxValues;
            MinBlueComboBox.SelectedValue = 0;
            MaxBlueComboBox.ItemsSource = minMaxValues;
            MaxBlueComboBox.SelectedValue = 255;

            // 最大最小百分比
            List<int> percentValues = Enumerable.Range(0, 100).ToList();
            MinimumValueSlider.ItemsSource = percentValues;
            MaximumValueSlider.ItemsSource = percentValues;
            MinimumValueSlider.SelectedValue = 0;
            MaximumValueSlider.SelectedIndex = 0;

            // 预设色带
            ColorRampCsy.SelectedIndex = 0;
            // 色带大小
            ColorRampSizeCsy.ItemsSource = minMaxValues;
            ColorRampSizeCsy.SelectedIndex = 255;

            // Fill the standard deviation factor combo box and set a default value.
            IEnumerable<int> wholeStdDevs = Enumerable.Range(1, 10);
            List<double> halfStdDevs = wholeStdDevs.Select(i => (double)i / 2).ToList();
            StdDeviationFactorComboBox.ItemsSource = halfStdDevs;
            StdDeviationFactorComboBox.SelectedValue = 2.0;
        }

        private void ApplyRgbRendererButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Create the correct type of StretchParameters with the corresponding user inputs.
            StretchParameters stretchParameters = null;

            // See which type is selected and apply the corresponding input parameters to create the renderer.
            switch (StretchTypeComboBox.SelectedValue.ToString())
            {
                case "Min Max":
                    // Read the minimum and maximum values for the red, green, and blue bands.
                    double minRed = Convert.ToDouble(MinRedComboBox.SelectedValue);
                    double minGreen = Convert.ToDouble(MinGreenComboBox.SelectedValue);
                    double minBlue = Convert.ToDouble(MinBlueComboBox.SelectedValue);
                    double maxRed = Convert.ToDouble(MaxRedComboBox.SelectedValue);
                    double maxGreen = Convert.ToDouble(MaxGreenComboBox.SelectedValue);
                    double maxBlue = Convert.ToDouble(MaxBlueComboBox.SelectedValue);

                    // Create an array of the minimum and maximum values.
                    double[] minValues = { minRed, minGreen, minBlue };
                    double[] maxValues = { maxRed, maxGreen, maxBlue };

                    // Create a new MinMaxStretchParameters with the values.
                    stretchParameters = new MinMaxStretchParameters(minValues, maxValues);
                    break;
                case "Percent Clip":
                    // Get the percentile cutoff below which values in the raster dataset are to be clipped.
                    double minimumPercent = MinimumValueSlider.SelectedIndex;

                    // Get the percentile cutoff above which pixel values in the raster dataset are to be clipped.
                    double maximumPercent = MaximumValueSlider.SelectedIndex;

                    // Create a new PercentClipStretchParameters with the inputs.
                    stretchParameters = new PercentClipStretchParameters(minimumPercent, maximumPercent);
                    break;
                case "Standard Deviation":
                    // Read the standard deviation factor (the number of standard deviations used to define the range of pixel values).
                    double standardDeviationFactor = Convert.ToDouble(StdDeviationFactorComboBox.SelectedValue);

                    // Create a new StandardDeviationStretchParameters with the selected number of standard deviations.
                    stretchParameters = new StandardDeviationStretchParameters(standardDeviationFactor);
                    break;
            }

            // Create an array to specify the raster bands (red, green, blue).
            int[] bands = { 0, 1, 2 };

            // Create the RgbRenderer with the stretch parameters created above, then apply it to the raster layer.

            RgbRenderer rgbRenderer;
            StretchRenderer stretchRenderer;

            if (stretchItem.IsEnabled == false)
            {
                rgbRenderer = new RgbRenderer(stretchParameters, bands, null, true);
                _rasterLayer.Renderer = rgbRenderer;
            }
            else if (stretchItem.IsEnabled == true)
            {
                PresetColorRampType pcrt;
                int colorType = Convert.ToInt32(ColorRampCsy.SelectedIndex - 1);
                switch(colorType)
                {
                    case -1:
                        pcrt = PresetColorRampType.Elevation;
                        break;
                    case 0:
                        pcrt = PresetColorRampType.Elevation;
                        break;
                    case 1:
                        pcrt = PresetColorRampType.DemScreen;
                        break;
                    case 2:
                        pcrt = PresetColorRampType.DemScreen;
                        break;
                    default:
                        pcrt = PresetColorRampType.DemScreen;
                        break;
                }
                uint colorSize = Convert.ToUInt32(ColorRampSizeCsy.SelectedValue);
                stretchRenderer = new StretchRenderer(stretchParameters, null, true, ColorRamp.Create(pcrt, colorSize));
                _rasterLayer.Renderer = stretchRenderer;
            }
        }

        private void StretchTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Hide all UI controls for the input parameters.
            MinMaxParametersGrid.Visibility = System.Windows.Visibility.Collapsed;
            PercentClipParametersGrid.Visibility = System.Windows.Visibility.Collapsed;
            StdDeviationParametersGrid.Visibility = System.Windows.Visibility.Collapsed;

            // See which type was selected and show the corresponding input controls.
            switch (StretchTypeComboBox.SelectedValue.ToString())
            {
                case "Min Max":
                    MinMaxParametersGrid.Visibility = System.Windows.Visibility.Visible;
                    break;
                case "Percent Clip":
                    PercentClipParametersGrid.Visibility = System.Windows.Visibility.Visible;
                    break;
                case "Standard Deviation":
                    StdDeviationParametersGrid.Visibility = System.Windows.Visibility.Visible;
                    break;
            }
        }

        private void myUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            w = Window.GetWindow(this);
            myMapView = (MapView)w.FindName("myMapView");
        }

        private void HideButton_Click(object sender, RoutedEventArgs e)
        {
            StackPanel s = (StackPanel)w.FindName("RasterRGBRenderCsy");
            s.Visibility = Visibility.Collapsed;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            stretchItem.IsEnabled = false;
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            stretchItem.IsEnabled = true;
        }
    }
}

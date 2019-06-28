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
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.Geometry;
using Colors = System.Drawing.Color;
using Grid = Esri.ArcGISRuntime.UI.Grid;
using Esri.ArcGISRuntime.UI.Controls;

namespace ArcGISApp1.Controls.OtherTool
{
    /// <summary>
    /// GridCsy.xaml 的交互逻辑
    /// </summary>
    public partial class GridCsy : UserControl
    {
        Window w;
        MapView myMapView;
        TextBlock t1;

        public GridCsy()
        {
            InitializeComponent();
            init();
        }

        private void init()
        {
            // Configure the UI options.
            GridTypeCombo.ItemsSource = new[] { "LatLong", "MGRS", "UTM", "USNG" };
            Colors[] colorItemsSource = { Colors.Red, Colors.Green, Colors.Blue, Colors.White, Colors.Purple };
            GridColorCombo.ItemsSource = colorItemsSource;
            LabelColorCombo.ItemsSource = colorItemsSource;
            HaloColorCombo.ItemsSource = colorItemsSource;
            LabelPositionCombo.ItemsSource = Enum.GetNames(typeof(GridLabelPosition));
            LabelFormatCombo.ItemsSource = Enum.GetNames(typeof(LatitudeLongitudeGridLabelFormat));
            ComboBox[] boxes = { GridTypeCombo, GridColorCombo, LabelColorCombo, HaloColorCombo, LabelPositionCombo, LabelFormatCombo };
            foreach (ComboBox combo in boxes)
            {
                combo.SelectedIndex = 0;
            }

            // Update the halo color so it isn't the same as the text color.
            HaloColorCombo.SelectedIndex = 3;

            // Subscribe to change events so the label format combo can be disabled as necessary.
            GridTypeCombo.SelectionChanged += (o, e) =>
            {
                LabelFormatCombo.IsEnabled = GridTypeCombo.SelectedItem.ToString() == "LatLong";
            };            
        }

        private void ApplySettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Grid grid;
            try
            {
                grid = myMapView.Grid;
            }
            catch(Exception ex1)
            {
                MessageBox.Show(ex1.Message);
            }

            // First, update the grid based on the type selected.
            switch (GridTypeCombo.SelectedValue.ToString())
            {
                case "LatLong":
                    grid = new LatitudeLongitudeGrid();
                    // Apply the label format setting.
                    string selectedFormatString = LabelFormatCombo.SelectedValue.ToString();
                    ((LatitudeLongitudeGrid)grid).LabelFormat =
                        (LatitudeLongitudeGridLabelFormat)Enum.Parse(typeof(LatitudeLongitudeGridLabelFormat),
                            selectedFormatString);
                    break;

                case "MGRS":
                    grid = new MgrsGrid();
                    break;

                case "UTM":
                    grid = new UtmGrid();
                    break;
                case "USNG":
                default:
                    grid = new UsngGrid();
                    break;
            }

            // Next, apply the label visibility setting.
            grid.IsLabelVisible = LabelVisibilityCheckbox.IsChecked.Value;
            grid.IsVisible = GridVisibilityCheckbox.IsChecked.Value;

            // Next, apply the grid color and label color settings for each zoom level.
            for (long level = 0; level < grid.LevelCount; level++)
            {
                // Set the line symbol.
                Symbol lineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid,
                    (Colors)GridColorCombo.SelectedValue, 2);
                grid.SetLineSymbol(level, lineSymbol);

                // Set the text symbol.
                Symbol textSymbol = new TextSymbol
                {
                    Color = (Colors)LabelColorCombo.SelectedValue,
                    OutlineColor = (Colors)HaloColorCombo.SelectedValue,
                    Size = 16,
                    HaloColor = (Colors)HaloColorCombo.SelectedValue,
                    HaloWidth = 3
                };
                grid.SetTextSymbol(level, textSymbol);
            }

            // Next, apply the label position setting.
            grid.LabelPosition = (GridLabelPosition)Enum.Parse(typeof(GridLabelPosition), LabelPositionCombo.SelectedValue.ToString());

            // Apply the updated grid.
            myMapView.Grid = grid;

        }

        private void myUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            w = Window.GetWindow(this);
            myMapView = (MapView)w.FindName("myMapView");
            t1 = (TextBlock)w.FindName("myTest");           
        }
    }
}

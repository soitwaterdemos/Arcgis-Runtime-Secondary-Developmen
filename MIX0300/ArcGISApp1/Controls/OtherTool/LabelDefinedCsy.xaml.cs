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

namespace ArcGISApp1.Controls.OtherTool
{
    /// <summary>
    /// LabelDefinedCsy.xaml 的交互逻辑
    /// </summary>
    public partial class LabelDefinedCsy : UserControl
    {
        Window w;
        MapView myMapView;
        public static List<string> fieldSource;
        public static int index = 0;
        string chooseField = "FID";
        FeatureLayer a;

        public LabelDefinedCsy()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            w = Window.GetWindow(this);
            myMapView = (MapView)w.FindName("myMapView");
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            string redLabelJson =
            @"{
                   ""labelExpressionInfo"":{""expression"":""return $feature." + chooseField + @";""},
                   ""labelPlacement"":""esriServerPolygonPlacementAlwaysHorizontal"",
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
                    ""labelExpressionInfo"":{""expression"":""return $feature." + chooseField + @";""},
                    ""labelPlacement"":""esriServerPolygonPlacementAlwaysHorizontal"",
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

            LabelDefinition redLabelDefinition = LabelDefinition.FromJson(redLabelJson);
            LabelDefinition blueLabelDefinition = LabelDefinition.FromJson(blueLabelJson);

            if (labelTypeComboBox.SelectedIndex == 0)
            {
                LabelDefinition l3 = LabelDefinition.FromJson(redLabelJson);
                l3 = LabelDefinition.FromJson(redLabelJson);
                a.LabelDefinitions.Add(redLabelDefinition);
                a.LabelDefinitions.Add(l3);
            }
            else if (labelTypeComboBox.SelectedIndex == 1)
            {
                LabelDefinition l3 = LabelDefinition.FromJson(redLabelJson);
                l3 = LabelDefinition.FromJson(blueLabelJson);
                a.LabelDefinitions.Add(blueLabelDefinition);
                a.LabelDefinitions.Add(l3);
            }

            a.LabelsEnabled = true;
        }

        private void fieldComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            chooseField = fieldComboBox.SelectedItem + "";
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            fieldComboBox.ItemsSource = fieldSource;
            fieldComboBox.IsEnabled = true;
            addBtn.IsEnabled = true;
            labelTypeComboBox.IsEnabled = true;
            a = (FeatureLayer)myMapView.Map.OperationalLayers[index];
            clearBtn.IsEnabled = true;
        }

        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            a.LabelDefinitions.Clear();
        }

        private void hideBtn_Click(object sender, RoutedEventArgs e)
        {
            StackPanel s = (StackPanel)w.FindName("AddLabelTool");
            s.Visibility = Visibility.Collapsed;
        }
    }
}

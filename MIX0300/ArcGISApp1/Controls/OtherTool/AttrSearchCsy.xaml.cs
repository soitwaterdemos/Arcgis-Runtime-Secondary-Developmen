using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
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
    /// AttrSearchCsy.xaml 的交互逻辑
    /// </summary>
    public partial class AttrSearchCsy : UserControl
    {
        Window w;
        MapView myMapView;
        TextBlock t1;
        public static int index = 0;
        public AttrSearchCsy()
        {
            InitializeComponent();
        }

        private void myUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            w = Window.GetWindow(this);
            myMapView = (MapView)w.FindName("myMapView");
            t1 = (TextBlock)w.FindName("myTest");
        }

        private async void sureBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FeatureLayer tempLayer = (FeatureLayer)myMapView.Map.OperationalLayers[index];
                Esri.ArcGISRuntime.Data.FeatureTable tempTable = tempLayer.FeatureTable;

                // 语句
                QueryParameters query = new QueryParameters();
                query.WhereClause = string.Format(inTxt.Text);

                FeatureQueryResult queryResult = await tempTable.QueryFeaturesAsync(query);
                IEnumerator<Feature> resultFeatures = queryResult.GetEnumerator();
                List<Feature> features = new List<Feature>();
                while (resultFeatures.MoveNext())
                {
                    features.Add(resultFeatures.Current);
                }

                MessageBox.Show(inTxt.Text + "\n" + features.Count + "\n" + query.WhereClause);

                //long row = tempTable.NumberOfFeatures;
                long row = features.Count;
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

                var window = new Window();
                window.Content = stackPanel;
                window.SizeToContent = SizeToContent.WidthAndHeight;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Title = "属性查询结果";
                window.Show();
            }
            catch(Exception ex2)
            {
                MessageBox.Show("查询错误!\n" + ex2.Message + "\n");
            }
            
        }
    }
}

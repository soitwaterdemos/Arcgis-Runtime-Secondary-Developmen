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
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI.Controls;

namespace ArcGISApp1.Controls.OtherTool
{
    /// <summary>
    /// ArcgisOnlineMap.xaml 的交互逻辑
    /// </summary>
    public partial class ArcgisOnlineMap : UserControl
    {

        Window w;
        MapView myMapView;

        public ArcgisOnlineMap()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            w = Window.GetWindow(this);
            myMapView = (MapView)w.FindName("myMapView2");
        }

        private async void sureBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MapView myMapView1 = (MapView)w.FindName("myMapView");
                myMapView1.Visibility = Visibility.Collapsed;
                myMapView.Visibility = Visibility.Visible;

                if (myMapView.Map != null)
                {
                    myMapView.Map = null;
                    Uri uri = new Uri(inUrl.Text);
                    myMapView.Map = await Map.LoadFromUriAsync(uri);
                }
            } catch (Exception e1)
            {
                MessageBox.Show("错误\n" + e1.Message);
            }
        }

        private void hideBtn_Click(object sender, RoutedEventArgs e)
        {
            StackPanel stack = (StackPanel)w.FindName("ArcgisOnlineMapPanel");
            stack.Visibility = Visibility.Collapsed;
        }

        private async void seeBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = myUrl.Text;
            proc.Start();
        }
    }
}

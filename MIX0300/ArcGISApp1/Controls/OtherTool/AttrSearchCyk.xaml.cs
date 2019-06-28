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
    /// AttrSearchCyk.xaml 的交互逻辑
    /// </summary>
    public partial class AttrSearchCyk : UserControl
    {
        Window w;
        MapView myMapView;
        TextBlock t1;

        public List<GeodatabaseFeatureTable> _tables;//表集合     
        public String sqlString; //查询语句的where子句      
        public GeodatabaseFeatureTable selectedTable;//当前所选要素表对象 

        public AttrSearchCyk()
        {
            InitializeComponent();
            //Tables = new List<GeodatabaseFeatureTable>();//初始化表集合            
            //sqlString = string.Empty;//初始化字符串             
            //selectedTable = null;
            //listBoxFields.Items.Clear();//清空字段组合框的内容            
            //listBoxFieldValue.Items.Clear();//清空字段值组合框的内容             
            //textBoxWhere.Text = string.Empty;//清空文本框 
        }

        private void myLayerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void myUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            w = Window.GetWindow(this);
            myMapView = (MapView)w.FindName("myMapView");
            t1 = (TextBlock)w.FindName("myTest");
        }
    }
}

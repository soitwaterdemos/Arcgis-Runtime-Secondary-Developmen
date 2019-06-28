using Esri.ArcGISRuntime.LocalServices;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Rasters;
using Esri.ArcGISRuntime.Tasks;
using Esri.ArcGISRuntime.Tasks.Geoprocessing;
using Esri.ArcGISRuntime.UI.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace ArcGISApp1.Controls.Hydrology
{
    /// <summary>
    /// ZonalStatisticsCsy.xaml 的交互逻辑
    /// </summary>
    public partial class ZonalStatisticsCsy : UserControl
    {
        Window w;
        MapView myMapView;
        TextBlock t1;

        // gp本地地址
        string toolUrl = "";
        // 目标文件名
        String layerName = "";
        string layerNameWithoutExtension = "";
        private LocalGeoprocessingService localServiceGP;
        // 
        private LocalGeoprocessingService _gpService;
        private GeoprocessingTask _gpTask;
        private GeoprocessingJob _gpJob;

        string inZonePath = "";
        string inValuePath = "";
        string rasterResultPath = "";
        string dbfResultPath = "";
        string style = "MEAN";
        string gpServiceUrl = System.IO.Path.GetFullPath(@"..\..\MyResources\zonalStatistics1.gpk");

        public ZonalStatisticsCsy()
        {
            InitializeComponent();
        }

        private void myUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            w = Window.GetWindow(this);
            myMapView = (MapView)w.FindName("myMapView");
            t1 = (TextBlock)w.FindName("myTest");
        }

        private void styleBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = styleBox.SelectedItem as ComboBoxItem;
            style = item.Content.ToString();
        }

        private void inZoneBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofdlg = new System.Windows.Forms.OpenFileDialog();
            ofdlg.Filter = "TIFF文件(*.tif)|*.tif";
            ofdlg.Title = "打开GeoTiff文件";
            if (ofdlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                inZonePath = ofdlg.FileName;
                inZoneTxt.Text = ofdlg.FileName;
                layerNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(ofdlg.FileName);
            }
        }

        private void inValueBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofdlg = new System.Windows.Forms.OpenFileDialog();
            ofdlg.Filter = "TIFF文件(*.tif)|*.tif";
            ofdlg.Title = "打开GeoTiff文件";
            if (ofdlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                inValuePath = ofdlg.FileName;
                inValueTxt.Text = ofdlg.FileName;
            }
        }

        private void outRasterBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog fbd = new System.Windows.Forms.SaveFileDialog()
            {
                Title = "保存为",
                FileName = layerNameWithoutExtension + "_zonal.tif",
                Filter = "TIFF文件|*.tif;"
            };       
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                rasterResultPath = fbd.FileName;
                outRasterTxt.Text = fbd.FileName;
            }
        }

        private void outDBFBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog fbd = new System.Windows.Forms.SaveFileDialog()
            {
                Title = "保存为",
                FileName = layerNameWithoutExtension + "_zonal.dbf",
                Filter = "DBF文件|*.dbf;"
            };
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                dbfResultPath = fbd.FileName;
                outDBFTxt.Text = fbd.FileName;
            }
        }

        private void sureBtn_Click(object sender, RoutedEventArgs e)
        {
            if (inZonePath == "" || inValuePath == "" || rasterResultPath == "" || dbfResultPath == "")
            {
                t1.Text = "注意: 请检查参数是否填写完整.";
                return;
            }
            something();
            showBtn.IsEnabled = true;
            openInBowserBtn.IsEnabled = true;
            myUserControl.IsEnabled = false;
        }

        private void resetBtn_Click(object sender, RoutedEventArgs e)
        {
            inZonePath = "";
            inValuePath = "";
            rasterResultPath = "";
            dbfResultPath = "";
            style = "MEAN";

            inZoneTxt.Text = "";
            inValueTxt.Text = "";
            outRasterTxt.Text = "";
            outDBFTxt.Text = "";
            styleBox.SelectedIndex = 0;
        }

        private async void showBtn_Click(object sender, RoutedEventArgs e)
        {
            // 读取栅格文件
            Raster myRasterFile = new Raster(rasterResultPath);
            // 创建栅格图层
            RasterLayer myRasterLayer = new RasterLayer(myRasterFile);
            // 显示到窗口
            MainWindow.myMapLayersList.Add(myRasterLayer);
            // 加入到业务图层列表
            myMapView.Map.OperationalLayers.Add(myRasterLayer);
            // 等待图层加载完成
            await myRasterLayer.LoadAsync();
            // 设置四至，缩放至图层
            await myMapView.SetViewpointGeometryAsync(myRasterLayer.FullExtent);
            // 加入到checkbox列表
            ((TableOfContent)w.FindName("myTableOfContent")).addLayersList(System.IO.Path.GetFileName(rasterResultPath));

            try {
                System.Diagnostics.Process.Start(System.IO.Path.GetFileNameWithoutExtension(dbfResultPath));
            } catch (Exception e1) {

            }
        }

        private void openInBowserBtn_Click(object sender, RoutedEventArgs e)
        {
            if (toolUrl != "")
            {
                t1.Text = "正打开链接: " + toolUrl;
                Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = toolUrl;
                proc.Start();
            }
        }

        private async void something()
        {
            localServiceGP = new LocalGeoprocessingService(gpServiceUrl);
            localServiceGP.ServiceType = GeoprocessingServiceType.SynchronousExecute;
            localServiceGP.StatusChanged += async (svc, args) =>
            {
                if (args.Status == LocalServerStatus.Started)
                {
                    // 获取工具的本地地址
                    var gpSvcUrl = (svc as LocalGeoprocessingService).Url.AbsoluteUri + "/zonalStatistics1";
                    // 在浏览器打开
                    toolUrl = gpSvcUrl;
                    GeoprocessingTask gpRouteTask = new GeoprocessingTask(new Uri(gpSvcUrl));
                    GeoprocessingParameters para = new GeoprocessingParameters(GeoprocessingExecutionType.SynchronousExecute);

                    para.Inputs.Add("iZone", new GeoprocessingRaster(new Uri(inZonePath), "GPRasterDataLayer"));
                    para.Inputs.Add("iData", new GeoprocessingRaster(new Uri(inValuePath), "GPRasterDataLayer"));
                    para.Inputs.Add("iStyle", new GeoprocessingString(style));
                    para.Inputs.Add("oRaster", new GeoprocessingString(rasterResultPath));
                    para.Inputs.Add("oDbf", new GeoprocessingString(dbfResultPath));

                    GeoprocessingJob routeJob = gpRouteTask.CreateJob(para);

                    try
                    {
                        t1.Text = "处理中...";
                        GeoprocessingResult geoprocessingResult = await routeJob.GetResultAsync();
                        myUserControl.IsEnabled = true;
                        t1.Text = "分区统计处理完成.";
                    }
                    catch (Exception ex)
                    {
                        if (routeJob.Status == JobStatus.Failed && routeJob.Error != null)
                            MessageBox.Show("GP流处理错误:\n" + routeJob.Error.Message, "Geoprocessing error");
                        else
                            MessageBox.Show("非GP流错误:\n" + ex.ToString() + "/**/", "Sample error");
                    }
                }
            };
            await localServiceGP.StartAsync();
        }
    }
}

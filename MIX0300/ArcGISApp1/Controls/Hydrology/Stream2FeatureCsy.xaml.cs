using Esri.ArcGISRuntime.Data;
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
    /// Stream2FeatureCsy.xaml 的交互逻辑
    /// </summary>
    public partial class Stream2FeatureCsy : UserControl
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

        // Hold a reference to the task
        private GeoprocessingTask _gpTask;

        // Hold a reference to the job
        private GeoprocessingJob _gpJob;

        string rasterPath = @"C:\Users\CSY\Desktop\GISex2016\GISData\Result\demTiff.tif";
        string rasterFlowDirPath = @"C:\Users\CSY\Desktop\GISex2016\GISData\Result\demTiff.tif";
        string rasterResultPath = @"C:\Users\CSY\Desktop\GISex2016\GISData\Result\demTiff123.tif";
        string gpServiceUrl = System.IO.Path.GetFullPath(@"..\..\MyResources\stream2Feature1.gpk");


        public Stream2FeatureCsy()
        {
            InitializeComponent();
        }

        private void myUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            w = Window.GetWindow(this);
            myMapView = (MapView)w.FindName("myMapView");
            t1 = (TextBlock)w.FindName("myTest");
        }

        private async void inRasterBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofdlg = new System.Windows.Forms.OpenFileDialog();
            ofdlg.Filter = "TIFF文件(*.tif)|*.tif";
            ofdlg.Title = "打开GeoTiff文件";
            if (ofdlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                rasterPath = ofdlg.FileName;
                //
                inRasterTxt.Text = rasterPath;
                // 获取文件名
                layerName = System.IO.Path.GetFileName(rasterPath);
                layerNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(rasterPath);
            }
        }

        private void inFlowDirRasterBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofdlg = new System.Windows.Forms.OpenFileDialog();
            ofdlg.Filter = "TIFF文件(*.tif)|*.tif";
            ofdlg.Title = "打开GeoTiff文件";
            if (ofdlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                rasterFlowDirPath = ofdlg.FileName;
                //
                inFlowDirRasterTxt.Text = rasterPath;
            }
        }

        private void outRasterBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog fbd = new System.Windows.Forms.SaveFileDialog()
            {
                Title = "保存为",
                FileName = layerNameWithoutExtension + ".shp",
                Filter = "Shapefile文件|*.shp;"
            };
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                rasterResultPath = fbd.FileName;
                outRasterTxt.Text = fbd.FileName;
            }        
        }

        private void sureBtn_Click(object sender, RoutedEventArgs e)
        {
            if (outRasterTxt.Text == "" || inRasterTxt.Text != rasterPath)
            {
                t1.Text = "注意: 请选择目标图层位置或结果保存位置.";
                return;
            }
            something();
            showBtn.IsEnabled = true;
            openInBowserBtn.IsEnabled = true;
            myUserControl.IsEnabled = false;
        }

        private void resetBtn_Click(object sender, RoutedEventArgs e)
        {
            rasterPath = @"C:\Users\CSY\Desktop\GISex2016\GISData\Result\demTiff.tif";
            rasterResultPath = @"C:\Users\CSY\Desktop\GISex2016\GISData\Result\demTiff.tif";
            rasterFlowDirPath = @"C:\Users\CSY\Desktop\GISex2016\GISData\Result\demTiff.tif";
            inFlowDirRasterTxt.Text = "";
            inRasterTxt.Text = "";
            outRasterTxt.Text = "";
        }

        private async void showBtn_Click(object sender, RoutedEventArgs e)
        {
            ShapefileFeatureTable myShapefile = await ShapefileFeatureTable.OpenAsync(rasterResultPath);
            FeatureLayer newFeaturelayer = new FeatureLayer(myShapefile);
            
            MainWindow.myMapLayersList.Add(newFeaturelayer);
            myMapView.Map.OperationalLayers.Add(newFeaturelayer);
            await myMapView.SetViewpointGeometryAsync(newFeaturelayer.FullExtent);
            // 获取文件名
            String layerName = System.IO.Path.GetFileName(rasterResultPath);
            // 新建checkbox并入栈
            ((TableOfContent)w.FindName("myTableOfContent")).addLayersList(layerName);
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
                    var gpSvcUrl = (svc as LocalGeoprocessingService).Url.AbsoluteUri + "/stream2Feature1";
                    // 在浏览器打开
                    toolUrl = gpSvcUrl;
                    GeoprocessingTask gpRouteTask = new GeoprocessingTask(new Uri(gpSvcUrl));
                    GeoprocessingParameters para = new GeoprocessingParameters(GeoprocessingExecutionType.SynchronousExecute);

                    para.Inputs.Add("i", new GeoprocessingRaster(new Uri(rasterPath), "GPRasterDataLayer"));
                    para.Inputs.Add("ifd", new GeoprocessingRaster(new Uri(rasterFlowDirPath), "GPRasterDataLayer"));
                    para.Inputs.Add("o", new GeoprocessingString(rasterResultPath));

                    GeoprocessingJob routeJob = gpRouteTask.CreateJob(para);

                    try
                    {
                        t1.Text = "处理中...";
                        GeoprocessingResult geoprocessingResult = await routeJob.GetResultAsync();
                        myUserControl.IsEnabled = true;
                        t1.Text = "栅格河网矢量化处理完成.";
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

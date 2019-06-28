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


namespace ArcGISApp1.Controls.DataChangeCsy
{
    /// <summary>
    /// DBF2ExcelCsy.xaml 的交互逻辑
    /// </summary>
    public partial class DBF2ExcelCsy : UserControl
    {
        Window w;
        MapView myMapView;
        TextBlock t1;

        string toolUrl = "";
        String layerName = "";
        string layerNameWithoutExtension = "";
        private LocalGeoprocessingService localServiceGP;

        private LocalGeoprocessingService _gpService;
        private GeoprocessingTask _gpTask;
        private GeoprocessingJob _gpJob;

        string rasterPath = @"C:\Users\CSY\Desktop\GISex2016\GISData\Result\demTiff.tif";
        string rasterResultPath = @"C:\Users\CSY\Desktop\GISex2016\GISData\Result\demTiff123.tif";
        string gpServiceUrl = System.IO.Path.GetFullPath(@"..\..\MyResources\table2Excel.gpk");

        public DBF2ExcelCsy()
        {
            InitializeComponent();
        }

        private void myUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            w = Window.GetWindow(this);
            myMapView = (MapView)w.FindName("myMapView");
            t1 = (TextBlock)w.FindName("myTest");
        }

        private void inRasterBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofdlg = new System.Windows.Forms.OpenFileDialog();
            ofdlg.Filter = "DBF文件(*.dbf)|*.dbf";
            ofdlg.Title = "打开DBF文件";
            if (ofdlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                rasterPath = ofdlg.FileName;
                inRasterTxt.Text = ofdlg.FileName;
                layerName = System.IO.Path.GetFileName(rasterPath);
                layerNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(rasterPath);
            }
        }

        private void outRasterBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog fbd = new System.Windows.Forms.SaveFileDialog()
            {
                Title = "保存为",
                FileName = layerNameWithoutExtension + ".xls",
                Filter = "Excel文件|*.xls;"
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
                t1.Text = "注意: 参数设置不能为空.";
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
            inRasterTxt.Text = "";
            outRasterTxt.Text = "";
        }

        private async void showBtn_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo(rasterResultPath);
            Process.Start(psi);
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
                    var gpSvcUrl = (svc as LocalGeoprocessingService).Url.AbsoluteUri + "/table2Excel";
                    // 在浏览器打开
                    toolUrl = gpSvcUrl;
                    GeoprocessingTask gpRouteTask = new GeoprocessingTask(new Uri(gpSvcUrl));
                    GeoprocessingParameters para = new GeoprocessingParameters(GeoprocessingExecutionType.SynchronousExecute);

                    para.Inputs.Add("i", new GeoprocessingString(rasterPath));
                    para.Inputs.Add("o", new GeoprocessingString(rasterResultPath));

                    GeoprocessingJob routeJob = gpRouteTask.CreateJob(para);

                    try
                    {
                        t1.Text = "处理中...";
                        GeoprocessingResult geoprocessingResult = await routeJob.GetResultAsync();
                        myUserControl.IsEnabled = true;
                        t1.Text = "表转Excel处理完成.";
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

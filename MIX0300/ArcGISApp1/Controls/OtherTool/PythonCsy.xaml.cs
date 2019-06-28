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
namespace ArcGISApp1.Controls.OtherTool
{
    /// <summary>
    /// PythonCsy.xaml 的交互逻辑
    /// </summary>
    public partial class PythonCsy : UserControl
    {
        Window w;
        MapView myMapView;
        TextBlock t1;

        string pyPath = "";
        string rasterPath = "";
        string rasterResultPath = "";

        String py105Path = @"C:\Python27\ArcGIS10.5\python.exe";
        string layerNameWithoutExtension = "";

        public PythonCsy()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            w = Window.GetWindow(this);
            myMapView = (MapView)w.FindName("myMapView");
            t1 = (TextBlock)w.FindName("myTest");
        }

        private void inPy_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofdlg = new System.Windows.Forms.OpenFileDialog();
            ofdlg.Filter = "Python脚本文件(*.py)|*.py";
            ofdlg.Title = "打开Python文件";
            if (ofdlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pyPath = ofdlg.FileName;
                inPyTxt.Text = ofdlg.FileName;
            }
        }

        private void inRasterBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofdlg = new System.Windows.Forms.OpenFileDialog();
            ofdlg.Filter = "TIFF文件(*.tif)|*.tif";
            ofdlg.Title = "打开GeoTiff文件";
            if (ofdlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                rasterPath = ofdlg.FileName;
                inRasterTxt.Text = ofdlg.FileName;
                layerNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(ofdlg.FileName);
            }
        }

        private void outRasterBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog fbd = new System.Windows.Forms.SaveFileDialog()
            {
                Title = "保存为",
                FileName = layerNameWithoutExtension + "_pyResult.tif",
                Filter = "TIFF文件|*.tif;"
            };
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                rasterResultPath = fbd.FileName;
                outRasterTxt.Text = fbd.FileName;
            }
        }

        private void sureBtn_Click(object sender, RoutedEventArgs e)
        {
            if (pyPath == "" || rasterResultPath == "" || rasterPath == "")
            {
                t1.Text = "注意: 请检查参数是否填写完整.";
                return;
            }

            string arg1_ = rasterPath;//获取参数arg1[](待合并图层集合)
            string[] arg1 = arg1_.Split('\r', '\n');
            string arg2 = rasterResultPath;//arg2(输出图层)
            List<string> listArr = arg1.ToList();
            listArr.Add(arg2);
            string[] strArr = listArr.ToArray(); //参数列表，需要传递的参数
            string sArguments = pyPath;//这里是python的文件名字
            RunPythonScript(sArguments, "-u", strArr);//运行脚本文件
        
       
        }

        public void RunPythonScript(string sArgName, string args = "", params string[] teps)
        {
            Process p = new Process();
            string path = pyPath;// 待处理python文件的路径
            string sArguments = path;
            foreach (string sigstr in teps)//添加参数
            {
                sArguments += " " + sigstr;//传递参数
            }
            //下面为启动一个进程来执行脚本文件设置参数
            p.StartInfo.FileName = py105Path; //注意路径
            p.StartInfo.Arguments = sArguments;//这样参数就是merge.py 路径1 路径2 路径3....
            //Console.WriteLine(sArguments);
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();//启动进程
        }

        private void resetBtn_Click(object sender, RoutedEventArgs e)
        {
            rasterPath = "";
            pyPath = "";
            rasterResultPath = "";

            inPyTxt.Text = "";
            inRasterTxt.Text = "";
            outRasterTxt.Text = "";
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
        }

        private void Py_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofdlg = new System.Windows.Forms.OpenFileDialog();
            ofdlg.Filter = "Python脚本文件(*.exe)|*.exe";
            ofdlg.Title = "打开本地Python";
            if (ofdlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                py105Path = ofdlg.FileName;
                PyTxt.Text = ofdlg.FileName;
            }
        }
    }
}

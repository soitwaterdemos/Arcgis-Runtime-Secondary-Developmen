using Esri.ArcGISRuntime.Symbology;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArcGISApp1
{
    public enum enumMarkerType
    {
        SimpleMarker,
        PictureMarker,
        Text
    }
    public partial class PointSymbolForm : Form
    {
        // 符号类型
        public enumMarkerType MarkerType;
        // 颜色
        public Color resultColor = Color.Red;
        // 点样式--类型
        public SimpleMarkerSymbolStyle simpleMarkerStyle;
        // 点样式大小
        public int symbolSize = 12;
        // 图片地址(本地)
        public Uri ImgUri;
        // 测试 *********************************************************************************************************************************
        //public string test = "";

        public PointSymbolForm()
        {
            InitializeComponent();
        }

        // 只能输入数字
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }
        }

        // 颜色选择器
        private void myPointSymbolColorChoose_Click(object sender, EventArgs e)
        {
            ColorDialog ColorForm = new ColorDialog();
            if (ColorForm.ShowDialog() == DialogResult.OK)
            {
                resultColor = ColorForm.Color;
                myPointSymbolColorChoose.BackColor = resultColor;
            }
        }

        // ’确认‘按钮
        private void myPointSymbolSure_Click(object sender, EventArgs e)
        { 
            if (myPointComboBox.SelectedIndex == 0)
            {
                MarkerType = enumMarkerType.SimpleMarker;
                symbolSize = Convert.ToInt32(myPointSymbolSize.Text);
            }
            else if (myPointComboBox.SelectedIndex == 1)
            {
                MarkerType = enumMarkerType.PictureMarker;
            }
            else if (myPointComboBox.SelectedIndex == 2)
            {
                MarkerType = enumMarkerType.Text;
            }
            this.DialogResult = DialogResult.OK;
            
        }

        // 窗口加载后函数
        private void PointSymbolForm_Load(object sender, EventArgs e)
        {
            // 默认为0
            myPointComboBox.SelectedIndex = 0;
        }

        // 导入图片
        private void inputPictureAsPointSymbol_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofDlg = new OpenFileDialog();
            ofDlg.Title = "选择图像文件";
            ofDlg.Filter = "Bitmap图像（*.bmp）|*.bmp|Png图像（*.png） |*.png|Jpeg图像（*.jpg）|*.jpg|Tif图像（*.tif）|*.tif|所有文件（*.*）|*.*";
            if (ofDlg.ShowDialog() == DialogResult.OK)
            {
                myPointPictureBox.Load(ofDlg.FileName);
                myPointPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                string path = System.IO.Path.GetFullPath(ofDlg.FileName);
                ImgUri = new Uri(path);
            }
        }

        private void myPointSymbolStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (myPointSymbolStyle.SelectedIndex)
            {
                case 0: simpleMarkerStyle = SimpleMarkerSymbolStyle.Circle; break;
                case 1: simpleMarkerStyle = SimpleMarkerSymbolStyle.Cross; break;
                case 2: simpleMarkerStyle = SimpleMarkerSymbolStyle.Diamond; break;
                case 3: simpleMarkerStyle = SimpleMarkerSymbolStyle.Square; break;
                case 4: simpleMarkerStyle = SimpleMarkerSymbolStyle.Triangle; break;
                case 5: simpleMarkerStyle = SimpleMarkerSymbolStyle.X; break;
                default: simpleMarkerStyle = SimpleMarkerSymbolStyle.Circle; break;
            }
        }

        // 矢量还是图片
        private void myPointComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (myPointComboBox.SelectedIndex == 0)
            {
                myPointGroupBoxVector.Enabled = true;
                myPointGroupBoxPicture.Enabled = false;
            }
            else if (myPointComboBox.SelectedIndex == 1)
            {
                myPointGroupBoxVector.Enabled = false;
                myPointGroupBoxPicture.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}

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
    public partial class FillSymbolForm : Form
    {
        // 边框
        public SimpleLineSymbol simpleLineSymbol = new SimpleLineSymbol()
        {
            Style = SimpleLineSymbolStyle.Solid,
            Width = 4,
            Color = Color.Red
        };
        // 图片地址(本地)
        public Uri ImgUri;
        // 面符号-矢量
        public SimpleFillSymbol simpleFillSymbol = new SimpleFillSymbol();
        // 面符号类型
        public enumMarkerType MarkerType;
        // 颜色
        public Color resultColor = Color.Red;
        // 边框颜色
        public Color outlineColor = Color.Gray;
        // 图片
        public PictureFillSymbol pictureFillSymbol;

        public FillSymbolForm()
        {
            InitializeComponent();
        }

        // 矢量或者图片
        private void myFillSymbolComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (myFillSymbolComboBox.SelectedIndex == 0)
            {
                myFillSymGroupBoxVector.Enabled = true;
                myFillSymGroupBoxPicture.Enabled = false;
            }
            else if (myFillSymbolComboBox.SelectedIndex == 1)
            {
                myFillSymGroupBoxVector.Enabled = false;
                myFillSymGroupBoxPicture.Enabled = true;
            }
        }

        private void myInputImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofDlg = new OpenFileDialog();
            ofDlg.Title = "选择图像文件";
            ofDlg.Filter = "Bitmap图像（*.bmp）|*.bmp|Png图像（*.png） |*.png|Jpeg图像（*.jpg）|*.jpg|Tif图像（*.tif）|*.tif|所有文件（*.*）|*.*";
            if (ofDlg.ShowDialog() == DialogResult.OK)
            {
                myPictureBox.Load(ofDlg.FileName);
                myPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                string path = System.IO.Path.GetFullPath(ofDlg.FileName);
                ImgUri = new Uri(path);
            }
        }

        // 窗口初始化加载
        private void FillSymbolForm_Load(object sender, EventArgs e)
        {
            // 默认为0
            myFillSymbolComboBox.SelectedIndex = 0;
            // myLineStyle
            for (int i = 1; i <= 5; i++)
            {
                myLineStyle.Items.Add(i.ToString());
            }

            this.myLineStyle.DrawMode = DrawMode.OwnerDrawFixed;
            this.myLineStyle.DrawItem += delegate (object cmb, DrawItemEventArgs args)
            {
                args.DrawBackground(); System.Drawing.Drawing2D.DashStyle ds;
                switch (args.Index)
                {
                    case 0: ds = System.Drawing.Drawing2D.DashStyle.Solid; break;
                    case 1: ds = System.Drawing.Drawing2D.DashStyle.Dash; break;
                    case 2: ds = System.Drawing.Drawing2D.DashStyle.Dot; break;
                    case 3: ds = System.Drawing.Drawing2D.DashStyle.DashDot; break;
                    case 4: ds = System.Drawing.Drawing2D.DashStyle.DashDotDot; break;
                    default: ds = System.Drawing.Drawing2D.DashStyle.Custom; break;
                }
                Rectangle r = args.Bounds; float ly = r.Bottom - r.Height / 2;
                using (Pen p = new Pen(Color.FromArgb(0, 0, 255), 2))
                {
                    p.DashStyle = ds; args.Graphics.DrawLine(p, r.Left, ly, r.Right, ly);
                }
            };
        }

        // 边框颜色
        private void myLineColor_Click(object sender, EventArgs e)
        {
            ColorDialog ColorForm = new ColorDialog();
            if (ColorForm.ShowDialog() == DialogResult.OK)
            {
                outlineColor = ColorForm.Color;
                myLineColor.BackColor = ColorForm.Color;
            }
        }

        // 只能输入数字
        private void myLineSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }
        }

        // 边框样式
        private void myLineStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (myLineStyle.SelectedIndex)
            {
                case 0: simpleLineSymbol.Style = SimpleLineSymbolStyle.Solid; break;
                case 1: simpleLineSymbol.Style = SimpleLineSymbolStyle.Dash; break;
                case 2: simpleLineSymbol.Style = SimpleLineSymbolStyle.Dot; break;
                case 3: simpleLineSymbol.Style = SimpleLineSymbolStyle.DashDot; break;
                case 4: simpleLineSymbol.Style = SimpleLineSymbolStyle.DashDotDot; break;
                default: simpleLineSymbol.Style = SimpleLineSymbolStyle.Null; break;
            }
        }

        private void myVectorColor_Click(object sender, EventArgs e)
        {
            ColorDialog ColorForm = new ColorDialog();
            if (ColorForm.ShowDialog() == DialogResult.OK)
            {
               resultColor = ColorForm.Color;
                myVectorColor.BackColor = ColorForm.Color;
            }
        }

        private void myVectorStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (myVectorStyle.SelectedIndex)
            {
                case 0: simpleFillSymbol.Style = SimpleFillSymbolStyle.Solid; break;
                case 1: simpleFillSymbol.Style = SimpleFillSymbolStyle.Horizontal; break;
                case 2: simpleFillSymbol.Style = SimpleFillSymbolStyle.Vertical; break;
                case 3: simpleFillSymbol.Style = SimpleFillSymbolStyle.Cross; break;
                case 4: simpleFillSymbol.Style = SimpleFillSymbolStyle.DiagonalCross; break;
                case 5: simpleFillSymbol.Style = SimpleFillSymbolStyle.BackwardDiagonal; break;
                case 6: simpleFillSymbol.Style = SimpleFillSymbolStyle.ForwardDiagonal; break;
                default: simpleFillSymbol.Style = SimpleFillSymbolStyle.Null; break;
            }
        }

        private void myFillSymCancelBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void myFillSymSureBtn_Click(object sender, EventArgs e)
        {
            if (myFillSymbolComboBox.SelectedIndex == 0)
            {
                MarkerType = enumMarkerType.SimpleMarker;
                simpleFillSymbol.Color = resultColor;
                simpleLineSymbol.Color = outlineColor;
                simpleFillSymbol.Outline = simpleLineSymbol;
            }
            else if (myFillSymbolComboBox.SelectedIndex == 1)
            {
                MarkerType = enumMarkerType.PictureMarker;
                //simpleFillSymbol.Outline = simpleLineSymbol;
                pictureFillSymbol = new PictureFillSymbol(ImgUri);
                pictureFillSymbol.Outline = simpleLineSymbol;
            }
            this.DialogResult = DialogResult.OK;
        }
    }
}

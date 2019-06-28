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

    public partial class LineSymbolForm : Form
    {
        // 颜色
        public Color resultColor;
        // 线宽
        public int symbolSize;
        // 样式
        public SimpleLineSymbolStyle lineSymbolStyle;
        // 线符号
        public SimpleLineSymbol simpleLineSymbol;

        public LineSymbolForm()
        {
            InitializeComponent();
        }


        // 只能输入数字
        private void myLineSymbolSizeTxtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }
        }

        private void myLineSymbolColorBox_Click(object sender, EventArgs e)
        {
            ColorDialog ColorForm = new ColorDialog();
            if (ColorForm.ShowDialog() == DialogResult.OK)
            {
                resultColor = ColorForm.Color;
                myLineSymbolColorBox.BackColor = resultColor;
            }
        }

        private void LineSymbolForm_Load(object sender, EventArgs e)
        {
            for (int i = 1; i <= 5; i++)
            {
                myLineSymbolStyleComboBox.Items.Add(i.ToString());
            }

            this.myLineSymbolStyleComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            this.myLineSymbolStyleComboBox.DrawItem += delegate (object cmb, DrawItemEventArgs args)
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
            // 颜色
            resultColor = this.myLineSymbolColorBox.BackColor;
            // 宽度
            symbolSize = 3;
            // 样式
            lineSymbolStyle = SimpleLineSymbolStyle.Null;
        }

        private void myLineSymbolStyleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (myLineSymbolStyleComboBox.SelectedIndex)
            {
                case 0: lineSymbolStyle = SimpleLineSymbolStyle.Solid; break;
                case 1: lineSymbolStyle = SimpleLineSymbolStyle.Dash; break;
                case 2: lineSymbolStyle = SimpleLineSymbolStyle.Dot; break;
                case 3: lineSymbolStyle = SimpleLineSymbolStyle.DashDot; break;
                case 4: lineSymbolStyle = SimpleLineSymbolStyle.DashDotDot; break;
                default: lineSymbolStyle = SimpleLineSymbolStyle.Null; break;
            }
        }

        private void myLineSymbolSureBtn_Click(object sender, EventArgs e)
        {
            simpleLineSymbol = new SimpleLineSymbol()
            {
                Style = lineSymbolStyle,
                Width = Convert.ToInt32(myLineSymbolSizeTxtBox.Text),
                Color = resultColor
            };
            this.DialogResult = DialogResult.OK;
        }

        // 取消-按钮
        private void myLineSymbolCancelBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}

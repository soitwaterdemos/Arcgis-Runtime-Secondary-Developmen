namespace ArcGISApp1
{
    partial class PointSymbolForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.myPointComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.myPointGroupBoxPicture = new System.Windows.Forms.GroupBox();
            this.inputPictureAsPointSymbol = new System.Windows.Forms.Button();
            this.myPointPictureBox = new System.Windows.Forms.PictureBox();
            this.myPointGroupBoxVector = new System.Windows.Forms.GroupBox();
            this.myPointSymbolStyle = new System.Windows.Forms.ComboBox();
            this.myPointSymbolColorChoose = new System.Windows.Forms.PictureBox();
            this.myPointSymbolSize = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.myPointSymbolSure = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.myPointGroupBoxPicture.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.myPointPictureBox)).BeginInit();
            this.myPointGroupBoxVector.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.myPointSymbolColorChoose)).BeginInit();
            this.SuspendLayout();
            // 
            // myPointComboBox
            // 
            this.myPointComboBox.FormattingEnabled = true;
            this.myPointComboBox.Items.AddRange(new object[] {
            "矢量",
            "图片"});
            this.myPointComboBox.Location = new System.Drawing.Point(76, 13);
            this.myPointComboBox.Name = "myPointComboBox";
            this.myPointComboBox.Size = new System.Drawing.Size(246, 23);
            this.myPointComboBox.TabIndex = 0;
            this.myPointComboBox.SelectedIndexChanged += new System.EventHandler(this.myPointComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "符号类型";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.myPointGroupBoxPicture);
            this.panel1.Controls.Add(this.myPointGroupBoxVector);
            this.panel1.Location = new System.Drawing.Point(12, 51);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(310, 210);
            this.panel1.TabIndex = 2;
            // 
            // myPointGroupBoxPicture
            // 
            this.myPointGroupBoxPicture.Controls.Add(this.inputPictureAsPointSymbol);
            this.myPointGroupBoxPicture.Controls.Add(this.myPointPictureBox);
            this.myPointGroupBoxPicture.Enabled = false;
            this.myPointGroupBoxPicture.Location = new System.Drawing.Point(170, 15);
            this.myPointGroupBoxPicture.Name = "myPointGroupBoxPicture";
            this.myPointGroupBoxPicture.Size = new System.Drawing.Size(121, 192);
            this.myPointGroupBoxPicture.TabIndex = 5;
            this.myPointGroupBoxPicture.TabStop = false;
            this.myPointGroupBoxPicture.Text = "图片";
            // 
            // inputPictureAsPointSymbol
            // 
            this.inputPictureAsPointSymbol.Location = new System.Drawing.Point(21, 145);
            this.inputPictureAsPointSymbol.Name = "inputPictureAsPointSymbol";
            this.inputPictureAsPointSymbol.Size = new System.Drawing.Size(75, 23);
            this.inputPictureAsPointSymbol.TabIndex = 1;
            this.inputPictureAsPointSymbol.Text = "导入图片";
            this.inputPictureAsPointSymbol.UseVisualStyleBackColor = true;
            this.inputPictureAsPointSymbol.Click += new System.EventHandler(this.inputPictureAsPointSymbol_Click);
            // 
            // myPointPictureBox
            // 
            this.myPointPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.myPointPictureBox.Location = new System.Drawing.Point(6, 24);
            this.myPointPictureBox.Name = "myPointPictureBox";
            this.myPointPictureBox.Size = new System.Drawing.Size(109, 98);
            this.myPointPictureBox.TabIndex = 0;
            this.myPointPictureBox.TabStop = false;
            // 
            // myPointGroupBoxVector
            // 
            this.myPointGroupBoxVector.Controls.Add(this.myPointSymbolStyle);
            this.myPointGroupBoxVector.Controls.Add(this.myPointSymbolColorChoose);
            this.myPointGroupBoxVector.Controls.Add(this.myPointSymbolSize);
            this.myPointGroupBoxVector.Controls.Add(this.label3);
            this.myPointGroupBoxVector.Controls.Add(this.label4);
            this.myPointGroupBoxVector.Controls.Add(this.label2);
            this.myPointGroupBoxVector.Location = new System.Drawing.Point(14, 15);
            this.myPointGroupBoxVector.Name = "myPointGroupBoxVector";
            this.myPointGroupBoxVector.Size = new System.Drawing.Size(121, 192);
            this.myPointGroupBoxVector.TabIndex = 4;
            this.myPointGroupBoxVector.TabStop = false;
            this.myPointGroupBoxVector.Text = "矢量";
            // 
            // myPointSymbolStyle
            // 
            this.myPointSymbolStyle.FormattingEnabled = true;
            this.myPointSymbolStyle.Items.AddRange(new object[] {
            "Circle",
            "Cross",
            "Diamond",
            "Square",
            "Triangle",
            "X"});
            this.myPointSymbolStyle.Location = new System.Drawing.Point(12, 154);
            this.myPointSymbolStyle.Name = "myPointSymbolStyle";
            this.myPointSymbolStyle.Size = new System.Drawing.Size(100, 23);
            this.myPointSymbolStyle.TabIndex = 7;
            this.myPointSymbolStyle.SelectedIndexChanged += new System.EventHandler(this.myPointSymbolStyle_SelectedIndexChanged);
            // 
            // myPointSymbolColorChoose
            // 
            this.myPointSymbolColorChoose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.myPointSymbolColorChoose.Location = new System.Drawing.Point(12, 92);
            this.myPointSymbolColorChoose.Name = "myPointSymbolColorChoose";
            this.myPointSymbolColorChoose.Size = new System.Drawing.Size(100, 25);
            this.myPointSymbolColorChoose.TabIndex = 6;
            this.myPointSymbolColorChoose.TabStop = false;
            this.myPointSymbolColorChoose.Click += new System.EventHandler(this.myPointSymbolColorChoose_Click);
            // 
            // myPointSymbolSize
            // 
            this.myPointSymbolSize.Location = new System.Drawing.Point(9, 40);
            this.myPointSymbolSize.Name = "myPointSymbolSize";
            this.myPointSymbolSize.Size = new System.Drawing.Size(100, 25);
            this.myPointSymbolSize.TabIndex = 5;
            this.myPointSymbolSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 135);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "样式";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 15);
            this.label4.TabIndex = 3;
            this.label4.Text = "颜色";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "大小";
            // 
            // myPointSymbolSure
            // 
            this.myPointSymbolSure.Location = new System.Drawing.Point(52, 285);
            this.myPointSymbolSure.Name = "myPointSymbolSure";
            this.myPointSymbolSure.Size = new System.Drawing.Size(75, 23);
            this.myPointSymbolSure.TabIndex = 3;
            this.myPointSymbolSure.Text = "确定";
            this.myPointSymbolSure.UseVisualStyleBackColor = true;
            this.myPointSymbolSure.Click += new System.EventHandler(this.myPointSymbolSure_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(211, 285);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "取消";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // PointSymbolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(332, 313);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.myPointSymbolSure);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.myPointComboBox);
            this.Name = "PointSymbolForm";
            this.Text = "设置点符号样式";
            this.Load += new System.EventHandler(this.PointSymbolForm_Load);
            this.panel1.ResumeLayout(false);
            this.myPointGroupBoxPicture.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.myPointPictureBox)).EndInit();
            this.myPointGroupBoxVector.ResumeLayout(false);
            this.myPointGroupBoxVector.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.myPointSymbolColorChoose)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox myPointComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox myPointPictureBox;
        private System.Windows.Forms.Button myPointSymbolSure;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox myPointGroupBoxVector;
        private System.Windows.Forms.GroupBox myPointGroupBoxPicture;
        private System.Windows.Forms.Button inputPictureAsPointSymbol;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox myPointSymbolSize;
        private System.Windows.Forms.PictureBox myPointSymbolColorChoose;
        private System.Windows.Forms.ComboBox myPointSymbolStyle;
    }
}
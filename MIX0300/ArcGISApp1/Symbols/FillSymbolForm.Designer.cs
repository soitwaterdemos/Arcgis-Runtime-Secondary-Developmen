namespace ArcGISApp1
{
    partial class FillSymbolForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.myFillSymbolComboBox = new System.Windows.Forms.ComboBox();
            this.myFillSymGroupBoxVector = new System.Windows.Forms.GroupBox();
            this.myFillSymGroupBoxPicture = new System.Windows.Forms.GroupBox();
            this.myFillSymSureBtn = new System.Windows.Forms.Button();
            this.myFillSymCancelBtn = new System.Windows.Forms.Button();
            this.myPictureBox = new System.Windows.Forms.PictureBox();
            this.myInputImage = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.myVectorStyle = new System.Windows.Forms.ComboBox();
            this.myVectorColor = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.myLineStyle = new System.Windows.Forms.ComboBox();
            this.myLineColor = new System.Windows.Forms.PictureBox();
            this.myLineSize = new System.Windows.Forms.TextBox();
            this.myFillSymGroupBoxVector.SuspendLayout();
            this.myFillSymGroupBoxPicture.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.myPictureBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.myVectorColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.myLineColor)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "符号类型";
            // 
            // myFillSymbolComboBox
            // 
            this.myFillSymbolComboBox.FormattingEnabled = true;
            this.myFillSymbolComboBox.Items.AddRange(new object[] {
            "矢量",
            "图片"});
            this.myFillSymbolComboBox.Location = new System.Drawing.Point(86, 13);
            this.myFillSymbolComboBox.Name = "myFillSymbolComboBox";
            this.myFillSymbolComboBox.Size = new System.Drawing.Size(234, 23);
            this.myFillSymbolComboBox.TabIndex = 1;
            this.myFillSymbolComboBox.SelectedIndexChanged += new System.EventHandler(this.myFillSymbolComboBox_SelectedIndexChanged);
            // 
            // myFillSymGroupBoxVector
            // 
            this.myFillSymGroupBoxVector.Controls.Add(this.myVectorColor);
            this.myFillSymGroupBoxVector.Controls.Add(this.myVectorStyle);
            this.myFillSymGroupBoxVector.Controls.Add(this.label4);
            this.myFillSymGroupBoxVector.Controls.Add(this.label3);
            this.myFillSymGroupBoxVector.Location = new System.Drawing.Point(12, 48);
            this.myFillSymGroupBoxVector.Name = "myFillSymGroupBoxVector";
            this.myFillSymGroupBoxVector.Size = new System.Drawing.Size(121, 192);
            this.myFillSymGroupBoxVector.TabIndex = 2;
            this.myFillSymGroupBoxVector.TabStop = false;
            this.myFillSymGroupBoxVector.Text = "矢量";
            // 
            // myFillSymGroupBoxPicture
            // 
            this.myFillSymGroupBoxPicture.Controls.Add(this.myInputImage);
            this.myFillSymGroupBoxPicture.Controls.Add(this.myPictureBox);
            this.myFillSymGroupBoxPicture.Enabled = false;
            this.myFillSymGroupBoxPicture.Location = new System.Drawing.Point(181, 48);
            this.myFillSymGroupBoxPicture.Name = "myFillSymGroupBoxPicture";
            this.myFillSymGroupBoxPicture.Size = new System.Drawing.Size(121, 192);
            this.myFillSymGroupBoxPicture.TabIndex = 3;
            this.myFillSymGroupBoxPicture.TabStop = false;
            this.myFillSymGroupBoxPicture.Text = "图片";
            // 
            // myFillSymSureBtn
            // 
            this.myFillSymSureBtn.Location = new System.Drawing.Point(62, 378);
            this.myFillSymSureBtn.Name = "myFillSymSureBtn";
            this.myFillSymSureBtn.Size = new System.Drawing.Size(75, 23);
            this.myFillSymSureBtn.TabIndex = 4;
            this.myFillSymSureBtn.Text = "确认";
            this.myFillSymSureBtn.UseVisualStyleBackColor = true;
            this.myFillSymSureBtn.Click += new System.EventHandler(this.myFillSymSureBtn_Click);
            // 
            // myFillSymCancelBtn
            // 
            this.myFillSymCancelBtn.Location = new System.Drawing.Point(201, 378);
            this.myFillSymCancelBtn.Name = "myFillSymCancelBtn";
            this.myFillSymCancelBtn.Size = new System.Drawing.Size(75, 23);
            this.myFillSymCancelBtn.TabIndex = 5;
            this.myFillSymCancelBtn.Text = "取消";
            this.myFillSymCancelBtn.UseVisualStyleBackColor = true;
            this.myFillSymCancelBtn.Click += new System.EventHandler(this.myFillSymCancelBtn_Click);
            // 
            // myPictureBox
            // 
            this.myPictureBox.Location = new System.Drawing.Point(7, 25);
            this.myPictureBox.Name = "myPictureBox";
            this.myPictureBox.Size = new System.Drawing.Size(109, 98);
            this.myPictureBox.TabIndex = 0;
            this.myPictureBox.TabStop = false;
            // 
            // myInputImage
            // 
            this.myInputImage.Location = new System.Drawing.Point(20, 150);
            this.myInputImage.Name = "myInputImage";
            this.myInputImage.Size = new System.Drawing.Size(75, 23);
            this.myInputImage.TabIndex = 1;
            this.myInputImage.Text = "导入图片";
            this.myInputImage.UseVisualStyleBackColor = true;
            this.myInputImage.Click += new System.EventHandler(this.myInputImage_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.myLineSize);
            this.groupBox1.Controls.Add(this.myLineColor);
            this.groupBox1.Controls.Add(this.myLineStyle);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(16, 258);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(286, 100);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "边框";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 15);
            this.label3.TabIndex = 1;
            this.label3.Text = "颜色";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 117);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 15);
            this.label4.TabIndex = 2;
            this.label4.Text = "样式";
            // 
            // myVectorStyle
            // 
            this.myVectorStyle.FormattingEnabled = true;
            this.myVectorStyle.Items.AddRange(new object[] {
            "Solid",
            "Horizontal",
            "Vertical",
            "Cross",
            "DiagonalCross",
            "BackwardDiagonal",
            "ForwardDiagonal"});
            this.myVectorStyle.Location = new System.Drawing.Point(10, 151);
            this.myVectorStyle.Name = "myVectorStyle";
            this.myVectorStyle.Size = new System.Drawing.Size(95, 23);
            this.myVectorStyle.TabIndex = 3;
            this.myVectorStyle.SelectedIndexChanged += new System.EventHandler(this.myVectorStyle_SelectedIndexChanged);
            // 
            // myVectorColor
            // 
            this.myVectorColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.myVectorColor.Location = new System.Drawing.Point(10, 63);
            this.myVectorColor.Name = "myVectorColor";
            this.myVectorColor.Size = new System.Drawing.Size(95, 23);
            this.myVectorColor.TabIndex = 4;
            this.myVectorColor.TabStop = false;
            this.myVectorColor.Click += new System.EventHandler(this.myVectorColor_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "颜色";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 15);
            this.label5.TabIndex = 1;
            this.label5.Text = "宽度";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(169, 34);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 15);
            this.label6.TabIndex = 2;
            this.label6.Text = "线型";
            // 
            // myLineStyle
            // 
            this.myLineStyle.FormattingEnabled = true;
            this.myLineStyle.Location = new System.Drawing.Point(165, 67);
            this.myLineStyle.Name = "myLineStyle";
            this.myLineStyle.Size = new System.Drawing.Size(95, 23);
            this.myLineStyle.TabIndex = 3;
            this.myLineStyle.SelectedIndexChanged += new System.EventHandler(this.myLineStyle_SelectedIndexChanged);
            // 
            // myLineColor
            // 
            this.myLineColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.myLineColor.Location = new System.Drawing.Point(52, 34);
            this.myLineColor.Name = "myLineColor";
            this.myLineColor.Size = new System.Drawing.Size(49, 23);
            this.myLineColor.TabIndex = 4;
            this.myLineColor.TabStop = false;
            this.myLineColor.Click += new System.EventHandler(this.myLineColor_Click);
            // 
            // myLineSize
            // 
            this.myLineSize.Location = new System.Drawing.Point(52, 63);
            this.myLineSize.Name = "myLineSize";
            this.myLineSize.Size = new System.Drawing.Size(49, 25);
            this.myLineSize.TabIndex = 5;
            this.myLineSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.myLineSize_KeyPress);
            // 
            // FillSymbolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(332, 413);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.myFillSymCancelBtn);
            this.Controls.Add(this.myFillSymSureBtn);
            this.Controls.Add(this.myFillSymGroupBoxPicture);
            this.Controls.Add(this.myFillSymGroupBoxVector);
            this.Controls.Add(this.myFillSymbolComboBox);
            this.Controls.Add(this.label1);
            this.Name = "FillSymbolForm";
            this.Text = "设置面符号样式";
            this.Load += new System.EventHandler(this.FillSymbolForm_Load);
            this.myFillSymGroupBoxVector.ResumeLayout(false);
            this.myFillSymGroupBoxVector.PerformLayout();
            this.myFillSymGroupBoxPicture.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.myPictureBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.myVectorColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.myLineColor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox myFillSymbolComboBox;
        private System.Windows.Forms.GroupBox myFillSymGroupBoxVector;
        private System.Windows.Forms.GroupBox myFillSymGroupBoxPicture;
        private System.Windows.Forms.Button myInputImage;
        private System.Windows.Forms.PictureBox myPictureBox;
        private System.Windows.Forms.Button myFillSymSureBtn;
        private System.Windows.Forms.Button myFillSymCancelBtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox myVectorColor;
        private System.Windows.Forms.ComboBox myVectorStyle;
        private System.Windows.Forms.TextBox myLineSize;
        private System.Windows.Forms.PictureBox myLineColor;
        private System.Windows.Forms.ComboBox myLineStyle;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
    }
}
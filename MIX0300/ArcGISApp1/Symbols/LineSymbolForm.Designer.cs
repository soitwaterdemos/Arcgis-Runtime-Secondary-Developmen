namespace ArcGISApp1
{
    partial class LineSymbolForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.myLineSymbolColorBox = new System.Windows.Forms.PictureBox();
            this.myLineSymbolSizeTxtBox = new System.Windows.Forms.TextBox();
            this.myLineSymbolStyleComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.myLineSymbolSureBtn = new System.Windows.Forms.Button();
            this.myLineSymbolCancelBtn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.myLineSymbolColorBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "线型";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.myLineSymbolColorBox);
            this.panel1.Controls.Add(this.myLineSymbolSizeTxtBox);
            this.panel1.Controls.Add(this.myLineSymbolStyleComboBox);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(25, 30);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(281, 226);
            this.panel1.TabIndex = 1;
            // 
            // myLineSymbolColorBox
            // 
            this.myLineSymbolColorBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.myLineSymbolColorBox.Location = new System.Drawing.Point(102, 165);
            this.myLineSymbolColorBox.Name = "myLineSymbolColorBox";
            this.myLineSymbolColorBox.Size = new System.Drawing.Size(157, 25);
            this.myLineSymbolColorBox.TabIndex = 5;
            this.myLineSymbolColorBox.TabStop = false;
            this.myLineSymbolColorBox.Click += new System.EventHandler(this.myLineSymbolColorBox_Click);
            // 
            // myLineSymbolSizeTxtBox
            // 
            this.myLineSymbolSizeTxtBox.Location = new System.Drawing.Point(102, 99);
            this.myLineSymbolSizeTxtBox.Name = "myLineSymbolSizeTxtBox";
            this.myLineSymbolSizeTxtBox.Size = new System.Drawing.Size(157, 25);
            this.myLineSymbolSizeTxtBox.TabIndex = 4;
            this.myLineSymbolSizeTxtBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.myLineSymbolSizeTxtBox_KeyPress);
            // 
            // myLineSymbolStyleComboBox
            // 
            this.myLineSymbolStyleComboBox.FormattingEnabled = true;
            this.myLineSymbolStyleComboBox.Location = new System.Drawing.Point(102, 36);
            this.myLineSymbolStyleComboBox.Name = "myLineSymbolStyleComboBox";
            this.myLineSymbolStyleComboBox.Size = new System.Drawing.Size(157, 23);
            this.myLineSymbolStyleComboBox.TabIndex = 3;
            this.myLineSymbolStyleComboBox.SelectedIndexChanged += new System.EventHandler(this.myLineSymbolStyleComboBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 165);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "颜色";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "宽度";
            // 
            // myLineSymbolSureBtn
            // 
            this.myLineSymbolSureBtn.Location = new System.Drawing.Point(46, 278);
            this.myLineSymbolSureBtn.Name = "myLineSymbolSureBtn";
            this.myLineSymbolSureBtn.Size = new System.Drawing.Size(75, 23);
            this.myLineSymbolSureBtn.TabIndex = 2;
            this.myLineSymbolSureBtn.Text = "确认";
            this.myLineSymbolSureBtn.UseVisualStyleBackColor = true;
            this.myLineSymbolSureBtn.Click += new System.EventHandler(this.myLineSymbolSureBtn_Click);
            // 
            // myLineSymbolCancelBtn
            // 
            this.myLineSymbolCancelBtn.Location = new System.Drawing.Point(209, 278);
            this.myLineSymbolCancelBtn.Name = "myLineSymbolCancelBtn";
            this.myLineSymbolCancelBtn.Size = new System.Drawing.Size(75, 23);
            this.myLineSymbolCancelBtn.TabIndex = 3;
            this.myLineSymbolCancelBtn.Text = "取消";
            this.myLineSymbolCancelBtn.UseVisualStyleBackColor = true;
            this.myLineSymbolCancelBtn.Click += new System.EventHandler(this.myLineSymbolCancelBtn_Click);
            // 
            // LineSymbolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(332, 313);
            this.Controls.Add(this.myLineSymbolCancelBtn);
            this.Controls.Add(this.myLineSymbolSureBtn);
            this.Controls.Add(this.panel1);
            this.Name = "LineSymbolForm";
            this.Text = "设置线符号样式";
            this.Load += new System.EventHandler(this.LineSymbolForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.myLineSymbolColorBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox myLineSymbolSizeTxtBox;
        private System.Windows.Forms.ComboBox myLineSymbolStyleComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox myLineSymbolColorBox;
        private System.Windows.Forms.Button myLineSymbolSureBtn;
        private System.Windows.Forms.Button myLineSymbolCancelBtn;
    }
}
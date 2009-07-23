namespace WITSniff
{
    partial class frmConfig
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
            this.txtDelay = new System.Windows.Forms.TextBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.chkDelayEnable = new System.Windows.Forms.CheckBox();
            this.grpDelay = new System.Windows.Forms.GroupBox();
            this.chkRequireGPS = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numMax = new System.Windows.Forms.NumericUpDown();
            this.chkLogtoFile = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtURL = new System.Windows.Forms.TextBox();
            this.chkLogtoURL = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.txtBaudRate = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.drpPort = new System.Windows.Forms.ComboBox();
            this.txtLogDir = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.grpDelay.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMax)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(6, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Scan Delay";
            // 
            // txtDelay
            // 
            this.txtDelay.Enabled = false;
            this.txtDelay.Location = new System.Drawing.Point(74, 40);
            this.txtDelay.Name = "txtDelay";
            this.txtDelay.Size = new System.Drawing.Size(100, 20);
            this.txtDelay.TabIndex = 1;
            // 
            // btnApply
            // 
            this.btnApply.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnApply.Location = new System.Drawing.Point(0, 382);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(263, 39);
            this.btnApply.TabIndex = 2;
            this.btnApply.Text = "Save";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // chkDelayEnable
            // 
            this.chkDelayEnable.AutoSize = true;
            this.chkDelayEnable.Location = new System.Drawing.Point(6, 19);
            this.chkDelayEnable.Name = "chkDelayEnable";
            this.chkDelayEnable.Size = new System.Drawing.Size(59, 17);
            this.chkDelayEnable.TabIndex = 3;
            this.chkDelayEnable.Text = "Enable";
            this.chkDelayEnable.UseVisualStyleBackColor = true;
            this.chkDelayEnable.CheckedChanged += new System.EventHandler(this.chkDelayEnable_CheckedChanged);
            // 
            // grpDelay
            // 
            this.grpDelay.Controls.Add(this.chkDelayEnable);
            this.grpDelay.Controls.Add(this.label1);
            this.grpDelay.Controls.Add(this.txtDelay);
            this.grpDelay.Location = new System.Drawing.Point(8, 6);
            this.grpDelay.Name = "grpDelay";
            this.grpDelay.Size = new System.Drawing.Size(200, 77);
            this.grpDelay.TabIndex = 4;
            this.grpDelay.TabStop = false;
            this.grpDelay.Text = "Delay";
            // 
            // chkRequireGPS
            // 
            this.chkRequireGPS.AutoSize = true;
            this.chkRequireGPS.Location = new System.Drawing.Point(17, 275);
            this.chkRequireGPS.Name = "chkRequireGPS";
            this.chkRequireGPS.Size = new System.Drawing.Size(119, 17);
            this.chkRequireGPS.TabIndex = 5;
            this.chkRequireGPS.Text = "Require GPS Coord";
            this.chkRequireGPS.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtLogDir);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numMax);
            this.groupBox1.Controls.Add(this.chkLogtoFile);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtURL);
            this.groupBox1.Controls.Add(this.chkLogtoURL);
            this.groupBox1.Location = new System.Drawing.Point(8, 89);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 180);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Logging";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(146, 108);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "kB";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Max Size:";
            // 
            // numMax
            // 
            this.numMax.Location = new System.Drawing.Point(71, 106);
            this.numMax.Name = "numMax";
            this.numMax.Size = new System.Drawing.Size(69, 20);
            this.numMax.TabIndex = 8;
            // 
            // chkLogtoFile
            // 
            this.chkLogtoFile.AutoSize = true;
            this.chkLogtoFile.Location = new System.Drawing.Point(9, 88);
            this.chkLogtoFile.Name = "chkLogtoFile";
            this.chkLogtoFile.Size = new System.Drawing.Size(72, 17);
            this.chkLogtoFile.TabIndex = 9;
            this.chkLogtoFile.Text = "Log to file";
            this.chkLogtoFile.UseVisualStyleBackColor = true;
            this.chkLogtoFile.CheckedChanged += new System.EventHandler(this.chkLogtoFile_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Web Address:";
            // 
            // txtURL
            // 
            this.txtURL.Location = new System.Drawing.Point(9, 59);
            this.txtURL.Name = "txtURL";
            this.txtURL.Size = new System.Drawing.Size(165, 20);
            this.txtURL.TabIndex = 1;
            // 
            // chkLogtoURL
            // 
            this.chkLogtoURL.AutoSize = true;
            this.chkLogtoURL.Location = new System.Drawing.Point(9, 19);
            this.chkLogtoURL.Name = "chkLogtoURL";
            this.chkLogtoURL.Size = new System.Drawing.Size(93, 17);
            this.chkLogtoURL.TabIndex = 0;
            this.chkLogtoURL.Text = "Upload Online";
            this.chkLogtoURL.UseVisualStyleBackColor = true;
            this.chkLogtoURL.CheckedChanged += new System.EventHandler(this.chkLogtoURL_CheckedChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(263, 382);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.checkBox1);
            this.tabPage1.Controls.Add(this.grpDelay);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.chkRequireGPS);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(255, 356);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(17, 298);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(105, 17);
            this.checkBox1.TabIndex = 8;
            this.checkBox1.Text = "Advanced Mode";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.txtBaudRate);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.drpPort);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(223, 280);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "GPS Settings";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // txtBaudRate
            // 
            this.txtBaudRate.Location = new System.Drawing.Point(75, 31);
            this.txtBaudRate.Name = "txtBaudRate";
            this.txtBaudRate.Size = new System.Drawing.Size(100, 20);
            this.txtBaudRate.TabIndex = 3;
            this.txtBaudRate.Text = "4800";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 34);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Baud Rate:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 60);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Port:";
            // 
            // drpPort
            // 
            this.drpPort.FormattingEnabled = true;
            this.drpPort.Location = new System.Drawing.Point(43, 57);
            this.drpPort.Name = "drpPort";
            this.drpPort.Size = new System.Drawing.Size(121, 21);
            this.drpPort.TabIndex = 0;
            // 
            // txtLogDir
            // 
            this.txtLogDir.Location = new System.Drawing.Point(9, 154);
            this.txtLogDir.Name = "txtLogDir";
            this.txtLogDir.Size = new System.Drawing.Size(165, 20);
            this.txtLogDir.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 138);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(84, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Log file location:";
            // 
            // frmConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(263, 421);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnApply);
            this.Name = "frmConfig";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuration";
            this.Load += new System.EventHandler(this.frmConfig_Load);
            this.grpDelay.ResumeLayout(false);
            this.grpDelay.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMax)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDelay;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.CheckBox chkDelayEnable;
        private System.Windows.Forms.GroupBox grpDelay;
        private System.Windows.Forms.CheckBox chkRequireGPS;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numMax;
        private System.Windows.Forms.CheckBox chkLogtoFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtURL;
        private System.Windows.Forms.CheckBox chkLogtoURL;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txtBaudRate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ComboBox drpPort;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtLogDir;
    }
}
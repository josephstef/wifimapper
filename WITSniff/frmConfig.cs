using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO.Ports;

namespace WITSniff
{
    public partial class frmConfig : Form
    {
        public frmConfig()
        {
            InitializeComponent();
            for (int i = 0; i < 17; i++)
            {
                try
                {
                    SerialPort testCOM = new SerialPort();
                    testCOM.PortName = "COM" + i.ToString();
                    testCOM.Open();
                    if (testCOM.IsOpen)
                    {
                        drpPort.Items.Add("COM" + i.ToString());
                    }
                    testCOM.Close();
                    testCOM.Dispose();
                }
                catch (Exception) { }
            }
            //Load values
            txtDelay.Text = Properties.Settings.Default.scanDelay.ToString();
            chkRequireGPS.Checked = Properties.Settings.Default.requireGPS ? true : false;
            chkDelayEnable.Checked = (Properties.Settings.Default.enableDelay == true ? true : false);
            chkLogtoFile.Checked = (Properties.Settings.Default.logtoFile == true ? true : false);
            chkLogtoURL.Checked = (Properties.Settings.Default.logtoURL == true ? true : false);
            checkBox1.Checked = Properties.Settings.Default.modeAdvanced == true ? true : false;
            numMax.Value = Properties.Settings.Default.maxlogsize;
            txtURL.Text = Properties.Settings.Default.webURL;
            txtBaudRate.Text = Properties.Settings.Default.baudRate.ToString();
            drpPort.SelectedItem = Properties.Settings.Default.comPort.ToString();
            txtLogDir.Text = Properties.Settings.Default.logFileLocation;
        }
        private void disableEnable()
        {
            //Enable or disable controls
            txtDelay.Enabled = chkDelayEnable.Checked ? true : false;
            numMax.Enabled = chkLogtoFile.Checked ? true : false;
            txtURL.Enabled = chkLogtoURL.Checked ? true : false;
            label1.Enabled = chkDelayEnable.Checked ? true : false;
        }

        #region Event Handlers
        private void btnApply_Click(object sender, EventArgs e)
        {
            if (txtDelay.Text != string.Empty)
            {
                if (chkDelayEnable.Checked)
                    Properties.Settings.Default.scanDelay = uint.Parse(txtDelay.Text);

                Properties.Settings.Default.logtoFile = chkLogtoFile.Checked ? true : false;
                Properties.Settings.Default.maxlogsize = Convert.ToUInt32(numMax.Value);
                Properties.Settings.Default.logtoURL = chkLogtoURL.Checked ? true : false;
                Properties.Settings.Default.webURL = txtURL.Text;
                Properties.Settings.Default.logFileLocation = txtLogDir.Text;
                Properties.Settings.Default.comPort = drpPort.SelectedItem.ToString();
                Properties.Settings.Default.baudRate = uint.Parse(txtBaudRate.Text);
                Properties.Settings.Default.enableDelay = chkDelayEnable.Checked ? true : false;
                Properties.Settings.Default.requireGPS = chkRequireGPS.Checked ? true : false;
                Properties.Settings.Default.modeAdvanced = checkBox1.Checked ? true : false;
                Properties.Settings.Default.Save();
            }

            this.Close();
        }
        private void chkDelayEnable_CheckedChanged(object sender, EventArgs e)
        {
            disableEnable();
        }
        private void chkLogtoURL_CheckedChanged(object sender, EventArgs e)
        {
            disableEnable();
        }
        private void chkLogtoFile_CheckedChanged(object sender, EventArgs e)
        {
            disableEnable();
        }
        #endregion

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            disableEnable();
        }

        private void frmConfig_Load(object sender, EventArgs e)
        {

        }
    }
}

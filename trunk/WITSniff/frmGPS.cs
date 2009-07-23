using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpGis.SharpGps;

namespace WITSniff
{
    public partial class frmGPS : Form
    {
        public const string ActiveSerialPort = "COM4";
        public const int SerialPortSped = 4800;

        public frmGPS()
        {
            InitializeComponent();
            GPS = new GPSHandler(this);
            GPS.TimeOut = 5;
          /*  GPS.NewGPSFix += new GPSHandler.NewGPSFixHandler(this.GPSEventHandler);*/
        }

        public static GPSHandler GPS;
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GPS.IsPortOpen)
            {
                try
                {
                    //GPS.EnableEmulate(@"..\Bristol_Nottingham.txt"); //Uncomment this and change filepath to valid NMEA log file for emulating GPS data

                    GPS.Start(ActiveSerialPort, SerialPortSped); //Open serial port
                    startToolStripMenuItem.Text = "Stop";
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("An error occured when trying to open port: " + ex.Message);
                }
            }
            else
            {
                GPS.Stop(); //Close serial port
                startToolStripMenuItem.Text = "Start";
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Net;
using System.Media;
using System.Text.RegularExpressions;
using SpeechLib;
using MySql.Data.MySqlClient;
using SharpGis.SharpGps;
using NativeWifi;

namespace WITSniff
{
    public partial class Form1 : Form
    {
        System.Windows.Forms.Timer Clock;
        delegate void updateMessageLog(string data);

        #region Variables
        bool hasFix = false, wifiFound = false, scanStarted = false, GPSAlerted = false;
        int NetworkIndex = -1, LogNetworkIndex = -1;
        string[,] Networks = new string[10000, 9];
        string[,] logNetworks = new string[10000, 15];
        public static GPSHandler GPS;
        enum Message { gpsScanStart = 0, gpsScanStarted, gpsError, gpsNoFix, gpsStopping, gpsStopped, logScanning, logDuplicate, logResult, logLogging, logPushing, };
        #endregion

        #region Event Handlers
        public Form1()
        {
            InitializeComponent();
            GPS = new GPSHandler(this);
            GPS.TimeOut = 5;
            GPS.NewGPSFix += new GPSHandler.NewGPSFixHandler(this.GPSEventHandler);
            refreshInformationPanel();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Hide();
            bool done = false;
            ThreadPool.QueueUserWorkItem((x) =>
            {
                using (var splashForm = new frmSplash())
                {
                    splashForm.Show();
                    while (!done)
                        Application.DoEvents();
                    splashForm.Close();
                }
            });

            Thread.Sleep(2000); // Emulate hardwork
            done = true;
            Show();
        }
        private void refreshInformationPanel()
        {
            lblInfoGPSReq.Text = Properties.Settings.Default.requireGPS ? "Yes" : "No";
            lblInfoLogFile.Text = Properties.Settings.Default.logtoFile ? "Yes" : "No";
            lblInfoLogSite.Text = Properties.Settings.Default.requireGPS ? "Yes" : "No";
            if (Properties.Settings.Default.enableDelay)
            {
                lblInfoDelay.Text = "Enabled";
                lblInfoSpeed.Text = (Properties.Settings.Default.scanDelay / 1000).ToString() + " seconds";
            }
        }
        private void updateLog(int msg, string info)
        {
            switch (msg)
            {
                case (int)Message.gpsScanStart:
                    txtLog.Text += DateTime.Now + " - GPS: Starting...\r\n";
                    break;
                case (int)Message.gpsScanStarted:
                    txtLog.Text += DateTime.Now + " - GPS: Started\r\n";
                    break;
                case (int)Message.gpsError:
                    txtLog.Text += DateTime.Now + " - GPS: ERROR\r\n";
                    break;
                case (int)Message.gpsNoFix:
                    txtLog.Text += DateTime.Now + " - GPS: No Fix\r\n";
                    break;
                case (int)Message.gpsStopping:
                    txtLog.Text += DateTime.Now + " - GPS: Stopping\r\n";
                    break;
                case (int)Message.gpsStopped:
                    txtLog.Text = DateTime.Now + " - GPS: Stopped\r\n";
                    break;
                case (int)Message.logScanning:
                    txtLog.Text += DateTime.Now + " - Scanner: Scanning...\r\n";
                    break;
                case (int)Message.logDuplicate:
                    txtLog.Text += DateTime.Now + " - Scanner: Duplicate found, skipping [ " + info + " ]\r\n";
                    break;
                case (int)Message.logResult:
                    txtLog.Text += DateTime.Now + " - Scanner: Found Hotspot [ " + info + " ]\r\n";
                    break;
                case (int)Message.logLogging:
                    txtLog.Text += DateTime.Now + " - Scanner: Logging Hotspot [ " + info + " ]...\r\n";
                    break;
                case (int)Message.logPushing:
                    txtLog.Text += DateTime.Now + " - Scanner: Pushing Hotspot [ " + info + " ]...\r\n";
                    break;
                default:
                    break;
            }
        } //Update log with operational data
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex != 0)
            {
                LogNetworkIndex = 0;
                StreamReader reader = new StreamReader(@"C:\wifi.log");
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("##"))
                        logNetworks[LogNetworkIndex, 0] = "####";
                    if (line.StartsWith("$lat"))
                        logNetworks[LogNetworkIndex, 1] = line.Replace("$lat - ", string.Empty);
                    if (line.StartsWith("$longitude"))
                        logNetworks[LogNetworkIndex, 2] = line.Replace("$longitude - ", string.Empty);
                    if (line.StartsWith("$mac"))
                        logNetworks[LogNetworkIndex, 3] = line.Replace("$mac - ", string.Empty);
                    if (line.StartsWith("$ssid"))
                        logNetworks[LogNetworkIndex, 4] = line.Replace("$ssid - ", string.Empty);
                    if (line.StartsWith("$channel"))
                        logNetworks[LogNetworkIndex, 5] = line.Replace("$channel - ", string.Empty);
                    if (line.StartsWith("$signal"))
                        logNetworks[LogNetworkIndex, 6] = line.Replace("$signal - ", string.Empty);
                    if (line.StartsWith("$auth"))
                        logNetworks[LogNetworkIndex, 7] = line.Replace("$auth - ", string.Empty);
                    if (line.StartsWith("$encryption"))
                        logNetworks[LogNetworkIndex, 8] = line.Replace("$encryption - ", string.Empty);
                    if (line.StartsWith("$radio"))
                        logNetworks[LogNetworkIndex, 9] = line.Replace("$radio - ", string.Empty);
                    if (line.StartsWith("$nettype"))
                        logNetworks[LogNetworkIndex, 10] = line.Replace("$nettype - ", string.Empty);
                    if (line.StartsWith("$speed"))
                        logNetworks[LogNetworkIndex, 11] = line.Replace("$speed - ", string.Empty);
                    if (line.StartsWith("$user"))
                        logNetworks[LogNetworkIndex, 12] = line.Replace("$user - ", string.Empty);
                    if (line.StartsWith("$entrydtm"))
                        logNetworks[LogNetworkIndex, 13] = line.Replace("$entrydtm - ", string.Empty);
                    if (line.StartsWith("$updatedtm"))
                    {
                        logNetworks[LogNetworkIndex, 14] = line.Replace("$updatedtm - ", string.Empty);
                        LogNetworkIndex++;
                    }
                }
                reader.Close();

                listView2.Items.Clear();

                for (int i = 0; i < LogNetworkIndex + 1; i++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        ListViewItem SearchItem = new ListViewItem();
                        if (logNetworks[i, 0] == " ") continue; // don't search if no valid MAC Address !
                        if (logNetworks[i, 0] == null) continue; // don't search if no valid MAC Address !
                        SearchItem = listView2.FindItemWithText(logNetworks[i, k]);
                        if (SearchItem == null)
                        {
                            // New discovery - add it to the list

                            SystemSounds.Question.Play();

                            listView2.Items.Add(logNetworks[i, 1]);                                          // MAC Address
                            listView2.Items[listView2.Items.Count - 1].SubItems.Add(logNetworks[i, 2]);      // SSID
                            listView2.Items[listView2.Items.Count - 1].SubItems.Add(logNetworks[i, 3]);      // Channel
                            listView2.Items[listView2.Items.Count - 1].SubItems.Add(logNetworks[i, 4]);      // Signal
                            listView2.Items[listView2.Items.Count - 1].SubItems.Add(logNetworks[i, 5]);      // Authenticatiopn
                            listView2.Items[listView2.Items.Count - 1].SubItems.Add(logNetworks[i, 6]);      // Encryption
                            listView2.Items[listView2.Items.Count - 1].SubItems.Add(logNetworks[i, 7]);      // Radio Type
                            listView2.Items[listView2.Items.Count - 1].SubItems.Add(logNetworks[i, 8]);      // Network Type
                            listView2.Items[listView2.Items.Count - 1].SubItems.Add(logNetworks[i, 9]);      // Speed

                            if ((logNetworks[i, 7] == "Open") & (logNetworks[i, 8] == "None")) listView2.Items[listView2.Items.Count - 1].BackColor = Color.PaleGreen;
                            listView2.Items[listView2.Items.Count - 1].EnsureVisible();
                            if ((logNetworks[i, 7] == "Open") & (logNetworks[i, 8] != "None")) listView2.Items[listView2.Items.Count - 1].BackColor = Color.Pink;
                            listView2.Items[listView2.Items.Count - 1].EnsureVisible();

                            wifiFound = true;
                        }
                    }
                }
            }
        }
        private void Timer_Tick(object sender, EventArgs eArgs)
        {
            takeSnapshot();
        }
        private void txtLog_TextChanged(object sender, EventArgs e)
        {
            txtLog.Focus();
            txtLog.Select(txtLog.Text.Length, 0);
            txtLog.ScrollToCaret();
        }
        private void txtRaw_TextChanged(object sender, EventArgs e)
        {
            txtRaw.Focus();
            txtRaw.Select(txtRaw.Text.Length, 0);
            txtRaw.ScrollToCaret();
        }
        private void GPSEventHandler(object sender, GPSHandler.GPSEventArgs e)
        {
            txtRaw.Text += e.Sentence + "\r\n"; //Add the incoming data
            if (txtRaw.Text.Length > 20 * 1024 * 1024) //20Kb maximum
            {
                txtRaw.Text = txtRaw.Text.Substring(10 * 1024 * 1024);
            }

            if (GPS.HasGPSFix)
            {
                if (!GPSAlerted)
                {
                    //SoundPlayer player = ne%w SoundPlayer(@"C:\Windows\Media\chimes.wav");   //Play sound to indicate that hotspots have been located
                    //player.Play();
                    GPSAlerted = true;
                }

                lblGPSStatus.BackColor = System.Drawing.Color.Lime;
                lblGPSStatus.ForeColor = System.Drawing.Color.Black;
                lblGPSStatus.Text = "GPS Online";
                hasFix = true;
            }
            else
            {
                lblGPSStatus.BackColor = System.Drawing.Color.Red;
                lblGPSStatus.ForeColor = System.Drawing.Color.White;
                lblGPSStatus.Text = "GPS Offline";
                lblGPSStatus.Text = "No Satellite fix";
                lblGPSStatus.BackColor = System.Drawing.Color.Pink;
                lblCourse.Text = "N/A";
                lblSpeed.Text = "N/A";
                lblGPGGATime.Text = GPS.GPRMC.TimeOfFix.ToString("F");
                hasFix = false;
            }

            switch (e.TypeOfEvent)
            {
                case GPSEventType.GPGSA:  //Recommended minimum specific GPS/Transit data
                    lblGPGGAPosition.Text = GPS.GPGGA.Position.ToString("DMS");
                    lblSpeed.Text = GPS.GPRMC.Speed.ToString() + " mph";
                    lblGPGGATime.Text = GPS.GPRMC.TimeOfFix.ToString("F");
                    lblMagVar.Text = GPS.GPRMC.MagneticVariation.ToString();
                    lblCourse.Text = GPS.GPRMC.Course.ToString();

                    //Determine from the course which direction. North/South/East/West
                    if (float.Parse(GPS.GPRMC.Course.ToString()) < 90 && float.Parse(GPS.GPRMC.Course.ToString()) > 0)
                        lblGPSDirection.Text = "East";
                    else if (float.Parse(GPS.GPRMC.Course.ToString()) < 180 && float.Parse(GPS.GPRMC.Course.ToString()) > 90)
                        lblGPSDirection.Text = "South";
                    else if (float.Parse(GPS.GPRMC.Course.ToString()) < 270 && float.Parse(GPS.GPRMC.Course.ToString()) > 180)
                        lblGPSDirection.Text = "West";
                    else if (float.Parse(GPS.GPRMC.Course.ToString()) < 360 && float.Parse(GPS.GPRMC.Course.ToString()) > 270)
                        lblGPSDirection.Text = "North";
                    break;
                case GPSEventType.GPGGA:
                    lblGPSAltitude.Text = GPS.GPGGA.Altitude.ToString() + " " + GPS.GPGGA.AltitudeUnits;
                    lblGPSNumSatellites.Text = GPS.GPGGA.NoOfSats.ToString();
                    lblGPSAccuracy.Text = GPS.GPGGA.FixQuality.ToString();
                    break;
                case GPSEventType.TimeOut:  //GPS Timeout
                    lblGPSStatus.Text = "Serialport timeout";
                    lblGPSStatus.BackColor = System.Drawing.Color.Red;
                    break;
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            GPS.Dispose();  //Closes serial port and cleans up. This is important !
        }
        #endregion

        #region Menu Items
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void aboToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout about = new frmAbout();
            about.ShowDialog();
        }
        private void gPSScanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startRecording();
        }
        private void scanToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
        private void scannerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            takeSnapshot();
        }
        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmConfig config = new frmConfig();
            config.ShowDialog();
        }
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GPS.IsPortOpen)
            {
                startGPS();
                startToolStripMenuItem.Text = "Stop";
            }
            else
            {
                GPS.Stop();
                updateLog((int)Message.gpsStopped, null);
                startToolStripMenuItem.Text = "Start";
            }
        }
        private void sendDataToWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logLogToSite();
        }
        #endregion

        //Take a wifi snapshot, and log if indicated
        private void takeSnapshot()
        {
            //Turn GPS on if required
            if (Properties.Settings.Default.requireGPS && !GPS.IsPortOpen)
            {
                startGPS();
            }

            refreshInformationPanel();

            //SoundPlayer player;
            //Do we have a fix? OR Do we not required a GPS?

            if (hasFix || !Properties.Settings.Default.requireGPS)
            {
                //Start wifi scanner
                if (Properties.Settings.Default.modeAdvanced)
                {
                    scanWifi();
                }
                else
                {
                    scanWifiBasic();
                }


                //Has wifi been found?
                if (wifiFound)
                {
                    string LatandLong = string.Empty;

                    //player = new SoundPlayer(@"C:\Windows\Media\chimes.wav");   //Play sound to indicate that hotspots have been located
                    //player.Play();

                    //Are we recording? If so, log the data or push to site if needed
                    if (scanStarted)
                    {
                        if (Properties.Settings.Default.logtoFile)  //Write the data to a local log file?
                        {
                            lblStatus.Text = "...Writing to log file";
                            Thread saveLog = new Thread(saveToLog);
                            saveLog.Start();
                        }
                        if (Properties.Settings.Default.logtoURL)   //Write the data to a remote website?
                        {
                            lblStatus.Text = "...Sending data to site";
                            Thread pushThread = new Thread(saveToSite);
                            pushThread.Start();
                        }
                    }
                }
            }
            else  //We do NOT have a fix and we REQUIRE GPS coordinates
            {
                //Play error sound
                //player = new SoundPlayer(@"C:\Windows\Media\Borealis\Question_background.wav");  //If we do not have a fix, play a sound
                //player.Play();
                updateLog((int)Message.gpsNoFix, string.Empty);
                lblStatus.BackColor = System.Drawing.Color.Yellow;
                lblStatus.Text = "...GPS Fix Required";

            }
        }

        #region Scanning
        //Scanning/GPS
        private void startRecording()
        {
            if (scanStarted)
            {
                Clock.Stop();
                GPS.Stop();
                gPSScanToolStripMenuItem.Text = "Start Recording";
                scanStarted = false;
            }
            else
            {
                if (Properties.Settings.Default.scanDelay.ToString() != string.Empty)
                {
                    startGPS();
                    Clock = new System.Windows.Forms.Timer();
                    Clock.Interval = int.Parse(Properties.Settings.Default.scanDelay.ToString());
                    Clock.Start();
                    Clock.Tick += new EventHandler(Timer_Tick);
                    gPSScanToolStripMenuItem.Text = "Stop Recording";
                    scanStarted = true;
                }
                else
                    scanWifi();
            }
        }
        private void startGPS()
        {
            DateTime timenow = DateTime.Now;
            if (!GPS.IsPortOpen)
            {
                try
                {
                    GPS.Start(Properties.Settings.Default.comPort, 4800);
                    startToolStripMenuItem.Text = "Stop";
                    string messageText = DateTime.Now + " - GPS: Scanner Started...\r\n";
                    if (txtLog.InvokeRequired)
                        txtLog.Invoke(new updateMessageLog(OutputUpdateCallback),
                        new object[] { messageText });
                    else
                        OutputUpdateCallback(messageText); //call directly
                }
                catch (Exception ex)
                {
                    string messageText = DateTime.Now + " - GPS: ERROR - " + ex.Message + "\r\n";
                    if (txtLog.InvokeRequired)
                        txtLog.Invoke(new updateMessageLog(OutputUpdateCallback),
                        new object[] { messageText });
                    else
                        OutputUpdateCallback(messageText); //call directly
                }
            }

        }
        private void scanWifi()
        {
            string output;
            string line;
            int BSSIDNumber = 0;
            DateTime timenow = DateTime.Now;
            NetworkIndex = -1;
            wifiFound = false;

            refreshInformationPanel();
            updateLog((int)Message.logScanning, string.Empty);
            lblStatus.BackColor = System.Drawing.Color.Lime;
            lblStatus.Text = "...Scanning";

            Process proc = new Process();
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.FileName = "netsh";
            proc.StartInfo.Arguments = "wlan show networks mode=bssid";
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.Start();
            output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            StringReader sr = new StringReader(output.ToString());
            line = null;

            while ((line = sr.ReadLine()) != null)
            {
                if (line.StartsWith("General Failure"))
                {
                    // Wifi disconnect or not installed
                    break;
                }

                if (line.StartsWith("SSID"))
                {
                    NetworkIndex++;

                    for (int i = 0; i < 9; i++)
                    {
                        Networks[NetworkIndex, i] = " ";
                    }

                    Networks[NetworkIndex, 3] = "0%";
                    BSSIDNumber = 0;
                    Networks[NetworkIndex, 1] = line.Substring(line.IndexOf(":") + 1).TrimEnd(' ').TrimStart(' ');
                    continue;
                }


                if (line.IndexOf("Network type") > 0)
                {
                    if (line.EndsWith("Infrastructure"))
                    {
                        Networks[NetworkIndex, 7] = "AP";
                        continue;
                    }
                    else
                    {
                        Networks[NetworkIndex, 7] = line.Substring(line.IndexOf(":") + 1); //"Ad-hoc";
                    }
                }
                if (line.IndexOf("Authentication") > 0)
                {
                    Networks[NetworkIndex, 4] = line.Substring(line.IndexOf(":") + 1).TrimStart(' ').TrimEnd(' ');
                    continue;
                }
                if (line.IndexOf("Encryption") > 0)
                {
                    Networks[NetworkIndex, 5] = line.Substring(line.IndexOf(":") + 1).TrimStart(' ').TrimEnd(' ');
                    continue;
                }
                if (line.IndexOf("BSSID") > 0)
                {
                    if ((Convert.ToInt32(line.IndexOf("BSSID" + 6)) > BSSIDNumber))
                    {
                        BSSIDNumber = Convert.ToInt32(line.IndexOf("BSSID" + 6));
                        NetworkIndex++;
                        Networks[NetworkIndex, 1] = Networks[NetworkIndex - 1, 1]; // same SSID 
                        Networks[NetworkIndex, 7] = Networks[NetworkIndex - 1, 7]; // same Network Type
                        Networks[NetworkIndex, 4] = Networks[NetworkIndex - 1, 4]; // Same authorization
                        Networks[NetworkIndex, 5] = Networks[NetworkIndex - 1, 5]; // same encryption
                    }
                    Networks[NetworkIndex, 0] = line.Substring(line.IndexOf(":") + 1);
                    continue;
                }
                if (line.IndexOf("Signal") > 0)
                {
                    Networks[NetworkIndex, 3] = line.Substring(line.IndexOf(":") + 1);
                    continue;
                }
                if (line.IndexOf("Radio Type") > 0)
                {
                    Networks[NetworkIndex, 6] = line.Substring(line.IndexOf(":") + 1);
                    continue;
                }
                if (line.IndexOf("Channel") > 0)
                {
                    Networks[NetworkIndex, 2] = line.Substring(line.IndexOf(":") + 1);
                    continue;
                }
                if (line.IndexOf("Basic Rates") > 0)
                {
                    //Networks[NetworkIndex, 8] = line.Substring(line.Length - 2, 2);
                    Networks[NetworkIndex, 8] = line.Substring(line.IndexOf(":"));
                    if (Networks[NetworkIndex, 8] == ":") { Networks[NetworkIndex, 8] = "not shown"; continue; }
                    Networks[NetworkIndex, 8] = Networks[NetworkIndex, 8].TrimStart(':').TrimStart(' ').TrimEnd(' ');
                    for (int i = Networks[NetworkIndex, 8].Length - 1; i > 0; i--)
                    {
                        if (Networks[NetworkIndex, 8].Substring(i, 1) == " ")
                        {
                            Networks[NetworkIndex, 8] = Networks[NetworkIndex, 8].Substring(i + 1, Networks[NetworkIndex, 8].Length - 1 - i);
                            break;
                        }
                    }
                }
                if (line.IndexOf("Other Rates") > 0)
                {
                    // overwrite the basic rates if this entry is present
                    Networks[NetworkIndex, 8] = line.Substring(line.IndexOf(":"));
                    if (Networks[NetworkIndex, 8] == ":") { Networks[NetworkIndex, 8] = "not shown"; continue; }
                    Networks[NetworkIndex, 8] = Networks[NetworkIndex, 8].TrimStart(':').TrimStart(' ').TrimEnd(' ');
                    for (int i = Networks[NetworkIndex, 8].Length - 1; i >= 0; i--)
                    {
                        if (Networks[NetworkIndex, 8].Substring(i, 1) == " ")
                        {
                            Networks[NetworkIndex, 8] = Networks[NetworkIndex, 8].Substring(i + 1, Networks[NetworkIndex, 8].Length - 1 - i);
                            break;
                        }
                    }
                }
            }

            listView1.Items.Clear();

            for (int i = 0; i < NetworkIndex + 1; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    ListViewItem SearchItem = new ListViewItem();
                    if (Networks[i, 0] == " ") continue; // don't search if no valid MAC Address !
                    SearchItem = listView1.FindItemWithText(Networks[i, k]);
                    if (SearchItem == null)
                    {
                        // New discovery - add it to the list

                        SystemSounds.Question.Play();

                        listView1.Items.Add(Networks[i, 0]);                                          // MAC Address
                        listView1.Items[listView1.Items.Count - 1].SubItems.Add(Networks[i, 1]);      // SSID
                        listView1.Items[listView1.Items.Count - 1].SubItems.Add(Networks[i, 2]);      // Channel
                        listView1.Items[listView1.Items.Count - 1].SubItems.Add(Networks[i, 3]);      // Signal
                        listView1.Items[listView1.Items.Count - 1].SubItems.Add(Networks[i, 4]);      // Authenticatiopn
                        listView1.Items[listView1.Items.Count - 1].SubItems.Add(Networks[i, 5]);      // Encryption
                        listView1.Items[listView1.Items.Count - 1].SubItems.Add(Networks[i, 6]);      // Radio Type
                        listView1.Items[listView1.Items.Count - 1].SubItems.Add(Networks[i, 7]);      // Network Type
                        listView1.Items[listView1.Items.Count - 1].SubItems.Add(Networks[i, 8]);      // Speed

                        if ((Networks[i, 4] == "Open") & (Networks[i, 5] == "None")) listView1.Items[listView1.Items.Count - 1].BackColor = Color.PaleGreen;
                        listView1.Items[listView1.Items.Count - 1].EnsureVisible();
                        if ((Networks[i, 4] != "Open")) listView1.Items[listView1.Items.Count - 1].BackColor = Color.Pink;
                        listView1.Items[listView1.Items.Count - 1].EnsureVisible();

                        wifiFound = true;
                    }
                }
            }
        }
        private void scanWifiBasic()
        {
            NetworkIndex = -1;
            lblStatus.Text = "...Scanning Basic";
            WlanClient client = new WlanClient();
            foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
            {
                // Lists all networks with WEP security
                Wlan.WlanAvailableNetwork[] networks = wlanIface.GetAvailableNetworkList(0);
                foreach (Wlan.WlanAvailableNetwork network in networks)
                {
                    NetworkIndex++;

                    string auth = string.Empty;
                    switch (network.dot11DefaultAuthAlgorithm)
                    {
                        case Wlan.Dot11AuthAlgorithm.IEEE80211_Open:
                            auth = "Open";
                            break;
                        case Wlan.Dot11AuthAlgorithm.IEEE80211_SharedKey:
                            auth = "WEP";
                            break;
                        case Wlan.Dot11AuthAlgorithm.IHV_End:
                            break;
                        case Wlan.Dot11AuthAlgorithm.IHV_Start:
                            break;
                        case Wlan.Dot11AuthAlgorithm.RSNA:
                            auth = "RSNA";
                            break;
                        case Wlan.Dot11AuthAlgorithm.RSNA_PSK:
                            auth = "RSNA PSK";
                            break;
                        case Wlan.Dot11AuthAlgorithm.WPA:
                            auth = "WPA";
                            break;
                        case Wlan.Dot11AuthAlgorithm.WPA_None:
                            auth = "WPA None";
                            break;
                        case Wlan.Dot11AuthAlgorithm.WPA_PSK:
                            auth = "WPA PSK";
                            break;
                        default:
                            auth = "N/A";
                            break;
                    }

                    Networks[NetworkIndex, 0] = "N/A";
                    Networks[NetworkIndex, 1] = GetStringForSSID(network.dot11Ssid);
                    Networks[NetworkIndex, 2] = "0";
                    Networks[NetworkIndex, 3] = network.wlanSignalQuality.ToString() + "%";
                    Networks[NetworkIndex, 4] = auth;
                    Networks[NetworkIndex, 5] = network.dot11DefaultCipherAlgorithm.ToString();
                    Networks[NetworkIndex, 6] = "N/A";
                    Networks[NetworkIndex, 7] = "N/A";
                    Networks[NetworkIndex, 8] = "N/A";


                    listView1.Items.Add(Networks[NetworkIndex, 0]);                                          // MAC Address
                    listView1.Items[listView1.Items.Count - 1].SubItems.Add(Networks[NetworkIndex, 1]);      // SSID
                    listView1.Items[listView1.Items.Count - 1].SubItems.Add(Networks[NetworkIndex, 2]);      // Channel
                    listView1.Items[listView1.Items.Count - 1].SubItems.Add(Networks[NetworkIndex, 3]);      // SignaL
                    listView1.Items[listView1.Items.Count - 1].SubItems.Add(Networks[NetworkIndex, 4]);      // Authenticatiopn
                    listView1.Items[listView1.Items.Count - 1].SubItems.Add(Networks[NetworkIndex, 5]);      // Encryption
                    listView1.Items[listView1.Items.Count - 1].SubItems.Add(Networks[NetworkIndex, 6]);      // Radio Type
                    listView1.Items[listView1.Items.Count - 1].SubItems.Add(Networks[NetworkIndex, 7]);      // Network Type
                    listView1.Items[listView1.Items.Count - 1].SubItems.Add(Networks[NetworkIndex, 8]);      // Speed
                }
            }
        }
        #endregion

        #region Logging
        //Send data to log
        private void saveToLog()
        {
            string messageText = string.Empty;
            for (int i = 0; i < NetworkIndex + 1; i++)
            {
                //Does log file exist?
                if (File.Exists(Properties.Settings.Default.logFileLocation))
                {
                    string[] coords = new string[2];
                    string logData;

                    //Get the coordinates
                    coords = convertCoords(lblGPGGAPosition.Text);

                    //Begin to read log file
                    StreamReader reader = new StreamReader(Properties.Settings.Default.logFileLocation);
                    logData = reader.ReadToEnd();
                    reader.Close();

                    //Does the MAC address exist? Are we in basic mode?
                    if (Regex.IsMatch(logData, Networks[i, 0]) && Properties.Settings.Default.modeAdvanced)
                        messageText += DateTime.Now + " - Scanner: Duplicate Found, skipping [ " + Networks[i, 1] + " ]\r\n";
                    else
                    {
                        //Write to file
                        StreamWriter SW;
                        messageText = DateTime.Now + " - Scanner: Writing to File [ " + Networks[i, 1] + " ]\r\n";
                        SW = File.AppendText(@"C:\wifi.log");
                        SW.WriteLine("################ Wifi Point - " + i.ToString() + "################");
                        SW.WriteLine("$lat - " + coords[0] + "");
                        SW.WriteLine("$longitude - " + coords[1] + "");
                        SW.WriteLine("$mac - " + Networks[i, 0] + "");
                        SW.WriteLine("$ssid - " + Networks[i, 1] + "");
                        SW.WriteLine("$channel - " + int.Parse(Networks[i, 2].Replace(" ", string.Empty)) + "");
                        SW.WriteLine("$signal - " + int.Parse(Networks[i, 3].Replace("%", string.Empty)) + "");
                        SW.WriteLine("$auth - " + Networks[i, 4] + "");
                        SW.WriteLine("$encryption - " + Networks[i, 5] + "");
                        SW.WriteLine("$radio - " + Networks[i, 6] + "");
                        SW.WriteLine("$nettype - " + Networks[i, 7] + "");
                        SW.WriteLine("$speed - " + Networks[i, 8] + "");
                        SW.WriteLine("$user - Lee");
                        SW.WriteLine("$entrydtm - " + DateTime.Now + "");
                        SW.Close();
                    }
                }
                else
                {
                    //Create file
                    FileInfo wifiLog = new FileInfo(Properties.Settings.Default.logFileLocation);
                    wifiLog.Create();
                    messageText = DateTime.Now + " - Scanner: WifiLog does not exist. Creating...\r\n";

                    //Attempt to log again
                  
                    saveToLog();
                }
                if (txtLog.InvokeRequired)
                    txtLog.Invoke(new updateMessageLog(OutputUpdateCallback),
                    new object[] { messageText });
                else
                    OutputUpdateCallback(messageText); //call directly  
            }
        }
        //Send data to site
        private void saveToSite()
        {
            for (int i = 0; i < NetworkIndex + 1; i++)
            {
                string postData;
                string signal = Networks[i, 3];
                string[] coords = new string[2];

                //Calculate Latitude and Longitude
                coords = convertCoords(lblGPGGAPosition.Text);

                //Generate Post data
                postData = "lat=" + coords[0] +
                    "&long=" + coords[1] +
                    "&mac=" + Networks[i, 0] +
                    "&ssid=" + Networks[i, 1] +
                    "&channel=" + Networks[i, 2] +
                    "&signal=" + signal +
                    "&auth=" + Networks[i, 4] +
                    "&encryption=" + Networks[i, 5] +
                    "&radio=" + Networks[i, 6] +
                    "&nettype=" + Networks[i, 7] +
                    "&lspeedat=" + Networks[i, 8] +
                    "&user=Test";

                //Update Log
                string messageText = DateTime.Now + " - Scanner: Pushing Hotspot [ " + Networks[i, 1] + " ]...\r\n";
                if (txtLog.InvokeRequired)
                    txtLog.Invoke(new updateMessageLog(OutputUpdateCallback),
                    new object[] { messageText });
                else
                    OutputUpdateCallback(messageText); //call directly

                //Send to site
                pushDataToSite(postData);
            }
        }
        //*Send local log to website
        private void logLogToSite()
        {
            for (int i = 0; i < LogNetworkIndex; i++)
            {

                string latt = logNetworks[i, 1];
                string lon = logNetworks[i, 2];
                string[] replace = { "\"", "N", "S", "E", "W" };
                string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string url = Properties.Settings.Default.webURL;
                for (int s = 0; s < replace.Length; s++)
                {
                    lon = lon.Replace(replace[s], string.Empty);
                    latt = latt.Replace(replace[s], string.Empty);
                }
                float lat = float.Parse(latt);

                float longi = float.Parse(lon);

                if (logNetworks[i, 1].StartsWith("0"))
                {
                    lat = -lat;
                }
                if (logNetworks[i, 2].StartsWith("0"))
                {
                    longi = -longi;
                }

                string postData = "lat=" + lat.ToString() +
                    "&long=" + longi.ToString() +
                    "&mac=" + logNetworks[i, 3] +
                    "&ssid=" + logNetworks[i, 4] +
                    "&channel=" + logNetworks[i, 5] +
                    "&signal=" + logNetworks[i, 6] +
                    "&auth=" + logNetworks[i, 7] +
                    "&encryption=" + logNetworks[i, 8] +
                    "&radio=" + logNetworks[i, 9] +
                    "&nettype=" + logNetworks[i, 10] +
                    "&lspeedat=" + logNetworks[i, 11] +
                    "&user=Test";
                url += postData;

                string messageText = DateTime.Now + " - Scanner: Pushing Hotspot [ " + logNetworks[i, 1] + " ]...\r\n";
                if (txtLog.InvokeRequired)
                    txtLog.Invoke(new updateMessageLog(OutputUpdateCallback),
                    new object[] { messageText });
                else
                    OutputUpdateCallback(messageText); //call directly

                lblStatus.Text = "Sending: " + logNetworks[i, 1];

                try
                {
                    //updateLog((int)Message.logPushing, logNetworks[i, 1]);
                    //Open request to the site
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "POST";
                    request.AllowWriteStreamBuffering = true;
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = postData.Length;

                    //Create buffer of POST data
                    byte[] Buffer = System.Text.Encoding.ASCII.GetBytes(postData);

                    Stream PostData = request.GetRequestStream();
                    PostData.Write(Buffer, 0, Buffer.Length);
                    request.GetRequestStream().Close();
                    request.Abort();
                    request.GetRequestStream().Dispose();
                    PostData.Close();
                    PostData.Dispose();
                }
                catch (Exception ex)
                {
                    messageText = DateTime.Now + " - Scanner: ERROR - " + ex.Message + "\r\n";
                    if (txtLog.InvokeRequired)
                        txtLog.Invoke(new updateMessageLog(OutputUpdateCallback),
                        new object[] { messageText });
                    else
                        OutputUpdateCallback(messageText); //call directly
                }
            }
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Send data to site that is set in the settings of the application
        /// </summary>
        /// <param name="postData"></param>
        private void pushDataToSite(string postData)
        {
            string url = Properties.Settings.Default.webURL + postData;
            try
            {
                //Open request to the site
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.AllowWriteStreamBuffering = true;
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postData.Length;

                //Create buffer of POST data
                byte[] Buffer = System.Text.Encoding.ASCII.GetBytes(postData);

                Stream PostData = request.GetRequestStream();
                PostData.Write(Buffer, 0, Buffer.Length);

                //Close and clean up
                request.GetRequestStream().Close();
                request.Abort();
                request.GetRequestStream().Dispose();
                PostData.Close();
                PostData.Dispose();
            }
            catch (Exception ex)
            {
                string messageText = DateTime.Now + " - Scanner: ERROR - " + ex.Message + "\r\n";
                if (txtLog.InvokeRequired)
                    txtLog.Invoke(new updateMessageLog(OutputUpdateCallback),
                    new object[] { messageText });
                else
                    OutputUpdateCallback(messageText); //call directly
            }
        }

        /// <summary>
        /// Converts a 802.11 SSID to a string.
        /// </summary>
        static string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        /// <summary>
        /// Converts DMS latitude and longitude format to decimal
        /// </summary>
        /// <param name="position">Latitude and longitude in DMS format</param>
        /// <returns>Array of latitude and longitude in decimal format</returns>
        public string[] convertCoords(string position)
        {
            string[] coords = new string[2];

            if (position != "No Data")
            {
                string[] replace = { "\"", "N", "S", "E", "W" };

                for (int n = 0; n < replace.Length; n++)
                    position = position.Replace(replace[n], string.Empty);

                position = position.Replace("\'", ":");
                position = position.Replace("°", ":");

                coords = position.Split(' ');

                string[] latParts = coords[0].Split(':');
                string[] longParts = coords[1].Split(':');

                float lat = float.Parse(latParts[0]);
                float latm = float.Parse(latParts[1]);
                float lats = float.Parse(latParts[2]);

                float longi = float.Parse(longParts[0]);
                float longim = float.Parse(longParts[1]);
                float longis = float.Parse(longParts[2]);

                if (latParts[0].StartsWith("0"))
                {
                    lat = -lat;
                }
                if (longParts[0].StartsWith("0"))
                {
                    longi = -longi;
                }
                if (lat < 0)
                {
                    latm = -latm;
                    lats = -lats;
                }
                if (longi < 0)
                {
                    longim = -longim;
                    longis = -longis;
                }

                lat = lat + latm / 60 + lats / 3600;
                longi = longi + longim / 60 + longis / 3600;

                coords[0] = lat.ToString();
                coords[1] = longi.ToString();
            }
            else
            {
                coords[0] = "N/A";
                coords[1] = "N/A";
            }
            return coords;
        }

        private void OutputUpdateCallback(string data)
        {
            txtLog.Text += data;
        }
        #endregion

        /// <summary>
        /// Sends current data to website
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sendToSiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveToSite();
        }

        /// <summary>
        /// Sends current data to log
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveToLog();
        }
    }
}

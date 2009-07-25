using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Media;
using System.Text.RegularExpressions;
using System.Threading;

using NativeWifi;
using SharpGis.SharpGps;

namespace WITSniff
{
    public partial class Form1 : Form
    {
        System.Windows.Forms.Timer Clock;
        delegate void updateMessageLog(string data);
        ArrayList hotspots = new ArrayList();
        ArrayList hotspotsLogged = new ArrayList();

        #region Variables
        public static GPSHandler GPS;
        bool hasFix = false, wifiFound = false, scanStarted = false, GPSAlerted = false;
        int NetworkIndex = -1, LogNetworkIndex = -1;
        enum Message
        {
            gpsScanStart = 0,
            gpsScanStarted,
            gpsError,
            gpsNoFix,
            gpsStopping,
            gpsStopped,
            logScanning,
            logDuplicate,
            logResult,
            logLogging,
            logPushing,
        };
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
            /*
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
             * */
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
        #endregion

        private void OutputUpdateCallback(string data)
        {
            txtLog.Text += data;
        }

        //Take a wifi snapshot, and log if indicated
        private void takeSnapshot()
        {
            //Turn GPS on if required
            if (Properties.Settings.Default.requireGPS && !GPS.IsPortOpen)
                startGPS();

            refreshInformationPanel();

            //Do we have a fix? OR Do we not required a GPS?
            if (hasFix || !Properties.Settings.Default.requireGPS)
            {
                //Start wifi scanner
                if (Properties.Settings.Default.modeAdvanced)
                    scanWifi();
                else
                    scanWifiBasic();

                //Has wifi been found?
                if (wifiFound)
                {
                    //Are we recording? If so, log the data or push to site if needed
                    if (scanStarted)
                    {
                        if (Properties.Settings.Default.logtoFile)  //Write the data to a local log file?
                        {
                            lblStatus.Text = "...Writing to log file";
                            wifiMapper.saveListviewToLog(listView1);
                        }
                        if (Properties.Settings.Default.logtoURL)   //Write the data to a remote website?
                        {
                            lblStatus.Text = "...Sending data to site";
                            wifiMapper.saveListviewToSite(listView1);
                        }
                    }
                }
            }
            else  //We do NOT have a fix and we REQUIRE GPS coordinates
            {
                updateLog((int)Message.gpsNoFix, string.Empty);
                lblStatus.BackColor = System.Drawing.Color.Yellow;
                lblStatus.Text = "...GPS Fix Required";

            }
        }

        /// <summary>
        /// Start Recording
        /// -Begin Clock if not started
        /// -Stop if it has already begun
        /// </summary>
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

        /// <summary>
        /// Start GPS
        /// </summary>
        private void startGPS()
        {
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

        #region Scanning
        /// <summary>
        /// Scan wifi
        /// -CMD command
        /// -Send findings to listview
        /// </summary>
        private void scanWifi()
        {
            string output, line;
            int BSSIDNumber = 0;
            hotspots.Clear();
            wifiFound = false;                  //wifi has not been found

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

            HotSpot hotspot = new HotSpot();
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
                    hotspot = new HotSpot();
                    string[] coords;
                    coords = Utilities.convertCoords(lblGPGGAPosition.Text);

                    hotspot.Signal = "0";
                    BSSIDNumber = 0;

                    hotspot.Latitude = coords[0];
                    hotspot.Longitude = coords[1];
                    hotspot.SSID = line.Substring(line.IndexOf(":") + 1).TrimEnd(' ').TrimStart(' ');
                    continue;
                }

                if (line.IndexOf("Network type") > 0)
                {
                    if (line.EndsWith("Infrastructure"))
                    {
                        hotspot.NetworkType = "AP";
                        continue;
                    }
                    else
                    {
                        hotspot.NetworkType = line.Substring(line.IndexOf(":") + 1); //"Ad-hoc";
                    }
                }

                if (line.IndexOf("Authentication") > 0)
                {
                    hotspot.Authentication = line.Substring(line.IndexOf(":") + 1).TrimStart(' ').TrimEnd(' ');
                    continue;
                }
                if (line.IndexOf("Encryption") > 0)
                {
                    hotspot.Encryption = line.Substring(line.IndexOf(":") + 1).TrimStart(' ').TrimEnd(' ');
                    continue;
                }
                if (line.IndexOf("BSSID") > 0)
                {
                    if ((Convert.ToInt32(line.IndexOf("BSSID" + 6)) > BSSIDNumber))
                    {
                        BSSIDNumber = Convert.ToInt32(line.IndexOf("BSSID" + 6));
                        NetworkIndex++;
                        HotSpot previousHotspot = (HotSpot)hotspots[NetworkIndex - 1];
                        hotspot = previousHotspot;  //They are the same.
                    }
                    hotspot.Mac = line.Substring(line.IndexOf(":") + 1);
                    continue;
                }
                if (line.IndexOf("Signal") > 0)
                {
                    hotspot.Signal = line.Substring(line.IndexOf(":") + 1);
                    continue;
                }
                if (line.IndexOf("Radio Type") > 0)
                {
                    hotspot.RadioType = line.Substring(line.IndexOf(":") + 1);
                    continue;
                }
                if (line.IndexOf("Channel") > 0)
                {
                    hotspot.Channel = line.Substring(line.IndexOf(":") + 1);
                    hotspots.Add(hotspot);
                    continue;
                }
            }

            listView1.Items.Clear();

            if (hotspots != null)
            {
                foreach (HotSpot spot in hotspots)
                {
                    wifiMapper.SendToList(listView1, spot);
                }
            }
        }

        /// <summary>
        /// Scan wifi BASIC
        /// -Used only for XP machines
        /// -Send findings to listview
        /// </summary>
        private void scanWifiBasic()
        {
            NetworkIndex = -1;
            lblStatus.Text = "...Scanning Basic";
            WlanClient client = new WlanClient();
            HotSpot hotspot;
            foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
            {
                // Lists all networks with WEP security
                Wlan.WlanAvailableNetwork[] networks = wlanIface.GetAvailableNetworkList(0);
                foreach (Wlan.WlanAvailableNetwork network in networks)
                {
                    NetworkIndex++;
                    hotspot = new HotSpot();
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


                    hotspot.Mac = "N/A";
                    hotspot.SSID = Utilities.GetStringForSSID(network.dot11Ssid);
                    hotspot.Channel = "0";
                    hotspot.Signal = network.wlanSignalQuality.ToString() + "%";
                    hotspot.Authentication = auth;
                    hotspot.Encryption = network.dot11DefaultCipherAlgorithm.ToString();
                    hotspot.RadioType = "N/A";
                    hotspot.NetworkType = "N/A";
                    hotspot.Speed = "N/A";

                    //Send to the main list
                    wifiMapper.SendToList(listView1, hotspot);
                }
            }
        }
        #endregion

        #region Saving Buttons
        /// <summary>
        /// Send History hotspots to site
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogSendSite_Click(object sender, EventArgs e)
        {
            wifiMapper.saveListviewToSite(listView2);
        }

        /// <summary>
        /// Send History hotspots to log
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogSendLog_Click(object sender, EventArgs e)
        {
            wifiMapper.saveListviewToLog(listView2);
        }

        /// <summary>
        /// Send hotspots to site
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendSite_Click(object sender, EventArgs e)
        {
            wifiMapper.saveListviewToSite(listView1);
        }

        /// <summary>
        /// Send hotspots to log
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendLog_Click(object sender, EventArgs e)
        {
            wifiMapper.saveListviewToLog(listView1);
        }
        #endregion
    }
}

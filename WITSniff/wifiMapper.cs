using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Text;

namespace WITSniff
{
    /// <summary>
    /// Hotspot Object
    /// </summary>
    public class HotSpot
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Mac { get; set; }
        public string SSID { get; set; }
        public string Channel { get; set; }
        public string Signal { get; set; }
        public string Authentication { get; set; }
        public string Encryption { get; set; }
        public string RadioType { get; set; }
        public string NetworkType { get; set; }
        public string Speed { get; set; }
    }

    public class wifiMapper
    {
        /// <summary>
        /// Send a hotspot to a list
        /// </summary>
        /// <param name="hotspot"></param>
        static public void SendToList(ListView listview, HotSpot hotspot)
        {
            ListViewItem searchItem;
            //Make sure hotspot does not already exist.
            if ((searchItem = listview.FindItemWithText(hotspot.Mac)) == null)
            {
                listview.Items.Add(hotspot.Latitude);
                listview.Items[listview.Items.Count - 1].SubItems.Add(hotspot.Longitude);
                listview.Items[listview.Items.Count - 1].SubItems.Add(hotspot.Mac);
                listview.Items[listview.Items.Count - 1].SubItems.Add(hotspot.SSID);
                listview.Items[listview.Items.Count - 1].SubItems.Add(hotspot.Channel);
                listview.Items[listview.Items.Count - 1].SubItems.Add(hotspot.Signal);
                listview.Items[listview.Items.Count - 1].SubItems.Add(hotspot.Authentication);
                listview.Items[listview.Items.Count - 1].SubItems.Add(hotspot.Encryption);
                listview.Items[listview.Items.Count - 1].SubItems.Add(hotspot.RadioType);
                listview.Items[listview.Items.Count - 1].SubItems.Add(hotspot.NetworkType);
                listview.Items[listview.Items.Count - 1].SubItems.Add(hotspot.Speed);

                if (hotspot.Authentication == "Open" && hotspot.Encryption == "None")
                {
                    listview.Items[listview.Items.Count - 1].BackColor = System.Drawing.Color.LightGreen;
                }
                else
                {
                    listview.Items[listview.Items.Count - 1].BackColor = System.Drawing.Color.LightPink;
                }
            }
        }

        /// <summary>
        /// Send a hotspot to a site
        /// </summary>
        /// <param name="url">Website URL in format: http://www.SITENAME.com/page.php?</param>
        /// <param name="postData">POST data to send in format: var1&var2&var3</param>
        /// <returns>True/False</returns>
        static private string SendToSite(HotSpot hotspot)
        {
            try
            {
                //Generate Post data
                string postData = "lat=" + hotspot.Latitude +
                    "&long=" + hotspot.Longitude +
                    "&mac=" + hotspot.Mac +
                    "&ssid=" + hotspot.SSID +
                    "&channel=" + hotspot.Channel +
                    "&signal=" + hotspot.Signal +
                    "&auth=" + hotspot.Authentication +
                    "&encryption=" + hotspot.Encryption +
                    "&radio=" + hotspot.RadioType +
                    "&nettype=" + hotspot.NetworkType +
                    "&lspeedat=" + hotspot.Speed +
                    "&user=Test";

                string url = Properties.Settings.Default.webURL + postData;
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

                return DateTime.Now + " - Scanner: Sending to site [ " + hotspot.SSID + " ]\r\n";
            }
            catch
            {
                return DateTime.Now + " - Scanner: Error sending to site [ " + hotspot.SSID + " ]\r\n";
            }
        }

        /// <summary>
        /// Send a hotspot to a file
        /// </summary>
        /// <param name="hotspot"></param>
        /// <returns></returns>
        static private string SendToFile(HotSpot hotspot)
        {
            string messageText = string.Empty;
            //Does log file exist?
            if (File.Exists(Properties.Settings.Default.logFileLocation))
            {
                string logData;

                //Begin to read log file
                StreamReader reader = new StreamReader(Properties.Settings.Default.logFileLocation);
                logData = reader.ReadToEnd();
                reader.Close();

                //Does the MAC address exist? Are we in basic mode?
                if (Regex.IsMatch(logData, hotspot.Mac) && Properties.Settings.Default.modeAdvanced)
                {
                    return (DateTime.Now + " - Scanner: Duplicate Found, skipping [ " + hotspot.SSID + " ]\r\n");
                }
                else
                {
                    //Write to file
                    StreamWriter SW;
                    SW = File.AppendText(Properties.Settings.Default.logFileLocation);
                    SW.WriteLine("##########");
                    SW.WriteLine("$lat - " + hotspot.Latitude);
                    SW.WriteLine("$longitude - " + hotspot.Longitude);
                    SW.WriteLine("$mac - " + hotspot.Mac);
                    SW.WriteLine("$ssid - " + hotspot.SSID);
                    SW.WriteLine("$channel - " + int.Parse(hotspot.Channel.Replace(" ", string.Empty)));
                    SW.WriteLine("$signal - " + int.Parse(hotspot.Signal.Replace("%", string.Empty)));
                    SW.WriteLine("$auth - " + hotspot.Authentication);
                    SW.WriteLine("$encryption - " + hotspot.Encryption);
                    SW.WriteLine("$radio - " + hotspot.RadioType);
                    SW.WriteLine("$nettype - " + hotspot.NetworkType);
                    SW.WriteLine("$speed - " + hotspot.Speed);
                    SW.WriteLine("$user - Lee");
                    SW.WriteLine("$entrydtm - " + DateTime.Now);
                    SW.Close();
                    return (DateTime.Now + " - Scanner: Writing to File [ " + hotspot.SSID + " ]\r\n");
                }
            }
            else
            {
                return (DateTime.Now + " - Scanner: Log file does not exist!\r\n");
            }
        }

        /// <summary>
        /// Save listview to log
        /// </summary>
        /// <param name="listView"></param>
        static public void saveListviewToLog(ListView listView)
        {
            HotSpot hotspot;
            string message;
            foreach (ListViewItem item in listView.Items)
            {
                hotspot = new HotSpot();
                hotspot.Latitude = item.SubItems[0].Text;
                hotspot.Longitude = item.SubItems[1].Text;
                hotspot.Mac = item.SubItems[2].Text;
                hotspot.SSID = item.SubItems[3].Text;
                hotspot.Channel = item.SubItems[4].Text;
                hotspot.Signal = item.SubItems[5].Text;
                hotspot.Authentication = item.SubItems[6].Text;
                hotspot.Encryption = item.SubItems[7].Text;
                hotspot.RadioType = item.SubItems[8].Text;
                hotspot.NetworkType = item.SubItems[9].Text;
                hotspot.Speed = item.SubItems[10].Text;

                message = wifiMapper.SendToFile(hotspot);

                //TODO: Update Log
            }
        }

        /// <summary>
        /// Save listivew to a site
        /// </summary>
        /// <param name="listView"></param>
        static public void saveListviewToSite(ListView listView)
        {
            HotSpot hotspot;
            string message;
            foreach (ListViewItem item in listView.Items)
            {
                hotspot = new HotSpot();
                hotspot.Latitude = item.SubItems[0].Text;
                hotspot.Longitude = item.SubItems[1].Text;
                hotspot.Mac = item.SubItems[2].Text;
                hotspot.SSID = item.SubItems[3].Text;
                hotspot.Channel = item.SubItems[4].Text;
                hotspot.Signal = item.SubItems[5].Text;
                hotspot.Authentication = item.SubItems[6].Text;
                hotspot.Encryption = item.SubItems[7].Text;
                hotspot.RadioType = item.SubItems[8].Text;
                hotspot.NetworkType = item.SubItems[9].Text;
                hotspot.Speed = item.SubItems[10].Text;

                message = SendToSite(hotspot);

                //TODO: Update Log
            }
        }
    }
}

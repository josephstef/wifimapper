using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WITSniff
{
    public class Utilities
    {

        public string[] convertCoords(string position)
        {
            string[] replace = { "\"", "N", "S", "E", "W" };

            for (int n = 0; n < replace.Length; n++)
                position = position.Replace(replace[n], string.Empty);

            position = position.Replace("\'", ":");
            position = position.Replace("°", ":");

            string[] coords = position.Split(' ');

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
            return coords;
        }

        public void test()
        {

        }
    }
}

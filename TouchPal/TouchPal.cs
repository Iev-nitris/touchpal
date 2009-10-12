/*
 * This file is part of the TouchPal project hosted on Google Code
 * (http://code.google.com/p/touchpal). See the accompanying license.txt file for 
 * applicable licenses.
 */

/*
 * (c) Copyright Craig Courtney 2009 All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Drawing;

namespace TouchPal
{

    static class TouchPal
    {
        public enum LOG_LEVEL {
            DEBUG=1,
            WARN,
            ERROR
        }

        private static Dictionary<string,LOG_LEVEL> logLevels = new Dictionary<string,LOG_LEVEL> {{"debug", LOG_LEVEL.DEBUG},
                                                                                                  {"warn", LOG_LEVEL.WARN},
                                                                                                  {"error", LOG_LEVEL.ERROR}};
        static LOG_LEVEL logLevel = LOG_LEVEL.WARN;
        static string logFilename = "TouchPal.log";
        static string touchPalDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TouchPal";
        static bool udp = true;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            int forceX = -1;
            int forceY = -1;

            string profile = "TouchPal.xml";

            foreach (string argument in args)
            {
                if (argument.StartsWith("-log=")) {
                    string level = argument.Substring(5).ToLower();
                    if (logLevels.ContainsKey(level))
                        logLevel = logLevels[level];
                }

                if (argument.StartsWith("-profile=")) {
                    profile = argument.Substring(9);
                }

                if (argument.StartsWith("-homedir="))
                {
                    touchPalDirectory = argument.Substring(9);
                }

                if (argument.StartsWith("-tcp"))
                {
                    udp = false;
                }

                if (argument.StartsWith("-udp"))
                {
                    udp = true;
                }

                if (argument.StartsWith("-x="))
                {
                    forceX = Convert.ToInt32(argument.Substring(3));
                    TouchPal.Debug("Forceing window x to " + argument.Substring(3));
                }

                if (argument.StartsWith("-y="))
                {
                    forceY = Convert.ToInt32(argument.Substring(3));
                    TouchPal.Debug("Forceing window y to " + argument.Substring(3));
                }
            }

            ResetEnvironment();

            INetConnection connection = null;
            if (udp)
                connection = new UDPNetConnection();
            else
                connection = new TCPNetConnection();

            IImageCache cache = new BasicImageCache();

            try {
                TouchPal.Debug("Parsing profile " + profile);
                CockpitXML.Cockpit co = ReadConfig(profile);

                TouchPal.Debug("Loading controls");
                ControlManager manager = new ControlManager(connection, cache, co.StartAction, co.ResetAction, co.Controls);

                connection.StartConnection();

                TouchPal.Debug("Setting up form");
                CockpitForm form = new CockpitForm(manager, cache, co.Layout, forceX, forceY);

                Application.Run(form);
            }
            catch (XmlSchemaException e)
            {
                TouchPal.Log(LOG_LEVEL.ERROR, "Error parsing profile at position " + e.LinePosition + " on line " + e.LineNumber + " {" + e.Message + "}"); 
            }
        }

        static private CockpitXML.Cockpit ReadConfig(string filename)
        {
            // Create an instance of the XmlSerializer class;
            // specify the type of object to be deserialized.
            XmlSerializer serializer = new XmlSerializer(typeof(CockpitXML.Cockpit));

            /* If the XML document has been altered with unknown 
            nodes or attributes, handle them with the 
            UnknownNode and UnknownAttribute events.*/
            serializer.UnknownNode += new
            XmlNodeEventHandler(serializer_UnknownNode);
            serializer.UnknownAttribute += new
            XmlAttributeEventHandler(serializer_UnknownAttribute);

            // A FileStream is needed to read the XML document.
            FileStream fs = new FileStream(filename, FileMode.Open);
            // Declare an object variable of the type to be deserialized.
            CockpitXML.Cockpit co;
            /* Use the Deserialize method to restore the object's state with
            data from the XML document. */
            co = (CockpitXML.Cockpit)serializer.Deserialize(fs);

            return co;
        }

        static private void serializer_UnknownNode
        (object sender, XmlNodeEventArgs e)
        {
            TouchPal.Log(LOG_LEVEL.WARN, "Unknown Node:" + e.Name + "\t" + e.Text);
        }

        static private void serializer_UnknownAttribute
        (object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            TouchPal.Log(LOG_LEVEL.WARN, "Unknown attribute " + attr.Name + "='" + attr.Value + "'");
        }

        static private void ResetEnvironment()
        {
            if (!Directory.Exists(touchPalDirectory))
                Directory.CreateDirectory(touchPalDirectory);

            Directory.SetCurrentDirectory(touchPalDirectory);

            if (File.Exists(touchPalDirectory + "\\" + logFilename))
                File.Delete(logFilename);
        }

        static public void Warn(string message)
        {
            Log(LOG_LEVEL.WARN, "WARN:" + message);
        }

        static public void Debug(string message)
        {
            Log(LOG_LEVEL.DEBUG, "DEBUG:" + message);
        }

        static public void Error(string message)
        {
            Log(LOG_LEVEL.ERROR, "ERROR:" + message);
        }

        static private void Log(LOG_LEVEL level, string message)
        {
            if (level >= logLevel)
            {
                System.IO.StreamWriter sw = System.IO.File.AppendText(logFilename);
                try
                {
                    string logLine = System.String.Format(
                        "{0:G}: {1}.", System.DateTime.Now, message);
                    sw.WriteLine(logLine);
                }
                finally
                {
                    sw.Close();
                }
            }

            if (logLevel == LOG_LEVEL.DEBUG)
                Console.WriteLine(message);
        }
    }
}

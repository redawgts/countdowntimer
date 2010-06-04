using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace CountdownTimer
{
    public class AppSettings
    {

        public AppSettings()
        {
            Location = new Point(100, 100);
            MessageText = "Times Up!";
            AudioFilename = "";
            AudioAlert = false;
            HideOnStart = false;
        }

        public Point Location { get; set; }
        public string MessageText { get; set; }
        public string AudioFilename { get; set; }
        public bool AudioAlert { get; set; }
        public bool HideOnStart { get; set; }


        #region Standard Code

        private static readonly string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
#if DEBUG
        private static readonly string LocalAppPath = Path.Combine(LocalAppData, Path.Combine(Application.CompanyName, Application.ProductName + " (DEBUG)"));
#else
        private static readonly string LocalAppPath = Path.Combine(LocalAppData, Path.Combine(Application.CompanyName, Application.ProductName));
#endif
        private static readonly string SettingFile = Path.Combine(LocalAppPath, "Settings.xml");


        public void Save()
        {
            if (!Directory.Exists(LocalAppPath))
                Directory.CreateDirectory(LocalAppPath);
            try
            {
                XmlSerializer serial = new XmlSerializer(typeof(AppSettings));
                using (FileStream fs = File.Create(SettingFile))
                {
                    serial.Serialize(fs, this);
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Error saving settings.");
            }
        }

        public static AppSettings Load()
        {
            try
            {
                XmlSerializer serial = new XmlSerializer(typeof(AppSettings));
                using (FileStream fs = File.OpenRead(SettingFile))
                {
                    return serial.Deserialize(fs) as AppSettings;
                }
            }
            catch (IOException)
            {
                return new AppSettings();
            }
        }

        #endregion

    }
}

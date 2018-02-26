using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.IO;

namespace TverDownloader
{
    public class Settings
    {
        public string path { get; set; }

        public Settings()
        {
        }

        public void LoadSettings()
        {
            Settings _set;
            try
            {
                StreamReader sr = new StreamReader(@"settings.json");
                string s = sr.ReadToEnd();
                sr.Close();

                _set = (Settings)new JavaScriptSerializer().Deserialize(s, typeof(Settings));

            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                _set = new Settings();
            }
            path = _set.path;
        }

        public void SaveSettings()
        {
            var json = new JavaScriptSerializer().Serialize(this);
            StreamWriter sw = new StreamWriter(@"settings.json", false, Encoding.GetEncoding("utf-8"));
            sw.Write(json);
            sw.Close();
        }
    }
}

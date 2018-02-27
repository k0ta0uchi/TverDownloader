using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TverDownloader
{
    class Informations
    {
        public string department { get; set; }
        public string date { get; set; }
        public string title { get; set; }
        public string subtitle { get; set; }
        public string url { get; set; }
        public const string PAUSE = "⏸";

        public Informations(string _department = "", string _date = "", string _title = "", string _subtitle = "", string _url = "")
        {
            department = _department;
            date = _date;
            title = _title;
            subtitle = _subtitle;
            url = _url;
        }

        public string[] ToArray()
        {
            string[] info = { PAUSE, department, date, title, subtitle, url };
            return info;
        }
    }
}

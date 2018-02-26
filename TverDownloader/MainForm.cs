using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TverDownloader
{
    public partial class MainForm : Form
    {
        private ClipboardViewer viewer;
        private M3u8Extractor ext;
        private Settings set;

        public MainForm()
        {
            viewer = new ClipboardViewer(this);
            viewer.ClipboardHandler += this.OnClipBoardChanged;

            set = new Settings();
            set.LoadSettings();

            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void OnClipBoardChanged(object sender, ClipboardEventArgs e)
        {
            ext = new M3u8Extractor(e.Text);

            if (ext.IsUrlValid())
            {
                Informations info = ext.GetInformations();

                if (info != null)
                {
                    bool isFOD = info.department.Equals("フジテレビ") ? true : false;
                    info.url = ext.GetM3u8Url(isFOD);

                    DownloadList.Items.Add(new ListViewItem(info.ToArray()));
                }
            }

            ext.DestroyDriver();
            ext = null;
        }

        private void DownloadList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ext.DestroyDriver();
            ext = null;
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm f = new SettingsForm(ref set);
            f.ShowDialog(this);
        }
    }
}

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TverDownloader
{
    public partial class MainForm : Form
    {
        private ClipboardViewer viewer;
        private M3u8Extractor extractor;
        private Settings set;
        private const string FOD = "フジテレビ";
        private const string RECORD = "⏺";
        private const string STOP = "⏹";

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

        private async void OnClipBoardChanged(object sender, ClipboardEventArgs e)
        {
            extractor = new M3u8Extractor(e.Text);

            if (extractor.IsUrlValid())
            {
                Informations info = null;

                await Task.Run(() =>
                {
                    info = extractor.GetInformations();

                    if (info != null)
                    {
                        bool isFOD = info.department.Equals(FOD) ? true : false;
                        info.url = extractor.GetM3u8Url(isFOD);
                    }
                });

                if (info != null)
                {
                    ListViewItem item = new ListViewItem(info.ToArray());
                    item.UseItemStyleForSubItems = false;
                    Font f = new Font(item.Font.Name, item.Font.Size + 8);
                    item.SubItems[0].Font = f;
                    item.SubItems[0].ForeColor = Color.Green;

                    DownloadList.Items.Add(item);
                }
            }

            extractor.DestroyDriver();
            extractor = null;
        }

        private void DownloadList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DownloadVideo(DownloadList.SelectedItems[0]);
        }

        private async void DownloadVideo(ListViewItem target)
        {
            if (Directory.Exists(set.path))
            {
                string dramaPath = Path.Combine(set.path, target.SubItems[3].Text);
                string videoPath = Path.Combine(dramaPath, target.SubItems[3].Text + " " + target.SubItems[4].Text + ".mp4");

                if (!Directory.Exists(dramaPath))
                {
                    Directory.CreateDirectory(dramaPath);
                }
                target.SubItems[0].Text = RECORD;
                target.SubItems[0].ForeColor = Color.Red;

                await Task.Run(() =>
                {
                   UseFfmpeg(target.SubItems[5].Text);

                   File.Move("video.mp4", videoPath);
                });
                
                target.SubItems[0].Text = STOP;
                target.SubItems[0].ForeColor = Color.Black;
            } else
            {    
                MessageBox.Show("There is no directory \"" + set.path + "\" exists.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void UseFfmpeg(string url)
        {
            string cmd = @"ffmpeg.exe";
            ProcessStartInfo psInfo = new ProcessStartInfo();
            psInfo.FileName = cmd;
            psInfo.Arguments = "-headers \"origin: https://tver.jp\" " +
                               "-headers \"accept-encoding: gzip, deflate, br\" " +
                               "-headers \"accept-language: ja,en-US;q=0.8,en;q=0.6\" " +
                               "-user_agent \"Mozilla / 5.0(Macintosh; Intel Mac OS X 10_12_6) AppleWebKit / " +
                               "537.36(KHTML, like Gecko) Chrome / 61.0.3163.100 Safari / 537.36\" " +
                               "-i \"" + url + "\" " +
                               "-codec copy " +
                               "-bsf:a aac_adtstoasc " +
                               "-movflags faststart " +
                               "video.mp4";
            psInfo.CreateNoWindow = true;
            psInfo.UseShellExecute = false;

            psInfo.RedirectStandardOutput = true;

            Process p = Process.Start(psInfo);
            string output = p.StandardOutput.ReadToEnd();
            output = output.Replace("\r\r\n", "\n");
            Debug.Write(output);
        }

        private void Main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            extractor.DestroyDriver();
            extractor = null;
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm f = new SettingsForm(ref set);
            f.ShowDialog(this);
        }
    }
}

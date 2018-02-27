using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        private bool isExtracting;
        private bool isDownloading;
        private Queue<string> extractUrls;
        private Queue<ListViewItem> downloadItems;
        private Process downloadProcess;
        private const string FOD = "フジテレビ";
        private const string RECORD = "⏺";
        private const string STOP = "⏹";
        private const string PAUSE = "⏸";

        public MainForm()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            viewer = new ClipboardViewer(this);
            viewer.ClipboardHandler += this.OnClipBoardChanged;

            set = new Settings();
            set.LoadSettings();

            isExtracting = false;
            isDownloading = false;
            extractUrls = new Queue<string>();
            downloadItems = new Queue<ListViewItem>();
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void startButton_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Point[] points =
            {
                new Point(6, 4),
                new Point(28, 16),
                new Point(6, 28),
                new Point(6, 4)
            };
            byte[] types =
            {
                (byte)PathPointType.Line,
                (byte)PathPointType.Line,
                (byte)PathPointType.Line,
                (byte)PathPointType.Line
            };
            GraphicsPath path = new GraphicsPath(points, types);
            g.FillPath(Brushes.LimeGreen, path);
        }

        private void stopButton_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Point[] points =
            {
                new Point(6, 6),
                new Point(26, 6),
                new Point(26, 26),
                new Point(6, 26),
                new Point(6, 6)
            };
            byte[] types =
            {
                (byte)PathPointType.Line,
                (byte)PathPointType.Line,
                (byte)PathPointType.Line,
                (byte)PathPointType.Line,
                (byte)PathPointType.Line
            };
            GraphicsPath path = new GraphicsPath(points, types);
            g.FillPath(Brushes.Red, path);
        }

        private void OnClipBoardChanged(object sender, ClipboardEventArgs e)
        {
            if (!isExtracting)
            {
                ExtractM3u8(e.Text);
            } else
            {
                extractUrls.Enqueue(e.Text);
            }
        }

        private async void ExtractM3u8(string url)
        {
            isExtracting = true;
            extractor = new M3u8Extractor(url);

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

            if (extractUrls.Count > 0)
            {
                ExtractM3u8(extractUrls.Dequeue());
            }

            isExtracting = false;
        }

        private void DownloadList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DownloadVideo(DownloadList.SelectedItems[0]);
        }

        private async void DownloadVideo(ListViewItem target)
        {
            bool isInterrupted = false;
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

                    try
                    {
                        File.Move("video.mp4", videoPath);
                    }
                    catch (FileNotFoundException e)
                    {
                        Console.WriteLine(e.Message);
                        isInterrupted = true;
                    }
                });

                if (!isInterrupted)
                {
                    target.SubItems[0].Text = STOP;
                    target.SubItems[0].ForeColor = Color.Black;
                } else
                {
                    target.SubItems[0].Text = PAUSE;
                    target.SubItems[0].ForeColor = Color.Green;
                }

                if (isDownloading && downloadItems.Count > 0)
                    DownloadVideo(downloadItems.Dequeue());
                else
                    isDownloading = false;
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
                               "-i \"" + url + "\" -y " +
                               "-codec copy " +
                               "-bsf:a aac_adtstoasc " +
                               "-movflags faststart " +
                               "video.mp4";
            psInfo.CreateNoWindow = true;
            psInfo.UseShellExecute = false;

            psInfo.RedirectStandardOutput = true;

            downloadProcess = Process.Start(psInfo);
            string output = downloadProcess.StandardOutput.ReadToEnd();
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

        private void startButton_Click(object sender, EventArgs e)
        {
            bool isAreadyDownloading = false;
            foreach(ListViewItem item in DownloadList.Items)
            {
                if (item.SubItems[0].Text.Equals(PAUSE))
                    downloadItems.Enqueue(item);
                else if (item.SubItems[0].Text.Equals(RECORD))
                    isAreadyDownloading = true;
            }

            isDownloading = downloadItems.Count > 0 ? true : false;
            if (!isAreadyDownloading && downloadItems.Count > 0)
                DownloadVideo(downloadItems.Dequeue());
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            try
            {
                downloadProcess.Kill();
            } catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }
            isDownloading = false;
        }
    }
}

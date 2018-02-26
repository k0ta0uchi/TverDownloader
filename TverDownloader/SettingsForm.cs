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
    public partial class SettingsForm : Form
    {
        private Settings set;
        public SettingsForm(ref Settings _set)
        {
            InitializeComponent();
            set = _set;

            path.Text = set.path;        }

        private void cancelSettings_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void okSettings_Click(object sender, EventArgs e)
        {
            set.SaveSettings();
            Close();
        }

        private void path_TextChanged(object sender, EventArgs e)
        {
            set.path = path.Text;
        }

        private void browse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() ==DialogResult.OK)
            {
                path.Text = fbd.SelectedPath;
                set.path = fbd.SelectedPath;
            }
        }
    }
}

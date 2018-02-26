namespace TverDownloader
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.path = new System.Windows.Forms.TextBox();
            this.browse = new System.Windows.Forms.Button();
            this.pathLabel = new System.Windows.Forms.Label();
            this.okSettings = new System.Windows.Forms.Button();
            this.cancelSettings = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // path
            // 
            this.path.Location = new System.Drawing.Point(12, 33);
            this.path.Name = "path";
            this.path.Size = new System.Drawing.Size(344, 19);
            this.path.TabIndex = 0;
            this.path.TextChanged += new System.EventHandler(this.path_TextChanged);
            // 
            // browse
            // 
            this.browse.Location = new System.Drawing.Point(362, 32);
            this.browse.Name = "browse";
            this.browse.Size = new System.Drawing.Size(33, 20);
            this.browse.TabIndex = 1;
            this.browse.Text = "...";
            this.browse.UseVisualStyleBackColor = true;
            this.browse.Click += new System.EventHandler(this.browse_Click);
            // 
            // pathLabel
            // 
            this.pathLabel.AutoSize = true;
            this.pathLabel.Location = new System.Drawing.Point(12, 15);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Size = new System.Drawing.Size(81, 12);
            this.pathLabel.TabIndex = 2;
            this.pathLabel.Text = "Download Path";
            // 
            // okSettings
            // 
            this.okSettings.Location = new System.Drawing.Point(231, 64);
            this.okSettings.Name = "okSettings";
            this.okSettings.Size = new System.Drawing.Size(75, 23);
            this.okSettings.TabIndex = 3;
            this.okSettings.Text = "OK";
            this.okSettings.UseVisualStyleBackColor = true;
            this.okSettings.Click += new System.EventHandler(this.okSettings_Click);
            // 
            // cancelSettings
            // 
            this.cancelSettings.Location = new System.Drawing.Point(320, 64);
            this.cancelSettings.Name = "cancelSettings";
            this.cancelSettings.Size = new System.Drawing.Size(75, 23);
            this.cancelSettings.TabIndex = 4;
            this.cancelSettings.Text = "Cancel";
            this.cancelSettings.UseVisualStyleBackColor = true;
            this.cancelSettings.Click += new System.EventHandler(this.cancelSettings_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 99);
            this.Controls.Add(this.cancelSettings);
            this.Controls.Add(this.okSettings);
            this.Controls.Add(this.pathLabel);
            this.Controls.Add(this.browse);
            this.Controls.Add(this.path);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox path;
        private System.Windows.Forms.Button browse;
        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.Button okSettings;
        private System.Windows.Forms.Button cancelSettings;
    }
}
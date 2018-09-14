namespace Spotify_Recorder
{
    partial class LogBox
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listView_log = new System.Windows.Forms.ListView();
            this.colIcon = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colText = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.chk_showErrors = new System.Windows.Forms.CheckBox();
            this.chk_showWarnings = new System.Windows.Forms.CheckBox();
            this.chk_showInfos = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btn_clearLog = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView_log
            // 
            this.listView_log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView_log.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colIcon,
            this.colTime,
            this.colText});
            this.listView_log.FullRowSelect = true;
            this.listView_log.Location = new System.Drawing.Point(3, 32);
            this.listView_log.Name = "listView_log";
            this.listView_log.Size = new System.Drawing.Size(508, 166);
            this.listView_log.TabIndex = 0;
            this.listView_log.UseCompatibleStateImageBehavior = false;
            this.listView_log.View = System.Windows.Forms.View.Details;
            // 
            // colIcon
            // 
            this.colIcon.Text = "Icon";
            // 
            // colTime
            // 
            this.colTime.Text = "Time";
            this.colTime.Width = 103;
            // 
            // colText
            // 
            this.colText.Text = "Text";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(-15, -15);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(80, 17);
            this.checkBox1.TabIndex = 2;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // chk_showErrors
            // 
            this.chk_showErrors.Appearance = System.Windows.Forms.Appearance.Button;
            this.chk_showErrors.AutoSize = true;
            this.chk_showErrors.Checked = true;
            this.chk_showErrors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_showErrors.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chk_showErrors.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.chk_showErrors.Location = new System.Drawing.Point(3, 3);
            this.chk_showErrors.Name = "chk_showErrors";
            this.chk_showErrors.Size = new System.Drawing.Size(92, 23);
            this.chk_showErrors.TabIndex = 3;
            this.chk_showErrors.Text = "      Show Errors";
            this.toolTip1.SetToolTip(this.chk_showErrors, "Show all error log entries.");
            this.chk_showErrors.UseVisualStyleBackColor = true;
            this.chk_showErrors.CheckedChanged += new System.EventHandler(this.chk_showErrors_CheckedChanged);
            // 
            // chk_showWarnings
            // 
            this.chk_showWarnings.Appearance = System.Windows.Forms.Appearance.Button;
            this.chk_showWarnings.AutoSize = true;
            this.chk_showWarnings.Checked = true;
            this.chk_showWarnings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_showWarnings.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chk_showWarnings.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.chk_showWarnings.Location = new System.Drawing.Point(101, 3);
            this.chk_showWarnings.Name = "chk_showWarnings";
            this.chk_showWarnings.Size = new System.Drawing.Size(110, 23);
            this.chk_showWarnings.TabIndex = 4;
            this.chk_showWarnings.Text = "      Show Warnings";
            this.toolTip1.SetToolTip(this.chk_showWarnings, "Show all warning log entries.");
            this.chk_showWarnings.UseVisualStyleBackColor = true;
            this.chk_showWarnings.CheckedChanged += new System.EventHandler(this.chk_showWarnings_CheckedChanged);
            // 
            // chk_showInfos
            // 
            this.chk_showInfos.Appearance = System.Windows.Forms.Appearance.Button;
            this.chk_showInfos.AutoSize = true;
            this.chk_showInfos.Checked = true;
            this.chk_showInfos.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_showInfos.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chk_showInfos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.chk_showInfos.Location = new System.Drawing.Point(217, 3);
            this.chk_showInfos.Name = "chk_showInfos";
            this.chk_showInfos.Size = new System.Drawing.Size(88, 23);
            this.chk_showInfos.TabIndex = 5;
            this.chk_showInfos.Text = "      Show Infos";
            this.toolTip1.SetToolTip(this.chk_showInfos, "Show all info log entries.");
            this.chk_showInfos.UseVisualStyleBackColor = true;
            this.chk_showInfos.CheckedChanged += new System.EventHandler(this.chk_showInfos_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.flowLayoutPanel1);
            this.panel1.Controls.Add(this.listView_log);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(514, 201);
            this.panel1.TabIndex = 6;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.chk_showErrors);
            this.flowLayoutPanel1.Controls.Add(this.chk_showWarnings);
            this.flowLayoutPanel1.Controls.Add(this.chk_showInfos);
            this.flowLayoutPanel1.Controls.Add(this.btn_clearLog);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(514, 31);
            this.flowLayoutPanel1.TabIndex = 6;
            // 
            // btn_clearLog
            // 
            this.btn_clearLog.Image = global::Spotify_Recorder.Properties.Resources.Clear_small;
            this.btn_clearLog.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_clearLog.Location = new System.Drawing.Point(311, 3);
            this.btn_clearLog.Name = "btn_clearLog";
            this.btn_clearLog.Size = new System.Drawing.Size(86, 23);
            this.btn_clearLog.TabIndex = 6;
            this.btn_clearLog.Text = "Clear Log";
            this.btn_clearLog.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.btn_clearLog, "Clear all log entries.");
            this.btn_clearLog.UseVisualStyleBackColor = true;
            this.btn_clearLog.Click += new System.EventHandler(this.btn_clearLog_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 32767;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ReshowDelay = 100;
            // 
            // LogBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.checkBox1);
            this.Name = "LogBox";
            this.Size = new System.Drawing.Size(514, 201);
            this.Load += new System.EventHandler(this.LogBox_Load);
            this.panel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView_log;
        private System.Windows.Forms.ColumnHeader colIcon;
        private System.Windows.Forms.ColumnHeader colTime;
        private System.Windows.Forms.ColumnHeader colText;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox chk_showErrors;
        private System.Windows.Forms.CheckBox chk_showWarnings;
        private System.Windows.Forms.CheckBox chk_showInfos;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btn_clearLog;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

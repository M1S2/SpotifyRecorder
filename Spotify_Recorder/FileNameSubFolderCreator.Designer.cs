namespace Spotify_Recorder
{
    partial class FileNameSubFolderCreator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileNameSubFolderCreator));
            this.txt_filename_prototype = new System.Windows.Forms.TextBox();
            this.btn_interpret = new System.Windows.Forms.Button();
            this.btn_title = new System.Windows.Forms.Button();
            this.btn_album = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lbl_sample_file_name = new System.Windows.Forms.Label();
            this.btn_ok = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.btn_directory_separator = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txt_filename_prototype
            // 
            this.txt_filename_prototype.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_filename_prototype.Location = new System.Drawing.Point(12, 12);
            this.txt_filename_prototype.Name = "txt_filename_prototype";
            this.txt_filename_prototype.Size = new System.Drawing.Size(478, 20);
            this.txt_filename_prototype.TabIndex = 0;
            this.txt_filename_prototype.TextChanged += new System.EventHandler(this.txt_filename_prototype_TextChanged);
            // 
            // btn_interpret
            // 
            this.btn_interpret.Location = new System.Drawing.Point(93, 48);
            this.btn_interpret.Name = "btn_interpret";
            this.btn_interpret.Size = new System.Drawing.Size(75, 23);
            this.btn_interpret.TabIndex = 2;
            this.btn_interpret.Text = "{Interpret}";
            this.btn_interpret.UseVisualStyleBackColor = true;
            this.btn_interpret.Click += new System.EventHandler(this.btn_interpret_Click);
            // 
            // btn_title
            // 
            this.btn_title.Location = new System.Drawing.Point(12, 48);
            this.btn_title.Name = "btn_title";
            this.btn_title.Size = new System.Drawing.Size(75, 23);
            this.btn_title.TabIndex = 3;
            this.btn_title.Text = "{Title}";
            this.btn_title.UseVisualStyleBackColor = true;
            this.btn_title.Click += new System.EventHandler(this.btn_title_Click);
            // 
            // btn_album
            // 
            this.btn_album.Location = new System.Drawing.Point(174, 48);
            this.btn_album.Name = "btn_album";
            this.btn_album.Size = new System.Drawing.Size(75, 23);
            this.btn_album.TabIndex = 4;
            this.btn_album.Text = "{Album}";
            this.btn_album.UseVisualStyleBackColor = true;
            this.btn_album.Click += new System.EventHandler(this.btn_album_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Sample file name:";
            // 
            // lbl_sample_file_name
            // 
            this.lbl_sample_file_name.AutoSize = true;
            this.lbl_sample_file_name.Location = new System.Drawing.Point(108, 86);
            this.lbl_sample_file_name.Name = "lbl_sample_file_name";
            this.lbl_sample_file_name.Size = new System.Drawing.Size(16, 13);
            this.lbl_sample_file_name.TabIndex = 6;
            this.lbl_sample_file_name.Text = "---";
            // 
            // btn_ok
            // 
            this.btn_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_ok.Location = new System.Drawing.Point(444, 107);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(46, 23);
            this.btn_ok.TabIndex = 8;
            this.btn_ok.Text = "OK";
            this.btn_ok.UseVisualStyleBackColor = true;
            // 
            // btn_cancel
            // 
            this.btn_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_cancel.Location = new System.Drawing.Point(383, 107);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(55, 23);
            this.btn_cancel.TabIndex = 7;
            this.btn_cancel.Text = "Cancel";
            this.btn_cancel.UseVisualStyleBackColor = true;
            // 
            // btn_directory_separator
            // 
            this.btn_directory_separator.Location = new System.Drawing.Point(255, 48);
            this.btn_directory_separator.Name = "btn_directory_separator";
            this.btn_directory_separator.Size = new System.Drawing.Size(48, 23);
            this.btn_directory_separator.TabIndex = 9;
            this.btn_directory_separator.Text = "\\";
            this.btn_directory_separator.UseVisualStyleBackColor = true;
            this.btn_directory_separator.Click += new System.EventHandler(this.btn_directory_separator_Click);
            // 
            // FileNameSubFolderCreator
            // 
            this.AcceptButton = this.btn_ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_cancel;
            this.ClientSize = new System.Drawing.Size(502, 142);
            this.Controls.Add(this.btn_directory_separator);
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.lbl_sample_file_name);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btn_album);
            this.Controls.Add(this.btn_title);
            this.Controls.Add(this.btn_interpret);
            this.Controls.Add(this.txt_filename_prototype);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FileNameSubFolderCreator";
            this.Text = "Filename & Subfolder Creator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_filename_prototype;
        private System.Windows.Forms.Button btn_interpret;
        private System.Windows.Forms.Button btn_title;
        private System.Windows.Forms.Button btn_album;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbl_sample_file_name;
        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.Button btn_directory_separator;
    }
}
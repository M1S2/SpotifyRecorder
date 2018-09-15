namespace Spotify_Recorder
{
    partial class SpotifyRecorder
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

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpotifyRecorder));
            this.lbl_track_duration = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_change_output_path = new System.Windows.Forms.Button();
            this.txt_output_path = new System.Windows.Forms.TextBox();
            this.btn_arm_disarm = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbl_track_album = new System.Windows.Forms.Label();
            this.lbl_track_title = new System.Windows.Forms.Label();
            this.lbl_track_interpret = new System.Windows.Forms.Label();
            this.lbl_sp_volume = new System.Windows.Forms.Label();
            this.lbl_sp_isPlaying = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lbl_track_time = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.pic_album_art = new System.Windows.Forms.PictureBox();
            this.prog_track_time = new System.Windows.Forms.ProgressBar();
            this.label10 = new System.Windows.Forms.Label();
            this.lbl_isRecording = new System.Windows.Forms.Label();
            this.chk_output_wav = new System.Windows.Forms.CheckBox();
            this.chk_output_mp3 = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pic_recordState = new System.Windows.Forms.PictureBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cmb_fileExistMode = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txt_filename_prototype = new System.Windows.Forms.TextBox();
            this.btn_create_file_name_prototype = new System.Windows.Forms.Button();
            this.lbl_record_file_path = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_sp_connect = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDropDownButton_recorder_ctrl = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripButton_recorder_start_manually = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton_recorder_stop_manually = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDropDownButton_spotify_ctrl = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripButton_sp_start = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton_sp_pause = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton_sp_skip = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton_sp_previous = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_sp_update_infos = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.hilfeToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.logBox1 = new Spotify_Recorder.LogBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_album_art)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_recordState)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_track_duration
            // 
            this.lbl_track_duration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_track_duration.AutoSize = true;
            this.lbl_track_duration.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_track_duration.Location = new System.Drawing.Point(803, 343);
            this.lbl_track_duration.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_track_duration.Name = "lbl_track_duration";
            this.lbl_track_duration.Size = new System.Drawing.Size(44, 17);
            this.lbl_track_duration.TabIndex = 36;
            this.lbl_track_duration.Text = "00:00";
            this.toolTip1.SetToolTip(this.lbl_track_duration, "Complete track time.");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(5, 30);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(125, 17);
            this.label4.TabIndex = 30;
            this.label4.Text = "Record base path:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(5, 91);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 17);
            this.label3.TabIndex = 29;
            this.label3.Text = "Interpret:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(5, 125);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 17);
            this.label2.TabIndex = 28;
            this.label2.Text = "Title:";
            // 
            // btn_change_output_path
            // 
            this.btn_change_output_path.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_change_output_path.Location = new System.Drawing.Point(804, 28);
            this.btn_change_output_path.Margin = new System.Windows.Forms.Padding(2);
            this.btn_change_output_path.Name = "btn_change_output_path";
            this.btn_change_output_path.Size = new System.Drawing.Size(43, 23);
            this.btn_change_output_path.TabIndex = 23;
            this.btn_change_output_path.Text = "...";
            this.toolTip1.SetToolTip(this.btn_change_output_path, "Select the record base path.");
            this.btn_change_output_path.UseVisualStyleBackColor = true;
            this.btn_change_output_path.Click += new System.EventHandler(this.btn_change_output_path_Click);
            // 
            // txt_output_path
            // 
            this.txt_output_path.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_output_path.Location = new System.Drawing.Point(146, 28);
            this.txt_output_path.Margin = new System.Windows.Forms.Padding(2);
            this.txt_output_path.Name = "txt_output_path";
            this.txt_output_path.Size = new System.Drawing.Size(654, 23);
            this.txt_output_path.TabIndex = 21;
            this.toolTip1.SetToolTip(this.txt_output_path, "The base path, where records are saved. See the file name prototype for further i" +
        "nformation.");
            this.txt_output_path.TextChanged += new System.EventHandler(this.txt_output_path_TextChanged);
            this.txt_output_path.Leave += new System.EventHandler(this.txt_output_path_Leave);
            // 
            // btn_arm_disarm
            // 
            this.btn_arm_disarm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_arm_disarm.BackColor = System.Drawing.SystemColors.Control;
            this.btn_arm_disarm.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_arm_disarm.ImageKey = "(Keine)";
            this.btn_arm_disarm.Location = new System.Drawing.Point(714, 162);
            this.btn_arm_disarm.Margin = new System.Windows.Forms.Padding(2);
            this.btn_arm_disarm.Name = "btn_arm_disarm";
            this.btn_arm_disarm.Size = new System.Drawing.Size(133, 38);
            this.btn_arm_disarm.TabIndex = 20;
            this.btn_arm_disarm.Text = "Arm recorder";
            this.toolTip1.SetToolTip(this.btn_arm_disarm, resources.GetString("btn_arm_disarm.ToolTip"));
            this.btn_arm_disarm.UseVisualStyleBackColor = false;
            this.btn_arm_disarm.Click += new System.EventHandler(this.btn_arm_disarm_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(5, 84);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 17);
            this.label1.TabIndex = 41;
            this.label1.Text = "Output type:";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.lbl_track_album);
            this.groupBox2.Controls.Add(this.lbl_track_title);
            this.groupBox2.Controls.Add(this.lbl_track_interpret);
            this.groupBox2.Controls.Add(this.lbl_sp_volume);
            this.groupBox2.Controls.Add(this.lbl_sp_isPlaying);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.lbl_track_time);
            this.groupBox2.Controls.Add(this.lbl_track_duration);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.pic_album_art);
            this.groupBox2.Controls.Add(this.prog_track_time);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(4, 221);
            this.groupBox2.MinimumSize = new System.Drawing.Size(286, 211);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(853, 366);
            this.groupBox2.TabIndex = 43;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Spotify Info / Control";
            // 
            // lbl_track_album
            // 
            this.lbl_track_album.AutoSize = true;
            this.lbl_track_album.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_track_album.Location = new System.Drawing.Point(92, 159);
            this.lbl_track_album.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_track_album.Name = "lbl_track_album";
            this.lbl_track_album.Size = new System.Drawing.Size(23, 17);
            this.lbl_track_album.TabIndex = 47;
            this.lbl_track_album.Text = "---";
            this.toolTip1.SetToolTip(this.lbl_track_album, "Track album name");
            // 
            // lbl_track_title
            // 
            this.lbl_track_title.AutoSize = true;
            this.lbl_track_title.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_track_title.Location = new System.Drawing.Point(92, 125);
            this.lbl_track_title.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_track_title.Name = "lbl_track_title";
            this.lbl_track_title.Size = new System.Drawing.Size(23, 17);
            this.lbl_track_title.TabIndex = 37;
            this.lbl_track_title.Text = "---";
            this.toolTip1.SetToolTip(this.lbl_track_title, "Track title");
            // 
            // lbl_track_interpret
            // 
            this.lbl_track_interpret.AutoSize = true;
            this.lbl_track_interpret.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_track_interpret.Location = new System.Drawing.Point(92, 91);
            this.lbl_track_interpret.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_track_interpret.Name = "lbl_track_interpret";
            this.lbl_track_interpret.Size = new System.Drawing.Size(23, 17);
            this.lbl_track_interpret.TabIndex = 38;
            this.lbl_track_interpret.Text = "---";
            this.toolTip1.SetToolTip(this.lbl_track_interpret, "Track interpret");
            // 
            // lbl_sp_volume
            // 
            this.lbl_sp_volume.AutoSize = true;
            this.lbl_sp_volume.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_sp_volume.Location = new System.Drawing.Point(92, 23);
            this.lbl_sp_volume.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_sp_volume.Name = "lbl_sp_volume";
            this.lbl_sp_volume.Size = new System.Drawing.Size(23, 17);
            this.lbl_sp_volume.TabIndex = 33;
            this.lbl_sp_volume.Text = "---";
            this.toolTip1.SetToolTip(this.lbl_sp_volume, "Spotify volume between 0% and 100%");
            // 
            // lbl_sp_isPlaying
            // 
            this.lbl_sp_isPlaying.AutoSize = true;
            this.lbl_sp_isPlaying.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_sp_isPlaying.Location = new System.Drawing.Point(92, 57);
            this.lbl_sp_isPlaying.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_sp_isPlaying.Name = "lbl_sp_isPlaying";
            this.lbl_sp_isPlaying.Size = new System.Drawing.Size(23, 17);
            this.lbl_sp_isPlaying.TabIndex = 32;
            this.lbl_sp_isPlaying.Text = "---";
            this.toolTip1.SetToolTip(this.lbl_sp_isPlaying, "Is spotify playing?");
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(5, 159);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(51, 17);
            this.label12.TabIndex = 43;
            this.label12.Text = "Album:";
            // 
            // lbl_track_time
            // 
            this.lbl_track_time.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_track_time.AutoSize = true;
            this.lbl_track_time.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_track_time.Location = new System.Drawing.Point(5, 343);
            this.lbl_track_time.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_track_time.Name = "lbl_track_time";
            this.lbl_track_time.Size = new System.Drawing.Size(44, 17);
            this.lbl_track_time.TabIndex = 41;
            this.lbl_track_time.Text = "00:00";
            this.toolTip1.SetToolTip(this.lbl_track_time, "Current track time.");
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(5, 57);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 17);
            this.label6.TabIndex = 31;
            this.label6.Text = "IsPlaying:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(5, 23);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 17);
            this.label5.TabIndex = 30;
            this.label5.Text = "Volume:";
            // 
            // pic_album_art
            // 
            this.pic_album_art.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pic_album_art.Location = new System.Drawing.Point(261, 22);
            this.pic_album_art.Name = "pic_album_art";
            this.pic_album_art.Size = new System.Drawing.Size(586, 309);
            this.pic_album_art.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pic_album_art.TabIndex = 42;
            this.pic_album_art.TabStop = false;
            this.toolTip1.SetToolTip(this.pic_album_art, "Album art. Double click to show a bigger picture.");
            this.pic_album_art.DoubleClick += new System.EventHandler(this.pic_album_art_DoubleClick);
            // 
            // prog_track_time
            // 
            this.prog_track_time.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prog_track_time.Location = new System.Drawing.Point(54, 337);
            this.prog_track_time.Name = "prog_track_time";
            this.prog_track_time.Size = new System.Drawing.Size(744, 23);
            this.prog_track_time.TabIndex = 48;
            this.toolTip1.SetToolTip(this.prog_track_time, "Track time");
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(5, 173);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(93, 17);
            this.label10.TabIndex = 45;
            this.label10.Text = "Record state:";
            // 
            // lbl_isRecording
            // 
            this.lbl_isRecording.AutoSize = true;
            this.lbl_isRecording.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_isRecording.Location = new System.Drawing.Point(148, 173);
            this.lbl_isRecording.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_isRecording.Name = "lbl_isRecording";
            this.lbl_isRecording.Size = new System.Drawing.Size(23, 17);
            this.lbl_isRecording.TabIndex = 46;
            this.lbl_isRecording.Text = "---";
            this.toolTip1.SetToolTip(this.lbl_isRecording, "Recorder state. This could be one of the following:\r\n- RECORDING\r\n- PAUSED\r\n- STO" +
        "PPED\r\n- NORMALIZING_WAV\r\n- CONVERTING_WAV_TO_MP3\r\n- ADDING_TAGS");
            // 
            // chk_output_wav
            // 
            this.chk_output_wav.AutoSize = true;
            this.chk_output_wav.Location = new System.Drawing.Point(146, 83);
            this.chk_output_wav.Name = "chk_output_wav";
            this.chk_output_wav.Size = new System.Drawing.Size(58, 21);
            this.chk_output_wav.TabIndex = 49;
            this.chk_output_wav.Text = "WAV";
            this.toolTip1.SetToolTip(this.chk_output_wav, "Create a .wav file of the record");
            this.chk_output_wav.UseVisualStyleBackColor = true;
            this.chk_output_wav.CheckedChanged += new System.EventHandler(this.chk_output_wav_CheckedChanged);
            // 
            // chk_output_mp3
            // 
            this.chk_output_mp3.AutoSize = true;
            this.chk_output_mp3.Checked = true;
            this.chk_output_mp3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_output_mp3.Location = new System.Drawing.Point(210, 83);
            this.chk_output_mp3.Name = "chk_output_mp3";
            this.chk_output_mp3.Size = new System.Drawing.Size(55, 21);
            this.chk_output_mp3.TabIndex = 50;
            this.chk_output_mp3.Text = "MP3";
            this.toolTip1.SetToolTip(this.chk_output_mp3, "Create a .mp3 file of the record");
            this.chk_output_mp3.UseVisualStyleBackColor = true;
            this.chk_output_mp3.CheckedChanged += new System.EventHandler(this.chk_output_mp3_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.pic_recordState);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.cmb_fileExistMode);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.txt_filename_prototype);
            this.groupBox1.Controls.Add(this.btn_create_file_name_prototype);
            this.groupBox1.Controls.Add(this.chk_output_wav);
            this.groupBox1.Controls.Add(this.lbl_record_file_path);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.chk_output_mp3);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.btn_arm_disarm);
            this.groupBox1.Controls.Add(this.txt_output_path);
            this.groupBox1.Controls.Add(this.btn_change_output_path);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.lbl_isRecording);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(853, 212);
            this.groupBox1.TabIndex = 52;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Recorder Control";
            // 
            // pic_recordState
            // 
            this.pic_recordState.Location = new System.Drawing.Point(103, 166);
            this.pic_recordState.Name = "pic_recordState";
            this.pic_recordState.Size = new System.Drawing.Size(40, 40);
            this.pic_recordState.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pic_recordState.TabIndex = 64;
            this.pic_recordState.TabStop = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(5, 113);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(131, 17);
            this.label9.TabIndex = 63;
            this.label9.Text = "If file already exists:";
            // 
            // cmb_fileExistMode
            // 
            this.cmb_fileExistMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_fileExistMode.FormattingEnabled = true;
            this.cmb_fileExistMode.Location = new System.Drawing.Point(146, 110);
            this.cmb_fileExistMode.Name = "cmb_fileExistMode";
            this.cmb_fileExistMode.Size = new System.Drawing.Size(179, 24);
            this.cmb_fileExistMode.TabIndex = 62;
            this.toolTip1.SetToolTip(this.cmb_fileExistMode, resources.GetString("cmb_fileExistMode.ToolTip"));
            this.cmb_fileExistMode.SelectedIndexChanged += new System.EventHandler(this.cmb_fileExistMode_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(5, 58);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(137, 17);
            this.label13.TabIndex = 61;
            this.label13.Text = "File name prototype:";
            // 
            // txt_filename_prototype
            // 
            this.txt_filename_prototype.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_filename_prototype.Location = new System.Drawing.Point(146, 55);
            this.txt_filename_prototype.Margin = new System.Windows.Forms.Padding(2);
            this.txt_filename_prototype.Name = "txt_filename_prototype";
            this.txt_filename_prototype.Size = new System.Drawing.Size(654, 23);
            this.txt_filename_prototype.TabIndex = 60;
            this.toolTip1.SetToolTip(this.txt_filename_prototype, resources.GetString("txt_filename_prototype.ToolTip"));
            this.txt_filename_prototype.TextChanged += new System.EventHandler(this.txt_filename_prototype_TextChanged);
            // 
            // btn_create_file_name_prototype
            // 
            this.btn_create_file_name_prototype.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_create_file_name_prototype.Location = new System.Drawing.Point(804, 55);
            this.btn_create_file_name_prototype.Name = "btn_create_file_name_prototype";
            this.btn_create_file_name_prototype.Size = new System.Drawing.Size(43, 23);
            this.btn_create_file_name_prototype.TabIndex = 59;
            this.btn_create_file_name_prototype.Text = "...";
            this.toolTip1.SetToolTip(this.btn_create_file_name_prototype, "Edit the file name prototype.");
            this.btn_create_file_name_prototype.UseVisualStyleBackColor = true;
            this.btn_create_file_name_prototype.Click += new System.EventHandler(this.btn_create_file_name_prototype_Click);
            // 
            // lbl_record_file_path
            // 
            this.lbl_record_file_path.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_record_file_path.AutoEllipsis = true;
            this.lbl_record_file_path.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_record_file_path.Location = new System.Drawing.Point(121, 143);
            this.lbl_record_file_path.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_record_file_path.Name = "lbl_record_file_path";
            this.lbl_record_file_path.Size = new System.Drawing.Size(726, 17);
            this.lbl_record_file_path.TabIndex = 58;
            this.lbl_record_file_path.Text = "---";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(5, 143);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(112, 17);
            this.label11.TabIndex = 57;
            this.label11.Text = "Record file path:";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_sp_connect,
            this.toolStripSeparator3,
            this.toolStripDropDownButton_recorder_ctrl,
            this.toolStripSeparator4,
            this.toolStripDropDownButton_spotify_ctrl,
            this.toolStripSeparator5,
            this.hilfeToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(886, 25);
            this.toolStrip1.TabIndex = 53;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton_sp_connect
            // 
            this.toolStripButton_sp_connect.Image = global::Spotify_Recorder.Properties.Resources.Connect;
            this.toolStripButton_sp_connect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_sp_connect.Name = "toolStripButton_sp_connect";
            this.toolStripButton_sp_connect.Size = new System.Drawing.Size(126, 22);
            this.toolStripButton_sp_connect.Text = "Connect to Spotify";
            this.toolStripButton_sp_connect.Click += new System.EventHandler(this.toolStripButton_sp_connect_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripDropDownButton_recorder_ctrl
            // 
            this.toolStripDropDownButton_recorder_ctrl.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_recorder_start_manually,
            this.toolStripButton_recorder_stop_manually});
            this.toolStripDropDownButton_recorder_ctrl.Enabled = false;
            this.toolStripDropDownButton_recorder_ctrl.Image = global::Spotify_Recorder.Properties.Resources.Record;
            this.toolStripDropDownButton_recorder_ctrl.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton_recorder_ctrl.Name = "toolStripDropDownButton_recorder_ctrl";
            this.toolStripDropDownButton_recorder_ctrl.Size = new System.Drawing.Size(126, 22);
            this.toolStripDropDownButton_recorder_ctrl.Text = "Recorder Control";
            // 
            // toolStripButton_recorder_start_manually
            // 
            this.toolStripButton_recorder_start_manually.Image = global::Spotify_Recorder.Properties.Resources.Play;
            this.toolStripButton_recorder_start_manually.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_recorder_start_manually.Name = "toolStripButton_recorder_start_manually";
            this.toolStripButton_recorder_start_manually.Size = new System.Drawing.Size(211, 36);
            this.toolStripButton_recorder_start_manually.Text = "Start recorder manually";
            this.toolStripButton_recorder_start_manually.ToolTipText = "Start the recorder manually.";
            this.toolStripButton_recorder_start_manually.Click += new System.EventHandler(this.toolStripButton_recorder_start_manually_Click);
            // 
            // toolStripButton_recorder_stop_manually
            // 
            this.toolStripButton_recorder_stop_manually.Image = global::Spotify_Recorder.Properties.Resources.Stop;
            this.toolStripButton_recorder_stop_manually.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_recorder_stop_manually.Name = "toolStripButton_recorder_stop_manually";
            this.toolStripButton_recorder_stop_manually.Size = new System.Drawing.Size(211, 36);
            this.toolStripButton_recorder_stop_manually.Text = "Stop recorder manually";
            this.toolStripButton_recorder_stop_manually.ToolTipText = "Stop the recorder manually.";
            this.toolStripButton_recorder_stop_manually.Click += new System.EventHandler(this.toolStripButton_recorder_stop_manually_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripDropDownButton_spotify_ctrl
            // 
            this.toolStripDropDownButton_spotify_ctrl.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_sp_start,
            this.toolStripButton_sp_pause,
            this.toolStripButton_sp_skip,
            this.toolStripButton_sp_previous,
            this.toolStripMenuItem2,
            this.toolStripButton_sp_update_infos});
            this.toolStripDropDownButton_spotify_ctrl.Enabled = false;
            this.toolStripDropDownButton_spotify_ctrl.Image = global::Spotify_Recorder.Properties.Resources.Spotify;
            this.toolStripDropDownButton_spotify_ctrl.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton_spotify_ctrl.Name = "toolStripDropDownButton_spotify_ctrl";
            this.toolStripDropDownButton_spotify_ctrl.Size = new System.Drawing.Size(116, 22);
            this.toolStripDropDownButton_spotify_ctrl.Text = "Spotify Control";
            // 
            // toolStripButton_sp_start
            // 
            this.toolStripButton_sp_start.Image = global::Spotify_Recorder.Properties.Resources.Play;
            this.toolStripButton_sp_start.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_sp_start.Name = "toolStripButton_sp_start";
            this.toolStripButton_sp_start.Size = new System.Drawing.Size(173, 36);
            this.toolStripButton_sp_start.Text = "Spotify Start";
            this.toolStripButton_sp_start.ToolTipText = "Start Spotify playback";
            this.toolStripButton_sp_start.Click += new System.EventHandler(this.toolStripButton_sp_start_Click);
            // 
            // toolStripButton_sp_pause
            // 
            this.toolStripButton_sp_pause.Image = global::Spotify_Recorder.Properties.Resources.Pause;
            this.toolStripButton_sp_pause.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_sp_pause.Name = "toolStripButton_sp_pause";
            this.toolStripButton_sp_pause.Size = new System.Drawing.Size(173, 36);
            this.toolStripButton_sp_pause.Text = "Spotify Pause";
            this.toolStripButton_sp_pause.ToolTipText = "Pause Spotify playback";
            this.toolStripButton_sp_pause.Click += new System.EventHandler(this.toolStripButton_sp_pause_Click);
            // 
            // toolStripButton_sp_skip
            // 
            this.toolStripButton_sp_skip.Image = global::Spotify_Recorder.Properties.Resources.Skip;
            this.toolStripButton_sp_skip.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_sp_skip.Name = "toolStripButton_sp_skip";
            this.toolStripButton_sp_skip.Size = new System.Drawing.Size(173, 36);
            this.toolStripButton_sp_skip.Text = "Spotify Skip";
            this.toolStripButton_sp_skip.ToolTipText = "Skip Spotify track";
            this.toolStripButton_sp_skip.Click += new System.EventHandler(this.toolStripButton_sp_skip_Click);
            // 
            // toolStripButton_sp_previous
            // 
            this.toolStripButton_sp_previous.Image = global::Spotify_Recorder.Properties.Resources.Previous;
            this.toolStripButton_sp_previous.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_sp_previous.Name = "toolStripButton_sp_previous";
            this.toolStripButton_sp_previous.Size = new System.Drawing.Size(173, 36);
            this.toolStripButton_sp_previous.Text = "Spotify Previous";
            this.toolStripButton_sp_previous.ToolTipText = "Go to previous Spotify track";
            this.toolStripButton_sp_previous.Click += new System.EventHandler(this.toolStripButton_sp_previous_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(170, 6);
            // 
            // toolStripButton_sp_update_infos
            // 
            this.toolStripButton_sp_update_infos.Image = global::Spotify_Recorder.Properties.Resources.Update;
            this.toolStripButton_sp_update_infos.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_sp_update_infos.Name = "toolStripButton_sp_update_infos";
            this.toolStripButton_sp_update_infos.Size = new System.Drawing.Size(173, 36);
            this.toolStripButton_sp_update_infos.Text = "Update Infos";
            this.toolStripButton_sp_update_infos.ToolTipText = "Update Spotify infos";
            this.toolStripButton_sp_update_infos.Click += new System.EventHandler(this.toolStripButton_sp_update_infos_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // hilfeToolStripButton
            // 
            this.hilfeToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.hilfeToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("hilfeToolStripButton.Image")));
            this.hilfeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.hilfeToolStripButton.Name = "hilfeToolStripButton";
            this.hilfeToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.hilfeToolStripButton.Text = "Hi&lfe";
            this.hilfeToolStripButton.ToolTipText = "Help";
            this.hilfeToolStripButton.Click += new System.EventHandler(this.hilfeToolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(191, 6);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.logBox1);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(854, 212);
            this.groupBox3.TabIndex = 49;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Log";
            // 
            // logBox1
            // 
            this.logBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logBox1.Location = new System.Drawing.Point(1, 23);
            this.logBox1.Margin = new System.Windows.Forms.Padding(4);
            this.logBox1.Name = "logBox1";
            this.logBox1.ShowErrors = true;
            this.logBox1.ShowInfos = true;
            this.logBox1.ShowWarnings = true;
            this.logBox1.Size = new System.Drawing.Size(847, 182);
            this.logBox1.TabIndex = 49;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(12, 28);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.AutoScroll = true;
            this.splitContainer1.Panel1.AutoScrollMinSize = new System.Drawing.Size(286, 460);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox3);
            this.splitContainer1.Size = new System.Drawing.Size(864, 820);
            this.splitContainer1.SplitterDistance = 594;
            this.splitContainer1.TabIndex = 54;
            this.toolTip1.SetToolTip(this.splitContainer1, "Double click to reset log area size.");
            this.splitContainer1.DoubleClick += new System.EventHandler(this.splitContainer1_DoubleClick);
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 32767;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ReshowDelay = 100;
            // 
            // SpotifyRecorder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 860);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(480, 400);
            this.Name = "SpotifyRecorder";
            this.Text = "Spotify Recorder";
            this.Load += new System.EventHandler(this.SpotifyRecorder_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_album_art)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_recordState)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lbl_track_duration;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_change_output_path;
        private System.Windows.Forms.TextBox txt_output_path;
        private System.Windows.Forms.Button btn_arm_disarm;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbl_sp_volume;
        private System.Windows.Forms.Label lbl_sp_isPlaying;
        private System.Windows.Forms.Label lbl_track_interpret;
        private System.Windows.Forms.Label lbl_track_title;
        private System.Windows.Forms.Label lbl_track_time;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lbl_isRecording;
        private System.Windows.Forms.CheckBox chk_output_wav;
        private System.Windows.Forms.CheckBox chk_output_mp3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pic_album_art;
        private System.Windows.Forms.Label lbl_record_file_path;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lbl_track_album;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btn_create_file_name_prototype;
        private System.Windows.Forms.TextBox txt_filename_prototype;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ProgressBar prog_track_time;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmb_fileExistMode;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton_sp_connect;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton_recorder_ctrl;
        private System.Windows.Forms.ToolStripMenuItem toolStripButton_recorder_start_manually;
        private System.Windows.Forms.ToolStripMenuItem toolStripButton_recorder_stop_manually;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton_spotify_ctrl;
        private System.Windows.Forms.ToolStripMenuItem toolStripButton_sp_start;
        private System.Windows.Forms.ToolStripMenuItem toolStripButton_sp_pause;
        private System.Windows.Forms.ToolStripMenuItem toolStripButton_sp_update_infos;
        private System.Windows.Forms.ToolStripMenuItem toolStripButton_sp_previous;
        private System.Windows.Forms.ToolStripMenuItem toolStripButton_sp_skip;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.GroupBox groupBox3;
        private LogBox logBox1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton hilfeToolStripButton;
        private System.Windows.Forms.PictureBox pic_recordState;
    }
}


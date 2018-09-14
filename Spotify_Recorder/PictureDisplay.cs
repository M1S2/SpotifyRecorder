using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spotify_Recorder
{
    public partial class PictureDisplay : Form
    {
        public string Title { private set; get; }
        public Image Picture { private set; get; }

        public PictureDisplay()
        {
            InitializeComponent();
        }

        public PictureDisplay(string title, Image picture)
        {
            InitializeComponent();
            Title = title;
            Picture = picture;

            if (title != null) { this.Text = title; }
            pic_Picture.Image = picture;
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            SaveImage();
        }

        private void SaveImage()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save AlbumArt";
            saveFileDialog.DefaultExt = ".jpg";
            saveFileDialog.Filter = ".jpg|*.jpg";
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Bitmap picture = (Bitmap)pic_Picture.Image;
                Bitmap pictureCopy = new Bitmap(picture);
                pictureCopy.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                pictureCopy.Dispose();
            }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SamSeifert.CSCV;
using System.IO;

namespace solution
{
    public partial class Neighbor : UserControl
    {
        public Neighbor()
        {
            InitializeComponent();
        }



        public readonly SVG _SVG;
        private readonly Size _PictureBoxSize;
        public readonly String _Path;
        public readonly int _Index;

        public Neighbor(int picturebox_size, SortableData sd)
        {
            this.SuspendLayout();
            this.InitializeComponent();

            this._PictureBoxSize = new Size(picturebox_size, picturebox_size);

//            this.lGroup.Text = sd._GroupName;
//            this.lFileName.Text = sd._FileName;
//            this.lIndex.Text = sd._Data._Index.ToString();

            this._Index = sd._Data._Index;
            this._Path = Path.Combine(sd._GroupName, sd._FileName);

            Bitmap bp = new Bitmap(
                picturebox_size,
                picturebox_size,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);


            var path = Path.Combine(
                Properties.Settings.Default.DatabaseLocation, this._Path);

            if (File.Exists(path))
            {
                string text;
                lock (MainForm.FileSystemL) text = File.ReadAllText(path);

                this._SVG = new SVG(text, "Compare");
                this._SVG.getImageForSize(ref bp, picturebox_size, 9999999, true);

                if (sd._Flipped)
                    this._SVG.setFlipped(this._Index);

            }

            this.pictureBox1.Image = bp;
            this.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        private bool _RectNotSect = true;
        private RectangleF _Rect;
        public RectangleF OriginalRect()
        {
            if (this._RectNotSect)
            {
                this._RectNotSect = false;
                this._Rect = this._SVG.getRectangle(this._Index);
            }
            return this._Rect;
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            if (this.DesignMode) return;

            if (this.pictureBox1.Size.Equals(this._PictureBoxSize)) return;

            int dx = this._PictureBoxSize.Width - this.pictureBox1.Width;
            int dy = this._PictureBoxSize.Height - this.pictureBox1.Height;

            var sz = this.Size;

            this.Size = new Size(sz.Width + dx, sz.Height + dy);
        }

        private void Neighbor_Load(object sender, EventArgs e)
        {
            this.pictureBox1_Resize(sender, e);
        }
    }
}

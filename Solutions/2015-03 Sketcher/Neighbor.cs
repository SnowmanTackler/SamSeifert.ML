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



        private readonly Size _PictureBoxSize;
        public readonly String _Path;
        public readonly int _Index;

        public Neighbor(Size picutrebox_size, Size image_size, SortableData sd) : this()
        {
            this._PictureBoxSize = picutrebox_size;

            this.lGroup.Text = sd._GroupName;
            this.lFileName.Text = sd._FileName;
            this.lIndex.Text = sd._Data._Index.ToString();

            this._Index = sd._Data._Index;
            this._Path = Path.Combine(sd._GroupName, sd._FileName);

            Bitmap bp = new Bitmap(
                picutrebox_size.Width,
                picutrebox_size.Height,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            var sect = new SectArray(SectType.Gray, image_size.Width, image_size.Height);


            int dex = 0;
            for (int y = 0; y < image_size.Height; y++)
                for (int x = 0; x < image_size.Width; x++)
                    sect[y, x] = sd._Data._Data[dex++] - 48;

            sect.RefreshImage(ref bp);

            this.pictureBox1.Image = bp;
            this.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;

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

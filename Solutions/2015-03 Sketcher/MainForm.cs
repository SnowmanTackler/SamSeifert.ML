using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using SamSeifert.Utilities.FileParsing;

using SamSeifert.ML;
using SamSeifert.ML.Controls;
using SamSeifert.ML.Data;
using SamSeifert.Utilities;
using SamSeifert.CSCV;

namespace solution
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// Scale 800 x 800 iamges down to 400 x 400
        /// </summary>
        const float RENDER_LARGE_SCALE = 0.5f;

        /// <summary>
        /// Scale 800 x 800 iamges down to 50 x 50
        /// </summary>
        const float L1_SCALE = L1_SIZE / 800.0f;
        const int L1_SIZE = 20;
        const int L1_DRAW_SCALE = 4;




        private SVG _SVG;



        private static MainForm Instance_;
        public static MainForm Instance
        {
            get
            {
                if (Instance_ == null) Instance_ = new MainForm();
                return Instance_;
            }
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (this.DesignMode) return;

            if (!ModifierKeys.HasFlag(Keys.Control)) // Set to default position!
                this.LoadFormState();

            int x = this.pDrawMain.Left;

            var sz = L1_SIZE * L1_DRAW_SCALE;

            foreach (var pan in new Control[] {
                this.pDrawTrailScaled,
                this.pDrawTrailScaledFiltered
            })
            {
                pan.Left = x;
                pan.Size = new Size(sz, sz);
                x += 10 + sz;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.SaveFormState();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.DesignMode) return;

            this.timerStartup.Enabled = false;

            this.textBox1.Text = Properties.Settings.Default.DatabaseLocation;

            this.nudCountourSections_ValueChanged(sender, e);

            this.LoadFileText(Properties.Resources.Sample);
        }


        private void pDrawMainLarge_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.ScaleTransform(RENDER_LARGE_SCALE, RENDER_LARGE_SCALE);

            using (var pen = new Pen(Color.Black, 1.0f))
            {
                if (this.timerDraw.Enabled)
                    this._SVG.LiveDraw[this._LiveDrawIndex].Draw(pen, e.Graphics);
                else if (this._SVG != null)
                    foreach (var continuity in this._SVG._Drawn)
                        foreach (var element in continuity)
                            element.Draw(pen, e.Graphics);
            }
        }

        Bitmap DrawTrail = null;
        private void pDrawTrail_Paint(object sender, PaintEventArgs e)
        {
            if (this.timerDraw.Enabled)
            {
                this._SVG.getForSize(ref DrawTrail, this.pDrawTrail.Width, this._LiveDrawIndex, false);
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                e.Graphics.DrawImage(DrawTrail, new Rectangle(Point.Empty, this.pDrawTrail.Size));
            }
            else e.Graphics.Clear(Color.White);
        }

        Bitmap DrawTrailScaled = new Bitmap(L1_SIZE, L1_SIZE, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        private void pDrawTrailScaled_Paint(object sender, PaintEventArgs e)
        {
            if (this.timerDraw.Enabled)
            {
                this._SVG.getForSize(ref DrawTrailScaled, L1_SIZE, this._LiveDrawIndex);
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                e.Graphics.DrawImage(DrawTrailScaled, new Rectangle(Point.Empty, this.pDrawTrailScaled.Size));
            }
            else e.Graphics.Clear(Color.White);
        }

        const int DrawTrailScaledFiltered_Size = 2;
        Sect DrawTrailScaledFiltered1;
        Sect DrawTrailScaledFiltered2;
        Sect DrawTrailScaledFiltered3;
        Sect DrawTrailScaledFiltered4;
        Sect DrawTrailScaledFiltered5;
        Sect DrawTrailScaledFilteredX = SectArray.Build.Gaussian.NormalizedSum1D(
            SectType.Gray,
            1.0f,
            1 + 2 * DrawTrailScaledFiltered_Size);

        Bitmap DrawTrailScaledFiltered = new Bitmap(L1_SIZE, L1_SIZE, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

        private void pDrawTrailScaledFiltered_Paint(object sender, PaintEventArgs e)
        {
            if (this.timerDraw.Enabled)
            {
                this.DrawTrailScaledFiltered1 = SectHolder.FromImage(DrawTrailScaled, true);

                SingleImage.PaddingAdd(
                    this.DrawTrailScaledFiltered1,
                    PaddingType.Unity,
                    DrawTrailScaledFiltered_Size,
                    ref this.DrawTrailScaledFiltered2);

                var filt = DrawTrailScaledFilteredX;
                MultipleImages.Convolute(this.DrawTrailScaledFiltered2, filt, ref this.DrawTrailScaledFiltered3);

                filt = filt.Transpose();
                MultipleImages.Convolute(this.DrawTrailScaledFiltered3, filt, ref this.DrawTrailScaledFiltered4);

                SingleImage.PaddingOff(
                    this.DrawTrailScaledFiltered4,
                    DrawTrailScaledFiltered_Size,
                    ref this.DrawTrailScaledFiltered5);

                this.DrawTrailScaledFiltered5.RefreshImage(ref this.DrawTrailScaledFiltered);

                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                e.Graphics.DrawImage(DrawTrailScaledFiltered, new Rectangle(Point.Empty, this.pDrawTrailScaledFiltered.Size));

            }
            else e.Graphics.Clear(Color.White);
        }













        private void nudCountourSections_ValueChanged(object sender, EventArgs e)
        {
            Contour.Sections = (int)Math.Round(this.nudCountourSections.Value);
            this.pDrawMain.ClearBackground();
            this.pDrawMain.ClearBackground();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            this.textBox1.Text =
                Directory.GetParent(this.openFileDialog1.FileName).Parent.FullName;

            this.LoadFile(this.openFileDialog1.FileName);
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            this.openFileDialog1.ShowDialog();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.textBox1.ForeColor = Directory.Exists(this.textBox1.Text) ? Color.Green : Color.Red;

            Properties.Settings.Default.DatabaseLocation = this.textBox1.Text;
            Properties.Settings.Default.Save();
        }

        public void LoadFile(string file_name)
        {
            this.lGroup.Text = Directory.GetParent(file_name).Name;
            this.LoadFileText(File.ReadAllText(file_name));
        }

        public void LoadFileText(string file_text)
        {
            DateTime dt = DateTime.Now;
            this._SVG = new SVG(file_text);
            Console.WriteLine("LOAD ELAPSED:" + (DateTime.Now - dt).TotalMilliseconds);

            if (this._SVG._ViewBox.Equals(new Rectangle(0, 0, 800, 800)))
            {
                this.timerDraw.Enabled = false;
                this.pDrawMain.ClearBackground();
                this.pDrawMain.ClearBackground();
            }
            else
            {
                MessageBox.Show("MainForm: Incorrect Size");
            }
        }

        private void bRandom_Click(object sender, EventArgs e)
        {
            Random r = new Random(Environment.TickCount);
            if (Directory.Exists(this.textBox1.Text))
            {
                var groups = Directory.GetDirectories(this.textBox1.Text);
                if (groups.Length == 0) return;
                var group = groups[r.Next() % groups.Length];

                var files = Directory.GetFiles(group);
                if (files.Length == 0) return;
                var file = files[r.Next() % files.Length];

                this.LoadFile(file);
            }
        }













        private int _LiveDrawIndex = 0;
        private void bPlayback_Click(object sender, EventArgs e)
        {
            if (this._SVG == null) return;

            DateTime dt = DateTime.Now;

            int count = this._SVG.LiveDraw.Length;

            Console.WriteLine("INTERPOLATE ELAPSED:" + (DateTime.Now - dt).TotalMilliseconds);

            if (count == 0)
            {
                this.timerDraw.Enabled = false;
            }
            else
            {
                this._LiveDrawIndex = 0;
                this.pDrawMain.ClearBackground();

                this.timerDraw.Enabled = true;
            }
        }

        private void timerDraw_Tick(object sender, EventArgs e)
        {
            if (this._LiveDrawIndex == this._SVG.LiveDraw.Length - 1)
            {
                this.timerDraw.Enabled = false;
            }
            else
            {
                this._LiveDrawIndex++;
                this.pDrawMain.Invalidate();
                this.pDrawMain.Invalidate();
                this.pDrawTrail.Invalidate();
                this.pDrawTrailScaled.Invalidate();
                this.pDrawTrailScaledFiltered.Invalidate();
            }
        }

    }
}

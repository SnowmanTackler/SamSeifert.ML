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

namespace solution
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// Scale 800 x 800 iamges down to 400 x 400
        /// </summary>
        const float RENDER_SCALE = 0.5f;

        /// <summary>
        /// Scale 800 x 800 iamges down to 50 x 50
        /// </summary>
        const float L1_SCALE = L1_SIZE / 800.0f;
        const int L1_SIZE = 50;

        /// <summary>
        /// How many pixels per broken up point.
        /// </summary>
        const int INCREMENT_DISTANCE = 10;



        const int TRAIL_LENGTH = 400 // Pixels
            / INCREMENT_DISTANCE; // Turns pixels into counts



        private SVG _SVG;




        public MainForm()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.DesignMode) return;

            this.timerStartup.Enabled = false;

            this.textBox1.Text = Properties.Settings.Default.DatabaseLocation;

            this.nudCountourSections_ValueChanged(sender, e);

            this.LoadFileText(Properties.Resources.Sample);
        }




        private void pDrawMain_Paint(object sender, PaintEventArgs e)
        {
            if (sender == this.pDrawMain1) e.Graphics.ScaleTransform(RENDER_SCALE, RENDER_SCALE);
            else if (sender == this.pDrawMain2) e.Graphics.ScaleTransform(L1_SCALE, L1_SCALE);
            else return;

            using (var pen = new Pen(Color.Black, 1.0f))
            {
                if (this._LiveDraw != null)
                    this._LiveDraw[this._LiveDrawIndex].Draw(pen, e.Graphics);
                else if (this._SVG != null)
                    foreach (var continuity in this._SVG._Drawn)
                        foreach (var element in continuity)
                            element.Draw(pen, e.Graphics);
            }
        }

        private void pDrawTrail_Paint(object sender, PaintEventArgs e)
        {
            if (this._LiveDraw != null)
            {
                e.Graphics.ScaleTransform(L1_SCALE, L1_SCALE);
                using (var pen = new Pen(Color.Black, 1.0f))
                {
                    for (int i = Math.Max(0, this._LiveDrawIndex - TRAIL_LENGTH); i < this._LiveDrawIndex; i++)
                    {
                        this._LiveDraw[i].Draw(pen, e.Graphics);
                    }
                }
            }
        }

        private void pDrawTrailScaled_Paint(object sender, PaintEventArgs e)
        {
            if (this._LiveDraw != null)
            {
                float min_x = float.MaxValue;
                float max_x = float.MinValue;
                float min_y = float.MaxValue;
                float max_y = float.MinValue;

                for (int i = Math.Max(0, this._LiveDrawIndex - TRAIL_LENGTH); i < this._LiveDrawIndex; i++)
                {
                    this._LiveDraw[i].UpdateBounds(
                        ref min_x,
                        ref max_x,
                        ref min_y,
                        ref max_y);
                }

                float range_x = max_x - min_x;
                float range_y = max_y - min_y;

                if (float.IsNaN(range_x) || float.IsInfinity(range_x) || (range_x > 1000)) return;
                if (float.IsNaN(range_y) || float.IsInfinity(range_y) || (range_y > 1000)) return;

                if (range_x + range_y == 0) return;

                float biggest_range = Math.Max(range_x, range_y);

                float scale = L1_SIZE / biggest_range;

                e.Graphics.ResetTransform();

                e.Graphics.ScaleTransform(scale, scale);
                e.Graphics.TranslateTransform(
                    (biggest_range - range_x) / 2 - min_x,
                    (biggest_range - range_y) / 2 - min_y);





                using (var pen = new Pen(Color.Black, 1 / scale))
                {
                    for (int i = Math.Max(0, this._LiveDrawIndex - TRAIL_LENGTH); i < this._LiveDrawIndex; i++)
                    {
                        this._LiveDraw[i].Draw(pen, e.Graphics);
                    }
                }
            }
        }












        private static MainForm Instance_;
        public static MainForm Instance
        {
            get
            {
                if (Instance_ == null) Instance_ = new MainForm();
                return Instance_;
            }
        }

        private void nudCountourSections_ValueChanged(object sender, EventArgs e)
        {
            Contour.Sections = (int)Math.Round(this.nudCountourSections.Value);
            this.pDrawMain1.ClearBackground();
            this.pDrawMain2.ClearBackground();
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
                this._LiveDraw = null;
                this.timerDraw.Enabled = false;
                this.pDrawMain1.ClearBackground();
                this.pDrawMain2.ClearBackground();
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
        private Drawable[] _LiveDraw = null;
        private void bPlayback_Click(object sender, EventArgs e)
        {
            if (this._SVG == null) return;

            DateTime dt = DateTime.Now;

            var ls = new List<Drawable>();
            DrawableGroup last_group = null;
            float last_group_length = 0;

            foreach (var continuity in this._SVG._Drawn)
                foreach (var element in continuity)
                    element.BreakIntoSmallPortions(
                        ref ls,
                        ref last_group,
                        ref last_group_length,
                        INCREMENT_DISTANCE);

            Console.WriteLine("INTERPOLATE ELAPSED:" + (DateTime.Now - dt).TotalMilliseconds);

            if (ls.Count > 0)
            {
                this._LiveDraw = ls.ToArray();
                this._LiveDrawIndex = 0;
                this.pDrawMain1.ClearBackground();
                this.pDrawMain2.ClearBackground();
                this.timerDraw.Enabled = true;
            }
            else
            {
                this._LiveDraw = null;
                this.timerDraw.Enabled = false;
            }
        }

        private void timerDraw_Tick(object sender, EventArgs e)
        {
            if (this._LiveDrawIndex == this._LiveDraw.Length - 1)
            {
                this._LiveDraw = null;
                this.timerDraw.Enabled = false;
            }
            else
            {
                this._LiveDrawIndex++;
                this.pDrawMain1.Invalidate();
                this.pDrawMain2.Invalidate();
                this.pDrawTrail1.Invalidate();
                this.pDrawTrailScaled.Invalidate();
            }
        }
    }
}

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
        /// Scale 800 x 800 immges down to L1_SIZE x L1_SIZE
        /// </summary>
        const float L1_SCALE = L1_SIZE / 800.0f;
        const int L1_SIZE = 15;
        const int L1_DRAW_SCALE = 5;


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

            this.LoadFileText(Properties.Resources.Sample, "Sample");
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

        Bitmap DrawTrail_Final = null;
        private void pDrawTrail_Paint(object sender, PaintEventArgs e)
        {
            if (this.timerDraw.Enabled)
            {
                this._SVG.getForSize(ref DrawTrail_Final, this.pDrawTrail.Width, this._LiveDrawIndex, false);
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                e.Graphics.DrawImage(DrawTrail_Final, new Rectangle(Point.Empty, this.pDrawTrail.Size));
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
            this.LoadFileText(File.ReadAllText(file_name), file_name);
        }

        public void LoadFileText(string file_text, string file_name)
        {
            DateTime dt = DateTime.Now;
            this._SVG = new SVG(file_text, file_name);
            this._SVG.InitializeImageChain(L1_SIZE);
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

        private void bSave_Click(object sender, EventArgs e)
        {
            #if !DEBUG
            try
            {
            #endif
            String dir = Directory.GetParent(this.textBox1.Text).FullName;

                dir = Path.Combine(dir, "SKETCHES_PARSED");

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                String file_name = "size_" + L1_SIZE + "___trail_" + SVG.TRAIL_LENGTH_PIXELS;

                dir = Path.Combine(dir, file_name);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var groups = Directory.GetDirectories(this.textBox1.Text);

                for (int g = 0; g < groups.Length; g++)
                {
                    DateTime dt = DateTime.Now;

                    String group = groups[g];
                    String group_name = group.Replace(this.textBox1.Text + Path.DirectorySeparatorChar, "");
                    String new_path = Path.Combine(dir, group_name);

                    if (String.Compare(group_name, "computer-mouse") < 0) continue; // Skip to

                    if (File.Exists(new_path)) File.Delete(new_path);
                    using (var fs = new StreamWriter(File.Create(new_path)))
                    {
                        fs.Write("[");


                        var files = Directory.GetFiles(group);
                        for (int f = 0; f < files.Length; f++)
                        {
                            var file = files[f];

                            var svg = new SVG(File.ReadAllText(file), file);
                            svg.InitializeImageChain(L1_SIZE);

                            var ob = new Dictionary<string, object>();

                            foreach (var stuff in svg.SaveImageChain())
                                ob[stuff._LiveDrawIndex.ToString()] = stuff._Pixels;

                            JsonParser.print(
                                ob,
                                fs.Write,
                                fs.Write
                                );

                        }

                        fs.Write("]");
                    }

                    Console.WriteLine(group_name + ": " + (DateTime.Now - dt).TotalMilliseconds);
                }
            #if !DEBUG
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            #endif
        }

        private void bLoad_Click(object sender, EventArgs e)
        {

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

                this._SVG.SetImageChain(this._LiveDrawIndex);

                this._SVG._SectScaled.RefreshImage(ref this.DrawTrailScaled_Final);
                this._SVG._SectScaledFiltered.RefreshImage(ref this.DrawTrailScaledFiltered_Final);


                this.pDrawMain.Invalidate();
                this.pDrawMain.Invalidate();
                this.pDrawTrail.Invalidate();
                this.pDrawTrailScaled.Invalidate();
                this.pDrawTrailScaledFiltered.Invalidate();
            }
        }



        Bitmap DrawTrailScaled_Final = new Bitmap(
            L1_SIZE * L1_DRAW_SCALE,
            L1_SIZE * L1_DRAW_SCALE,
            System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        private void pDrawTrailScaled_Paint(object sender, PaintEventArgs e)
        {
            if (this.timerDraw.Enabled)
            {
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                e.Graphics.DrawImageUnscaled(DrawTrailScaled_Final, Point.Empty);
            }
            else e.Graphics.Clear(Color.White);
        }

        Bitmap DrawTrailScaledFiltered_Final = new Bitmap(
            L1_SIZE * L1_DRAW_SCALE,
            L1_SIZE * L1_DRAW_SCALE,
            System.Drawing.Imaging.PixelFormat.Format24bppRgb);

        private void pDrawTrailScaledFiltered_Paint(object sender, PaintEventArgs e)
        {
            if (this.timerDraw.Enabled)
            {
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                e.Graphics.DrawImageUnscaled(DrawTrailScaledFiltered_Final, Point.Empty);
            }
            else e.Graphics.Clear(Color.White);
        }

    }
}

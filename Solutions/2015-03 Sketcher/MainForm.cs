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

        const float scale = 0.5f;
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.ScaleTransform(scale, scale);

            using (var pen = new Pen(Color.Black, 1.0f))
            {
                if (this._LiveDraw != null)
                {
                    this._LiveDraw[this._LiveDrawIndex++].Draw(pen, e.Graphics);
                    if (this._LiveDrawIndex == this._LiveDraw.Length)
                    {
                        this._LiveDraw = null;
                        this.timerDraw.Enabled = false;
                    }
                }
                else if (this._SVG != null)
                    foreach (var continuity in this._SVG._Drawn)
                        foreach (var element in continuity)
                            element.Draw(pen, e.Graphics);
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
            this.panelDraw.ClearBackground();
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

            this.panelDraw.Width = (int)Math.Round(scale * this._SVG._ViewBox.Width);
            this.panelDraw.Height = (int)Math.Round(scale * this._SVG._ViewBox.Height);

            this._LiveDraw = null;
            this.timerDraw.Enabled = false;
            this.panelDraw.ClearBackground();
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
                    element.Interpolate(
                        ref ls,
                        ref last_group,
                        ref last_group_length,
                        10);

            Console.WriteLine("INTERPOLATE ELAPSED:" + (DateTime.Now - dt).TotalMilliseconds);

            if (ls.Count > 0)
            {
                this._LiveDraw = ls.ToArray();
                this._LiveDrawIndex = 0;
                this.panelDraw.ClearBackground();
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
            this.panelDraw.Invalidate();
        }
    }
}

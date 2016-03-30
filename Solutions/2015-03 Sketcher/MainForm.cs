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
using SamSeifert.ML.Datas;
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

            int dy = sz - this.pDrawTrailScaled.Height;
            this.pSelect.Height -= dy;
            this.pSelect.Top += dy;

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

        private void timerStartup_Tick(object sender, EventArgs e)
        {
            if (this.DesignMode) return;

            this.timerStartup.Enabled = false;

            this.textBox1.Text = Properties.Settings.Default.DatabaseLocation;

            this.nudCountourSections_ValueChanged(sender, e);

            this.LoadFileText(Properties.Resources.Sample, "Sample");
        }

        private int _LiveDrawIndex = -1;
        private void timerDraw_Tick(object sender, EventArgs e)
        {
            if (this._LiveDrawIndex == this._SVG.LiveDraw.Length - 1)
            {
                this.timerDraw.Enabled = false;
                this._LiveDrawIndex = -1;
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

        private void pDrawMainLarge_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.ScaleTransform(RENDER_LARGE_SCALE, RENDER_LARGE_SCALE);

            using (var pen = new Pen(Color.Black, 1.0f))
            {
                if (this._LiveDrawIndex != -1)
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
            if (this._LiveDrawIndex != -1)
            {
                this._SVG.getForSize(ref DrawTrail_Final, this.pDrawTrail.Width, this._LiveDrawIndex, false);
                e.Graphics.ResetTransform();
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                e.Graphics.DrawImage(DrawTrail_Final, new Rectangle(Point.Empty, this.pDrawTrail.Size));

                if (this._LastNeighbor != null)
                {
                    e.Graphics.TranslateTransform(
                        this._NeighborSvgSize.X - this._NeighborSvgSizeOriginal.X,
                        this._NeighborSvgSize.Y - this._NeighborSvgSizeOriginal.Y);

                    float sc = this._NeighborSvgSizeOriginal.Width / this._NeighborSvgSize.Width;
                    e.Graphics.ScaleTransform(sc, sc);
                    e.Graphics.ScaleTransform(RENDER_LARGE_SCALE, RENDER_LARGE_SCALE);

                    sc *= RENDER_LARGE_SCALE;

                    using (var green = new Pen(Color.Green, 2.0f / sc))
                    using (var red = new Pen(Color.Red, 2.0f / sc))
                    {
                        int end_index = this._LastNeighbor._Index;
                        for (int i = Math.Max(0, end_index - SVG.TRAIL_LENGTH); i < this._LastNeighborKid.LiveDraw.Length; i++)
                        { 
                            Pen p = green;
                            if (i < end_index) p = red;
                            this._LastNeighborKid.LiveDraw[i].Draw(p, e.Graphics);
                        }
                    }
                }
            }
            else e.Graphics.Clear(Color.White);
        }

        Bitmap DrawTrailScaled_Final = new Bitmap(
            L1_SIZE * L1_DRAW_SCALE,
            L1_SIZE * L1_DRAW_SCALE,
            System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        private void pDrawTrailScaled_Paint(object sender, PaintEventArgs e)
        {
            if (this._LiveDrawIndex != -1)
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
            if (this._LiveDrawIndex != -1)
            {
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                e.Graphics.DrawImageUnscaled(DrawTrailScaledFiltered_Final, Point.Empty);
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
            if (Directory.Exists(this.textBox1.Text))
            {
                this.textBox1.ForeColor = Color.Green;
                Properties.Settings.Default.DatabaseLocation = this.textBox1.Text;
                Properties.Settings.Default.Save();
            }
            else this.textBox1.ForeColor = Color.Red;
        }

        public void LoadFile(string file_name)
        {
            this.lFileName.Text = Path.GetFileName(file_name);
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













        private void bPlayback_Click(object sender, EventArgs e)
        {
            if (this._SVG == null) return;

            if (this.timerDraw.Enabled)
            {
                this.timerDraw.Enabled = false;
            }
            else
            {
                if (this._LiveDrawIndex == -1)
                {
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
                else this.timerDraw.Enabled = true;
            }
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            if (!File.Exists(this.textBox1.Text)) return;
#if !DEBUG
            try
#endif
            {
                String dir = Directory.GetParent(this.textBox1.Text).FullName;

                dir = Path.Combine(dir, "SKETCHES_PARSED");

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                String file_name = "size_" + L1_SIZE + "___trail_" + SVG.TRAIL_LENGTH_PIXELS;

                dir = Path.Combine(dir, file_name);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                foreach (var group in Directory.GetDirectories(this.textBox1.Text))
                {
                    DateTime dt = DateTime.Now;

                    String group_name = group.Replace(this.textBox1.Text + Path.DirectorySeparatorChar, "");
                    String new_path = Path.Combine(dir, group_name);

                    // if (String.Compare(group_name, "computer-mouse") < 0) continue; // Skip to

                    if (File.Exists(new_path)) File.Delete(new_path);
                    using (var fs = new StreamWriter(File.Create(new_path)))
                    {
                        fs.Write("[");

                        foreach (var file in Directory.GetFiles(group))
                        {
                            var svg = new SVG(File.ReadAllText(file), file);
                            svg.InitializeImageChain(L1_SIZE);

                            var ob = new Dictionary<string, object>();
                            ob["file_name"] = Path.GetFileName(file);

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
            }
#if !DEBUG
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
#endif
        }




        /// <summary>
        /// First level is group name, second level is file name
        /// </summary>
        Dictionary<String, Dictionary<String, RawData[]>> _Data = null;

        private void LoadData()
        {
            if (this._Data != null) return;
            if (!Directory.Exists(this.textBox1.Text)) return;

            String dir = Directory.GetParent(this.textBox1.Text).FullName;

            dir = Path.Combine(dir, "SKETCHES_PARSED");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            String file_name = "size_" + L1_SIZE + "___trail_" + SVG.TRAIL_LENGTH_PIXELS;

            dir = Path.Combine(dir, file_name);

            this._Data = new Dictionary<String, Dictionary<String, RawData[]>>();

            foreach (var file in Directory.GetFiles(dir))
            {
                DateTime dt = DateTime.Now;

                var file_dict = new Dictionary<String, RawData[]>();


                using (var sr = new StreamReader(file))
                {
                    while (sr.Read() != '[') ;

                    foreach (var ob in JsonParser.parseArray(sr))
                    {
                        if (ob is Dictionary<string, object>)
                        {
                            var dict = ob as Dictionary<string, object>;
                            var svg_file_name = dict["file_name"] as string;

                            var ls = new List<RawData>();

                            foreach (var kvp in dict)
                            {
                                int index;
                                if (int.TryParse(kvp.Key, out index))
                                {
                                    ls.Add(new RawData(index, kvp.Value as string));
                                }
                            }

                            file_dict[svg_file_name] = ls.ToArray();
                        }
                    }

                }

                string group_name = Path.GetFileName(file);
                this._Data[group_name] = file_dict;
                Console.WriteLine(group_name + ": " + (DateTime.Now - dt).TotalMilliseconds);
                if (this._Data.Count > 10) return;
            }

        }

        private void buttonFindNearest_Click(object sender, EventArgs e)
        {
            if (this._SVG == null) return;
            if (this._SVG._SectScaledFiltered == null) return;

            this.LoadData();
            if (this._Data == null) return;

            var sect = this._SVG._SectScaled;
            var im_size = sect.getPrefferedSize();

            var current = new float[im_size.Width * im_size.Height];

            int dex = 0;
            for (int y = 0; y < im_size.Height; y++)
                for (int x = 0; x < im_size.Width; x++)
                    current[dex++] = sect[y, x];

            int knn = 10;
            int index = 0;

            var top_points = new SortableData[knn + 1];
            top_points[knn] = SortableData.Maximum;

            // Get top knn neighbors (each from different file)
            foreach (var kvp_group in this._Data)
            {
                Console.WriteLine(kvp_group.Key);

                foreach (var kvp_file in kvp_group.Value)
                {
                    //if (this.lFileName.Text.Equals(kvp_file.Key)) continue;

                    var top_point_file = SortableData.Maximum;

                    // Find best for each file
                    foreach (var data in kvp_file.Value)
                    {
                        int lens = Math.Min(data._Data.Length, current.Length);
                        float dist = 0;

                        for (int i = 0; i < lens; i++)
                        {
                            // - 48 converts char 1 or 0 to the int value of 1 or 0
                            float diff = current[i] - (data._Data[i] - 48);
                            dist += diff * diff;

                        }

                        if (dist < top_point_file._Distance)
                            top_point_file = new SortableData(
                                kvp_group.Key,
                                kvp_file.Key,
                                data,
                                dist);
                    }

                    // Each file only gets one best
                    if (top_point_file._Data != null)
                    {
                        if (index < knn)
                        {
                            top_points[index] = top_point_file;
                            index++;
                            if (index == knn)
                                Array.Sort(top_points, (a, b) => b._Distance.CompareTo(a._Distance));
                        }
                        else
                        {
                            top_points[0] = top_point_file;
                            for (int offset = 0; // Bubble Sort
                                (offset < knn) && (top_points[offset]._Distance < top_points[offset + 1]._Distance);
                                offset++)
                            {
                                MiscUtil.Swap(
                                    ref top_points[offset],
                                    ref top_points[offset + 1]);
                            }
                        }
                    }
                }

            }

            var disp_size = new Size(L1_SIZE * L1_DRAW_SCALE, L1_SIZE * L1_DRAW_SCALE);

            this._LastNeighbor = null;

            while (this.pSelect.Controls.Count > 0)
            {
                var c = this.pSelect.Controls[0];
                (c as Neighbor).pictureBox1.Click -= N_Click;
                c.RemoveFromParent();
                c.Dispose();
            }

            const int inc = 10;
            int control_y = inc;
            for (int i = 0; i < index; i++)
            {
                var n = new Neighbor(disp_size, im_size, top_points[knn - i] );
                this.pSelect.Controls.Add(n);
                n.pictureBox1.Click += N_Click;
                n.Top = control_y;
                n.Left = inc;
                control_y += inc + 20 + disp_size.Height; // 20 is for margin between neighbor picture box and container
            }

            Bitmap bp = null;
            this._NeighborSvgSizeOriginal = this._SVG.getForSize(ref bp, 1, this._LiveDrawIndex, true);
            bp.Dispose(); // TODO: THIS BETTER.
        }


        private Neighbor _LastNeighbor = null;
        private SVG _LastNeighborKid;
        private RectangleF _NeighborSvgSizeOriginal;
        private RectangleF _NeighborSvgSize;

        private void N_Click(object sender, EventArgs e)
        {
            var tableview = (sender as Control).Parent;
            if (tableview == null) return;
            var neighbor = tableview.Parent as Neighbor;
            if (neighbor == null) return;

            if (this._LastNeighbor != null)
                this._LastNeighbor.BackColor = this.panelRight.BackColor;

            this._LastNeighbor = neighbor;
            this._LastNeighbor.BackColor = Color.LimeGreen;

            var file_text = File.ReadAllText(Path.Combine(
                Properties.Settings.Default.DatabaseLocation, this._LastNeighbor._Path));
            this._LastNeighborKid = new SVG(file_text, "Compare");
            this._LastNeighborKid.InitializeImageChain(L1_SIZE);

            Bitmap bp = null;
            this._NeighborSvgSize = this._LastNeighborKid.getForSize(ref bp, 1, this._LastNeighbor._Index, true);
            bp.Dispose(); // TODO: THIS BETTER.

            Console.WriteLine(this._NeighborSvgSizeOriginal);
            Console.WriteLine(this._NeighborSvgSize);

            this.pDrawTrail.Invalidate();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


using SamSeifert.Utilities.FileParsing;

using SamSeifert.ML;
using SamSeifert.ML.Controls;
using SamSeifert.ML.Datas;
using SamSeifert.Utilities;
using SamSeifert.CSCV;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace solution
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// Scale 800 x 800 iamges down to 400 x 400
        /// </summary>
        const float RENDER_LARGE_SCALE = ORIGINAL_IMAGE_RENDERED_SIZE / (float)ORIGINAL_IMAGE_SIZE;
        const int ORIGINAL_IMAGE_SIZE = 800;
        const int ORIGINAL_IMAGE_RENDERED_SIZE = 400;

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

            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundWorker1_DoWork);


            if (!ModifierKeys.HasFlag(Keys.Control)) // Set to default position!
                this.LoadFormState();

            int x = this.pDrawTrailScaled.Left;

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
            if (this._LiveDrawIndex == this._SVG.LiveDraw.Length)
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

                this.glControl1.Invalidate();
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
            //Contour.Sections = (int)Math.Round(this.nudCountourSections.Value);
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
            if (this._SVG != null) this._DeleteableSVGS.Add(this._SVG);
            this._LiveDrawIndex = -1;
            this._SVG = new SVG(file_text, file_name);
            this._SVG.InitializeImageChain(L1_SIZE);
            this.glControl1.Invalidate();
            Console.WriteLine("LOAD ELAPSED:" + (DateTime.Now - dt).TotalMilliseconds);

            this.timerDraw.Enabled = false;
            if (!this._SVG._ViewBox.Equals(new Rectangle(0, 0, 800, 800)))
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
                    if (count == 0)
                    {
                        this.timerDraw.Enabled = false;
                    }
                    else
                    {
                        this._LiveDrawIndex = 0;
                        this.timerDraw.Enabled = true;
                    }
                }
                // Resume
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
        private bool _DataLoadingOrLoaded = false;

        private void LoadData()
        {
            if (this._DataLoadingOrLoaded) return;
            if (!Directory.Exists(this.textBox1.Text)) return;
            this._DataLoadingOrLoaded = true;

            String dir = Directory.GetParent(this.textBox1.Text).FullName;

            dir = Path.Combine(dir, "SKETCHES_PARSED");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            String file_name = "size_" + L1_SIZE + "___trail_" + SVG.TRAIL_LENGTH_PIXELS;

            dir = Path.Combine(dir, file_name);

            this.backgroundWorker1.RunWorkerAsync(dir);
        }

        private static void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int threads = 2;

            // One event is used for each Fibonacci object.
            ManualResetEvent[] events = new ManualResetEvent[threads];
            LoadFilePoolResponder[] thread_objects = new LoadFilePoolResponder[threads];

            var dt = DateTime.Now;

            var files = Directory.GetFiles(e.Argument as String);

            // Configure and start threads using ThreadPool.
            Console.WriteLine("Launching {0} tasks...", threads);
            for (int i = 0; i < threads; i++)
            {
                var ls = new List<String>();
                for (int dex = 0; dex < files.Length; dex++)
                    if (dex % threads == i)
                        ls.Add(files[dex]);
                    else if (dex > 10) break;

                events[i] = new ManualResetEvent(false);
                thread_objects[i] = new LoadFilePoolResponder(ls.ToArray(), events[i]);
                ThreadPool.QueueUserWorkItem(thread_objects[i].ThreadPoolCallback, i);
            }
            // Wait for all threads in pool to calculate.
            WaitHandle.WaitAll(events);
            Console.WriteLine("All calculations are complete in " + (DateTime.Now - dt).TotalSeconds + " seconds");

            var data = new Dictionary<String, Dictionary<String, RawData[]>>();

            // Display the results.
            foreach (var thread_object in thread_objects)
                foreach (var kvp in thread_object._Data)
                    data[kvp.Key] = kvp.Value;

            e.Result = data;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this._Data = e.Result as Dictionary<String, Dictionary<String, RawData[]>>;

            this.buttonFindNearest_Click(sender, e);
        }


        private class LoadFilePoolResponder
        {
            // Only one thread talk to file system at a time
            private static readonly object FileSystemL = new object();

            public Dictionary<String, Dictionary<String, RawData[]>> _Data { get; private set; }
            private ManualResetEvent _DoneEvent;
            private String[] _Files;

            // Constructor.
            public LoadFilePoolResponder(String[] files, ManualResetEvent doneEvent)
            {
                this._Files = files;
                _DoneEvent = doneEvent;
            }

            // Wrapper method for use with thread pool.
            public void ThreadPoolCallback(Object threadContext)
            {
                int threadIndex = (int)threadContext;
                this._Data = Calculate(this._Files);
                _DoneEvent.Set();
            }

            private static Dictionary<String, Dictionary<String, RawData[]>> Calculate(IEnumerable<String> files)
            {
                var data = new Dictionary<String, Dictionary<String, RawData[]>>();
                foreach (var file in files)
                {
                    DateTime dt = DateTime.Now;

                    var file_dict = new Dictionary<String, RawData[]>();

                    String text;

                    lock (FileSystemL) // One guy reads a file at a time
                        text = File.ReadAllText(file);
                   
                    using (var sr = new StreamReader(text.AsStream()))
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
                    data[group_name] = file_dict;
                    Console.WriteLine(group_name + ": " + (DateTime.Now - dt).TotalMilliseconds);
                }
                return data;
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

            this._NeighborSvgSizeOriginal = this._SVG.getRectangle(this._LiveDrawIndex);
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

            if (this._LastNeighborKid != null)
                this._DeleteableSVGS.Add(this._LastNeighborKid);

            this._LastNeighborKid = new SVG(file_text, "Compare");
            this._LastNeighborKid.InitializeImageChain(L1_SIZE);

            this._NeighborSvgSize = this._LastNeighborKid.getRectangle(this._LastNeighbor._Index);

            this.glControl1.Invalidate();
        }

        private void nudFuture_ValueChanged(object sender, EventArgs e)
        {
            this.glControl1.Invalidate();
        }















        private HashSet<GLControl> _Hashset = new HashSet<GLControl>();

        private void glControl1_Load(object sender, EventArgs e)
        {
            this._Hashset.Add(sender as GLControl);

            GL.LineWidth(1.0f);

            GL.MatrixMode(MatrixMode.Modelview); // Always the default.

            GL.ClearColor(Color.Black);
            GL.DepthFunc(DepthFunction.Less);
            GL.CullFace(CullFaceMode.Back);

            GL.Enable(EnableCap.CullFace);
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);

            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

//            this.MouseDown += new MouseEventHandler(mouseDown);
//            this.MouseUp += new MouseEventHandler(mouseUp);
//            this.MouseMove += new MouseEventHandler(mouseMove);
//            this.Load -= new EventHandler(ControlLoaded);
//            Application.Idle += new EventHandler(Application_Idle);

            (sender as GLControl).Invalidate();
        }

        private readonly List<SVG> _DeleteableSVGS = new List<SVG>();


        private void DrawRectange(RectangleF r, PrimitiveType pt)
        {
            this.DrawRectange(r.X, r.Y, r.Width, r.Height, pt);
        }

        private void DrawRectange(float x, float y, float w, float h, PrimitiveType pt)
        {
            GL.Begin(pt);
            {
                GL.Vertex2(x, y);
                GL.Vertex2(x, y + h);
                GL.Vertex2(x + w, y + h);
                GL.Vertex2(x + w, y);
            }
            GL.End();
        }


        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!this._Hashset.Contains(sender as GLControl)) return;

            foreach (var svg in this._DeleteableSVGS) svg.GL_Delete();
            this._DeleteableSVGS.Clear();

            GL.Clear(ClearBufferMask.ColorBufferBit);


            GL.MatrixMode(MatrixMode.Projection);
            var proj = Matrix4.CreateOrthographicOffCenter(0, this.glControl1.Width, this.glControl1.Height, 0, 0, 1);
            GL.LoadMatrix(ref proj);

            GL.Viewport(0, 0, this.glControl1.Width, this.glControl1.Height);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Translate(0, 1, -0.5f);

            if (this._SVG != null)
            {

                this._SVG.GL_BindBuffer();
                GL.EnableClientState(ArrayCap.VertexArray);
                GL.VertexPointer(2, VertexPointerType.Float, Vector2.SizeInBytes, IntPtr.Zero);

                if (true)
                { // Draw Main
                    GL.Translate(1, 0, 0); // 1 Pixel Shift
                    GL.PushMatrix();
                    {
                        GL.Color3(Color.White);
                        this.DrawRectange(0, 0, ORIGINAL_IMAGE_RENDERED_SIZE, ORIGINAL_IMAGE_RENDERED_SIZE, PrimitiveType.Quads);

                        GL.Scale(RENDER_LARGE_SCALE, RENDER_LARGE_SCALE, 1.0f);

                        GL.Color3(Color.Black);
                        this._SVG.GL_DrawAll(this._LiveDrawIndex);

                    }
                    GL.PopMatrix();
                    GL.Translate(ORIGINAL_IMAGE_RENDERED_SIZE, 0, 0); // 1 Pixel Shift
                }


                if (true)
                { // Draw Trail
                    GL.Translate(1, 0, 0); // 1 Pixel Shift
                    GL.Color3(Color.White);
                    this.DrawRectange(0, 0, ORIGINAL_IMAGE_RENDERED_SIZE, ORIGINAL_IMAGE_RENDERED_SIZE, PrimitiveType.Quads);




                    if (this._LastNeighbor == null)
                    {
                        GL.PushMatrix();
                        {
                            GL.Scale(RENDER_LARGE_SCALE, RENDER_LARGE_SCALE, 1.0f);
                            GL.Color3(Color.Black);
                            this._SVG.GL_DrawRecent(this._LiveDrawIndex);
                        }
                        GL.PopMatrix();
                    }
                    else
                    {
                        GL.PushMatrix();

                        GL.Scale(RENDER_LARGE_SCALE, RENDER_LARGE_SCALE, 1.0f);


                        /*{
                        }*/


                        float szc = this._NeighborSvgSizeOriginal.Width / this._NeighborSvgSize.Width;

                        this._LastNeighborKid.GL_BindBuffer();

                        int future = (int)Math.Round(this.nudFuture.Value);
                        int end_index = this._LastNeighbor._Index;

                        GL.Translate(
                            this._NeighborSvgSizeOriginal.X,
                            this._NeighborSvgSizeOriginal.Y,
                            0);

                        GL.Scale(szc, szc, 1);

                        GL.Translate(
                            -this._NeighborSvgSize.X,
                            -this._NeighborSvgSize.Y,
                            0);

                        GL.Color3(Color.Blue);
                        this._LastNeighborKid.GL_DrawRecent(end_index);

                        GL.Color3(Color.Red);

                        this._LastNeighborKid.GL_DrawFromLength(end_index, future);

                        /*
                        float global_scale = 1;

                        global_scale /= RENDER_LARGE_SCALE;

                        using (var yellow = new Pen(Color.Brown, global_scale))
                        {
                            RectangleF r = this._NeighborSvgSizeOriginal;
                            e.Graphics.DrawRectangle(yellow, -r.X, -r.Y, r.Width, r.Height);
                        }

                        e.Graphics.ScaleTransform(locale_scale, locale_scale, System.Drawing.Drawing2D.MatrixOrder.Prepend);
                        global_scale /= locale_scale;

                        using (var blue = new Pen(Color.Blue, global_scale))
                        {
                            RectangleF r = this._NeighborSvgSize;
                            e.Graphics.DrawRectangle(blue, -r.X, -r.Y, r.Width, r.Height);
                        }

                        global_scale *= 0.5f;
                        e.Graphics.TranslateTransform(
                            (this._NeighborSvgSize.X - this._NeighborSvgSizeOriginal.X) * global_scale,
                            (this._NeighborSvgSize.Y - this._NeighborSvgSizeOriginal.Y) * global_scale,
                            System.Drawing.Drawing2D.MatrixOrder.Prepend);

                        using (var blue = new Pen(Color.Green, global_scale))
                        {
                            RectangleF r = this._NeighborSvgSize;
                            e.Graphics.DrawRectangle(blue, -r.X, -r.Y, r.Width, r.Height);
                        }

                        using (var green = new Pen(Color.Green, 1.0f / global_scale))
                        using (var red = new Pen(Color.Red, 1.0f / global_scale))
                        {
                            int future = (int)Math.Round(this.nudFuture.Value);
                            int end_index = this._LastNeighbor._Index;
                            for (int i = Math.Max(0, end_index - SVG.TRAIL_LENGTH);
                                i < Math.Min(end_index + future, this._LastNeighborKid.LiveDraw.Length);
                                i++)
                            {
                                Pen p = green;
                                if (i < end_index) p = red;
                                this._LastNeighborKid.LiveDraw[i].Draw(p, e.Graphics);
                            }
                        }*/
                        GL.PopMatrix();
                    }

                    GL.Translate(ORIGINAL_IMAGE_RENDERED_SIZE, 0, 0); // 1 Pixel Shift

                }


                this.DrawMainView();



                GL.DisableClientState(ArrayCap.VertexArray);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);


            }

            GL.Flush();
            this.glControl1.SwapBuffers();
        }

        public void DrawMainView()
        {


        }
    }
}

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
        // Only one thread talk to file system at a time
        private static readonly object FileSystemL = new object();

        const int processors = 4;


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
        const int L1_SIZE = 21;
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

            this.bwLoad.DoWork += new DoWorkEventHandler(bwLoad_Work);
            this.bwSave.DoWork += new DoWorkEventHandler(bwSave_Work);


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
            if (this._LiveDrawIndex == this._SVG.LiveDrawLength)
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

                    int count = this._SVG.LiveDrawLength;
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






        #region SAVE
        private void bSave_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(this.textBox1.Text)) return;
            if (this.bwSave.IsBusy) return;

            String dir = Directory.GetParent(this.textBox1.Text).FullName;

            dir = Path.Combine(dir, "SKETCHES_PARSED");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

//                String file_name = "size_" + L1_SIZE + "___trail_" + SVG.TRAIL_LENGTH_PIXELS;
            String file_name = "size_" + L1_SIZE + "___all";

            dir = Path.Combine(dir, file_name);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var groups = Directory.GetDirectories(this.textBox1.Text);

            this.bwSave.RunWorkerAsync(new SaveThreadArgs(
                null,
                0,
                groups,
                dir,
                this.textBox1.Text));
        }

        private static void bwSave_Work(object sender, DoWorkEventArgs e)
        {
            var args = e.Argument as SaveThreadArgs;

            // One event is used for each Fibonacci object.
            ManualResetEvent[] doneEvents = new ManualResetEvent[processors];

            // Configure and start threads using ThreadPool.
            for (int i = 0; i < processors; i++)
            {
                doneEvents[i] = new ManualResetEvent(false);

                ThreadPool.QueueUserWorkItem(new SaveThreadArgs(
                    doneEvents[i],
                    processors,
                    args._Groups,
                    args._Dir,
                    args._RootDir
                    ).ThreadPoolCallback, i);
            }

            // Wait for all threads in pool to calculate.
            WaitHandle.WaitAll(doneEvents);

            Console.WriteLine("Save Complete");
        }

        private class SaveThreadArgs
        {
            public readonly int _Processors;
            public readonly ManualResetEvent _DoneEvent;
            public readonly String[] _Groups;
            public readonly String _Dir;
            public readonly String _RootDir;

            public SaveThreadArgs(
                ManualResetEvent done_event,                
                int processors,
                string[] groups,
                string dir,
                string root_dir)
            {
                this._DoneEvent = done_event;
                this._Processors = processors;
                this._Groups = groups;
                this._Dir = dir;
                this._RootDir = root_dir;
            }

            public void ThreadPoolCallback(Object threadContext)
            {
                int threadIndex = (int)threadContext;

                for (int i = 0; i < this._Groups.Length; i++)
                {
                    if (i % this._Processors != threadIndex) continue;

                    var group = this._Groups[i];
                    String group_name = group.Replace(this._RootDir + Path.DirectorySeparatorChar, "");

#if !DEBUG
                    try
#endif
                    {
                        DateTime dt = DateTime.Now;

                        String new_path = Path.Combine(this._Dir, group_name);

                        // if (String.Compare(group_name, "computer-mouse") < 0) continue; // Skip to

                        if (File.Exists(new_path)) File.Delete(new_path);

                        var ls = new List<object>();

                        foreach (var file in Directory.GetFiles(group))
                        {
                            var svg = new SVG(File.ReadAllText(file), file);
                            svg.InitializeImageChain(L1_SIZE);

                            var ob = new Dictionary<string, object>();
                            ob["file_name"] = Path.GetFileName(file);

                            foreach (var stuff in svg.SaveImageChain())
                                ob[stuff._LiveDrawIndex.ToString()] = stuff._Pixels;

                            ls.Add(ob);
                        }

                        var json_array = ls.ToArray();

                        lock (MainForm.FileSystemL)
                        {
                            using (var fs = new StreamWriter(File.Create(new_path)))
                            {
                                JsonParser.print(
                                json_array,
                                fs.Write,
                                fs.Write
                                );
                            }
                        }

                        Console.WriteLine(group_name + ": " + (DateTime.Now - dt).TotalMilliseconds);
                    }
#if !DEBUG
                    catch (Exception exc)
                    {
                        Console.WriteLine(group_name + ": " + exc.Message);
                    }
#endif
                }
            }
        }
        #endregion SAVE

        #region LOAD
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

//            String file_name = "size_" + L1_SIZE + "___trail_" + SVG.TRAIL_LENGTH_PIXELS;
            String file_name = "size_" + L1_SIZE + "___all";

            dir = Path.Combine(dir, file_name);

            if (!Directory.Exists(dir)) return;

            this.bwLoad.RunWorkerAsync(dir);
        }

        private static void bwLoad_Work(object sender, DoWorkEventArgs e)
        {
            int read_threads = 3;

            DateTime dt = DateTime.Now;

            ManualResetEvent[] events = new ManualResetEvent[read_threads];
            LoadFilePoolResponder[] thread_objects = new LoadFilePoolResponder[read_threads];
            var files = Directory.GetFiles(e.Argument as String);

            for (int i = 0; i < read_threads; i++)
            {
                var ls = new List<String>();
                for (int dex = 0; dex < files.Length; dex++)
                    if (dex % read_threads == i)
                        ls.Add(files[dex]);
                    else if (dex > 10) break;

                events[i] = new ManualResetEvent(false);
                thread_objects[i] = new LoadFilePoolResponder(ls.ToArray(), events[i]);
                ThreadPool.QueueUserWorkItem(thread_objects[i].ThreadPoolCallback, i);
            }

            WaitHandle.WaitAll(events);
            Console.WriteLine("All files loaded in " + (DateTime.Now - dt).TotalSeconds + " seconds");

            var data = new Dictionary<String, Dictionary<String, RawData[]>>();

            foreach (var thread_object in thread_objects)
                foreach (var kvp in thread_object._Data)
                    data[kvp.Key] = kvp.Value;

            e.Result = data;
        }

        private void bwLoad_Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            this._Data = e.Result as Dictionary<String, Dictionary<String, RawData[]>>;
            this.buttonFindNearest_Click(sender, e);
        }

        private class LoadFilePoolResponder
        {
            public Dictionary<String, Dictionary<String, RawData[]>> _Data { get; private set; }
            private ManualResetEvent _DoneEvent;
            private String[] _Files;

            public LoadFilePoolResponder(String[] files, ManualResetEvent doneEvent)
            {
                this._Files = files;
                _DoneEvent = doneEvent;
            }

            public void ThreadPoolCallback(Object threadContext)
            {
                int threadIndex = (int)threadContext;
                var data = new Dictionary<String, Dictionary<String, RawData[]>>();
                foreach (var file in this._Files)
                {
                    DateTime dt = DateTime.Now;

                    var file_dict = new Dictionary<String, RawData[]>();

                    String text;

                    lock (MainForm.FileSystemL) // One guy reads a file at a time
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
                this._Data = data;
                _DoneEvent.Set();
            }
        }
        #endregion




















        #region FindNearest
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

            if (this._LastNeighbor._Flipped) 
                this._LastNeighborKid.setFlipped(this._LastNeighbor._Index);

            this._NeighborSvgSize = this._LastNeighborKid.getRectangle(this._LastNeighbor._Index);

            this.glControl1.Invalidate();
        }

        private void nudFuture_ValueChanged(object sender, EventArgs e)
        {
            this.glControl1.Invalidate();
        }

        private void buttonFindNearest_Click(object sender, EventArgs e)
        {
            if (this._SVG == null) return;
            if (this._SVG._SectScaledFiltered == null) return;

            this.LoadData();

            if (this._Data == null) return;

            var sect = this._SVG._SectScaledFiltered;
            var im_size = sect.getPrefferedSize();

            /// 0 is white, 1 is blackest.
            var current_norm = new float[im_size.Width * im_size.Height];
            var current_flip = new float[im_size.Width * im_size.Height];
            int currrent_black = 0;

            int dex = 0;
            for (int y = 0; y < im_size.Height; y++)
                for (int x = 0; x < im_size.Width; x++)
                {
                    current_norm[dex] = 1 - sect[y, x];
                    if (current_norm[dex] > 0.99f) currrent_black++;
                    dex++;
                }

            dex = 0;
            for (int y = 0; y < im_size.Height; y++)
                for (int x = im_size.Width - 1; x >= 0; x--)
                {
                    current_flip[dex] = 1 - sect[y, x];
                    dex++;
                }

            if (currrent_black == 0) return; // No Data

           const int knn = 10;

            var top_points = new SortableData[knn + 1];
            for (int i = 0; i <= knn; i++)
                top_points[i] = SortableData.Minimum;

            // Get top knn neighbors (each from different file)
            foreach (var kvp_group in this._Data)
            {
                foreach (var kvp_file in kvp_group.Value)
                {
                    //if (this.lFileName.Text.Equals(kvp_file.Key)) continue;

                    var top_point_file = SortableData.Minimum;

                    // Find best for each file
                    foreach (var data in kvp_file.Value)
                    {
                        int lens = Math.Min(data._Data.Length, current_norm.Length);

                        float running_sum_norm = 0;
                        float running_sum_flip = 0;
                        int compare_black = 0;

                        for (int i = 0; i < lens; i++)
                        {
                            // char - 48 converts char 1 or 0 to the int value of 1 or 0
                            // 49 - char converts char 1 or 0 to the int value of 0 or 1
                            int read = (49 - data._Data[i]);
                            if (read == 1) compare_black++;
                            running_sum_norm += current_norm[i] * read;
                            running_sum_flip += current_flip[i] * read;
                        }

                        running_sum_norm /= Math.Max(compare_black, currrent_black);
                        running_sum_flip /= Math.Max(compare_black, currrent_black);

                        if (running_sum_norm > top_point_file._Sum)
                            top_point_file = new SortableData(
                                kvp_group.Key,
                                kvp_file.Key,
                                data,
                                running_sum_norm,
                                false);

                        if (running_sum_flip > top_point_file._Sum)
                            top_point_file = new SortableData(
                                kvp_group.Key,
                                kvp_file.Key,
                                data,
                                running_sum_flip,
                                true);
                    }

                    // Each file only gets one best
                    if (top_point_file._Data != null)
                    {
                        top_points[0] = top_point_file;
                        for (int offset = 0; // Bubble Sort
                            (offset < knn) && (top_points[offset]._Sum > top_points[offset + 1]._Sum);
                            offset++)
                        {
                            MiscUtil.Swap(
                                ref top_points[offset],
                                ref top_points[offset + 1]);
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

            const int control_y_inc = 10;
            int control_y = control_y_inc;
            for (int i = 0; i < knn; i++)
            {
                var tp = top_points[knn - i];
                if (tp._Data == null) break;
                var n = new Neighbor(disp_size, im_size, tp);
                this.pSelect.Controls.Add(n);
                n.pictureBox1.Click += N_Click;
                n.Top = control_y;
                n.Left = control_y_inc;
                control_y += control_y_inc + 20 + disp_size.Height; // 20 is for margin between neighbor picture box and container
            }

            this._NeighborSvgSizeOriginal = this._SVG.getRectangle(this._LiveDrawIndex);
        }
        #endregion FindNearest



























        private HashSet<GLControl> _Hashset = new HashSet<GLControl>();

        private void glControl1_Load(object sender, EventArgs e)
        {
            this._Hashset.Add(sender as GLControl);

            GL.LineWidth(1.0f);

            GL.MatrixMode(MatrixMode.Modelview); // Always the default.

            GL.DepthFunc(DepthFunction.Less);
            GL.CullFace(CullFaceMode.Back);

            GL.Enable(EnableCap.CullFace);
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);

            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            this.glControl1.MouseDown += new MouseEventHandler(GLMouseDown);
            this.glControl1.MouseUp += new MouseEventHandler(GLMouseUp);
            this.glControl1.MouseMove += new MouseEventHandler(GLMouseMove);

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

            GL.MatrixMode(MatrixMode.Projection);
            var proj = Matrix4.CreateOrthographicOffCenter(0, this.glControl1.Width, this.glControl1.Height, 0, 0, 1);
            GL.LoadMatrix(ref proj);

            GL.Viewport(0, 0, this.glControl1.Width, this.glControl1.Height);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Translate(0, 0, -0.5f);

            if (this.rbModePlayback.Checked)
            {
                this.DrawPlayback();
            }
            else
            {
                this.DrawSketching();
            }

            GL.Flush();
            this.glControl1.SwapBuffers();
        }

        public void DrawPlayback()
        {
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.Translate(0, 1, 0);
            if (this._SVG != null)
            {
                this._SVG.GL_BindBuffer();
                GL.EnableClientState(ArrayCap.VertexArray);

                if (true)
                { // Draw Main
                    GL.Translate(1, 0, 0); // 1 Pixel Shift
                    GL.PushMatrix();
                    {
                        GL.Color3(Color.White);
                        this.DrawRectange(0, 0, ORIGINAL_IMAGE_RENDERED_SIZE, ORIGINAL_IMAGE_RENDERED_SIZE, PrimitiveType.Quads);

                        GL.Scale(RENDER_LARGE_SCALE, RENDER_LARGE_SCALE, 1.0f);

                        GL.Color3(Color.Black);
                        if (this._LiveDrawIndex == -1) this._SVG.GL_DrawAll();
                        else  this._SVG.GL_DrawBeforeIncluding(this._LiveDrawIndex);

                    }
                    GL.PopMatrix();
                    GL.Translate(ORIGINAL_IMAGE_RENDERED_SIZE, 0, 0); // 1 Pixel Shift
                }

                if (true)
                { // Draw Trail
                    GL.Translate(1, 0, 0); // 1 Pixel Shift
                    GL.Color3(Color.White);
                    this.DrawRectange(0, 0, ORIGINAL_IMAGE_RENDERED_SIZE, ORIGINAL_IMAGE_RENDERED_SIZE, PrimitiveType.Quads);

                    GL.PushMatrix();
                    {
                        GL.Scale(RENDER_LARGE_SCALE, RENDER_LARGE_SCALE, 1.0f);
                        GL.Color4(1, 0, 0.0f, 0.25f);
                        GL.Enable(EnableCap.Blend);
                        this._SVG.GL_DrawBeforeIncluding(this._LiveDrawIndex);
                        GL.Disable(EnableCap.Blend);
                    }
                    GL.PopMatrix();

                    if (this._LastNeighbor != null)
                    {
                        GL.PushMatrix();

                        GL.Scale(RENDER_LARGE_SCALE, RENDER_LARGE_SCALE, 1.0f);

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

                        GL.Color4(0, 0, 1.0f, 0.1f);
                        this._LastNeighborKid.GL_DrawBeforeIncluding(end_index);
                        this._LastNeighborKid.GL_DrawAfterExcluding(end_index, future);

                        GL.Enable(EnableCap.Blend);
                        this._LastNeighborKid.GL_DrawAfterExcluding(end_index);
                        GL.Disable(EnableCap.Blend);


                        GL.PopMatrix();
                    }

                    GL.Translate(ORIGINAL_IMAGE_RENDERED_SIZE, 0, 0); // 1 Pixel Shift

                }

                GL.DisableClientState(ArrayCap.VertexArray);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }
        }

        public void DrawSketching()
        {
            GL.ClearColor(Color.White);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            this._SVG_Drawn.GL_BindBuffer();
            GL.EnableClientState(ArrayCap.VertexArray);

            GL.Color3(Color.Black);
            this._SVG_Drawn.GL_DrawAll();

            GL.DisableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void rbModeDraw_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton)
            {
                if ((sender as RadioButton).Checked)
                {
                    this.glControl1.Invalidate();
                }
            }
        }














        private SVG_Drawn _SVG_Drawn = new SVG_Drawn();

        private Point _MouseLastPoint;
        private bool _MouseDown = false;

        // Hand Drawn
        private void GLMouseMove(object sender, MouseEventArgs e)
        {
            if (this._MouseDown)
            {
                var p = e.Location;

                this._SVG_Drawn.Append(p, this._MouseLastPoint);
                this._MouseLastPoint = p;
                this.glControl1.Invalidate();
            }
        }

        private void GLMouseUp(object sender, MouseEventArgs e)
        {
            this._MouseDown = false;
        }

        private void GLMouseDown(object sender, MouseEventArgs e)
        {
            this._MouseDown = true;
            this._MouseLastPoint = e.Location;          
        }
    
    }
}

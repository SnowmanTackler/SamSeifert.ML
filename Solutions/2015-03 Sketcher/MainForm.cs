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
        public static readonly object FileSystemL = new object();

        const int processors = 4;
        const int knn = 25;


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


        private SVG _SVG_Playback;



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

            //this.pDrawTrailScaled.Hide();
            //this.pDrawTrailScaledFiltered.Hide();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (this.DesignMode) return;

            this.bwLoad.DoWork += new DoWorkEventHandler(bwLoad_Work);
            this.bwSave.DoWork += new DoWorkEventHandler(bwSave_Work);
            this.bwSearch.DoWork += new DoWorkEventHandler(bwSearch_Work);


            if (!ModifierKeys.HasFlag(Keys.Control)) // Set to default position!
                this.LoadFormState();

            this._SVG_Drawn.InitializeImageChain(L1_SIZE);


            var sz = L1_SIZE * L1_DRAW_SCALE;

            int x = 10;
            this.panelBottom.Height = x * 2 + sz;
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









        #region Playback
        private void bPlayback_Click(object sender, EventArgs e)
        {
            if (this._SVG_Playback == null) return;

            if (this.timerDraw.Enabled)
            {
                this.timerDraw.Enabled = false;
                this.findNearest();
            }
            else
            {
                if (this._LiveDrawIndex == -1)
                {
                    DateTime dt = DateTime.Now;

                    int count = this._SVG_Playback.LiveDrawLength;
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

        private int _LiveDrawIndex = -1;
        private void timerDraw_Tick(object sender, EventArgs e)
        {
            if (this._LiveDrawIndex == this._SVG_Playback.LiveDrawLength)
            {
                this.timerDraw.Enabled = false;
                this.findNearest();
                this._LiveDrawIndex = -1;
            }
            else
            {
                this._LiveDrawIndex++;

                this._SVG_Playback.SetImageChain(this._LiveDrawIndex);
                this._SVG_Playback._SectScaled.RefreshImage(ref this.DrawTrailScaled_Final);
                this._SVG_Playback._SectScaledFiltered.RefreshImage(ref this.DrawTrailScaledFiltered_Final);

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
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImageUnscaled(DrawTrailScaled_Final, Point.Empty);
        }

        Bitmap DrawTrailScaledFiltered_Final = new Bitmap(
            L1_SIZE * L1_DRAW_SCALE,
            L1_SIZE * L1_DRAW_SCALE,
            System.Drawing.Imaging.PixelFormat.Format24bppRgb);

        private void pDrawTrailScaledFiltered_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImageUnscaled(DrawTrailScaledFiltered_Final, Point.Empty);
        }
        #endregion Playback















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
            if (this._SVG_Playback != null) this._DeleteableSVGS.Add(this._SVG_Playback);
            this._LiveDrawIndex = -1;
            this._SVG_Playback = new SVG(file_text, file_name);
            this._SVG_Playback.InitializeImageChain(L1_SIZE);
            this.glControl1.Invalidate();
            Console.WriteLine("LOAD ELAPSED:" + (DateTime.Now - dt).TotalMilliseconds);

            this.timerDraw.Enabled = false;
            if (!this._SVG_Playback._ViewBox.Equals(new Rectangle(0, 0, 800, 800)))
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
            DateTime dt = DateTime.Now;

            ManualResetEvent[] events = new ManualResetEvent[processors];
            LoadFilePoolResponder[] thread_objects = new LoadFilePoolResponder[processors];
            var files = Directory.GetFiles(e.Argument as String);

            for (int i = 0; i < processors; i++)
            {
                var ls = new List<String>();
                for (int dex = 0; dex < files.Length; dex++)
                    if (dex % processors == i)
                        ls.Add(files[dex]);
//                    else if (dex > 20) break;

                events[i] = new ManualResetEvent(false);
                thread_objects[i] = new LoadFilePoolResponder(ls.ToArray(), events[i]);
                ThreadPool.QueueUserWorkItem(thread_objects[i].ThreadPoolCallback, null);
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

            public void ThreadPoolCallback(Object anything)
            {
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
        private Neighbor _Neighbor = null;
        private RectangleF _SearchSize;

        private void N_Click(object sender, EventArgs e)
        {
            var neighbor = (sender as Control).Parent as Neighbor;
            if (neighbor == null) return;

            if (this.DeleteNeighbor() != neighbor)
            { 
                this._Neighbor = neighbor;
                this._Neighbor.BackColor = Color.LimeGreen;
            }

            this.glControl1.Invalidate();
        }

        private void nudFuture_ValueChanged(object sender, EventArgs e)
        {
            this.glControl1.Invalidate();
        }

        private void findNearest()
        {
            this.timerNearest.Stop();
            this.timerNearest.Enabled = true;
            this.timerNearest.Start();
        }

        private void timerNearest_Tick(object sender, EventArgs e)
        {
            SVG svg = null;
            RectangleF r;

            if (this.rbModeDraw.Checked)
            {
                this._SVG_Drawn.SetImageChain();
                this._SVG_Drawn._SectScaled.RefreshImage(ref this.DrawTrailScaled_Final);
                this._SVG_Drawn._SectScaledFiltered.RefreshImage(ref this.DrawTrailScaledFiltered_Final);
                this.pDrawTrailScaled.Invalidate();
                this.pDrawTrailScaledFiltered.Invalidate();
                svg = this._SVG_Drawn;
                r = svg.getRectangle();
            }
            else
            {
                svg = this._SVG_Playback;
                r = svg.getRectangle(this._LiveDrawIndex);
            }


            this.LoadData();

            if (this._Data == null) return;
            if (this.bwSearch.IsBusy) return;
            if (svg == null) return;
            if (svg._SectScaledFiltered == null) return;

            var sect = svg._SectScaledFiltered;
            var im_size = sect.getPrefferedSize();

            /// 0 is white, 1 is blackest.
            var current_norm = new float[im_size.Width * im_size.Height];
            var current_flip = new float[im_size.Width * im_size.Height];
            int current_black = 0;

            int dex = 0;
            for (int y = 0; y < im_size.Height; y++)
                for (int x = 0; x < im_size.Width; x++)
                {
                    current_norm[dex] = 1 - sect[y, x];
                    if (current_norm[dex] > 0.99f) current_black++;
                    dex++;
                }

            dex = 0;
            for (int y = 0; y < im_size.Height; y++)
                for (int x = im_size.Width - 1; x >= 0; x--)
                {
                    current_flip[dex] = 1 - sect[y, x];
                    dex++;
                }

            if (current_black == 0) return; // No Data

            /// WILL RUN FROM THIS POINT ON
            this.timerNearest.Enabled = false;

            this.DeleteNeighbor();

            string key = null;

            if (this.cbRestrictSearch.Checked)
                key = this.rbModeDraw.Checked ? this.comboBoxSelection.SelectedItem as String : this.lGroup.Text;

            if ((key != null) && this._Data.ContainsKey(key))
            {
                var ls = new List<Tuple<String, Dictionary<String, RawData[]>>>[1];
                ls[0] = new List<Tuple<String, Dictionary<String, RawData[]>>>();
                ls[0].Add(new Tuple<String, Dictionary<String, RawData[]>>(key, this._Data[key]));
                this.bwSearch.RunWorkerAsync(new SearchArgs(ls, current_flip, current_norm, current_black, r));
            }
            else
            { 
                var ls = new List<Tuple<String, Dictionary<String, RawData[]>>>[processors];
                for (int i = 0; i < ls.Length; i++) ls[i] = new List<Tuple<String, Dictionary<String, RawData[]>>>();
                dex = 0;
                bool add = this.comboBoxSelection.Items.Count == 0;
                foreach (var kvp in this._Data)
                {
                    if (add) this.comboBoxSelection.Items.Add(kvp.Key);
                    ls[dex++ % ls.Length].Add(new Tuple<String, Dictionary<String, RawData[]>>( kvp.Key, kvp.Value));
                }
                if (add) this.updateComboEnabled();
                this.bwSearch.RunWorkerAsync(new SearchArgs(ls, current_flip, current_norm, current_black, r));
            }
        }

        private class SearchArgs
        {
            public readonly int current_black;
            public readonly float[] current_flip;
            public readonly float[] current_norm;
            public List<Tuple<string, Dictionary<string, RawData[]>>>[] ls;
            public RectangleF _Rect;

            public SearchArgs(
                List<Tuple<string, Dictionary<string, RawData[]>>>[] ls,
                float[] current_flip, 
                float[] current_norm,
                int current_black,
                RectangleF r)
            {
                this.ls = ls;
                this.current_flip = current_flip;
                this.current_norm = current_norm;
                this.current_black = current_black;
                this._Rect = r;
            }
        }

        private static void bwSearch_Work(object sender, DoWorkEventArgs e)
        {
            var arg = e.Argument as SearchArgs;
            var args = arg.ls;
            arg.ls = null;

            DateTime dt = DateTime.Now;

            ManualResetEvent[] events = new ManualResetEvent[args.Length];
            SearchPoolResponder[] thread_objects = new SearchPoolResponder[args.Length];

            for (int i = 0; i < args.Length; i++)
            {
                events[i] = new ManualResetEvent(false);
                thread_objects[i] = new SearchPoolResponder(args[i].ToArray(), events[i]);
                ThreadPool.QueueUserWorkItem(thread_objects[i].ThreadPoolCallback, arg);
            }

            WaitHandle.WaitAll(events);
            
            var top_points = new SortableData[knn + 1];
            for (int i = 0; i <= knn; i++)
                top_points[i] = SortableData.Minimum;

            foreach (var resp in thread_objects)
            {
                foreach (var top_point_file in resp._Data)
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

            e.Result = new Tuple<RectangleF, SortableData[]>(arg._Rect,  top_points.SubArray(1, knn));
        }

        private void bwSearch_Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            var response = e.Result as Tuple<RectangleF, SortableData[]>;

            var oldcs = new List<Control>();
            foreach (Control c in this.pSelect.Controls)
                oldcs.Add(c);

            this.pSelect.SuspendLayout();

            foreach (var c in oldcs)
            {
                c.SuspendLayout();
                (c as Neighbor).pictureBox1.Click -= N_Click;
                c.RemoveFromParent();
                c.Dispose();
            }

            var disp_size = 85;
            const int control_y_inc = 0;
            int control_y = control_y_inc;
            Neighbor first = null;
            for (int i = knn - 1; i >= 0; i--)
            {
                var tp = response.Item2[i];
                if (tp._Data == null) break;
                var n = new Neighbor(disp_size, tp);
                this.pSelect.Controls.Add(n);
                n.pictureBox1.Click += N_Click;
                n.Top = control_y;
                n.Left = control_y_inc;
                control_y += control_y_inc + 10 + disp_size;
                // 20 is for margin between neighbor picture box and container
                if (first == null) first = n;
            }

            this._SearchSize = response.Item1;

            foreach (Control c in this.pSelect.Controls)
                c.ResumeLayout();

            this.pSelect.ResumeLayout();

            if (first != null)
                this.N_Click(first.pictureBox1, EventArgs.Empty);
        }

        private class SearchPoolResponder
        {
            public SortableData[] _Data;
            private ManualResetEvent _DoneEvent;
            private Tuple<String, Dictionary<String, RawData[]>>[] _Groups;

            public SearchPoolResponder(Tuple<String, Dictionary<String, RawData[]>>[] files, ManualResetEvent doneEvent)
            {
                this._Groups = files;
                this._Data = new SortableData[files.Length];
                this._DoneEvent = doneEvent;
            }

            public void ThreadPoolCallback(Object o)
            {
                var arg = o as SearchArgs;

                var top_points = new SortableData[knn + 1];
                for (int i = 0; i <= knn; i++)
                    top_points[i] = SortableData.Minimum;

                foreach (var grp in this._Groups)
                {
                    string grp_name = grp.Item1;
                    foreach (var kvp_file in grp.Item2)
                    {
                        var top_point_file = SortableData.Minimum;

                        // Find best for each file
                        foreach (var data in kvp_file.Value)
                        {
                            int lens = Math.Min(data._Data.Length, arg.current_norm.Length);

                            float running_sum_norm = 0;
                            float running_sum_flip = 0;
                            int compare_black = 0;

                            for (int i = 0; i < lens; i++)
                            {
                                // char - 48 converts char 1 or 0 to the int value of 1 or 0
                                // 49 - char converts char 1 or 0 to the int value of 0 or 1
                                int read = (49 - data._Data[i]);
                                if (read == 1) compare_black++;
                                running_sum_norm += arg.current_norm[i] * read;
                                running_sum_flip += arg.current_flip[i] * read;
                            }

                            running_sum_norm /= Math.Max(compare_black, arg.current_black);
                            running_sum_flip /= Math.Max(compare_black, arg.current_black);

                            if (running_sum_norm > top_point_file._Sum)
                                top_point_file = new SortableData(
                                    grp_name,
                                    kvp_file.Key,
                                    data,
                                    running_sum_norm,
                                    false);

                            if (running_sum_flip > top_point_file._Sum)
                                top_point_file = new SortableData(
                                    grp_name,
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

                this._Data = top_points.SubArray(1, knn);

                _DoneEvent.Set();
            }
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
            if (this._SVG_Playback != null)
            {
                this._SVG_Playback.GL_BindBuffer();
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
                        if (this._LiveDrawIndex == -1) this._SVG_Playback.GL_DrawAll();
                        else  this._SVG_Playback.GL_DrawBeforeIncluding(this._LiveDrawIndex);

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
                        this._SVG_Playback.GL_DrawBeforeIncluding(this._LiveDrawIndex);
                        GL.Disable(EnableCap.Blend);
                    }
                    GL.PopMatrix();

                    if (this._Neighbor != null)
                    {
                        if (this._Neighbor._SVG != null)
                        {
                            GL.PushMatrix();

                            GL.Scale(RENDER_LARGE_SCALE, RENDER_LARGE_SCALE, 1.0f);

                            DrawNeighbor(
                                this._SearchSize,
                                this._Neighbor,
                                (int)Math.Round(this.nudFuture.Value)
                                );

                            GL.PopMatrix();
                        }
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

            GL.EnableClientState(ArrayCap.VertexArray);

            if (this._Neighbor != null)
            {
                if (this._Neighbor._SVG != null)
                {
                    GL.PushMatrix();

                    DrawNeighbor(
                        this._SearchSize,
                        this._Neighbor,
                        (int)Math.Round(this.nudFuture.Value)
                        );

                    GL.PopMatrix();
                }
            }

            GL.Color3(Color.Black);
            this._SVG_Drawn.GL_BindBuffer();
            this._SVG_Drawn.GL_DrawAll();

            foreach (var neigh in this._AddedNeighbors)
            {
                GL.PushMatrix();
                DrawNeighbor(neigh.Item1, neigh.Item2);
                GL.PopMatrix();
            }

            GL.DisableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private static void DrawNeighbor(RectangleF search_rect, Neighbor neighbor, int future = 9999999)
        {
            var or = neighbor.OriginalRect();
            float szc = search_rect.Width / or.Width;

            neighbor._SVG.GL_BindBuffer();

            int end_index = neighbor._Index;

            GL.Translate(
                search_rect.X,
                search_rect.Y,
                0);

            GL.Scale(szc, szc, 1);

            GL.Translate(
                -or.X,
                -or.Y,
                0);

            if (future != 9999999)
            {
                GL.Color4(0, 0, 1.0f, 0.15f);
                neighbor._SVG.GL_DrawBeforeIncluding(end_index);
                neighbor._SVG.GL_DrawAfterExcluding(end_index, future);

                GL.Enable(EnableCap.Blend);
            }

            neighbor._SVG.GL_DrawAll();
            GL.Disable(EnableCap.Blend);
        }

        private void rbModeDraw_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton)
            {
                if ((sender as RadioButton).Checked)
                {
                    this.bPlayback.Enabled = sender == this.rbModePlayback;
                    this.bDrawAdd.Enabled = !this.bPlayback.Enabled;
                    this.bDrawClear.Enabled = !this.bPlayback.Enabled;
                    this.updateComboEnabled();
                    this.DeleteNeighbor();
                    this._Neighbor = null;

                    this.findNearest();
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

                this.findNearest();

                this.glControl1.Invalidate();
            }
        }

        private void GLMouseUp(object sender, MouseEventArgs e)
        {
            this._MouseDown = false;
        }

        private void GLMouseDown(object sender, MouseEventArgs e)
        {
            this._MouseDown = this.rbModeDraw.Checked;
            this._MouseLastPoint = e.Location;          
        }

        private void cbRestrictSearch_CheckedChanged(object sender, EventArgs e)
        {
            this.findNearest();
            this.updateComboEnabled();
        }

        private void updateComboEnabled()
        {
            this.comboBoxSelection.Enabled = (this.comboBoxSelection.Items.Count > 0) &&
                this.cbRestrictSearch.Checked && this.rbModeDraw.Checked;
        }

        private void comboBoxSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.findNearest();
        }

        private void bDrawClear_Click(object sender, EventArgs e)
        {
            if (sender == this.bDrawClear) this._AddedNeighbors.Clear();
            this._SVG_Drawn.Clear();
            this.DeleteNeighbor();
            this.findNearest();
            this.glControl1.Invalidate();
        }

        private Neighbor DeleteNeighbor()
        {
            if (this._Neighbor != null)
            {
                this._Neighbor.BackColor = Color.Black;
                if (this._Neighbor._SVG != null)
                    this._DeleteableSVGS.Add(this._Neighbor._SVG);
            }
            var neigh = this._Neighbor;
            this._Neighbor = null;
            return neigh;
        }

        private List<Tuple<RectangleF, Neighbor>> _AddedNeighbors = new List<Tuple<RectangleF, Neighbor>>();
        private void bDrawAdd_Click(object sender, EventArgs e)
        {
            if (this._Neighbor != null)
            {
                this._AddedNeighbors.Add(new Tuple<RectangleF, Neighbor>(this._SearchSize, this._Neighbor));
                this.bDrawClear_Click(sender, e);
            }
        }


















        private void bExportClick(object sender, EventArgs e)
        {
            //            if (this._Data == null) return;
            String dir = Directory.GetParent(this.textBox1.Text).FullName;

            dir = Path.Combine(dir, "SKETCHES_PARSED");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            dir = Path.Combine(dir, "size_" + L1_SIZE + "___all_images");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var dir_test = Path.Combine(dir, "test");
            if (!Directory.Exists(dir_test))
                Directory.CreateDirectory(dir_test);

            var dir_train = Path.Combine(dir, "train");
            if (!Directory.Exists(dir_train))
                Directory.CreateDirectory(dir_train);

            var r = new Random();

            var sect = new SectArray(SectType.Gray, L1_SIZE, L1_SIZE);
            var bp = sect.getImage();

            foreach (var grp_kvp in this._Data)
            {
                var grp_name = grp_kvp.Key;

                var random_80 = new bool[grp_kvp.Value.Count];
                for (int i = 0; i < random_80.Length; i++) random_80[i] = false;
                int ctrue = 0;
                while (ctrue < 10)
                {
                    int i = r.Next() % random_80.Length;
                    if (!random_80[i])
                    {
                        random_80[i] = true;
                        ctrue++;
                    }
                }

                int dex_file = 0;
                foreach (var file_kvp in grp_kvp.Value)
                {
                    var dir_real = random_80[dex_file++] ? dir_test : dir_train;

                    var file_name = file_kvp.Key;
                    dir_real = Path.Combine(dir_real, grp_name);

                    if (!Directory.Exists(dir_real))
                        Directory.CreateDirectory(dir_real);

                    dir_real = Path.Combine(dir_real, Path.GetFileNameWithoutExtension(file_name) + "_");

                    foreach (var raw_data in file_kvp.Value)
                    {
                        int dex_image_coordinate = 0;
                        for (int y = 0; y < L1_SIZE; y++)
                            for (int x = 0; x < L1_SIZE; x++)
                                sect[y, x] = 49 - raw_data._Data[dex_image_coordinate++];

                        sect.RefreshImage(ref bp);
                        bp.Save(dir_real + (raw_data._Index * SVG.INCREMENT_DISTANCE) + "pixels.png",
                            System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
            }

        }
    }
}

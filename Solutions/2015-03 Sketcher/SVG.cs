using MathNet.Numerics.LinearAlgebra;
using SamSeifert.CSCV;
using SamSeifert.Utilities.FileParsing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solution
{
    public class SVG
    {
        /// <summary>
        /// How many pixels per broken up point.
        /// </summary>
        private const int INCREMENT_DISTANCE = 10;
        public const int TRAIL_LENGTH_PIXELS = 600; // Pixels
        public const int TRAIL_LENGTH = TRAIL_LENGTH_PIXELS
           / SVG.INCREMENT_DISTANCE; // Turns pixels into counts

        public readonly Rectangle _ViewBox = new Rectangle();
        public readonly Matrix<float> _Transform;
        public readonly Drawable[][] _Drawn;

        public SVG(string file_text, string file_path)
        {
            int viewboxes = 0;
            int transforms = 0;

            var list = new List<List<Drawable>>();

            var file = TagFile.ParseText(file_text);
            foreach (var b in file.Enumerate())
                if (b is TagFile)
                {
                    var tf = b as TagFile;

                    string outp;

                    switch (tf._Name)
                    {
                        case "svg":
                            if (tf._Params.TryGetValue("viewBox", out outp))
                            {
                                var strs = outp.Split(
                                    new char[] { ' ' },
                                    StringSplitOptions.RemoveEmptyEntries );

                                if (strs.Length == 4)
                                {
                                    viewboxes++;
                                    this._ViewBox.X = int.Parse(strs[0]);
                                    this._ViewBox.Y = int.Parse(strs[1]);
                                    this._ViewBox.Width = int.Parse(strs[2]);
                                    this._ViewBox.Height = int.Parse(strs[3]);
                                }
                                else Console.WriteLine("ViewBox Error: " + outp);
                            }
                            break;
                        case "g":
                            if (tf._Params.TryGetValue("transform", out outp))
                            {
                                transforms++;
                                this._Transform = this.ParseTransform(outp);
                            }
                            break;
                        case "path":
                            // Handled Later!
                            break;
                    }
                }

            if (transforms != 1)
            {
                string s = "SVG has " + transforms + " transforms";
                Console.WriteLine(s);
                throw new Exception(s);
            }

            if (viewboxes != 1)
            {
                string s = "SVG has " + transforms + " transforms";
                Console.WriteLine(s);
                throw new Exception(s);
            }

            foreach (var b in file.Enumerate())
                if (b is TagFile)
                {
                    var tf = b as TagFile;
                    switch (tf._Name)
                    {
                        case "path":
                            foreach (var kvp in tf._Params)
                            {
                                switch (kvp.Key)
                                {
                                    case "id":
                                        break;
                                    case "d":
                                        if (kvp.Value.Contains("NaN"))
                                        {
                                            Console.WriteLine(file_path + " contains some NaN's");
                                        } else this.ParsePath(this.Enumerate(kvp.Value), ref list);
                                        break;
                                    default:
                                        Console.WriteLine("Path Error: " + kvp.Key + ", " + kvp.Value);
                                        break;
                                }

                            }

                            break;
                    }
                }

            this._Drawn = new Drawable[list.Count][];
            int index = 0;
            foreach (var vecs in list)
                this._Drawn[index++] = vecs.ToArray();
        }

        IEnumerable<char> Enumerate(IEnumerable<char> values)
        {
            foreach (var c in values) yield return c;
            yield return 'M'; // End of file!
        }

        private void ParsePath(IEnumerable<char> values, ref List<List<Drawable>> list)
        {
            StringBuilder sb = new StringBuilder();
            char last_char = '0';

            float out_float;

            var floats = new List<float>();

            Func<float, float, Vector<float>> vec3 = (float x, float y) =>
            {
                return Vector<float>.Build.Dense(new float[] { x, y, 1 });
            };

            var current_point = vec3(0, 0);

            List<Drawable> vecs = null;
            if (list.Count > 0) vecs = list.Last();

            foreach (var c in values)
            {
                switch (c)
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '-': // Negative
                    case '.': // Decimal
                    case 'E': // Exponent
                    case 'e': // Exponent
                        sb.Append(c);
                        break;

                    case ' ': // Splitter
                    case ',': // Splitter
                        if (float.TryParse(sb.ToString(), out out_float))
                            floats.Add(out_float);
                        sb.Clear();
                        break;
                    default:
                        if (float.TryParse(sb.ToString(), out out_float)) floats.Add(out_float);
                        sb.Clear();

                        switch (last_char)
                        {
                            case '0':
                                break;
                            case 'M':
                                if (floats.Count == 2)
                                {
                                    current_point[0] = floats[0];
                                    current_point[1] = floats[1];

                                    vecs = new List<Drawable>();
                                    list.Add(vecs);
                                }
                                else
                                {
                                    string s = "SVG M has " + floats.Count + " floats";
                                    Console.WriteLine(s);
                                    throw new Exception(s);
                                }
                                break;
                            case 'C':
                                if (floats.Count == 6)
                                {
                                    var P1 = vec3(floats[0], floats[1]);
                                    var P2 = vec3(floats[2], floats[3]);
                                    var P3 = vec3(floats[4], floats[5]);

                                    vecs.Add(new BezierQuadratic(
                                        this._Transform * current_point,
                                        this._Transform * P1,
                                        this._Transform * P2,
                                        this._Transform * P3));

                                    current_point = P3;
                                }
                                else
                                {
                                    string s = "SVG C has " + floats.Count + " floats";
                                    Console.WriteLine(s);
                                    throw new Exception(s);
                                }
                                break;
                            case 'L':
                                if (floats.Count == 2)
                                {
                                    var new_point = vec3(floats[0], floats[1]);

                                    vecs.Add(new Line(
                                        this._Transform * current_point,
                                        this._Transform * new_point));

                                    current_point = new_point;
                                }
                                else
                                {
                                    string s = "SVG L has " + floats.Count + " floats";
                                    Console.WriteLine(s);
                                    throw new Exception(s);
                                }
                                break;
                            default:
                                Console.WriteLine("SVG Unrecognized Char: " + last_char + " " + floats.Count);
                                break;
                        }

                        last_char = c;
                        floats.Clear();
                        break;
                }
            }
        }

        private Matrix<float> ParseTransform(string inp)
        {
            var of_jaffar = Matrix<float>.Build.DenseIdentity(3);

            foreach (var str in inp.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = str.Split(new char[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0)
                    Console.WriteLine("ParseTransform Error: " + str);

                switch (parts[0])
                {
                    case "scale":
                        if (parts.Length == 2)
                        {
                            var mult = Matrix<float>.Build.DenseIdentity(3);
                            mult[0, 0] = float.Parse(parts[1]);
                            mult[1, 1] = mult[0, 0];
                            of_jaffar = of_jaffar * mult;
                        }
                        else goto default;
                        break;
                    case "translate":
                        if (parts.Length == 3)
                        {
                            var mult = Matrix<float>.Build.DenseIdentity(3);
                            mult[0, 2] = float.Parse(parts[1]);
                            mult[1, 2] = float.Parse(parts[2]);
                            of_jaffar = of_jaffar * mult;
                        }
                        else goto default;
                        break;
                    default:
                        Console.WriteLine("ParseTransform Error: " + str);
                        break;

                }
            }

            return of_jaffar;
        }




        private Drawable[] _LiveDraw = null;
        public Drawable[] LiveDraw
        {
            get
            {
                if (this._LiveDraw == null)
                {
                    var ls = new List<Drawable>();
                    DrawableGroup last_group = null;
                    float last_group_length = 0;

                    foreach (var continuity in this._Drawn)
                        foreach (var element in continuity)
                            element.BreakIntoSmallPortions(
                                ref ls,
                                ref last_group,
                                ref last_group_length,
                                SVG.INCREMENT_DISTANCE);

                    this._LiveDraw = ls.ToArray();
                }
                return this._LiveDraw;
            }

        }


        public RectangleF getRectangle(int end_index)
        {
            RectangleF ret = RectangleF.Empty;

            float min_x = float.MaxValue;
            float max_x = float.MinValue;
            float min_y = float.MaxValue;
            float max_y = float.MinValue;

            for (int i = Math.Max(0, end_index - SVG.TRAIL_LENGTH); i < end_index; i++)
            {
                this.LiveDraw[i].UpdateBounds(
                    ref min_x,
                    ref max_x,
                    ref min_y,
                    ref max_y);
            }

            float range_x = (float)(Math.Ceiling(max_x) - Math.Floor(min_x));
            float range_y = (float)(Math.Ceiling(max_y) - Math.Floor(min_y));

            if (float.IsNaN(range_x) || float.IsInfinity(range_x) || (range_x > 1000)) return ret;
            if (float.IsNaN(range_y) || float.IsInfinity(range_y) || (range_y > 1000)) return ret;

            if (range_x + range_y == 0) return ret;


            float biggest_range = Math.Max(range_x, range_y);

            ret.X = (biggest_range - range_x) / 2 - min_x;
            ret.Y = (biggest_range - range_y) / 2 - min_y;
            ret.Width = biggest_range;
            ret.Height = biggest_range;

            return ret;
        }

        public void getImageForSize(
            ref Bitmap bp, 
            int image_size,
            int end_index, 
            bool custom_scale = true)
        {

            bool resize = true;

            if (bp != null)
            {
                if (bp.Size.Equals(new Size(image_size, image_size))) resize = false;
                else bp.Dispose();
            }

            if (resize) bp = new Bitmap(image_size, image_size, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            using (var g = Graphics.FromImage(bp))
            {
                float scale = 1.0f;

                g.Clear(Color.White);
                g.ResetTransform();

                if (custom_scale)
                {
                    var rect = this.getRectangle(end_index);

                    /// Should have width == height but w / e
                    scale = image_size / Math.Max(rect.Width, rect.Height);

                    g.ScaleTransform(scale, scale);
                    g.TranslateTransform(rect.X, rect.Y);
                }
                else
                {
                    float biggest_range = Math.Max(this._ViewBox.Width, this._ViewBox.Height);
                    scale = image_size / biggest_range;
                    g.ScaleTransform(scale, scale);
                }

                using (var pen = new Pen(Color.Black, 1 / scale))
                {
                    for (int i = Math.Max(0, end_index - TRAIL_LENGTH); i < end_index; i++)
                    {
                        this.LiveDraw[i].Draw(pen, g);
                    }
                }
            }
        }

















        int _ImageChainSize = 0;
        public void InitializeImageChain(int size)
        {
            this._ImageChainSize = size;
            this._BitmapDrawnScaled = new Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

        }

        const int DrawTrailScaledFiltered_Size = 2;

        Bitmap _BitmapDrawnScaled;
        public Sect _SectScaled;
        Sect _SectScaledFiltered_Temp1;
        Sect _SectScaledFiltered_Temp2;
        Sect _SectScaledFiltered_Temp3;
        public Sect _SectScaledFiltered;

        static readonly Sect GaussianX = SectArray.Build.Gaussian.NormalizedSum1D(
            SectType.Gray,
            1.0f,
            1 + 2 * DrawTrailScaledFiltered_Size);
        static readonly Sect GaussianY = GaussianX.Transpose();

        public void SetImageChain(int index)
        {
            if (this._ImageChainSize == 0) return; // Call InitializeImageChain first!

            this.SetImageChain1(index);
            this.SetImageChain2();
        }

        private void SetImageChain1(int index)
        {
            this.getImageForSize(ref _BitmapDrawnScaled, this._ImageChainSize, index);
            this._SectScaled = SectHolder.FromImage(_BitmapDrawnScaled, true);

        }

        private void SetImageChain2()
        {
            SingleImage.PaddingAdd(
                this._SectScaled,
                PaddingType.Unity,
                DrawTrailScaledFiltered_Size,
                ref this._SectScaledFiltered_Temp1);

            MultipleImages.Convolute(this._SectScaledFiltered_Temp1, GaussianX, ref this._SectScaledFiltered_Temp2);
            MultipleImages.Convolute(this._SectScaledFiltered_Temp2, GaussianY, ref this._SectScaledFiltered_Temp3);

            SingleImage.PaddingOff(
                this._SectScaledFiltered_Temp3,
                DrawTrailScaledFiltered_Size,
                ref this._SectScaledFiltered);
        }


        public class Descriptor
        {
            public readonly int _LiveDrawIndex;
            public readonly string _Pixels;

            public Descriptor(char[] pixels, int live_draw_index)
            {
                this._Pixels = new String(pixels);
                this._LiveDrawIndex = live_draw_index;
            }
        }

        public List<Descriptor> SaveImageChain()
        {
            if (this._ImageChainSize == 0)
            {
                Console.WriteLine("Call InitializeImageChain first!");
                return new List<Descriptor>();
            }

            int num_pixels = this._ImageChainSize * this._ImageChainSize;

            var pixels = new char[num_pixels];
            for (int pixel = 0; pixel < num_pixels; pixel++) pixels[pixel] = '0';

            this.SetImageChain1(0); // Initializes Sects

            var sz = this._SectScaled.getPrefferedSize();
            int w = sz.Width;
            int h = sz.Height;


            var ls = new List<Descriptor>(this.LiveDraw.Length);

            for (int i = 1; i < this.LiveDraw.Length; i++)
            {
                this.SetImageChain1(i);

                int pixel = 0;
                bool new_img = false;

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        var next = (this._SectScaled[y, x] > 0.5f) ? '1' : '0';

                        if (next != pixels[pixel])
                        {
                            pixels[pixel] = next;
                            new_img = true;
                        }

                        pixel++;
                    }
                }

                if (new_img)
                {
                    ls.Add(new Descriptor(pixels, i));
                }
            }

            return ls;
        }
    }
}

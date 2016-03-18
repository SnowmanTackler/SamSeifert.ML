using MathNet.Numerics.LinearAlgebra;
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
        public const int INCREMENT_DISTANCE = 10;
        public const int TRAIL_LENGTH = 400 // Pixels
           / SVG.INCREMENT_DISTANCE; // Turns pixels into counts

        public readonly Rectangle _ViewBox = new Rectangle();
        public readonly Matrix<float> _Transform;
        public readonly Drawable[][] _Drawn;

        public SVG(string sample)
        {
            int viewboxes = 0;
            int transforms = 0;

            var list = new List<List<Drawable>>();

            var file = TagFile.ParseText(sample);
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
                                        this.ParsePath(this.Enumerate(kvp.Value), ref list);
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


        public void getForSize(
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

                    float range_x = max_x - min_x;
                    float range_y = max_y - min_y;

                    if (float.IsNaN(range_x) || float.IsInfinity(range_x) || (range_x > 1000)) return;
                    if (float.IsNaN(range_y) || float.IsInfinity(range_y) || (range_y > 1000)) return;

                    if (range_x + range_y == 0) return;

                    float biggest_range = Math.Max(range_x, range_y);

                    scale = image_size / biggest_range;

                    g.ScaleTransform(scale, scale);
                    g.TranslateTransform(
                        (biggest_range - range_x) / 2 - min_x,
                        (biggest_range - range_y) / 2 - min_y);
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
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML
{
    // This is a collection of data points.
    public class DataImported
    {
        public string _FileName;
        private float[][] _DataPoints;

        public int _Columns
        {
            get
            {
                return this._DataPoints[0].Length;
            }
        }

        public int _Rows
        {
            get
            {
                return this._DataPoints.Length;
            }
        }

        public DataImported(String file_name, bool transpose, out string err)
        {
            this._FileName = file_name;

            var _StreamReader = new StreamReader(file_name);

            var sb = new StringBuilder();
            var ls = new List<float>();
            var points = new List<float[]>();

            bool new_line_comma = false;

            while (true)
            {
                bool quit = false;
                char current;

                if (_StreamReader.EndOfStream)
                {
                    if (new_line_comma)
                    {
                        quit = true;
                        current = '\n';
                    }
                    else
                    {
                        current = ',';
                        new_line_comma = true;
                    }
                }
                else if (new_line_comma)
                {
                    current = '\n';
                    new_line_comma = false;
                }
                else
                {
                    current = (char)_StreamReader.Read();
                    if (current == '\n') // If lines don't end with a comma, end them with a comma
                    {
                        current = ',';
                        new_line_comma = true;
                    }
                }

                switch (current)
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
                        sb.Append(current);
                        break;
                    case ',': // Comma Separated Text File
                        String txt = sb.ToString().Trim();
                        sb.Length = 0; // Reset String Thing
                        if (txt.Length > 0)
                        {
                            float next;
                            if (float.TryParse(sb.ToString(), out next))
                            {
                                err = "Couln't Parse Float: \"" + txt + "\"";
                                return;
                            }
                            ls.Add(next);
                        }
                        break;
                    case '\n': // Comma Separated Text File
                        if (ls.Count > 0)
                        {
                            var np = ls.ToArray();
                            ls.Clear();

                            if (points.Count != 0)
                                if (points[0].Length != np.Length)
                                {
                                    err = "Different number of data points per line";
                                    return;
                                }

                            points.Add(np);
                        }
                        break;
                }

                if (quit) break;
            }

            if (points.Count == 0)
            {
                err = file_name + " has no data points!";
                return;
            }

            if (transpose)
            {
                this._DataPoints = new float[points[0].Length][];

                for (int i = 0; i < this._DataPoints.Length; i++)
                {
                    this._DataPoints[i] = new float[points.Count];
                    int j = 0;
                    foreach (var dp in points)
                        this._DataPoints[i][j++] = dp[i];                            
                }

                err = null;
            }
            else
            {
                this._DataPoints = points.ToArray();
                err = null;
            }
        }
    }
}

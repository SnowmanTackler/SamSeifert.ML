using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.ML.Datas
{
    // This is a collection of data points.
    public class ImportCSV
    {
        public string _FileName;
        public float[][] _DataPoints;

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

        private IEnumerable<char> ReadStream(StreamReader sr)
        {
            while (!sr.EndOfStream)
            {
                char current = (char) sr.Read();
                if (current == '\n') yield return ','; // Commas at end of each line!
                yield return current;
            }
            yield return '\n'; // Endline at end of each file!
            sr.Dispose();
        }

        public ImportCSV(String file_name, bool transpose, out string err)
        {
            this._FileName = file_name;

            var sb = new StringBuilder();
            var ls = new List<float>();
            var points = new List<float[]>();


            foreach (var current in this.ReadStream(new StreamReader(file_name)))
            {
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
                            if (!float.TryParse(txt, out next))
                            {
                                var txttemp = txt.Replace("-", "E-");
                                if (txttemp[0] == 'E') txttemp = txttemp.Substring(1);
                                if (!float.TryParse(txttemp, out next))
                                {
                                    err = "Couln't Parse Float: \"" + txt + "\" " + (int)txt[0];
                                    return;
                                }
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

        private ImportCSV(float[][] data)
        {
            this._DataPoints = data;
        }

        /// <summary>
        /// Randomly splits the data into a train and test set.
        /// </summary>
        /// <param name="count_train"></param>
        /// <param name="count_test"></param>
        /// <returns></returns>
        internal Datas.ImportCSV[] Split(int count_train, int count_test)
        {
            if (count_test + count_train != this._Rows)
            {
                String s = "Can't Split Data";
                Console.WriteLine(s);
                throw new Exception(s);
            }

            var train = new float[count_train][];
            var test = new float[count_test][];


            int dex_train = 0;
            int dex_test = 0;
            int main_dex = 0;

            foreach (var b in Util.PickRandom(count_train, count_test))
            {
                if (b) train[dex_train++] = this._DataPoints[main_dex++];
                else    test[dex_test++]  = this._DataPoints[main_dex++];
            }

            return new ImportCSV[] { new ImportCSV(train), new ImportCSV(test) };
        }
    }
}

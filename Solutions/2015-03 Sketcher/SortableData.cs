using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solution
{
    public class RawData
    {
        public readonly int _Index;
        public readonly string _Data = null;

        public RawData(int index, string data)
        {
            this._Index = index;
            this._Data = data;
        }
    }

    public class SortableData
    {
        /// <summary>
        /// Used For Sorting.
        /// </summary>
        public float _Sum;
        public readonly string _FileName;
        public readonly RawData _Data = null;
        public readonly string _GroupName;

        private SortableData(float distance)
        {
            this._Sum = distance;
        }

        public SortableData(string group_name, string file_name, RawData d, float distance)
        {
            this._FileName = file_name;
            this._GroupName = group_name;
            this._Data = d;
            this._Sum = distance;
        }

        public static SortableData Minimum
        {
            get
            {
                return new SortableData(float.MinValue);
            }
        }

        public static SortableData Maximum
        {
            get
            {
                return new SortableData(float.MaxValue);
            }
        }

    }


}

using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SamSeifert.MathNet.Numerics.Extensions;

namespace solution
{
    public interface Drawable
    {
        void Draw(Pen pen, Graphics graphics);

        void BreakIntoSmallPortions(
            ref List<Drawable> ls,
            ref DrawableGroup last_group,
            ref float last_group_length,
            int max_length);
        void UpdateBounds(ref float min_x, ref float max_x, ref float min_y, ref float max_y);
    }






    /// <summary>
    /// Used to control bezier curve resolution
    /// </summary>
    public class Contour
    {
        public static int Sections = 1;
    }






    /// <summary>
    /// Just a line man!
    /// </summary>
    public class Line : Drawable
    {
        private Vector<float> First;
        private Vector<float> Second;

        public Line(Vector<float> vector1, Vector<float> vector2)
        {
            this.First = vector1.SubVector(0, 2);
            this.Second = vector2.SubVector(0, 2);
        }

        public void Draw(Pen pen, Graphics graphics)
        {
            graphics.DrawLine(pen, First[0], First[1], Second[0], Second[1]);
        }

        public void BreakIntoSmallPortions(
            ref List<Drawable> ls,
            ref DrawableGroup last_group,
            ref float last_group_length,
            int max_length)
        {
            float included_length = 0;
            var normalized_difference = (this.Second - this.First);
            float this_length = normalized_difference.Normalize();

            if (last_group != null)
            {
                float length_left = max_length - last_group_length - this_length;
                if (length_left > 0)
                {
                    last_group._List.Add(this);
                    last_group_length += this_length;
                    return;
                }
                else if (length_left == 0)
                {
                    last_group._List.Add(this);
                    last_group = null;
                    return;
                }
                else
                {
                    included_length = max_length - last_group_length;
                    ls.Add(new Line(this.First, this.First + normalized_difference * included_length));
                    last_group = null;
                    // Continue
                }
            }

            while (included_length + max_length <= this_length)
            {
                ls.Add(new Line(
                    this.First + normalized_difference * included_length,
                    this.First + normalized_difference * (included_length + max_length)));
                included_length += max_length;
            }

            if (included_length == this_length) return;

            last_group_length = this_length - included_length;
            last_group = new DrawableGroup(new Line(
                this.First + normalized_difference * included_length,
                this.Second));

            ls.Add(last_group);
        }

        public void UpdateBounds(
            ref float min_x,
            ref float max_x,
            ref float min_y,
            ref float max_y)
        {
            min_x = Math.Min(this.First[0], min_x);
            min_x = Math.Min(this.Second[0], min_x);

            max_x = Math.Max(this.First[0], max_x);
            max_x = Math.Max(this.Second[0], max_x);

            min_y = Math.Min(this.First[1], min_y);
            min_y = Math.Min(this.Second[1], min_y);

            max_y = Math.Max(this.First[1], max_y);
            max_y = Math.Max(this.Second[1], max_y);
        }
    }




    /// <summary>
    /// Quadratic Bezier Curve
    /// </summary>
    public class BezierQuadratic : Drawable
    {
        private Vector<float> P0;
        private Vector<float> P1;
        private Vector<float> P2;
        private Vector<float> P3;

        public BezierQuadratic(Vector<float> vector1, Vector<float> vector2, Vector<float> vector3, Vector<float> vector4)
        {
            this.P0 = vector1.SubVector(0, 2);
            this.P1 = vector2.SubVector(0, 2);
            this.P2 = vector3.SubVector(0, 2);
            this.P3 = vector4.SubVector(0, 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val">0 to 1</param>
        /// <returns></returns>
        private Vector<float> GetLocation(float t)
        {
            float t_2 = t * t;
            float t_3 = t * t_2;

            float one_minus_t = 1 - t;
            float one_minus_t_2 = one_minus_t * one_minus_t;
            float one_minus_t_3 = one_minus_t * one_minus_t_2;

            return
                this.P0 * one_minus_t_3 +
                this.P1 * one_minus_t_2 * t * 3 +
                this.P2 * one_minus_t * t_2 * 3 +
                this.P3 * t_3;
        }

        private IEnumerable<Vector<float>> Enumerate(int lens)
        {
            for (int i = 0; i <= lens; i++)
            {
                yield return this.GetLocation(i / (float)lens);
            }
        }

        public void Draw(Pen pen, Graphics graphics)
        {
            bool first = true;

            Vector<float> current_point = null;

            foreach (var e in this.Enumerate(Contour.Sections))
            {
                if (!first) graphics.DrawLine(pen, current_point[0], current_point[1], e[0], e[1]);
                first = false;
                current_point = e;
            }
        }

        private float GetLength(int lens)
        {
            bool first = true;

            float length = 0;
            Vector<float> current_point = null;

            foreach (var e in this.Enumerate(lens))
            {
                if (!first) length += (e - current_point).Normalize();
                first = false;
                current_point = e;
            }

            return length;
        }

        public void BreakIntoSmallPortions(
            ref List<Drawable> ls,
            ref DrawableGroup last_group,
            ref float last_group_length,
            int max_length)
        {
            float straight_length = (this.P3 - this.P0).Normalize();

            int lens = Math.Max(1, (int)Math.Round(straight_length / max_length));

            float this_length = this.GetLength(lens);

            float included_length = 0;

            if (last_group != null)
            {
                float length_left = max_length - last_group_length - this_length;
                if (length_left > 0)
                {
                    last_group._List.Add(new Line(this.P0, this.P3));
                    last_group_length += this_length;
                    return;
                }
                else if (length_left == 0)
                {
                    last_group._List.Add(new Line(this.P0, this.P3));
                    last_group = null;
                    return;
                }
                else
                {
                    included_length = max_length - last_group_length;
                    ls.Add(new Line(this.P0, this.GetLocation(included_length / this_length)));
                    last_group = null;
                    // Continue
                }
            }

            while (included_length + max_length <= this_length)
            {
                ls.Add(new Line(
                    this.GetLocation(included_length / this_length),
                    this.GetLocation((included_length + max_length) / this_length)));
                included_length += max_length;
            }

            if (included_length == this_length) return;

            var second_last_point = this.GetLocation(included_length / this_length);

            last_group_length = (second_last_point - this.P3).Normalize();
            last_group = new DrawableGroup(new Line(
                second_last_point,
                this.P3));

            ls.Add(last_group);
        }

        public void UpdateBounds(
            ref float min_x,
            ref float max_x,
            ref float min_y,
            ref float max_y)
        {
            throw new Exception("Should never happen");
        }
    }






    /// <summary>
    /// Groups of lines (used BreakIntoSmallPortions for line and bezier curve) whose length sums
    /// to required length of BreakIntoSmallPortions
    /// </summary>
    public class DrawableGroup : Drawable
    {
        public readonly List<Drawable> _List = new List<Drawable>();

        public DrawableGroup(params Drawable[] drawables)
        {
            this._List.AddRange(drawables);
        }

        public void Draw(Pen pen, Graphics graphics)
        {
            foreach (var element in this._List)
                element.Draw(pen, graphics);
        }

        public void BreakIntoSmallPortions(
            ref List<Drawable> ls,
            ref DrawableGroup last_group, 
            ref float last_group_length,
            int max_length)
        {
            throw new Exception("Should never happen");
        }

        public void UpdateBounds(
            ref float min_x,
            ref float max_x,
            ref float min_y,
            ref float max_y)
        {
            foreach (var element in this._List)
                element.UpdateBounds(
                    ref min_x,
                    ref max_x,
                    ref min_y,
                    ref max_y);
        }
    }
}

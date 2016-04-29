using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using MathNet.Numerics.LinearAlgebra;
using System.Drawing;

namespace solution
{
    public class SVG_Drawn : SVG
    {
        public SVG_Drawn() : base()
        {


        }

        private bool _NewData = false;
        private int _LiveDrawLength = 0;

        private readonly Vector2[] _Data = new Vector2[100000];

        public override int LiveDrawLength { get { return this._LiveDrawLength / 2; } }
        public override IEnumerable<Drawable> LiveDraw
        {
            get
            {
                for (int i = 0; i < this._LiveDrawLength; i += 2)
                {
                    yield return new DrawableLine(
                        Vector<float>.Build.DenseOfArray(new float[] { this._Data[i].X, this._Data[i].Y }),
                        Vector<float>.Build.DenseOfArray(new float[] { this._Data[i + 1].X, this._Data[i + 1].Y }));
                }
            }
        }

        protected override int VertexBufferDataLength { get; set; }
        public override void GL_BindBuffer()
        {
            if (this.VertexBufferUnallocated)
            {
                this.VertexBufferUnallocated = false;
                int bufferSize;
                int bufferSizeE;

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                var data = this._Data;
                this.VertexBufferDataLength = data.Length;

                bufferSizeE = data.Length * Vector2.SizeInBytes;
                GL.GenBuffers(1, out this.VertexBufferIndex);
                GL.BindBuffer(BufferTarget.ArrayBuffer, this.VertexBufferIndex);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(bufferSizeE), data, BufferUsageHint.StaticDraw);
                GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
                if (bufferSizeE != bufferSize) return;
            }
            else GL.BindBuffer(BufferTarget.ArrayBuffer, this.VertexBufferIndex);

            if (this._NewData)
            {
                this._NewData = false;
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, this._LiveDrawLength * Vector2.SizeInBytes, this._Data);
            }

            GL.VertexPointer(2, VertexPointerType.Float, Vector2.SizeInBytes, IntPtr.Zero);
        }

        internal void Append(Point newp, Point oldp)
        {
            if (this._LiveDrawLength < this._Data.Length - 1)
            {
                this._Data[this._LiveDrawLength] = new Vector2(oldp.X, oldp.Y);
                this._LiveDrawLength++;
                this._Data[this._LiveDrawLength] = new Vector2(newp.X, newp.Y);
                this._LiveDrawLength++;
                this._NewData = true;
            }
        }
    }
}

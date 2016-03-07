using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace solution
{
    public class PanelOverlay : Panel
    {
        bool invalidate_back = true;

        public PanelOverlay() : base()
        {

        }

        public void ClearBackground()
        {
            this.invalidate_back = true;
            this.Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.invalidate_back)
            {
                base.OnPaintBackground(e);
                this.invalidate_back = false;
            }

            base.OnPaint(e);
        }
    }
}

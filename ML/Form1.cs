using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ML
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Enabled = false;
            this.dataLoader1.Loaded();
        }

        private void LoaderDataPop(DataImported[] di)
        {
            this.dataSplitter1.SetData(di);
        }

        private void dataSplitter1_DataPop(DataImported train, DataImported test)
        {
            this.dataLabeler1.SetData(train, test);

        }
    }
}

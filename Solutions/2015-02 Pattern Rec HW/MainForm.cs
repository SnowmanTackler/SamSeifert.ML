using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SamSeifert.ML;
using SamSeifert.ML.Controls;
using SamSeifert.ML.Data;

namespace solution
{
    public partial class MainForm : Form
    {
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
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            SamSeifert.Utilities.Logger.WriteLine = this.WriteLine;

            this.timer1.Enabled = false;
            this.dataLoader1.Loaded();
        }

        private void LoaderDataPop(ImportCSV[] di)
        {
            this.dataSplitter1.SetData(di);
        }

        private void dataSplitter1_DataPop(ImportCSV train, ImportCSV test)
        {
            this.dataLabeler1.SetData(train, test);
        }

        private void dataLabeler1_DataPop(Useable train, Useable test)
        {
            this.dataDistributionNormalizer1.SetData(train, test);
        }

        private void dataDistributionNormalizer1_DataPop(Useable train, Useable test)
        {
            this.dataNormalizer1.SetData(train, test);
        }

        private void dataNormalizer1_DataPop(Useable train, Useable test)
        {
            this.dataPreProcess1.SetData(train, test);
        }

        private void dataPreProcess1_DataPop(Preprocess.Transform fi)
        {
            this.dataTransformer1.SetData(fi);
        }

        private void dataTransformer1_DataPop(Useable train, Useable test)
        {
            this.dataTrainer1.SetData(train, test);
        }

        public void WriteLine(String s)
        {
            if (this.IsDisposed) return;
            else if (this.InvokeRequired) this.Invoke(new Action((() => { this.WriteLine(s); })));
            else
            {
                Console.WriteLine(s);
                this.textBox1.AppendText(s.Trim() + Environment.NewLine);
            }
        }
    }
}

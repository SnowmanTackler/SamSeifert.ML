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
            this.labeler1.SetData(di);
        }

        private void labeler1_DataPop(Useable[] di)
        {
            this.splitter1.SetData(di);
        }

        private void splitter1_DataPop(Useable[] data)
        {
            this.trainingDataLabelNormalizer1.SetData(data);
        }

        private void trainingDataLabelNormalizer1_DataPop(Useable[] data)
        {
            this.valueNormalizer1.SetData(data);
        }

        private void valueNormalizer1_DataPop(Useable[] data)
        {
            this.preprocess1.SetData(data);
        }

        private void preprocess1_DataPop(Preprocess.Transform fi)
        {
            this.transformer1.SetData(fi);
        }

        private void transformer1_DataPop(Useable[] data)
        {
            this.dataTrainer1.SetData(data[0], data[1]);

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

        private void bResetText_Click(object sender, EventArgs e)
        {
            this.textBox1.Clear();
        }
    }
}

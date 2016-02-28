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
        private static Form1 Instance_;
        public static Form1 Instance
        {
            get
            {
                if (Instance_ == null) Instance_ = new Form1();
                return Instance_;                
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

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

        private void dataLabeler1_DataPop(DataUseable train, DataUseable test)
        {
            this.dataNormalizer1.SetData(train, test);
        }

        private void dataNormalizer1_DataPop(DataUseable train, DataUseable test)
        {
            this.dataPreProcess1.SetData(train, test);
        }

        private void dataPreProcess1_DataPop(DataPreProcess.PreProcessTransform fi)
        {
            this.dataTransformer1.SetData(fi);
        }

        private void dataTransformer1_DataPop(DataUseable train, DataUseable test)
        {
            this.dataTrainer1.SetData(train, test);
        }

        public void WriteLine(String s)
        {
            if (this.InvokeRequired)
                this.Invoke(new Action((() => { this.WriteLine(s); })));
            else
            {
                Console.WriteLine(s);
                this.textBox1.AppendText(s.Trim() + Environment.NewLine);
            }
        }
    }
}

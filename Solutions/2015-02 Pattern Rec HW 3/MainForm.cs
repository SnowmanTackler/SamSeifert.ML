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
using SamSeifert.ML.Datas;

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










        public struct IterationValue
        {
            public int _K;
            public int _NumberTest;
            public bool _UseGlobal;

            public IterationValue(int k, int test)
            {
                this._UseGlobal = false;
                this._K = k;
                this._NumberTest = Math.Max(0, test);
            }

            public IterationValue(int k)
            {
                this._UseGlobal = true;
                this._K = k;
                this._NumberTest = 0;
            }
        }

        public IEnumerable<int> enumerate_K()
        {
            for (int k = 1; k <= 15; k++)
                yield return k;
        }

        public IEnumerable<IterationValue> enumerate_folds(int data_length)
        {
            for (int i = 0; i < 5; i++) // 5 Fold Cross Validation
            {
                if (i != 0) this.WriteLine("];");
                this.WriteLine("");
                this.WriteLine("test_data__1__fold_" + (i + 1) + " = [");
                foreach (var k in enumerate_K()) // 1 Fold Cross Validation
                    yield return new IterationValue(k, 1);
            }

            for (int i = 0; i < 5; i++) // 5 Fold Cross Validation
            {
                this.WriteLine("];");
                this.WriteLine("");
                this.WriteLine("test_data__20p__fold_" + (i + 1) + " = [");
                foreach (var k in enumerate_K())
                    yield return new IterationValue(k, (int)Math.Round(data_length * 0.2f));
            }

            for (int i = 0; i < 5; i++) // 5 Fold Cross Validation
            {
                this.WriteLine("];");
                this.WriteLine("");
                this.WriteLine("test_data__50p__fold_" + (i + 1) + " = [");
                if (i > 2)
                foreach (var k in enumerate_K())
                    yield return new IterationValue(k, (int)Math.Round(data_length * 0.5f));
            }

            for (int i = 0; i < 5; i++) // 5 Fold Cross Validation
            {
                this.WriteLine("];");
                this.WriteLine("");
                this.WriteLine("test_data__80p__fold_" + (i + 1) + " = [");
                foreach (var k in enumerate_K())
                    yield return new IterationValue(k, (int)Math.Round(data_length * 0.8f));
            }

            this.WriteLine("];");
            this.WriteLine("");
            this.WriteLine("global_test = [");
            foreach (var k in enumerate_K())
                yield return new IterationValue(k);
            this.WriteLine("];");

        }

        private Useable _GlobalTrain;
        private Useable _GlobalTest;

        private Useable _CurrentTest;
        private Useable _CurrentTrain;

        private IEnumerator<IterationValue> _Enumerator = null;

        private void transformer1_DataPop(Useable[] data)
        {
            this._GlobalTrain = data[0];
            this._GlobalTest = data[1];

            this._Enumerator = this.enumerate_folds(this._GlobalTrain._CountRows).GetEnumerator();
            this.enumerate_pop();
        }

        private void dataTrainer1_DataPop(Trainer.TrainerReturn data)
        {
            this.enumerate_pop();
        }

        private void enumerate_pop()
        {
            if (this._Enumerator.MoveNext())
            {
                var valid = this._Enumerator.Current;
                this.dataTrainer1._K = valid._K;

                if (valid._UseGlobal)
                {
                    this.dataTrainer1.SetData(this._GlobalTrain, this._GlobalTest);
                }
                else
                {

                    this._GlobalTrain.Split(
                        valid._NumberTest,
                        out this._CurrentTest,
                        out this._CurrentTrain);

                    this.dataTrainer1.SetData(this._CurrentTrain, this._CurrentTest);
                }
            }

        }     
    }
}

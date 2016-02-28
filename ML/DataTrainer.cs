using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ML.Classifiers;

namespace ML
{
    public partial class DataTrainer : UserControl
    {
        private bool _Loaded = false;
        private DataUseable _Train;
        private DataUseable _Test;
        private DateTime _DateLoadStart;
        private ClassifierType _LastClassifierType = ClassifierType.DecisionTree;

        public DataTrainer()
        {
            InitializeComponent();

            this.rbDecisionTree.Tag = Convert.ToInt32(ClassifierType.DecisionTree);
            this.rbRandomForest.Tag = Convert.ToInt32(ClassifierType.RandomForest);

            foreach (var rb in new RadioButton[]
            {
                this.rbDecisionTree,
                this.rbRandomForest,
            }) if (Properties.Settings.Default.ClassifierType == (int)rb.Tag) rb.Checked = true;

            this.nudDecisionTreeDepth.Value = Properties.Settings.Default.TrainTreeDepth;
            this.nudForestTrees.Value = Properties.Settings.Default.TrainForestCount;
        }

        public enum ClassifierType
        {
            DecisionTree = 0,
            RandomForest = 1,
        }

        internal void SetData(DataUseable train, DataUseable test)
        {
            this._Train = train;
            this._Test = test;
            this._Loaded = true;
            this.LoadData();
        }

        private void NudChanged(object sender, EventArgs e)
        {
            if (!this._Loaded) return;

            Properties.Settings.Default.TrainTreeDepth = (int)Math.Round(this.nudDecisionTreeDepth.Value);
            Properties.Settings.Default.TrainForestCount = (int)Math.Round(this.nudForestTrees.Value);
            Properties.Settings.Default.Save();

            this.LoadData();
        }

        private void rbDecisionTree_CheckedChanged(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb != null)
            {
                if (rb.Checked)
                {
                    this._LastClassifierType = (ClassifierType)rb.Tag;
                    this.LoadData();


                    this.nudDecisionTreeDepth.Enabled = false;
                    this.nudForestTrees.Enabled = false;

                    switch (this._LastClassifierType)
                    {
                        case ClassifierType.DecisionTree:
                            this.nudDecisionTreeDepth.Enabled = true;
                            break;
                        case ClassifierType.RandomForest:
                            this.nudDecisionTreeDepth.Enabled = true;
                            this.nudForestTrees.Enabled = true;
                            break;
                    }

                    Properties.Settings.Default.ClassifierType = (int)rb.Tag;
                    Properties.Settings.Default.Save();
                }
            }

        }

        private void LoadData()
        {
            if (!this._Loaded) return;
            if (this.bwLoadData.IsBusy)
            {
                this.labelDataStatus.Text = "Canceling last training...";
                this.bwLoadData.CancelAsync();
            }
            else
            {
                this.labelDataStatus.ForeColor = Color.OrangeRed;
                this.labelDataStatus.Text = "Training...";
                this._DateLoadStart = DateTime.Now;

                switch (this._LastClassifierType)
                {
                    case ClassifierType.DecisionTree:
                        this.bwLoadData.RunWorkerAsync(new ToBackgroundWorkerArgsTree(
                            this._Train,
                            this._Test,
                            (int)Math.Round(this.nudDecisionTreeDepth.Value)));
                        break;
                    case ClassifierType.RandomForest:
                        this.bwLoadData.RunWorkerAsync(new ToBackgroundWorkerArgsForest(
                            this._Train,
                            this._Test,
                            (int)Math.Round(this.nudDecisionTreeDepth.Value),
                            (int)Math.Round(this.nudForestTrees.Value)));
                        break;
                }
            }
        }

        private class ToBackgroundWorkerArgsTree
        {
            public DataUseable _Test;
            public DataUseable _Train;
            public int _MaxDepth;

            public ToBackgroundWorkerArgsTree(
                DataUseable train,
                DataUseable test, 
                int max_depth)
            {
                this._Train = train;
                this._Test = test;
                this._MaxDepth = max_depth == 0 ? -1 : max_depth;
            }
        }
        private class ToBackgroundWorkerArgsForest
        {
            public DataUseable _Test;
            public DataUseable _Train;
            public int _MaxDepth;
            public int _TreeCount;

            public ToBackgroundWorkerArgsForest(
                DataUseable train,
                DataUseable test,
                int max_depth, 
                int tree_count)
            {
                this._Train = train;
                this._Test = test;
                this._MaxDepth = max_depth == 0 ? -1 : max_depth;
                this._TreeCount = tree_count;
            }
        }


        private void bwLoadData_DoWork(object sender, DoWorkEventArgs e)
        {
            Classifier classif = null;

            DataUseable train = null;
            DataUseable test = null;
            string front = "";

            if (e.Argument is ToBackgroundWorkerArgsTree)
            {
                var args = e.Argument as ToBackgroundWorkerArgsTree;
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff ") + "Creating Tree");
                classif = new DecisionTree(args._Train, args._MaxDepth);
                train = args._Train;
                test = args._Test;
            }
            else if (e.Argument is ToBackgroundWorkerArgsForest)
            {
                var args = e.Argument as ToBackgroundWorkerArgsForest;
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff ") + "Creating Forest");
                classif = new RandomForest(args._Train, args._MaxDepth, args._TreeCount);
                train = args._Train;
                test = args._Test;
                front = args._MaxDepth.ToString() + ",";
            }

            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff ") + "Compiling");
            var func = classif.Compile();
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff ") + "Evaluating Data");
            var conf_train = new ConfusionMatrix(func, train);
            var conf_test = new ConfusionMatrix(func, test);
            Form1.Instance.WriteLine("             " + front + " " + conf_train.Accuracy + ", " + conf_test.Accuracy + ";");
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff ") + "Done");
            Console.WriteLine();
            Console.WriteLine();


            if (this.bwLoadData.CancellationPending) e.Result = null;
            else e.Result = new DataUseable[0];
        }

        private void bwLoadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.labelDataStatus.ForeColor = Color.Green;
            this.labelDataStatus.Text = "Data trained in " + (DateTime.Now - this._DateLoadStart).TotalSeconds.ToString("0.00") + " seconds!";
            if (e.Result is DataUseable[])
            {
            }
            else
            {
                // Params Changed
                this.LoadData();
            }
        }
    }
}


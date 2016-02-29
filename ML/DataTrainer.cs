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

        private int _RepetitionsLeft = 0;
        float _RepetitionsCumulativeAccuracyTest = 0;
        float _RepetitionsCumulativeAccuracyTrain = 0;

        public DataTrainer()
        {
            InitializeComponent();

            this.rbDecisionTree.Tag = Convert.ToInt32(ClassifierType.DecisionTree);
            this.rbRandomForest.Tag = Convert.ToInt32(ClassifierType.RandomForest);
            this.rbAdaBoostStump.Tag = Convert.ToInt32(ClassifierType.AdaBoostStump);
            this.rbAdaBoostTree.Tag = Convert.ToInt32(ClassifierType.AdaBoostTree);

            foreach (var rb in new RadioButton[]
            {
                this.rbDecisionTree,
                this.rbRandomForest,
                this.rbAdaBoostStump,
                this.rbAdaBoostTree
            }) if (Properties.Settings.Default.ClassifierType == (int)rb.Tag) rb.Checked = true;

            this.nudDecisionTreeDepth.Value = Properties.Settings.Default.TrainTreeDepth;
            this.nudForestTrees.Value = Properties.Settings.Default.TrainForestCount;
            this.nudBoosts.Value = Properties.Settings.Default.TrainBoosts;
        }

        public enum ClassifierType
        {
            DecisionTree = 0,
            RandomForest = 1,
            AdaBoostStump = 2,
            AdaBoostTree = 3,
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

            Properties.Settings.Default.TrainTreeDepth = this._TreeDepth;
            Properties.Settings.Default.TrainForestCount = this._TreeCount;
            Properties.Settings.Default.TrainBoosts = this._BoostCount;
            Properties.Settings.Default.Save();

            this.LoadData();
        }

        private int _RepetitionsNeeded
        {
            get
            {
                if (!this.nudRepetitions.Enabled) return 1;
                return (int)Math.Round(this.nudRepetitions.Value);
            }
        }

        private int _TreeDepth
        {
            get
            {
                return (int)Math.Round(this.nudDecisionTreeDepth.Value);
            }
        }

        private int _TreeCount
        {
            get
            {
                return (int)Math.Round(this.nudForestTrees.Value);
            }
        }

        private int _BoostCount
        {
            get
            {
                return (int)Math.Round(this.nudBoosts.Value);
            }
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
                    this.nudRepetitions.Enabled = false;
                    this.nudBoosts.Enabled = false;

                    switch (this._LastClassifierType)
                    {
                        case ClassifierType.DecisionTree:
                            this.nudDecisionTreeDepth.Enabled = true;
                            break;
                        case ClassifierType.RandomForest:
                            this.nudDecisionTreeDepth.Enabled = true;
                            this.nudForestTrees.Enabled = true;
                            this.nudRepetitions.Enabled = true;
                            break;
                        case ClassifierType.AdaBoostStump:
                            this.nudBoosts.Enabled = true;
                            break;
                        case ClassifierType.AdaBoostTree:
                            this.nudBoosts.Enabled = true;
                            this.nudDecisionTreeDepth.Enabled = true;
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

            this._RepetitionsLeft = this._RepetitionsNeeded;
            this._RepetitionsCumulativeAccuracyTest = 0;
            this._RepetitionsCumulativeAccuracyTrain = 0;

            this.DoBackgroundWork();
        }

        private void DoBackgroundWork()
        {
            if (this.bwLoadData.IsBusy)
            {
                this.labelDataStatus.Text = "Canceling last training...";
                this.bwLoadData.CancelAsync();
            }
            else
            {
                try
                {
                    switch (this._LastClassifierType)
                    {
                        case ClassifierType.DecisionTree:
                            this.bwLoadData.RunWorkerAsync(new ToBackgroundWorkerArgsTree(
                                this._Train,
                                this._Test,
                                this._TreeDepth));
                            break;
                        case ClassifierType.RandomForest:
                            this.bwLoadData.RunWorkerAsync(new ToBackgroundWorkerArgsForest(
                                this._Train,
                                this._Test,
                                this._TreeDepth,
                                this._TreeCount));
                            break;
                        case ClassifierType.AdaBoostStump:
                            this.bwLoadData.RunWorkerAsync(new ToBackgroundWorkerArgsAdaBoost(
                                this._Train,
                                this._Test,
                                () => { return new Classifiers.AdaBoostClassifiers.DecisionStump(); },
                                this._BoostCount
                                ));
                            break;
                        case ClassifierType.AdaBoostTree:
                            this.bwLoadData.RunWorkerAsync(new ToBackgroundWorkerArgsAdaBoost(
                                this._Train,
                                this._Test,
                                () => { return new Classifiers.AdaBoostClassifiers.DecisionTree(this._TreeDepth); },
                                this._BoostCount
                                ));
                            break;
                    }

                    this.labelDataStatus.ForeColor = Color.OrangeRed;
                    this.labelDataStatus.Text = "Training...";
                    this._DateLoadStart = DateTime.Now;
                }
                catch (InvalidOperationException) // Thread Busy
                {
                    this.labelDataStatus.Text = "Canceling last training...";
                    this.bwLoadData.CancelAsync();
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
                this._MaxDepth = max_depth;
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
                this._MaxDepth = max_depth;
                this._TreeCount = tree_count;
            }
        }

        private class ToBackgroundWorkerArgsAdaBoost
        {
            public Func<Classifiers.AdaBoostClassifiers.AdaBoostClassifier> _Factory;
            public DataUseable _Test;
            public DataUseable _Train;
            public int _Boosts;

            public ToBackgroundWorkerArgsAdaBoost(
                DataUseable train,
                DataUseable test,
                Func<Classifiers.AdaBoostClassifiers.AdaBoostClassifier> f,
                int boosts)
            {
                this._Train = train;
                this._Test = test;
                this._Factory = f;
                this._Boosts = boosts;
            }
        }


        private void bwLoadData_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Classifier classif = null;
                DataUseable train = null;
                DataUseable test = null;

                if (e.Argument is ToBackgroundWorkerArgsTree)
                {
                    var args = e.Argument as ToBackgroundWorkerArgsTree;
                    train = args._Train;
                    test = args._Test;
                    classif = new DecisionTree(train, args._MaxDepth);
                }
                else if (e.Argument is ToBackgroundWorkerArgsForest)
                {
                    var args = e.Argument as ToBackgroundWorkerArgsForest;
                    train = args._Train;
                    test = args._Test;
                    classif = new RandomForest(train, args._MaxDepth, args._TreeCount);
                }
                else if (e.Argument is ToBackgroundWorkerArgsAdaBoost)
                {
                    var args = e.Argument as ToBackgroundWorkerArgsAdaBoost;
                    train = args._Train;
                    test = args._Test;
                    classif = new AdaBoost(train, test, args._Boosts, args._Factory);
                }

                var func = classif.Compile();
                var conf_train = new ConfusionMatrix(func, train);
                var conf_test = new ConfusionMatrix(func, test);

                if (this.bwLoadData.CancellationPending) e.Result = null;
                else e.Result = new ConfusionMatrix[] { conf_train, conf_test };
            }
            catch (Exception exc)
            {
                e.Result = exc.ToString();
            }
        }

        private void bwLoadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is ConfusionMatrix[])
            {
                var cfs = e.Result as ConfusionMatrix[];
                this._RepetitionsCumulativeAccuracyTrain += cfs[0].Accuracy;
                this._RepetitionsCumulativeAccuracyTest += cfs[1].Accuracy;

                if (--this._RepetitionsLeft == 0)
                {
                    this._RepetitionsCumulativeAccuracyTrain /= this._RepetitionsNeeded;
                    this._RepetitionsCumulativeAccuracyTest /= this._RepetitionsNeeded;
                    this.labelTrain.Text = "Training Accuracy: " + this._RepetitionsCumulativeAccuracyTrain.ToString("0.00%");
                    this.labelTest.Text = "Testing Accuracy: " + this._RepetitionsCumulativeAccuracyTest.ToString("0.00%");

                    String s = "";
                    switch (this._LastClassifierType)
                    {
                        case ClassifierType.DecisionTree:
                            s = "% Decision Tree: ";
                            if (this._TreeDepth == -1) s += "no max depth";
                            else s += this._TreeDepth + " max depth";
                            break;
                        case ClassifierType.AdaBoostStump:
                            s = "% Ada Boost Decision Stump: ";
                            s += this._BoostCount + " boosts";
                            break;
                        case ClassifierType.AdaBoostTree:
                            s = "% Ada Boost Decision Tree: ";
                            s += this._BoostCount + " boosts, ";
                            if (this._TreeDepth == -1) s += "no max depth";
                            else s += this._TreeDepth + " max depth";
                            break;
                        case ClassifierType.RandomForest:
                            s = "% Random Forest: ";
                            if (this._TreeDepth == -1) s += "no max depth, ";
                            else s += this._TreeDepth + " max depth, ";
                            s += this._TreeCount + " trees";

                            if (this._RepetitionsNeeded != 1)
                                s += ", average accuracy of " + this._RepetitionsNeeded + " forests";
                            break;
                    }

                    Form1.Instance.WriteLine(s);
                    Form1.Instance.WriteLine("\t" + this._RepetitionsCumulativeAccuracyTrain + ", " + this._RepetitionsCumulativeAccuracyTest + ";");

                    this.labelDataStatus.ForeColor = Color.Green;

                    if (this._RepetitionsNeeded == 1)
                    {
                        this.labelDataStatus.Text = "Trained in " + (DateTime.Now - this._DateLoadStart).TotalSeconds.ToString("0.00") + " seconds!";

                        Form1.Instance.WriteLine("");
                        Form1.Instance.WriteLine("Train Confusion Matrix:");
                        Form1.Instance.WriteLine(cfs[0].ToString());
                        Form1.Instance.WriteLine("");
                        Form1.Instance.WriteLine("Test Confusion Matrix:");
                        Form1.Instance.WriteLine(cfs[1].ToString());
                    }
                    else
                    {
                        this.labelDataStatus.Text = "Trained " + this._RepetitionsNeeded + " times in " + (DateTime.Now - this._DateLoadStart).TotalSeconds.ToString("0.00") + " seconds!";
                    }
                }
                else this.DoBackgroundWork();
            }
            else if (e.Result is String)
            {
                this.labelDataStatus.Text = e.Result as String;
                Console.WriteLine(e.Result as String);
            }
            else
            {
                // Params Changed
                this.LoadData();
            }
        }
    }

    
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SamSeifert.ML.Classifiers;
using SamSeifert.Utilities;

namespace SamSeifert.ML.Controls
{
    public partial class Trainer : UserControl
    {
        private bool _Loaded = false;
        private Data.Useable _Train;
        private Data.Useable _Test;
        private DateTime _DateLoadStart;
        private ClassifierType _LastClassifierType = ClassifierType.DecisionTree;

        private int _RepetitionsLeft = 0;
        float _RepetitionsCumulativeAccuracyTest = 0;
        float _RepetitionsCumulativeAccuracyTrain = 0;

        public Trainer()
        {
            InitializeComponent();

            this.rbDecisionTree.Tag = Convert.ToInt32(ClassifierType.DecisionTree);
            this.rbRandomForest.Tag = Convert.ToInt32(ClassifierType.RandomForest);
            this.rbAdaBoostStump.Tag = Convert.ToInt32(ClassifierType.AdaBoostStump);
            this.rbAdaBoostTree.Tag = Convert.ToInt32(ClassifierType.AdaBoostTree);
            this.rbkNN.Tag = Convert.ToInt32(ClassifierType.kNN);

            foreach (var rb in new RadioButton[]
            {
                this.rbDecisionTree,
                this.rbRandomForest,
                this.rbAdaBoostStump,
                this.rbAdaBoostTree,
                this.rbkNN
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
            kNN = 4,
        }

        public void SetData(Data.Useable train, Data.Useable test)
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

        private int _K
        {
            get
            {
                return (int)Math.Round(this.nudK.Value);
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


        private void rbClassifier_CheckChanged(object sender, EventArgs e)
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
                    this.nudK.Enabled = false;

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
                        case ClassifierType.kNN:
                            this.nudK.Enabled = true;
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
                                () => { return new Classifiers.BoostableClassifiers.DecisionStump(); },
                                this._BoostCount
                                ));
                            break;
                        case ClassifierType.AdaBoostTree:
                            this.bwLoadData.RunWorkerAsync(new ToBackgroundWorkerArgsAdaBoost(
                                this._Train,
                                this._Test,
                                () => { return new Classifiers.BoostableClassifiers.DecisionTree(this._TreeDepth); },
                                this._BoostCount
                                ));
                            break;
                        case ClassifierType.kNN:
                            this.bwLoadData.RunWorkerAsync(new ToBackgroundWorkerkNN(
                                this._Train,
                                this._Test,
                                this._K));
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
            public Data.Useable _Test;
            public Data.Useable _Train;
            public int _MaxDepth;

            public ToBackgroundWorkerArgsTree(
                Data.Useable train,
                Data.Useable test, 
                int max_depth)
            {
                this._Train = train;
                this._Test = test;
                this._MaxDepth = max_depth;
            }
        }

        private class ToBackgroundWorkerArgsForest
        {
            public Data.Useable _Test;
            public Data.Useable _Train;
            public int _MaxDepth;
            public int _TreeCount;

            public ToBackgroundWorkerArgsForest(
                Data.Useable train,
                Data.Useable test,
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
            public Func<Classifiers.BoostableClassifiers.BoostableClassifier> _Factory;
            public Data.Useable _Test;
            public Data.Useable _Train;
            public int _Boosts;

            public ToBackgroundWorkerArgsAdaBoost(
                Data.Useable train,
                Data.Useable test,
                Func<Classifiers.BoostableClassifiers.BoostableClassifier> f,
                int boosts)
            {
                this._Train = train;
                this._Test = test;
                this._Factory = f;
                this._Boosts = boosts;
            }
        }

        private class ToBackgroundWorkerkNN
        {
            public Data.Useable _Test;
            public Data.Useable _Train;
            public int _kNN;

            public ToBackgroundWorkerkNN(
                Data.Useable train,
                Data.Useable test,
                int kNN)
            {
                this._Train = train;
                this._Test = test;
                this._kNN = kNN;
            }
        }

        private void bwLoadData_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Classifier classif = null;
                Data.Useable train = null;
                Data.Useable test = null;

                if (e.Argument is ToBackgroundWorkerArgsTree)
                {
                    var args = e.Argument as ToBackgroundWorkerArgsTree;
                    train = args._Train;
                    test = args._Test;
                    classif = new DecisionTree(args._MaxDepth);
                }
                else if (e.Argument is ToBackgroundWorkerArgsForest)
                {
                    var args = e.Argument as ToBackgroundWorkerArgsForest;
                    train = args._Train;
                    test = args._Test;
                    classif = new RandomForest(args._MaxDepth, args._TreeCount);
                }
                else if (e.Argument is ToBackgroundWorkerArgsAdaBoost)
                {
                    var args = e.Argument as ToBackgroundWorkerArgsAdaBoost;
                    train = args._Train;
                    test = args._Test;
                    classif = new AdaBoost(args._Factory, args._Boosts);
                }
                else if (e.Argument is ToBackgroundWorkerkNN)
                {
                    var args = e.Argument as ToBackgroundWorkerkNN;
                    train = args._Train;
                    test = args._Test;
                    classif = new kNN(args._kNN);
                }

                classif.Train(train);

                var conf_train = new ConfusionMatrix(classif.Compile, train);
                var conf_test = new ConfusionMatrix(classif.Compile, test);

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
                        case ClassifierType.kNN:
                            s = "% kNN: ";
                            s += this._K;
                            break;
                    }

                    Logger.WriteLine(s);
                    Logger.WriteLine("\t" + this._RepetitionsCumulativeAccuracyTrain + ", " + this._RepetitionsCumulativeAccuracyTest + ";");

                    this.labelDataStatus.ForeColor = Color.Green;

                    if (this._RepetitionsNeeded == 1)
                    {
                        this.labelDataStatus.Text = "Trained in " + (DateTime.Now - this._DateLoadStart).TotalSeconds.ToString("0.00") + " seconds!";

                        if (this.cbConfusionPrint.Checked)
                        {
                            Logger.WriteLine("");
                            Logger.WriteLine("Train Confusion Matrix:");
                            Logger.WriteLine(cfs[0].ToString());
                            Logger.WriteLine("");
                            Logger.WriteLine("Test Confusion Matrix:");
                            Logger.WriteLine(cfs[1].ToString());
                        }
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


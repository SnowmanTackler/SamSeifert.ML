using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ML
{
    public partial class MainForm : Form
    {
        private int LastBrowseClick = 0;
        private bool _Loaded = false;
        private DateTime _DateLoadStart;

        public MainForm()
        {
            InitializeComponent();

            this.button1.Tag = 0;
            this.button2.Tag = 1;

            this.textBox1.Text = Properties.Settings.Default.File1 ?? "";
            this.textBox2.Text = Properties.Settings.Default.File2 ?? "";

            this.radioButton2.Checked = Properties.Settings.Default.OneTwoFiles;

            this.checkBox1.Checked = Properties.Settings.Default.Transpose;
        }

        private void BrowseClicked(object sender, EventArgs e)
        {
            var butt = sender as Button;

            if (butt != null)
            {
                this.LastBrowseClick = (int)butt.Tag;
                this.openFileDialog1.ShowDialog();
            }

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb != null)
            {
                if (rb.Checked)
                {
                    if (rb == this.radioButton1)
                    {
                        this.textBox2.Enabled = false;
                        this.button2.Enabled = false;
                    }
                    else
                    {
                        this.textBox2.Enabled = true;
                        this.button2.Enabled = true;
                    }
                    this.LoadData();

                    Properties.Settings.Default.OneTwoFiles = this.radioButton2.Checked;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            switch (this.LastBrowseClick)
            {
                case 0:
                    this.textBox1.Text = this.openFileDialog1.FileName;
                    break;

                case 1:
                    this.textBox2.Text = this.openFileDialog1.FileName;
                    break;
            }

            Properties.Settings.Default.Save();
        }


        private void TextBoxChanged(object sender, EventArgs e)
        {
            var tb = sender as TextBox;

            if (tb != null)
            {
                if (File.Exists(tb.Text))
                {
                    tb.ForeColor = Color.Green;
                }
                else
                {
                    tb.ForeColor = Color.OrangeRed;
                }

                if (tb == this.textBox1) Properties.Settings.Default.File1 = tb.Text;
                if (tb == this.textBox2) Properties.Settings.Default.File2 = tb.Text;
                Properties.Settings.Default.Save();
            }

            this.LoadData();
        }

        private void TransposeCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Transpose = this.checkBox1.Checked;
            Properties.Settings.Default.Save();
            this.LoadData();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this._Loaded = true;
            this.LoadData();
        }

        private void LoadData()
        {
            if (this._Loaded)
            {
                if ((!this.textBox1.Enabled || File.Exists(this.textBox1.Text)) &&
                    (!this.textBox2.Enabled || File.Exists(this.textBox2.Text)) )
                {
                    if (!this.bwLoadData.IsBusy)
                    {
                        this._DateLoadStart = DateTime.Now;
                        this.labelDataStatus.Text = "Loading Data!";
                        Console.WriteLine("Got Data");

                        var files = new List<String>();
                        if (this.textBox1.Enabled) files.Add(this.textBox1.Text);
                        if (this.textBox2.Enabled) files.Add(this.textBox2.Text);

                        this.bwLoadData.RunWorkerAsync(new ToBackgroundWorkerArgs(
                            files.ToArray(),
                            this.checkBox1.Checked));
                    }
                }
            }

        }

        public class ToBackgroundWorkerArgs
        {
            public bool _Transpose;
            public string[] _FileNames;

            public ToBackgroundWorkerArgs(string[] file_names, bool transpose)
            {
                this._FileNames = file_names;
                this._Transpose = transpose;
            }
        }

        private void bwLoadData_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = e.Argument as ToBackgroundWorkerArgs;

            if (args == null)
            {
                e.Result = "Background Worker Getting Null Input";
            }
            else
            {
                var data = new List<DataImported>();
                foreach (var file_name in args._FileNames)
                {
                    String error;
                    data.Add(new DataImported(file_name, args._Transpose, out error));

                    if (error != null)
                    {
                        e.Result = error;
                        return;
                    }
                }
                e.Result = data.ToArray();
            }
        }

        private void bwLoadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is String)
            {
                this.labelDataStatus.Text = "Error: " + e.Result as String;
            }
            else if (e.Result is DataImported[])
            {
                var dat = e.Result as DataImported[];

                for (int i = 1; i < dat.Length; i++)
                {
                    if (dat[0]._Columns != dat[i]._Columns)
                    {
                        this.labelDataStatus.Text = "Error: Data Size Mismatch " +
                            dat[0]._Columns + " " + dat[i]._Columns;
                        return;
                    }
                }

                this.labelDataStatus.Text = "Loaded in " + (DateTime.Now - this._DateLoadStart).TotalSeconds.ToString("0.00") + " seconds!";

            }
            else
            {
                this.labelDataStatus.Text = "Error: No return from DataLoad"; 
            }
        }

    }
}

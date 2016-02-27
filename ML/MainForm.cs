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
                    Properties.Settings.Default.File1 = this.openFileDialog1.FileName;
                    this.textBox1.Text = this.openFileDialog1.FileName;
                    break;

                case 1:
                    Properties.Settings.Default.File2 = this.openFileDialog1.FileName;
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
            }

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
                        this.labelStatus.Text = "Loading Data!";
                        Console.WriteLine("Got Data");

                        var files = new List<String>();
                        if (this.textBox1.Enabled) files.Add(this.textBox1.Text);
                        if (this.textBox2.Enabled) files.Add(this.textBox2.Text);

                        this.bwLoadData.RunWorkerAsync(files.ToArray());
                    }
                }
            }

        }

        private void bwLoadData_DoWork(object sender, DoWorkEventArgs e)
        {
            var file_names = e.Argument as String[];

            if (file_names == null)
            {
                e.Result = "Background Worker Getting Null Input";
            }
            else
            {
                var data = new List<Data>();
                foreach (var file_name in file_names)
                {
                    String error;
                    data.Add(new Data(file_name, out error));

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
                this.labelStatus.Text = "Error: " + e.Result as String;
            }
            else if (e.Result is Data[])
            {
                var dat = e.Result as Data[];

                for (int i = 1; i < dat.Length; i++)
                {
                    if (dat[0]._Columns != dat[i]._Columns)
                    {
                        this.labelStatus.Text = "Error: Data Size Mismatch " +
                            dat[0]._Columns + " " + dat[i]._Columns;
                        return;
                    }
                }

                this.labelStatus.Text = "Loaded in " + (DateTime.Now - this._DateLoadStart).TotalSeconds.ToString("0.00") + " seconds!";

            }
            else
            {
                this.labelStatus.Text = "Error: No return from DataLoad"; 

            }
        }
    }
}

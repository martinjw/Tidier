using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tidier.FormUtils;
using Tidier.Properties;
using TidyNet;

namespace Tidier
{
    public partial class Form1 : Form
    {
        private readonly FileGridViewHelper _fileHelper = new FileGridViewHelper();
        private readonly MessageGridViewHelper _messageHelper = new MessageGridViewHelper();
        public Form1()
        {
            InitializeComponent();
            _fileHelper.AddFormDragDrop(this);

            //form closing - save settings
            Closing += Form1_Closing;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _fileHelper.Init(dataGridView1);
            _messageHelper.Init(dataMessages);
            var size = Settings.Default.Size;
            if (size.Width > 0 && size.Height > 0)
            {
                Size = size;
            }
        }

        void Form1_Closing(object sender, CancelEventArgs e)
        {
            Settings.Default.Save();
        }

        private void btnDirectoryPicker_Click(object sender, EventArgs e)
        {
            using (var folderBrowserDialog1 = new FolderBrowserDialog())
            {
                folderBrowserDialog1.SelectedPath = txtDirectory.Text;
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    txtDirectory.Text = folderBrowserDialog1.SelectedPath;
                }
            }
        }

        private void btnAddFiles_Click(object sender, EventArgs e)
        {
            Settings.Default.TargetDirectory = _fileHelper.AddFilesWithDialog(Settings.Default.TargetDirectory);
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            var dir = txtDirectory.Text.Trim();
            if (!Directory.Exists(dir)) return;

            Cursor.Current = Cursors.WaitCursor;
            toolStripStatusLabel1.Text = "Working";
            btnScan.Visible = false;
            progressBar1.Visible = true;
            dataMessages.Visible = false;

            var tidy = new Tidy();
            tidy.InitOptions();
            var extension = txtExtension.Text.Trim();
            if (string.IsNullOrEmpty(extension)) extension = ".html";
            tidy.Options.Extension = extension;
            var bw = new BackgroundWorker();
            bw.DoWork += (s, ea) =>
                    {
                        ea.Result = tidy.Scan(dir);
                    };
            bw.RunWorkerCompleted += (s, ea) =>
            {
                progressBar1.Visible = false;
                if (ea.Error == null)
                {
                    var results = (TidyMessageCollection)ea.Result;
                    var count = results.Count;
                    if (count > 0)
                    {
                        //for a large result set this will take a second or two
                        _messageHelper.AddMessages(results.Where(x => x.Level != MessageLevel.Info));
                        dataMessages.Visible = true;
                    }

                    toolStripStatusLabel1.Text = count + " messages. Double click row to open each file.";

                }
                else
                {
                    toolStripStatusLabel1.Text = "Error trying to scan files. " + ea.Error.Message;
                }
                btnScan.Visible = true;
                Cursor.Current = Cursors.Default;
            };
            bw.RunWorkerAsync();

        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            var files = _fileHelper.Files();
            Cursor.Current = Cursors.WaitCursor;
            toolStripStatusLabel1.Text = "";
            var backup = chkBackup.Checked;
            var extension = txtExtension.Text.Trim().ToUpperInvariant();
            var tidy = new Tidy();
            tidy.InitOptions();
            tidy.Options.Extension = extension;
            var messageCollection = new TidyMessageCollection();

            var bw = new BackgroundWorker();
            bw.DoWork += (s, ea) =>
            {
                int i = 0;
                foreach (var fileInfo in files)
                {
                    if (!fileInfo.Exists)
                        continue;
                    var ext = fileInfo.Extension.ToUpperInvariant();
                    if (ext != extension)
                        continue;
                    var fullName = fileInfo.FullName;
                    var orig = File.ReadAllText(fullName);
                    var html = tidy.Load(fullName, messageCollection);
                    if (!string.IsNullOrEmpty(html) && orig != html)
                    {
                        if (backup)
                        {
                            var backUp = Path.Combine(fileInfo.DirectoryName,
                                Path.GetFileNameWithoutExtension(fileInfo.Name) + ".orig.html");
                            fileInfo.MoveTo(backUp);
                        }
                        File.WriteAllText(fullName, html);
                        i++;
                    }
                }
                ea.Result = i;
            };
            bw.RunWorkerCompleted += (s, ea) =>
            {
                var i = (int)ea.Result;
                toolStripStatusLabel1.Text += (i + " html changed. ");
                _fileHelper.Clear();
            };
            bw.RunWorkerAsync();

            var results = messageCollection.Where(x => x.Level != MessageLevel.Info).ToList();
            if (results.Count > 0)
            {
                _messageHelper.AddMessages(results);
                dataMessages.Visible = true;
                toolStripStatusLabel1.Text += (results.Count + " warnings/errors. ");
            }


            Cursor.Current = Cursors.Default;
        }

        private void txtDirectory_Validating(object sender, CancelEventArgs e)
        {
            var error = string.Empty;
            var dir = txtDirectory.Text.Trim();
            if (string.IsNullOrEmpty(dir))
            {
                e.Cancel = true;
                error = "Cannot be empty";
            }
            else if (!Directory.Exists(dir))
            {
                e.Cancel = true;
                error = "Directory does not exist";
            }
            errorProvider1.SetError(txtDirectory, error);
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            var results = ((BindingSource)dataMessages.DataSource).List as IList<TidyMessage>;
            if (results == null) return;
            if (!results.Any())
            {
                toolStripStatusLabel1.Text = "No messages to save";
                return;
            }

            using (var dialog = new SaveFileDialog())
            {
                dialog.InitialDirectory = Settings.Default.TargetDirectory;
                dialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                dialog.FilterIndex = 1;
                dialog.DefaultExt = ".txt";
                dialog.RestoreDirectory = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var fileName = dialog.FileName;
                    var sb = new StringBuilder();
                    var count = 0;
                    foreach (var result in results.Where(x => x.Level != MessageLevel.Info))
                    {
                        count++;
                        var format = string.Format("{0,60} \t{1,4}:{2,4} \t{3}", result.Filename, result.Line, result.Column, result.Message);
                        sb.AppendLine(format);
                    }
                    File.WriteAllText(fileName, sb.ToString());
                    toolStripStatusLabel1.Text = fileName + " saved";
                }
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripSplitButton1_ButtonClick(sender, e);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            Settings.Default.Size = Size;
        }
    }
}

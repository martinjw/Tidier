using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Tidier.Data;

namespace Tidier.FormUtils
{
    /// <summary>
    /// Helper to turn an unbound DataGridView into a simple file list
    /// </summary>
    public class FileGridViewHelper
    {
        private readonly SortableBindingList<FileInfo> FileList = new SortableBindingList<FileInfo>();
        private DataGridView _myGrid;

        /// <summary>
        /// Initializes the DataGridView. Call this in Form1_Load.
        /// </summary>
        /// <param name="gv">The DataGridView.</param>
        public void Init(DataGridView gv)
        {
            _myGrid = gv;

            gv.AllowUserToOrderColumns = true;
            //select a entire row, or a range or rows
            gv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gv.MultiSelect = true;
            gv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            gv.AutoGenerateColumns = false;
            var nameCol = new DataGridViewTextBoxColumn();
            nameCol.Name = "Name";
            nameCol.DataPropertyName = "FullName";
            nameCol.ReadOnly = true;
            nameCol.Width = 200;
            gv.Columns.Add(nameCol);

            var sizeCol = new DataGridViewTextBoxColumn();
            sizeCol.Name = "Size";
            sizeCol.DataPropertyName = "Length";
            sizeCol.ReadOnly = true;
            sizeCol.Width = 50;
            sizeCol.ValueType = typeof(long);
            sizeCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //formating doesn't work - you have to add CellFormatting event
            sizeCol.DefaultCellStyle.Format = "{0:KB1}";
            sizeCol.DefaultCellStyle.FormatProvider = new FileSizeFormatProvider();
            gv.Columns.Add(sizeCol);

            var extCol = new DataGridViewTextBoxColumn();
            extCol.Name = "Extension";
            extCol.DataPropertyName = "Extension";
            extCol.ReadOnly = true;
            extCol.Width = 50;
            gv.Columns.Add(extCol);

            var modCol = new DataGridViewTextBoxColumn();
            modCol.Name = "Last Changed";
            modCol.DataPropertyName = "LastWriteTime";
            modCol.ReadOnly = true;
            modCol.ValueType = typeof(DateTime);
            gv.Columns.Add(modCol);

            var bs = new BindingSource();
            bs.DataSource = FileList;
            gv.DataSource = bs;

            //some events we want to watch
            gv.CellFormatting += gv_CellFormatting; //for custom formatting of file size
            gv.DataBindingComplete += gv_DataBindingComplete; //resize columns
            //gv.DataError += gv_DataError; //for error handling (not used)
            gv.MouseUp += gv_MouseUp; //right click context menu to delete rows
        }

        #region private DataGridView events
        //void gv_DataError(object sender, DataGridViewDataErrorEventArgs e)
        //{
        //    DataGridView gv = (DataGridView)sender;
        //    //handle any errors
        //}

        void gv_MouseUp(object sender, MouseEventArgs e)
        {
            var gv = (DataGridView)sender;
            if (e.Button == MouseButtons.Right)
            {
                var hitTestInfo = gv.HitTest(e.X, e.Y);
                if (hitTestInfo.Type == DataGridViewHitTestType.Cell)
                {
                    var row = gv.Rows[hitTestInfo.RowIndex];
                    if (!row.Selected)
                    {
                        gv.ClearSelection();
                        gv.Rows[hitTestInfo.RowIndex].Selected = true;
                    }
                    var cm = new ContextMenuStrip();
                    cm.Items.Add("Delete", null, ContextMenu_Delete);
                    cm.Show(gv, new System.Drawing.Point(e.X, e.Y));
                }
            }
        }

        void ContextMenu_Delete(object sender, EventArgs e)
        {
            foreach (DataGridViewRow selectedRow in _myGrid.SelectedRows)
            {
                var fi = selectedRow.DataBoundItem as FileInfo;
                if (fi != null) FileList.Remove(fi);
            }
            _myGrid.ClearSelection();
        }

        static void gv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //because custom formatters aren't honoured
            var gv = (DataGridView)sender;
            var col = gv.Columns[e.ColumnIndex];
            if (col.DefaultCellStyle.FormatProvider is ICustomFormatter)
                e.Value = string.Format(col.DefaultCellStyle.FormatProvider, col.DefaultCellStyle.Format, e.Value);
        }

        static void gv_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            var gv = (DataGridView)sender;
            gv.AutoResizeColumns();
        }
        #endregion

        #region Add Files
        /// <summary>
        /// Adds a single file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public bool AddFile(string filePath)
        {
            if (!File.Exists(filePath)) return false;
            var fi = new FileInfo(filePath);
            if (Find(fi.FullName) != null) return false;
            FileList.Add(fi);
            return true;
        }

        /// <summary>
        /// Adds a number of files.
        /// </summary>
        /// <param name="filePaths">The file paths.</param>
        public bool AddFiles(string[] filePaths)
        {
            Array.Sort(filePaths);
            foreach (var filePath in filePaths)
            {
                if (!AddFile(filePath)) return false;
            }
            return true;
        }

        /// <summary>
        /// Add files with an OpenFileDialog.
        /// </summary>
        /// <param name="directory">The directory (.</param>
        /// <returns></returns>
        /// <example>Use with Application Settings: <code>
        /// Settings.Default.LastDirectory = fileHelper.AddFilesWithDialog(Settings.Default.LastDirectory);
        /// </code></example>
        public string AddFilesWithDialog(string directory)
        {
            using (var fd = new OpenFileDialog())
            {
                fd.CheckFileExists = true;
                fd.CheckPathExists = true;
                fd.Multiselect = true;
                fd.InitialDirectory = string.IsNullOrEmpty(directory) ?
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) :
                    directory;

                var dr = fd.ShowDialog();

                if (dr != DialogResult.OK) return directory;
                if (string.IsNullOrEmpty(fd.FileName)) return directory;

                Cursor.Current = Cursors.WaitCursor;
                AddFiles(fd.FileNames);
                Cursor.Current = Cursors.Default;

                return Path.GetDirectoryName(fd.FileName);
            }
        }
        #endregion

        #region Public access to the file list
        /// <summary>
        /// Finds the specified file path within the list. NB: Contains won't work.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public FileInfo Find(string filePath)
        {
            foreach (var item in FileList)
            {
                if (item.FullName.Equals(filePath, StringComparison.OrdinalIgnoreCase))
                    return item;
            }
            return null;
        }

        /// <summary>
        /// The first file in the list, or null if none. To show previews.
        /// </summary>
        /// <value>The first file.</value>
        public string FirstFile
        {
            get
            {
                if (FileList.Count == 0) return null;
                return FileList[0].FullName;
            }
        }

        /// <summary>
        /// Returns an enumerator of the filelist - so you can foreach through them all
        /// </summary>
        public IEnumerable<FileInfo> Files()
        {
            return FileList;
        }

        /// <summary>
        /// Clears this file list. Do this after you change the files or our list will be invalid.
        /// </summary>
        public void Clear()
        {
            FileList.Clear();
        }
        #endregion

        /// <summary>
        /// Ensures the form supports file drag-drop, putting files into our file list. Call this in Form1_Load.
        /// </summary>
        public void AddFormDragDrop(Form fm)
        {
            //dragdrop props
            fm.AllowDrop = true;
            fm.DragEnter += delegate (object sender, DragEventArgs e)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                    e.Effect = DragDropEffects.All;
            };
            fm.DragDrop += delegate (object sender, DragEventArgs e)
            {
                var fileList = e.Data.GetData(DataFormats.FileDrop) as string[];
                AddFiles(fileList);
            };
        }
    }
}

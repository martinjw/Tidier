using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Tidier.Data;
using TidyNet;

namespace Tidier
{
    class MessageGridViewHelper
    {
        private readonly SortableBindingList<TidyMessage> _messageList = new SortableBindingList<TidyMessage>();

        /// <summary>
        /// Initializes the DataGridView. Call this in Form1_Load.
        /// </summary>
        /// <param name="gv">The DataGridView.</param>
        public void Init(DataGridView gv)
        {
            gv.AllowUserToOrderColumns = true;
            //select a entire row, or a range or rows
            gv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gv.MultiSelect = true;
            gv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            gv.AutoGenerateColumns = false;
            var nameCol = new DataGridViewTextBoxColumn();
            nameCol.Name = "Filename";
            nameCol.DataPropertyName = "Filename";
            nameCol.ReadOnly = true;
            nameCol.Width = 200;
            gv.Columns.Add(nameCol);

            var lineCol = new DataGridViewTextBoxColumn();
            lineCol.Name = "Line";
            lineCol.DataPropertyName = "Line";
            lineCol.ReadOnly = true;
            lineCol.Width = 50;
            lineCol.ValueType = typeof(long);
            lineCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gv.Columns.Add(lineCol);

            var colCol = new DataGridViewTextBoxColumn();
            colCol.Name = "Column";
            colCol.DataPropertyName = "Column";
            colCol.ReadOnly = true;
            colCol.Width = 50;
            colCol.ValueType = typeof(long);
            colCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gv.Columns.Add(colCol);

            var extCol = new DataGridViewTextBoxColumn();
            extCol.Name = "Message";
            extCol.DataPropertyName = "Message";
            extCol.ReadOnly = true;
            extCol.Width = 250;
            gv.Columns.Add(extCol);

            var bs = new BindingSource();
            bs.DataSource = _messageList;
            gv.DataSource = bs;

            //some events we want to watch
            gv.CellFormatting += gv_CellFormatting; //for custom formatting of file size
            gv.DataBindingComplete += gv_DataBindingComplete; //resize columns
            //gv.DataError += gv_DataError; //for error handling (not used)
            gv.MouseUp += gv_MouseUp; //right click context menu to delete rows
            gv.CellDoubleClick += gv_CellDoubleClick;
        }

        void gv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var gv = (DataGridView)sender;
            var msg = gv.Rows[e.RowIndex].DataBoundItem as TidyMessage;
            if (msg != null)
            {
                const string notepad = @"C:\Program Files (x86)\Notepad++\Notepad++.exe";
                if (File.Exists(notepad))
                {
                    Process.Start(notepad, "-n" + msg.Line + " -c" + msg.Column + " " + msg.Filename);
                }
                else
                {
                    Process.Start("notepad", msg.Filename);
                }
            }
        }


        #region private DataGridView events

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
                }
            }
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

        public void AddMessages(IEnumerable<TidyMessage> messages)
        {
            _messageList.Clear();
            foreach (var tidyMessage in messages)
            {
                _messageList.Add(tidyMessage);
            }
        }
    }
}

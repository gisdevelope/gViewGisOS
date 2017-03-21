using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using gView.Framework.Data;
using gView.Framework.system;

namespace gView.Framework.UI.Dialogs
{
    public partial class FormQueryBuilder : Form
    {
        private ITableClass _tc;
        private ITable _table;
        private CancelTracker _cancelTracker = new CancelTracker();
        private Dictionary<string, List<string>> _values = new Dictionary<string, List<string>>();

        public FormQueryBuilder(IFeatureLayer layer) : this((layer!=null) ? layer.FeatureClass : null)
        {
            
        }
        public FormQueryBuilder(ITableClass tc)
        {
            _tc = tc;

            InitializeComponent();
            if (_tc == null) return;

            gView.Framework.Data.QueryFilter filter = new gView.Framework.Data.QueryFilter();
            filter.SubFields = "*";

            using (ICursor cursor = _tc.Search(filter))
            {
                if (cursor is IFeatureCursor)
                {
                    _table = new FeatureTable((IFeatureCursor)cursor, _tc.Fields, _tc);
                }
                else if (cursor is IRowCursor)
                {
                    _table = new RowTable((IRowCursor)cursor, _tc.Fields);
                }
                else
                {
                    return;
                }
                _table.Fill(2000);
            }

            cmbMethod.SelectedIndex = 0;
            foreach (IField field in _tc.Fields)
            {
                if (field.type == FieldType.binary ||
                    field.type == FieldType.Shape) continue;

                lstFields.Items.Add(Field.WhereClauseFieldName(field.name));
            }
        }

        public string whereClause
        {
            get { return txtWhereClause.Text; }
            set { txtWhereClause.Text = value; }
        }

        public CombinationMethod combinationMethod
        {
            get
            {
                switch (cmbMethod.SelectedIndex)
                {
                    case 0: return CombinationMethod.New;
                    case 1: return CombinationMethod.Union;
                    case 2: return CombinationMethod.Difference;
                    case 3: return CombinationMethod.Intersection;
                }
                return CombinationMethod.New;
            }
            set
            {
                switch(value) 
                {
                    case CombinationMethod.New:
                        cmbMethod.SelectedIndex = 0;
                        break;
                    case CombinationMethod.Union:
                        cmbMethod.SelectedIndex = 1;
                        break;
                    case CombinationMethod.Difference:
                        cmbMethod.SelectedIndex = 2;
                        break;
                    case CombinationMethod.Intersection:
                        cmbMethod.SelectedIndex = 3;
                        break;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!(sender is Button)) return;
            string txt = " "+((Button)sender).Text+" ";

            insertText2WhereClause(txt);
        }

        private void insertText2WhereClause(string txt)
        {
            if (txtWhereClause.SelectionStart > 0 && txtWhereClause.SelectionStart < txtWhereClause.Text.Length)
            {
                string txt1 = txtWhereClause.Text.Substring(0, txtWhereClause.SelectionStart);
                string txt2 = txtWhereClause.Text.Substring(txtWhereClause.SelectionStart + txtWhereClause.SelectionLength,
                    txtWhereClause.Text.Length - txtWhereClause.SelectionStart - txtWhereClause.SelectionLength);
                txtWhereClause.Text = txt1 + txt + txt2;
            }
            else
            {
                txtWhereClause.Text += txt;
            }
        }

        private void FillUniqueValues()
        {
            if (_tc == null) return;
            lstUniqueValues.Items.Clear();
            if (lstFields.SelectedItem == null) return;
            
            string colName = lstFields.SelectedItem.ToString();
            colName = colName.Replace("[", String.Empty).Replace("]", String.Empty); // Joined Fields
            IField field = _tc.FindField(colName);
            if (field == null) return;

            List<string> list;
            if (_values.TryGetValue(colName, out list))
            {
                foreach (string val in list)
                {
                    if (field.type == FieldType.String)
                        lstUniqueValues.Items.Add("'" + val + "'");
                    else
                        lstUniqueValues.Items.Add(val);
                }
            }
            else
            {
                foreach (DataRow row in _table.Table.Select("", colName))
                {
                    string val = row[colName].ToString();
                    if (field.type == FieldType.String) val = "'" + val + "'";
                    if (lstUniqueValues.Items.IndexOf(val) != -1) continue;

                    lstUniqueValues.Items.Add(val);
                }
            }
        }

        private void lstFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnCompleteList.Enabled = lstFields.SelectedItem != null;
            lstUniqueValues.Items.Clear();

            if (_tc == null || _table.Table == null || lstFields.SelectedItem == null) return;
            string colName=lstFields.SelectedItem.ToString();
            colName = colName.Replace("[", String.Empty).Replace("]", String.Empty); // Joined Fields
            if (_table.Table.Columns[colName] == null) return;

            FillUniqueValues();
        }

        private void lstFields_DoubleClick(object sender, EventArgs e)
        {
            if (lstFields.SelectedItem == null) return;
            insertText2WhereClause(lstFields.SelectedItem.ToString());
        }

        private void lstUniqueValues_DoubleClick(object sender, EventArgs e)
        {
            if (lstUniqueValues.SelectedItem == null) return;
            insertText2WhereClause(lstUniqueValues.SelectedItem.ToString());
        }

        private void btnCompleteList_Click(object sender, EventArgs e)
        {
            if (_tc == null || lstFields.SelectedItems == null) return;
            string fieldName = lstFields.SelectedItem.ToString().Replace("[", String.Empty).Replace("]", String.Empty); // Joined Fields [...]
            IField field = _tc.FindField(fieldName);

            DistinctFilter filter = new DistinctFilter(field.name);

            lstUniqueValues.Items.Clear();

            this.Cursor = Cursors.WaitCursor;

            ICursor cursor = _tc.Search(filter);
            if(cursor==null) return;

            _cancelTracker.Reset();
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(CompleteList_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompleteList_RunWorkerCompleted);
            bw.RunWorkerAsync(cursor);
        }

        void CompleteList_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FillUniqueValues();
            this.Cursor = Cursors.Default;
        }

        void CompleteList_DoWork(object sender, DoWorkEventArgs e)
        {
            ICursor cursor = e.Argument as ICursor;
            if (cursor == null) return;

            List<string> list = null;
            try
            {
                IRow row;
                while ((row = (cursor is IFeatureCursor) ? ((IFeatureCursor)cursor).NextFeature :
                           (cursor is IRowCursor) ? ((IRowCursor)cursor).NextRow : null) != null)
                {
                    if (list == null)
                    {
                        if (_values.TryGetValue(row.Fields[0].Name, out list))
                        {
                            list.Clear();
                        }
                        else
                        {
                            _values.Add(row.Fields[0].Name, list = new List<string>());
                        }
                    }
                    list.Add(row[0].ToString());
                    if (!_cancelTracker.Continue) break;
                }
            }
            finally
            {
                if (cursor != null) cursor.Dispose();
            }
        }

        private void FormQueryBuilder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                _cancelTracker.Cancel();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

        }
    }
}
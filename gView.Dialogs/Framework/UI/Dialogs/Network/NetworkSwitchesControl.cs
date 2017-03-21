﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using gView.Framework.Data;
using gView.Framework.FDB;
using gView.Framework.Network;
using gView.Framework.UI.Controls.Wizard;

namespace gView.Framework.UI.Dialogs.Network
{
    public partial class NetworkSwitchesControl : UserControl, IWizardPageNotification, IWizardPageNecessity
    {
        private IFeatureDatabase3 _database = null;
        private SelectFeatureclassesControl _selected;

        public NetworkSwitchesControl(IFeatureDataset dataset, SelectFeatureclassesControl selected)
        {
            InitializeComponent();

            if (dataset != null)
                _database = dataset.Database as IFeatureDatabase3;

            if (_database == null)
                throw new ArgumentException();

            _selected = selected;
            if (_selected == null)
                throw new ArgumentException();
        }

        private void FillGrid()
        {
            if (_selected.NodeFeatureclasses == null)
                return;

            #region Delete Rows
            List<DataGridViewRow> delete = new List<DataGridViewRow>();
            foreach (DataGridViewRow r in gridFcs.Rows)
            {
                int fcId = (int)r.Cells[0].Value;

                bool found = false;
                foreach (IFeatureClass fc in _selected.NodeFeatureclasses)
                {
                    if (_database.GetFeatureClassID(fc.Name) == fcId)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    delete.Add(r);
            }
            foreach (DataGridViewRow r in delete)
                gridFcs.Rows.Remove(r);
            #endregion

            foreach (IFeatureClass fc in _selected.NodeFeatureclasses)
            {
                AddGridRow(fc);
            }

        }

        private void AddGridRow(IFeatureClass fc)
        {
            int fcId = _database.GetFeatureClassID(fc.Name);

            foreach (DataGridViewRow r in gridFcs.Rows)
            {
                if (r.Cells[0].Value.Equals(fcId))
                    return;
            }

            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell idCell = new DataGridViewTextBoxCell();
            idCell.Value = fcId;
            row.Cells.Add(idCell);

            DataGridViewCheckBoxCell chkCell = new DataGridViewCheckBoxCell();
            chkCell.Value = true;
            row.Cells.Add(chkCell);

            DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();
            nameCell.Value = fc.Name;
            row.Cells.Add(nameCell);

            DataGridViewComboBoxCell fieldCell = new DataGridViewComboBoxCell();
            fieldCell.Items.Add("<none>");
            foreach (IField field in fc.Fields)
            {
                switch (field.type)
                {
                    case FieldType.integer:
                    case FieldType.smallinteger:
                    case FieldType.biginteger:
                    case FieldType.boolean:
                        fieldCell.Items.Add(field.name);
                        break;
                }
            }
            fieldCell.Value = "<none>";
            row.Cells.Add(fieldCell);

            DataGridViewComboBoxCell typeCell = new DataGridViewComboBoxCell();
            foreach (object nodeType in Enum.GetValues(typeof(NetworkNodeType)))
                typeCell.Items.Add(nodeType.ToString());
            typeCell.Value = gView.Framework.Network.NetworkNodeType.Unknown.ToString();
            row.Cells.Add(typeCell);

            gridFcs.Rows.Add(row);
        }

        public Dictionary<int, string> SwitchNodeFcIds
        {
            get
            {
                Dictionary<int, string> ret = new Dictionary<int, string>();

                foreach (DataGridViewRow row in gridFcs.Rows)
                {
                    if ((bool)row.Cells[1].Value == true)
                    {
                        string fieldname = (string)row.Cells[3].Value;
                        if (fieldname == "<none>")
                            fieldname = String.Empty;

                        ret.Add((int)row.Cells[0].Value, fieldname);
                    }
                }

                return ret.Keys.Count > 0 ? ret : null;
            }
        }

        public Dictionary<int, gView.Framework.Network.NetworkNodeType> NetworkNodeTypeFcIds
        {
            get
            {
                Dictionary<int, gView.Framework.Network.NetworkNodeType> ret = new Dictionary<int, gView.Framework.Network.NetworkNodeType>();
                foreach (DataGridViewRow row in gridFcs.Rows)
                {
                    foreach (gView.Framework.Network.NetworkNodeType nodeType in Enum.GetValues(typeof(NetworkNodeType)))
                    {
                        if ((string)row.Cells[4].Value == nodeType.ToString())
                        {
                            ret.Add((int)row.Cells[0].Value, nodeType);
                            break;
                        }
                    }
                }

                return ret;
            }
        }

        #region IWizardPageNotification Member

        public void OnShowWizardPage()
        {
            FillGrid();
        }

        #endregion

        #region IWizardPageNecessity Member

        public bool CheckNecessity()
        {
            if (_selected == null || _selected.NodeFeatureclasses == null || _selected.NodeFeatureclasses.Count == 0)
                return false;

            return true;
        }

        #endregion
    }
}

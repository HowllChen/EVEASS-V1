using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using Newtonsoft.Json;
using EVEASS_V1.ESI_Code;
using System.Threading;

namespace EVEASS_V1
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();

            Characters c = DC.Characters.First();
            bool success = c.UpdateSkills();
            MessageBox.Show("更新完成");
        }

        public static DCEVEDataContext DC = new DCEVEDataContext();

        void lvBluePrints_Load()
        {
            lvBluePrints.Columns.Clear();

            ColumnHeader ch = new ColumnHeader();
            ch.Text = "序号";
            lvBluePrints.Columns.Add(ch);


            //DataGridViewColumn dgvc = new DataGridViewColumn();
            //dgvc.HeaderText = "序号";
            //dgvc.Name = "序号";
            //dgvc.SortMode = DataGridViewColumnSortMode.Automatic;
            //dgvc.CellTemplate = new DataGridViewTextBoxCell();
            //lvBluePrints.Columns.Add(dgvc);

            //dgvc = new DataGridViewColumn();
            //dgvc.HeaderText = "ID";
            //dgvc.Name = "ID";
            //dgvc.SortMode = DataGridViewColumnSortMode.Automatic;
            //dgvc.CellTemplate = new DataGridViewTextBoxCell();
            //lvBluePrints.Columns.Add(dgvc);

            //dgvc = new DataGridViewColumn();
            //dgvc.HeaderText = "蓝图名称";
            //dgvc.Name = "蓝图名称";
            //dgvc.SortMode = DataGridViewColumnSortMode.Automatic;
            //dgvc.CellTemplate = new DataGridViewTextBoxCell();
            //lvBluePrints.Columns.Add(dgvc);

            //dgvc = new DataGridViewColumn();
            //dgvc.HeaderText = "ME";
            //dgvc.Name = "ME";
            //dgvc.SortMode = DataGridViewColumnSortMode.Automatic;
            //dgvc.CellTemplate = new DataGridViewTextBoxCell();
            //lvBluePrints.Columns.Add(dgvc);

            //dgvc = new DataGridViewColumn();
            //dgvc.HeaderText = "TE";
            //dgvc.Name = "TE";
            //dgvc.SortMode = DataGridViewColumnSortMode.Automatic;
            //dgvc.CellTemplate = new DataGridViewTextBoxCell();
            //lvBluePrints.Columns.Add(dgvc);

            //lvBluePrints.Rows.Clear();
            //DCEVEDataContext dc = new DCEVEDataContext();
            //DataGridViewRow dgvr;
            //int i = 1;
            //int rowIndex;
            //foreach (var c in dc.CustomerBluePrints)
            //{
            //    dgvr = new DataGridViewRow();
            //    rowIndex = lvBluePrints.Rows.Add(dgvr);
            //    lvBluePrints.Rows[rowIndex].Cells[0].Value = i++;
            //    lvBluePrints.Rows[rowIndex].Cells[1].Value = c.itemID;
            //    lvBluePrints.Rows[rowIndex].Cells[2].Value = c.industryBlueprints.invTypes.typeNameCN;
            //    lvBluePrints.Rows[rowIndex].Cells[3].Value = c.MaterialLevel;
            //    lvBluePrints.Rows[rowIndex].Cells[4].Value = c.TimeLevel;
            //}

            //lvBluePrints.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
        }

        private void btnExchangeKey_Click(object sender, EventArgs e)
        {
        }


    }
}

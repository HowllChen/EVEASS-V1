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
using System.Runtime.InteropServices;
using System.Web.UI.WebControls;

namespace EVEASS_V1
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            //UniversePrice.Update();

            //var marketprice = DC.MarketPrice.Take(5);
            //string[] names = { "程序A", "程序B", "程序C" };

            //foreach (var mp in marketprice)
            //{
            //    foreach (var n in names)
            //    {
            //        Task.Run(new Action(() => {
            //            invTypes.UpdateMarketPrice(mp, n);
            //            Console.WriteLine("{0}输出物品{1}的价格为:{2}", n, mp.TypeID, mp.MinSellPrice);
            //        }));
            //    }
            //}

            load_lbxBluePrints();
        }

        private void load_lbxBluePrints()
        {
            DCEVEDataContext dc = new DCEVEDataContext();


            foreach (var bp in dc.OwnedBluePrints)
            {
                lbxBluePrints.Items.Add(bp);
            }
        }

        public void load_dgvBluePrints()
        {
            List<DataGridViewColumn> dataGridViewColumns = new List<DataGridViewColumn>();

            DataGridViewColumn dgvc;

            dgvc = new DataGridViewTextBoxColumn();
            dgvc.HeaderText = "TypeID";
            dataGridViewColumns.Add(dgvc);

            dgvc = new DataGridViewTextBoxColumn();
            dgvc.HeaderText = "蓝图名称";
            dataGridViewColumns.Add(dgvc);

            dgvc = new DataGridViewTextBoxColumn();
            dgvc.HeaderText = "MT";
            dataGridViewColumns.Add(dgvc);

            dgvc = new DataGridViewTextBoxColumn();
            dgvc.HeaderText = "TE";
            dataGridViewColumns.Add(dgvc);

            //dgvc = new DataGridViewTextBoxColumn();
            //dgvc.HeaderText = "产品名称";
            //dataGridViewColumns.Add(dgvc);

            //dgvc = new DataGridViewTextBoxColumn();
            //dgvc.HeaderText = "产品数量";
            //dgvc.DefaultCellStyle.Format = "#,###";
            //dataGridViewColumns.Add(dgvc);

            //dgvc = new DataGridViewTextBoxColumn();
            //dgvc.HeaderText = "产品价值";
            //dgvc.DefaultCellStyle.Format = "#,###.00";
            //dataGridViewColumns.Add(dgvc);

            dgvc = new DataGridViewTextBoxColumn();
            dgvc.HeaderText = "材料花费";
            dgvc.DefaultCellStyle.Format = "#,###.00";
            dataGridViewColumns.Add(dgvc);

            dgvBluePrints.Columns.AddRange(dataGridViewColumns.ToArray());

            List<BluePrints> blueprints = new List<BluePrints>();

            DCEVEDataContext dc = new DCEVEDataContext();
            var character = dc.Characters.First();

            foreach (var bp in dc.BluePrints)
            {
                if (character.CheckSkills(bp.industryActivitySkills.Where(p=>p.activityID == 1)))
                    blueprints.Add(bp);
            }

            foreach (var bp in blueprints)
            {
                int rowIndex = dgvBluePrints.Rows.Add();
                dgvBluePrints.Rows[rowIndex].Cells[0].Value = bp.BluePrintTypeID;
                dgvBluePrints.Rows[rowIndex].Cells[1].Value = bp.invTypes.Name;

                if (bp.OwnedBluePrints.Count == 0)
                {
                    dgvBluePrints.Rows[rowIndex].Cells[2].Value = 0;
                    dgvBluePrints.Rows[rowIndex].Cells[3].Value = 0;
                }
                else
                {
                    dgvBluePrints.Rows[rowIndex].Cells[2].Value = bp.OwnedBluePrints.First().MaterialEfficiency;
                    dgvBluePrints.Rows[rowIndex].Cells[3].Value = bp.OwnedBluePrints.First().TimeEfficiency;
                }

                Task.Run(() =>
                {
                    DCEVEDataContext temp = new DCEVEDataContext();

                    var blue = temp.BluePrints.Single(p => p.BluePrintTypeID == bp.BluePrintTypeID);
                    decimal cost = blue.industryActivityMaterials.Where(p => p.activityID == 1)
                        .Sum(p => (decimal)Math.Ceiling(p.quantity * (1 - (bp.OwnedBluePrints.Count == 0 ? 0 : bp.OwnedBluePrints.First().MaterialEfficiency) * 0.01)) * p.invTypeMaterial.MarketPriceOfLocation.MinSellPrice);
                    SetDGVMaterialsCost(rowIndex, cost);
                });
            }
        }

        private delegate void DeleSetDGVMaterialsCost(int rowIndex, decimal cost);
        private void SetDGVMaterialsCost(int rowIndex, decimal cost)
        {
            if (this.InvokeRequired)
            {
                DeleSetDGVMaterialsCost dele = new DeleSetDGVMaterialsCost(SetDGVMaterialsCost);
                this.Invoke(dele, new object[] { rowIndex, cost });
            }
            else
            {
                dgvBluePrints.Rows[rowIndex].Cells[4].Value = cost;
            }
        }

        //public static DCEVEDataContext DC = new DCEVEDataContext();
        public static int ESIDateUpdateTimeSpan = 15;           // ESI 数据更新过期时间延长(分钟)
        public static decimal ISKPerDayOfHistory = 20000000;
        public static Locations MarketLocation = (new DCEVEDataContext()).Locations.First();
        public static Characters SelectedCharacters = (new DCEVEDataContext()).Characters.First();

        void lvBluePrints_Load()
        {
            //lvBluePrints.Columns.Clear();

            //ColumnHeader ch = new ColumnHeader();
            //ch.Text = "序号";
            //lvBluePrints.Columns.Add(ch);


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

        private void btnUpdateBluePrints_Click(object sender, EventArgs e)
        {

        }

        private void selectBluePrint(int blueprintID)
        {
            DCEVEDataContext dc = new DCEVEDataContext();
            var blueprint = dc.BluePrints.Single(p => p.BluePrintTypeID == blueprintID);

            lblBluePrintID.Text = blueprint.BluePrintTypeID.ToString();
            lblBluePrintName.Text = blueprint.invTypes.Name;
            //lblCorporationTrade.Text = 
            lblBluePrintBasePrice.Text = string.Format("蓝图原价:{0:#,###.00} ISK", blueprint.invTypes.basePrice);
            lblMaterialEfficiency.Text = string.Format("材料:{0}", blueprint.MaterialEfficiency);
            lblTimeEfficiency.Text = string.Format("时间:{0}", blueprint.TimeEfficiency);

        }

        private void lbxBluePrints_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedBluePrint = (OwnedBluePrints)lbxBluePrints.SelectedItem;

            lblBluePrintName.Text = selectedBluePrint.BluePrints.invTypes.Name;         // 蓝图名称

            // 载入蓝图图标
            string imgPath = Path.Combine("Images", selectedBluePrint.BluePrintID.ToString()+"_64.png");
            if (File.Exists(imgPath))
                pictBP.Image = Image.FromFile(imgPath);
            else
                pictBP.Image = null;
            pictBP.Update();
        }
    }
}

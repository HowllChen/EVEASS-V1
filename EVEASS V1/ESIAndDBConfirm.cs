using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EVEASS_V1
{
    public partial class ESIAndDBConfirm : Form
    {
        public ESIAndDBConfirm()
        {
            InitializeComponent();
        }

        public List<ColumnHeader> ESIListViewColumnHeaders;
        public List<ColumnHeader> DBListViewColumnHeaders;
        public List<ListViewItem> ESIListViewItemCollection = new List<ListViewItem>();
        public List<ListViewItem> DBListViewItemCollection = new List<ListViewItem>();

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        public void LoadListViewData()
        {
            if (DBListViewColumnHeaders != null && DBListViewItemCollection.Count() != 0)
            {
                lvwDBData.Columns.AddRange(DBListViewColumnHeaders.ToArray());
                lvwDBData.Items.AddRange(DBListViewItemCollection.ToArray());
                lvwDBData.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }

            if (ESIListViewColumnHeaders != null && ESIListViewItemCollection.Count() != 0)
            {
                lvwESIData.Columns.AddRange(ESIListViewColumnHeaders.ToArray());
                lvwESIData.Items.AddRange(ESIListViewItemCollection.ToArray());
                lvwESIData.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
        }
    }
}

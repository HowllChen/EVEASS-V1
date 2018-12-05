using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EVEASS_V1.ESI_Code;

namespace EVEASS_V1
{
    public partial class SignCharacter : Form
    {
        public SignCharacter()
        {
            InitializeComponent();
        }

        private Characters _characters = null;

        private void btnSign_Click(object sender, EventArgs e)
        {
            _characters = Characters.SignCharcher();

            MessageBox.Show("角色注册成功.");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // TODO: 若相同军团有一个角色设置了军团权限, 则其余角色不可以设置军团权限
            _characters.Market = cbxMarket.Checked;
            _characters.Corporation = cbxCEO.Checked;
            DCEVEDataContext dc = new DCEVEDataContext();
            dc.Characters.InsertOnSubmit(_characters);
            dc.SubmitChanges();
            MessageBox.Show("角色保存成功.");
        }
    }
}

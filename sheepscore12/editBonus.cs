using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace sheepscore12
{
    public partial class editBonus : Form
    {
        public editBonus(bool IsOverride, decimal cur_bonus)
        {
            InitializeComponent();

            radioOverride.Checked = IsOverride;
            radioBonus.Checked = !IsOverride;
            numScore.Value = cur_bonus;
        }

        public ShGame.ShBonusType BonusType
        {
            get
            {
                if (radioBonus.Checked) return ShGame.ShBonusType.Add;
                else return ShGame.ShBonusType.Override;
            }
        }

        public decimal BonusValue
        {
            get
            {
                return numScore.Value;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void editBonus_Shown(object sender, EventArgs e)
        {
            numScore.Select();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AirConditioning
{
    public partial class frmMoneyBox : Form
    {
        public frmMoneyBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtComName.Text.Trim() == "")
                {
                    MessageBox.Show("请填写LPT口", "信息提示");
                    return;
                }
                byte[] buffer = new byte[5];
                buffer[0] = 0x1B;
                buffer[1] = 0x70;
                buffer[2] = 0x00;
                buffer[3] = 0x3C;
                buffer[4] = 0XFF;
                PrintFactory.SendCMDToLPT(txtComName.Text.Trim(), buffer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误信息");
            }
        }
    }
}
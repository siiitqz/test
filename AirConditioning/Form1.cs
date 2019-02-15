using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AirConditioning
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string strSql = @"select distinct ProjectID,ProjectName 
from W_InOut_Detail where state = 0 and Type = -1 
group by ProjectID,ProjectName";
            DataTable dt = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, strSql).Tables[0];
            if (dt != null) {
                foreach (DataRow  var in dt.Rows)
                {
                    strSql = string.Format(@"Select Count(*) from(select '' as EqualNum,a.MaterialID,a.MaterialName,a.SupplierName,
a.Unit,a.Spec,sum(a.Num)as Num, 0.00 as Price,
Money,''as CostType,a.BillID 
from W_InOut_Detail a,W_InOut b where a.State = 0 and a.Type = -1 and 
a.BillID = b.BillID and a.ProjectID = b.ProjectID and 
a.ProjectID = '{0}' and 
b.BillDate between '2009-09-01 00:00:00' and  '2012-10-31 23:59:59' 
group by MaterialID,MaterialName,a.SupplierName,a.Unit,a.Spec,a.Money,a.BillID) dd", var["ProjectID"]);
                    if (int.Parse(DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, strSql).Tables[0].Rows[0][0].ToString()) == 0) {
                        listBox1.Items.Add(var["ProjectID"].ToString());
                    }
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AirConditioning
{
    public partial class frmStreetsLightsNew : Form
    {
        public frmStreetsLightsNew()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, "Select distinct MaterialID from w_warehouse").Tables[0];
                label1.Text = dt.Rows.Count.ToString();
                foreach (DataRow row in dt.Rows)
                {
                    string strSql = "Select MaterialID,MaterialName,Spec,sum(Num) as Num,Unit,BatchNumber from w_warehouse where MaterialID='"
                        + row["MaterialID"].ToString().Trim() + "'group by MaterialID,MaterialName,Spec,Unit,BatchNumber order by MaterialId,BatchNumber";
                    DataTable materialTable = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, strSql).Tables[0];
                    if (materialTable != null)
                    {
                        strSql = "Delete from w_warehouse Where MaterialID='" + row["MaterialID"].ToString().Trim() + "'";
                        DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, strSql);
                        foreach (DataRow var in materialTable.Rows)
                        {
                            strSql = string.Format("Insert into w_warehouse(InnerID,BillID,MaterialID,MaterialName,Spec,Num,Unit,Price,Money,BatchNumber) Values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                                Guid.NewGuid().ToString(), "", var["MaterialID"], var["MaterialName"], var["Spec"], var["Num"], var["Unit"], "0", "0", var["BatchNumber"]);
                            DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, strSql);
                        }
                        if (materialTable.Rows.Count > 0)
                        {
                            label2.Text = Convert.ToString(int.Parse(label2.Text.Trim()) + 1);
                        }
                    }
                }
                MessageBox.Show("成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private static DataView lastDv = new DataView();
        private int currentIndex = 0;
        private int sumIndex = 0;
        private void BingData()
        {
            try
            {
                string flagMaterialId = lastDv[currentIndex]["MaterialID"].ToString();
                textBox1.Text = flagMaterialId;
                string SDate = "2012-01-01 00:00:00";
                string EDate = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
                //结存数量
                decimal CNum = 0;
                decimal SourceNum = 0;
                decimal Num = 0;
                decimal WNum = 0;

                //如果没有选择了封帐信息
                if (this.valHistory.Checked == false)
                {
                    //开始日期向前减去一天
                    string DDate = "SELECT dateadd(ms,-3,DATEADD(dd, DATEDIFF(dd,0,'" + SDate.ToString() + "'), 0)) as DDate";
                    DataView dvDDate = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, DDate).Tables[0].DefaultView;

                    string FromComb = string.Empty; //原来数据库
                    string ToComb = string.Empty; //封帐数据库

                    string sqlDatabase = "select * from b_DataBase where CompanyID = 'Company1'";
                    DataView dvDatabase = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlDatabase).Tables[0].DefaultView;
                    //表示还没有封帐
                    if (dvDatabase.Count == 0)
                    {
                        //最早入库日期
                        string sqlDateIn = "select Min(BillDate) as BillDate from W_InOut where IOFlag = 1";
                        DataView dvDateIn = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlDateIn).Tables[0].DefaultView;

                        //最早出库库日期
                        string sqlDateOut = "select Min(BillDate) as BillDate from W_InOut where IOFlag = -1";
                        DataView dvDateOut = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlDateOut).Tables[0].DefaultView;

                        //最早加工日期
                        string sqlDateM = "select Min(TakeDate) as BillDate from W_Machining ";
                        DataView dvDateM = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlDateM).Tables[0].DefaultView;

                        //以往入库
                        string sqlIn = "select sum(SourceNum) as SourceNum from V_MaterialOutDetail where MaterialID = '" + flagMaterialId + "' and BillDate between '" + dvDateIn[0]["BillDate"] + "'  and  '" + dvDDate[0]["DDate"] + "' and IOFlag = 1";
                        DataView dvIn = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlIn).Tables[0].DefaultView;

                        //以往出库
                        string sqlOut = "select sum(Num) as Num from V_MaterialOutDetail where MaterialID = '" + flagMaterialId + "' and BillDate between '" + dvDateOut[0]["BillDate"] + "' and  '" + dvDDate[0]["DDate"] + "' and IOFlag = -1";
                        DataView dvOut = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlOut).Tables[0].DefaultView;

                        //加工出库
                        string sqlMaching = "select sum(SendCount) as WNum from V_W_Machining  where  MaterialID = '" + flagMaterialId + "' and TakeDate between '" + dvDateM[0]["BillDate"] + "'  and  '" + dvDDate[0]["DDate"] + "'";
                        DataView dvMaching = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlMaching).Tables[0].DefaultView;

                        if (dvIn[0]["SourceNum"] == DBNull.Value)
                        {
                            SourceNum = 0;
                        }
                        else
                        {
                            SourceNum = Convert.ToDecimal(dvIn[0]["SourceNum"]);
                        }
                        if (dvOut[0]["Num"] == DBNull.Value)
                        {
                            Num = 0;
                        }
                        else
                        {
                            Num = Convert.ToDecimal(dvOut[0]["Num"]);
                        }
                        if (dvMaching[0]["WNum"] == DBNull.Value)
                        {
                            WNum = 0;
                        }
                        else
                        {
                            WNum = Convert.ToDecimal(dvMaching[0]["WNum"]);
                        }

                        CNum = SourceNum - Num - WNum;
                    }
                    else  //已经有封帐信息
                    {
                        FromComb = dvDatabase[0]["DataBaseY"].ToString(); //原来数据库
                        ToComb = dvDatabase[0]["DataBaseH"].ToString(); //封帐数据库

                        //最早入库日期
                        string sqlDateIn = "select Min(BillDate) as BillDate from " + ToComb + "..W_InOut where IOFlag = 1 order by BillDate";
                        DataView dvDateIn = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlDateIn).Tables[0].DefaultView;

                        //最早出库库日期
                        string sqlDateOut = "select Min(BillDate) as BillDate from " + ToComb + "..W_InOut where IOFlag = -1";
                        DataView dvDateOut = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlDateOut).Tables[0].DefaultView;

                        //最早加工日期
                        string sqlDateM = "select Min(TakeDate) as BillDate from " + ToComb + "..W_Machining ";
                        DataView dvDateM = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlDateM).Tables[0].DefaultView;

                        //以往入库
                        string sqlIn = "select sum(SourceNum) as SourceNum from " + FromComb + "..V_MaterialOutDetail where MaterialID = '" + flagMaterialId + "' and BillDate between '" + dvDateIn[0]["BillDate"] + "'  and  '" + dvDDate[0]["DDate"] + "' and IOFlag = 1";
                        sqlIn += " union all ";
                        sqlIn += " select sum(SourceNum) as SourceNum from " + ToComb + "..V_MaterialOutDetail where MaterialID = '" + flagMaterialId + "' and BillDate between '" + dvDateIn[0]["BillDate"] + "'  and  '" + dvDDate[0]["DDate"] + "' and IOFlag = 1";
                        DataView dvIn = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlIn).Tables[0].DefaultView;

                        for (int j = 0; j < dvIn.Count; j++)
                        {
                            if (dvIn[j]["SourceNum"] == DBNull.Value)
                            {
                                SourceNum = 0;
                            }
                            else
                            {
                                SourceNum = SourceNum + Convert.ToDecimal(dvIn[j]["SourceNum"]);
                            }
                        }

                        //以往出库
                        string sqlOut = "select sum(Num) as Num from " + FromComb + "..V_MaterialOutDetail where MaterialID = '" + flagMaterialId + "' and BillDate between '" + dvDateOut[0]["BillDate"] + "' and  '" + dvDDate[0]["DDate"] + "' and IOFlag = -1";
                        sqlOut += " union all ";
                        sqlOut += " select sum(Num) as Num from " + ToComb + "..V_MaterialOutDetail where MaterialID = '" + flagMaterialId + "' and BillDate between '" + dvDateOut[0]["BillDate"] + "' and  '" + dvDDate[0]["DDate"] + "' and IOFlag = -1";
                        DataView dvOut = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlOut).Tables[0].DefaultView;

                        for (int k = 0; k < dvOut.Count; k++)
                        {
                            if (dvOut[k]["Num"] == DBNull.Value)
                            {
                                Num = 0;
                            }
                            else
                            {
                                Num = Num + Convert.ToDecimal(dvOut[k]["Num"]);
                            }
                        }

                        //加工出库
                        string sqlMaching = "select sum(SendCount) as WNum from " + FromComb + "..V_W_Machining  where  MaterialID = '" + flagMaterialId + "' and TakeDate between '" + dvDateM[0]["BillDate"] + "'  and  '" + dvDDate[0]["DDate"] + "'";
                        sqlMaching += " union all ";
                        sqlMaching += " select sum(SendCount) as WNum from " + ToComb + "..V_W_Machining  where  MaterialID = '" + flagMaterialId + "' and TakeDate between '" + dvDateM[0]["BillDate"] + "'  and  '" + dvDDate[0]["DDate"] + "'";
                        DataView dvMaching = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlMaching).Tables[0].DefaultView;

                        for (int m = 0; m > dvMaching.Count; m++)
                        {
                            if (dvMaching[m]["WNum"] == DBNull.Value)
                            {
                                WNum = 0;
                            }
                            else
                            {
                                WNum = WNum + Convert.ToDecimal(dvMaching[m]["WNum"]);
                            }
                        }
                        CNum = SourceNum - Num - WNum;
                    }

                    string sql = string.Empty;

                    //以往结存
                    sql = "select '' as BillDate,'上期结存' as IOFlag,'' as BillID,'' as SupplierName,'' as ProjectName,'' as Storekeeper,'" + CNum.ToString() + "' as Num,0.00 as SNum,'' as Remark,'' as BatchNumber ";
                    sql += " union all  ";
                    sql += "select BillDate,'入 库' as IOFlag,BillID,SupplierName,ProjectName,'' as Storekeeper,SourceNum as Num,0.00 as SNum,Remark,BatchNumber from V_MaterialOutDetail  where MaterialID = '" + flagMaterialId + "' and BillDate between '" + SDate.ToString() + "'  and  '" + EDate.ToString() + "' and IOFlag = 1";
                    sql += " union all  ";
                    sql += "select BillDate,'出 库' as IOFlag,BillID,ProjectID as SupplierName,ProjectName,Storekeeper,Num,0.00 as SNum,Remark,BatchNumber from V_MaterialOutDetail where MaterialID = '" + flagMaterialId + "' and BillDate between '" + SDate.ToString() + "'  and  '" + EDate.ToString() + "' and IOFlag = -1";
                    sql += " order by BillDate,BillID";

                    DataTable dt = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sql).Tables[0];

                    //材料加工出库
                    string strSql = "select TakeDate as BillDate,'加 工' as IOFlag,BillID,ProcesCon as SupplierName,ProcessNo as ProjectName,RecMan as Storekeeper,SendCount,0.00 as SNum,'' as Remark,BatchNumber,BatchCount from V_W_Machining  where  MaterialID = '" + flagMaterialId + "' and TakeDate between '" + SDate.ToString() + "'  and  '" + EDate.ToString() + "'";
                    DataTable dtMaching = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, strSql).Tables[0];
                    if (dtMaching != null && dtMaching.Rows.Count > 0)
                    {
                        for (int m = 0; m < dtMaching.Rows.Count; m++)
                        {
                            DataRow dr = dtMaching.Rows[m];
                            string BatchNumber = dr["BatchNumber"].ToString();
                            if (BatchNumber.Trim() != "")
                            {
                                string[] arrBatchNumber = BatchNumber.Trim().Split(',');
                                string[] arrBatchCount = dr["BatchCount"].ToString().Trim().Split(',');
                                for (int b = 0; b < arrBatchNumber.Length; b++)
                                {
                                    if (arrBatchNumber[b].Trim() != "" && arrBatchCount[b].Trim() != "")
                                    {
                                        DataRow newRow = dt.NewRow();
                                        newRow["BillDate"] = dr["BillDate"];
                                        newRow["IOFlag"] = dr["IOFlag"];
                                        newRow["BillID"] = dr["BillID"];
                                        newRow["SupplierName"] = dr["SupplierName"];
                                        newRow["ProjectName"] = dr["ProjectName"];
                                        newRow["Storekeeper"] = dr["Storekeeper"];
                                        newRow["BillDate"] = dr["BillDate"];
                                        newRow["Remark"] = dr["Remark"];
                                        newRow["Num"] = arrBatchCount[b].Trim();
                                        newRow["BatchNumber"] = arrBatchNumber[b].Trim();
                                        dt.Rows.Add(newRow);
                                    }
                                }
                            }
                            else
                            {
                                DataRow newRow = dt.NewRow();
                                newRow["BillDate"] = dr["BillDate"];
                                newRow["IOFlag"] = dr["IOFlag"];
                                newRow["BillID"] = dr["BillID"];
                                newRow["SupplierName"] = dr["SupplierName"];
                                newRow["ProjectName"] = dr["ProjectName"];
                                newRow["Storekeeper"] = dr["Storekeeper"];
                                newRow["BillDate"] = dr["BillDate"];
                                newRow["Remark"] = dr["Remark"];
                                newRow["Num"] = dr["SendCount"];
                                newRow["BatchNumber"] = dr["BatchNumber"];
                                dt.Rows.Add(newRow);
                            }
                        }
                    }

                    dt.DefaultView.Sort = "BillDate Asc,BillID Asc";
                    DataView dv = dt.DefaultView;
                    decimal SNum = 0;
                    for (int i = 0; i < dv.Count; i++)
                    {
                        //出库入库都选中

                        if (dv[i]["IOFlag"].ToString() == "入 库" || dv[i]["IOFlag"].ToString() == "上期结存")
                        {
                            SNum = SNum + Convert.ToDecimal(dv[i]["Num"]);
                            dv[i]["SNum"] = SNum.ToString(); //结存数量
                            dv[i]["SupplierName"] = dv[i]["SupplierName"].ToString() + " " + dv[i]["ProjectName"].ToString() + " " + dv[i]["Remark"].ToString();
                        }
                        if (dv[i]["IOFlag"].ToString() == "出 库")
                        {
                            SNum = SNum - Convert.ToDecimal(dv[i]["Num"]);
                            dv[i]["SNum"] = SNum.ToString(); //结存数量
                            //SupplierName
                            dv[i]["SupplierName"] = dv[i]["SupplierName"].ToString() + " " + dv[i]["ProjectName"].ToString() + " " + dv[i]["Remark"].ToString();
                        }
                        if (dv[i]["IOFlag"].ToString() == "加 工")
                        {
                            SNum = SNum - Convert.ToDecimal(dv[i]["Num"]);
                            dv[i]["SNum"] = SNum.ToString(); //结存数量
                            //SupplierName
                            dv[i]["SupplierName"] = dv[i]["ProjectName"].ToString() + " " + dv[i]["SupplierName"].ToString() + " " + dv[i]["Remark"].ToString();
                        }
                    }
                    dataGridView1.DataSource = dv;
                    string strSqlssss = "Select sum(Num) from w_warehouse Where MaterialID='" + flagMaterialId + "'";
                    if (Convert.ToDecimal(dv[dv.Count - 1]["SNum"]) != Convert.ToDecimal(DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, strSqlssss).Tables[0].Rows[0][0].ToString()))
                    {
                        textBox2.Text = textBox2.Text + "," + flagMaterialId;
                    }
                }
                //如果选择了封帐
                if (this.valHistory.Checked == true)
                {
                    string FromComb = string.Empty; //原来数据库
                    string ToComb = string.Empty; //封帐数据库

                    string sqlDatabase = "select * from b_DataBase where CompanyID = 'Company1'";
                    DataView dvDatabase = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlDatabase).Tables[0].DefaultView;

                    //开始日期向前减去一天
                    string DDate = "SELECT dateadd(ms,-3,DATEADD(dd, DATEDIFF(dd,0,'" + SDate.ToString() + "'), 0)) as DDate";
                    DataView dvDDate = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, DDate).Tables[0].DefaultView;

                    if (dvDatabase.Count == 0)
                    {
                        //最早入库日期
                        string sqlDateIn = "select Min(BillDate) as BillDate from W_InOut where IOFlag = 1";
                        DataView dvDateIn = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlDateIn).Tables[0].DefaultView;

                        //最早出库库日期
                        string sqlDateOut = "select Min(BillDate) as BillDate from W_InOut where IOFlag = -1";
                        DataView dvDateOut = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlDateOut).Tables[0].DefaultView;

                        //最早加工日期
                        string sqlDateM = "select Min(TakeDate) as BillDate from W_Machining ";
                        DataView dvDateM = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlDateM).Tables[0].DefaultView;

                        //以往入库
                        string sqlIn = "select sum(SourceNum) as SourceNum from V_MaterialOutDetail where MaterialID = '" + flagMaterialId + "' and BillDate between '" + dvDateIn[0]["BillDate"] + "'  and  '" + dvDDate[0]["DDate"] + "' and IOFlag = 1";
                        DataView dvIn = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlIn).Tables[0].DefaultView;

                        //以往出库
                        string sqlOut = "select sum(Num) as Num from V_MaterialOutDetail where MaterialID = '" + flagMaterialId + "' and BillDate between '" + dvDateOut[0]["BillDate"] + "' and  '" + dvDDate[0]["DDate"] + "' and IOFlag = -1";
                        DataView dvOut = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlOut).Tables[0].DefaultView;

                        //加工出库
                        string sqlMaching = "select sum(SendCount) as WNum from V_W_Machining  where  MaterialID = '" + flagMaterialId + "' and TakeDate between '" + dvDateM[0]["BillDate"] + "'  and  '" + dvDDate[0]["DDate"] + "'";
                        DataView dvMaching = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlMaching).Tables[0].DefaultView;

                        if (dvIn[0]["SourceNum"] == DBNull.Value)
                        {
                            SourceNum = 0;
                        }
                        else
                        {
                            SourceNum = Convert.ToDecimal(dvIn[0]["SourceNum"]);
                        }
                        if (dvOut[0]["Num"] == DBNull.Value)
                        {
                            Num = 0;
                        }
                        else
                        {
                            Num = Convert.ToDecimal(dvOut[0]["Num"]);
                        }
                        if (dvMaching[0]["WNum"] == DBNull.Value)
                        {
                            WNum = 0;
                        }
                        else
                        {
                            WNum = Convert.ToDecimal(dvMaching[0]["WNum"]);
                        }

                        CNum = SourceNum - Num - WNum;

                        string sql = string.Empty;

                        //以往结存
                        sql = "select '' as BillDate,'上期结存' as IOFlag,'' as BillID,'' as SupplierName,'' as ProjectName,'' as Storekeeper,'" + CNum.ToString() + "' as Num,0.00 as SNum,'' as Remark ";
                        sql += " union all ";
                        sql += "select BillDate,'入 库' as IOFlag,BillID,SupplierName,ProjectName,'' as Storekeeper,SourceNum as Num,0.00 as SNum,Remark from V_MaterialOutDetail  where MaterialID = '" + flagMaterialId + "' and BillDate between '" + SDate.ToString() + "'  and  '" + EDate.ToString() + "' and IOFlag = 1";
                        sql += " union all  ";
                        sql += "select BillDate,'出 库' as IOFlag,BillID,ProjectID as SupplierName,ProjectName,Storekeeper,Num,0.00 as SNum,Remark from V_MaterialOutDetail where MaterialID = '" + flagMaterialId + "' and BillDate between '" + SDate.ToString() + "'  and  '" + EDate.ToString() + "' and IOFlag = -1";
                        sql += " union all  ";
                        sql += " select TakeDate as BillDate,'加 工' as IOFlag,BillID,ProcesCon as SupplierName,ProcessNo as ProjectName,RecMan as Storekeeper,SendCount,0.00 as SNum,'' as Remark from V_W_Machining  where  MaterialID = '" + flagMaterialId + "' and TakeDate between '" + SDate.ToString() + "'  and  '" + EDate.ToString() + "'";
                        sql += " order by BillDate,BillID";

                        DataView dv = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sql).Tables[0].DefaultView;

                        decimal SNum = 0;
                        for (int i = 0; i < dv.Count; i++)
                        {
                            //出库入库都选中

                            if (dv[i]["IOFlag"].ToString() == "入 库" || dv[i]["IOFlag"].ToString() == "上期结存")
                            {
                                SNum = SNum + Convert.ToDecimal(dv[i]["Num"]);
                                dv[i]["SNum"] = SNum.ToString(); //结存数量
                                dv[i]["SupplierName"] = dv[i]["SupplierName"].ToString() + " " + dv[i]["ProjectName"].ToString() + " " + dv[i]["Remark"].ToString();
                            }
                            if (dv[i]["IOFlag"].ToString() == "出 库")
                            {
                                SNum = SNum - Convert.ToDecimal(dv[i]["Num"]);
                                dv[i]["SNum"] = SNum.ToString(); //结存数量
                                //SupplierName
                                dv[i]["SupplierName"] = dv[i]["SupplierName"].ToString() + " " + dv[i]["ProjectName"].ToString() + " " + dv[i]["Remark"].ToString();
                            }
                            if (dv[i]["IOFlag"].ToString() == "加 工")
                            {
                                SNum = SNum - Convert.ToDecimal(dv[i]["Num"]);
                                dv[i]["SNum"] = SNum.ToString(); //结存数量
                                //SupplierName
                                dv[i]["SupplierName"] = dv[i]["ProjectName"].ToString() + " " + dv[i]["SupplierName"].ToString() + " " + dv[i]["Remark"].ToString();
                            }
                        }
                        dataGridView1.DataSource = dv;
                        string strSqlssss = "Select sum(Num) from w_warehouse Where MaterialID='" + flagMaterialId + "'";
                        if (Convert.ToDecimal(dv[dv.Count - 1]["SNum"]) != Convert.ToDecimal(DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, strSqlssss).Tables[0].Rows[0][0].ToString()))
                        {
                            textBox2.Text = textBox2.Text + "," + flagMaterialId;
                        }
                    }
                    //如果有封帐记录
                    if (dvDatabase.Count > 0)
                    {
                        FromComb = dvDatabase[0]["DataBaseY"].ToString(); //原来数据库
                        ToComb = dvDatabase[0]["DataBaseH"].ToString(); //封帐数据库

                        //最早入库日期
                        string sqlDateIn = "select Min(BillDate) as BillDate from " + ToComb + "..W_InOut where IOFlag = 1";
                        DataView dvDateIn = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlDateIn).Tables[0].DefaultView;

                        //最早出库库日期
                        string sqlDateOut = "select Min(BillDate) as BillDate from " + ToComb + "..W_InOut where IOFlag = -1";
                        DataView dvDateOut = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlDateOut).Tables[0].DefaultView;

                        //最早加工日期
                        string sqlDateM = "select Min(TakeDate) as BillDate from " + ToComb + "..W_Machining ";
                        DataView dvDateM = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlDateM).Tables[0].DefaultView;

                        //以往入库
                        string sqlIn = "select sum(SourceNum) as SourceNum from " + FromComb + "..V_MaterialOutDetail where MaterialID = '" + flagMaterialId + "' and BillDate between '" + dvDateIn[0]["BillDate"] + "'  and  '" + dvDDate[0]["DDate"] + "' and IOFlag = 1";
                        sqlIn += " union all   ";
                        sqlIn += " select sum(SourceNum) as SourceNum from " + ToComb + "..V_MaterialOutDetail where MaterialID = '" + flagMaterialId + "' and BillDate between '" + dvDateIn[0]["BillDate"] + "'  and  '" + dvDDate[0]["DDate"] + "' and IOFlag = 1";
                        DataView dvIn = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlIn).Tables[0].DefaultView;

                        for (int j = 0; j < dvIn.Count; j++)
                        {
                            if (dvIn[j]["SourceNum"] == DBNull.Value)
                            {
                                SourceNum = 0;
                            }
                            else
                            {
                                SourceNum = SourceNum + Convert.ToDecimal(dvIn[j]["SourceNum"]);
                            }
                        }

                        //以往出库
                        string sqlOut = "select sum(Num) as Num from " + FromComb + "..V_MaterialOutDetail where MaterialID = '" + flagMaterialId + "' and BillDate between '" + dvDateOut[0]["BillDate"] + "' and  '" + dvDDate[0]["DDate"] + "' and IOFlag = -1";
                        sqlOut += " union all   ";
                        sqlOut += " select sum(Num) as Num from " + ToComb + "..V_MaterialOutDetail where MaterialID = '" + flagMaterialId + "' and BillDate between '" + dvDateOut[0]["BillDate"] + "' and  '" + dvDDate[0]["DDate"] + "' and IOFlag = -1";
                        DataView dvOut = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlOut).Tables[0].DefaultView;

                        for (int k = 0; k < dvOut.Count; k++)
                        {
                            if (dvOut[k]["Num"] == DBNull.Value)
                            {
                                Num = 0;
                            }
                            else
                            {
                                Num = Num + Convert.ToDecimal(dvOut[k]["Num"]);
                            }
                        }

                        //加工出库
                        string sqlMaching = "select sum(SendCount) as WNum from " + FromComb + "..V_W_Machining  where  MaterialID = '" + flagMaterialId + "' and TakeDate between '" + dvDateM[0]["BillDate"] + "'  and  '" + dvDDate[0]["DDate"] + "'";
                        sqlMaching += " union all   ";
                        sqlMaching += " select sum(SendCount) as WNum from " + ToComb + "..V_W_Machining  where  MaterialID = '" + flagMaterialId + "' and TakeDate between '" + dvDateM[0]["BillDate"] + "'  and  '" + dvDDate[0]["DDate"] + "'";
                        DataView dvMaching = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlMaching).Tables[0].DefaultView;

                        for (int m = 0; m > dvMaching.Count; m++)
                        {
                            if (dvMaching[m]["WNum"] == DBNull.Value)
                            {
                                WNum = 0;
                            }
                            else
                            {
                                WNum = WNum + Convert.ToDecimal(dvMaching[m]["WNum"]);
                            }
                        }
                        CNum = SourceNum - Num - WNum;

                        string sql = string.Empty;

                        //以往结存
                        sql = "select '' as BillDate,'上期结存' as IOFlag,'' as BillID,'' as SupplierName,'' as ProjectName,'' as Storekeeper,'" + CNum.ToString() + "' as Num,0.00 as SNum,'' as Remark ";
                        sql += " union all   ";
                        sql += "select BillDate,'入 库' as IOFlag,BillID,SupplierName,ProjectName,'' as Storekeeper,SourceNum as Num,0.00 as SNum,Remark from " + FromComb + "..V_MaterialOutDetail  where MaterialID = '" + flagMaterialId + "' and BillDate between '" + SDate.ToString() + "'  and  '" + EDate.ToString() + "' and IOFlag = 1";
                        sql += " union all   ";
                        sql += "select BillDate,'入 库' as IOFlag,BillID,SupplierName,ProjectName,'' as Storekeeper,SourceNum as Num,0.00 as SNum,Remark from " + ToComb + "..V_MaterialOutDetail  where MaterialID = '" + flagMaterialId + "' and BillDate between '" + SDate.ToString() + "'  and  '" + EDate.ToString() + "' and IOFlag = 1";
                        sql += " union all   ";
                        sql += "select BillDate,'出 库' as IOFlag,BillID,ProjectID as SupplierName,ProjectName,Storekeeper,Num,0.00 as SNum,Remark from " + FromComb + "..V_MaterialOutDetail where MaterialID = '" + flagMaterialId + "' and BillDate between '" + SDate.ToString() + "'  and  '" + EDate.ToString() + "' and IOFlag = -1";
                        sql += " union all   ";
                        sql += "select BillDate,'出 库' as IOFlag,BillID,ProjectID as SupplierName,ProjectName,Storekeeper,Num,0.00 as SNum,Remark from " + ToComb + "..V_MaterialOutDetail where MaterialID = '" + flagMaterialId + "' and BillDate between '" + SDate.ToString() + "'  and  '" + EDate.ToString() + "' and IOFlag = -1";
                        sql += " union all   ";
                        sql += " select TakeDate as BillDate,'加 工' as IOFlag,BillID,ProcesCon as SupplierName,ProcessNo as ProjectName,RecMan as Storekeeper,SendCount,0.00 as SNum,'' as Remark from " + FromComb + "..V_W_Machining  where  MaterialID = '" + flagMaterialId + "' and TakeDate between '" + SDate.ToString() + "'  and  '" + EDate.ToString() + "'";
                        sql += " union all   ";
                        sql += " select TakeDate as BillDate,'加 工' as IOFlag,BillID,ProcesCon as SupplierName,ProcessNo as ProjectName,RecMan as Storekeeper,SendCount,0.00 as SNum,'' as Remark from " + ToComb + "..V_W_Machining  where  MaterialID = '" + flagMaterialId + "' and TakeDate between '" + SDate.ToString() + "'  and  '" + EDate.ToString() + "'";
                        sql += " order by BillDate,BillID";

                        DataView dv = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sql).Tables[0].DefaultView;


                        decimal SNum = 0;
                        for (int i = 0; i < dv.Count; i++)
                        {

                            if (dv[i]["IOFlag"].ToString() == "入 库" || dv[i]["IOFlag"].ToString() == "上期结存")
                            {
                                SNum = SNum + Convert.ToDecimal(dv[i]["Num"]);
                                dv[i]["SNum"] = SNum.ToString(); //结存数量
                                dv[i]["SupplierName"] = dv[i]["SupplierName"].ToString() + " " + dv[i]["ProjectName"].ToString() + " " + dv[i]["Remark"].ToString();
                            }
                            if (dv[i]["IOFlag"].ToString() == "出 库")
                            {
                                SNum = SNum - Convert.ToDecimal(dv[i]["Num"]);
                                dv[i]["SNum"] = SNum.ToString(); //结存数量
                                //SupplierName
                                dv[i]["SupplierName"] = dv[i]["SupplierName"].ToString() + " " + dv[i]["ProjectName"].ToString() + " " + dv[i]["Remark"].ToString();
                            }
                            if (dv[i]["IOFlag"].ToString() == "加 工")
                            {
                                SNum = SNum - Convert.ToDecimal(dv[i]["Num"]);
                                dv[i]["SNum"] = SNum.ToString(); //结存数量
                                //SupplierName
                                dv[i]["SupplierName"] = dv[i]["ProjectName"].ToString() + " " + dv[i]["SupplierName"].ToString() + " " + dv[i]["Remark"].ToString();
                            }
                        }
                        dataGridView1.DataSource = dv;
                        string strSqlssss = "Select sum(Num) from w_warehouse Where MaterialID='" + flagMaterialId + "'";
                        if (Convert.ToDecimal(dv[dv.Count - 1]["SNum"]) != Convert.ToDecimal(DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, strSqlssss).Tables[0].Rows[0][0].ToString()))
                        {
                            textBox2.Text = textBox2.Text + "," + flagMaterialId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            lastDv = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, "Select distinct MaterialID from w_warehouse").Tables[0].DefaultView;
            sumIndex = lastDv.Count;
            currentIndex = 0;
        }


        /// <summary>
        /// 上
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (currentIndex - 1 < 0)
                currentIndex = 0;
            else
                currentIndex--;
            BingData();
        }

        /// <summary>
        /// 下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (currentIndex + 1 > sumIndex)
                currentIndex = sumIndex;
            else
                currentIndex++;
            BingData();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            for (int i = 0; i < sumIndex; i++)
            {
                currentIndex = i;
                BingData();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MessageBox.Show(textBox2.Text.Trim().Split(',').Length.ToString());
        }
    }
}
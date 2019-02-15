using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AirConditioning
{
    public partial class frmStreetsLightNew_HouseCount : Form
    {
        public frmStreetsLightNew_HouseCount()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 库存对比
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWarehouse_Click(object sender, EventArgs e)
        {
            DialogResult rs = MessageBox.Show("你确定要重新计算库存吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (rs == DialogResult.No)
            {
                return;
            }
            string deleteSql = "delete from W_WareHouse ";
            DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, deleteSql);

            string sql = "select * from m_goods where order by GoodsNo";
            DataView dvDDate = DBHelper.ExecuteDataset(DBHelper.connStringMain, CommandType.Text, sql).Tables[0].DefaultView;

            for (int i = 0; i < dvDDate.Count; i++)
            {
                //以往入库
                //string sqlIn = "select ifnull(sum(ifnull(SourceNum,0)),0) as SourceNum,ifnull(BatchNumber,CONCAT('016',MaterialID)) as BatchNumber from V_MaterialOutDetail where MaterialID = '" + dvDDate[i]["GoodsNo"] + "'  and IOFlag = 1 group by BatchNumber";
                string sqlIn = "select isnull(sum(isnull(SourceNum,0)),0) as SourceNum from V_MaterialOutDetail where MaterialID = '" + dvDDate[i]["GoodsNo"] + "'  and IOFlag = 1";
                DataView dvIn = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlIn).Tables[0].DefaultView;

                //以往出库
                //string sqlOut = "select ifnull(sum(ifnull(Num,0)),0) as Num,ifnull(BatchNumber,CONCAT('016',MaterialID)) as BatchNumber from V_MaterialOutDetail where MaterialID = '" + dvDDate[i]["GoodsNo"] + "' and IOFlag = -1 group by BatchNumber";
                string sqlOut = "select isnull(sum(isnull(Num,0)),0) as Num from V_MaterialOutDetail where MaterialID = '" + dvDDate[i]["GoodsNo"] + "' and IOFlag = -1 ";
                DataView dvOut = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlOut).Tables[0].DefaultView;

                //加工出库
                //string sqlMaching = "select ifnull(sum(ifnull(ApplyCount,0)),0) as WNum,ifnull(BatchNumber,CONCAT('016',MaterialID)) as BatchNumber from V_W_Machining  where  MaterialID = '" + dvDDate[i]["GoodsNo"] + "' group by BatchNumber";
                string sqlMaching = "select isnull(sum(isnull(ApplyCount,0)),0) as WNum from V_W_Machining  where  MaterialID = '" + dvDDate[i]["GoodsNo"] + "'";
                DataView dvMaching = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlMaching).Tables[0].DefaultView;

                for (int j = 0; j < dvIn.Count; j++)
                {
                    //string sqlInsert = "insert into W_WareHouse(InnerID,MaterialID,MaterialName,Spec,Unit,Num,Price,Money,BatchNumber) values ('" + Guid.NewGuid().ToString() + "','" + dvDDate[i]["GoodsNo"].ToString() + "','" + dvDDate[i]["GoodsName"].ToString() + "','" + dvDDate[i]["Spec"].ToString() + "','" + dvDDate[i]["UnitName"].ToString() + "','" + dvIn[j]["SourceNum"].ToString() + "',0,0,'" + dvIn[j]["BatchNumber"].ToString() + "')";
                    string sqlInsert = "insert into W_WareHouse(InnerID,MaterialID,MaterialName,Spec,Unit,Num,Price,Money) values ('" + Guid.NewGuid().ToString() + "','" + dvDDate[i]["GoodsNo"].ToString() + "','" + dvDDate[i]["GoodsName"].ToString() + "','" + dvDDate[i]["Spec"].ToString() + "','" + dvDDate[i]["UnitName"].ToString() + "','" + dvIn[j]["SourceNum"].ToString() + "',0,0)";
                    DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, sqlInsert);
                }

                for (int j = 0; j < dvOut.Count; j++)
                {
                    //string strSql = "Select * from W_WareHouse Where MaterialID='" + dvDDate[i]["GoodsNo"] + "' and BatchNumber='" + dvOut[j]["BatchNumber"] + "'";
                    string strSql = "Select * from W_WareHouse Where MaterialID='" + dvDDate[i]["GoodsNo"] + "' ";
                    DataView dv = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, strSql).Tables[0].DefaultView;
                    if (dv.Count > 0)
                    {
                        //if (Convert.ToDecimal(dvOut[j]["Num"].ToString()) < 0)
                        //    strSql = "Update W_WareHouse Set Num=Num+" + Math.Abs(Convert.ToDecimal(dvOut[j]["Num"].ToString())) + " Where InnerID='" + dv[0]["InnerID"] + "'";
                        //else
                        //    strSql = "Update W_WareHouse Set Num=Num-" + dvOut[j]["Num"] + " Where InnerID='" + dv[0]["InnerID"] + "'";
                        strSql = "Update W_WareHouse Set Num=Num-" + dvOut[j]["Num"] + " Where InnerID='" + dv[0]["InnerID"] + "'";
                        DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, strSql);
                    }
                    else
                    {
                        string sqlInsert = "";
                        //if (Convert.ToDecimal(dvOut[j]["Num"].ToString()) < 0)
                        //sqlInsert = "insert into W_WareHouse(InnerID,MaterialID,MaterialName,Spec,Unit,Num,Price,Money,BatchNumber) values ('" + Guid.NewGuid().ToString() + "','" + dvDDate[i]["GoodsNo"].ToString() + "','" + dvDDate[i]["GoodsName"].ToString() + "','" + dvDDate[i]["Spec"].ToString() + "','" + dvDDate[i]["UnitName"].ToString() + "','" + Convert.ToDecimal(dvOut[j]["Num"].ToString()) * -1 + "',0,0,'" + dvOut[j]["BatchNumber"].ToString() + "')";
                        sqlInsert = "insert into W_WareHouse(InnerID,MaterialID,MaterialName,Spec,Unit,Num,Price,Money) values ('" + Guid.NewGuid().ToString() + "','" + dvDDate[i]["GoodsNo"].ToString() + "','" + dvDDate[i]["GoodsName"].ToString() + "','" + dvDDate[i]["Spec"].ToString() + "','" + dvDDate[i]["UnitName"].ToString() + "','" + Convert.ToDecimal(dvOut[j]["Num"].ToString()) * -1 + "',0,0)";
                      
                        DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, sqlInsert);
                    }
                }
                for (int j = 0; j < dvMaching.Count; j++)
                {
                    //string strSql = "Select * from W_WareHouse Where MaterialID='" + dvDDate[i]["GoodsNo"] + "' and BatchNumber='" + dvMaching[j]["BatchNumber"] + "'";
                    string strSql = "Select * from W_WareHouse Where MaterialID='" + dvDDate[i]["GoodsNo"] + "'";
                    DataView dv = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, strSql).Tables[0].DefaultView;
                    if (dv.Count > 0)
                    {
                        if (Convert.ToDecimal(dvMaching[j]["WNum"].ToString()) < 0)
                            strSql = "Update W_WareHouse Set Num=Num+" + Math.Abs(Convert.ToDecimal(dvMaching[j]["WNum"].ToString())) + " Where InnerID='" + dv[0]["InnerID"] + "'";
                        else
                            strSql = "Update W_WareHouse Set Num=Num-" + dvMaching[j]["WNum"] + " Where InnerID='" + dv[0]["InnerID"] + "'";

                        DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, strSql);
                    }
                    else
                    {
                        string sqlInsert = "";
                        if (Convert.ToDecimal(dvMaching[j]["WNum"].ToString()) < 0)
                        //sqlInsert = "insert into W_WareHouse(InnerID,MaterialID,MaterialName,Spec,Unit,Num,Price,Money,BatchNumber) values ('" + Guid.NewGuid().ToString() + "','" + dvDDate[i]["GoodsNo"].ToString() + "','" + dvDDate[i]["GoodsName"].ToString() + "','" + dvDDate[i]["Spec"].ToString() + "','" + dvDDate[i]["UnitName"].ToString() + "','" + Convert.ToDecimal(dvMaching[j]["WNum"].ToString()) * -1 + "',0,0,'" + dvMaching[j]["BatchNumber"].ToString() + "')";
                            sqlInsert = "insert into W_WareHouse(InnerID,MaterialID,MaterialName,Spec,Unit,Num,Price,Money) values ('" + Guid.NewGuid().ToString() + "','" + dvDDate[i]["GoodsNo"].ToString() + "','" + dvDDate[i]["GoodsName"].ToString() + "','" + dvDDate[i]["Spec"].ToString() + "','" + dvDDate[i]["UnitName"].ToString() + "','" + Convert.ToDecimal(dvMaching[j]["WNum"].ToString()) * -1 + "',0,0)";
                        
                        DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, sqlInsert);
                    }
                }
            }
            MessageBox.Show("完成");
        }

        /// <summary>
        /// 盘点对比
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStock_Click(object sender, EventArgs e)
        {
            DialogResult rs = MessageBox.Show("你确定要重新计算盘点表吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (rs == DialogResult.No)
            {
                return;
            }
            string deleteSql = "delete from W_StockCount";
            DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, deleteSql);

            string sql = "select * from m_goods order by GoodsNo";
            DataView dvDateIn = DBHelper.ExecuteDataset(DBHelper.connStringMain, CommandType.Text, sql).Tables[0].DefaultView;
            string eeweInnerId = "";
            for (int i = 0; i < dvDateIn.Count; i++)
            {
                //以往入库
                string sqlIn = "select ifnull(SourceNum,0) as SourceNum,ifnull(Money,0) as SourceMoney,ifnull(BatchNumber,CONCAT('016',MaterialID)) as BatchNumber,ifnull(Taxes,0) as Taxes from V_MaterialOutDetail1 where MaterialID = '" + dvDateIn[i]["GoodsNo"] + "' and State = 1 and IOFlag = 1";
                DataView dvIn = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlIn).Tables[0].DefaultView;

                //以往出库
                string sqlOut = "select ifnull(Num,0) as Num,ifnull(Money,0) as OutMoney,ifnull(BatchNumber,CONCAT('016',MaterialID)) as BatchNumber,ifnull(Taxes,0) as Taxes  from V_MaterialOutDetail1 where MaterialID = '" + dvDateIn[i]["GoodsNo"] + "'  and WState = 1  and IOFlag = -1 and Type = -1";
                DataView dvOut = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlOut).Tables[0].DefaultView;

                //加工出库
                string sqlMaching = "select ifnull(ApplyCount,0) as WNum,ifnull(TotalMoney,0) as WMoney,ifnull(BatchNumber,CONCAT('016',MaterialID)) as BatchNumber,ifnull(Taxes,0) as Taxes from V_W_Machining where  MaterialID = '" + dvDateIn[i]["GoodsNo"] + "' and State =1";
                DataView dvMaching = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlMaching).Tables[0].DefaultView;

                for (int j = 0; j < dvIn.Count; j++)
                {
                    //string strSql = "Select * from W_StockCount Where MaterialID='" + dvDateIn[i]["GoodsNo"] + "' and BatchNumber='" + dvIn[j]["BatchNumber"] + "'";
                    string strSql = "Select * from W_StockCount Where MaterialID='" + dvDateIn[i]["GoodsNo"] + "'";
                    DataView dv = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, strSql).Tables[0].DefaultView;
                    //for (int z = 0; z < dv.Count; z++)
                    //{
                    if (dv.Count > 0)
                    {
                        string money = decimal.Parse(dvIn[j]["SourceMoney"].ToString()) >= 0 ? "+" + dvIn[j]["SourceMoney"].ToString() : dvIn[j]["SourceMoney"].ToString();
                        string Taxes = decimal.Parse(dvIn[j]["Taxes"].ToString()) >= 0 ? "+" + dvIn[j]["Taxes"].ToString() : dvIn[j]["Taxes"].ToString();
                        string SourceNum = decimal.Parse(dvIn[j]["SourceNum"].ToString()) >= 0 ? "+" + dvIn[j]["SourceNum"].ToString() : dvIn[j]["SourceNum"].ToString();

                        if (decimal.Parse(dvIn[j]["SourceNum"].ToString()) >= 0)
                            strSql = "Update W_StockCount Set Taxes=Taxes" + Taxes + ",Num=Num" + SourceNum + ",Money=Money" + money + " Where InnerID='" + dv[0]["InnerID"] + "'";
                        else
                            strSql = "Update W_StockCount Set Taxes=Taxes" + Taxes + ",Num=Num" + SourceNum + ",Money=Money" + money + " Where InnerID='" + dv[0]["InnerID"] + "'";

                        DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, strSql);

                    }
                    else
                    {
                        string sqlInsert = "insert into W_StockCount(InnerID,MaterialID,MaterialName,MaterialType,Spec,Unit,Num,Price,Money,BatchNumber,Taxes) values ('" + Guid.NewGuid().ToString() + "','" + dvDateIn[i]["GoodsNo"].ToString() + "','" + dvDateIn[i]["GoodsName"].ToString() + "','" + dvDateIn[i]["GoodsType"].ToString() + "','" + dvDateIn[i]["Spec"].ToString() + "','" + dvDateIn[i]["UnitName"].ToString() + "','" + dvIn[j]["SourceNum"] + "',0,'" + dvIn[j]["SourceMoney"] + "','" + dvIn[j]["BatchNumber"] + "','" + dvIn[j]["Taxes"] + "')";
                        DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, sqlInsert);

                    }
                    //}

                }

                for (int j = 0; j < dvOut.Count; j++)
                {
                    //string strSql = "Select * from W_StockCount Where MaterialID='" + dvDateIn[i]["GoodsNo"] + "' and BatchNumber='" + dvOut[j]["BatchNumber"] + "'";
                    string strSql = "Select * from W_StockCount Where MaterialID='" + dvDateIn[i]["GoodsNo"] + "'";
                    DataView dv = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, strSql).Tables[0].DefaultView;

                    if (dv.Count > 0)
                    {

                        string Taxes = decimal.Parse(dvOut[j]["Taxes"].ToString()) >= 0 ? "-" + dvOut[j]["Taxes"].ToString() : "+" + Math.Abs(decimal.Parse(dvOut[j]["Taxes"].ToString()));
                        string Num = decimal.Parse(dvOut[j]["Num"].ToString()) >= 0 ? "-" + dvOut[j]["Num"].ToString() : "+" + Math.Abs(decimal.Parse(dvOut[j]["Num"].ToString()));
                        string Money = decimal.Parse(dvOut[j]["OutMoney"].ToString()) >= 0 ? "-" + dvOut[j]["OutMoney"].ToString() : "+" + Math.Abs(decimal.Parse(dvOut[j]["OutMoney"].ToString()));
                        if (decimal.Parse(dvOut[j]["OutMoney"].ToString()) >= 0)
                            strSql = "Update W_StockCount Set Taxes=Taxes" + Taxes + ",Num=Num" + Num + ",Money=Money" + Money + " Where InnerID='" + dv[0]["InnerID"] + "'";
                        else
                            strSql = "Update W_StockCount Set Taxes=Taxes" + Taxes + ",Num=Num" + Num + ",Money=Money" + Money + " Where InnerID='" + dv[0]["InnerID"] + "'";
                        DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, strSql);

                    }
                    else
                    {
                        string Taxes = decimal.Parse(dvOut[j]["Taxes"].ToString()) >= 0 ? "-" + dvOut[j]["Taxes"].ToString() : "" + Math.Abs(decimal.Parse(dvOut[j]["Taxes"].ToString()));
                        string Num = decimal.Parse(dvOut[j]["Num"].ToString()) >= 0 ? "-" + dvOut[j]["Num"].ToString() : "" + Math.Abs(decimal.Parse(dvOut[j]["Num"].ToString()));
                        string Money = decimal.Parse(dvOut[j]["OutMoney"].ToString()) >= 0 ? "-" + dvOut[j]["OutMoney"].ToString() : "" + Math.Abs(decimal.Parse(dvOut[j]["OutMoney"].ToString()));

                        string sqlInsert = "insert into W_StockCount(InnerID,MaterialID,MaterialName,MaterialType,Spec,Unit,Num,Price,Money,BatchNumber,Taxes) values ('" + Guid.NewGuid().ToString() + "','" + dvDateIn[i]["GoodsNo"].ToString() + "','" + dvDateIn[i]["GoodsName"].ToString() + "','" + dvDateIn[i]["GoodsType"].ToString() + "','" + dvDateIn[i]["Spec"].ToString() + "','" + dvDateIn[i]["UnitName"].ToString() + "','" + Num + "',0,'" + Money + "','" + dvOut[j]["BatchNumber"] + "','" + Taxes + "')";
                        DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, sqlInsert);

                    }
                }
                for (int j = 0; j < dvMaching.Count; j++)
                {
                    //string strSql = "Select * from W_StockCount Where MaterialID='" + dvDateIn[i]["GoodsNo"] + "' and BatchNumber='" + dvMaching[j]["BatchNumber"] + "'";
                    string strSql = "Select * from W_StockCount Where MaterialID='" + dvDateIn[i]["GoodsNo"] + "'";
                    DataView dv = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, strSql).Tables[0].DefaultView;

                    if (dv.Count > 0)
                    {
                        string Taxes = decimal.Parse(dvMaching[j]["Taxes"].ToString()) >= 0 ? "-" + dvMaching[j]["Taxes"].ToString() : "+" + Math.Abs(decimal.Parse(dvMaching[j]["Taxes"].ToString()));
                        string WNum = decimal.Parse(dvMaching[j]["WNum"].ToString()) >= 0 ? "-" + dvMaching[j]["WNum"].ToString() : "+" + Math.Abs(decimal.Parse(dvMaching[j]["WNum"].ToString()));
                        string WMoney = decimal.Parse(dvMaching[j]["WMoney"].ToString()) >= 0 ? "-" + dvMaching[j]["WMoney"].ToString() : "+" + Math.Abs(decimal.Parse(dvMaching[j]["WMoney"].ToString()));
                        if (decimal.Parse(dvMaching[j]["WMoney"].ToString()) >= 0)
                        {
                            strSql = "Update W_StockCount Set  Taxes=Taxes" + Taxes + ",Num=Num" + WNum + ",Money=Money" + WMoney + " Where InnerID='" + dv[0]["InnerID"] + "'";
                        }
                        else
                        {
                            strSql = "Update W_StockCount Set  Taxes=Taxes" + Taxes + ",Num=Num" + WNum + ",Money=Money" + WMoney + " Where InnerID='" + dv[0]["InnerID"] + "'";
                        }
                        DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, strSql);

                    }
                    else
                    {
                        string Taxes = decimal.Parse(dvMaching[j]["Taxes"].ToString()) >= 0 ? "-" + dvMaching[j]["Taxes"].ToString() : "" + Math.Abs(decimal.Parse(dvMaching[j]["Taxes"].ToString()));
                        string WNum = decimal.Parse(dvMaching[j]["WNum"].ToString()) >= 0 ? "-" + dvMaching[j]["WNum"].ToString() : "" + Math.Abs(decimal.Parse(dvMaching[j]["WNum"].ToString()));
                        string WMoney = decimal.Parse(dvMaching[j]["WMoney"].ToString()) >= 0 ? "-" + dvMaching[j]["WMoney"].ToString() : "" + Math.Abs(decimal.Parse(dvMaching[j]["WMoney"].ToString()));

                        string sqlInsert = "insert into W_StockCount(InnerID,MaterialID,MaterialName,MaterialType,Spec,Unit,Num,Price,Money,BatchNumber,Taxes) values ('" + Guid.NewGuid().ToString() + "','" + dvDateIn[i]["GoodsNo"].ToString() + "','" + dvDateIn[i]["GoodsName"].ToString() + "','" + dvDateIn[i]["GoodsType"].ToString() + "','" + dvDateIn[i]["Spec"].ToString() + "','" + dvDateIn[i]["UnitName"].ToString() + "','" + WNum + "',0,'" + WMoney + "','" + dvMaching[j]["BatchNumber"] + "','" + Taxes + "')";
                        DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, sqlInsert);

                    }
                }
            }
            //string updateSql = "Update W_StockCount Set Price = Money / (case when Num = 0 then 1 else Num end),Taxes = Money * 0.17,TaxesPrice = (Money + Money * 0.17) / (case when Num = 0 then 1 else Num end),SumMoney = (Money + Money * 0.17)";
            string updateSql = "Update W_StockCount Set Price = Money / (case when Num = 0 then 1 else Num end),TaxesPrice = (Money + Taxes) / (case when Num = 0 then 1 else Num end),SumMoney = (Money + Taxes)";
            DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, updateSql);
            MessageBox.Show("完成");
        }

        string BillIds = "";
        /// <summary>
        /// 浏览
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook();
                workbook.Open(textBox1.Text.Trim());
                Aspose.Cells.Cells cells = workbook.Worksheets[0].Cells;
                int rowCount = cells.Rows.Count;
                int colCount = cells.Columns.Count;
                DataTable dt = cells.ExportDataTable(0, 0, rowCount, 23, false);
                for (int i = 0; i < 23; i++)
                {
                    dt.Columns[i].ColumnName = dt.Rows[0][i].ToString();
                }
                dt.Rows.Remove(dt.Rows[0]);
                dt.DefaultView.RowFilter = "IOFlag='-1'";
                DataView dv = dt.DefaultView;

                for (int i = 0; i < dv.Count; i++)
                {
                    BillIds += "'" + dv[i]["BillID"] + "',";
                }
                BillIds = BillIds.Trim(',');
                Aspose.Cells.Cells cellsDetail = workbook.Worksheets[1].Cells;
                rowCount = cellsDetail.Rows.Count;
                colCount = cellsDetail.Columns.Count;
                DataTable dt_Detail = cellsDetail.ExportDataTable(0, 0, rowCount, 29, false);
                for (int i = 0; i < 29; i++)
                {
                    dt_Detail.Columns[i].ColumnName = dt_Detail.Rows[0][i].ToString();
                }
                dt_Detail.Rows.Remove(dt_Detail.Rows[0]);
                dt_Detail.DefaultView.RowFilter = "BillID in (" + BillIds + ")";
                groupBox1.Text = dt_Detail.DefaultView.Count.ToString();
                dataGridView1.DataSource = dt_Detail.DefaultView;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnData_Click(object sender, EventArgs e)
        {
            DataView dv_Detail = dataGridView1.DataSource as DataView;
            DataTable dt = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, "Select * from W_InOut_Detail Where W_InOut_Detail.BillID in(" + BillIds + ")").Tables[0];
            DataTable dt_New = dt.Clone();
            DataTable dt3 = dv_Detail.Table.Clone();
            for (int i = 0; i < dv_Detail.Count; i++)
            {
                string MaterialId = dv_Detail[i]["MaterialID"].ToString();
                decimal Num = 0;
                string InnerId = dv_Detail[i]["InnerID"].ToString();
                string BillID = dv_Detail[i]["BillID"].ToString();

                DataRow[] arrRow = dt.Select("MaterialID='" + MaterialId + "' and BillID='" + BillID + "'");
                DataRow[] arrDetailRow = dv_Detail.Table.Select("MaterialID='" + MaterialId + "' and BillID='" + BillID + "'");
                decimal flagNum = 0;
                foreach (DataRow var in arrDetailRow)
                {
                    Num += decimal.Parse(var["Num"].ToString());
                }
                foreach (DataRow var in arrRow)
                {
                    flagNum += decimal.Parse(var["Num"].ToString());
                }
                if (Num != flagNum)
                {
                    foreach (DataRow var in arrRow)
                    {
                        dt_New.ImportRow(var);
                    }
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                    dt3.ImportRow(dv_Detail[i].Row);
                }
            }
            groupBox2.Text = dt_New.Rows.Count.ToString();
            groupBox3.Text = dt3.Rows.Count.ToString();
            dataGridView2.DataSource = dt_New;
            dataGridView3.DataSource = dt3;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModify_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable SourceTable = dataGridView3.DataSource as DataTable;
                DataTable ErrorTable = dataGridView2.DataSource as DataTable;
                foreach (DataRow var in SourceTable.Rows)
                {
                    string billId = var["BillID"].ToString().Trim();
                    string materialId = var["MaterialID"].ToString().Trim();
                    decimal Num = 0;
                    decimal JLNum = 0;
                    DataRow[] arrRow = ErrorTable.Select("MaterialID='" + materialId + "' and BillID='" + billId + "'");
                    DataRow[] arrDetailRow = SourceTable.Select("MaterialID='" + materialId + "' and BillID='" + billId + "'");
                    foreach (DataRow row in arrDetailRow)
                    {
                        Num += decimal.Parse(row["Num"].ToString());
                        JLNum += decimal.Parse(row["JLNum"].ToString());
                    }
                    decimal flagNum = 0;
                    decimal flagJLNum = 0;
                    foreach (DataRow row in arrRow)
                    {
                        flagNum += decimal.Parse(row["Num"].ToString());
                        flagJLNum += decimal.Parse(row["JLNum"].ToString());
                    }
                    decimal YNum = Num - flagNum;
                    decimal YJLNum = JLNum - flagJLNum;
                    if (YNum > 0)
                    {
                        string insertSql = string.Format("Insert Into W_InOut_Detail(InnerID,BillID,SendBillNum,Depositary,MaterialID,OldMaterialID,MaterialName,MaterialTypeID,MaterialType,SupplierName,Unit,Spec,SourceNum,Num,Price,Money,MInType,Type,BGR,Remark,State,ProjectID,ProjectName,JLState,JLNum,BatchNumber,Taxes,TaxesPrice,SumMoney) Values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}')",
                            Guid.NewGuid().ToString(), billId, var["SendBillNum"].ToString(), var["Depositary"].ToString(), materialId, var["OldMaterialID"].ToString(), var["MaterialName"].ToString(), var["MaterialTypeID"].ToString(), var["MaterialType"].ToString(), var["SupplierName"].ToString(), var["Unit"].ToString(), var["Spec"].ToString(), "0.00", YNum, var["Price"].ToString(), decimal.Parse(var["Price"].ToString()) * YNum, var["MInType"].ToString(), var["Type"].ToString(), var["BGR"].ToString(), var["Remark"].ToString(), var["State"].ToString(),
                            var["ProjectID"].ToString(), var["ProjectName"].ToString(), var["JLState"].ToString(), YJLNum, (DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Ticks.ToString()), (decimal.Parse(var["Price"].ToString()) * YNum * decimal.Parse("0.17")), ((decimal.Parse(var["Price"].ToString()) * YNum * decimal.Parse("0.17") + decimal.Parse(var["Price"].ToString()) * YNum) / YNum), ((decimal.Parse(var["Price"].ToString()) * YNum * decimal.Parse("0.17") + decimal.Parse(var["Price"].ToString()) * YNum)));
                        DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, insertSql);
                    }
                }
                MessageBox.Show("完成");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sql = "select * from m_goods order by GoodsNo";
            DataView dvDDate = DBHelper.ExecuteDataset(DBHelper.connStringMain, CommandType.Text, sql).Tables[0].DefaultView;

            for (int i = 0; i < dvDDate.Count; i++)
            {
                string sql1 = "SELECT ISNULL((SUM(SourceNum)-SUM(Num)),0) AS Num FROM W_InOut_Detail  WHERE MaterialID='" + dvDDate[i]["GoodsNo"].ToString() + "' UNION all SELECT ISNULL(SUM(ApplyCount),0) AS Num FROM  V_W_Machining  WHERE MaterialID='" + dvDDate[i]["GoodsNo"].ToString() + "' ";
                DataView dv1 = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sql1).Tables[0].DefaultView;
                if (dv1.Count > 0)
                {
                    decimal num = Convert.ToDecimal(dv1[0]["Num"]) - Convert.ToDecimal(dv1[1]["Num"]);
                    string sql2 = "SELECT ISNULL(SUM(Num),0) as Num FROM dbo.W_WareHouse where MaterialID='" + dvDDate[i]["GoodsNo"].ToString() + "'";
                    DataView dv2 = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sql2).Tables[0].DefaultView;
                    if (dv2.Count > 0)
                    {
                        decimal cnum = Convert.ToDecimal(dv2[0]["Num"]);
                        if (num != cnum)
                        {
                            this.textBox2.Text += dvDDate[i]["GoodsNo"].ToString() + "\r\n";
                        }
                    }
                    
                }
            }
            MessageBox.Show("ok");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string sql = "select distinct BarCode from W_InOut_BarCodeDetail where ShelfBarCode is null and BarCode in (select BarCode from W_InOut_BarCodeDetail where ShelfBarCode is not null)";
            DataView dvDDate = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sql).Tables[0].DefaultView;
            for (int i = 0; i < dvDDate.Count; i++)
            {
                string sql1 = "select ShelfBarCode from W_InOut_BarCodeDetail where BarCode='" + dvDDate[i]["BarCode"] + "' and ShelfBarCode is not null";
                DataView dv1 = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sql1).Tables[0].DefaultView;
                if (dv1.Count > 0)
                {
                    string sql2 = "update W_InOut_BarCodeDetail set ShelfBarCode='" + dv1[0]["ShelfBarCode"] + "' where BarCode='" + dvDDate[i]["BarCode"] + "' and ShelfBarCode is null";
                    DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, sql2);
                }

            }
            MessageBox.Show("ok");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string deleteSql = "delete from W_WareHousetemp where MaterialID='"+this.textBox3.Text.Trim()+"'";
            DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, deleteSql);

            string sql = "select * from m_goods where GoodsNo='" + this.textBox3.Text.Trim() + "' order by GoodsNo";
            DataView dvDDate = DBHelper.ExecuteDataset(DBHelper.connStringMain, CommandType.Text, sql).Tables[0].DefaultView;

            for (int i = 0; i < dvDDate.Count; i++)
            {
                //以往入库
                string sqlIn = "select ifnull(sum(ifnull(SourceNum,0)),0) as SourceNum,ifnull(BatchNumber,CONCAT('016',MaterialID)) as BatchNumber from V_MaterialOutDetail where MaterialID = '" + dvDDate[i]["GoodsNo"] + "'  and IOFlag = 1 group by BatchNumber";
                //string sqlIn = "select isnull(sum(isnull(SourceNum,0)),0) as SourceNum from V_MaterialOutDetail where MaterialID = '" + dvDDate[i]["GoodsNo"] + "'  and IOFlag = 1";
                DataView dvIn = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlIn).Tables[0].DefaultView;

                //以往出库
                string sqlOut = "select ifnull(sum(ifnull(Num,0)),0) as Num,ifnull(BatchNumber,CONCAT('016',MaterialID)) as BatchNumber from V_MaterialOutDetail where MaterialID = '" + dvDDate[i]["GoodsNo"] + "' and IOFlag = -1 group by BatchNumber";
                //string sqlOut = "select isnull(sum(isnull(Num,0)),0) as Num from V_MaterialOutDetail where MaterialID = '" + dvDDate[i]["GoodsNo"] + "' and IOFlag = -1 ";
                DataView dvOut = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlOut).Tables[0].DefaultView;

                //加工出库
                string sqlMaching = "select ifnull(sum(ifnull(ApplyCount,0)),0) as WNum,ifnull(BatchNumber,CONCAT('016',MaterialID)) as BatchNumber from V_W_Machining  where  MaterialID = '" + dvDDate[i]["GoodsNo"] + "' group by BatchNumber";
                //string sqlMaching = "select isnull(sum(isnull(ApplyCount,0)),0) as WNum from V_W_Machining  where  MaterialID = '" + dvDDate[i]["GoodsNo"] + "'";
                DataView dvMaching = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, sqlMaching).Tables[0].DefaultView;

                for (int j = 0; j < dvIn.Count; j++)
                {
                    string sqlInsert = "insert into W_WareHousetemp(InnerID,MaterialID,MaterialName,Spec,Unit,Num,Price,Money,BatchNumber) values ('" + Guid.NewGuid().ToString() + "','" + dvDDate[i]["GoodsNo"].ToString() + "','" + dvDDate[i]["GoodsName"].ToString() + "','" + dvDDate[i]["Spec"].ToString() + "','" + dvDDate[i]["UnitName"].ToString() + "','" + dvIn[j]["SourceNum"].ToString() + "',0,0,'" + dvIn[j]["BatchNumber"].ToString() + "')";
                    //string sqlInsert = "insert into W_WareHouse(InnerID,MaterialID,MaterialName,Spec,Unit,Num,Price,Money) values ('" + Guid.NewGuid().ToString() + "','" + dvDDate[i]["GoodsNo"].ToString() + "','" + dvDDate[i]["GoodsName"].ToString() + "','" + dvDDate[i]["Spec"].ToString() + "','" + dvDDate[i]["UnitName"].ToString() + "','" + dvIn[j]["SourceNum"].ToString() + "',0,0)";
                    DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, sqlInsert);
                }

                for (int j = 0; j < dvOut.Count; j++)
                {
                    string strSql = "Select * from W_WareHousetemp Where MaterialID='" + dvDDate[i]["GoodsNo"] + "' and BatchNumber='" + dvOut[j]["BatchNumber"] + "'";
                    //string strSql = "Select * from W_WareHouse Where MaterialID='" + dvDDate[i]["GoodsNo"] + "' ";
                    DataView dv = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, strSql).Tables[0].DefaultView;
                    if (dv.Count > 0)
                    {
                        if (Convert.ToDecimal(dvOut[j]["Num"].ToString()) < 0)
                            strSql = "Update W_WareHousetemp Set Num=Num+" + Math.Abs(Convert.ToDecimal(dvOut[j]["Num"].ToString())) + " Where InnerID='" + dv[0]["InnerID"] + "'";
                        else
                            strSql = "Update W_WareHousetemp Set Num=Num-" + dvOut[j]["Num"] + " Where InnerID='" + dv[0]["InnerID"] + "'";
                        //strSql = "Update W_WareHouse Set Num=Num-" + dvOut[j]["Num"] + " Where InnerID='" + dv[0]["InnerID"] + "'";
                        DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, strSql);
                    }
                    else
                    {
                        string sqlInsert = "";
                        //if (Convert.ToDecimal(dvOut[j]["Num"].ToString()) < 0)
                        sqlInsert = "insert into W_WareHousetemp(InnerID,MaterialID,MaterialName,Spec,Unit,Num,Price,Money,BatchNumber) values ('" + Guid.NewGuid().ToString() + "','" + dvDDate[i]["GoodsNo"].ToString() + "','" + dvDDate[i]["GoodsName"].ToString() + "','" + dvDDate[i]["Spec"].ToString() + "','" + dvDDate[i]["UnitName"].ToString() + "','" + Convert.ToDecimal(dvOut[j]["Num"].ToString()) * -1 + "',0,0,'" + dvOut[j]["BatchNumber"].ToString() + "')";
                        //sqlInsert = "insert into W_WareHouse(InnerID,MaterialID,MaterialName,Spec,Unit,Num,Price,Money) values ('" + Guid.NewGuid().ToString() + "','" + dvDDate[i]["GoodsNo"].ToString() + "','" + dvDDate[i]["GoodsName"].ToString() + "','" + dvDDate[i]["Spec"].ToString() + "','" + dvDDate[i]["UnitName"].ToString() + "','" + Convert.ToDecimal(dvOut[j]["Num"].ToString()) * -1 + "',0,0)";

                        DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, sqlInsert);
                    }
                }
                for (int j = 0; j < dvMaching.Count; j++)
                {
                    string strSql = "Select * from W_WareHousetemp Where MaterialID='" + dvDDate[i]["GoodsNo"] + "' and BatchNumber='" + dvMaching[j]["BatchNumber"] + "'";
                    //string strSql = "Select * from W_WareHouse Where MaterialID='" + dvDDate[i]["GoodsNo"] + "'";
                    DataView dv = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, strSql).Tables[0].DefaultView;
                    if (dv.Count > 0)
                    {
                        if (Convert.ToDecimal(dvMaching[j]["WNum"].ToString()) < 0)
                            strSql = "Update W_WareHousetemp Set Num=Num+" + Math.Abs(Convert.ToDecimal(dvMaching[j]["WNum"].ToString())) + " Where InnerID='" + dv[0]["InnerID"] + "'";
                        else
                            strSql = "Update W_WareHousetemp Set Num=Num-" + dvMaching[j]["WNum"] + " Where InnerID='" + dv[0]["InnerID"] + "'";

                        DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, strSql);
                    }
                    else
                    {
                        string sqlInsert = "";
                        if (Convert.ToDecimal(dvMaching[j]["WNum"].ToString()) < 0)
                            sqlInsert = "insert into W_WareHousetemp(InnerID,MaterialID,MaterialName,Spec,Unit,Num,Price,Money,BatchNumber) values ('" + Guid.NewGuid().ToString() + "','" + dvDDate[i]["GoodsNo"].ToString() + "','" + dvDDate[i]["GoodsName"].ToString() + "','" + dvDDate[i]["Spec"].ToString() + "','" + dvDDate[i]["UnitName"].ToString() + "','" + Convert.ToDecimal(dvMaching[j]["WNum"].ToString()) * -1 + "',0,0,'" + dvMaching[j]["BatchNumber"].ToString() + "')";
                        //sqlInsert = "insert into W_WareHouse(InnerID,MaterialID,MaterialName,Spec,Unit,Num,Price,Money) values ('" + Guid.NewGuid().ToString() + "','" + dvDDate[i]["GoodsNo"].ToString() + "','" + dvDDate[i]["GoodsName"].ToString() + "','" + dvDDate[i]["Spec"].ToString() + "','" + dvDDate[i]["UnitName"].ToString() + "','" + Convert.ToDecimal(dvMaching[j]["WNum"].ToString()) * -1 + "',0,0)";

                        DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, sqlInsert);
                    }
                }
            }
            MessageBox.Show("完成");
        }
    }
}
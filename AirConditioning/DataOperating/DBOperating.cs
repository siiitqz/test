using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

public class DBOperating
{
    /// <summary>
    /// Select SupplierEntity By SupplierCode
    /// </summary>
    /// <param name="shopCode"></param>
    /// <returns></returns>
    public static DataRow GetSupplierEntity(string supplierCode)
    {
        DataRow row = null;
        string strSql = "Select * from B_Supplier Where SupplierCode='" + supplierCode + "'";
        DataTable dt = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, strSql).Tables[0];
        if (dt != null && dt.Rows.Count > 0)
            row = dt.Rows[0];
        return row;
    }

    /// <summary>
    /// Select ShopProductEntity By ProductCode
    /// </summary>
    /// <param name="shopCode"></param>
    /// <returns></returns>
    public static DataRow GetShopProductEntity(string productCode)
    {
        DataRow row = null;
        string strSql = "Select * from B_Supplier_Product Where ProductCode='" + productCode + "'";
        DataTable dt = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, strSql).Tables[0];
        if (dt != null && dt.Rows.Count > 0)
            row = dt.Rows[0];
        return row;
    }

    /// <summary>
    /// Get UserEntity
    /// </summary>
    /// <param name="userCode"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static DataRow GetUser(string userCode, string password)
    {
        DataRow row = null;
        string strSql = "Select * from B_User Where UserCode='" + userCode + "' and Password='" + password + "'";
        DataTable dt = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, strSql).Tables[0];
        if (dt != null && dt.Rows.Count > 0)
            row = dt.Rows[0];
        return row;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="no"></param>
    /// <param name="updateType"></param>
    /// <returns></returns>
    public static DataRow GetSalesVouchersEntity(string no, string updateType)
    {
        DataRow row = null;
        string strSql = "";
        if (updateType == "现金退款")
            strSql = "Select * from SalesVouchers Where SerialNumber='" + no + "'";
        else
            strSql = "Select * from SalesVouchers Where TransactionNumber='" + no + "'";
        DataTable dt = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, strSql).Tables[0];
        if (dt != null && dt.Rows.Count > 0)
            row = dt.Rows[0];
        return row;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="innerId"></param>
    /// <returns></returns>
    public static int UpdateSalesVouchers(int innerId)
    {
        string strSql = "Update SalesVouchers Set IsBack='1' where InnerId='" + innerId + "'";
        return DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, strSql);
    }

    /// <summary>
    /// Add SalesVouchers(Paid in Cash)
    /// </summary>
    /// <param name="_SerialNumber"></param>
    /// <param name="_ShopCode"></param>
    /// <param name="_SupplierId"></param>
    /// <param name="_Brand"></param>
    /// <param name="_Balance"></param>
    /// <param name="_MainTotalMoney"></param>
    /// <param name="_CreateTime"></param>
    /// <param name="_UserId"></param>
    /// <returns>返回自增主键</returns>
    public static int AddSalesVouchers(string _SerialNumber, string _ShopCode, int _SupplierId, string _Brand, decimal _Balance, decimal _MainTotalMoney, DateTime _CreateTime, string _UserId)
    {
        string strSql = string.Format("Insert Into SalesVouchers(SerialNumber,ShopCode,SupplierId,Brand,Balance,MainTotalMoney,CreateTime,CreateUser,IsBack) Values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','0');Select @@identity",
            _SerialNumber, _ShopCode, _SupplierId, _Brand, _Balance, _MainTotalMoney, _CreateTime, _UserId);
        return int.Parse(DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, strSql).Tables[0].Rows[0][0].ToString());
    }

    /// <summary>
    /// Add SalesVouchers(Paid in bank card)
    /// </summary>
    /// <param name="_SerialNumber"></param>
    /// <param name="_ShopCode"></param>
    /// <param name="_SupplierId"></param>
    /// <param name="_Brand"></param>
    /// <param name="_Balance"></param>
    /// <param name="_MainTotalMoney"></param>
    /// <param name="_CreateTime"></param>
    /// <param name="_UserId"></param>
    /// <param name="_TransactionNumber"></param>
    /// <returns>返回自增主键</returns>
    public static int AddSalesVouchers(string _SerialNumber, string _ShopCode, int _SupplierId, string _Brand, decimal _Balance, decimal _MainTotalMoney, DateTime _CreateTime, string _UserId, string _TransactionNumber)
    {
        string strSql = string.Format("Insert Into SalesVouchers(SerialNumber,ShopCode,SupplierId,Brand,Balance,MainTotalMoney,CreateTime,CreateUser,IsBack,TransactionNumber) Values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','0','{8}');Select @@identity",
            _SerialNumber, _ShopCode, _SupplierId, _Brand, _Balance, _MainTotalMoney, _CreateTime, _UserId, _TransactionNumber);
        return int.Parse(DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, strSql).Tables[0].Rows[0][0].ToString());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_InnerId"></param>
    /// <param name="_CommodityCode"></param>
    /// <param name="_CommodityName"></param>
    /// <param name="_Unit"></param>
    /// <param name="_Count"></param>
    /// <param name="_UnitPrice"></param>
    /// <param name="_Money"></param>
    /// <param name="_Discount"></param>
    /// <param name="_RealMoney"></param>
    /// <param name="_Brand"></param>
    /// <returns></returns>
    public static int AddSalesVouchersDetail(string _InnerId, string _CommodityCode, string _CommodityName, string _Unit, string _Count, string _UnitPrice, string _Money, string _Discount, string _RealMoney, string _Brand)
    {
        string strSql = string.Format("Insert Into SalesVouchers_Detail(InnerId,CommodityCode,CommodityName,Unit,Count,UnitPrice,Money,Discount,RealMoney,Brand) Values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}');",
            _InnerId, _CommodityCode, _CommodityName, _Unit, _Count, _UnitPrice, _Money, _Discount, _RealMoney, _Brand);
        return DBHelper.ExecuteNonQuery(DBHelper.connString, CommandType.Text, strSql);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static string GetSerialNumber()
    {
        try
        {
            string serialName = "";
            string strSql = "Select isnull(Max(SerialNumber),0) as SerialNumber from SalesVouchers";
            DataTable dt = DBHelper.ExecuteDataset(DBHelper.connString, CommandType.Text, strSql).Tables[0];
            int index = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                index = int.Parse(dt.Rows[0][0].ToString()) + 1;
                serialName = GetSerialNumber(index);
            }

            return serialName;
        }
        catch (Exception ex)
        {
            return "";
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private static string GetSerialNumber(int index)
    {
        string serialName = "";
        switch (index.ToString().Length)
        {
            case 1: serialName = "00000" + index; break;
            case 2: serialName = "0000" + index; break;
            case 3: serialName = "000" + index; break;
            case 4: serialName = "00" + index; break;
            case 5: serialName = "0" + index; break;
            default: serialName = index.ToString(); break;
        }
        return serialName;
    }
}


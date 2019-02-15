using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace AirConditioning
{
    public partial class frmAir : Form
    {
        public frmAir()
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
                    MessageBox.Show("请填写COM口", "信息提示");
                    return;
                }
                serialPort1.PortName = txtComName.Text.Trim();
                //if (chkZigbee.Checked)
                //{
                //    serialPort1.BaudRate = 115200;
                //}
                //else { 

                //}
                string buffer = "2A 2F 41 88 00 00 00 00 00 00 82 AF 16 00 00 00 00 00 5C B5 16 00 00 00 00 00 00 00 25 38 00 00 A0 0F 00 00 0C 55 00 00 01 FE 01 82 1C 00 00 00 35 65 23";
                string[] arrBuffer = buffer.Split(' ');
                if (!serialPort1.IsOpen)
                    serialPort1.Open();
                byte[] SendBuffer = new byte[arrBuffer.Length];
                for (int i = 0; i < arrBuffer.Length; i++)
                {
                    SendBuffer[i] = Convert.ToByte(Convert.ToInt32(arrBuffer[i], 16));
                }
                serialPort1.Write(SendBuffer, 0, SendBuffer.Length);
                serialPort1.Close();

                Thread.Sleep(1000);
                GetTemperature();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误信息");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtComName.Text.Trim() == "")
                {
                    MessageBox.Show("请填写COM口", "信息提示");
                    return;
                }
                serialPort1.PortName = txtComName.Text.Trim();
                string buffer = "2A 2F 41 88 00 00 00 00 00 00 82 AF 16 00 00 00 00 00 5C B5 16 00 00 00 00 00 00 00 25 38 00 00 A0 0F 00 00 0C 55 00 00 01 FE 70 00 00 00 00 00 DA 65 23";
                string[] arrBuffer = buffer.Split(' ');
                if (!serialPort1.IsOpen)
                    serialPort1.Open();
                byte[] SendBuffer = new byte[arrBuffer.Length];
                for (int i = 0; i < arrBuffer.Length; i++)
                {
                    SendBuffer[i] = Convert.ToByte(Convert.ToInt32(arrBuffer[i], 16));
                }
                serialPort1.Write(SendBuffer, 0, SendBuffer.Length);
                serialPort1.Close();
                lblStatus.Text = "关"; lblTemperature.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误信息");
            }
        }

        private void GetTemperature()
        {
            try
            {
                //if (!serialPort1.IsOpen)
                //{
                string buffer = "55 00 00 02 FD 02 FF 00 01 00 00 56";
                string[] arrBuffer = buffer.Split(' ');
                serialPort1.PortName = txtComName.Text.Trim();
                if (!serialPort1.IsOpen)
                    serialPort1.Open();
                byte[] SendBuffer = new byte[arrBuffer.Length];
                for (int i = 0; i < arrBuffer.Length; i++)
                {
                    SendBuffer[i] = Convert.ToByte(Convert.ToInt32(arrBuffer[i], 16));
                }
                serialPort1.Write(SendBuffer, 0, SendBuffer.Length);
                Thread.Sleep(500);
                byte first;
                byte[] result;
                first = (byte)serialPort1.ReadByte();
                result = new byte[serialPort1.BytesToRead + 1];
                result[0] = first;
                serialPort1.Read(result, 1, result.Length - 1);
                if (result.Length != 12)
                {
                    serialPort1.Close();
                    serialPort1.Dispose();
                    return;
                }
                string STR_Temperature = Convert.ToString(Convert.ToInt32(result[7]));
                lblTemperature.Text = STR_Temperature;
                string STR_OPENORCLOSE = Convert.ToString(Convert.ToInt32(result[5]), 2);
                while (STR_OPENORCLOSE.Length < 8)
                {
                    STR_OPENORCLOSE = STR_OPENORCLOSE.Insert(0, "0");
                }
                if (STR_OPENORCLOSE.Substring(0, 4) == "0111" || STR_OPENORCLOSE == "00000000")
                { lblStatus.Text = "关"; lblTemperature.Text = "0"; }
                else
                    lblStatus.Text = "开";
                serialPort1.Close();
                serialPort1.Dispose();
                //}
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "错误信息");
                serialPort1.Close();
                serialPort1.Dispose();
            }
        }

        /// <summary>
        /// 空调状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStatus_Click(object sender, EventArgs e)
        {
            if (btnStatus.Text == "开始监控")
            {
                timer1.Enabled = true;
                btnStatus.Text = "停止监控";
            }
            else
            {
                timer1.Enabled = false;
                btnStatus.Text = "开始监控";
            }
            //AirStatus();
        }

        private void AirStatus()
        {
            try
            {
                if (txtComName.Text.Trim() == "")
                {
                    MessageBox.Show("请填写COM口", "信息提示");
                    return;
                }
                serialPort1.PortName = txtComName.Text.Trim();
                //string buffer = "2A 2F 41 88 00 00 00 00 00 00 82 AF 16 00 00 00 00 00 5C B5 16 00 00 00 00 00 00 00 25 38 00 00 A0 0F 00 00 0C 55 00 00 02 FD 02 FF 00 01 00 00 56 65 23";
                string buffer = "55 00 00 02 FD 02 FF 00 01 00 00 56";
                string[] arrBuffer = buffer.Split(' ');
                if (!serialPort1.IsOpen)
                    serialPort1.Open();
                byte[] SendBuffer = new byte[arrBuffer.Length];
                for (int i = 0; i < arrBuffer.Length; i++)
                {
                    SendBuffer[i] = Convert.ToByte(Convert.ToInt32(arrBuffer[i], 16));
                }
                serialPort1.Write(SendBuffer, 0, SendBuffer.Length);
                byte first;
                byte[] result;
                Thread.Sleep(1000);
                first = (byte)serialPort1.ReadByte();
                result = new byte[serialPort1.BytesToRead + 1];
                result[0] = first;
                serialPort1.Read(result, 1, result.Length - 1);
                string STR_OPENORCLOSE = Convert.ToString(Convert.ToInt32(result[5]), 2);
                while (STR_OPENORCLOSE.Length < 8)
                {
                    STR_OPENORCLOSE = STR_OPENORCLOSE.Insert(0, "0");
                }
                if (STR_OPENORCLOSE.Substring(0, 4) == "0111")
                    lblStatus.Text = "关";
                else
                    lblStatus.Text = "开";
                serialPort1.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误信息");
                serialPort1.Close();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (txtComName.Text.Trim() != "")
            {
                GetTemperature();
            }
        }
    }
}
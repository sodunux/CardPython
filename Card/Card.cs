/******************************************************************************
        Filename    :   Card.cs
        Author      :   FMSH
        Version     ：  V0.1
        Data        :   2015/09/16
        Description :   This file contains one class which intent to include all the 
                        functions related to the smartcard.
        History     :   Ypf-2015/09/16 
******************************************************************************/

using CardHeader;
using System;
using System.Runtime.InteropServices;
using System.IO;
using System.IO.Ports;
using System.Collections.Generic;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

public class SmartCardFunction
    {
        SmartCardContext FM12_Reader_Context = new SmartCardContext();
        SmartCard FM12XX_Card;
        /////////////////////////////////////////////////////////////////////////////////////
        public byte[] ProgFileBuf = new byte[1024 * 610];//Hex文件的缓冲区
        public byte[] ReadFileBuf = new byte[1024 * 610];//读取数据的缓冲区
        public int ProgFileLenth;//程序的长度
        public int ProgFileMaxLen = 1024 * 610;
        public int ProgOrReadEE_flag;//写程序还是读数据标志位
        public byte BackGround = 0xFF;//缓冲区背景数据
        public string display;
        public bool EEType, RAMType;//存储器的类型，是程序EE、RAM
        public string StartEEAddr, EndEEAddr, OffsetEEAddr;//编程地址的起始地址、终止地址和偏移地址
        public string Clmode = "A1";//CL的模式，A1模式还是CLA模式
        public byte ReadVerify = 1;//读写是否校检位
        public string Interface = "CL"; //数据交换的接口
        public string FileType = "Hex";//打开文件的类型
        public string[] ListReaders=null;//卡机的列表
        public string CrcSelcet = "ALL_CRC";//CRC校检模式
        public string CtVolt = "3.0";//CT电压值
        public string CST = "HIGH";//时钟停止状态
        public string timeout_CT = "02";//CT超时类型,01-std,02-100etu,03-notimeout
        public string timeout_CL = "09";//CL超时数值
        public string AuthKeys = "A0A1A2A3A4A5";//验证秘钥
        public string AuthBlockAddr="03";//验证区地址
        public string AuthType="Mifare";//00 Mifare,01 SH
        public string KeyMode="KeyA";//60 KeyA,61 KeyB
        /// ///////////////////////////////////////////////////////////////////////////////////
        //public string[] ListReaders;
        // return 0 means ok  return -1 represent failed and return the message
        // GetReaders_bnt_Click
        public int GetReaders()
        {
            try
            {
                ListReaders = FM12_Reader_Context.GetReaders();
                for (int i = 0; i < ListReaders.Length; i++) { 
                    PyPrint(ListReaders[i]);
                }
                return 0;
            }
            catch (Exception ex)
            {
                ListReaders = null;
                PyPrint(ex.Message);
                return -1;
            }
        }
        //ConnectReader_Click
        public int ConnectReader(byte readernum)
        {
           
            try
            {
                IntPtr cardPtr;
                string reader=ListReaders[readernum];
                /*
                for (int i = 0; i < ListReaders.Length; i++) 
                { 
                    reader = ListReaders[i];
                    if (reader.Contains("FMSH Reader")) break;//优先选择带有“FMSH Reader”字样的卡机
                    else reader = ListReaders[0];             //如果不存在的话，就选择第一个卡机   
                }
                */

             
                FM12_Reader_Context.Connect(reader, SmartCardShare.Shared, SmartCardProtocols.T0T1, out cardPtr);
                FM12XX_Card = new SmartCard(cardPtr);
                string Message = "Connect Reader:" + reader + " Succeeded";
                //PyPrint(Message);
                return 0;
            }
            catch (Exception ex)
            {
                string Message = ex.Message;
                PyPrint(Message);
                return -1;


            }
        }
        //Reset17_Click
        public int Reset17()
        {
            string Message;
            try
            {
                int i = FM12XX_Card.SetField(0, out Message);
                Message = "SetField: " + Message;
                PyPrint(Message);
                return 0;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                PyPrint( ex.Message);
                return -1;
            }
        }
        //Field_ON_Click
        public int Field_ON()
        {
            string Message;
            string StrReceived;
            try
            {
                FM12XX_Card.SetField(1, out Message);
                Message = "SetField: " + Message;
                PyPrint(Message);
                FM12XX_Card.Set_FM1715_reg(0x26, 0x02, out StrReceived);
                StrReceived =  "Write 17Reg 0x26(0x02): " + StrReceived;
                PyPrint(StrReceived);
                FM12XX_Card.Set_FM1715_reg(0x3A, 0x04, out StrReceived);
                StrReceived =  "Write 17Reg 0x3A(0x04): " + StrReceived;
                PyPrint(StrReceived);
                FM12XX_Card.Set_FM1715_reg(0x21, 0x06, out StrReceived);
                StrReceived =  "Write 17Reg 0x21(0x06): " + StrReceived;
                PyPrint(StrReceived);
                FM12XX_Card.Set_FM1715_reg(0x1E, 0x41, out StrReceived);
                StrReceived = "Write 17Reg 0x41(0x41): " + StrReceived;
                PyPrint(StrReceived);
                return 0;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                PyPrint(Message);
                return -1;
            }
        }
        //Active_Click
        public int Active17()
        {
            string Message;
            try
            {
                
                FM12XX_Card.Active(out Message);
                PyPrint(Message);
                return 0;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                PyPrint(Message);
                return -1;
            }


        }
        //button_RATS_Click
        public int RATS()
        {
            string Message;
            try
            {
                FM12XX_Card.RATS(out Message);
                Message = "ATS: " + Message;
                PyPrint(Message);
                return 0;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                PyPrint(Message);
                return -1;
            }

        }
        //pps_exchange_CL_btn_Click
        public int PpsExchangeCl(string pps1)
        {
            string Message;
            try
            {
                FM12XX_Card.PPS_CL(pps1, out Message);
                Message = "PPS Response: " + Message;
                PyPrint(Message);
                return 0;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                PyPrint(Message);
                return -1;
            }

        }
        //Halt_Click
        public int Halt()
        {
            string Message;
            try
            {
                FM12XX_Card.HALT(out Message);
                Message = "Halt: " + Message;
                PyPrint(Message);
                return 0;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                PyPrint(Message);
                return -1;
            }


        }
        //WupA_Click
        public int WupA()
        {
            string Message;
            try
            {
                FM12XX_Card.WUPA(out Message);
                Message = "ATQA: " + Message;
                PyPrint(Message);
                return 0;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                PyPrint(Message);
                return -1;

            }
        }
        //MI_REQA_Click
        public int ReqA()
        {
            string Message;
            try
            {
                FM12XX_Card.REQA(out Message);
                Message = "ATQA: " + Message;
                PyPrint(Message);
                return 0;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                PyPrint(Message);
                return -1;
            }
        }
        //MI_AntiColl_Click
        public int AntiColl()
        {
            string Message;
            try
            {
                FM12XX_Card.AntiColl_lv1(out Message);
                Message = "UID: " + Message;
                PyPrint(Message);
                return 0;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                PyPrint(Message);
                return -1;
            }

        }
        //MI_Select_Click
        public int MiSelect()
        {
            string Message;
            try
            {
                FM12XX_Card.Select(out Message);
                Message = "Sak: " + Message;
                PyPrint(Message);
                return 0;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                PyPrint(Message);
                return -1;
            }


        }
        //MI_Deselect_Click!
        public int MiDeselect(string timeout)
        {
            string Message;
            string data = "CA01";
            string crc_cfg = "01";//all  crc
            try
            {
                int i = FM12XX_Card.TransceiveCL(data, crc_cfg, timeout, out Message);
                Message = "Data Received: " + Message;
                PyPrint(Message);
                return 0;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                PyPrint(Message);
                return -1;
            }


        }
        //ReadBlock_Click!
        public int ReadBlock(string block_addr)
        {   string Message;
            string StrReceived;

            try
            {
                int i = FM12XX_Card.ReadBlock(block_addr, out StrReceived);
                Message = "ReadBlock " + block_addr + ": " + StrReceived;
                PyPrint(Message);
                return 0;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                PyPrint(Message);
                return -1;
            }
        }
        //ReadBlocks
        public int ReadBlocks(string block_addr,byte block_numb)
        {
            string Message;
            string StrReceived;
            byte[] block_addr_bytes = strToHexByte(block_addr);
            int tmp=block_addr_bytes[0]+block_numb;
            try
            {
            
            if (tmp > 255)
            {
                Message = "Error: Beyond the max blocks 256";
                PyPrint(Message);
                return -1;
            }
                for (int i = 0; i < block_numb; i++) {
                    
                    block_addr = byteToHexStr(1, block_addr_bytes);
                    FM12XX_Card.ReadBlock(block_addr, out StrReceived);
                    Message = "ReadBlock " + block_addr + ": " + StrReceived;
                    PyPrint(Message); 
                    block_addr_bytes[0]++;             
                }

                return 0;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                PyPrint(Message);
                return -1;
            }
  
        }       
        //WriteBlock_Click!
        public int WriteBlock(string block_addr, string block_data)
        {   string Message;
            if (block_data.Length != 32)
            {
                Message = "WriteBlock Error: The Length of the Data should be 32!";
                PyPrint(Message);
                return -1;
            }
            try
            {
                FM12XX_Card.WriteBlock(block_addr, block_data, out Message);
                Message = "WriteBlock " + block_addr + ": " + Message;
                PyPrint(Message);
                return 0;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                PyPrint(Message);
                return -1;
            }
        }
        //Read17Reg_Click!
        public int Read17Reg(string regAddrStr_17)
        {
            string Message;
            byte regData;
            string regDataStr_17;
            try
            {
                int i = FM12XX_Card.Read_FM1715_reg(regAddrStr_17, out Message, out regData);
                regDataStr_17 = regData.ToString("X2");
                if (i == 0)
                {
                    Message = regDataStr_17;//data
                    ;
                }
                else
                
                    Message = Message + "ER";
                    Message = "Read 17 Reg 0x" + regAddrStr_17 + ": " + Message;
                    PyPrint(Message);
                return 0;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return -1;
            }
        }
        //Write17Reg
        public int Write17Reg(string regAddrStr_17, string regDataStr_17)
        {
            string Message;
            Message = "";
            try
            {
                FM12XX_Card.Set_FM1715_reg(regAddrStr_17, regDataStr_17, out Message);
                Message = "Write FM17 Reg 0x" + regAddrStr_17 + ":\t" + Message;
                PyPrint(Message);
                return 0;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                PyPrint(Message);
                return -1;
            }

        }
        //TypeASel_radioButton_CheckedChanged 
        public int TypeASel()
        {
            string Message;
            FM12XX_Card.TypeAinit(out Message);
            if (Message == "Error")
            {
                Message = "Switch to 14443 TypeA \tFaild :TypeA Set FM1715Reg Faild";
                PyPrint(Message);
                return -1;
            }
            else
            {

                Message = "Switch to 14443 TypeA \tSucceeded";
                PyPrint(Message);
                return 0;

            }

        }
        //TypeBSel_radioButton_CheckedChanged 
        public int TypeBSel()
        {
            string Message;
            FM12XX_Card.TypeBinit(out Message);
            if (Message == "Error")
            {
                Message = "Switch to 14443 TypeB \tFaild :TypeB Set FM1715Reg Faild";
                PyPrint(Message);
                return -1;
            }
            else
            {
                Message = "Switch to 14443 TypeB \tSucceeded";
                Message = "ATQB:" + Message;
                PyPrint(Message);
                return 0;
            }
        }
        //Tx106k_radioButton_CheckedChanged
        public int Tx106k()
        {
            string Message;
            FM12XX_Card.Set_TxBaudRate_106k(out Message);
            switch (Message)
            {
                case "Error_B":
                    Message = "TypeB Change Tx rate  \tFaild: Write 17reg Faild";
                    PyPrint(Message);
                    return -1;
                    //break;
                case "Succeeded_B":
                    Message = "TypeB Change Tx rate  \tSucceeded  Tx rate=106K";
                    PyPrint(Message);
                    return 0;
                    //break;
                case "Error_A":
                    Message = "TypeA Change Tx rate  \tFaild: Write 17reg Faild";
                    PyPrint(Message);
                    return -1;
                    //break;
                default:
                    Message = "TypeA Change Tx rate  \tSucceeded  Tx rate=106K";
                    PyPrint(Message);
                    return 0;
                    //break;
            }

        }
        //Tx212k_radioButton_CheckedChanged
        public int Tx212k()
        {
            string Message;
            FM12XX_Card.Set_TxBaudRate_212k(out Message);
            switch (Message)
            {
                case "Error_B":
                    Message = "TypeB Change Tx rate  \tFaild: Write 17reg Faild";
                    PyPrint(Message);
                    return -1;
                    //break;
                case "Succeeded_B":
                    Message = "TypeB Change Tx rate  \tSucceeded  Tx rate=212K";
                    PyPrint(Message);
                    return 0;
                    //break;
                case "Error_A":
                    Message = "TypeA Change Tx rate  \tFaild: Write 17reg Faild";
                    PyPrint(Message);
                    return -1;
                    //break;
                default:
                    Message = "TypeA Change Tx rate  \tSucceeded  Tx rate=212K";
                    PyPrint(Message);
                    return 0;
                    //break;
            }
        }
        //Tx424k_radioButton_CheckedChanged
        public int Tx424k()
        {
            string Message;
            FM12XX_Card.Set_TxBaudRate_424k(out Message);
            switch (Message)
            {
                case "Error_B":
                    Message = "TypeB Change Tx rate  \tFaild: Write 17reg Faild";
                    PyPrint(Message);
                    return -1;
                    //break;
                case "Succeeded_B":
                    Message = "TypeB Change Tx rate  \tSucceeded  Tx rate=424K";
                    PyPrint(Message);
                    return 0;
                    //break;
                case "Error_A":
                    Message = "TypeA Change Tx rate  \tFaild: Write 17reg Faild";
                    PyPrint(Message);
                    return -1;
                    //break;
                default:
                    Message = "TypeA Change Tx rate  \tSucceeded  Tx rate=424K";
                    PyPrint(Message);
                    return 0;
                    //break;
            }
        }
        //Tx848k_radioButton_CheckedChanged
        public int Tx848k()
        {
            string Message;
            FM12XX_Card.Set_TxBaudRate_848k(out Message);
            switch (Message)
            {
                case "Error_B":
                    Message = "TypeB Change Tx rate  \tFaild: Write 17reg Faild";
                    PyPrint(Message);
                    return -1;
                    //break;
                case "Succeeded_B":

                    Message = "TypeB Change Tx rate  \tSucceeded  Tx rate=848K";
                    PyPrint(Message);
                    return 0;
                    //break;
                case "Error_A":
                    Message = "TypeA Change Tx rate  \tFaild: Write 17reg Faild";
                    PyPrint(Message);
                    return -1;
                    //break;
                default:
                    Message = "TypeA Change Tx rate  \tSucceeded  Tx rate=848K";
                    PyPrint(Message);
                    return 0;
                    //break;
            }
        }
        //Tx106k_radioButton_CheckedChanged
        public int Rx106k()
        {
            string Message;
            FM12XX_Card.Set_RxBaudRate_106k(out Message);
            switch (Message)
            {
                case "Error_B":
                    Message = "TypeB Change Rx rate  \tFaild: Write 17reg Faild";
                    PyPrint(Message);
                    return -1;
                    //break;
                case "Succeeded_B":
                    Message = "TypeB Change Rx rate  \tSucceeded  Rx rate=106K";
                    PyPrint(Message);
                    return 0;
                    //break;
                case "Error_A":
                    Message = "TypeA Change Rx rate  \tFaild: Write 17reg Faild";
                    PyPrint(Message);
                    return -1;
                    //break;
                default:
                    Message = "TypeA Change Rx rate  \tSucceeded  Rx rate=106K";
                    PyPrint(Message);
                    return 0;
                    //break;
            }

        }
        //Tx212k_radioButton_CheckedChanged
        public int Rx212k()
        {
            string Message;
            FM12XX_Card.Set_RxBaudRate_212k(out Message);
            switch (Message)
            {
                case "Error_B":
                    Message = "TypeB Change Rx rate  \tFaild: Write 17reg Faild";
                    PyPrint(Message);
                    return -1;
                    //break;
                case "Succeeded_B":
                    Message = "TypeB Change Rx rate  \tSucceeded  Rx rate=212K";
                    PyPrint(Message);
                    return 0;
                    //break;
                case "Error_A":
                    Message = "TypeA Change Rx rate  \tFaild: Write 17reg Faild";
                    PyPrint(Message);
                    return -1;
                    //break;
                default:
                    Message = "TypeA Change Rx rate  \tSucceeded  Rx rate=212K";
                    PyPrint(Message);
                    return 0;
                   //break;
            }
        }
        //Tx424k_radioButton_CheckedChanged
        public int Rx424k()
        {
            string Message;
            FM12XX_Card.Set_RxBaudRate_424k(out Message);
            switch (Message)
            {
                case "Error_B":
                    Message = "TypeB Change Rx rate  \tFaild: Write 17reg Faild";
                    PyPrint(Message);
                    return -1;
                    //break;
                case "Succeeded_B":
                    Message = "TypeB Change Rx rate  \tSucceeded  Rx rate=424K";
                    PyPrint(Message);
                    return 0;
                    //break;
                case "Error_A":
                    Message = "TypeA Change Rx rate  \tFaild: Write 17reg Faild";
                    PyPrint(Message);
                    return -1;
                    //break;
                default:
                    Message = "TypeA Change Rx rate  \tSucceeded  Rx rate=424K";
                    PyPrint(Message);
                    return 0;
                    //break;
            }
        }
        //Tx848k_radioButton_CheckedChanged
        public int Rx848k()
        {
            string Message;
            FM12XX_Card.Set_RxBaudRate_848k(out Message);
            switch (Message)
            {
                case "Error_B":
                    Message = "TypeB Change Rx rate  \tFaild: Write 17reg Faild";
                    PyPrint(Message);
                    return -1;
                    //break;
                case "Succeeded_B":
                    Message = "TypeB Change Rx rate  \tSucceeded  Rx rate=848K";
                    PyPrint(Message);
                    return 0;
                    //break;
                case "Error_A":
                    Message = "TypeA Change Rx rate  \tFaild: Write 17reg Faild";
                    PyPrint(Message);
                    return -1;
                    //break;
                default:
                    Message = "TypeA Change Rx rate  \tSucceeded  Rx rate=848K";
                    PyPrint(Message);
                    return 0;
                    //break;
            }
        }
        //TransceiveCL_Click APDUmode
        public string TxApduCl(string APDU_data)
        {
            string Message;
            try
            {
                int i = FM12XX_Card.SendAPDUCL(APDU_data, out Message);
                Message = "Data Received: " + Message;
                //PyPrint(Message);
                return Message;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                //PyPrint(Message);
                return Message;
            }

        }
        //TransceiveCL_Click direct mode
        public string TxDirectCl(string data)
        {
            string Message;
            string crc_cfg="01";
            if (CrcSelcet == "ALL_CRC") crc_cfg = "01";
            if (CrcSelcet == "RX_CRC") crc_cfg = "02";
            if (CrcSelcet == "TX_CRC") crc_cfg = "03";
            if (CrcSelcet == "NO_CRC") crc_cfg = "00";
            //crc_cfg=01 all crc cfg
            //crc_cfg=00 no crc cfg
            //crc_cfg=02 rx crc cfg
            //crc_cfg=03 tx crx cfg
            Message = "";
            try
            {
                int i = FM12XX_Card.TransceiveCL(data, crc_cfg, timeout_CL, out Message);
                Message = "Data Received:" + Message;
                PyPrint(Message);
                return Message;
            }
            catch (Exception ex)
            {
                PyPrint(Message);
                Message = ex.Message;
                return Message;

            }
        }
        //Init_TDA8007_Click
        public int InitTda8007()
        {
            string Message;
            try
            {
                FM12XX_Card.Set_TDA8007_reg(TDA8007Reg.GTR, 0x00, out Message);
                //Message = "Set EGT:00";
                //PyPrint(Message);
                FM12XX_Card.Init_TDA8007(out Message);
                Message = "Init TDA8007: " + Message;
                PyPrint(Message);
                return 0;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                PyPrint(Message);
                return -1;
            }
        }
        //Cold_Reset_Click
        public int ColdReset()
        {
            string Message;
            string volt;
            if (CtVolt == "5.0") volt = "01";
            else if (CtVolt == "3.0") volt = "02";
            else volt = "03";
            // volt=01  5.0V
            // volt=02  3.0V
            // volt=03  1.8V
            try
            {
                int i = FM12XX_Card.Cold_Reset(volt, out Message);
                Message = "ColdReset ATR: " + Message;
                PyPrint(Message);
                return 0;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                PyPrint(Message);
                return -1;
            }
        }
        //PPS_exchange_ct_btn_Click
        public int PpsExchangeCt(string pps1)
        {
            string Message;
            try
            {
                FM12XX_Card.PPS_CT(pps1, out Message);
                Message = "PPS Response: " + Message;
                PyPrint(Message);
                return 0;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                PyPrint(Message);
                return -1;
            }


        }
        //Warm_Reset_Click
        public int WarmReset()
        {
            string Message;
            // volt=01  5.0V
            // volt=02  3.0V
            // volt=03  1.8V
            string volt;
            if (CtVolt == "5.0") volt = "01";
            else if (CtVolt == "3.0") volt = "02";
            else volt = "03";
            try
            {
                int i = FM12XX_Card.Warm_Reset(volt, out Message);
                Message = "WarmReset ATR: " + Message;
                PyPrint(Message);
                return 0;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                PyPrint(Message);
                return -1;

            }


        }
        //Clock_Stop_Btn_Click
        public int ClockStop()
        {
            string Message;
            byte data = 0;
            try
            {
                FM12XX_Card.Read_TDA8007_reg(TDA8007Reg_Str.CCR, out Message, out data);
                if (CST == "HIGH")
                    data |= 0x20;
                else
                    data |= 0x10;
                FM12XX_Card.Set_TDA8007_reg(TDA8007Reg_Str.CCR, data.ToString("X2"), out Message);
                Message = "Clock:Stopped " + Message;
                PyPrint(Message);
                return 0;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                PyPrint(Message);
                return -1;
            }


        }
        //Clock_Resume_Btn_Click
        public int ClockResume()
        {
            string Message;
            byte data = 0;
            try
            {
                FM12XX_Card.Read_TDA8007_reg(TDA8007Reg_Str.CCR, out Message, out data);
                if (CST == "HIGH")
                    data |= 0x20;
                else
                    data |= 0x10;
                data &= 0xEF;
                FM12XX_Card.Set_TDA8007_reg(TDA8007Reg_Str.CCR, data.ToString("X2"), out Message);
                Message = "Clock:Resumed " + Message;
                PyPrint(Message);
                return 0;

            }
            catch (Exception ex)
            {
                Message = ex.Message;
                PyPrint(Message);
                return -1;
            }


        }
        //TransceiveCT_Click
        public string TxApduCt(string APDU_data)
        {
            string Message;
            try
            {
                int i = FM12XX_Card.SendAPDUCT(APDU_data, out Message);
                //Message = "Data Received: " + Message;
                //PyPrint(Message);

                return Message;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                //PyPrint(Message);
                return Message;
            }
        }
        //TransceiveCT_Click
        public string TxDirectCt(string data)
        {
            string Message;
            //timeout_CT ="01" Std 
            //timeout_CT ="02" 100etu
            //timeout_CT ="03" no timeout
            try
            {
                int i = FM12XX_Card.TransceiveCT(data, timeout_CT, out Message);
                //Message = "Data Received: " + Message;
                //PyPrint(TMessage);
                return Message;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                //PyPrint(Message);
                return Message;
            }
        }
        //string transfer to HexByte
        public static byte[] strToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString = "0" + hexString;  //如果最后不足两位，最后添“0”。
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
        public static string byteToHexStr(int len, byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < len; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
        // OpenFile_Button_Click the filename path is important hex文件解析为bin文件
        public int OpenFile(string filename )
        {
            string Message;
            int hex_addr_change = 0;//04和02的偏移地址
            string[] RawCode;
            string TempStr;
            string pth = filename;
            ProgFileLenth = 0;
            Message = null;
            try
            {
                if (FileType == "Hex")
                {
                    RawCode = File.ReadAllLines(pth);
                    //Initial the FileBuffer 
                    //PyPrintInt(RawCode.Length);//读取文件成功了
                    //PyPrintInt(ProgFileBuf.Length);
                    for (int i = 0; i < ProgFileBuf.Length; i++) { 
                        ProgFileBuf[i] = BackGround;
                        //PyPrintByte(ProgFileBuf[i]);
                    }
                    
                    //解析Hex文件
                   // PyPrintInt(RawCode.Length);
                    
                    for (int i = 0; i < RawCode.Length; i++)
                    {
                        TempStr = RawCode[i];
                        //PyPrintStr(TempStr);
                        if (TempStr == "") { continue; }
                        if (TempStr.Substring(0, 1) == ":")
                        {
                            if (TempStr.Substring(1, 8) == "00000001") { break; }//结束指令
                            if (TempStr.Substring(7, 2) == "02")//段偏移扩展
                            {
                                hex_addr_change = Convert.ToUInt16(TempStr.Substring(9, 4), 16) << 0x4;
                                PyPrint(hex_addr_change);
                                continue;
                            }
                            if (TempStr.Substring(7, 2) == "04")//线性偏移扩展
                            { //349
                                //PyPrintInt(349);
                                if (TempStr.Substring(11, 2) == "80")
                                {
                                    Message = "The RAM section has been removed";
                                    PyPrint(Message);
                                    break;
                                }
                            }
                            else if (TempStr.Substring(7, 2) == "05") {  }//start linear address
                            else if (TempStr.Substring(7, 2) == "03") { }//start segment address
                            else
                            {
                                //PyPrintInt(349);                                       //“00”的情况
                                string datacountStr = TempStr.Substring(1, 2);//记录该行的字节个数
                                string addrStr = TempStr.Substring(3, 4);//记录该行的地址
                                string szHex = TempStr.Substring(9, TempStr.Length - 11);
                                byte[] bytedata = strToHexByte(szHex);
                                int addr = hex_addr_change + (int)(strToHexByte(addrStr)[1]) + (int)(strToHexByte(addrStr)[0] * 256);
                                //PyPrintInt(addr);
                                //PyPrintInt(23);
                                if (addr >= ProgFileLenth)
                                {
                                    ProgFileLenth = addr + strToHexByte(datacountStr)[0];
                                    //PyPrintInt(ProgFileLenth);
                                }
                                for (int j = 0; j < strToHexByte(datacountStr)[0]; j++)
                                {
                                    ProgFileBuf[addr + j] = bytedata[j];
                                }
                            }
                        }
                        ProgFileLenth = ((ProgFileLenth + 3) / 4) * 4;
                    }
                    Message = "OPen Hex file succeeded!";
                    PyPrint(Message);
                    //PyPrintInt(ProgFileLenth);                    
                    //Message = byteToHexStr(ProgFileLenth, ProgFileBuf);                    
                    return 0;
                }
                else
                {   
                    for (int i = 0; i < ProgFileBuf.Length; i++) { ProgFileBuf[i] = BackGround; }
                    FileStream s2 = File.OpenRead(filename);
                    ProgFileLenth = (int)s2.Length;
                    s2.Read(ProgFileBuf, 0, ProgFileLenth);
                    s2.Close();
                    ProgFileLenth = ((ProgFileLenth + 3) / 4) * 4;
                    Message = "Open Bin file succeeded!";
                    PyPrint(Message);              
                    Message = byteToHexStr(ProgFileLenth, ProgFileBuf);
                    //PyPrintStr(Message);//打印出来缓冲区数据
                    return 0;
                }            
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return -1;
            }

        }
        public static bool Delay(int delayTime)
        {
            DateTime now = DateTime.Now;
            int s;
            do
            {
                TimeSpan spand = DateTime.Now - now;
                s = spand.Seconds;
            }
            while (s < delayTime);
            return true;
        }
        //将数据写出到a.bin文件
        public int SaveFileBuffer(string filename)
        {
            byte[] TempBuffer=new byte[ProgFileLenth];
            for (int i = 0; i < ProgFileLenth; i++) {
                TempBuffer[i] =ProgFileBuf[i];
            }
            PyPrint(12);
            File.WriteAllBytes(filename,TempBuffer);//此处为输出文件名字
            return 0;
        }
        //读写CL
        public int ProgOrReadEEInCLA349(uint nProgStartBlock, uint nProgEndBlock, uint nProgOffsetAddr)
        {
            uint j, ProgNumi, nProgSum, buf_nAddr, nAddr, nAddr_base;
            string strReceived, sendbuf;

            byte[] buff;
            buff = new byte[0x103];
            int write_length, read_length;
            nProgSum = nProgEndBlock - nProgStartBlock + 1;
            write_length = 0x80; // 写操作长度0x80
            read_length = 0x10; // 读操作长度0x10
            strReceived = "";
            //setup communication channel
            FM12XX_Card.Init_TDA8007(out strReceived);
            Delay(1);
            if (strReceived == "Error")
            {
                display = "Init TDA8007失败 ";
                return 1;
            }
            FM12XX_Card.SetField(0, out display);
            display = "SetField     \t\t\t" + display;
            FM12XX_Card.SetField(1, out display);
            display = "SetField     \t\t\t" + display;
            FM12XX_Card.Active(out strReceived);
            if ((strReceived == "Error") || (strReceived == "NO REQA"))
            {
                display = "CLA选卡失败";
            }
            Delay(1);

            if (EEType == true)        //如果勾选了数据NVM
            {
                nAddr_base = 0xB00000 + nProgOffsetAddr;
            }
            else if (RAMType == true)       //如果勾选了RAM
            {
                nAddr_base = 0x800000 + nProgOffsetAddr;
            }
            else
            {
                nAddr_base = nProgOffsetAddr;       //写入程序NVM
            }

            if ((ProgOrReadEE_flag == 0) && (RAMType == false))
            {
                // 带片擦的编程模式
                FM12XX_Card.TransceiveCL("3383", "01", "09", out strReceived);
                if (strReceived != "00000000000000000000000000000000")
                {
                    display = "编程模式配置出错，返回：" + strReceived;
                    return 1;
                }
                else
                {
                    display = "编程模式配置成功";
                }
            }

            //Programming
            for (ProgNumi = 0; ProgNumi < nProgSum; ProgNumi++)
            {
                if (ProgOrReadEE_flag == 0)     //仅编程，不读出
                {
                    buf_nAddr = (nProgStartBlock + ProgNumi) * (uint)write_length;
                    nAddr = nAddr_base + buf_nAddr;
                    if (nAddr % 0x1000 == 0)
                    {
                        display = "正在编程地址：" + nAddr.ToString("X6");

                        buff[0] = 0x31;
                        buff[1] = (byte)(nAddr >> 20);
                        buff[1] += 0x80;
                        sendbuf = byteToHexStr(2, buff);
                        FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);
                        if (strReceived != "00000000000000000000000000000000")
                        {
                            display = "0x31初始化指令错误";
                            return 1;
                        }
                        buff[0] = 0x32;
                        buff[1] = (byte)(nAddr >> 12);
                        sendbuf = byteToHexStr(2, buff);
                        FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);
                        if (strReceived != "00000000000000000000000000000000")
                        {
                            display = "0x32初始化指令错误";
                            return 1;
                        }
                    }
                    // 一个block——128bytes的编程操作
                    buff[0] = 0xA1;
                    buff[1] = (byte)(nAddr >> 4);     //addr[11:5]
                    sendbuf = byteToHexStr(2, buff);
                    FM12XX_Card.TransceiveCL(sendbuf, "03", "09", out strReceived);
                    if ((strReceived != "0A") || (buff[0] != 0xA1))
                    {//启动编程出错
                        strReceived = "EE启动编程出错" + nAddr.ToString("X6");
                        display = strReceived;
                        return 1;
                    }
                    for (j = 0; j < 0x80; j++)
                    {
                        buff[j] = ProgFileBuf[buf_nAddr + j];
                    }
                    sendbuf = byteToHexStr(write_length, buff);
                    FM12XX_Card.TransceiveCL(sendbuf, "03", "09", out strReceived);
                    if (strReceived != "0A")
                    {//编程出错
                        strReceived = "EE编程出错" + nAddr.ToString("X6");
                        display = strReceived;
                        return 1;
                    }

                }
                else//读取EE操作
                {
                    //读取操作                      
                    nAddr = nAddr_base + (nProgStartBlock + ProgNumi) * (uint)read_length;
                    if (nAddr % 0x1000 == 0)
                    {
                        display = "正在读取地址：" + nAddr.ToString("X6");

                        buff[0] = 0x31;
                        buff[1] = (byte)(nAddr >> 20);
                        buff[1] += 0x80;
                        sendbuf = byteToHexStr(2, buff);
                        FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);
                        if (strReceived != "00000000000000000000000000000000")
                        {
                            display = "0x31初始化指令错误";
                            return 1;
                        }
                        buff[0] = 0x32;
                        buff[1] = (byte)(nAddr >> 12);
                        sendbuf = byteToHexStr(2, buff);
                        FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);
                        if (strReceived != "00000000000000000000000000000000")
                        {
                            display = "0x32初始化指令错误";
                            return 1;
                        }
                    }
                    //单次读操作
                    buff[0] = 0x30;
                    buff[1] = (byte)(nAddr >> 0x04);
                    sendbuf = byteToHexStr(2, buff);
                    FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);

                    if (strReceived == "Error")
                    {
                        display = "读取数据错误：" + nAddr.ToString("X6");
                        return 1;
                    }
                    else
                    {
                        buff = strToHexByte(strReceived);
                    }

                    for (j = 0; j < read_length; j++)//第一个字节开始放入BUFF
                    {
                        ReadFileBuf[ProgNumi * read_length + j] = buff[j];
                    }
                    if (ReadVerify == 1)
                    {
                        for (j = 0; j < read_length; j++)
                        {
                            buf_nAddr = (nProgStartBlock + ProgNumi) * (uint)read_length + j;
                            if (buf_nAddr > (strToHexByte(EndEEAddr)[2] + strToHexByte(EndEEAddr)[1] * 256 + strToHexByte(EndEEAddr)[0] * 65536) + 1)
                            {
                                display = "读取完毕";
                                break;
                            }
                            if (ReadFileBuf[ProgNumi * read_length + j] != ProgFileBuf[buf_nAddr])
                            {
                                display = "读EE校验错误" + "\t" + "错误地址: 0x" + buf_nAddr.ToString("X6");
                                display = "读出内容: 0x" + ReadFileBuf[buf_nAddr].ToString("X2") + "\t" + "期望内容: 0x" + ProgFileBuf[buf_nAddr].ToString("X2");
                                return 1;
                            }
                        }
                    }
                    for (j = 0; j < read_length; j++)//第一个字节开始放入BUFF
                    {
                        buf_nAddr = (nProgStartBlock + ProgNumi) * (uint)read_length + j;
                        if (buf_nAddr > (strToHexByte(EndEEAddr)[2] + strToHexByte(EndEEAddr)[1] * 256 + strToHexByte(EndEEAddr)[0] * 65536) + 1)
                        {
                            break;
                        }
                        ProgFileBuf[buf_nAddr] = ReadFileBuf[ProgNumi * (uint)read_length + j];
                    }
                    //display = "当前写入文件的源地址：" + nAddr.ToString("X6");
                    //DisplayMessageLine(display);

                }
            }
            if (ProgOrReadEE_flag == 1)
            {
                ProgFileLenth = (strToHexByte(EndEEAddr)[2] + strToHexByte(EndEEAddr)[1] * 256 + strToHexByte(EndEEAddr)[0] * 65536) + 1;
                SaveFileBuffer("buffer.bin");//输出到文件
            }
            return 0;
        }
        //读写CT
        public int ProgOrReadEEInCT349(uint nProgStartBlock, uint nProgEndBlock, uint nProgOffsetAddr)
        {
            uint j, ProgNumi, nProgSum, buf_nAddr, nAddr, nAddr_base;
            string strReceived,sendbuf;
            byte[] buff;
            buff = new byte[0x103];
            int write_length, read_length;
            nProgSum = nProgEndBlock - nProgStartBlock + 1;//数据页数
            write_length = 0x80; // 写操作长度0x80
            read_length = 0x10; // 读操作长度0x10
            strReceived = "";

            //setup communication channel
            FM12XX_Card.Init_TDA8007(out strReceived);
            Delay(1);
            if (strReceived == "Error")
            {
                display = "Init TDA8007失败 ";
                return 1;
            }
            FM12XX_Card.SetField(0, out display);
            display = "SetField     \t\t\t" + display;
            FM12XX_Card.SetField(1, out display);
            display = "SetField     \t\t\t" + display;
            FM12XX_Card.Active(out strReceived);
            if ((strReceived == "Error") || (strReceived == "NO REQA"))
            {
                display = "CLA选卡失败";
            }
            Delay(1);

            if (EEType == true)        //如果勾选了数据NVM
            {
                nAddr_base = 0xB00000 + nProgOffsetAddr;
            }
            else if (RAMType == true)       //如果勾选了RAM
            {
                nAddr_base = 0x800000 + nProgOffsetAddr;
            }
            else
            {
                nAddr_base = nProgOffsetAddr;       //写入程序NVM
            }

            if ((ProgOrReadEE_flag == 0) && (RAMType == false))
            {
                // 带片擦的编程模式
                FM12XX_Card.TransceiveCL("3383", "01", "09", out strReceived);
                if (strReceived != "00000000000000000000000000000000")
                {
                    display = "编程模式配置出错，返回：" + strReceived;
                    return 1;
                }
                else
                {
                    display = "编程模式配置成功"; ;
                }
            }

            //Programming
            for (ProgNumi = 0; ProgNumi < nProgSum; ProgNumi++)
            {
                if (ProgOrReadEE_flag == 0)     //仅编程，不读出
                {
                    buf_nAddr = (nProgStartBlock + ProgNumi) * (uint)write_length;
                    nAddr = nAddr_base + buf_nAddr;
                    if (nAddr % 0x1000 == 0)
                    {
                        display = "正在编程地址：" + nAddr.ToString("X6");

                        buff[0] = 0x31;
                        buff[1] = (byte)(nAddr >> 20);
                        buff[1] += 0x80;
                        sendbuf = byteToHexStr(2, buff);
                        FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);
                        if (strReceived != "00000000000000000000000000000000")
                        {
                            display = "0x31初始化指令错误";
                            return 1;
                        }
                        buff[0] = 0x32;
                        buff[1] = (byte)(nAddr >> 12);
                        sendbuf = byteToHexStr(2, buff);
                        FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);
                        if (strReceived != "00000000000000000000000000000000")
                        {
                            display = "0x32初始化指令错误";
                            return 1;
                        }
                    }
                    // 一个block——128bytes的编程操作
                    buff[0] = 0xA1;
                    buff[1] = (byte)(nAddr >> 4);     //addr[11:5]
                    sendbuf = byteToHexStr(2, buff);
                    FM12XX_Card.TransceiveCL(sendbuf, "03", "09", out strReceived);
                    if ((strReceived != "0A") || (buff[0] != 0xA1))
                    {//启动编程出错
                        strReceived = "EE启动编程出错" + nAddr.ToString("X6");
                        display = strReceived;
                        return 1;
                    }
                    for (j = 0; j < 0x80; j++)
                    {
                        buff[j] = ProgFileBuf[buf_nAddr + j];
                    }
                    sendbuf = byteToHexStr(write_length, buff);
                    FM12XX_Card.TransceiveCL(sendbuf, "03", "09", out strReceived);
                    if (strReceived != "0A")
                    {//编程出错
                        strReceived = "EE编程出错" + nAddr.ToString("X6");
                        display = strReceived;
                        return 1;
                    }

                }
                else//读取EE操作
                {
                    //读取操作                      
                    nAddr = nAddr_base + (nProgStartBlock + ProgNumi) * (uint)read_length;
                    if (nAddr % 0x1000 == 0)
                    {
                        display = "正在读取地址：" + nAddr.ToString("X6");

                        buff[0] = 0x31;
                        buff[1] = (byte)(nAddr >> 20);
                        buff[1] += 0x80;
                        sendbuf = byteToHexStr(2, buff);
                        FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);
                        if (strReceived != "00000000000000000000000000000000")
                        {
                            display = "0x31初始化指令错误";
                            return 1;
                        }
                        buff[0] = 0x32;
                        buff[1] = (byte)(nAddr >> 12);
                        sendbuf = byteToHexStr(2, buff);
                        FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);
                        if (strReceived != "00000000000000000000000000000000")
                        {
                            display = "0x32初始化指令错误";
                            return 1;
                        }
                    }
                    //单次读操作
                    buff[0] = 0x30;
                    buff[1] = (byte)(nAddr >> 0x04);
                    sendbuf = byteToHexStr(2, buff);
                    FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);

                    if (strReceived == "Error")
                    {
                        display = "读取数据错误：" + nAddr.ToString("X6");
                        return 1;
                    }
                    else
                    {
                        buff = strToHexByte(strReceived);
                    }

                    for (j = 0; j < read_length; j++)//第一个字节开始放入BUFF
                    {
                        ReadFileBuf[ProgNumi * read_length + j] = buff[j];
                    }
                    if (ReadVerify == 1)
                    {
                        for (j = 0; j < read_length; j++)
                        {
                            buf_nAddr = (nProgStartBlock + ProgNumi) * (uint)read_length + j;
                            if (buf_nAddr > (strToHexByte(EndEEAddr)[2] + strToHexByte(EndEEAddr)[1] * 256 + strToHexByte(EndEEAddr)[0] * 65536) + 1)
                            {
                                display = "读取完毕";
                                break;
                            }
                            if (ReadFileBuf[ProgNumi * read_length + j] != ProgFileBuf[buf_nAddr])
                            {
                                display = "读EE校验错误" + "\t" + "错误地址: 0x" + buf_nAddr.ToString("X6");
                                display = "读出内容: 0x" + ReadFileBuf[buf_nAddr].ToString("X2") + "\t" + "期望内容: 0x" + ProgFileBuf[buf_nAddr].ToString("X2");
                                return 1;
                            }
                        }
                    }
                    for (j = 0; j < read_length; j++)//第一个字节开始放入BUFF
                    {
                        buf_nAddr = (nProgStartBlock + ProgNumi) * (uint)read_length + j;
                        if (buf_nAddr > (strToHexByte(EndEEAddr)[2] + strToHexByte(EndEEAddr)[1] * 256 + strToHexByte(EndEEAddr)[0] * 65536) + 1)
                        {
                            display = "读取完毕";
                            break;
                        }
                        ProgFileBuf[buf_nAddr] = ReadFileBuf[ProgNumi * (uint)read_length + j];
                    }
                }
            }
            if (ProgOrReadEE_flag == 1)
            {
                //ProgFileLenth = (int)(nProgSum)*0x100;
                ProgFileLenth = (strToHexByte(EndEEAddr)[2] + strToHexByte(EndEEAddr)[1] * 256 + strToHexByte(EndEEAddr)[0] * 65536) + 1;
                SaveFileBuffer("buffer.bin"); //输入到文件
            }
            return 0;
        }
        //ProEE_Button_Click 编程下载,只针对EE编程，Flash编程另外编写函数
        public int ProgEE()
        {
            uint nTemp1, nTemp2, nTemp3;
            byte[] endaddr, startaddr, offsetaddr;
            int nRet = 1;
            //string Message;
            uint nProgStartBlock = 0;
            uint nProgEndBlock = 0;
            ProgOrReadEE_flag = 0;//EE编程
            try
            {

                for (int i = 0; i < 6 - StartEEAddr.Length; i++)
                {
                    StartEEAddr = "0" + StartEEAddr;
                }
                for (int i = 0; i < 6 - EndEEAddr.Length; i++)
                {
                    EndEEAddr = "0" + EndEEAddr;
                }
                for (int i = 0; i < 6 - OffsetEEAddr.Length; i++)
                {
                    OffsetEEAddr = "0" + OffsetEEAddr;
                }
                startaddr = new byte[3];
                endaddr = new byte[3];
                offsetaddr = new byte[3];
                startaddr = strToHexByte(StartEEAddr);
                endaddr = strToHexByte(EndEEAddr);
                offsetaddr = strToHexByte(OffsetEEAddr);
                nTemp1 = (uint)(startaddr[2] + startaddr[1] * 256 + startaddr[0] * 65536);
                nTemp2 = (uint)(endaddr[2] + endaddr[1] * 256 + endaddr[0] * 65536);
                nTemp3 = (uint)(offsetaddr[2] + offsetaddr[1] * 256 + offsetaddr[0] * 65536);
                if (Interface == "CL")
                {
                    if (Clmode == "A1")
                    {
                        //A1命令,最小单位128BYTES
                        if (nTemp1 % 128 == 0)
                        {
                            nProgStartBlock = nTemp1 / 128;	//开始扇区
                            //nProgStartBlock *= 8;
                        }
                        else
                        {
                            nTemp1 = nTemp1 - (nTemp1 % 128);
                            nProgStartBlock = nTemp1 / 128;	//开始扇区
                            //nProgStartBlock *= 8;
                        }

                        if (nTemp2 % 128 == 0)
                        {
                            nProgEndBlock = nTemp2 / 128;		//结束扇区
                            //nProgEndBlock *= 8;
                        }
                        else
                        {
                            nTemp2 = nTemp2 - (nTemp2 % 128);
                            nProgEndBlock = nTemp2 / 128;		//结束扇区
                            //nProgEndBlock *= 8;
                        }
                    }
                    else
                    {
                        //cla,最小单位16BYTES
                        if (nTemp1 % 16 == 0)
                        {
                            nProgStartBlock = nTemp1 / 16;	//开始扇区
                        }
                        else
                        {
                            nTemp1 = nTemp1 - (nTemp1 % 16);
                            nProgStartBlock = nTemp1 / 16;	//开始扇区
                        }

                        if (nTemp2 % 16 == 0)
                        {
                            nProgEndBlock = nTemp2 / 16;	//结束扇区
                        }
                        else
                        {
                            nTemp2 = nTemp2 - (nTemp2 % 16);
                            nProgEndBlock = nTemp2 / 16;	//结束扇区
                        }
                    }
                }
                else
                {	//ct,最小单位128YTES
                    if (nTemp1 % 128 == 0)
                    {
                        nProgStartBlock = nTemp1 / 128;	//开始页
                    }
                    else
                    {
                        nTemp1 = nTemp1 - (nTemp1 % 128);
                        nProgStartBlock = nTemp1 / 128;	//开始页
                    }
                    if (nTemp2 % 128 == 0)
                    {
                        nProgEndBlock = nTemp2 / 128;	//结束页
                    }
                    else
                    {
                        nTemp2 = nTemp2 - (nTemp2 % 128);
                        nProgEndBlock = nTemp2 / 128;	//结束页
                    }
                }
                Reset17();
                InitTda8007();
                if (Interface == "CL")
                {	//cla编程接口
                    display = "CL开始编程……";
                    PyPrint(display);
                    nRet = ProgOrReadEEInCLA349(nProgStartBlock, nProgEndBlock, nTemp3);

                }
                else
                {	//ct编程接口
                    display = "CT开始编程……";
                    PyPrint(display);
                    nRet = ProgOrReadEEInCT349(nProgStartBlock, nProgEndBlock, nTemp3);

                }
                if (nRet == 0)
                {
                    display = "编程结束   \t\t\tSucceeded ";
                    PyPrint(display);
                    return 0;
                }
                else
                {
                    display = "编程失败!   ";
                    PyPrint(display);
                    return -1;
                }
            }
            catch (Exception ex)
            {
                
                display = ex.Message;
                return -1;
            }
        }

        //初始化EE缓冲器里面的数据
        public int InitEEdata()
        {
            int i, datalenth;            
            ProgFileLenth = (int)ProgFileMaxLen;
            if (EEType)
            {
                ProgFileLenth = 1024 * 160;
            }
            StartEEAddr = "000000";
            for (i = 0; i < 6 - StartEEAddr.Length; i++)
            {
                StartEEAddr = "0" + StartEEAddr;
            }
            for (i = 0; i < 6 - EndEEAddr.Length; i++)
            {
                EndEEAddr = "0" + EndEEAddr;
            }


            // 初始化缓冲区根据结束地址变化 hj 2015-03-03
            ProgFileLenth = 1 + strToHexByte(EndEEAddr)[2] + strToHexByte(EndEEAddr)[1] * 256 + strToHexByte(EndEEAddr)[0] * 65536;

            datalenth = ProgFileLenth;


            for (i = 0; i < datalenth; i++)
            {
                ProgFileBuf[i] = BackGround;		//缓冲区初始化为0x00
            }
            return 0;

        }
        //读取EE的数据
        public int ReadEEdata()
        {
            string Message;
            uint  nTemp1, nTemp2, nTemp3;
            byte[] endaddr, startaddr, offsetaddr;
            int nRet = 1;
            uint nProgStartBlock = 0;
            uint nProgEndBlock = 0;
            ProgOrReadEE_flag = 1;//EE读取


            /*if (chkWREncrypt.Checked == true)
                btEncryptOpt = 0;
            else
                btEncryptOpt = 1;*/


            for (int i = 0; i < 6 - StartEEAddr.Length; i++)
            {
                StartEEAddr = "0" + StartEEAddr;
            }
            for (int i = 0; i < 6 - EndEEAddr.Length; i++)
            {
                EndEEAddr = "0" + EndEEAddr;
            }
            for (int i = 0; i < 6 - OffsetEEAddr.Length; i++)
            {
                OffsetEEAddr = "0" + OffsetEEAddr;
            }

            startaddr = new byte[3];
            endaddr = new byte[3];
            offsetaddr = new byte[3];
            startaddr = strToHexByte(StartEEAddr);
            endaddr = strToHexByte(EndEEAddr);
            offsetaddr = strToHexByte(OffsetEEAddr);
            nTemp1 = (uint)(startaddr[2] + startaddr[1] * 256 + startaddr[0] * 65536);
            nTemp2 = (uint)(endaddr[2] + endaddr[1] * 256 + endaddr[0] * 65536);
            nTemp3 = (uint)(offsetaddr[2] + offsetaddr[1] * 256 + offsetaddr[0] * 65536);
            //A1Program.Checked = false;

            if (Interface=="CL")
            {
                if (nTemp1 % 16 == 0)
                {
                    nProgStartBlock = nTemp1 / 16;	//开始扇区
                }
                else
                {
                    nTemp1 = nTemp1 - (nTemp1 % 16);
                    nProgStartBlock = nTemp1 / 16;	//开始扇区
                }

                if (nTemp2 % 16 == 0)
                {
                    nProgEndBlock = nTemp2 / 16;	//结束扇区
                }
                else
                {
                    nTemp2 = nTemp2 - (nTemp2 % 16);
                    nProgEndBlock = nTemp2 / 16;	//结束扇区
                }

            }
            else
            {	//ct,最小单位128YTES
                if (nTemp1 % 128 == 0)
                {
                    nProgStartBlock = nTemp1 / 128;	//开始页
                }
                else
                {
                    nTemp1 = nTemp1 - (nTemp1 % 128);
                    nProgStartBlock = nTemp1 / 128;	//开始页
                }

                if (nTemp2 % 128 == 0)
                {
                    nProgEndBlock = nTemp2 / 128;	//结束页
                }
                else
                {
                    nTemp2 = nTemp2 - (nTemp2 % 128);
                    nProgEndBlock = nTemp2 / 128;	//结束页
                }
            }
            try
            {

                Reset17();
                InitTda8007();
                if (Interface=="CL")
                {	//cla编程接口
                    display = "CL读取EE……";


                        nRet = ProgOrReadEEInCLA349(nProgStartBlock, nProgEndBlock, nTemp3);


                }
                else
                {	//ct编程接口
                    display = "CT读取EE……";
                    

                        nRet = ProgOrReadEEInCT349(nProgStartBlock, nProgEndBlock, nTemp3);


                }
                if (nRet == 0)
                {
                    if (ReadVerify==1)
                    {
                        display = "校验正确   \t\t\tSucceeded ";

                    }
                    else
                    {
                        display = "读取结束   \t\t\tSucceeded ";

                    }
                }
                else
                {
                    display = "读取失败!   ";

                }
                return 0;
            }
            catch (Exception ex)
            {
                Message=ex.Message;
                return -1;
            }
        }       
        //加载秘钥
        public int LoadKey() {
            int result;
            string StrReceived;
            result = FM12XX_Card.LOADKEY(AuthKeys, out StrReceived);
            if (result != 0)
            display = result.ToString();
            display = "加载密钥:  \t<-\t" + StrReceived + display;
            PyPrint(display);
            return 0;
        }

        //认证秘钥
        public int Auth()
        {
            string StrReceived;
            int result;
            display = "";
            byte authtype,key;
            if(AuthType=="SH") authtype=0x01;
            else authtype=0x00;
            if(KeyMode=="KeyB") key=0x61;
            else key=0x60;


            result = FM12XX_Card.AUTH(authtype, key, AuthBlockAddr, out StrReceived);
            if (result != 0)
                display = result.ToString();
            display = "认证:  \t<-\t" + StrReceived + display;
            if (authtype == 0) display = "Mifare" + display;
            else display = "SH" + display;
            PyPrint(display);
            return 0;

        }

        //测试用 测试时OK的,能用C#调用python脚本
        public void PyPrint(string temp)
        {
           
            ScriptRuntime pyRunTime = Python.CreateRuntime();
            string path = "PyPrint.py";
            dynamic obj = pyRunTime.UseFile(path);
            obj.PyPrint(temp);
            
        }

        public void PyPrint(byte temp)
        {
            ScriptRuntime pyRunTime = Python.CreateRuntime();
            //string path = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            //string path = path.Substring(8, path.Length - 16) + "PyPrint.py";
            string path = "PyPrint.py";
            dynamic obj = pyRunTime.UseFile(path);
            obj.PyPrint(temp);
            
        }

        public void PyPrint(int temp)
        {

            ScriptRuntime pyRunTime = Python.CreateRuntime();
            //string path = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            //string path = path.Substring(8, path.Length - 16) + "PyPrint.py";
            string path = "PyPrint.py";
            dynamic obj = pyRunTime.UseFile(path);
            obj.PyPrint(temp);
             
        }      

       
        }

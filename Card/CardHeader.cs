
namespace CardHeader
{
    using System;
    using System.Runtime.InteropServices;
    using System.IO;
    using System.IO.Ports;
    using System.Collections.Generic;

    public class SmartCardCmd
    {
        public const string SEL_CT_50V = "FF0100000111";
        public const string SEL_CT_30V = "FF0100000121";
        public const string SEL_CT_18V = "FF0100000131";
        public const string SEL_CL = "FF0100000102";
        public const string TR_CT_HEAD = "FF0900";	    //CLA,INS,P1
        public const string TR_CL_HEAD = "FF09";         //CLA INS
        public const string TR_GETDATA = "FFC00000";     //CLA,INS,P1,P2
        public const string TR_GETDATA_NORM = "00C00000";     //CLA,INS,P1,P2
        public const string MI_FieldON = "FF18010001";	//CLA,INS,P1,P2,P3 后面加一个Byte的00是关场，01是开场
        public const string MI_REQA = "FF18010102";	//CLA,INS,P1,P2,P3
        public const string MI_ANTICOLL = "FF18010206";	//CLA,INS,P1,P2,P3
        public const string MI_SEL = "FF18010301";	//CLA,INS,P1,P2,P3
        public const string MI_RATS = "FF18010401";	//CLA,INS,P1,P2,P3
        public const string MI_RDBLOCK = "FF18010501";	//CLA,INS,P1,P2,P3 
        public const string MI_WRITEBLOCK = "FF18010611";	//CLA,INS,P1,P2,P3 后面加一个byte的地址和16byte的数据
        public const string MI_WRITE128BYTES = "FF18010C81";	//CLA,INS,P1,P2,P3 后面加一个byte的地址和128byte的数据
        public const string MI_HALT = "FF18010701";	//CLA,INS,P1,P2,P3
        public const string MI_BAUDRATE = "FF18010801";	//CLA,INS,P1,P2,P3
        public const string MI_REQB = "FF18010B03";	//CLB,INS,P1,P2,P3 // 由09改为0B 原因是下位机程序处理会和TR_CL_HEAD产生冲突 cnn
        public const string MI_WUPA = "FF18010A02";	//CLA,INS,P1,P2,P3
        public const string MI_IncBlock = "FF18011105";	//CLA,INS,P1,P2,P3  后接一byte地址及4byte增加值
        public const string MI_DecBlock = "FF18011205";	//CLA,INS,P1,P2,P3  后接一byte地址及4byte减少值
        public const string MI_Restore = "FF18011301";	//CLA,INS,P1,P2,P3  后接一byte地址
        public const string MI_Transfer = "FF18011401";	//CLA,INS,P1,P2,P3  后接一byte地址
        public const string MI_PPS_CL = "FFFF000001";	//CLB,INS,P1,P2,P3 后面加一个参数0x11 :106kbps .0x13：212kbps。0x94：424kbps。0x18：848kbps
        public const string MI_PPS_CT = "FFFF000101";	//CLB,INS,P1,P2,P3  0x11\0x13\0x94\0x18\0x95\0x96
        public const string CT_INITTDA = "FF19010001";   //
        public const string CT_COLDRESET = "FF19010101";   //CLA,INS,P1,P2,P3 后面加一个byte的电压配置
        public const string CT_WARMRESET = "FF19010201";   //CLA,INS,P1,P2,P3 后面加一个byte的电压配置

    }

    public class SPIandI2Ccmd
    {
        public const string SPI_SEND = "FF1A00";   //CLA,INS,P1 后面加P2为长度，P3为想要接收的数据，后面接发送的数据   
        public const string I2C_SEND = "FF1B00";   //CLA,INS,P1 后面加P2为长度，P3为想要接收的数据，后面接发送的数据
    }
    //the address of the TDA8007 registers
    public class TDA8007Reg
    {
        public const byte CSR = 0x00;    /*Card Select Register*/
        public const byte CCR = 0x01;    /*Clock Configuration Register*/
        public const byte PDR = 0x02;    /*Programmable Divider Register*/
        public const byte UCR2 = 0x03;    /*UART Configuration Register 2*/
        public const byte GTR = 0x05;    /*Guard Time Register*/
        public const byte UCR1 = 0x06;    /*UART Configuration Register 1*/
        public const byte PCR = 0x07;    /*Power Control Register*/
        public const byte TOC = 0x08;    /*Time-Out Configuration register*/
        public const byte TOR1 = 0x09;    /*Time-Out Register 1*/
        public const byte TOR2 = 0x0A;    /*Time-Out Register 2*/
        public const byte TOR3 = 0x0B;    /*Time-Out Register 3*/
        public const byte MSR = 0x0C;    /*Mixed Status Register*/
        public const byte FCR = 0x0C;    /*FIFO Control Register*/
        public const byte URR = 0x0D;    /*UART Receive Register*/
        public const byte UTR = 0x0D;    /*UART Transmit Register*/
        public const byte USR = 0x0E;    /*UART Status Register*/
        public const byte HSR = 0x0F;   /*Hardware Status Register*/
    }
    public class TDA8007Reg_Str
    {
        public const string CSR = "00";    /*Card Select Register*/
        public const string CCR = "01";    /*Clock Configuration Register*/
        public const string PDR = "02";    /*Programmable Divider Register*/
        public const string UCR2 = "03";    /*UART Configuration Register 2*/
        public const string GTR = "05";    /*Guard Time Register*/
        public const string UCR1 = "06";    /*UART Configuration Register 1*/
        public const string PCR = "07";    /*Power Control Register*/
        public const string TOC = "08";    /*Time-Out Configuration register*/
        public const string TOR1 = "09";    /*Time-Out Register 1*/
        public const string TOR2 = "0A";    /*Time-Out Register 2*/
        public const string TOR3 = "0B";    /*Time-Out Register 3*/
        public const string MSR = "0C";    /*Mixed Status Register*/
        public const string FCR = "0C";    /*FIFO Control Register*/
        public const string URR = "0D";    /*UART Receive Register*/
        public const string UTR = "0D";    /*UART Transmit Register*/
        public const string USR = "0E";    /*UART Status Register*/
        public const string HSR = "0F";   /*Hardware Status Register*/
    }
    public class FM1715Reg
    {
        public const byte Page = 0x00;            //页写寄存器
        public const byte Command = 0x01;            //命令寄存器
        public const byte FIFO = 0x02;            //64字节FIFO缓冲的输入输出寄存器
        public const byte PrimaryStatus = 0x03;            //发射器，接收器及FIFO的状态
        public const byte FIFOLength = 0x04;            //当前FIFO内字节数寄存器
        public const byte SecondaryStatus = 0x05;            //各种状态寄存器2
        public const byte InterruptEn = 0x06;            //中断使能/禁止寄存器
        public const byte InterruptRq = 0x07;            //中断请求标识寄存器
        public const byte Control = 0x09;            //控制寄存器
        public const byte ErrorFlag = 0x0A;            //错误状态寄存器
        public const byte CollPos = 0x0B;            //冲突检测寄存器
        public const byte TimerValue = 0x0C;            //定时器当前值
        public const byte BitFraming = 0x0F;            //位帧调整寄存器
        public const byte TxControl = 0x11;            //发送控制寄存器
        public const byte CwConductance = 0x12;            //选择发射脚TX1和TX2发射天线
        public const byte ModConductance = 0x13;            //定义输出驱动阻抗
        public const byte CoderControl = 0x14;            //定义编码模式和时钟频率
        public const byte ModWidth = 0x15;            //选择调制Pulse的宽度
        public const byte TypeBFraming = 0x17;            //定义ISO14443B帧格式
        public const byte RxControl1 = 0x19;            //定义接收器的SubC,LPF,Gain
        public const byte DecoderControl = 0x1A;            //解码控制寄存器
        public const byte BitPhase = 0x1B;            //定义发送器和接收器之间的时钟相位关系
        public const byte RxThreshold = 0x1C;            //选择位解码器的阈值
        public const byte BPSKDemControl = 0x1D;            //控制BPSK解调
        public const byte RxControl2 = 0x1E;            //解码控制及选择接收源
        public const byte ClockQControl = 0x1F;            //Q通道时钟生成的控制和显示
        public const byte RxWait = 0x21;            //选择发射和接收之间的时间间隔
        public const byte ChannelRedundancy = 0x22;            //RF通道检验模式设置寄存器
        public const byte CRCPresetLSB = 0x23;
        public const byte CRCPresetMSB = 0x24;
        public const byte MFOUTSelect = 0x26;            //mf OUT 选择配置寄存器
        public const byte FIFOLevel = 0x29;            //定义FIFO上下溢出报警的门限
        public const byte TimerClock = 0x2A;            //定时器周期设置寄存器
        public const byte TimerControl = 0x2B;            //定时器控制寄存器
        public const byte TimerReload = 0x2C;            //定时器初值寄存器
        public const byte IRQPinConfig = 0x2D;            //定义中断输出的有效电平
        public const byte AUTHType = 0x31;            //认证算法选择寄存器
        public const byte TestAnaSelect = 0x3A;            //选择模拟测试输出信号
        public const byte TestDigiSelect = 0x3D;            //测试管脚配置寄存器
    }
    //the Address of the FM1715's registers
    public class FM1715Reg_Str
    {
        public const string Page = "00";            //页写寄存器
        public const string Command = "01";            //命令寄存器
        public const string FIFO = "02";            //64字节FIFO缓冲的输入输出寄存器
        public const string PrimaryStatus = "03";            //发射器，接收器及FIFO的状态
        public const string FIFOLength = "04";            //当前FIFO内字节数寄存器
        public const string SecondaryStatus = "05";            //各种状态寄存器2
        public const string InterruptEn = "06";            //中断使能/禁止寄存器
        public const string InterruptRq = "07";            //中断请求标识寄存器
        public const string Control = "09";            //控制寄存器
        public const string ErrorFlag = "0A";            //错误状态寄存器
        public const string CollPos = "0B";            //冲突检测寄存器
        public const string TimerValue = "0C";            //定时器当前值
        public const string BitFraming = "0F";            //位帧调整寄存器
        public const string TxControl = "11";            //发送控制寄存器
        public const string CwConductance = "12";            //选择发射脚TX1和TX2发射天线
        public const string ModConductance = "13";            //定义输出驱动阻抗
        public const string CoderControl = "14";            //定义编码模式和时钟频率
        public const string ModWidth = "15";            //选择调制Pulse的宽度
        public const string TypeBFraming = "17";            //定义ISO14443B帧格式
        public const string RxControl1 = "19";            //定义接收器的SubC,LPF,Gain
        public const string DecoderControl = "1A";            //解码控制寄存器
        public const string BitPhase = "1B";            //定义发送器和接收器之间的时钟相位关系
        public const string RxThreshold = "1C";            //选择位解码器的阈值
        public const string BPSKDemControl = "1D";            //控制BPSK解调
        public const string RxControl2 = "1E";            //解码控制及选择接收源
        public const string ClockQControl = "1F";            //Q通道时钟生成的控制和显示
        public const string RxWait = "21";            //选择发射和接收之间的时间间隔
        public const string ChannelRedundancy = "22";            //RF通道检验模式设置寄存器
        public const string CRCPresetLSB = "23";
        public const string CRCPresetMSB = "24";
        public const string MFOUTSelect = "26";            //mf OUT 选择配置寄存器
        public const string FIFOLevel = "29";            //定义FIFO上下溢出报警的门限
        public const string TimerClock = "2A";            //定时器周期设置寄存器
        public const string TimerControl = "2B";            //定时器控制寄存器
        public const string TimerReload = "2C";            //定时器初值寄存器
        public const string IRQPinConfig = "2D";            //定义中断输出的有效电平
        public const string AUTHType = "31";            //认证算法选择寄存器
        public const string TestAnaSelect = "3A";            //选择模拟测试输出信号
        public const string TestDigiSelect = "3D";            //测试管脚配置寄存器
    }
    //the address of the FM309's register
    public class FM309Reg
    {
        public const UInt32 Cl_swp_Rxram0 = 0xF200;	//非接触接口CL/SWP RAM（低144bytes）
        public const UInt32 Cl_swp_Rxram1 = 0xF220;	//非接触接口CL/SWP RAM（低144bytes）
        public const UInt32 Cl_swp_Rxram2 = 0xF240;	//非接触接口CL/SWP RAM（低144bytes）
        public const UInt32 Cl_swp_Rxram3 = 0xF260;	//非接触接口CL/SWP RAM（低144bytes）
        public const UInt32 Cl_swp_Txram0 = 0xF290;	//非接触接口CL/SWP RAM（高144bytes）
        public const UInt32 Cl_swp_Txram1 = 0xF2B0;	//非接触接口CL/SWP RAM（高144bytes）
        public const UInt32 Cl_swp_Txram2 = 0xF2D0;	//非接触接口CL/SWP RAM（高144bytes）
        public const UInt32 Cl_swp_Txram3 = 0xF2F0;	//非接触接口CL/SWP RAM（高144bytes）
        public const UInt32 swp_ctrl = 0xF5C0;	//SWP控制寄存器
        public const UInt32 swp_tx_trigger = 0xF5C1;	//SWP发送触发寄存器
        public const UInt32 swp_tx_lenth0 = 0xF5C2;	//SWP第0块RAM发送数据长度寄存器
        public const UInt32 swp_tx_lenth1 = 0xF5C3;	//SWP第1块RAM发送数据长度寄存器
        public const UInt32 swp_tx_lenth2 = 0xF5C4;	//SWP第2块RAM发送数据长度寄存器
        public const UInt32 swp_tx_lenth3 = 0xF5C5;	//SWP第3块RAM发送数据长度寄存器
        public const UInt32 swp_raram_st = 0xF5C6;	//SWP接收RAM状态寄存器
        public const UInt32 swp_rx_lenth0 = 0xF5C7;	//SWP第0块RAM接收数据长度寄存器
        public const UInt32 swp_rx_lenth1 = 0xF5C8;	//SWP第1块RAM接收数据长度寄存器
        public const UInt32 swp_rx_lenth2 = 0xF5C9;	//SWP第2块RAM接收数据长度寄存器
        public const UInt32 swp_rx_lenth3 = 0xF5CA;	//SWP第3块RAM接收数据长度寄存器
        public const UInt32 swp_status = 0xF5CB;	//SWP状态寄存器
        public const UInt32 swp_crc_ctrl = 0xF5CC;	//SWP CRC控制寄存器
        public const UInt32 swp_irq = 0xF5CD;	//SWP中断请求寄存器
        public const UInt32 swp_rx_err1 = 0xF5CE;	//SWP接收错误寄存器
        public const UInt32 swp_rx_err2 = 0xF5CF;	//SWP接收错误寄存器
    }
    public class FM309Reg_Str
    {
        public const string Cl_swp_Rxram0 = "00F200";	//非接触接口CL/SWP RAM（低144bytes）
        public const string Cl_swp_Rxram1 = "00F220";	//非接触接口CL/SWP RAM（低144bytes）
        public const string Cl_swp_Rxram2 = "00F240";	//非接触接口CL/SWP RAM（低144bytes）
        public const string Cl_swp_Rxram3 = "00F260";	//非接触接口CL/SWP RAM（低144bytes）
        public const string Cl_swp_Txram0 = "00F290";	//非接触接口CL/SWP RAM（高144bytes）
        public const string Cl_swp_Txram1 = "00F2B0";	//非接触接口CL/SWP RAM（高144bytes）
        public const string Cl_swp_Txram2 = "00F2D0";	//非接触接口CL/SWP RAM（高144bytes）
        public const string Cl_swp_Txram3 = "00F2F0";	//非接触接口CL/SWP RAM（高144bytes）

        public const string swp_ctrl = "00F5C0";	//SWP控制寄存器
        public const string swp_tx_trigger = "00F5C1";	//SWP发送触发寄存器
        public const string swp_tx_lenth0 = "00F5C2";	//SWP第0块RAM发送数据长度寄存器
        public const string swp_tx_lenth1 = "00F5C3";	//SWP第1块RAM发送数据长度寄存器
        public const string swp_tx_lenth2 = "00F5C4";	//SWP第2块RAM发送数据长度寄存器
        public const string swp_tx_lenth3 = "00F5C5";	//SWP第3块RAM发送数据长度寄存器
        public const string swp_raram_st = "00F5C6";	//SWP接收RAM状态寄存器
        public const string swp_rx_lenth0 = "00F5C7";	//SWP第0块RAM接收数据长度寄存器
        public const string swp_rx_lenth1 = "00F5C8";	//SWP第1块RAM接收数据长度寄存器
        public const string swp_rx_lenth2 = "00F5C9";	//SWP第2块RAM接收数据长度寄存器
        public const string swp_rx_lenth3 = "00F5CA";	//SWP第3块RAM接收数据长度寄存器
        public const string swp_status = "00F5CB";	//SWP状态寄存器
        public const string swp_crc_ctrl = "00F5CC";	//SWP CRC控制寄存器
        public const string swp_irq = "00F5CD";	//SWP中断请求寄存器
        public const string swp_rx_err1 = "00F5CE";	//SWP接收错误寄存器
        public const string swp_rx_err2 = "00F5CF";	//SWP接收错误寄存器
    }
    public class FM295Reg_Str
    {
        public const string swp_master_ctrl = "F220";	//swp_ctrl寄存器
        public const string swp_master_timer_ctrl = "F221";	//swp_timer_ctrl寄存器
        public const string swp_master_timer_l = "F222";	//SWP计时器数据低位寄存器
        public const string swp_master_timer_h = "F223";	//swp计时器数据高位
        public const string swp_master_tx_trigger = "F224";	//swp发送触发寄存器
        public const string swp_master_tx_lenth0 = "F225";	//swp第0块RAM发送数据长度寄存器
        public const string swp_master_tx_lenth1 = "F226";	//swp第1块RAM发送数据长度寄存器
        public const string swp_master_tx_lenth2 = "F227";	//swp第2块RAM发送数据长度寄存器
        public const string swp_master_tx_lenth3 = "F228";	//swp第3块RAM发送数据长度寄存器
        public const string swp_master_raram_st = "F229";	//swp接收RAM状态寄存器
        public const string swp_master_rx_lenth0 = "F22A";	//swp第0块RAM接收数据长度寄存器
        public const string swp_master_rx_lenth1 = "F22B";	//swp第1块RAM接收数据长度寄存器
        public const string swp_master_rx_lenth2 = "F22C";	//swp第2块RAM接收数据长度寄存器
        public const string swp_master_rx_lenth3 = "F22D";	//swp第3块RAM接收数据长度寄存器
        public const string swp_master_rx_baud = "F22E";	//swp发送波特率寄存器
        public const string swp_master_crc_ctrl = "F22F";	//swp CRC控制寄存器
        public const string swp_master_irq = "F230";	//swp模块中断请求寄存器
        public const string swp_master_rx_err1 = "F231";	//swp接收错误寄存器
        public const string swp_master_rx_err2 = "F232";	//swp接收错误寄存器
        public const string swp_master_txram_addr = "F233";	//swp发送RAM地址寄存器
        public const string swp_master_txram_data = "F234";	//swp发送RAM数据寄存器
        public const string swp_master_rxram_addr = "F235";	//swp接收RAM地址寄存器
        public const string swp_master_rxram_data = "F236";	//swp接收RAM数据寄存器
        public const string swp_master_swio_sel = "F340";

        public const string swp_master_Rxram0 = "F480";	//SWP RAM（高128bytes）
        public const string swp_master_Rxram1 = "F4A0";	//WP RAM（高128bytes）
        public const string swp_master_Rxram2 = "F4C0";	//SWP RAM（高128bytes）
        public const string swp_master_Rxram3 = "F4E0";	//SWP RAM（高128bytes）
        public const string swp_master_Txram0 = "F400";	//SWP RAM（低128bytes）
        public const string swp_master_Txram1 = "F420";	//SWP RAM（低128bytes）
        public const string swp_master_Txram2 = "F440";	//SWP RAM（低128bytes）
        public const string swp_master_Txram3 = "F460";	//SWP RAM（低128bytes）
    }
    public class FM295Reg
    {
        public const UInt32 swp_master_ctrl = 0xF220;	//swp_ctrl寄存器
        public const UInt32 swp_master_timer_ctrl = 0xF221;	//swp_timer_ctrl寄存器
        public const UInt32 swp_master_timer_l = 0xF222;	//SWP计时器数据低位寄存器
        public const UInt32 swp_master_timer_h = 0xF223;	//swp计时器数据高位
        public const UInt32 swp_master_tx_trigger = 0xF224;	//swp发送触发寄存器
        public const UInt32 swp_master_tx_lenth0 = 0xF225;	//swp第0块RAM发送数据长度寄存器
        public const UInt32 swp_master_tx_lenth1 = 0xF226;	//swp第1块RAM发送数据长度寄存器
        public const UInt32 swp_master_tx_lenth2 = 0xF227;	//swp第2块RAM发送数据长度寄存器
        public const UInt32 swp_master_tx_lenth3 = 0xF228;	//swp第3块RAM发送数据长度寄存器
        public const UInt32 swp_master_raram_st = 0xF229;	//swp接收RAM状态寄存器
        public const UInt32 swp_master_rx_lenth0 = 0xF22A;	//swp第0块RAM接收数据长度寄存器
        public const UInt32 swp_master_rx_lenth1 = 0xF22B;	//swp第1块RAM接收数据长度寄存器
        public const UInt32 swp_master_rx_lenth2 = 0xF22C;	//swp第2块RAM接收数据长度寄存器
        public const UInt32 swp_master_rx_lenth3 = 0xF22D;	//swp第3块RAM接收数据长度寄存器
        public const UInt32 swp_master_rx_baud = 0xF22E;	//swp发送波特率寄存器
        public const UInt32 swp_master_crc_ctrl = 0xF22F;	//swp CRC控制寄存器
        public const UInt32 swp_master_irq = 0xF230;	//swp模块中断请求寄存器
        public const UInt32 swp_master_rx_err1 = 0xF231;	//swp接收错误寄存器
        public const UInt32 swp_master_rx_err2 = 0xF232;	//swp接收错误寄存器
        public const UInt32 swp_master_txram_addr = 0xF233;	//swp发送RAM地址寄存器
        public const UInt32 swp_master_txram_data = 0xF234;	//swp发送RAM数据寄存器
        public const UInt32 swp_master_rxram_addr = 0xF235;	//swp接收RAM地址寄存器
        public const UInt32 swp_master_rxram_data = 0xF236;	//swp接收RAM数据寄存器
        public const UInt32 swp_master_swio_sel = 0xF340;

        public const UInt32 swp_master_Rxram0 = 0xF480;	//SWP RAM（高128bytes）
        public const UInt32 swp_master_Rxram1 = 0xF4A0;	//WP RAM（高128bytes）
        public const UInt32 swp_master_Rxram2 = 0xF4C0;	//SWP RAM（高128bytes）
        public const UInt32 swp_master_Rxram3 = 0xF4E0;	//SWP RAM（高128bytes）
        public const UInt32 swp_master_Txram0 = 0xF400;	//SWP RAM（低128bytes）
        public const UInt32 swp_master_Txram1 = 0xF420;	//SWP RAM（低128bytes）
        public const UInt32 swp_master_Txram2 = 0xF440;	//SWP RAM（低128bytes）
        public const UInt32 swp_master_Txram3 = 0xF460;	//SWP RAM（低128bytes）
    }
    //form CardDisposition.cs: Smart card disposition
    public enum SmartCardDisposition : int
    {
        Leave = 0,
        Reset = 1,
        Unpower = 2,
        Eject = 3
    }
    //form Scope.cs
    public enum SmartCardScope : int
    {
        User = 0,
        Local = 1,
        System = 2
    }
    //form CardState.cs
    [Flags()]
    public enum SmartCardState : int
    {
        Unaware = 0,
        Ignore = 1,
        Changed = 2,
        Unknown = 4,
        Unavailable = 8,
        Empty = 0x10,
        Present = 0x20,
        AtrMatch = 0x40,
        Exclusive = 0x80,
        InUse = 0x100,
        Mute = 0x200,
        Unpowered = 0x400
    }
    //form CardShare.cs
    public enum SmartCardShare : int
    {
        Exclusive = 1,
        Shared = 2,
        Direct = 3
    }
    //form CardProtocol.cs
    [Flags()]
    public enum SmartCardProtocols : int
    {
        Undefined = 0,
        T0 = 1,
        T1 = 2,
        T0T1 = 3,
        Raw = 0x10000
    }
    //from Errors.cs
    public enum SmartCardErrors : uint
    {
        Success = 0,

        InternalError = 0x80100001,
        Cancelled = 0x80100002,
        InvalidHandle = 0x80100003,
        InvalidParameter = 0x80100004,
        InvalidTarget = 0x80100005,
        NoMemory = 0x80100006,
        WaitedTooLong = 0x80100007,
        InsufficientBuffer = 0x80100008,
        UnknownReader = 0x80100009,
        Timeout = 0x8010000A,
        SharingViolation = 0x8010000B,
        NoSmartcard = 0x8010000C,
        UnknownCard = 0x8010000D,
        CantDispose = 0x8010000E,
        ProtocolMismatch = 0x8010000F,
        NotReady = 0x80100010,
        InvalidValue = 0x80100011,
        SystemCancelled = 0x80100012,
        CommError = 0x80100013,
        UnknownError = 0x80100014,
        InvalidAtr = 0x80100015,
        NotTransacted = 0x80100016,
        ReaderUnavailable = 0x80100017,
        Shutdown = 0x80100018,
        PciTooSmall = 0x80100019,
        ReaderUnsupported = 0x8010001A,
        DuplicateReader = 0x8010001B,
        CardUnsupported = 0x8010001C,
        NoService = 0x8010001D,
        ServiceStopped = 0x8010001E,
        Unexpected = 0x8010001F,
        IccInstallation = 0x80100020,
        IccCreateOrder = 0x80100021,
        UnsupportedFeature = 0x80100022,
        DirNotFound = 0x80100023,
        FileNotFound = 0x80100024,
        NoDir = 0x80100025,
        NoFile = 0x80100026,
        NoAccess = 0x80100027,
        WriteTooMany = 0x80100028,
        BadSeek = 0x80100029,
        InvalidChv = 0x8010002A,
        UnknownResMng = 0x8010002B,
        NoSuchCertificate = 0x8010002C,
        CertificateUnavailable = 0x8010002D,
        NoReadersAvailable = 0x8010002E,
        CommDataLost = 0x8010002F,
        NoKeyContainer = 0x80100030,
        ServerTooBusy = 0x80100031,

        UnsupportedCard = 0x80100065,
        UnresponsiveCard = 0x80100066,
        UnpoweredCard = 0x80100067,
        ResetCard = 0x80100068,
        RemovedCard = 0x80100069,
        SecurityViolation = 0x8010006A,
        WrongChv = 0x8010006B,
        ChvBlocked = 0x8010006C,
        Eof = 0x8010006D,
        CancelledByUser = 0x8010006E,
        CardNotAuthenticated = 0x8010006F
    }

    public class PcscException : System.ComponentModel.Win32Exception
    {
        public PcscException(int error)
            : base(error)
        {
        }

        internal PcscException(SmartCardErrors error)
            : this((int)error)
        {
        }
    }
    public class SmartCardSharingException : IOException
    {
        public SmartCardSharingException(PcscException innerException)
            : base("A sharing violation occurred.", innerException)
        {
        }
    }
    //from IORequest.cs
    public struct SmartCardIORequest
    {
        internal SmartCardIORequest(SmartCardProtocols protocol)
        {
            this.protocol = protocol;
            len = (uint)Marshal.SizeOf(typeof(SmartCardIORequest));
        }
        public SmartCardProtocols protocol;
        public uint len;
        public static readonly SmartCardIORequest T0 = new SmartCardIORequest(SmartCardProtocols.T0);
        public static readonly SmartCardIORequest T1 = new SmartCardIORequest(SmartCardProtocols.T1);
        public static readonly SmartCardIORequest Raw = new SmartCardIORequest(SmartCardProtocols.Raw);
    }

    //from flash_isp.cs
    public class flash_isp
    {
        byte[] sendBuf = new byte[256];
        private SerialPort serialPort1 = new SerialPort();
        public flash_isp()
        {

        }

        /*打开串口函数*/
        public virtual int CommOpen(string PortName, Int32 BaudRate, int ComState, out string StrReceived)
        {
            if (ComState == 0)//打开串口
            {

                serialPort1.PortName = PortName;
                serialPort1.BaudRate = BaudRate;
                serialPort1.Parity = Parity.None;
                serialPort1.StopBits = StopBits.One;
                serialPort1.ReceivedBytesThreshold = 8;
                if (serialPort1.IsOpen)
                {
                    StrReceived = "COM Port already open";
                    return 1;
                }
                serialPort1.Open();
                StrReceived = "Open COM:\t" + PortName + " " + BaudRate;
                return 0;
            }
            else//关闭串口
            {
                serialPort1.Close();
                StrReceived = "Close COM:\t" + PortName + " " + BaudRate;
                return 0;
            }
        }


        public void Uart_send(string SendData)//发送数据
        {
            byte[] sendBuffer = new byte[SendData.Length / 2];
            sendBuffer = strToHexByte(SendData);
            this.serialPort1.Write(sendBuffer, 0, sendBuffer.Length);
        }


        public int Uart_Rev(int len, out string StrReceived)//接收数据 
        {
            string revdata_Str = "";
            int cnt = 5000;
            while (cnt > 0)
            {
                if (serialPort1.BytesToRead >= len)
                    break;
                System.Threading.Thread.Sleep(1);
                cnt--;
            }
            if (cnt == 0)
            {
                StrReceived = "Uart receive data timeout";
                return 1;
            }
            for (int i = len; i > 0; i--)
            {
                revdata_Str += serialPort1.ReadByte().ToString("X2");

            }
            StrReceived = revdata_Str;
            return 0;
        }

        public static string DeleteSpaceString(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            hexString = hexString.Replace("\t", "");
            if ((hexString.Length % 2) != 0)
                hexString = "0" + hexString;  //如果最后不足两位，最后添“0”。

            return hexString;
        }

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

        public int uart_bandrate_init()
        {
            Uart_send("78");
            return 0;
        }

        public int flash_isp_start_end()
        {
            Uart_send("A0");
            return 0;
        }

        public int falsh_chip_erase(out string StrReceived)
        {
            string tmp;
            Uart_send("5A");

            Uart_Rev(1, out tmp);
            if (tmp == "55")
            {
                StrReceived = "Chip Erase\t\t\tSucceeded";
                return 0;
            }
            else
            {
                StrReceived = "Chip Erase\t\t\tFailed";
                return 1;
            }
        }
        public int flash_sector_erase(string addr, out string StrReceived)
        {
            string tmp;
            Uart_send("5B" + addr);
            Uart_Rev(1, out tmp);
            if (tmp == "55")
            {
                StrReceived = "Sector Erase\t\t\tSucceeded";
                return 0;
            }
            else
            {
                StrReceived = "Sector Erase\t\t\tFailed";
                return 1;
            }
        }
        public int flash_read(string addr, string len, out string StrReceived)
        {
            int rtn;
            StrReceived = "";
            Uart_send("A6" + addr + len);
            rtn = Uart_Rev(Convert.ToByte(len, 16), out StrReceived);
            return rtn;
        }

        public int flash_write(string addr, string len, string data, out string StrReceived)
        {
            StrReceived = "";
            if (data.Length / 2 != Convert.ToByte(len, 16))
            {
                StrReceived = "Data length error";
                return 1;
            }
            Uart_send("A5" + addr + len + data);
            Uart_Rev(1, out StrReceived);
            if (StrReceived == "55")
            {
                StrReceived = "Flash write Succeeded";
                return 0;
            }
            else
            {
                StrReceived = "Flash write Failed: 0x" + addr;
                return 1;
            }
        }
        public int Uart_Rev_test(out string StrReceived)//接收数据 
        {
            string revdata_Str = "";
            int cnt = 5000;
            while (cnt > 0)
            {
                if (serialPort1.BytesToRead >= 1)
                    break;
                System.Threading.Thread.Sleep(1);
                cnt--;
            }
            if (cnt == 0)
            {
                StrReceived = "Uart receive data timeout";
                return 1;
            }
            for (int i = serialPort1.BytesToRead; i > 0; i--)
            {
                revdata_Str += serialPort1.ReadByte().ToString("X2");

            }
            StrReceived = revdata_Str;
            return 0;
        }

    }
    //from Context.cs
    public class SmartCardContext : MarshalByRefObject, IDisposable
    {
        protected IntPtr context;

        public SmartCardContext()
        {
            int ret = SCardEstablishContext(SmartCardScope.User, IntPtr.Zero, IntPtr.Zero, out context);
            if (ret != 0)
                throw ToException(ret);
        }

        ~SmartCardContext()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (context != IntPtr.Zero)
            {
                try
                {
                    Cancel();
                }
                catch (Exception) { }
                int ret = SCardReleaseContext(context);
                if (ret != 0)
                    throw ToException(ret);
                context = IntPtr.Zero;
            }
        }

        public IntPtr Handle
        {
            get
            {
                return context;
            }
        }

        public string[] GetReaders()
        {
            IntPtr readersPtr = IntPtr.Zero;
            int len = -1;
            int ret;
            try
            {
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    ret = SCardListReaders(context, IntPtr.Zero, IntPtr.Zero, ref len);
                    if (ret != 0)
                        throw ToException(ret);
                    readersPtr = Marshal.AllocHGlobal(len * 1);
                    ret = SCardListReaders(context, IntPtr.Zero, readersPtr, ref len);
                }
                else
                {
                    ret = SCardListReaders(context, IntPtr.Zero, out readersPtr, ref len);
                }
                if (ret != 0)
                    throw ToException(ret);
                List<string> readers = new List<string>();
                long offset = 0;
                string str;
                while ((str = Marshal.PtrToStringAnsi((IntPtr)((long)readersPtr + offset))) != String.Empty)
                {
                    readers.Add(str);
                    offset += (str.Length + 1) * 1;
                }
                return readers.ToArray();
            }
            finally
            {
                if (readersPtr != IntPtr.Zero)
                {
                    if (Environment.OSVersion.Platform == PlatformID.Unix)
                    {
                        Marshal.FreeHGlobal(readersPtr);
                    }
                    else
                    {
                        SCardFreeMemory(context, readersPtr);
                    }
                }
            }
        }

        public void WaitForStatusChange(string reader, SmartCardState currentState, SmartCardState newState)
        {
            WaitForStatusChange(reader, currentState, newState, uint.MaxValue);
        }

        public void WaitForStatusChange(string reader, SmartCardState currentState, SmartCardState newState, long timeout)
        {
            if ((timeout < 0) || (timeout > uint.MaxValue))
                throw new ArgumentOutOfRangeException("timeout must be a 32-bit unsigned integer value");
            ReaderState readerState = new ReaderState();
            readerState.Reader = reader;
            readerState.UserData = IntPtr.Zero;
            readerState.CurrentState = currentState;
            readerState.EventState = newState;
            readerState.AtrLength = 0;
            int ret = SCardGetStatusChange(context, (uint)timeout, ref readerState, 1);
            if (ret != 0)
                throw ToException(ret);
        }

        public void Cancel()
        {
            int ret = SCardCancel(context);
            if (ret != 0)
                throw ToException(ret);
        }

        public SmartCard Connect(string reader, SmartCardShare shareMode, SmartCardProtocols protocol)
        {
            IntPtr card;
            SmartCardProtocols activeProtocol;
            int ret = SCardConnect(context, reader, shareMode, protocol, out card, out activeProtocol);
            if (ret != 0)
                throw ToException(ret);
            //Console.WriteLine("Protocol: {0}", activeProtocol);
            return new SmartCard(card);
        }

        public void Connect(string reader, SmartCardShare shareMode, SmartCardProtocols protocol, out IntPtr card)
        {
            //IntPtr card;
            SmartCardProtocols activeProtocol;
            int ret = SCardConnect(context, reader, shareMode, protocol, out card, out activeProtocol);
            if (ret != 0)
                throw ToException(ret);
            //Console.WriteLine("Protocol: {0}", activeProtocol);
            return;
        }

        internal static Exception ToException(int returnValue)
        {
            PcscException ex = new PcscException(returnValue);
            switch (returnValue)
            {
                case 109:
                    return new NotSupportedException("The local system does not support smart card redirection", ex);
                default:
                    {
                        SmartCardErrors value = (SmartCardErrors)returnValue;
                        switch (value)
                        {
                            case SmartCardErrors.InternalError:
                                return new SystemException("PC/SC internal error", ex);
                            case SmartCardErrors.Cancelled:
                                return new OperationCanceledException("Cancelled", ex);
                            case SmartCardErrors.InvalidHandle:
                                return new ArgumentException("Invalid handle", ex);
                            case SmartCardErrors.InvalidParameter:
                                return new ArgumentException("Invalid parameter", ex);
                            case SmartCardErrors.InvalidTarget:
                                return new SystemException("Invalid target", ex);
                            case SmartCardErrors.NoMemory:
                                return new OutOfMemoryException("No memory", ex);
                            case SmartCardErrors.WaitedTooLong:
                                return new TimeoutException("Waited too long", ex);
                            case SmartCardErrors.InsufficientBuffer:
                                return new InternalBufferOverflowException("Insufficient buffer", ex);
                            case SmartCardErrors.UnknownReader:
                                return new ArgumentException("Unknown reader", ex);
                            case SmartCardErrors.Timeout:
                                return new TimeoutException("Timeout", ex);
                            case SmartCardErrors.SharingViolation:
                                return new SmartCardSharingException(ex);
                            case SmartCardErrors.NoSmartcard:
                                return new Exception("No smart card", ex);
                            case SmartCardErrors.UnknownCard:
                                return new Exception("Unknown card", ex);
                            case SmartCardErrors.CantDispose:
                                return new Exception("Can't dispose", ex);
                            case SmartCardErrors.ProtocolMismatch:
                                return new IOException("Protocol mismatch", ex);
                            case SmartCardErrors.NotReady:
                                return new InvalidOperationException("Not ready", ex);
                            case SmartCardErrors.InvalidValue:
                                return new ArgumentException("Invalid value", ex);
                            case SmartCardErrors.SystemCancelled:
                                return new Exception("System cancelled", ex);
                            case SmartCardErrors.CommError:
                                return new IOException("Comm error", ex);
                            case SmartCardErrors.UnknownError:
                                return new Exception("Unknown error", ex);
                            case SmartCardErrors.InvalidAtr:
                                return new ArgumentException("Invalid ATR", ex);
                            case SmartCardErrors.NotTransacted:
                                return new Exception("Not transacted", ex);
                            case SmartCardErrors.ReaderUnavailable:
                                return new Exception("Reader unavailable", ex);
                            case SmartCardErrors.Shutdown:
                                return new SystemException("Shutdown", ex);
                            case SmartCardErrors.PciTooSmall:
                                return new SystemException("PCI too small", ex);
                            case SmartCardErrors.ReaderUnsupported:
                                return new NotSupportedException("Reader unsupported", ex);
                            case SmartCardErrors.DuplicateReader:
                                return new ArgumentException("Duplicate reader", ex);
                            case SmartCardErrors.CardUnsupported:
                                return new NotSupportedException("Card unsupported", ex);
                            case SmartCardErrors.NoService:
                                return new SystemException("No service", ex);
                            case SmartCardErrors.ServiceStopped:
                                return new SystemException("Service stopped", ex);
                            case SmartCardErrors.Unexpected:
                                return new Exception("Unexpected", ex);
                            case SmartCardErrors.IccInstallation:
                                return new Exception("ICC installation", ex);
                            case SmartCardErrors.IccCreateOrder:
                                return new Exception("ICC create order", ex);
                            case SmartCardErrors.UnsupportedFeature:
                                return new NotSupportedException("Unsupported feature", ex);
                            case SmartCardErrors.DirNotFound:
                                return new DirectoryNotFoundException("Directory not found", ex);
                            case SmartCardErrors.FileNotFound:
                                return new FileNotFoundException("File not found", ex);
                            case SmartCardErrors.NoDir:
                                return new InvalidOperationException("No directory", ex);
                            case SmartCardErrors.NoFile:
                                return new InvalidOperationException("No file", ex);
                            case SmartCardErrors.NoAccess:
                                return new Exception("No access", ex);
                            case SmartCardErrors.WriteTooMany:
                                return new Exception("Write too many", ex);
                            case SmartCardErrors.BadSeek:
                                return new Exception("Bad seek", ex);
                            case SmartCardErrors.InvalidChv:
                                return new Exception("Invalid CHV", ex);
                            case SmartCardErrors.UnknownResMng:
                                return new ArgumentException("Unknown resource manager", ex);
                            case SmartCardErrors.NoSuchCertificate:
                                return new Exception("No such certificate", ex);
                            case SmartCardErrors.CertificateUnavailable:
                                return new Exception("Certificate unavailable", ex);
                            case SmartCardErrors.NoReadersAvailable:
                                return new Exception("No readers available", ex);
                            case SmartCardErrors.CommDataLost:
                                return new IOException("Comm data lost", ex);
                            case SmartCardErrors.NoKeyContainer:
                                return new Exception("No key container", ex);
                            case SmartCardErrors.ServerTooBusy:
                                return new SystemException("Server too busy", ex);

                            case SmartCardErrors.UnsupportedCard:
                                return new NotSupportedException("Unsupported card", ex);
                            case SmartCardErrors.UnresponsiveCard:
                                return new IOException("UnresponsiveCard", ex);
                            case SmartCardErrors.UnpoweredCard:
                                return new IOException("Unpowered card", ex);
                            case SmartCardErrors.ResetCard:
                                return new IOException("Reset card", ex);
                            case SmartCardErrors.RemovedCard:
                                return new IOException("Removed card", ex);
                            case SmartCardErrors.SecurityViolation:
                                return new System.Security.SecurityException("Security violation", ex);
                            case SmartCardErrors.WrongChv:
                                return new Exception("Wrong CHV", ex);
                            case SmartCardErrors.ChvBlocked:
                                return new Exception("CHV blocked", ex);
                            case SmartCardErrors.Eof:
                                return new IOException("EOF", ex);
                            case SmartCardErrors.CancelledByUser:
                                return new Exception("Cancelled by user", ex);
                            case SmartCardErrors.CardNotAuthenticated:
                                return new Exception("Card not authenticated", ex);

                            default:
                                return new Exception("PC/SC error " + value.ToString() + " (" + returnValue.ToString("X") + ")", ex);
                        }
                    }
            }
        }

        [DllImport("Winscard.dll")]
        private static extern int SCardEstablishContext(SmartCardScope scope, IntPtr reserved1, IntPtr reserved2, out IntPtr context);

        [DllImport("Winscard.dll")]
        private static extern int SCardReleaseContext(IntPtr context);

        [DllImport("Winscard.dll", CharSet = CharSet.Ansi)]
        private static extern int SCardListReaders(IntPtr context, IntPtr groups, out IntPtr readers, ref int length);

        [DllImport("Winscard.dll", CharSet = CharSet.Ansi)]
        private static extern int SCardListReaders(IntPtr context, IntPtr groups, IntPtr readers, ref int length);

        [DllImport("Winscard.dll")]
        private static extern int SCardFreeMemory(IntPtr context, IntPtr ptr);

        [DllImport("Winscard.dll", CharSet = CharSet.Ansi)]
        private static extern int SCardConnect(IntPtr context, [MarshalAs(UnmanagedType.LPStr)] string reader, SmartCardShare shareMode, SmartCardProtocols preferredProtocols, out IntPtr card, out SmartCardProtocols activeProtocol);

        [DllImport("Winscard.dll", CharSet = CharSet.Ansi)]
        private static extern int SCardGetStatusChange(IntPtr context, uint timeout, ref ReaderState readerState, uint count);


        private struct ReaderState
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string Reader;
            public IntPtr UserData;
            public SmartCardState CurrentState;
            public SmartCardState EventState;
            public uint AtrLength;
            public byte atr1;
            public byte atr2;
            public byte atr3;
            public byte atr4;
            public byte atr5;
            public byte atr6;
            public byte atr7;
            public byte atr8;
            public byte atr9;
            public byte atr10;
            public byte atr11;
            public byte atr12;
            public byte atr13;
            public byte atr14;
            public byte atr15;
            public byte atr16;
            public byte atr17;
            public byte atr18;
            public byte atr19;
            public byte atr20;
            public byte atr21;
            public byte atr22;
            public byte atr23;
            public byte atr24;
            public byte atr25;
            public byte atr26;
            public byte atr27;
            public byte atr28;
            public byte atr29;
            public byte atr30;
            public byte atr31;
            public byte atr32;
            public byte atr33;
        }

        [DllImport("Winscard.dll")]
        private static extern int SCardCancel(IntPtr context);
    }
    //from Card.cs
    public class SmartCard : MarshalByRefObject, IDisposable
    {
        protected IntPtr card;
        protected SmartCardDisposition dispose_disposition = SmartCardDisposition.Leave;
        int PRTCL;	 			//0表示现在的17寄存器设置为TypeA;1表示TypeB
        string receive = "";
        string send = "";
        int CurrentInterface = 0; //1为CL 2为CT,0为开机状态
        public string display = "";
        byte[] uid = new byte[4];
        //        public string GetLastError = "";
        //编程相关全局变量定义
        //Prog_struct	g_ProgParam;
        uint[] g_EncryptS0 = { 0xC, 0x1, 0xA, 0xF, 0x9, 0x2, 0x6, 0x8, 0x0, 0xD, 0x3, 0x4, 0xE, 0x7, 0x5, 0xB };
        uint[] g_EncryptS1 = { 0x9, 0xE, 0xF, 0x5, 0x2, 0x8, 0xC, 0x3, 0x7, 0x0, 0x4, 0xA, 0x1, 0xD, 0xB, 0x6 };
        uint[] g_EncryptS2 = { 0xe, 0x4, 0xd, 0x1, 0x2, 0xf, 0xb, 0x8, 0x3, 0xa, 0x6, 0xc, 0x5, 0x9, 0x0, 0x7 };
        uint[] g_EncryptS3 = { 0x4, 0x1, 0xE, 0x8, 0xD, 0x6, 0x2, 0xB, 0xF, 0xC, 0x9, 0x7, 0x3, 0xA, 0x5, 0x0 };
        uint[] g_EncryptS4 = { 0xe, 0x3, 0x4, 0x8, 0x1, 0xC, 0xA, 0xF, 0x7, 0xD, 0x9, 0x6, 0xB, 0x2, 0x0, 0x5 };
        uint[] g_EncryptS5 = { 0xF, 0x1, 0x6, 0xC, 0x0, 0xE, 0x5, 0xB, 0x3, 0xA, 0xD, 0x7, 0x9, 0x4, 0x2, 0x8 };



        public SmartCard(IntPtr card)
        {
            this.card = card;
        }

        ~SmartCard()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            try
            {
                int ret = SCardDisconnect(card, dispose_disposition);
                if (ret != 0)
                    throw SmartCardContext.ToException(ret);
            }
            catch (Exception)
            {

            }
            //Console.WriteLine(SmartCardContext.ToException(ret).Message);
        }

        public SmartCardDisposition Disposition
        {
            get
            {
                return dispose_disposition;
            }
            set
            {
                dispose_disposition = value;
            }
        }

        public SmartCardState GetStatus()
        {
            uint readerLen = 0;
            int ret = SCardGetStatus(card, IntPtr.Zero, ref readerLen, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            if (ret != 0)
                throw SmartCardContext.ToException(ret);
            IntPtr readerPtr = Marshal.AllocHGlobal((int)readerLen);
            SmartCardState state;
            SmartCardProtocols protocol;
            uint atrLen = 0;
            ret = SCardGetStatus(card, readerPtr, ref readerLen, out state, out protocol, IntPtr.Zero, out atrLen);
            if (ret != 0)
                throw SmartCardContext.ToException(ret);
            return state;
        }

        public int Transmit(byte[] sendBuffer, byte[] receiveBuffer)
        {
            SmartCardIORequest sendPci = SmartCardIORequest.T1;
            SmartCardIORequest recvPci = SmartCardIORequest.T1;
            uint len = (uint)receiveBuffer.Length;
            IntPtr ptr = Marshal.AllocHGlobal((int)len);
            try
            {
                int ret = SCardTransmit(card, ref sendPci, sendBuffer, (uint)sendBuffer.Length, ref recvPci, ptr, ref len);
                if (ret != 0)
                {
                    //throw SmartCardContext.ToException(ret);
                    receive = "PC/SC communication time out!";   //2014-04-04 尝试不抛出异常
                    //                    Marshal.FreeHGlobal(ptr);
                    return 0;
                }
                //Console.Write("Transmit received {0} bytes: ", len);
                Marshal.Copy(ptr, receiveBuffer, 0, (int)len);
                //Console.WriteLine(BitConverter.ToString(receiveBuffer, 0, (int)len));
                return (int)len;
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public int SendCommand(string send, out string receive)
        {
            SmartCardIORequest sendPci = SmartCardIORequest.T1;
            SmartCardIORequest recvPci = SmartCardIORequest.T1;
            byte[] sendBuffer = new byte[300];
            byte[] receiveBuffer = new byte[300];

            sendBuffer = strToHexByte(send);

            uint len = (uint)receiveBuffer.Length;
            IntPtr ptr = Marshal.AllocHGlobal((int)len);
            try
            {
                int ret = SCardTransmit(card, ref sendPci, sendBuffer, (uint)sendBuffer.Length, ref recvPci, ptr, ref len);
                if (ret != 0)
                {
                    //throw SmartCardContext.ToException(ret);
                    receive = "PC/SC communication time out!";   //2014-04-04 尝试不抛出异常
                    //                    Marshal.FreeHGlobal(ptr);
                    return 0;
                }
                //Console.Write("Transmit received {0} bytes: ", len);
                Marshal.Copy(ptr, receiveBuffer, 0, (int)len);
                //Console.WriteLine(BitConverter.ToString(receiveBuffer, 0, (int)len));
                receive = byteToHexStr((int)len, receiveBuffer);
                return (int)len;
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        [DllImport("Winscard.dll", CharSet = CharSet.Auto)]
        private static extern int SCardDisconnect(IntPtr card, SmartCardDisposition disposition);

        [DllImport("Winscard.dll", CharSet = CharSet.Auto)]
        private static extern int SCardTransmit(IntPtr card, [MarshalAs(UnmanagedType.Struct)] ref SmartCardIORequest sendPci, [MarshalAs(UnmanagedType.LPArray)] byte[] sendBuffer, uint sendLen, [MarshalAs(UnmanagedType.Struct)] ref SmartCardIORequest recvPci, IntPtr recvBuffer, ref uint recvLen);

        [DllImport("Winscard.dll", CharSet = CharSet.Auto)]
        private static extern int SCardGetStatus(IntPtr card, IntPtr readerName, ref uint readerLength, IntPtr state, IntPtr protocol, IntPtr atr, IntPtr atrLength);

        [DllImport("Winscard.dll", CharSet = CharSet.Auto)]
        private static extern int SCardGetStatus(IntPtr card, IntPtr readerName, ref uint readerLength, out SmartCardState state, out SmartCardProtocols protocol, IntPtr atr, out uint atrLength);


        /// <summary>
        /// strToHexByte(string hexString) 将hexstring转化为16进制表示，比如55AA 转化为0x55,0xAA
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] strToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString = "0" + hexString;  //如果最后不足两位，最后添“0”。
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }

        /// <summary>
        /// byteToHexStr(int len, byte[] bytes)将bytes数组中的前面len个元素转化为string
        /// </summary>
        /// <param name="len"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
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

        /// <summary>
        /// AddSpaceString(string str)    在string里加入空格便于显示
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AddSpaceString(string str)
        {
            string returnStr = "";
            for (int i = 0; i < str.Length; i += 2)
                returnStr += str.Substring(i, 2) + " ";
            return returnStr;
        }
        /// <summary>
        /// DeleteSpaceString(string hexString)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DeleteSpaceString(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString = "0" + hexString; //如果最后不足两位，最后添“0”。

            return hexString;
        }




        /// <summary>
        /// REQA(ref string StrReceived)
        /// </summary>
        /// <param name="StrReceived"></param>
        /// <returns></returns>
        public virtual int REQA(out string StrReceived)
        {
            receive = "";
            send = SmartCardCmd.MI_REQA;
            SendCommand(send, out receive);
            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";// +receive;
                return 1;
            }
            else
            {
                StrReceived = receive.Substring(0, 4);
                return 0;
            }

        }

        public virtual int Active(out string StrReceived)
        {
            string tmpStr = "";
            display = "";
            int rlt;
            Set_FM1715_reg(0x26, 0x02, out display);
            Set_FM1715_reg(0x3A, 0x05, out display);

            rlt = REQA(out display);
            if (rlt == 1)
            {
                StrReceived = "NO REQA";
                return 1;
            }
            tmpStr += "ATQA：" + display + "\t";
            AntiColl_lv1(out display);
            tmpStr += "UID：" + display + "\t";
            Select(out display);
            tmpStr += "Sak：" + display;
            StrReceived = tmpStr;
            return 0;
        }

        public virtual int WUPA(out string StrReceived)
        {
            receive = "";
            send = SmartCardCmd.MI_WUPA;
            SendCommand(send, out receive);
            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";// +receive;
                return 1;
            }
            else
            {
                StrReceived = receive.Substring(0, 4);
                return 0;
            }

        }

        public virtual int InitVal(string BlockAddr, string BlockData, out string StrReceived)
        {
            byte[] Data = new byte[16];
            UInt32 Value;
            uint i;

            Value = Convert.ToUInt32(BlockData);
            for (i = 0; i < 4; i++)
            {
                Data[i] = (byte)Value;
                Data[4 + i] = (byte)~Data[i];
                Data[8 + i] = Data[i];
                Value >>= 8;
            }
            Data[12] = Data[14] = (byte)Convert.ToSByte(BlockAddr, 16);
            Data[13] = Data[15] = (byte)~Data[12];

            send = SmartCardCmd.MI_WRITEBLOCK + BlockAddr;
            for (i = 0; i < 16; i++)
            {
                send += DeleteSpaceString(Data[i].ToString("X"));
            }

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }
        }

        public virtual int ReadVal(string BlockAddr, out string StrReceived)
        {
            byte[] Data = new byte[4];
            uint i;
            UInt32 str;

            send = SmartCardCmd.MI_RDBLOCK + BlockAddr;
            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                str = Convert.ToUInt32(receive.Substring(0, 8), 16);
                for (i = 0; i < 4; i++)
                {
                    Data[i] = (byte)str;
                    str >>= 8;
                }
                str = 0;
                for (i = 0; i < 4; i++)
                {
                    str <<= 8;
                    str |= Data[i];
                }
                StrReceived = str.ToString();
                return 0;
            }
        }

        public virtual int INCVAL(string BlockAddr, string IncData, out string StrReceived)     // 2014.3.6 Hong
        {
            UInt16 i, j;

            if (IncData.Length < 8)
            {
                j = (UInt16)IncData.Length;
                for (i = 0; i < (8 - j); i++)
                    IncData += "0";
            }
            send = SmartCardCmd.MI_IncBlock;
            send += BlockAddr + IncData;

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }
        }

        public virtual int DECVAL(string BlockAddr, string DecData, out string StrReceived)     // 2014.3.6 Hong
        {
            UInt16 i, j;

            if (DecData.Length < 8)
            {
                j = (UInt16)DecData.Length;
                for (i = 0; i < (8 - j); i++)
                    DecData += "0";
            }
            send = SmartCardCmd.MI_DecBlock;
            send += BlockAddr + DecData;

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }
        }

        public virtual int RESTORE(string BlockAddr, out string StrReceived)                    // 2014.3.6 Hong
        {
            send = SmartCardCmd.MI_Restore + BlockAddr;

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                do
                {
                    send = SmartCardCmd.TR_CL_HEAD + "03" + "09" + "04";
                    send += "00000000";
                    SendCommand(send, out receive);
                    if ((receive.Substring((receive.Length - 4), 4) != "9000"))
                    {
                        StrReceived = "Succeeded";
                        return 1;
                    }
                } while (receive.Substring((receive.Length - 4), 4) != "9000");
                StrReceived = "Succeeded";
                return 0;
            }
        }

        public virtual int TRANSFER(string BlockAddr, out string StrReceived)                   // 2014.3.6 Hong
        {
            send = SmartCardCmd.MI_Transfer + BlockAddr;

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }
        }

        public virtual int LOADKEY(string AuthKeys, out string StrReceived)
        {
            byte tmp, ln, hn;
            byte[] keybuffer = new byte[15];
            byte[] Fm17_buffer = new byte[15];
            int i;

            keybuffer = strToHexByte(AuthKeys);
            for (i = 0; i < 6; i++)			//密码转换成FM17密码格式
            {
                ln = (byte)(keybuffer[i] & 0x0f);	//低4位
                hn = (byte)(keybuffer[i] >> 4);		//高4位
                Fm17_buffer[i * 2 + 1] = (byte)((~ln << 4) | ln);
                Fm17_buffer[i * 2] = (byte)((~hn << 4) | hn);
            }
            Set_FM1715_reg(0x09, 0x09, out StrReceived);
            Set_FM1715_reg(0x01, 0x00, out StrReceived);

            for (i = 0; i < 12; i++)			//
            {
                Set_FM1715_reg(0x02, Fm17_buffer[i], out StrReceived);
            }
            Set_FM1715_reg(0x01, 0x19, out StrReceived);
            Read_FM1715_reg(0x01, out StrReceived, out tmp);
            if (tmp != 0)
            {
                StrReceived = "Error";
                return 1;	 //LoadKey出错返回
            }
            Read_FM1715_reg(0x0A, out StrReceived, out tmp);
            if ((tmp & 0x40) != 0)
            {
                StrReceived = "Error";
                return 2;	 //LoadKey出错返回
            }
            StrReceived = "Succeeded";
            return 0;	 //LoadKey正确返回            

        }


        public virtual int AUTH(byte AuthType, byte KeyMode, string AuthBlockAddr, out string StrReceived)
        {
            byte tmp, block;

            if (uid == null)
            {
                StrReceived = "Error";
                return 3;
            }

            block = strToHexByte(AuthBlockAddr)[0];

            Set_FM1715_reg(0x09, 0x09, out StrReceived);
            Set_FM1715_reg(0x01, 0x00, out StrReceived);

            Set_FM1715_reg(0x31, AuthType, out StrReceived);
            Set_FM1715_reg(0x02, KeyMode, out StrReceived);
            Set_FM1715_reg(0x02, block, out StrReceived);
            Set_FM1715_reg(0x02, uid[0], out StrReceived);
            Set_FM1715_reg(0x02, uid[1], out StrReceived);
            Set_FM1715_reg(0x02, uid[2], out StrReceived);
            Set_FM1715_reg(0x02, uid[3], out StrReceived);

            Set_FM1715_reg(0x01, 0x0C, out StrReceived);
            Read_FM1715_reg(0x01, out StrReceived, out tmp);
            if (tmp != 0)
            {
                Set_FM1715_reg(0x09, 0x01, out StrReceived);
                StrReceived = "Error";
                return 1;	 //MIF_Authen出错返回
            }
            Read_FM1715_reg(0x0A, out StrReceived, out tmp);
            if ((tmp & 0x60) != 0)
            {
                Set_FM1715_reg(0x09, 0x01, out StrReceived);
                StrReceived = "Error";
                return 2;	 //MIF_Authen出错返回
            }

            Set_FM1715_reg(0x01, 0x14, out StrReceived);
            Read_FM1715_reg(0x01, out StrReceived, out tmp);
            if (tmp != 0)
            {
                Set_FM1715_reg(0x09, 0x01, out StrReceived);
                StrReceived = "Error";
                return 3;	 //MIF_Authen出错返回
            }

            Read_FM1715_reg(0x09, out StrReceived, out tmp);
            if ((tmp & 0x08) == 0)
            {
                Set_FM1715_reg(0x09, 0x01, out StrReceived);
                StrReceived = "Error";
                return 4;	 //MIF_Authen出错返回
            }

            StrReceived = "Succeeded";
            return 0;

        }

        public virtual int HALT(out string StrReceived)
        {
            receive = "";
            send = SmartCardCmd.MI_HALT;
            SendCommand(send, out receive);
            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";// +receive;
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }

        }

        public virtual int RATS(out string StrReceived)
        {
            receive = "";
            send = SmartCardCmd.MI_RATS;
            SendCommand(send, out receive);
            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";// +receive;
                return 1;
            }
            else
            {
                StrReceived = receive.Substring(0, (receive.Length - 4));
                return 0;
            }

        }

        /// <summary>
        /// AntiColl_lv1(ref string StrReceived)
        /// </summary>
        /// <param name="StrReceived"></param>
        /// <returns></returns>
        public virtual int AntiColl_lv1(out string StrReceived)
        {
            receive = "";
            send = SmartCardCmd.MI_ANTICOLL;
            SendCommand(send, out receive);
            if (receive.Substring((receive.Length - 4), 2) != "90")
            {
                StrReceived = "Error";// + receive;
                return 1;
            }
            else
            {
                StrReceived = receive.Substring(0, 10);
                uid = strToHexByte(StrReceived);
                if (receive.Substring((receive.Length - 4), 4) == "9001")
                    StrReceived += "(BCC Fail)";
                return 0;
            }
        }
        /// <summary>
        /// Select(ref string StrReceived)
        /// </summary>
        /// <param name="StrReceived"></param>
        /// <returns></returns>
        public virtual int Select(out string StrReceived)
        {
            receive = "";
            send = SmartCardCmd.MI_SEL;
            SendCommand(send, out receive);
            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";// + receive;
                return 1;
            }
            else
            {
                StrReceived = receive.Substring(0, 2);
                return 0;
            }
        }
        /// <summary>
        /// Read_FM1715_reg( string regAddr,out string regData)
        /// </summary>
        /// <param name="regAddr"></param>
        /// <param name="regData"></param>
        /// <returns></returns>
        public int Read_FM1715_reg(string regAddr, out string StrReceived, out byte regData)
        {
            //string regAddr_s;
            //regAddr_s = regAddr.ToString("X2");

            send = "FF0F02";
            send = send + regAddr + "01";
            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                regData = 0xFF;
                return 1;
            }
            else
            {
                regData = Convert.ToByte(receive.Substring(0, 2), 16);
                StrReceived = receive.Substring(0, 2);
                return 0;
            }
        }

        public int Read_FM1715_reg(byte regAddr, out string StrReceived, out byte regData)
        {
            string regAddr_s;
            regAddr_s = regAddr.ToString("X2");

            send = "FF0F02";
            send = send + regAddr_s + "01";
            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                regData = 0xFF;
                return 1;
            }
            else
            {
                regData = Convert.ToByte(receive.Substring(0, 2), 16);
                StrReceived = receive.Substring(0, 2);
                return 0;
            }
        }

        /// <summary>
        ///  Read_TDA8007_reg( regAddr, out  regData)
        /// </summary>
        /// <param name="regAddr"></param>
        /// <param name="regData"></param>
        /// <returns></returns>
        public int Read_TDA8007_reg(string regAddr, out string StrReceived, out byte regData)
        {
            //string regAddr_s;
            //regAddr_s = regAddr.ToString("X2");

            send = "FF0F01";
            send = send + regAddr + "01";
            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                regData = 0xFF;
                return 1;
            }
            else
            {
                regData = Convert.ToByte(receive.Substring(0, 2), 16);
                StrReceived = receive.Substring(0, 2);
                return 0;
            }
        }

        public int Read_TDA8007_reg(byte regAddr, out string StrReceived, out byte regData)
        {
            string regAddr_s;
            regAddr_s = regAddr.ToString("X2");

            send = "FF0F01";
            send = send + regAddr_s + "01";
            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                regData = 0xFF;
                return 1;
            }
            else
            {
                regData = Convert.ToByte(receive.Substring(0, 2), 16);
                StrReceived = receive.Substring(0, 2);
                return 0;
            }
        }

        public int Init_TDA8007(out string StrReceived)
        {
            send = SmartCardCmd.CT_INITTDA;
            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) == "9000")
            {
                StrReceived = "Succeeded";
                return 0;
            }
            else
            {
                StrReceived = "Error";
                return 1;
            }

        }

        /// <summary>
        /// Set_FM1715_reg( regAddr, regData)
        /// </summary>
        /// <param name="regAddr"></param>
        /// <param name="regData"></param>
        /// <returns></returns>
        public int Set_FM1715_reg(string regAddr, string regData, out string StrReceived)
        {
            //string regAddr_s, regData_s;
            //regAddr_s = regAddr.ToString("X2");
            //regData_s = regData.ToString("X2");

            send = "FF1102";
            send += regAddr + "01";
            send += regData;

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }
        }

        public int Set_FM1715_reg(byte regAddr, byte regData, out string StrReceived)
        {
            string regAddr_s, regData_s;
            regAddr_s = regAddr.ToString("X2");
            regData_s = regData.ToString("X2");

            send = "FF1102";
            send += regAddr_s + "01";
            send += regData_s;

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }
        }

        public int Set_TDA8007_reg(string regAddr, string regData, out string StrReceived)
        {
            //string regAddr_s, regData_s;
            //regAddr_s = regAddr.ToString("X2");
            //regData_s = regData.ToString("X2");

            send = "FF1101";
            send += regAddr + "01";
            send += regData;

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }
        }
        /// <summary>
        /// Set_TDA8007_reg
        /// </summary>
        /// <param name="regAddr"></param>
        /// <param name="regData"></param>
        /// <param name="StrReceived"></param>
        /// <returns></returns>
        public int Set_TDA8007_reg(byte regAddr, byte regData, out string StrReceived)
        {
            string regAddr_s, regData_s;
            regAddr_s = regAddr.ToString("X2");
            regData_s = regData.ToString("X2");

            send = "FF1101";
            send += regAddr_s + "01";
            send += regData_s;

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }
        }
        public virtual int Set_TDA8007_regbit(string regAddr, int bitsel, int bitdata, out string StrReceived)
        {
            byte regData;
            int i, j = 1;
            string strReceived, regData_Str;
            i = Read_TDA8007_reg(regAddr, out strReceived, out regData);
            if (bitdata == 1)
                regData |= (byte)(0x01 << bitsel);
            else
                regData &= (byte)~(0x01 << bitsel);
            regData_Str = regData.ToString("X2");
            if (i == 0)
            {
                j = Set_TDA8007_reg(regAddr, regData_Str, out strReceived);
            }
            else
            {
                StrReceived = "Read_TDA8007 Error";
                return 1;
            }
            if (j == 0)
            {
                StrReceived = "Succeeded";
                return 0;
            }
            else
            {
                StrReceived = "Error";
                return 1;
            }
        }

        /// <summary>
        /// ReadBlock( string BlockAddr ,ref string StrReceived)
        /// </summary>
        /// <param name="BlockAddr"></param>
        /// <param name="StrReceived"></param>
        /// <returns></returns>
        public virtual int ReadBlock(string BlockAddr, out string StrReceived)
        {
            send = SmartCardCmd.MI_RDBLOCK;
            send += BlockAddr;

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = receive.Substring(0, receive.Length - 4);
                return 0;
            }
        }
        /// <summary>
        /// WriteBlock(string BlockAddr ,string BlockData)
        /// </summary>
        /// <param name="BlockAddr"></param>
        /// <param name="BlockData"></param>
        /// <returns></returns>
        public virtual int WriteBlock(string BlockAddr, string BlockData, out string StrReceived)
        {

            send = SmartCardCmd.MI_WRITEBLOCK;
            send += BlockAddr + BlockData;

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }
        }
        public virtual int Write128Bytes(string BlockAddr, string BlockData, out string StrReceived)
        {

            send = SmartCardCmd.MI_WRITE128BYTES;
            send += BlockAddr + BlockData;

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }

        }
        /// <summary>
        /// SelectCL(ref string StrReceived)
        /// </summary>
        /// <param name="StrReceived"></param>
        public virtual void SelectCL(out string StrReceived)
        {
            ;
            send = SmartCardCmd.SEL_CL;
            SendCommand(send, out StrReceived);

            return;
        }
        /// <summary>
        /// void SelectCT(ref string StrReceived)
        /// </summary>
        /// <param name="StrReceived"></param>
        public virtual void SelectCT(out string StrReceived)
        {

            send = SmartCardCmd.SEL_CT_30V;
            SendCommand(send, out StrReceived);

            return;
        }

        public virtual int SendAPDUCL(string sendData, out string StrReceived)
        {
            string sendData_noSpace;
            byte crc_l, crc_h, errflag;
            string CRC_L, CRC_H, ErrFlag, Display = "";

            if (CurrentInterface != 1)  //界面切换
            {

                SelectCL(out receive);

                if (receive.Substring((receive.Length - 4), 4) != "9000") //选择CL通道
                {
                    StrReceived = "CL interface select Error";
                    return 1;
                }
                CurrentInterface = 1;
            }
            sendData_noSpace = DeleteSpaceString(sendData);

            send = sendData_noSpace;

            SendCommand(send, out receive);
            if (receive.Length != 0)
            {
                if (receive.Substring((receive.Length - 4), 4) == "9000") //无等待延时，需手动去取数据
                {
                    StrReceived = receive;//.Substring((receive.Length - 4), 4);
                    return 0;
                }

                if (receive.Substring((receive.Length - 4), 2) == "61")//有数据返回，取出到dataReceive中 SW：61XX
                {
                    //if (receive.Substring((receive.Length - 2), 2) == "00")
                    //{
                    //    StrReceived = "No Data Received ";
                    //    return 1;
                    //}
                    do
                    {
                        send = SmartCardCmd.TR_GETDATA_NORM + receive.Substring((receive.Length - 2), 2);//最多256个
                        SendCommand(send, out receive);
                        if ((receive.Substring((receive.Length - 4), 4) != "9000") && (receive.Substring((receive.Length - 4), 2) != "61"))
                        {
                            StrReceived = "Get Data Error";
                            return 1;
                        }

                        StrReceived = receive.Substring(0, receive.Length - 4);//多带两byte "00"剔除掉 CRC无法读出 一般一次性取出

                    } while (receive.Substring((receive.Length - 4), 4) != "9000");

                }
                else
                {
                    StrReceived = receive;
                }
            }
            else
            {
                Read_FM1715_reg(0x0A, out ErrFlag, out errflag);
                Read_FM1715_reg(0x0D, out CRC_L, out crc_l);
                Read_FM1715_reg(0x0E, out CRC_H, out crc_h);
                if ((errflag & 0x0F) != 0x00)
                {//PICC data error, look up in the RxErr register
                    if ((errflag & 0x08) == 0x08)
                    {
                        Display = "PICC CRC Error:" + CRC_L + CRC_H;
                    }

                    if ((errflag & 0x04) == 0x04)
                    {
                        Display += "\n\t\t\t\t\t" + "PICC SOF error";
                    }
                    if ((errflag & 0x02) == 0x02)
                    {
                        Display += "\n\t\t\t\t\t" + "PICC parity error";
                    }
                    StrReceived = Display;
                    return 0;
                }
                else
                {
                    StrReceived = "Unknown Send Error";
                    return 1;
                }
            }
            return 0;
        }

        public virtual int SendAPDUCT(string sendData, out string StrReceived)
        {
            string sendData_noSpace;

            if (CurrentInterface != 2)  //界面切换
            {

                SelectCT(out receive);

                if (receive.Substring((receive.Length - 4), 4) != "9000") //选择CL通道
                {
                    StrReceived = "CL interface select Error";
                    return 1;
                }
                CurrentInterface = 2;
            }
            sendData_noSpace = DeleteSpaceString(sendData);

            send = sendData_noSpace;

            SendCommand(send, out receive);

            if (receive.Length != 0)
            {
                if (receive.Substring((receive.Length - 4), 4) == "9000") //无等待延时，需手动去取数据
                {
                    StrReceived = receive;//.Substring((receive.Length - 4), 4);
                    return 0;
                }
                StrReceived = "";
                if (receive.Substring((receive.Length - 4), 2) == "61")//有数据返回，取出到dataReceive中 SW：61XX
                {
                    //if (receive.Substring((receive.Length - 2), 2) == "00")
                    //{
                    //    StrReceived = "No Data Received ";
                    //    return 1;
                    //}
                    do
                    {
                        send = SmartCardCmd.TR_GETDATA_NORM + receive.Substring((receive.Length - 2), 2);//最多256个
                        SendCommand(send, out receive);
                        if ((receive.Substring((receive.Length - 4), 4) != "9000") && (receive.Substring((receive.Length - 4), 2) != "61"))
                        {
                            StrReceived = "Get Data Error";
                            return 1;
                        }

                        StrReceived += receive.Substring(0, receive.Length - 4);//多带两byte "00"剔除掉 CRC无法读出 假设一般一次性取出

                    } while (receive.Substring((receive.Length - 4), 4) != "9000");

                }
                else
                {
                    StrReceived = receive;
                }
            }
            else
            {
                StrReceived = "Send Error";
                return 1;
            }
            return 0;
        }
        /// <summary>
        /// TransceiveCL(string sendData,string CRC_En,string TimeOut,ref string StrReceived)
        /// </summary>
        /// <param name="sendData"></param>
        /// <param name="CRC_En"></param>
        /// <param name="TimeOut"></param>
        /// <param name="StrReceived"></param>
        /// <returns></returns>
        public virtual int TransceiveCL(string sendData, string CRC_En, string TimeOut, out string StrReceived)
        {
            string sendData_noSpace, len;
            byte crc_l, crc_h, errflag;
            string CRC_L, CRC_H, ErrFlag, Display = "";

            if (CurrentInterface != 1)  //界面切换
            {

                SelectCL(out receive);

                if (receive.Substring((receive.Length - 4), 4) != "9000") //选择CL通道
                {
                    StrReceived = "CL interface select Error";
                    return 1;
                }
                CurrentInterface = 1;
            }
            sendData_noSpace = DeleteSpaceString(sendData);
            len = (sendData_noSpace.Length / 2).ToString("X2");

            send = SmartCardCmd.TR_CL_HEAD + CRC_En + TimeOut + len;
            send += sendData_noSpace;

            SendCommand(send, out receive);
            if (receive.Length != 0)
            {
                if (receive.Substring((receive.Length - 4), 4) == "9000") //无等待延时，需手动去取数据
                {
                    StrReceived = receive.Substring((receive.Length - 4), 4);
                    return 0;
                }

                if (receive.Substring((receive.Length - 4), 2) == "61")//有数据返回，取出到dataReceive中 SW：61XX
                {
                    if (receive.Substring((receive.Length - 2), 2) == "00")
                    {
                        StrReceived = "No Data Received ";
                        return 1;
                    }
                    do
                    {
                        send = SmartCardCmd.TR_GETDATA + receive.Substring((receive.Length - 2), 2);//最多256个
                        SendCommand(send, out receive);
                        if ((receive.Substring((receive.Length - 4), 4) != "9000") && (receive.Substring((receive.Length - 4), 2) != "61"))
                        {
                            StrReceived = "Get Data Error";
                            return 1;
                        }

                        StrReceived = receive.Substring(0, receive.Length - 4);//CRC在卡机中以0000代替，未真正去取，此处假设一般一次性取出全部数据

                    } while (receive.Substring((receive.Length - 4), 4) != "9000");

                }
                else
                {
                    StrReceived = receive;
                }
            }
            else
            {
                Read_FM1715_reg(0x0A, out ErrFlag, out errflag);
                Read_FM1715_reg(0x0D, out CRC_L, out crc_l);
                Read_FM1715_reg(0x0E, out CRC_H, out crc_h);
                if ((errflag & 0x0F) != 0x00)
                {//PICC data error, look up in the RxErr register
                    if ((errflag & 0x08) == 0x08)
                    {
                        Display = "PICC CRC Error:" + CRC_L + CRC_H;
                    }

                    if ((errflag & 0x04) == 0x04)
                    {
                        Display += "\n\t\t\t\t\t" + "PICC SOF error";
                    }
                    if ((errflag & 0x02) == 0x02)
                    {
                        Display += "\n\t\t\t\t\t" + "PICC parity error";
                    }
                    StrReceived = Display;
                    return 0;
                }
                else
                {
                    StrReceived = "Unknown Send Error";
                    return 1;
                }
            }
            return 0;
        }
        public virtual int SendReceiveCL(string sendData, out string StrReceived)//简化，固定带CRC，timeout固定
        {
            int ret = TransceiveCL(sendData, "01", "09", out StrReceived);
            return ret;
        }

        public virtual int SendRATS(out string ATR)
        {
            int ret = SendReceiveCL("E081", out ATR);
            return ret;
        }
        /// <summary>
        /// TransceiveCT(string sendData,string TimeOut, ref string StrReceived)
        /// </summary>
        /// <param name="sendData"></param>
        /// <param name="TimeOut"></param>
        /// <param name="StrReceived"></param>
        /// <returns></returns>
        public virtual int TransceiveCT(string sendData, string TimeOut, out string StrReceived)
        {
            string sendData_noSpace, len;

            if (CurrentInterface != 2)
            {
                SelectCT(out receive);

                if (receive.Substring((receive.Length - 4), 4) != "9000") //选择CL通道
                {
                    StrReceived = "CT interface select Error";
                    return 1;
                }
                CurrentInterface = 2;
            }

            sendData_noSpace = DeleteSpaceString(sendData);
            if (sendData_noSpace.Length / 2 == 0x100)            //CNN 添加init模块  256字节的写操作
            {
                len = "00";
            }
            else
                len = (sendData_noSpace.Length / 2).ToString("X2");

            send = SmartCardCmd.TR_CT_HEAD + TimeOut + len;
            send += sendData_noSpace;

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) == "9000") //无等待延时，需手动去取数据
            {
                StrReceived = receive.Substring((receive.Length - 4), 4);
                return 0;
            }

            if (receive.Substring((receive.Length - 4), 2) == "61")//有数据返回，取出到dataReceive中 SW：61XX
            {
                if (receive.Substring((receive.Length - 2), 2) == "00")
                {
                    StrReceived = "No Data Received";
                    return 1;
                }
                do
                {
                    send = SmartCardCmd.TR_GETDATA + receive.Substring((receive.Length - 2), 2);
                    SendCommand(send, out receive);
                    if ((receive.Substring((receive.Length - 4), 4) != "9000") && (receive.Substring((receive.Length - 4), 2) != "61"))
                    {
                        StrReceived = "Get Data Error";
                        return 1;
                    }

                    StrReceived = receive.Substring(0, receive.Length - 4);//CRC在卡机中以0000代替，未真正去取，此处假设一般一次性取出全部数据

                } while (receive.Substring((receive.Length - 4), 4) != "9000");

            }
            else
            {
                StrReceived = "Send Error";
                return 1;
            }
            return 0;
        }
        public virtual int PPS_CL(string PPS1, out string StrReceived)
        {
            receive = "";
            send = SmartCardCmd.MI_PPS_CL + PPS1;
            SendCommand(send, out receive);
            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";// +receive;
                return 1;
            }
            else if (receive.Substring((receive.Length - 4), 4) == "9000")
            {
                StrReceived = "PPS Exchange Succeeded";
                return 0;
            }
            else
            {
                StrReceived = "卡片没有响应";
                return 0;
            }

        }
        public virtual int PPS_CT(string PPS1, out string StrReceived)
        {
            receive = "";
            send = SmartCardCmd.MI_PPS_CT + PPS1;
            SendCommand(send, out receive);
            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";// +receive;
                return 1;
            }
            else
            {
                StrReceived = "PPS Exchange Succeeded";
                return 0;
            }

        }
        /// <summary>
        /// Cold_Reset(string voltage ,ref string StrReceived)
        /// </summary>
        /// <param name="voltage"></param>
        /// <param name="StrReceived"></param>
        /// <returns></returns>
        public virtual int Cold_Reset(string voltage, out string StrReceived)
        {
            send = SmartCardCmd.CT_COLDRESET + voltage;
            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = receive.Substring(0, receive.Length - 4);
                return 0;
            }
        }
        /// <summary>
        /// Warm_Reset(string voltage, ref string StrReceived)
        /// </summary>
        /// <param name="voltage"></param>
        /// <param name="StrReceived"></param>
        /// <returns></returns>
        public virtual int Warm_Reset(string voltage, out string StrReceived)
        {
            send = SmartCardCmd.CT_WARMRESET + voltage;
            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = receive.Substring(0, receive.Length - 4);
                return 0;
            }
        }
        /// <summary>
        /// SetField(int OnOff)
        /// </summary>
        /// <param name="OnOff"></param>
        /// <returns></returns>
        public int SetField(int OnOff, out string StrReturned)
        {
            if (OnOff == 0)
                send = SmartCardCmd.MI_FieldON + "00";
            else
                send = SmartCardCmd.MI_FieldON + "01";

            SendCommand(send, out receive);


            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReturned = "Error";
                return 1;
            }
            else
            {
                if (OnOff == 0)
                    StrReturned = "Off";
                else
                    StrReturned = "On";
                return 0;
            }
        }

        public virtual int SPI_Send(string StrSend, string lenToReceive, out string StrReceived)
        {
            int len;
            receive = "";
            send = DeleteSpaceString(StrSend);
            len = send.Length / 2;

            send = SPIandI2Ccmd.SPI_SEND + len.ToString("X2") + lenToReceive + send;

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = receive;
                return 0;
            }
        }
        public virtual int I2C_Send(string StrSend, string lenToReceive, out string StrReceived)
        {
            int len;
            receive = "";
            send = DeleteSpaceString(StrSend);
            len = send.Length / 2;

            send = SPIandI2Ccmd.I2C_SEND + len.ToString("X2") + lenToReceive + send;

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = receive;
                return 0;
            }
        }
        public virtual int TypeAinit(out string StrReceived)
        {
            string strSend, strReceived;
            PRTCL = 0;//TypeA

            strSend = "FF09010102";
            SendCommand(strSend, out strReceived);
            if (strReceived.Substring((strReceived.Length - 4), 4) == "61FE")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }

        }
        public virtual int TypeBinit(out string StrReceived)
        {
            /* Set_FM1715_reg(0x11, 0x4B, out StrReceived);
             Set_FM1715_reg(0x13, 0x06, out StrReceived);
             Set_FM1715_reg(0x14, 0x20, out StrReceived);
             Set_FM1715_reg(0x17, 0x23, out StrReceived);
             Set_FM1715_reg(0x19, 0x73, out StrReceived);
             Set_FM1715_reg(0x1A, 0x19, out StrReceived);
             Set_FM1715_reg(0x1C, 0x44, out StrReceived);
             Set_FM1715_reg(0x1D, 0x1E, out StrReceived);
             Set_FM1715_reg(0x22, 0x2C, out StrReceived);
             Set_FM1715_reg(0x23, 0xFF, out StrReceived);
             Set_FM1715_reg(0x24, 0xFF, out StrReceived);*/



            string strSend, strReceived;
            PRTCL = 1;//TypeB

            strSend = "FF09010B03050000";
            SendCommand(strSend, out strReceived);
            if (strReceived.Substring((strReceived.Length - 4), 4) == "61FE")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = strReceived;
                return 0;
            }

        }
        public virtual int Set_TxBaudRate_106k(out string StrReceived)
        {
            string strReceived1, strReceived2;
            if (PRTCL == 1)
            {
                Set_FM1715_reg(0x14, 0x20, out strReceived1);
                Set_FM1715_reg(0x15, 0x13, out strReceived2);
                if ((strReceived1 == "Error") || (strReceived2 == "Error"))
                {
                    StrReceived = "Error_B";
                    return 1;
                }
                else
                {
                    StrReceived = "Succeeded_B";
                    return 0;
                }
            }
            else
            {
                Set_FM1715_reg(0x14, 0x19, out strReceived1);
                Set_FM1715_reg(0x15, 0x13, out strReceived2);
                if ((strReceived1 == "Error") || (strReceived2 == "Error"))
                {
                    StrReceived = "Error_A";
                    return 1;
                }
                else
                {
                    StrReceived = "Succeeded_A";
                    return 0;
                }
            }
        }
        public virtual int Set_TxBaudRate_212k(out string StrReceived)
        {
            string strReceived1, strReceived2;
            if (PRTCL == 1)
            {
                Set_FM1715_reg(0x14, 0x18, out strReceived1);
                Set_FM1715_reg(0x15, 0x13, out strReceived2);
                if ((strReceived1 == "Error") || (strReceived2 == "Error"))
                {
                    StrReceived = "Error_B";
                    return 1;
                }
                else
                {
                    StrReceived = "Succeeded_B";
                    return 0;
                }
            }
            else
            {
                Set_FM1715_reg(0x14, 0x11, out strReceived1);
                Set_FM1715_reg(0x15, 0x09, out strReceived2);
                if ((strReceived1 == "Error") || (strReceived2 == "Error"))
                {
                    StrReceived = "Error_A";
                    return 1;
                }
                else
                {
                    StrReceived = "Succeeded_A";
                    return 0;
                }
            }
        }
        public virtual int Set_TxBaudRate_424k(out string StrReceived)
        {
            string strReceived1, strReceived2;
            if (PRTCL == 1)
            {
                Set_FM1715_reg(0x14, 0x10, out strReceived1);
                Set_FM1715_reg(0x15, 0x13, out strReceived2);
                if ((strReceived1 == "Error") || (strReceived2 == "Error"))
                {
                    StrReceived = "Error_B";
                    return 1;
                }
                else
                {
                    StrReceived = "Succeeded_B";
                    return 0;
                }
            }
            else
            {
                Set_FM1715_reg(0x14, 0x09, out strReceived1);
                Set_FM1715_reg(0x15, 0x04, out strReceived2);
                if ((strReceived1 == "Error") || (strReceived2 == "Error"))
                {
                    StrReceived = "Error_A";
                    return 1;
                }
                else
                {
                    StrReceived = "Succeeded_A";
                    return 0;
                }
            }
        }
        public virtual int Set_TxBaudRate_848k(out string StrReceived)
        {
            string strReceived1, strReceived2;
            if (PRTCL == 1)
            {
                Set_FM1715_reg(0x14, 0x08, out strReceived1);
                Set_FM1715_reg(0x15, 0x13, out strReceived2);
                if ((strReceived1 == "Error") || (strReceived2 == "Error"))
                {
                    StrReceived = "Error_B";
                    return 1;
                }
                else
                {
                    StrReceived = "Succeeded_B";
                    return 0;
                }
            }
            else
            {
                Set_FM1715_reg(0x14, 0x01, out strReceived1);
                Set_FM1715_reg(0x15, 0x01, out strReceived2);
                if ((strReceived1 == "Error") || (strReceived2 == "Error"))
                {
                    StrReceived = "Error_A";
                    return 1;
                }
                else
                {
                    StrReceived = "Succeeded_A";
                    return 0;
                }
            }
        }
        public virtual int Set_RxBaudRate_848k(out string StrReceived)
        {
            string strReceived1, strReceived2;
            Set_FM1715_reg(0x19, 0x13, out strReceived1);
            if (PRTCL == 1)
            {
                Set_FM1715_reg(0x1A, 0x19, out strReceived2);
                if ((strReceived1 == "Error") || (strReceived2 == "Error"))
                {
                    StrReceived = "Error_B";
                    return 1;
                }
                else
                {
                    StrReceived = "Succeeded_B";
                    return 0;
                }
            }
            else
            {
                Set_FM1715_reg(0x1A, 0x09, out strReceived2);
                if ((strReceived1 == "Error") || (strReceived2 == "Error"))
                {
                    StrReceived = "Error_A";
                    return 1;
                }
                else
                {
                    StrReceived = "Succeeded_A";
                    return 0;
                }
            }
        }
        public virtual int Set_RxBaudRate_424k(out string StrReceived)
        {
            string strReceived1, strReceived2;
            Set_FM1715_reg(0x19, 0x33, out strReceived1);
            if (PRTCL == 1)
            {
                Set_FM1715_reg(0x1A, 0x19, out strReceived2);
                if ((strReceived1 == "Error") || (strReceived2 == "Error"))
                {
                    StrReceived = "Error_B";
                    return 1;
                }
                else
                {
                    StrReceived = "Succeeded_B";
                    return 0;
                }
            }
            else
            {
                Set_FM1715_reg(0x1A, 0x09, out strReceived2);
                if ((strReceived1 == "Error") || (strReceived2 == "Error"))
                {
                    StrReceived = "Error_A";
                    return 1;
                }
                else
                {
                    StrReceived = "Succeeded_A";
                    return 0;
                }
            }
        }
        public virtual int Set_RxBaudRate_212k(out string StrReceived)
        {
            string strReceived1, strReceived2;
            Set_FM1715_reg(0x19, 0x53, out strReceived1);
            if (PRTCL == 1)
            {
                Set_FM1715_reg(0x1A, 0x19, out strReceived2);
                if ((strReceived1 == "Error") || (strReceived2 == "Error"))
                {
                    StrReceived = "Error_B";
                    return 1;
                }
                else
                {
                    StrReceived = "Succeeded_B";
                    return 0;
                }
            }
            else
            {
                Set_FM1715_reg(0x1A, 0x09, out strReceived2);
                if ((strReceived1 == "Error") || (strReceived2 == "Error"))
                {
                    StrReceived = "Error_A";
                    return 1;
                }
                else
                {
                    StrReceived = "Succeeded_A";
                    return 0;
                }
            }
        }
        public virtual int Set_RxBaudRate_106k(out string StrReceived)
        {
            string strReceived1, strReceived2;
            Set_FM1715_reg(0x19, 0x73, out strReceived1);
            if (PRTCL == 1)
            {
                Set_FM1715_reg(0x1A, 0x19, out strReceived2);
                if ((strReceived1 == "Error") || (strReceived2 == "Error"))
                {
                    StrReceived = "Error_B";
                    return 1;
                }
                else
                {
                    StrReceived = "Succeeded_B";
                    return 0;
                }
            }
            else
            {
                Set_FM1715_reg(0x1A, 0x08, out strReceived2);
                if ((strReceived1 == "Error") || (strReceived2 == "Error"))
                {
                    StrReceived = "Error_A";
                    return 1;
                }
                else
                {
                    StrReceived = "Succeeded_A";
                    return 0;
                }
            }
        }

        public virtual int ReadFileByte(String fileName, out int datalength, out byte[] data)
        {
            //以独占方式打开一个文件
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            //创建一个Byte用来存放读取到的文件内容
            data = new Byte[fs.Length];
            datalength = data.Length;
            //定义变量存储初始读取位置
            int offset = 0;
            //定义变量存储当前数据剩余未读的长度
            int remaining = data.Length / 2;
            try
            {
                while (remaining > 0)
                {
                    int read = fs.Read(data, offset, remaining);
                    if (read <= 0)
                        return 1;
                    //throw new EndOfStreamException("文件读取到" + read.ToString() + "失败！");
                    // 减少剩余的字节数
                    remaining -= read;
                    // 增加偏移量
                    offset += read;
                }
            }
            catch
            {
                data = null;
            }
            fs.Dispose();
            return 0;
        }

        public static void Hex2Hexbin(byte[] bHex, byte[] bHexbin, int nLen)
        {
            byte c;
            for (int i = 0; i < nLen; i++)
            {
                c = Convert.ToByte((bHex[i] >> 4) & 0x0f);
                if (c < 0x0a)
                {
                    bHexbin[2 * i] = Convert.ToByte(c + 0x30);
                }
                else
                {
                    bHexbin[2 * i] = Convert.ToByte(c + 0x37);
                }
                c = Convert.ToByte(bHex[i] & 0x0f);
                if (c < 0x0a)
                {
                    bHexbin[2 * i + 1] = Convert.ToByte(c + 0x30);
                }
                else
                {
                    bHexbin[2 * i + 1] = Convert.ToByte(c + 0x37);
                }
            }
        }
        public virtual int Hex2Hexbin(byte[] bHex, int nLen, out byte[] bHexbin)
        {
            bHexbin = new byte[nLen * 2];
            Hex2Hexbin(bHex, bHexbin, nLen);
            return 0;
        }
        public virtual int DataEncryptPro(uint btEncryptOpt, uint P_Data, uint btAddr, out uint C_Data)
        {
            //string C_Data;
            if (btEncryptOpt == 0)
            {
                C_Data = P_Data;
            }
            else if (btEncryptOpt == 1)
            {		//加密
                DataEncrypt(1, P_Data, btAddr, out C_Data);
            }
            else
            {		//解密
                DataDeEncrypt(1, P_Data, btAddr, out C_Data);
            }
            return 0;
        }
        //---------------------------------------------------------------------------
        public virtual int DataEncrypt(int bWrEncrypt, uint P_Data, uint btAddr, out uint C_Data)		//编程数据加密函数
        {
            uint P_Data1, A_Key;

            if (bWrEncrypt == 0)
            {
                C_Data = P_Data;
                return 1;
            }

            A_Key = (g_EncryptS1[(btAddr & 0xF0) >> 4] << 4) | g_EncryptS0[(btAddr & 0x0F)];
            P_Data1 = A_Key ^ P_Data;
            C_Data = (g_EncryptS3[(P_Data1 & 0xF0) >> 4] << 4) | g_EncryptS2[(P_Data1 & 0x0F)];

            return 0;
        }

        //---------------------------------------------------------------------------
        public virtual int DataDeEncrypt(int bRdEncrypt, uint P_Data, uint btAddr, out uint C_Data)	//编程数据解密函数
        {
            uint P_Data1, A_Key;

            if (bRdEncrypt == 0)
            {
                C_Data = P_Data;
                return 1;
            }

            A_Key = (g_EncryptS1[(btAddr & 0xF0) >> 4] << 4) | g_EncryptS0[(btAddr & 0x0F)];
            P_Data1 = (g_EncryptS5[(P_Data & 0xF0) >> 4] << 4) | g_EncryptS4[(P_Data & 0x0F)];
            C_Data = A_Key ^ P_Data1;

            return 0;
        }
        public virtual int Get_timeCL(out string StrTime)
        {
            string tempStr;
            byte[] buf1;
            Int64 templ1, templ2;
            float TemplMs;

            SendAPDUCL("0040000008", out tempStr);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrTime = "Error";
                return 1;
            }
            else
            {
                buf1 = strToHexByte(tempStr);

                templ1 = buf1[2] * 0x100 + buf1[3];
                templ1 = 0x7FFF - templ1;
                templ2 = (buf1[6] * 0x100 + buf1[7]) * 0x8000;
                templ1 += templ2;
                TemplMs = (float)(templ1 / 105.9375);
                StrTime = TemplMs + "ms";
                return 0;
            }
        }
        public virtual int Get_timeCT(out string StrTime)
        {
            string tempStr;
            byte[] buf1;
            Int64 templ1, templ2;
            float TemplMs;

            SendAPDUCT("0040000008", out tempStr);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrTime = "Error";
                return 1;
            }
            else
            {
                buf1 = strToHexByte(tempStr);

                templ1 = buf1[0] * 0x100 + buf1[1];
                templ1 = 0x7FFF - templ1;
                templ2 = (buf1[4] * 0x100 + buf1[5]) * 0x8000;
                templ1 += templ2;
                TemplMs = (float)(templ1 / 3579.5);
                StrTime = TemplMs + "ms";
                return 0;
            }
        }

        public virtual int ProgExt_CwdCheck(int InterfaceSel, int FMXX_Sel, out int ExtStartAddr, out int prog_extand_mode, out int ExtOpt, out int prog_mem_type, out int EEmax, out int LIBsize, out int cwd14, out string STR)	//编程段后扩展，控制字检测EE OR RAM,EE大小或者RAM大小
        {
            string cwd0, cwd10, cwd11, cwd13, block0;
            int temp_cwd0, temp_cwd10, temp_cwd11, temp_cwd13;
            ExtStartAddr = 160 * 1024;
            ExtOpt = 0;        //EE  OR RAM
            prog_mem_type = 3;//ROM  OR  EE
            prog_extand_mode = 2;//段后扩展或整体扩展
            EEmax = 80 * 1024;
            cwd11 = "";
            LIBsize = 0;
            cwd14 = 0x94;

            try
            {
                if (InterfaceSel == 0)//非接
                {
                    TransceiveCL("32A0", "01", "09", out block0);
                    if (block0 == "00000000000000000000000000000000")
                    {
                        STR = "ok";
                    }
                    else
                    {
                        STR = "error";
                        return 0;
                    }
                    //TransceiveCL("3180", "01", "09", out block0);
                    ReadBlock("00", out block0);
                    cwd0 = block0.Substring(0, 2);
                    cwd10 = block0.Substring(20, 2);
                    cwd11 = block0.Substring(22, 2);
                    cwd13 = block0.Substring(26, 2);
                    cwd14 = strToHexByte(block0.Substring(28, 2))[0];

                }
                else
                {
                    //TransceiveCT("0001000000", "02", out block0);
                    TransceiveCT("0002010000", "02", out block0);
                    if (block0 == "9000")
                    {
                        STR = "ok";
                    }
                    else
                    {
                        STR = "error";
                        return 0;
                    }
                    TransceiveCT("0004000010", "02", out block0);
                    cwd0 = block0.Substring(2, 2);
                    cwd10 = block0.Substring(22, 2);
                    cwd11 = block0.Substring(24, 2);
                    cwd13 = block0.Substring(28, 2);
                    cwd14 = strToHexByte(block0.Substring(30, 2))[0];

                }
                temp_cwd0 = strToHexByte(cwd0)[0] & 0x60;
                temp_cwd10 = strToHexByte(cwd10)[0] & 0x03;
                temp_cwd11 = strToHexByte(cwd11)[0];
                temp_cwd13 = strToHexByte(cwd13)[0] & 0x7F;
                if ((strToHexByte(cwd10)[0] & 0x04) == 0x00)
                {
                    prog_mem_type = 0;
                }
                else
                    prog_mem_type = 1;

                if (FMXX_Sel == 1)
                {
                    temp_cwd10 = strToHexByte(cwd10)[0] & 0x08;
                }

                LIBsize = temp_cwd11 & 0xF8; // LIB区不为0，CNN 20130410          

                if ((temp_cwd0 & 0x20) == 0x20)//程序扩展选择段后扩展
                {
                    prog_extand_mode = 1;//段后扩展               
                }
                else
                    prog_extand_mode = 0;

                if ((temp_cwd0 & 0x40) == 0x00)//选择EE
                {
                    ExtOpt = 0;
                    if (FMXX_Sel == 1)
                    {
                        if (temp_cwd10 == 0x00)
                        {
                            EEmax = 40 * 1024;
                            ExtStartAddr = 40 * 1024 - temp_cwd13 * 1024;
                        }
                        else
                        {
                            EEmax = 80 * 1024;
                            ExtStartAddr = 80 * 1024 - temp_cwd13 * 1024;
                        }
                    }
                    else
                    {
                        if (temp_cwd10 == 0x00)
                        {
                            EEmax = 40 * 1024;
                            ExtStartAddr = 40 * 1024 - temp_cwd13 * 1024;
                        }
                        else if (temp_cwd10 == 0x01)
                        {
                            EEmax = 80 * 1024;
                            ExtStartAddr = 80 * 1024 - temp_cwd13 * 1024;
                        }
                        else
                        {
                            EEmax = 160 * 1024;
                            ExtStartAddr = 160 * 1024 - temp_cwd13 * 1024;
                        }
                    }
                }
                else if ((temp_cwd0 & 0x40) == 0x40)//选择Ram
                {
                    ExtOpt = 1;
                    if (temp_cwd13 > 7)
                        temp_cwd13 = 7;
                    ExtStartAddr = 0x2000 - temp_cwd13 * 1024;
                    EEmax = 8 * 1024; ;
                }
                return 0;
            }
            catch (Exception)
            {
                STR = "error";
                return 1;
            }
        }
    }
    //functions for output this should be the interface 
    //the Function return 0 means ok and return -1 means failed
}
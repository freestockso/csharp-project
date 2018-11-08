using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
namespace DzhSample
{
    public enum DATA_TYPE
    {
        TICK_DATA = 2,				//分笔成交
        MIN1_DATA,					//1分钟线
        MIN5_DATA,					//5分钟线					
        MIN15_DATA,					//15分钟线
        MIN30_DATA,					//30分钟线
        MIN60_DATA,					//60分钟线
        DAY_DATA,					//日线
        WEEK_DATA,					//周线
        MONTH_DATA,					//月线
        MULTI_DATA					//多日线
    }

    ///////////////////////////////////////////////////////////////////////////
    //基本数据

    public struct STKDATA
    {
        public Int32 m_time;
        //time_t	m_time;			//时间,UCT
        public float m_fOpen;		//开盘
        public float m_fHigh;		//最高
        public float m_fLow;			//最低
        public float m_fClose;		//收盘
        public float m_fVolume;		//成交量
        public float m_fAmount;		//成交额
        public UInt16 m_wAdvance;		//上涨家数(仅大盘有效)
        public UInt16 m_wDecline;		//下跌家数(仅大盘有效)
    }


    ////////////////////////////////////////////////////////////////////////////
    //扩展数据,用于描述分笔成交数据的买卖盘
    /*
    typedef union tagSTKDATAEx
    {
        struct
        {
            float m_fBuyPrice[3];		//买1--买3价
            float m_fBuyVol[3];			//买1--买3量
            float m_fSellPrice[3];		//卖1--卖3价	
            float m_fSellVol[3];		//卖1--卖3量
        };
        float m_fDataEx[12];			//保留
    } STKDATAEx;
    */
    /*
    public struct STKDATAEx
    {
        //大智慧的这个数据是不对的.
        public float m_fBuyPrice1;		//买1--买3价
        public float m_fBuyPrice2;		//买1--买3价
        public float m_fBuyPrice3;		//买1--买3价
        public float m_fBuyVol1;	    //买1--买3量
        public float m_fBuyVol2;		//买1--买3量
        public float m_fBuyVol3;		//买1--买3量
        public float m_fSellVol2;		//卖1--卖3量
        public float m_fSellVol3;		//卖1--卖3量
        public float m_fSellPrice3;		//卖1--卖3价	
        public float m_fSellVol1;		//卖1--卖3量
        public float m_fSellPrice1;		//卖1--卖3价	
        public float m_fSellPrice2;		//卖1--卖3价	
    }
     * */
    public struct STKDATAEx
    {
        //大智慧的这个数据是不对的.
        public float m_fBuyPrice1;
        public float m_fBuyPrice2;
        public float m_fBuyPrice3;
        public float m_fBuyPrice4;
        public float m_fBuyPrice5;
        public float m_fBuyVol1;
        public float m_fBuyVol2;
        public float m_fBuyVol3;
        public float m_fBuyVol4;
        public float m_fBuyVol5;
        public float m_fSellPrice1;
        public float m_fSellPrice2;
        public float m_fSellPrice3;
        public float m_fSellPrice4;
        public float m_fSellPrice5;
        public float m_fSellVol1;
        public float m_fSellVol2;
        public float m_fSellVol3;
        public float m_fSellVol4;
        public float m_fSellVol5;
    }
    /////////////////////////////////////////////////////////////////////////////
    /*财务数据顺序(m_pfFinData内容)

        序号	内容

        0	总股本(万股),
        1	国家股,
        2	发起人法人股,
        3	法人股,
        4	B股,
        5	H股,
        6	流通A股,
        7	职工股,
        8	A2转配股,
        9	总资产(千元),
        10	流动资产,
        11	固定资产,
        12	无形资产,
        13	长期投资,
        14	流动负债,
        15	长期负债,
        16	资本公积金,
        17	每股公积金,
        18	股东权益,
        19	主营收入,
        20	主营利润,
        21	其他利润,
        22	营业利润,
        23	投资收益,
        24	补贴收入,
        25	营业外收支,
        26	上年损益调整,
        27	利润总额,
        28	税后利润,
        29	净利润,
        30	未分配利润,
        31	每股未分配,
        32	每股收益,
        33	每股净资产,
        34	调整每股净资,
        35	股东权益比,
        36	净资收益率
    */

    /////////////////////////////////////////////////////////////////////////////
    //函数数据结构

    public struct CALCINFO_SHARP
    {
        public UInt32 m_dwSize;				//结构大小
        public UInt32 m_dwVersion;			//调用软件版本(V2.10 : 0x210)
        public UInt32 m_dwSerial;				//调用软件序列号
        public Int32 m_strStkLabel;			//股票代码
        public Int32 m_bIndex;				//大盘

        public Int32 m_nNumData;				//数据数量(pData,pDataEx,pResultBuf数据数量)
        public Int32 m_pData;				//常规数据,注意:当m_nNumData==0时可能为 NULL
        public Int32 m_pDataEx;				//扩展数据,分笔成交买卖盘,注意:可能为 NULL

        public Int32 m_nParam1Start;			//参数1有效位置
        public Int32 m_pfParam1;				//调用参数1	
        public Int32 m_pfParam2;				//调用参数2
        public Int32 m_pfParam3;				//调用参数3
        public Int32 m_pfParam4;				//调用参数3

        public Int32 m_pResultBuf;			//结果缓冲区
        public Int32 m_dataType;				//数据类型
        public Int32 m_pfFinData;			//财务数据
    }
    /* 
    注: 
        1.函数调用参数由m_pfParam1--m_pfParam4带入,若为NULL则表示该参数无效.
        2.当一个参数无效时,则其后的所有参数均无效.
            如:m_pfParam2为NULL,则m_pfParam3,m_pfParam4一定为NULL.
        3.参数1可以是常数参数或序列数参数,其余参数只能为常数参数.
        4.若m_nParam1Start<0, 则参数1为常数参数,参数等于*m_pfParam1;
        5.若m_nParam1Start>=0,则参数1为序列数参数,m_pfParam1指向一个浮点型数组,
            数组大小为m_nNumData,数据有效范围为m_nParam1Start--m_nNumData.
            在时间上m_pData[x] 与 m_pfParam1[x]是一致的
    */


    ///////////////////////////////////////////////////////////////////////////////////
    /* 函数输出

    __declspec(dllexport) int xxxxxxxx(CALCINFO* pData);	---------- A
    __declspec(dllexport) int xxxxxxxxVAR(CALCINDO* pData);	---------- B

    1.函数名称需全部大写.
    2.函数必须以上述A,B两种形式之一声明,请用实际函数名称替代xxxxxxxx;
        对于C++程序还需包括在 extern "C" {   } 括号中.
    3.上述形式A用于声明不带参数或全部参数为常数的函数;
        形式B用于声明参数1为序列数的函数;两种函数的区别在于后者以VAR结尾.
    4.函数计算结果用pData->m_pResultBuf带回.
    5.函数返回-1表示错误或全部数据无效,否则返回第一个有效值位置,即:
        m_pResultBuf[返回值] -- m_pResultBuf[m_nNumData-1]间为有效值.
    6.函数名称长度不能超过15字节,动态连接库文件名不能超过9字节(不包括扩展名),动态库名称不能叫SYSTEM,EXPLORER
    7.编译时请请选择1字节对齐

    */

}
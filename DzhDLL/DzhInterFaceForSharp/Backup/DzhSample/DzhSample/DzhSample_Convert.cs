using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
namespace DzhSample
{
    public partial class CDzhSample
    {
        //////////////
        //以下工作是将C++结构的指针拆解成C#的数据类型
        //有很多方法,这是其中一种.
        //也可以将拆解放在C++的DzhCLI.DLL中.
        string _GetStockCode(ref CALCINFO_SHARP pCalcInfoSharp)
        {
            //返回股票代码
            if (pCalcInfoSharp.m_strStkLabel == 0)
                return "";
            IntPtr hHandle = new IntPtr(pCalcInfoSharp.m_strStkLabel);
            byte[] bStockCode = new byte[12];
            Marshal.Copy(hHandle, bStockCode, 0, 10);
            int i;
            for (i = 0; i < 10; i++)
            {
                if (bStockCode[i] == 0)
                    break;
            }
            return System.Text.Encoding.Default.GetString(bStockCode, 0, i);
        }
        float[][] _GetParam(ref CALCINFO_SHARP pCalcInfoSharp)
        {
            //返回4个参数
            IntPtr hHandle = new IntPtr(pCalcInfoSharp.m_strStkLabel);
            float[][] fParam = new float[4][];
            if (pCalcInfoSharp.m_pfParam1 != 0)
            {
                hHandle = new IntPtr(pCalcInfoSharp.m_pfParam1);
                if (pCalcInfoSharp.m_nParam1Start >= 0)
                {
                    fParam[0] = new float[pCalcInfoSharp.m_nNumData];
                    Marshal.Copy(hHandle, fParam[0], pCalcInfoSharp.m_nParam1Start, pCalcInfoSharp.m_nNumData - pCalcInfoSharp.m_nParam1Start);
                }
                else
                {
                    fParam[0] = new float[1];
                    Marshal.Copy(hHandle, fParam[0], 0, 1);
                }
            }
            if (pCalcInfoSharp.m_pfParam2 != 0)
            {
                fParam[1] = new float[1];
                hHandle = new IntPtr(pCalcInfoSharp.m_pfParam2);
                Marshal.Copy(hHandle, fParam[1], 0, 1);
            }
            if (pCalcInfoSharp.m_pfParam3 != 0)
            {
                fParam[2] = new float[1];
                hHandle = new IntPtr(pCalcInfoSharp.m_pfParam3);
                Marshal.Copy(hHandle, fParam[2], 0, 1);
            }
            if (pCalcInfoSharp.m_pfParam4 != 0)
            {
                fParam[3] = new float[1];
                hHandle = new IntPtr(pCalcInfoSharp.m_pfParam4);
                Marshal.Copy(hHandle, fParam[3], 0, 1);
            }
            return fParam;
        }
        STKDATA[] _GetStockData(ref CALCINFO_SHARP pCalcInfoSharp)
        {
            //这是正常的数据,也就是固定时间间隔的数据,有开盘价,最高价等.
            //日线,周线,分时等都是这种数据
            if (pCalcInfoSharp.m_pData == 0)
                return null;
            IntPtr hHandle;
            STKDATA[] pStkData = new STKDATA[pCalcInfoSharp.m_nNumData];
            int i;
            for (i = 0; i < pCalcInfoSharp.m_nNumData; i++)
            {
                hHandle = new IntPtr(pCalcInfoSharp.m_pData + i * Marshal.SizeOf(typeof(STKDATA)));
                pStkData[i] = (STKDATA)Marshal.PtrToStructure(hHandle, typeof(STKDATA));
            }
            return pStkData;
        }
        STKDATAEx[] _GetStockDataEx(ref CALCINFO_SHARP pCalcInfoSharp)
        {
            if (pCalcInfoSharp.m_pDataEx == 0)
                return null;
            IntPtr hHandle;
            STKDATAEx[] pStkDataEx = new STKDATAEx[pCalcInfoSharp.m_nNumData];
            int i;
            for (i = 0; i < pCalcInfoSharp.m_nNumData; i++)
            {
                hHandle = new IntPtr(pCalcInfoSharp.m_pDataEx + i * Marshal.SizeOf(typeof(STKDATAEx)));
                pStkDataEx[i] = (STKDATAEx)Marshal.PtrToStructure(hHandle, typeof(STKDATAEx));
            }
            return pStkDataEx;
        }
        float[] _GetFinData(ref CALCINFO_SHARP pCalcInfoSharp)
        {
            if (pCalcInfoSharp.m_pfFinData == 0)
                return null;
            float[] fFinData = new float[37];//大智慧财务数据个数
            IntPtr hHandle = new IntPtr(pCalcInfoSharp.m_pfFinData);
            Marshal.Copy(hHandle, fFinData, 0, 37);
            return fFinData;
        }
        int _SetResultData(ref CALCINFO_SHARP pCalcInfoSharp, float[] fResult)
        {
            //将计算结果回送到大智慧
            if (pCalcInfoSharp.m_pResultBuf == 0)
                return -1;
            if (fResult == null || fResult.Length == 0)
                return -1;
            IntPtr hHandle = new IntPtr(pCalcInfoSharp.m_pResultBuf);
            Marshal.Copy(fResult, 0, hHandle, fResult.Length);
            Marshal.Copy(hHandle, fResult, 0, fResult.Length);
            return fResult.Length;
        }
    }
}
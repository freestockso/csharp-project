using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices; 
namespace DzhSample
{
    public partial class CDzhSample
    {
        ////////////
        //计算平均值.
        //大智慧公式语法:p1:= "DzhCLI @ AVERAGEVAR"(5);//5日平均
        public Int32 AVERAGEVAR(Int32 pCalcInfo)
        {
            if (pCalcInfo == 0)
                return -1;            
            IntPtr hHandle = new IntPtr(pCalcInfo);
            CALCINFO_SHARP pCalcInfoSharp = (CALCINFO_SHARP)Marshal.PtrToStructure(hHandle, typeof(CALCINFO_SHARP));
            if(pCalcInfoSharp.m_nNumData == 0)
                return -1;
            //4个参数
            float[][] fParam = _GetParam(ref pCalcInfoSharp);
            Int32 nAverageNumber = 1;
            if (fParam[0] != null)//平均天数由第一个参数传入
                nAverageNumber = (Int32)fParam[0][0];
            if (nAverageNumber <= 0)
                nAverageNumber = 1;
            if (nAverageNumber > pCalcInfoSharp.m_nNumData)
                return -1;
            float[] fResult = new float[pCalcInfoSharp.m_nNumData];
            if (pCalcInfoSharp.m_dataType == (Int32)DATA_TYPE.TICK_DATA)
            {
                //分笔数据,这里的样本对买一价格平均
                //STKDATAEx里大智慧的这些数据是不对的.已经修改
                //在C++DLL里监视也是不对的.
                STKDATAEx[] pStkDataEx = _GetStockDataEx(ref pCalcInfoSharp);
                if (pStkDataEx == null)
                    return -1;
                int i , j;
                for (i = nAverageNumber - 1; i < pCalcInfoSharp.m_nNumData; i++)
                {
                    fResult[i] = 0;
                    for (j = 0; j < nAverageNumber; j++)
                        fResult[i] += pStkDataEx[i - j].m_fBuyPrice1 ;
                    fResult[i] = fResult[i] / (float)nAverageNumber;
                }
            }
            else
            {
                //时间序列数据,这里的样本对收盘价平均
                STKDATA[] pStkData = _GetStockData(ref pCalcInfoSharp);
                if (pStkData == null)
                    return -1;
                int i, j;
                for (i = nAverageNumber - 1; i < pCalcInfoSharp.m_nNumData; i++)
                {
                    fResult[i] = 0;
                    for (j = 0; j < nAverageNumber; j++)
                        fResult[i] += pStkData[i - j].m_fClose;
                    fResult[i] = fResult[i] / (float)nAverageNumber;
                }
            }
            _SetResultData(ref pCalcInfoSharp, fResult);
            return nAverageNumber - 1;//返回第一个有效数据开始的位置
        }
        ////////////
        //计算平均值差.
        //大智慧公式语法:p1:= "DzhCLI @ AVERAGEDIFFVAR"(5,12);//5日平均 - 12日平均
        public Int32 AVERAGEDIFFVAR(Int32 pCalcInfo)
        {
            if (pCalcInfo == 0)
                return -1;
            IntPtr hHandle = new IntPtr(pCalcInfo);
            CALCINFO_SHARP pCalcInfoSharp = (CALCINFO_SHARP)Marshal.PtrToStructure(hHandle, typeof(CALCINFO_SHARP));
            if (pCalcInfoSharp.m_nNumData == 0)
                return -1;
            //4个参数
            float[][] fParam = _GetParam(ref pCalcInfoSharp);
            Int32 nAverageNumber1 = 1, nAverageNumber2 = 1;
            if (fParam[0] != null)//第一个平均数由第一个参数传入
                nAverageNumber1 = (Int32)fParam[0][0];
            if (nAverageNumber1 <= 0)
                nAverageNumber1 = 1;
            if (nAverageNumber1 > pCalcInfoSharp.m_nNumData)
                return -1;

            if (fParam[1] != null)//第二个平均数由第二个参数传入
                nAverageNumber2 = (Int32)fParam[1][0];
            if (nAverageNumber2 <= 0)
                nAverageNumber2 = 1;
            if (nAverageNumber2 > pCalcInfoSharp.m_nNumData)
                return -1;
            if (nAverageNumber1 == nAverageNumber2)
                return -1;

            float[] fResult = new float[pCalcInfoSharp.m_nNumData];
            if (pCalcInfoSharp.m_dataType == (Int32)DATA_TYPE.TICK_DATA)
            {
                //分笔数据,这里的样本对买一价格平均
                //STKDATAEx里大智慧的这些数据是不对的.已经修改
                //在C++DLL里监视也是不对的.
                STKDATAEx[] pStkDataEx = _GetStockDataEx(ref pCalcInfoSharp);
                if (pStkDataEx == null)
                    return -1;
                int i, j;
                float f1, f2;
                if (nAverageNumber1 > nAverageNumber2)
                    i = nAverageNumber1 - 1;
                else
                    i = nAverageNumber2 - 1;
                for (; i < pCalcInfoSharp.m_nNumData; i++)
                {
                    f1 = 0;
                    f2 = 0;
                    for (j = 0; j < nAverageNumber1; j++)
                        f1 += pStkDataEx[i - j].m_fBuyPrice1 ;
                    f1 /= (float)nAverageNumber1;
                    for (j = 0; j < nAverageNumber2; j++)
                        f2 += pStkDataEx[i - j].m_fBuyPrice1 ;
                    f2 /= (float)nAverageNumber2;
                    fResult[i] = f1 - f2;
                }
            }
            else
            {
                //时间序列数据,这里的样本对收盘价平均
                STKDATA[] pStkData = _GetStockData(ref pCalcInfoSharp);
                if (pStkData == null)
                    return -1;
                int i, j;
                float f1, f2;
                if (nAverageNumber1 > nAverageNumber2)
                    i = nAverageNumber1 - 1;
                else
                    i = nAverageNumber2 - 1;
                for (; i < pCalcInfoSharp.m_nNumData; i++)
                {
                    f1 = 0;
                    f2 = 0;
                    for (j = 0; j < nAverageNumber1; j++)
                        f1 += pStkData[i - j].m_fClose;
                    f1 /= (float)nAverageNumber1;
                    for (j = 0; j < nAverageNumber2; j++)
                        f2 += pStkData[i - j].m_fClose;
                    f2 /= (float)nAverageNumber2;
                    fResult[i] = f1 - f2;
                }
            }
            _SetResultData(ref pCalcInfoSharp, fResult);
            if (nAverageNumber1 > nAverageNumber2)
                return nAverageNumber1 - 1;//返回第一个有效数据开始的位置
            return nAverageNumber2 - 1;//返回第一个有效数据开始的位置
        }
        ////////////
        //计算MACD.
        //大智慧公式语法:p1:= "DzhCLI @ MACDVAR"(12,26,9);//12日平均 - 26日平均,对差值再做成9天平均
        //这个算法与大智慧自带的数值不一样,形状类似,不知道是否与EMA算法有关.
        //演示性质,不推敲细节了
        public Int32 MACDVAR(Int32 pCalcInfo)
        {
            if (pCalcInfo == 0)
                return -1;
            IntPtr hHandle = new IntPtr(pCalcInfo);
            CALCINFO_SHARP pCalcInfoSharp = (CALCINFO_SHARP)Marshal.PtrToStructure(hHandle, typeof(CALCINFO_SHARP));
            if (pCalcInfoSharp.m_nNumData == 0)
                return -1;
            //4个参数
            float[][] fParam = _GetParam(ref pCalcInfoSharp);
            Int32 nAverageNumber1 = 1, nAverageNumber2 = 1, nAverageNumber3 = 1; ;
            if (fParam[0] != null)//第一个平均数由第一个参数传入
                nAverageNumber1 = (Int32)fParam[0][0];
            if (nAverageNumber1 <= 0)
                nAverageNumber1 = 1;

            if (fParam[1] != null)//第二个平均数由第二个参数传入
                nAverageNumber2 = (Int32)fParam[1][0];
            if (nAverageNumber2 <= 0)
                nAverageNumber2 = 1;

            if (fParam[2] != null)//第三个平均数由第三个参数传入
                nAverageNumber3 = (Int32)fParam[1][0];
            if (nAverageNumber3 <= 0)
                nAverageNumber3 = 1;

            if (nAverageNumber1 == nAverageNumber2)
                return -1;
            if ((nAverageNumber1 + nAverageNumber3) > pCalcInfoSharp.m_nNumData)
                return -1;
            if ((nAverageNumber1 + nAverageNumber3) > pCalcInfoSharp.m_nNumData)
                return -1;

            float[] fResult = new float[pCalcInfoSharp.m_nNumData];
            float[] fMacd = new float[pCalcInfoSharp.m_nNumData];
            //时间序列数据,这里的样本对收盘价平均
            STKDATA[] pStkData = _GetStockData(ref pCalcInfoSharp);
            if (pStkData == null)
                return -1;
            int i, j;
            float f1, f2;
            if (nAverageNumber1 > nAverageNumber2)
                i = nAverageNumber1 - 1;
            else
                i = nAverageNumber2 - 1;
            for (; i < pCalcInfoSharp.m_nNumData; i++)
            {
                f1 = 0;
                f2 = 0;
                for (j = 0; j < nAverageNumber1; j++)
                    f1 += pStkData[i - j].m_fClose;
                f1 /= (float)nAverageNumber1;
                for (j = 0; j < nAverageNumber2; j++)
                    f2 += pStkData[i - j].m_fClose;
                f2 /= (float)nAverageNumber2;
                fResult[i] = f1 - f2;
            }
            if (nAverageNumber1 > nAverageNumber2)
                i = nAverageNumber1 + nAverageNumber3 - 1;
            else
                i = nAverageNumber2 + nAverageNumber3 - 1;
            for (; i < pCalcInfoSharp.m_nNumData; i++)
            {
                f1 = 0;
                for (j = 0; j < nAverageNumber3; j++)
                    fMacd[ i ] += fResult[i - j];
                fMacd[i] /= (float)nAverageNumber3;
            }
            for (i = 0; i < pCalcInfoSharp.m_nNumData; i++)
                fMacd[i] = fResult[i] - fMacd[i];

            _SetResultData(ref pCalcInfoSharp, fMacd);
            if (nAverageNumber1 > nAverageNumber2)
                return nAverageNumber1 + nAverageNumber3 - 1;//返回第一个有效数据开始的位置
            return nAverageNumber2 + nAverageNumber3 - 1;//返回第一个有效数据开始的位置
        }
        ////////////
        //将下面的代码连同Form文件夹全部删除,就没有Form了,不影响正常的公式输出
        //显示一个窗口.
        //大智慧公式语法:p1:= "DzhCLI @ SHOWFORMVAR";
        static Form1 m_pForm = null;
        public Int32 SHOWFORMVAR(Int32 pCalcInfo)
        {
            if (pCalcInfo == 0)
                return -1;
            IntPtr hHandle = new IntPtr(pCalcInfo);
            CALCINFO_SHARP pCalcInfoSharp = (CALCINFO_SHARP)Marshal.PtrToStructure(hHandle, typeof(CALCINFO_SHARP));
            if (pCalcInfoSharp.m_nNumData == 0)
                return -1;
            if (m_pForm == null)
            {
                m_pForm = new Form1();
                m_pForm.Show();
            }
            if (m_pForm.IsClose())
                return -1;
            string sStockCode = _GetStockCode(ref pCalcInfoSharp);
            if (m_pForm.IsRefreshData(sStockCode, pCalcInfoSharp.m_dataType, pCalcInfoSharp.m_nNumData))
            {
                if (m_pForm.GetShowType() == SHOW_TYPE.Tick)
                {
                    STKDATA[] pStkData = _GetStockData(ref pCalcInfoSharp);
                    STKDATAEx[] pStkDataEx = _GetStockDataEx(ref pCalcInfoSharp);
                    m_pForm.RefreshData(ref pStkData, ref pStkDataEx);
                }
                else if (m_pForm.GetShowType() == SHOW_TYPE.Fin)
                {
                    float[] pFinData = _GetFinData(ref pCalcInfoSharp);
                    m_pForm.RefreshData(ref pFinData);
                }
                else //if (m_pForm.GetShowType() == SHOW_TYPE.Time)
                {
                    STKDATA[] pStkData = _GetStockData(ref pCalcInfoSharp);
                    m_pForm.RefreshData(ref pStkData);
                }
            }
            return -1;
        }
        
    }
}

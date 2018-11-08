using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;

namespace MyTable
{
    public partial class _Table
    {
        public int Sort(int nColIndex , int nDirect)
        {
            if (_IsValidIndexOfColumn(nColIndex) < 0)
                return -1;
            if(m_pColumnParam[nColIndex].nType == _TypeToInt(typeof (Int32)))
                return _Sort_Int32(0, m_pTableParam.nRowCount - 1, nDirect , ref m_pData, m_pTableParam.nBytesPerRow, m_pColumnParam[nColIndex].nSize, m_pColumnParam[nColIndex].nOffset);
            else if (m_pColumnParam[nColIndex].nType == _TypeToInt(typeof(Int64)))
                return _Sort_Int64(0, m_pTableParam.nRowCount - 1, nDirect, ref m_pData, m_pTableParam.nBytesPerRow, m_pColumnParam[nColIndex].nSize, m_pColumnParam[nColIndex].nOffset);
            else if (m_pColumnParam[nColIndex].nType == _TypeToInt(typeof(Single)))
                return _Sort_Float(0, m_pTableParam.nRowCount - 1, nDirect, ref m_pData, m_pTableParam.nBytesPerRow, m_pColumnParam[nColIndex].nSize, m_pColumnParam[nColIndex].nOffset);
            return _Sort_Bytes(0, m_pTableParam.nRowCount - 1, nDirect, ref m_pData, m_pTableParam.nBytesPerRow, m_pColumnParam[nColIndex].nSize, m_pColumnParam[nColIndex].nOffset);
        }
        int _Sort_Bytes(int nFirstRow, int nLastRow, int nDirect, ref byte[] pData, int nBytesPerRow, int nColumnSize, int nColumnOffset)
        {
            if (nFirstRow >= nLastRow)
                return 0;

            int nPivotPos, nPivotRow, nStartRow, nEndRow, nStartPos, nEndPos;
            int nFlag, pRate;
            int i, j, k, n1, n2;
            if (nDirect == 0)//down
                pRate = 1;
            else
                pRate = -1;
            byte b;
            byte[] bPivot = new byte[nColumnSize + 1];

            int nExistExhange = 0;
            nStartRow = nFirstRow;
            nEndRow = nLastRow;
            nPivotRow = (nFirstRow + nLastRow) / 2;
            nStartPos = nStartRow * nBytesPerRow + m_pTableParam.nColumnCount * m_nStructLenOfColumn + m_nStructLenOfTable ;
            nEndPos = nEndRow * nBytesPerRow + m_pTableParam.nColumnCount * m_nStructLenOfColumn + m_nStructLenOfTable;
            nPivotPos = nPivotRow * nBytesPerRow + m_pTableParam.nColumnCount * m_nStructLenOfColumn + m_nStructLenOfTable;
            j = nPivotPos + nColumnOffset;
            for (i = 0; i < nColumnSize; i++, j++)
                bPivot[i] = pData[j];
            nFlag = 0;
            while (nStartPos < nEndPos)
            {
                if (nFlag == 1)
                {
                    if (nEndPos == nPivotPos)
                        nFlag = 0;
                    j = nEndPos + nColumnOffset;
                    for (i = 0; i < nColumnSize; i++, j++)
                    {
                        n1 = pRate * pData[j];
                        n2 = pRate * bPivot[i];
                        if (n1 < n2)
                        {
                            //交换行

                            k = nPivotPos;
                            j = nEndPos;
                            for (i = 0; i < nBytesPerRow; i++, j++, k++)
                            {
                                b = pData[k];
                                pData[k] = pData[j];
                                pData[j] = b;
                            }
                            //					OnSortExchangeRow(nPivotRow, nEndRow);
                            nPivotPos = nEndPos;
                            nPivotRow = nEndRow;
                            nFlag = 0;
                            nExistExhange = 1;
                            break;
                        }
                        if (n1 > n2)
                            break;
                    }
                    nEndPos -= nBytesPerRow;
                    nEndRow--;
                }
                else
                {
                    if (nStartPos == nPivotPos)
                        nFlag = 1;
                    j = nStartPos + nColumnOffset;
                    for (i = 0; i < nColumnSize; i++, j++)
                    {
                        n1 = pRate * pData[j];
                        n2 = pRate * bPivot[i];
                        if (n1 > n2)
                        {
                            //交换行

                            k = nPivotPos;
                            j = nStartPos;
                            for (i = 0; i < nBytesPerRow; i++, j++, k++)
                            {
                                b = pData[k];
                                pData[k] = pData[j];
                                pData[j] = b;
                            }
                            //					OnSortExchangeRow(nPivotRow, nStartRow);
                            nPivotPos = nStartPos;
                            nPivotRow = nStartRow;
                            nFlag = 1;
                            nExistExhange = 1;
                            break;
                        }
                        if (n1 < n2)
                            break;
                    }
                    nStartPos += nBytesPerRow;
                    nStartRow++;
                }

                if (nStartPos == nEndPos)
                {
                    if (nPivotPos < nStartPos)
                    {
                        j = nStartPos + nColumnOffset;
                        for (i = 0; i < nColumnSize; i++, j++)
                        {
                            n1 = pRate * pData[j];
                            n2 = pRate * bPivot[i];
                            if (n1 < n2)
                            {
                                //交换行
                                k = nPivotPos;
                                j = nStartPos;
                                for (i = 0; i < nBytesPerRow; i++, j++, k++)
                                {
                                    b = pData[k];
                                    pData[k] = pData[j];
                                    pData[j] = b;
                                }
                                //						OnSortExchangeRow(nPivotRow, nStartRow);
                                nPivotPos = nStartPos;
                                nPivotRow = nStartRow;
                                nExistExhange = 1;
                                break;
                            }
                            if (n1 > n2)
                                break;
                        }
                    }
                    else if (nPivotPos > nStartPos)
                    {
                        j = nStartPos + nColumnOffset;
                        for (i = 0; i < nColumnSize; i++, j++)
                        {
                            n1 = pRate * pData[j];
                            n2 = pRate * bPivot[i];
                            if (n1 > n2)
                            {
                                //交换行
                                k = nPivotPos;
                                j = nStartPos;
                                for (i = 0; i < nBytesPerRow; i++, j++, k++)
                                {
                                    b = pData[k];
                                    pData[k] = pData[j];
                                    pData[j] = b;
                                }
                                //						OnSortExchangeRow(nPivotRow, nStartRow);
                                nPivotPos = nStartPos;
                                nPivotRow = nStartRow;
                                nExistExhange = 1;
                                break;
                            }
                            if (n1 < n2)
                                break;
                        }
                    }
                }
            }
            nStartRow = nFirstRow;
            nEndRow = nPivotRow - 1;
            if (nStartRow < nEndRow)
                nExistExhange |= _Sort_Bytes(nStartRow, nEndRow, nDirect, ref pData, nBytesPerRow, nColumnSize, nColumnOffset);

            nStartRow = nPivotRow + 1;
            nEndRow = nLastRow;
            if (nStartRow < nEndRow)
                nExistExhange |= _Sort_Bytes(nStartRow, nEndRow, nDirect, ref pData, nBytesPerRow, nColumnSize, nColumnOffset);
            return nExistExhange;
        }
        int _Sort_Int32(int nFirstRow, int nLastRow, int nDirect, ref byte[] pData, int nBytesPerRow, int nColumnSize, int nColumnOffset)
        {
            if (nFirstRow >= nLastRow)
                return 0;

            int nPivotPos, nPivotRow, nStartRow, nEndRow, nStartPos, nEndPos;
            int nFlag, pRate;
            int i, j, k, n1, n2;
            if (nDirect == 0)//down
                pRate = 1;
            else
                pRate = -1;
            byte b;
            int nPivot32 , nData32;

            int nExistExhange = 0;
            nStartRow = nFirstRow;
            nEndRow = nLastRow;
            nPivotRow = (nFirstRow + nLastRow) / 2;
            nStartPos = nStartRow * nBytesPerRow + m_pTableParam.nColumnCount * m_nStructLenOfColumn + m_nStructLenOfTable;
            nEndPos = nEndRow * nBytesPerRow + m_pTableParam.nColumnCount * m_nStructLenOfColumn + m_nStructLenOfTable;
            nPivotPos = nPivotRow * nBytesPerRow + m_pTableParam.nColumnCount * m_nStructLenOfColumn + m_nStructLenOfTable;
            j = nPivotPos + nColumnOffset;
            nPivot32 = BitConverter.ToInt32(pData, j);
            //for (i = 0; i < nColumnSize; i++, j++)
            //    bPivot[i] = pData[j];
            nFlag = 0;
            while (nStartPos < nEndPos)
            {
                if (nFlag == 1)
                {
                    if (nEndPos == nPivotPos)
                        nFlag = 0;
                    j = nEndPos + nColumnOffset;
                    nData32 = BitConverter.ToInt32(pData, j);
                    n1 = pRate * nData32;
                    n2 = pRate * nPivot32;
                    if (n1 < n2)
                    {
                        //交换行
                        k = nPivotPos;
                        j = nEndPos;
                        for (i = 0; i < nBytesPerRow; i++, j++, k++)
                        {
                            b = pData[k];
                            pData[k] = pData[j];
                            pData[j] = b;
                        }
                        //					OnSortExchangeRow(nPivotRow, nEndRow);
                        nPivotPos = nEndPos;
                        nPivotRow = nEndRow;
                        nFlag = 0;
                        nExistExhange = 1;
                    }
                    nEndPos -= nBytesPerRow;
                    nEndRow--;
                }
                else
                {
                    if (nStartPos == nPivotPos)
                        nFlag = 1;
                    j = nStartPos + nColumnOffset;
                    nData32 = BitConverter.ToInt32(pData, j);
                    n1 = pRate * nData32;
                    n2 = pRate * nPivot32;
                    if (n1 > n2)
                    {
                        //交换行
                        k = nPivotPos;
                        j = nStartPos;
                        for (i = 0; i < nBytesPerRow; i++, j++, k++)
                        {
                            b = pData[k];
                            pData[k] = pData[j];
                            pData[j] = b;
                        }
                        //					OnSortExchangeRow(nPivotRow, nStartRow);
                        nPivotPos = nStartPos;
                        nPivotRow = nStartRow;
                        nFlag = 1;
                        nExistExhange = 1;
                    }
                    nStartPos += nBytesPerRow;
                    nStartRow++;
                }

                if (nStartPos == nEndPos)
                {
                    if (nPivotPos < nStartPos)
                    {
                        j = nStartPos + nColumnOffset;
                        nData32 = BitConverter.ToInt32(pData, j);
                        n1 = pRate * nData32;
                        n2 = pRate * nPivot32;
                        if (n1 < n2)
                        {
                            //交换行
                            k = nPivotPos;
                            j = nStartPos;
                            for (i = 0; i < nBytesPerRow; i++, j++, k++)
                            {
                                b = pData[k];
                                pData[k] = pData[j];
                                pData[j] = b;
                            }
                            //						OnSortExchangeRow(nPivotRow, nStartRow);
                            nPivotPos = nStartPos;
                            nPivotRow = nStartRow;
                            nExistExhange = 1;
                        }
                    }
                    else if (nPivotPos > nStartPos)
                    {
                        j = nStartPos + nColumnOffset;
                        nData32 = BitConverter.ToInt32(pData, j);
                        n1 = pRate * nData32;
                        n2 = pRate * nPivot32;
                        if (n1 > n2)
                        {
                            //交换行
                            k = nPivotPos;
                            j = nStartPos;
                            for (i = 0; i < nBytesPerRow; i++, j++, k++)
                            {
                                b = pData[k];
                                pData[k] = pData[j];
                                pData[j] = b;
                            }
                            //						OnSortExchangeRow(nPivotRow, nStartRow);
                            nPivotPos = nStartPos;
                            nPivotRow = nStartRow;
                            nExistExhange = 1;
                        }
                    }
                }
            }
            nStartRow = nFirstRow;
            nEndRow = nPivotRow - 1;
            if (nStartRow < nEndRow)
                nExistExhange |= _Sort_Int32(nStartRow, nEndRow, nDirect, ref pData, nBytesPerRow, nColumnSize, nColumnOffset);

            nStartRow = nPivotRow + 1;
            nEndRow = nLastRow;
            if (nStartRow < nEndRow)
                nExistExhange |= _Sort_Int32(nStartRow, nEndRow, nDirect, ref pData, nBytesPerRow, nColumnSize, nColumnOffset);
            return nExistExhange;
        }
        int _Sort_Int64(int nFirstRow, int nLastRow, int nDirect, ref byte[] pData, int nBytesPerRow, int nColumnSize, int nColumnOffset)
        {
            if (nFirstRow >= nLastRow)
                return 0;

            int nPivotPos, nPivotRow, nStartRow, nEndRow, nStartPos, nEndPos;
            int nFlag ;
            int i, j, k;
            Int64 n1, n2 , pRate;
            if (nDirect == 0)//down
                pRate = 1;
            else
                pRate = -1;
            byte b;
            Int64 nPivot64, nData64;

            int nExistExhange = 0;
            nStartRow = nFirstRow;
            nEndRow = nLastRow;
            nPivotRow = (nFirstRow + nLastRow) / 2;
            nStartPos = nStartRow * nBytesPerRow + m_pTableParam.nColumnCount * m_nStructLenOfColumn + m_nStructLenOfTable;
            nEndPos = nEndRow * nBytesPerRow + m_pTableParam.nColumnCount * m_nStructLenOfColumn + m_nStructLenOfTable;
            nPivotPos = nPivotRow * nBytesPerRow + m_pTableParam.nColumnCount * m_nStructLenOfColumn + m_nStructLenOfTable;
            j = nPivotPos + nColumnOffset;
            nPivot64 = BitConverter.ToInt64(pData, j);
            //for (i = 0; i < nColumnSize; i++, j++)
            //    bPivot[i] = pData[j];
            nFlag = 0;
            while (nStartPos < nEndPos)
            {
                if (nFlag == 1)
                {
                    if (nEndPos == nPivotPos)
                        nFlag = 0;
                    j = nEndPos + nColumnOffset;
                    nData64 = BitConverter.ToInt64(pData, j);
                    n1 = pRate * nData64;
                    n2 = pRate * nPivot64;
                    if (n1 < n2)
                    {
                        //交换行

                        k = nPivotPos;
                        j = nEndPos;
                        for (i = 0; i < nBytesPerRow; i++, j++, k++)
                        {
                            b = pData[k];
                            pData[k] = pData[j];
                            pData[j] = b;
                        }
                        //					OnSortExchangeRow(nPivotRow, nEndRow);
                        nPivotPos = nEndPos;
                        nPivotRow = nEndRow;
                        nFlag = 0;
                        nExistExhange = 1;
                    }
                    nEndPos -= nBytesPerRow;
                    nEndRow--;
                }
                else
                {
                    if (nStartPos == nPivotPos)
                        nFlag = 1;
                    j = nStartPos + nColumnOffset;
                    nData64 = BitConverter.ToInt64(pData, j);
                    n1 = pRate * nData64;
                    n2 = pRate * nPivot64;
                    if (n1 > n2)
                    {
                        //交换行

                        k = nPivotPos;
                        j = nStartPos;
                        for (i = 0; i < nBytesPerRow; i++, j++, k++)
                        {
                            b = pData[k];
                            pData[k] = pData[j];
                            pData[j] = b;
                        }
                        //					OnSortExchangeRow(nPivotRow, nStartRow);
                        nPivotPos = nStartPos;
                        nPivotRow = nStartRow;
                        nFlag = 1;
                        nExistExhange = 1;
                    }
                    nStartPos += nBytesPerRow;
                    nStartRow++;
                }
                if (nStartPos == nEndPos)
                {
                    if (nPivotPos < nStartPos)
                    {
                        j = nStartPos + nColumnOffset;
                        nData64 = BitConverter.ToInt64(pData, j);
                        n1 = pRate * nData64;
                        n2 = pRate * nPivot64;
                        if (n1 < n2)
                        {
                            //交换行
                            k = nPivotPos;
                            j = nStartPos;
                            for (i = 0; i < nBytesPerRow; i++, j++, k++)
                            {
                                b = pData[k];
                                pData[k] = pData[j];
                                pData[j] = b;
                            }
                            //						OnSortExchangeRow(nPivotRow, nStartRow);
                            nPivotPos = nStartPos;
                            nPivotRow = nStartRow;
                            nExistExhange = 1;
                        }
                    }
                    else if (nPivotPos > nStartPos)
                    {
                        j = nStartPos + nColumnOffset;
                        nData64 = BitConverter.ToInt64(pData, j);
                        n1 = pRate * nData64;
                        n2 = pRate * nPivot64;
                        if (n1 > n2)
                        {
                            //交换行
                            k = nPivotPos;
                            j = nStartPos;
                            for (i = 0; i < nBytesPerRow; i++, j++, k++)
                            {
                                b = pData[k];
                                pData[k] = pData[j];
                                pData[j] = b;
                            }
                            //						OnSortExchangeRow(nPivotRow, nStartRow);
                            nPivotPos = nStartPos;
                            nPivotRow = nStartRow;
                            nExistExhange = 1;
                        }
                    }
                }
            }
            nStartRow = nFirstRow;
            nEndRow = nPivotRow - 1;
            if (nStartRow < nEndRow)
                nExistExhange |= _Sort_Int64(nStartRow, nEndRow, nDirect, ref pData, nBytesPerRow, nColumnSize, nColumnOffset);

            nStartRow = nPivotRow + 1;
            nEndRow = nLastRow;
            if (nStartRow < nEndRow)
                nExistExhange |= _Sort_Int64(nStartRow, nEndRow, nDirect, ref pData, nBytesPerRow, nColumnSize, nColumnOffset);
            return nExistExhange;
        }
        int _Sort_Float(int nFirstRow, int nLastRow, int nDirect, ref byte[] pData, int nBytesPerRow, int nColumnSize, int nColumnOffset)
        {
            if (nFirstRow >= nLastRow)
                return 0;

            int nPivotPos, nPivotRow, nStartRow, nEndRow, nStartPos, nEndPos;
            int nFlag, pRate;
            int i, j, k;
            if (nDirect == 0)//down
                pRate = 1;
            else
                pRate = -1;
            byte b;
            float n1, n2;
            float fPivot32, fData32;

            int nExistExhange = 0;
            nStartRow = nFirstRow;
            nEndRow = nLastRow;
            nPivotRow = (nFirstRow + nLastRow) / 2;
            nStartPos = nStartRow * nBytesPerRow + m_pTableParam.nColumnCount * m_nStructLenOfColumn + m_nStructLenOfTable;
            nEndPos = nEndRow * nBytesPerRow + m_pTableParam.nColumnCount * m_nStructLenOfColumn + m_nStructLenOfTable;
            nPivotPos = nPivotRow * nBytesPerRow + m_pTableParam.nColumnCount * m_nStructLenOfColumn + m_nStructLenOfTable;
            j = nPivotPos + nColumnOffset;
            fPivot32 = BitConverter.ToSingle(pData, j);
            //for (i = 0; i < nColumnSize; i++, j++)
            //    bPivot[i] = pData[j];
            nFlag = 0;
            while (nStartPos < nEndPos)
            {
                if (nFlag == 1)
                {
                    if (nEndPos == nPivotPos)
                        nFlag = 0;
                    j = nEndPos + nColumnOffset;
                    fData32 = BitConverter.ToSingle(pData, j);
                    n1 = pRate * fData32;
                    n2 = pRate * fPivot32;
                    if (n1 < n2)
                    {
                        //交换行

                        k = nPivotPos;
                        j = nEndPos;
                        for (i = 0; i < nBytesPerRow; i++, j++, k++)
                        {
                            b = pData[k];
                            pData[k] = pData[j];
                            pData[j] = b;
                        }
                        //					OnSortExchangeRow(nPivotRow, nEndRow);
                        nPivotPos = nEndPos;
                        nPivotRow = nEndRow;
                        nFlag = 0;
                        nExistExhange = 1;
                    }
                    nEndPos -= nBytesPerRow;
                    nEndRow--;
                }
                else
                {
                    if (nStartPos == nPivotPos)
                        nFlag = 1;
                    j = nStartPos + nColumnOffset;
                    fData32 = BitConverter.ToSingle(pData, j);
                    n1 = pRate * fData32;
                    n2 = pRate * fPivot32;
                    if (n1 > n2)
                    {
                        //交换行

                        k = nPivotPos;
                        j = nStartPos;
                        for (i = 0; i < nBytesPerRow; i++, j++, k++)
                        {
                            b = pData[k];
                            pData[k] = pData[j];
                            pData[j] = b;
                        }
                        //					OnSortExchangeRow(nPivotRow, nStartRow);
                        nPivotPos = nStartPos;
                        nPivotRow = nStartRow;
                        nFlag = 1;
                        nExistExhange = 1;
                    }
                    nStartPos += nBytesPerRow;
                    nStartRow++;
                }

                if (nStartPos == nEndPos)
                {
                    if (nPivotPos < nStartPos)
                    {
                        j = nStartPos + nColumnOffset;
                        fData32 = BitConverter.ToSingle(pData, j);
                        n1 = pRate * fData32;
                        n2 = pRate * fPivot32;
                        if (n1 < n2)
                        {
                            //交换行
                            k = nPivotPos;
                            j = nStartPos;
                            for (i = 0; i < nBytesPerRow; i++, j++, k++)
                            {
                                b = pData[k];
                                pData[k] = pData[j];
                                pData[j] = b;
                            }
                            //						OnSortExchangeRow(nPivotRow, nStartRow);
                            nPivotPos = nStartPos;
                            nPivotRow = nStartRow;
                            nExistExhange = 1;
                        }
                    }
                    else if (nPivotPos > nStartPos)
                    {
                        j = nStartPos + nColumnOffset;
                        fData32 = BitConverter.ToSingle(pData, j);
                        n1 = pRate * fData32;
                        n2 = pRate * fPivot32;
                        if (n1 > n2)
                        {
                            //交换行
                            k = nPivotPos;
                            j = nStartPos;
                            for (i = 0; i < nBytesPerRow; i++, j++, k++)
                            {
                                b = pData[k];
                                pData[k] = pData[j];
                                pData[j] = b;
                            }
                            //						OnSortExchangeRow(nPivotRow, nStartRow);
                            nPivotPos = nStartPos;
                            nPivotRow = nStartRow;
                            nExistExhange = 1;
                        }
                    }
                }
            }
            nStartRow = nFirstRow;
            nEndRow = nPivotRow - 1;
            if (nStartRow < nEndRow)
                nExistExhange |= _Sort_Float(nStartRow, nEndRow, nDirect, ref pData, nBytesPerRow, nColumnSize, nColumnOffset);

            nStartRow = nPivotRow + 1;
            nEndRow = nLastRow;
            if (nStartRow < nEndRow)
                nExistExhange |= _Sort_Float(nStartRow, nEndRow, nDirect, ref pData, nBytesPerRow, nColumnSize, nColumnOffset);
            return nExistExhange;
        }
    }
}
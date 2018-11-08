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
        //add and delete row
        public int AddRow()
        {
            return AddRow(-1, 1);
        }
        public int AddRow(int nRowCount)
        {
            return AddRow(-1, nRowCount);
        }
        public int AddRow(int nRowIndex, int nRowCount)
        {
            if (m_pTableParam.nColumnCount == 0)
            {
                m_sErrorInfo = "Table Have Not Column";
                return -1;
            }
            if (nRowIndex < 0)
                nRowIndex = m_pTableParam.nRowCount;
            if (nRowCount < 1)
                nRowCount = 1;
            int nAddLen = nRowCount * m_pTableParam.nBytesPerRow;
            if ((m_nDataLen + nAddLen) > m_pData.Length)
            {
                int nLen = m_nDataLen + nAddLen + m_pTableParam.nBytesPerRow * 1000;
                try
                {
                    Array.Resize(ref m_pData, nLen);
                }
                catch (Exception e)
                {
                    m_sErrorInfo = e.Message;
                    return -2;
                }
            }
            //move data            
            int i, nOldPos, nNewPos;
            nOldPos = m_nDataLen - 1;
            nNewPos = m_nDataLen + nRowCount * m_pTableParam.nBytesPerRow - 1;
            i = m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable + nRowIndex * m_pTableParam.nBytesPerRow;
            for (; i < m_nDataLen; i++)
            {
                m_pData[nNewPos] = m_pData[nOldPos];
                nNewPos--;
                nOldPos--;
            }
            //clear old data
            /*
            nNewPos = m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable + nRowIndex * m_pTableParam.nBytesPerRow;
            for (i = 0; i < (m_pTableParam.nBytesPerRow * nRowCount); i++)
            {
                m_pData[nNewPos] = 0;
                nNewPos++;
            }
             */
            m_pTableParam.nRowCount += nRowCount;
            _SetParamToByte(ref m_pTableParam);

            m_nDataLen = m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable + m_pTableParam.nRowCount * m_pTableParam.nBytesPerRow;
            return nRowIndex;
        }
        public int DeleteRow(int nRowIndex)
        {
            return DeleteRow(nRowIndex, 1);
        }
        public int DeleteRow(int nRowIndex, int nRowCount)
        {
            if (m_pTableParam.nColumnCount == 0)
            {
                m_sErrorInfo = "Table Have Not Column";
                return -1;
            }
            if (nRowIndex < 0)
                return ClearRow();
            if (nRowCount < 1)
                nRowCount = 1;
            if (_IsValidIndexOfRow(nRowIndex) < 0)
                return -1;

            if (nRowCount + nRowIndex > m_pTableParam.nRowCount)
                nRowCount = m_pTableParam.nRowCount - nRowIndex;
            //move data
            int i, nOldPos, nNewPos;
            nOldPos = m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable + nRowIndex * m_pTableParam.nBytesPerRow + nRowCount * m_pTableParam.nBytesPerRow;
            nNewPos = m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable + nRowIndex * m_pTableParam.nBytesPerRow;
            i = nOldPos;
            for (; i < m_nDataLen; i++)
            {
                m_pData[nNewPos] = m_pData[nOldPos];
                nNewPos++;
                nOldPos++;
            }
            m_pTableParam.nRowCount -= nRowCount;
            _SetParamToByte(ref m_pTableParam);
            m_nDataLen = m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable + m_pTableParam.nRowCount * m_pTableParam.nBytesPerRow;
            return 0;
        }
        public int ClearRow()
        {
            m_pTableParam.nRowCount = 0;
            _SetParamToByte(ref m_pTableParam);
            m_nDataLen = m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable + m_pTableParam.nRowCount * m_pTableParam.nBytesPerRow;
            return 0;
        }
        public int ExchangeRow(int nRowIndex1, int nRowIndex2)
        {
            if (nRowIndex1 == nRowIndex2)
                return 0;
            if (_IsValidIndexOfRow(nRowIndex1) < 0)
                return -1;
            if (_IsValidIndexOfRow(nRowIndex2) < 0)
                return -1;

            int nPos1 = m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable + nRowIndex1 * m_pTableParam.nBytesPerRow;
            int nPos2 = m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable + nRowIndex2 * m_pTableParam.nBytesPerRow;
            int i;
            byte b;
            for (i = 0; i < m_pTableParam.nBytesPerRow; i++)
            {
                b = m_pData[nPos1];
                m_pData[nPos1] = m_pData[nPos2];
                m_pData[nPos2] = b;
                nPos1++;
                nPos2++;
            }
            return 0;
        }
    }
}
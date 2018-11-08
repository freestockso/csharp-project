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
        //add and delete column
        public int AddCol(string sName, Type pType, int nSize)
        {
            //First
            if (m_pData == null)
            {
                m_pData = new byte[m_nStructLenOfTable];
                _SetParamToByte(ref m_pTableParam);
                m_nDataLen = m_nStructLenOfTable;
            }
            //Verify
            if (sName == null || sName.Length == 0 || sName.Length > 255)
            {
                m_sErrorInfo = "Column Name Is Error";
                return -1;
            }
            int i;
            for (i = 0; i < m_pTableParam.nColumnCount; i++)
            {
                if (sName == m_pTableParam.sName)
                {
                    m_sErrorInfo = "Column Name Is Exist";
                    return -1;
                }
            }
            i = _GetSize(pType);
            if (i != 0)
                nSize = i;
            if (nSize <= 0)
            {
                m_sErrorInfo = "Type or Size Not Supported";
                return -1;
            }
            //Calucation bytes
            int nAddLen = nSize * m_pTableParam.nRowCount + m_nStructLenOfColumn;
            if ((nAddLen + m_nDataLen) > m_pData.Length)
            {
                try
                {
                    Array.Resize(ref m_pData, m_pData.Length + nAddLen);
                }
                catch (Exception e)
                {
                    m_sErrorInfo = e.Message;
                    return -2;
                }
            }
            //move data
            int j, nOldPos, nNewPos;
            nOldPos = m_nDataLen - 1;
            nNewPos = m_nDataLen - 1 + nAddLen - nSize;
            for (i = m_pTableParam.nRowCount - 1; i >= 0; i--)
            {
                for (j = 0; j < m_pTableParam.nBytesPerRow; j++)
                {
                    m_pData[nNewPos] = m_pData[nOldPos];
                    nOldPos--;
                    nNewPos--;
                }
                for (j = 0; j < nSize; j++)
                {
                    m_pData[nNewPos + m_pTableParam.nBytesPerRow + j] = 0;
                }
                nNewPos -= nSize;
            }
            if ((m_pTableParam.nColumnCount + 1) > m_nColumnParamLenPre)
            {
                try
                {
                    Array.Resize(ref m_pColumnParam, m_nColumnParamLenPre + 10);
                    m_nColumnParamLenPre += 10;
                }
                catch (Exception e)
                {
                    m_sErrorInfo = e.Message;
                    return -2;
                }
            }
            m_pColumnParam[m_pTableParam.nColumnCount].nVersion = 100;
            m_pColumnParam[m_pTableParam.nColumnCount].sName = sName;
            m_pColumnParam[m_pTableParam.nColumnCount].nType = _TypeToInt(pType);
            m_pColumnParam[m_pTableParam.nColumnCount].nSize = nSize;
            m_pColumnParam[m_pTableParam.nColumnCount].nPrecision = 3;
            m_pColumnParam[m_pTableParam.nColumnCount].nOffset = m_pTableParam.nBytesPerRow;

            _SetParamToByte(m_pTableParam.nColumnCount, ref m_pColumnParam[m_pTableParam.nColumnCount]);

            m_pTableParam.nBytesPerRow += nSize;
            m_pTableParam.nColumnCount += 1;
            _SetParamToByte(ref m_pTableParam);

            m_nDataLen = m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable + m_pTableParam.nRowCount * m_pTableParam.nBytesPerRow;
            return 0;
        }
        public int DeleteCol(string sName)
        {
            int nIndex;
            for (nIndex = 0; nIndex < m_pTableParam.nColumnCount; nIndex++)
            {
                if (sName == m_pColumnParam[nIndex].sName)
                    break;
            }
            if (nIndex >= m_pTableParam.nColumnCount)
            {
                m_sErrorInfo = "Column Name Is Not Exist";
                return -1;
            }
            return DeleteCol(nIndex);
        }
        public int DeleteCol(int nColumnIndex)
        {
            if (_IsValidIndexOfColumn(nColumnIndex) < 0)
                return -1;
            if (m_pTableParam.nColumnCount <= 1)
                return ClearCol();
            int i, j, k, nOldPos, nNewPos;
            //move head
            nOldPos = (nColumnIndex + 1) * m_nStructLenOfColumn + m_nStructLenOfTable;
            nNewPos = nColumnIndex * m_nStructLenOfColumn + m_nStructLenOfTable ;
            k = m_pTableParam.nColumnCount * m_nStructLenOfColumn + m_nStructLenOfTable;
            for (; nOldPos < k;)
            {
                m_pData[nNewPos] = m_pData[nOldPos];
                nOldPos++;
                nNewPos++;
            }
            //move data
            nOldPos = m_pTableParam.nColumnCount * m_nStructLenOfColumn + m_nStructLenOfTable;
            nNewPos = m_pTableParam.nColumnCount * m_nStructLenOfColumn + m_nStructLenOfTable - m_nStructLenOfColumn;
            for (i = 0; i < m_pTableParam.nRowCount; i++)
            {
                //nOldPos = m_pTableParam.nColumnCount * m_nStructLenOfColumn + m_nStructLenOfTable + i * m_pTableParam.nBytesPerRow;
                for (j = 0; j < m_pColumnParam[nColumnIndex].nOffset; j++)
                {
                    m_pData[nNewPos] = m_pData[nOldPos];
                    nOldPos++;
                    nNewPos++;
                }
                nOldPos += m_pColumnParam[nColumnIndex].nSize;
                j += m_pColumnParam[nColumnIndex].nSize;
                for (; j < m_pTableParam.nBytesPerRow; j++)
                {
                    m_pData[nNewPos] = m_pData[nOldPos];
                    nOldPos++;
                    nNewPos++;
                }
            }
            //move column param
            for (i = nColumnIndex + 1; i < m_pTableParam.nColumnCount; i++)
                m_pColumnParam[i - 1] = m_pColumnParam[i];
            m_pTableParam.nColumnCount -= 1;
            //update offset and bytes per row
            int nOffset = 0;
            m_pTableParam.nBytesPerRow = 0;
            for (i = 0; i < m_pTableParam.nColumnCount; i++)
            {
                m_pColumnParam[i].nOffset = nOffset;
                nOffset += m_pColumnParam[i].nSize;
                m_pTableParam.nBytesPerRow += m_pColumnParam[i].nSize;
            }
            _SetParamToByte(ref m_pTableParam);

            m_nDataLen = m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable + m_pTableParam.nRowCount * m_pTableParam.nBytesPerRow;
            return 0;
        }
        public int ClearCol()
        {
            m_pTableParam.nColumnCount = 0;
            m_pTableParam.nRowCount = 0;
            m_pTableParam.nBytesPerRow = 0;
            _SetParamToByte(ref m_pTableParam);
            m_nDataLen = m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable + m_pTableParam.nRowCount * m_pTableParam.nBytesPerRow;
            return 0;
        }
    }
}
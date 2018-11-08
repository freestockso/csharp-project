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
        public int CopyToTable(ref _Table pToTable)
        {
            //copy full table
            if (pToTable == null)
                pToTable = new _Table(m_nID);

            if (pToTable.m_pData.Length < m_nDataLen)
            {
                try
                {
                    Array.Resize(ref pToTable.m_pData, m_nDataLen);
                }
                catch (Exception e)
                {
                    m_sErrorInfo = e.Message;
                    return - 2;
                }

            }
            int i;
            for (i = 0; i < m_nDataLen; i++)
                pToTable.m_pData[i] = m_pData[i];
            pToTable.m_nDataLen = m_nDataLen;

            pToTable.m_pTableParam = m_pTableParam;
            for (i = 0; i < pToTable.m_pTableParam.nColumnCount; i++)
                pToTable.m_pColumnParam[i] = m_pColumnParam[i];
            return m_nDataLen;
        }
        public int AppendToTable(ref _Table pToTable)
        {
            if (pToTable == null)
                pToTable = new _Table(m_nID);
            if (m_pTableParam.nColumnCount != pToTable.m_pTableParam.nColumnCount)
            {
                m_sErrorInfo = "Count Of Column Is Not Equal";
                return -1;
            }
            int i;
            for (i = 0; i < m_pTableParam.nColumnCount; i++)
            {
                if (m_pColumnParam[i].nType != pToTable.m_pColumnParam[i].nType)
                {
                    m_sErrorInfo = "Data Type Of Column Is Not Equal";
                    return -1;
                }
                if (m_pColumnParam[i].nSize != pToTable.m_pColumnParam[i].nSize)
                {
                    m_sErrorInfo = "Size Of Column Is Not Equal";
                    return -1;
                }
            }
            int nDataLen = m_nDataLen - m_nStructLenOfTable - m_pTableParam.nColumnCount * m_nStructLenOfColumn;
            if ((nDataLen + pToTable.m_nDataLen) > pToTable.m_pData.Length)
            {
                try
                {
                    Array.Resize(ref pToTable.m_pData, nDataLen + pToTable.m_nDataLen);
                }
                catch (Exception e)
                {
                    m_sErrorInfo = e.Message;
                    return -2;
                }
            }
            i = m_nStructLenOfTable + m_pTableParam.nColumnCount * m_nStructLenOfColumn;
            for(; i < m_nDataLen ; i++ , pToTable.m_nDataLen ++)
                pToTable.m_pData [ pToTable.m_nDataLen ] = m_pData [ i ];
            pToTable.m_pTableParam.nRowCount += m_pTableParam.nRowCount;
            pToTable._SetParamToByte(ref pToTable.m_pTableParam);
            return nDataLen;
        }
        public int CopyColumnToTable(ref _Table pToTable)
        {
            //copy column struct
            try
            {
                if (pToTable == null)
                    pToTable = new _Table(m_nID);
                int nDataLen = m_nStructLenOfTable + m_pTableParam.nColumnCount * m_nStructLenOfColumn;
                if (pToTable.m_pData.Length < nDataLen)
                {
                    Array.Resize(ref pToTable.m_pData, nDataLen);
                }
                int i;
                for (i = 0; i < nDataLen; i++)
                    pToTable.m_pData[i] = m_pData[i];
                pToTable.m_nDataLen = nDataLen;

                pToTable.m_pTableParam.sName = m_pTableParam.sName;
                pToTable.m_pTableParam.nVersion = m_pTableParam.nVersion;
                pToTable.m_pTableParam.nBytesPerRow = m_pTableParam.nBytesPerRow;
                pToTable.m_pTableParam.nColumnCount = m_pTableParam.nColumnCount;
                pToTable.m_pTableParam.nRowCount = 0;
                pToTable._SetParamToByte(ref pToTable.m_pTableParam);
                for (i = 0; i < pToTable.m_pTableParam.nColumnCount; i++)
                    pToTable.m_pColumnParam[i] = m_pColumnParam[i];
                return nDataLen;
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
            }
            return -2;
        }
        //
        public int CopyFromTable(ref _Table pFromTable)
        {
            //copy full table
            if (pFromTable == null)
                return -1;
            if (pFromTable.m_nDataLen > m_pData.Length)
            {
                try
                {
                    Array.Resize(ref m_pData, pFromTable.m_nDataLen);
                }
                catch (Exception e)
                {
                    m_sErrorInfo = e.Message;
                    return -2;
                }

            }
            int i;
            for (i = 0; i < pFromTable.m_nDataLen; i++)
                m_pData[i] = pFromTable.m_pData[i];
            m_nDataLen = pFromTable.m_nDataLen;

            m_pTableParam = pFromTable.m_pTableParam;
            for (i = 0; i < pFromTable.m_pTableParam.nColumnCount; i++)
                m_pColumnParam[i] = pFromTable.m_pColumnParam[i];
            return pFromTable.m_nDataLen;
        }
        public int AppendFromTable(ref _Table pFromTable)
        {
            if (pFromTable == null)
                return -1;
            if (m_pTableParam.nColumnCount != pFromTable.m_pTableParam.nColumnCount)
            {
                m_sErrorInfo = "Count Of Column Is Not Equal";
                return -1;
            }
            int i;
            for (i = 0; i < m_pTableParam.nColumnCount; i++)
            {
                if (m_pColumnParam[i].nType != pFromTable.m_pColumnParam[i].nType)
                {
                    m_sErrorInfo = "Data Type Of Column Is Not Equal";
                    return -1;
                }
                if (m_pColumnParam[i].nSize != pFromTable.m_pColumnParam[i].nSize)
                {
                    m_sErrorInfo = "Size Of Column Is Not Equal";
                    return -1;
                }
            }
            int nDataLen = pFromTable.m_nDataLen - pFromTable.m_nStructLenOfTable - pFromTable.m_pTableParam.nColumnCount * pFromTable.m_nStructLenOfColumn;
            if ((nDataLen + m_nDataLen) > m_pData.Length)
            {
                try
                {
                    Array.Resize(ref m_pData, nDataLen + m_nDataLen);
                }
                catch (Exception e)
                {
                    m_sErrorInfo = e.Message;
                    return -2;
                }
            }
            i = pFromTable.m_nStructLenOfTable + pFromTable.m_pTableParam.nColumnCount * pFromTable.m_nStructLenOfColumn;
            for (; i < pFromTable.m_nDataLen; i++, m_nDataLen++)
                m_pData[m_nDataLen] = pFromTable.m_pData[i];
            m_pTableParam.nRowCount += pFromTable.m_pTableParam.nRowCount;
            _SetParamToByte(ref m_pTableParam);
            return nDataLen;
        }
        public int CopyColumnFromTable(ref _Table pFromTable)
        {
            //copy column struct
            try
            {
                if (pFromTable == null)
                    return -1;
                int nDataLen = pFromTable.m_nStructLenOfTable + pFromTable.m_pTableParam.nColumnCount * pFromTable.m_nStructLenOfColumn;
                if (m_pData.Length < nDataLen)
                {
                    Array.Resize(ref m_pData, nDataLen);
                }
                int i;
                for (i = 0; i < nDataLen; i++)
                    m_pData[i] = pFromTable.m_pData[i];
                m_nDataLen = nDataLen;

                m_pTableParam.sName = pFromTable.m_pTableParam.sName;
                m_pTableParam.nVersion = pFromTable.m_pTableParam.nVersion;
                m_pTableParam.nBytesPerRow = pFromTable.m_pTableParam.nBytesPerRow;
                m_pTableParam.nColumnCount = pFromTable.m_pTableParam.nColumnCount;
                m_pTableParam.nRowCount = 0;
                _SetParamToByte(ref m_pTableParam);
                for (i = 0; i < pFromTable.m_pTableParam.nColumnCount; i++)
                    m_pColumnParam[i] = pFromTable.m_pColumnParam[i];
                return nDataLen;
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
            }
            return -2;
        }
    }
}

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
        ////////////////////
        //Query value
        public int IsExist(int nColIndex, string sKey)
        {
            return IsExist(0, nColIndex, sKey);
        }
        public int IsExist(int nColIndex, Int32 nKey)
        {
            byte[] bValue = BitConverter .GetBytes (nKey);
            return IsExist(0, nColIndex,ref bValue);
        }
        public int IsExist(int nColIndex, Int64 nKey)
        {
            byte[] bValue = BitConverter.GetBytes(nKey);
            return IsExist(0, nColIndex,ref bValue);
        }
        public int IsExist(int nQueryStartRow, int nColIndex, string Value)
        {
            if (_IsValidIndex(nQueryStartRow, nColIndex) < 0)
                return -1;
            byte[] bValue = null;
            int nResult = _StringToBytes(m_pColumnParam[nColIndex].nType, Value, ref bValue);
            if (nResult < 0)
                return nResult;
            return IsExist(nQueryStartRow, nColIndex, ref bValue);
        }
        public int IsExist(int nQueryStartRow, int nColIndex,ref byte[] Value)
        {
            int nPos = nQueryStartRow * m_pTableParam.nBytesPerRow + m_pColumnParam[nColIndex].nOffset;
            nPos += m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable;
            for(; nPos < m_nDataLen ; nPos += m_pTableParam.nBytesPerRow , nQueryStartRow ++)
            {
                if(_IsExist(nPos, m_pColumnParam[nColIndex].nSize, ref Value) >= 0)
                    return nQueryStartRow;
            }
            return -1;
        }
        public int Query(int nColIndex, string Value, ref _Table pTable)
        {
            byte[] bValue = null;
            int nResult = _StringToBytes(m_pColumnParam[nColIndex].nType, Value, ref bValue);
            if (nResult < 0)
                return nResult;
            return Query(nColIndex, ref bValue, ref pTable);
        }
        public int Query(int nColIndex, Int32 Value, ref _Table pTable)
        {
            byte[] bValue = BitConverter.GetBytes(Value);
            return Query(nColIndex, ref bValue, ref pTable);
        }
        public int Query(int nColIndex, ref byte[] Value, ref _Table pTable)
        {
            if(pTable == null)
                pTable = new _Table (m_nID);
            CopyColumnToTable (ref pTable);
            int nPos = m_pColumnParam[nColIndex].nOffset + m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable;
            int nQueryRow;
            byte[] bRow = new byte[m_pTableParam.nBytesPerRow];
            for (nQueryRow = 0; nPos < m_nDataLen; nPos += m_pTableParam.nBytesPerRow, nQueryRow++)
            {
                if (_IsExist(nPos, m_pColumnParam[nColIndex].nSize, ref Value) >= 0)
                {
                    GetRow(nQueryRow ,ref bRow );
                    pTable.AppendRow(ref bRow);
                }
            }
            return pTable.GetRowCount ();
        }
        ///////////
        int _IsExist(int nPos, int nSize, ref byte[] Value)
        {
            int i;
            for (i = 0; i < Value.Length && i < nSize && nPos < m_nDataLen ; i++, nPos++)
            {
                if (m_pData[nPos] != Value[i])
                    break;
            }
            if (i == Value.Length)
            {
                return nPos;
            }
            return -1;
        }
    }
}
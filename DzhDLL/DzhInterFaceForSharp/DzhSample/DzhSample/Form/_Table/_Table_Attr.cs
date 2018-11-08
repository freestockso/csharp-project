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
        public object this[int nRowIndex, int nColumnIndex]
        {
            get
            {
                return GetValue(nRowIndex, nColumnIndex);
            }
            set
            {
                SetValue(nRowIndex, nColumnIndex, value);
            }
        }
        //set and get some table attr
        public string GetErrorInfo()
        {
            return m_sErrorInfo;
        }
        public void SetErrorInfo(string sErrorInfo)
        {
            m_sErrorInfo = sErrorInfo;
        }
        public int GetID()
        {
            return m_nID;
        }
        public int GetColCount()
        {
            return m_pTableParam.nColumnCount;
        }
        public Type GetColType(int nColumnIndex)
        {
            if (_IsValidIndexOfColumn(nColumnIndex) < 0)
                return null;
            return _IntToType(m_pColumnParam[nColumnIndex].nType);
        }
        public int GetColTypeInt(int nColumnIndex)
        {
            if (_IsValidIndexOfColumn(nColumnIndex) < 0)
                return 0;
            return m_pColumnParam[nColumnIndex].nType;
        }
        public string GetColName(int nColumnIndex)
        {
            if (_IsValidIndexOfColumn(nColumnIndex) < 0)
                return "";
            return m_pColumnParam[nColumnIndex].sName;
        }
        public int GetColOffset(int nColumnIndex)
        {
            if (_IsValidIndexOfColumn(nColumnIndex) < 0)
                return -1;
            return m_pColumnParam[nColumnIndex].nOffset;
        }
        public int GetColSize(int nColumnIndex)
        {
            if (_IsValidIndexOfColumn(nColumnIndex) < 0)
                return -1;
            return m_pColumnParam[nColumnIndex].nSize;
        }
        public int GetColPrecision(int nColumnIndex)
        {
            if (_IsValidIndexOfColumn(nColumnIndex) < 0)
                return -1;
            return m_pColumnParam[nColumnIndex].nPrecision ;
        }
        //extern set
        public int SetColPrecision(int nColumnIndex, int nPrecision)
        {
            if (_IsValidIndexOfColumn(nColumnIndex) < 0)
                return -1;
            int n = m_pColumnParam[nColumnIndex].nPrecision;
            m_pColumnParam[nColumnIndex].nPrecision = nPrecision;
            return n;
        }
        public int GetRowCount()
        {
            return m_pTableParam.nRowCount;
        }
        public int GetBytesPerRow()
        {
            return m_pTableParam.nBytesPerRow;
        }
        public string GetTableName()
        {
            return m_pTableParam.sName;
        }
        /////////////////
        protected int _GetDataStartPos()
        {
            return m_nStructLenOfTable + m_nStructLenOfColumn * m_pTableParam.nColumnCount;
        }
        protected int _GetColumnStartPos(int nColumnIndex)
        {
            return m_nStructLenOfTable + m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_pColumnParam[nColumnIndex].nOffset;
        }
        protected int _IsValidIndex(int nRowIndex, int nColumnIndex)
        {
            if (nRowIndex < 0 || nRowIndex >= m_pTableParam.nRowCount)
            {
                m_sErrorInfo = "Row Index Exceed Range";
                return -1;
            }
            if (nColumnIndex < 0 || nColumnIndex >= m_pTableParam.nColumnCount)
            {
                m_sErrorInfo = "Column Index Exceed Range";
                return -1;
            }
            return 0;
        }
        protected int _IsValidIndexOfRow(int nRowIndex)
        {
            if (nRowIndex < 0 || nRowIndex >= m_pTableParam.nRowCount)
            {
                m_sErrorInfo = "Row Index Exceed Range";
                return -1;
            }
            return 0;
        }
        protected int _IsValidIndexOfColumn(int nColumnIndex)
        {
            if (nColumnIndex < 0 || nColumnIndex >= m_pTableParam.nColumnCount)
            {
                m_sErrorInfo = "Column Index Exceed Range";
                return -1;
            }
            return 0;
        }

    }
}
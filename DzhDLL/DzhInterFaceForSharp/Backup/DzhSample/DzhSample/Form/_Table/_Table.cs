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
        struct COLUMN_PARAM
        {
            public int nVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string sName;
            public int nType;
            public int nSize;
            public int nPrecision;
            public int nOffset;
        }
        struct TABLE_PARAM
        {
            public int nVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string sName;
            public int nBytesPerRow;
            public int nColumnCount;
            public int nRowCount;
        }

        TABLE_PARAM m_pTableParam = new TABLE_PARAM ();
        COLUMN_PARAM[] m_pColumnParam = new COLUMN_PARAM[10];
        int m_nColumnParamLenPre = 10;

        int m_nStructLenOfColumn = 0;
        int m_nStructLenOfTable = 0;
        byte[] m_pData = null;
        int m_nDataLen = 0;
        string m_sErrorInfo = "";
        int m_nID = 0;

        public _Table()
        {
            _Initial(0, "Table");
        }
        public _Table(string sName)
        {
            _Initial(0, "Table");
        }
        public _Table(int nID)
        {
            _Initial(nID, nID.ToString ());
        }
        public _Table(int nID , string sName)
        {
            _Initial(nID, sName);
        }
        ~_Table()
        {
        }
        public void Destroy()
        {
        }
        void _Initial(int nID, string sName)
        {
            m_nID = nID;
            m_pTableParam.sName = sName;
            m_pTableParam.nVersion = 100;

            m_nStructLenOfColumn = Marshal.SizeOf(typeof(COLUMN_PARAM));
            m_nStructLenOfTable = Marshal.SizeOf(typeof(TABLE_PARAM));

            m_pData = new byte[m_nStructLenOfTable];
            _SetParamToByte(ref m_pTableParam);
            m_nDataLen = m_nStructLenOfTable;
        }
    }
}

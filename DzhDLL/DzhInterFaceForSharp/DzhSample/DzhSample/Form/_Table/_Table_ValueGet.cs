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
        public object GetValue(int nRowIndex, int nColIndex)
        {
            if (_IsValidIndex(nRowIndex, nColIndex) < 0)
                return null;
            object Value = null;
            int nPos = nRowIndex * m_pTableParam.nBytesPerRow + m_pColumnParam[nColIndex].nOffset;
            nPos += m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable;
            int nType = m_pColumnParam[nColIndex].nType;
            try
            {
                if (nType == 1)
                    Value = BitConverter.ToInt16(m_pData, nPos);
                else if (nType == 2)
                    Value = BitConverter.ToUInt16(m_pData, nPos);
                else if (nType == 3)
                    Value = BitConverter.ToInt32(m_pData, nPos);
                else if (nType == 4)
                    Value = BitConverter.ToUInt32(m_pData, nPos);
                else if (nType == 5)
                    Value = BitConverter.ToInt64(m_pData, nPos);
                else if (nType == 6)
                    Value = BitConverter.ToUInt64(m_pData, nPos);
                else if (nType == 7)
                    Value = m_pData[nPos];
                else if (nType == 8)
                    Value = m_pData[nPos];
                else if (nType == 9)
                {
                    int i;
                    for (i = 0; i < m_pColumnParam[nColIndex].nSize; i++)
                        Value += Convert.ToString(m_pData[nPos], 16);
                }
                else if (nType == 10)
                {
                    int i;
                    for (i = 0; i < m_pColumnParam[nColIndex].nSize; i++)
                        Value += Convert.ToString(m_pData[nPos], 16);
                }
                else if (nType == 11)
                {
                    int nSize;
                    for (nSize = 0; nSize < m_pColumnParam[nColIndex].nSize; nSize++)
                    {
                        if (m_pData[nPos + nSize] == 0)
                            break;
                    }
                    Value = System.Text.Encoding.Default.GetString(m_pData, nPos, nSize);
                }
                else if (nType == 12)
                    Value = BitConverter.ToSingle(m_pData, nPos);
                else if (nType == 13)
                    Value = BitConverter.ToDouble(m_pData, nPos);
                else if (nType == 14)
                {
                    int i, j;
                    int[] nValue = new int[4];
                    for (j = i = 0; i < m_pColumnParam[nColIndex].nSize && i < 16; i += 4, j++, nPos += 4)
                        nValue[j] = BitConverter.ToInt32(m_pData, nPos);
                    Value = new Decimal(nValue);
                }
                else if (nType == 15)
                {
                    Int64 nValue = BitConverter.ToInt64(m_pData, nPos);
                    Value = new DateTime(nValue);
                }
                else if (nType == 16)
                {
                    Value = BitConverter.ToBoolean(m_pData, nPos);
                }
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
                return -2;
            }
            return Value;
        }
        ////////////////////////////////////////////////////////
        //Get Value(int nRowIndex , int nColumnIndex)
        public int GetValue(int nRowIndex, int nColIndex, ref byte[] Value)
        {
            if (_IsValidIndex(nRowIndex, nColIndex) < 0)
                return -1;
            int nPos = nRowIndex * m_pTableParam.nBytesPerRow + m_pColumnParam[nColIndex].nOffset;
            nPos += m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable;
            int i;
            try
            {
                Value = new byte[m_pColumnParam[nColIndex].nSize];
                for (i = 0; i < m_pColumnParam[nColIndex].nSize; i++, nPos++)
                    Value[i] = m_pData[nPos];
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
                return -2;
            }
            return m_pColumnParam[nColIndex].nSize;
        }
        public int GetValue(int nRowIndex, int nColIndex, ref Int16 Value)
        {
            if (_IsValidIndex(nRowIndex, nColIndex) < 0)
                return -1;
            int nPos = nRowIndex * m_pTableParam.nBytesPerRow + m_pColumnParam[nColIndex].nOffset;
            nPos += m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable;
            try
            {
                Value = BitConverter.ToInt16(m_pData, nPos);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
                return -2;
            }
            return m_pColumnParam[nColIndex].nSize;
        }
        public int GetValue(int nRowIndex, int nColIndex, ref UInt16 Value)
        {
            if (_IsValidIndex(nRowIndex, nColIndex) < 0)
                return -1;
            int nPos = nRowIndex * m_pTableParam.nBytesPerRow + m_pColumnParam[nColIndex].nOffset;
            nPos += m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable;
            try
            {
                Value = BitConverter.ToUInt16(m_pData, nPos);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
                return -2;
            }
            return m_pColumnParam[nColIndex].nSize;
        }
        public int GetValue(int nRowIndex, int nColIndex, ref Int32 Value)
        {
            if (_IsValidIndex(nRowIndex, nColIndex) < 0)
                return -1;
            int nPos = nRowIndex * m_pTableParam.nBytesPerRow + m_pColumnParam[nColIndex].nOffset;
            nPos += m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable;
            try
            {
                Value = BitConverter.ToInt32(m_pData, nPos);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
                return -2;
            }
            return m_pColumnParam[nColIndex].nSize;
        }
        public int GetValue(int nRowIndex, int nColIndex, ref UInt32 Value)
        {
            if (_IsValidIndex(nRowIndex, nColIndex) < 0)
                return -1;
            int nPos = nRowIndex * m_pTableParam.nBytesPerRow + m_pColumnParam[nColIndex].nOffset;
            nPos += m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable;
            try
            {
                Value = BitConverter.ToUInt32(m_pData, nPos);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
                return -2;
            }
            return m_pColumnParam[nColIndex].nSize;
        }
        public int GetValue(int nRowIndex, int nColIndex, ref Int64 Value)
        {
            if (_IsValidIndex(nRowIndex, nColIndex) < 0)
                return -1;
            int nPos = nRowIndex * m_pTableParam.nBytesPerRow + m_pColumnParam[nColIndex].nOffset;
            nPos += m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable;
            try
            {
                Value = BitConverter.ToInt64(m_pData, nPos);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
                return -2;
            }
            return m_pColumnParam[nColIndex].nSize;
        }
        public int GetValue(int nRowIndex, int nColIndex, ref UInt64 Value)
        {
            if (_IsValidIndex(nRowIndex, nColIndex) < 0)
                return -1;
            int nPos = nRowIndex * m_pTableParam.nBytesPerRow + m_pColumnParam[nColIndex].nOffset;
            nPos += m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable;
            try
            {
                Value = BitConverter.ToUInt64(m_pData, nPos);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
                return -2;
            }
            return m_pColumnParam[nColIndex].nSize;
        }
        public int GetValue(int nRowIndex, int nColIndex, ref Single Value)
        {
            if (_IsValidIndex(nRowIndex, nColIndex) < 0)
                return -1;
            int nPos = nRowIndex * m_pTableParam.nBytesPerRow + m_pColumnParam[nColIndex].nOffset;
            nPos += m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable;
            try
            {
                Value = BitConverter.ToSingle(m_pData, nPos);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
                return -2;
            }
            return m_pColumnParam[nColIndex].nSize;
        }
        public int GetValue(int nRowIndex, int nColIndex, ref Double Value)
        {
            if (_IsValidIndex(nRowIndex, nColIndex) < 0)
                return -1;
            int nPos = nRowIndex * m_pTableParam.nBytesPerRow + m_pColumnParam[nColIndex].nOffset;
            nPos += m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable;
            try
            {
                Value = BitConverter.ToDouble(m_pData, nPos);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
                return -2;
            }
            return m_pColumnParam[nColIndex].nSize;
        }
        public int GetValue(int nRowIndex, int nColIndex, ref Byte Value)
        {
            if (_IsValidIndex(nRowIndex, nColIndex) < 0)
                return -1;
            int nPos = nRowIndex * m_pTableParam.nBytesPerRow + m_pColumnParam[nColIndex].nOffset;
            nPos += m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable;
            try
            {
                Value = m_pData[nPos];
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
                return -2;
            }
            return m_pColumnParam[nColIndex].nSize;
        }
        public int GetValue(int nRowIndex, int nColIndex, ref SByte Value)
        {
            if (_IsValidIndex(nRowIndex, nColIndex) < 0)
                return -1;
            int nPos = nRowIndex * m_pTableParam.nBytesPerRow + m_pColumnParam[nColIndex].nOffset;
            nPos += m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable;
            try
            {
                Value = (SByte)m_pData[nPos];
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
                return -2;
            }

            return m_pColumnParam[nColIndex].nSize;
        }
        public int GetValue(int nRowIndex, int nColIndex, ref String Value)
        {
            if (_IsValidIndex(nRowIndex, nColIndex) < 0)
                return -1;
            int nPos = nRowIndex * m_pTableParam.nBytesPerRow + m_pColumnParam[nColIndex].nOffset;
            nPos += m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable;
            int nSize;
            try
            {
                for (nSize = 0; nSize < m_pColumnParam[nColIndex].nSize; nSize++)
                {
                    if (m_pData[nPos + nSize] == 0)
                        break;
                }
                Value = System.Text.Encoding.Default.GetString(m_pData, nPos, nSize);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
                return -2;
            }
            return m_pColumnParam[nColIndex].nSize;
        }
        public int GetValue(int nRowIndex, int nColIndex, ref Decimal Value)
        {
            if (_IsValidIndex(nRowIndex, nColIndex) < 0)
                return -1;
            int nPos = nRowIndex * m_pTableParam.nBytesPerRow + m_pColumnParam[nColIndex].nOffset;
            nPos += m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable;
            int i , j;
            try
            {
                int[] nValue = new int[4];
                for (j = i = 0; i < m_pColumnParam[nColIndex].nSize && i < 16; i += 4, j++, nPos += 4)
                    nValue[j] = BitConverter.ToInt32(m_pData, nPos);
                Value = new Decimal(nValue);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
                return -2;
            }
            return m_pColumnParam[nColIndex].nSize;
        }
        public int GetValue(int nRowIndex, int nColIndex, ref DateTime Value)
        {
            if (_IsValidIndex(nRowIndex, nColIndex) < 0)
                return -1;
            int nPos = nRowIndex * m_pTableParam.nBytesPerRow + m_pColumnParam[nColIndex].nOffset;
            nPos += m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable;
            try
            {
                Int64 nValue = BitConverter.ToInt64(m_pData, nPos);
                Value = new DateTime(nValue);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
                return -2;
            }
            return m_pColumnParam[nColIndex].nSize;
        }
        public int GetValue(int nRowIndex, int nColIndex, ref Boolean Value)
        {
            if (_IsValidIndex(nRowIndex, nColIndex) < 0)
                return -1;
            int nPos = nRowIndex * m_pTableParam.nBytesPerRow + m_pColumnParam[nColIndex].nOffset;
            nPos += m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable;
            try
            {
                Value = BitConverter.ToBoolean(m_pData, nPos);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
                return -2;
            }
            return m_pColumnParam[nColIndex].nSize;
        }
        public string GetValueByString(int nRowIndex, int nColIndex)
        {
            string Value = "";
            GetValueByString(nRowIndex, nColIndex ,ref Value);
            return Value;
        }
        public int GetValueByString(int nRowIndex, int nColIndex ,ref string Value)
        {
            Value = "";
            if (_IsValidIndex(nRowIndex, nColIndex) < 0)
                return -1;
            int nPos = nRowIndex * m_pTableParam.nBytesPerRow + m_pColumnParam[nColIndex].nOffset;
            nPos += m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable;
            int nType = m_pColumnParam[nColIndex].nType;
            try
            {
                if (nType == 1)
                    Value = BitConverter.ToInt16(m_pData, nPos).ToString();
                else if (nType == 2)
                    Value = BitConverter.ToUInt16(m_pData, nPos).ToString();
                else if (nType == 3)
                    Value = BitConverter.ToInt32(m_pData, nPos).ToString();
                else if (nType == 4)
                    Value = BitConverter.ToUInt32(m_pData, nPos).ToString();
                else if (nType == 5)
                    Value = BitConverter.ToInt64(m_pData, nPos).ToString();
                else if (nType == 6)
                    Value = BitConverter.ToUInt64(m_pData, nPos).ToString();
                else if (nType == 7)
                    Value = m_pData[nPos].ToString();
                else if (nType == 8)
                    Value = m_pData[nPos].ToString();
                else if (nType == 9)
                {
                    int i;
                    for (i = 0; i < m_pColumnParam[nColIndex].nSize; i++)
                        Value += Convert.ToString(m_pData[nPos], 16);
                }
                else if (nType == 10)
                {
                    int i;
                    for (i = 0; i < m_pColumnParam[nColIndex].nSize; i++)
                        Value += Convert.ToString(m_pData[nPos], 16);
                }
                else if (nType == 11)
                {
                    int nSize;
                    for (nSize = 0; nSize < m_pColumnParam[nColIndex].nSize; nSize++)
                    {
                        if (m_pData[nPos + nSize] == 0)
                            break;
                    }
                    Value = System.Text.Encoding.Default.GetString(m_pData, nPos, nSize);
                }
                else if (nType == 12)
                {                    
                    Value = BitConverter.ToSingle(m_pData, nPos).ToString("F" + m_pColumnParam[nColIndex].nPrecision.ToString ());
                }
                else if (nType == 13)
                {
                    Value = BitConverter.ToDouble(m_pData, nPos).ToString("F" + m_pColumnParam[nColIndex].nPrecision.ToString());
                }
                else if (nType == 14)
                {
                    int i, j;
                    int[] nValue = new int[4];
                    for (j = i = 0; i < m_pColumnParam[nColIndex].nSize && i < 16; i += 4, j++, nPos += 4)
                        nValue[j] = BitConverter.ToInt32(m_pData, nPos);
                    Decimal cDecimal = new Decimal(nValue);
                    Value = cDecimal.ToString();
                }
                else if (nType == 15)
                {
                    Int64 nValue = BitConverter.ToInt64(m_pData, nPos);
                    DateTime cDateTime = new DateTime(nValue);
                    Value = cDateTime.ToString();
                }
                else if (nType == 16)
                {
                    Value = BitConverter.ToBoolean(m_pData, nPos).ToString();
                }
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
                return -2;
            }
            return m_pColumnParam[nColIndex].nSize;
        }
        public string GetRow(int nRowIndex)
        {
            string Value = "";
            GetRow(nRowIndex, ref Value);
            return Value;
        }
        public int GetRow(int nRowIndex, ref String Value)
        {
            Value = "";
            if (_IsValidIndexOfRow(nRowIndex) < 0)
                return -1;
            string s = "";
            int nColIndex , nValueLen , nResult;
            nValueLen = 0;
            for (nColIndex = 0; nColIndex < m_pTableParam.nColumnCount; nColIndex++)
            {
                nResult = GetValueByString(nRowIndex, nColIndex, ref s);
                if (nResult < 0)
                    return nResult;
                nValueLen += nResult;
                Value += s;
                if ((nColIndex + 1) < m_pTableParam.nColumnCount)
                {
                    Value += ",";
                    nValueLen++;
                }
            }
            return nValueLen;
        }
        public int GetRow(int nRowIndex, ref byte[] Value)
        {
            if (Value == null || Value.Length != m_pTableParam.nBytesPerRow)
            {
                m_sErrorInfo = "Value Length Is Not Equal";
                return -1;
            }
            int nPos = nRowIndex * m_pTableParam.nBytesPerRow;
            nPos += m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable;
            int i;
            for (i = 0; i < m_pTableParam.nBytesPerRow; i++, nPos++)
                Value[i] = m_pData[nPos];
            return m_pTableParam.nBytesPerRow;
        }
    }
}
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
        //Set Value(int nRowIndex, int nColIndex)
        public int SetValue(int nRowIndex, int nColIndex, object Value)
        {
            try
            {
                if (m_pColumnParam[nColIndex].nType == _TypeToInt(typeof(Int16)))
                    return SetValue(nRowIndex, nColIndex, (Int16)Value);
                else if (m_pColumnParam[nColIndex].nType == _TypeToInt(typeof(UInt16)))
                    return SetValue(nRowIndex, nColIndex, (UInt16)Value);
                else if (m_pColumnParam[nColIndex].nType == _TypeToInt(typeof(Int32)))
                    return SetValue(nRowIndex, nColIndex, (Int32)Value);
                else if (m_pColumnParam[nColIndex].nType == _TypeToInt(typeof(UInt32)))
                    return SetValue(nRowIndex, nColIndex, (UInt32)Value);
                else if (m_pColumnParam[nColIndex].nType == _TypeToInt(typeof(Int64)))
                    return SetValue(nRowIndex, nColIndex, (Int64)Value);
                else if (m_pColumnParam[nColIndex].nType == _TypeToInt(typeof(UInt64)))
                    return SetValue(nRowIndex, nColIndex, (UInt64)Value);
                else if (m_pColumnParam[nColIndex].nType == _TypeToInt(typeof(Byte)))
                    return SetValue(nRowIndex, nColIndex, (Byte)Value);
                else if (m_pColumnParam[nColIndex].nType == _TypeToInt(typeof(SByte)))
                    return SetValue(nRowIndex, nColIndex, (SByte)Value);
                else if (m_pColumnParam[nColIndex].nType == _TypeToInt(typeof(Single)))
                    return SetValue(nRowIndex, nColIndex, (Single)Value);
                else if (m_pColumnParam[nColIndex].nType == _TypeToInt(typeof(Double)))
                    return SetValue(nRowIndex, nColIndex, (Double)Value);
                else if (m_pColumnParam[nColIndex].nType == _TypeToInt(typeof(String)))
                    return SetValue(nRowIndex, nColIndex, (String)Value);
                else if (m_pColumnParam[nColIndex].nType == _TypeToInt(typeof(Decimal)))
                    return SetValue(nRowIndex, nColIndex, (Decimal)Value);
                else if (m_pColumnParam[nColIndex].nType == _TypeToInt(typeof(DateTime)))
                    return SetValue(nRowIndex, nColIndex, (DateTime)Value);
                else if (m_pColumnParam[nColIndex].nType == _TypeToInt(typeof(Boolean)))
                    return SetValue(nRowIndex, nColIndex, (Boolean)Value);
                else if (m_pColumnParam[nColIndex].nType == _TypeToInt(typeof(byte[])))
                    return SetValue(nRowIndex, nColIndex, (byte[])Value);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
            }
            return -1;
        }
        public int SetValue(int nRowIndex, int nColIndex, Int16 Value)
        {
            try
            {
                byte[] bValue = BitConverter.GetBytes(Value);
                return SetValue(nRowIndex, nColIndex, ref bValue);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
            }
            return -2;           
        }
        public int SetValue(int nRowIndex, int nColIndex, UInt16 Value)
        {
            try
            {
                byte[] bValue = BitConverter.GetBytes(Value);
                return SetValue(nRowIndex, nColIndex, ref bValue);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
            }
            return -2;
        }
        public int SetValue(int nRowIndex, int nColIndex, Int32 Value)
        {
            try
            {
                byte[] bValue = BitConverter.GetBytes(Value);
                return SetValue(nRowIndex, nColIndex, ref bValue);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
            }
            return -2;
        }
        public int SetValue(int nRowIndex, int nColIndex, UInt32 Value)
        {
            try
            {
                byte[] bValue = BitConverter.GetBytes(Value);
                return SetValue(nRowIndex, nColIndex, ref bValue);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
            }
            return -2;
        }
        public int SetValue(int nRowIndex, int nColIndex, Int64 Value)
        {
            try
            {
                byte[] bValue = BitConverter.GetBytes(Value);
                return SetValue(nRowIndex, nColIndex, ref bValue);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
            }
            return -2;
        }
        public int SetValue(int nRowIndex, int nColIndex, UInt64 Value)
        {
            try
            {
                byte[] bValue = BitConverter.GetBytes(Value);
                return SetValue(nRowIndex, nColIndex, ref bValue);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
            }
            return -2;
        }
        public int SetValue(int nRowIndex, int nColIndex, Byte Value)
        {
            try
            {
                byte[] bValue = BitConverter.GetBytes(Value);
                return SetValue(nRowIndex, nColIndex, ref bValue);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
            }
            return -2;
        }
        public int SetValue(int nRowIndex, int nColIndex, SByte Value)
        {
            try
            {
                byte[] bValue = BitConverter.GetBytes(Value);
                return SetValue(nRowIndex, nColIndex, ref bValue);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
            }
            return -2;
        }
        public int SetValue(int nRowIndex, int nColIndex,ref SByte[] Value)
        {
            
            return SetValue(nRowIndex, nColIndex, ref Value);
        }
        public int SetValue(int nRowIndex, int nColIndex, Single Value)
        {
            try
            {
                byte[] bValue = BitConverter.GetBytes(Value);
                return SetValue(nRowIndex, nColIndex, ref bValue);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
            }
            return -2;
        }
        public int SetValue(int nRowIndex, int nColIndex, Double Value)
        {
            try
            {
                byte[] bValue = BitConverter.GetBytes(Value);
                return SetValue(nRowIndex, nColIndex, ref bValue);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
            }
            return -2;
        }
        public int SetValue(int nRowIndex, int nColIndex, String Value)
        {
            try
            {
                byte[] bValue = System.Text.Encoding.Default.GetBytes(Value);
                return SetValue(nRowIndex, nColIndex, ref bValue);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
            }
            return -2;
        }
        public int SetValue(int nRowIndex, int nColIndex, Decimal Value)
        {
            byte[] bDecimal;
            try
            {
                int[] nValue = Decimal.GetBits(Value);
                byte[] bValue = new byte[nValue.Length * 4];
                int i, j;
                for (i = 0; i < nValue.Length; i++)
                {
                    bDecimal = BitConverter.GetBytes(nValue[i]);
                    for (j = 0; j < 4; j++)
                        bValue[i * 4 + j] = bDecimal[j];
                }
                return SetValue(nRowIndex, nColIndex, ref bValue);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
            }
            return -2;
        }
        public int SetValue(int nRowIndex, int nColIndex, DateTime Value)
        {
            try
            {
                Int64 nValue = Value.ToBinary();
                byte[] bValue = BitConverter.GetBytes(nValue);
                return SetValue(nRowIndex, nColIndex, ref bValue);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
            }
            return -2;
        }
        public int SetValue(int nRowIndex, int nColIndex, Boolean Value)
        {
            try
            {
                byte[] bValue = BitConverter.GetBytes(Value);
                return SetValue(nRowIndex, nColIndex, ref bValue);
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
            }
            return -2;
        }
        public int SetValue(int nRowIndex, int nColIndex,ref byte[] Value)
        {
            if (_IsValidIndex(nRowIndex, nColIndex) < 0)
                return -1;
            int nPos = nRowIndex * m_pTableParam.nBytesPerRow + m_pColumnParam[nColIndex].nOffset;
            nPos += m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable;
            int i;
            try
            {
                for (i = 0; i < m_pColumnParam[nColIndex].nSize && i < Value.Length; i++, nPos++)
                    m_pData[nPos] = Value[i];
                for (; i < m_pColumnParam[nColIndex].nSize; i++, nPos++)
                    m_pData[nPos] = 0;
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
                return -2;
            }
            return m_pColumnParam[nColIndex].nSize;
        }
        public int SetValueByString(int nRowIndex, int nColIndex, String Value)
        {
            if (_IsValidIndex(nRowIndex, nColIndex) < 0)
                return -1;

            byte[] bValue = null;
            int nResult = _StringToBytes(m_pColumnParam[nColIndex].nType, Value, ref bValue);
            if(nResult < 0)
                return nResult;
            int nPos = nRowIndex * m_pTableParam.nBytesPerRow + m_pColumnParam[nColIndex].nOffset;
            nPos += m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable;
            int i;
            for (i = 0; i < bValue.Length && i < m_pColumnParam[nColIndex].nSize; i++, nPos++)
                m_pData[nPos] = bValue[i];
            return m_pColumnParam[nColIndex].nSize;
        }
        public int AppendRow(ref byte[] Value)
        {
            if (Value.Length != m_pTableParam.nBytesPerRow)
            {
                m_sErrorInfo = "Value Length Is Not Equal";
                return -1;
            }
            int nRowIndex = AddRow();
            int nPos = nRowIndex * m_pTableParam.nBytesPerRow;
            nPos += m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable;
            int i;
            for (i = 0; i < m_pTableParam.nBytesPerRow; i++, nPos++)
                m_pData[nPos] = Value[i];
            return m_pTableParam.nBytesPerRow;
        }
        public int SetRow(int nRowIndex, ref byte[] Value)
        {
            if (_IsValidIndexOfRow(nRowIndex) < 0)
                return -1;
            if (Value.Length != m_pTableParam.nBytesPerRow)
            {
                m_sErrorInfo = "Value Length Is Not Equal";
                return -1;
            }
            int nPos = nRowIndex * m_pTableParam.nBytesPerRow;
            nPos += m_nStructLenOfColumn * m_pTableParam.nColumnCount + m_nStructLenOfTable;
            int i;
            for (i = 0; i < m_pTableParam.nBytesPerRow; i++, nPos++)
                m_pData[nPos] = Value[i];
            return m_pTableParam.nBytesPerRow;
        }
        protected int StringToBytes(Type pType, string Value, ref byte[] bValue)
        {
            int nType = _TypeToInt(pType);
            if (nType == 0)
            {
                m_sErrorInfo = "Not Supported Type";
                return -1;
            }
            return _StringToBytes(nType, Value, ref bValue);
        }
        /////////////////////////
        protected int _StringToBytes(int nType, string Value, ref byte[] bValue)
        {
            int i, j, nLen;
            try
            {
                if (nType == 1)
                    bValue = BitConverter.GetBytes(Int16.Parse(Value));
                else if (nType == 2)
                    bValue = BitConverter.GetBytes(UInt16.Parse(Value));
                else if (nType == 3)
                    bValue = BitConverter.GetBytes(Int32.Parse(Value));
                else if (nType == 4)
                    bValue = BitConverter.GetBytes(UInt32.Parse(Value));
                else if (nType == 5)
                    bValue = BitConverter.GetBytes(Int64.Parse(Value));
                else if (nType == 6)
                    bValue = BitConverter.GetBytes(UInt64.Parse(Value));
                else if (nType == 7)
                    bValue = BitConverter.GetBytes(Byte.Parse(Value));
                else if (nType == 8)
                    bValue = BitConverter.GetBytes(SByte.Parse(Value));
                else if (nType == 9)
                {
                    bValue = new byte[Value.Length / 2];
                    nLen = (Value.Length / 2) * 2;
                    for (i = j = 0; i < nLen; i += 2, j++)
                        bValue[j] = Convert.ToByte(Value.Substring(i, 2));
                }
                else if (nType == 10)
                {
                    bValue = new byte[Value.Length / 2];
                    nLen = (Value.Length / 2) * 2;
                    for (i = j = 0; i < nLen; i += 2, j++)
                        bValue[j] = (Byte)Convert.ToSByte(Value.Substring(i, 2));
                }
                else if (nType == 11)
                    bValue = System.Text.Encoding.Default.GetBytes(Value);
                else if (nType == 12)
                    bValue = BitConverter.GetBytes(Single.Parse(Value));
                else if (nType == 13)
                    bValue = BitConverter.GetBytes(Double.Parse(Value));
                else if (nType == 14)
                {
                    Decimal pDecimal = Convert.ToDecimal(Value);
                    byte[] bDecimal;
                    int[] nValue = Decimal.GetBits(pDecimal);
                    bValue = new byte[nValue.Length * 4];
                    for (i = 0; i < nValue.Length; i++)
                    {
                        bDecimal = BitConverter.GetBytes(nValue[i]);
                        for (j = 0; j < 4; j++)
                            bValue[i * 4 + j] = bDecimal[j];
                    }
                }
                else if (nType == 15)
                {
                    DateTime cDateTime = Convert.ToDateTime(Value);
                    Int64 nValue = cDateTime.ToBinary();
                    bValue = BitConverter.GetBytes(nValue);
                }
                else if (nType == 16)
                {
                    bValue = BitConverter.GetBytes(Boolean.Parse(Value));
                }
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
                return -2;
            }
            return 0;
        }
    }
}
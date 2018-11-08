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
        //////////////////////////////////////////
        //Write File
        public int WriteToBinary(string sFileName)
        {
            return WriteToBinary(sFileName, false);
        }
        public int WriteToBinary(string sFileName, bool bAppend)
        {
            try
            {
                FileStream cFile;
                if (bAppend)
                {
                    cFile = new FileStream(sFileName, FileMode.OpenOrCreate);
                    cFile.Seek(0, SeekOrigin.End);
                }
                else
                    cFile = new FileStream(sFileName, FileMode.Create);

                cFile.Write(m_pData, 0, m_nDataLen);
                cFile.Close();
                cFile.Dispose();
                cFile = null;
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
                return -2;
            }
            return 0;
        }
        public int WriteToText()
        {
            return WriteToText(m_pTableParam.sName + ".txt", 1, false);
        }
        public int WriteToText(string sFileName)
        {
            return WriteToText(sFileName, 1, false);
        }
        public int WriteToText(string sFileName, int bIsWriteColumn, bool bAppend)
        {
            try
            {
                FileStream cFile;
                if (bAppend)
                {
                    cFile = new FileStream(sFileName, FileMode.OpenOrCreate);
                    cFile.Seek(0, SeekOrigin.End);
                }
                else
                    cFile = new FileStream(sFileName, FileMode.Create);
                //Write Column
                string sValue = "";
                byte[] bValue = null;
                byte[] bContent = new byte[1024 * 1024];
                int nContentLen = 0;
                int i, j;
                if (bIsWriteColumn != 0)
                {
                    int nPos;
                    for (i = 0; i < m_pTableParam.nColumnCount; i++)
                    {
                        nPos = i * m_nStructLenOfColumn + 4 + m_nStructLenOfTable;
                        for (j = 0; j < 256; j++, nPos++)
                        {
                            if (m_pData[nPos] == 0)
                            {
                                if ((i + 1) < m_pTableParam.nColumnCount)
                                {
                                    bContent[nContentLen] = (byte)',';
                                    nContentLen++;
                                }
                                break;
                            }
                            bContent[nContentLen] = m_pData[nPos];
                            nContentLen++;
                        }
                    }
                    bContent[nContentLen] = (byte)'\r';
                    nContentLen++;
                    bContent[nContentLen] = (byte)'\n';
                    nContentLen++;
                }
                for (i = 0; i < m_pTableParam.nRowCount; i++)
                {
                    if (GetRow(i, ref sValue) < 0)
                        break;
                    bValue = System.Text.Encoding.Default.GetBytes(sValue);
                    if ((bValue.Length + nContentLen + 10) > bContent.Length)
                    {
                        int nLen = ((bValue.Length + nContentLen + 10) / (1024 * 1024)) * (1024 * 1024) + (1024 * 1024);
                        Array.Resize(ref bContent, nLen);
                    }
                    for (j = 0; j < bValue.Length; j++, nContentLen++)
                        bContent[nContentLen] = bValue[j];
                    bContent[nContentLen] = (byte)'\r';
                    nContentLen++;
                    bContent[nContentLen] = (byte)'\n';
                    nContentLen++;
                }
                cFile.Write(bContent, 0, nContentLen);
                cFile.Close();
                cFile.Dispose();
                cFile = null;
            }
            catch (Exception e)
            {
                m_sErrorInfo = e.Message;
                return -2;
            }
            return 0;
        }
        ////////////////////////////////////////////////////////////
        public int ReadFromBinary(string sFileName)
        {
            int nFileLen;
            try
            {
                if (!File.Exists(sFileName))
                    return -1;
                m_pTableParam.nBytesPerRow = 0;
                m_pTableParam.nColumnCount = 0;
                m_pTableParam.nRowCount = 0;
                m_nDataLen = 0;
                FileStream cFile;
                cFile = new FileStream(sFileName, FileMode.Open);
                nFileLen = (int)cFile.Length;
                if (nFileLen >= m_nStructLenOfTable)
                {

                    if (m_pData == null)
                        m_pData = new byte[nFileLen];
                    else
                    {
                        if (m_pData.Length < nFileLen)
                            Array.Resize(ref m_pData, nFileLen);
                    }
                    cFile.Read(m_pData, 0, nFileLen);

                    _GetParamFromByte(ref m_pTableParam);
                    if (m_pTableParam.nColumnCount > m_nColumnParamLenPre)
                    {
                        try
                        {
                            Array.Resize(ref m_pColumnParam, m_pTableParam.nColumnCount + 10);
                            m_nColumnParamLenPre = m_pTableParam.nColumnCount + 10;
                        }
                        catch (Exception e)
                        {
                            m_sErrorInfo = e.Message;
                            return -2;
                        }
                    }
                    int i;
                    for (i = 0; i < m_pTableParam.nColumnCount; i++)
                    {
                        _GetParamFromByte(i, ref m_pColumnParam[i]);
                    }
                    m_nDataLen = nFileLen;
                }
                cFile.Close();
                cFile.Dispose();
                cFile = null;
            }
            catch (Exception e)
            {
                string m_sErrorInfo = e.Message;
                return -2;
            }
            return m_pTableParam.nRowCount;
        }
    }
}
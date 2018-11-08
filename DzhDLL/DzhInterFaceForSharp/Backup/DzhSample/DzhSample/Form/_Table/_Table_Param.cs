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
        //change struct to bytes and change struct from bytes
        int _SetParamToByte(int nIndex, ref COLUMN_PARAM pParam)
        {
            int nStructLen = Marshal.SizeOf(pParam);
            int nPos = nIndex * nStructLen + Marshal.SizeOf(typeof(TABLE_PARAM));
            IntPtr pStructPtr = Marshal.AllocHGlobal(nStructLen);
            Marshal.StructureToPtr(pParam, pStructPtr, false);
            Marshal.Copy(pStructPtr, m_pData, nPos, nStructLen);
            Marshal.FreeHGlobal(pStructPtr);
            return 0;
        }
        int _GetParamFromByte(int nIndex, ref COLUMN_PARAM pParam)
        {
            int nStructLen = Marshal.SizeOf(pParam);
            int nPos = nIndex * nStructLen + Marshal.SizeOf(typeof(TABLE_PARAM));

            IntPtr pStructPtr = Marshal.AllocHGlobal(nStructLen);
            Marshal.Copy(m_pData, nPos, pStructPtr, nStructLen);
            pParam = (COLUMN_PARAM)Marshal.PtrToStructure(pStructPtr, pParam.GetType());
            Marshal.FreeHGlobal(pStructPtr);
            return 0;
        }
        int _SetParamToByte(ref TABLE_PARAM pParam)
        {
            int nStructLen = Marshal.SizeOf(pParam);
            int nPos = 0;
            IntPtr pStructPtr = Marshal.AllocHGlobal(nStructLen);
            Marshal.StructureToPtr(pParam, pStructPtr, false);
            Marshal.Copy(pStructPtr, m_pData, nPos, nStructLen);
            Marshal.FreeHGlobal(pStructPtr);
            return 0;
        }
        int _GetParamFromByte(ref TABLE_PARAM pParam)
        {
            int nStructLen = Marshal.SizeOf(pParam);
            int nPos = 0;

            IntPtr pStructPtr = Marshal.AllocHGlobal(nStructLen);
            Marshal.Copy(m_pData, nPos, pStructPtr, nStructLen);
            pParam = (TABLE_PARAM)Marshal.PtrToStructure(pStructPtr, pParam.GetType());
            Marshal.FreeHGlobal(pStructPtr);
            return 0;
        }
    }
}
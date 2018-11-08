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
        //type to int
        int _GetSize(Type pType)
        {
            if (pType == typeof(Int16))
                return 2;
            else if (pType == typeof(UInt16))
                return 2;
            else if (pType == typeof(Int32))
                return 4;
            else if (pType == typeof(UInt32))
                return 4;
            else if (pType == typeof(Int64))
                return 8;
            else if (pType == typeof(UInt64))
                return 8;
            else if (pType == typeof(Byte))
                return 1;
            else if (pType == typeof(SByte))
                return 1;
            else if (pType == typeof(Byte[]))
                return 0;
            else if (pType == typeof(SByte[]))
                return 0;
            else if (pType == typeof(String))
                return 0;
            else if (pType == typeof(Single))
                return 4;
            else if (pType == typeof(Double))
                return 8;
            else if (pType == typeof(Decimal))
                return 16;
            else if (pType == typeof(DateTime))
                return 8;
            else if (pType == typeof(Boolean))
                return 4;
            return 0;
        }
        int _GetSize(int nType)
        {
            if (nType == 1)
                return 2;
            else if (nType == 2)
                return 2;
            else if (nType == 3)
                return 4;
            else if (nType == 4)
                return 4;
            else if (nType == 5)
                return 8;
            else if (nType == 6)
                return 8;
            else if (nType == 7)
                return 1;
            else if (nType == 8)
                return 1;
            else if (nType == 9)
                return 0;
            else if (nType == 10)
                return 0;
            else if (nType == 11)
                return 0;
            else if (nType == 12)
                return 4;
            else if (nType == 13)
                return 8;
            else if (nType == 14)
                return 16;
            else if (nType == 15)
                return 8;
            else if (nType == 16)
                return 4;
            return 0;
        }
        int _TypeToInt(Type pType)
        {
            if (pType == typeof(Int16))
                return 1;
            else if (pType == typeof(UInt16))
                return 2;
            else if (pType == typeof(Int32))
                return 3;
            else if (pType == typeof(UInt32))
                return 4;
            else if (pType == typeof(Int64))
                return 5;
            else if (pType == typeof(UInt64))
                return 6;
            else if (pType == typeof(Byte))
                return 7;
            else if (pType == typeof(SByte))
                return 8;
            else if (pType == typeof(Byte[]))
                return 9;
            else if (pType == typeof(SByte[]))
                return 10;
            else if (pType == typeof(String))
                return 11;
            else if (pType == typeof(Single))
                return 12;
            else if (pType == typeof(Double))
                return 13;
            else if (pType == typeof(Decimal))
                return 14;
            else if (pType == typeof(DateTime))
                return 15;
            else if (pType == typeof(Boolean))
                return 16;
            return 0;
        }
        Type _IntToType(int nType)
        {
            if (nType == 1)
                return typeof(Int16);
            else if (nType == 2)
                return typeof(UInt16);
            else if (nType == 3)
                return typeof(Int32);
            else if (nType == 4)
                return typeof(UInt32);
            else if (nType == 5)
                return typeof(Int64);
            else if (nType == 6)
                return typeof(UInt64);
            else if (nType == 7)
                return typeof(Byte);
            else if (nType == 8)
                return typeof(SByte);
            else if (nType == 9)
                return typeof(Byte[]);
            else if (nType == 10)
                return typeof(SByte[]);
            else if (nType == 11)
                return typeof(String);
            else if (nType == 12)
                return typeof(Single);
            else if (nType == 13)
                return typeof(Double);
            else if (nType == 14)
                return typeof(Decimal);
            else if (nType == 15)
                return typeof(DateTime);
            else if (nType == 16)
                return typeof(Boolean);
            return null;
        }
    }
}
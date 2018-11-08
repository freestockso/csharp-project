using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaQuant.Data.DZH
{
    public class DZHStripReader : DZHReader
    {
        protected DZHStripReader(string path)
            : base(path)
        {
            fileName = "STKINFO70.DAT";
            indexRecordSize = 320;
            dataRecordSize = 320;
            securityCountOffset = 8;
        }

        protected override void OnFileChanged()
        {
            fileStream.Position =securityCountOffset;
            securityCount = reader.ReadUInt32();
            indexStartOffset = fileStream.Length - securityCount * indexRecordSize;
        }


    }
}

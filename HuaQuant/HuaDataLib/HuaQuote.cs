using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SmartQuant.Data;

namespace HuaQuant.Data
{
    public class HuaQuote : Quote
    {
        private double bid1;
        private int bid1Size;
        private double bid2;
        private int bid2Size;
        private double bid3;
        private int bid3Size;
        private double bid4;
        private int bid4Size;

        private double ask1;
        private int ask1Size;
        private double ask2;
        private int ask2Size;
        private double ask3;
        private int ask3Size;
        private double ask4;
        private int ask4Size;

        [View]
        public double Bid1
        {
            get { return bid1; }
            set { bid1 = value; }
        }
        [View]
        public int BidSize1
        {
            get { return bid1Size; }
            set { bid1Size = value; }
        }
        [View]
        public double Bid2
        {
            get { return bid2; }
            set { bid2 = value; }
        }
        [View]
        public int BidSize2
        {
            get { return bid2Size; }
            set { bid2Size = value; }
        }
        [View]
        public double Bid3
        {
            get { return bid3; }
            set { bid3 = value; }
        }
        [View]
        public int BidSize3
        {
            get { return bid3Size; }
            set { bid3Size = value; }
        }
        [View]
        public double Bid4
        {
            get { return bid4; }
            set { bid1 = value; }
        }
        [View]
        public int BidSize4
        {
            get { return bid4Size; }
            set { bid4Size = value; }
        }

        [View]
        public double Ask1
        {
            get {return ask1;}
            set {ask1=value;}
        }
        [View]
        public int Ask1Size
        {
            get { return ask1Size; }
            set { ask1Size = value; }
        }
        [View]
        public double Ask2
        {
            get { return ask2; }
            set { ask2 = value; }
        }
        [View]
        public int Ask2Size
        {
            get { return ask2Size; }
            set { ask2Size = value; }
        }
        [View]
        public double Ask3
        {
            get { return ask3; }
            set { ask3 = value; }
        }
        [View]
        public int Ask3Size
        {
            get { return ask3Size; }
            set { ask3Size = value; }
        }
        [View]
        public double Ask4
        {
            get { return ask4; }
            set { ask4 = value; }
        }
        [View]
        public int Ask4Size
        {
            get { return ask4Size; }
            set { ask4Size = value; }
        }


        public HuaQuote()
        {
        }
        public HuaQuote(DateTime datetime, double bid, int bidSize, double ask, int askSize,
            double bid1, int bid1Size, double ask1, int ask1Size, double bid2, int bid2Size, double ask2, int ask2Size,
            double bid3, int bid3Size, double ask3, int ask3Size, double bid4, int bid4Size, double ask4, int ask4Size)
            : base(datetime, bid, bidSize, ask, askSize)
        {
            this.bid1 = bid1;
            this.bid1Size = bid1Size;
            this.ask1 = ask1;
            this.ask1Size = ask1Size;
            this.bid2 = bid2;
            this.bid2Size = bid2Size;
            this.ask2 = ask2;
            this.ask2Size = ask2Size;
            this.bid3 = bid3;
            this.bid3Size = bid3Size;
            this.ask3 = ask3;
            this.ask3Size = ask3Size;
            this.bid4 = bid4;
            this.bid4Size = bid4Size;
            this.ask4 = ask4;
            this.ask4Size = ask4Size;
        }

        public HuaQuote(HuaQuote quote)
            : base(quote)
        {
            this.bid1 = quote.bid1;
            this.bid1Size = quote.bid1Size;
            this.ask1 = quote.ask1;
            this.ask1Size = quote.ask1Size;
            this.bid2 = quote.bid2;
            this.bid2Size = quote.bid2Size;
            this.ask2 = quote.ask2;
            this.ask2Size = quote.ask2Size;
            this.bid3 = quote.bid3;
            this.bid3Size = quote.bid3Size;
            this.ask3 = quote.ask3;
            this.ask3Size = quote.ask3Size;
            this.bid4 = quote.bid4;
            this.bid4Size = quote.bid4Size;
            this.ask4 = quote.ask4;
            this.ask4Size = quote.ask4Size;
        }

        public override ISeriesObject NewInstance()
        {
            return new HuaQuote(this);
        }
        public override object Clone()
        {
            return new HuaQuote(this);
        }
        public override void WriteTo(BinaryWriter writer)
        {

            base.WriteTo(writer);
            writer.Write(bid1);
            writer.Write(bid1Size);
            writer.Write(ask1);
            writer.Write(ask1Size);
            writer.Write(bid2);
            writer.Write(bid2Size);
            writer.Write(ask2);
            writer.Write(ask2Size);
            writer.Write(bid3);
            writer.Write(bid3Size);
            writer.Write(ask3);
            writer.Write(ask3Size);
            writer.Write(bid4);
            writer.Write(bid4Size);
            writer.Write(ask4);
            writer.Write(ask4Size);
        }

        public override void ReadFrom(BinaryReader reader)
        {
            base.ReadFrom(reader);
            bid1 = reader.ReadDouble();
            bid1Size = reader.ReadInt32();
            ask1 = reader.ReadDouble();
            ask1Size = reader.ReadInt32();
            bid2 = reader.ReadDouble();
            bid2Size = reader.ReadInt32();
            ask2 = reader.ReadDouble();
            ask2Size = reader.ReadInt32();
            bid3 = reader.ReadDouble();
            bid3Size = reader.ReadInt32();
            ask3 = reader.ReadDouble();
            ask3Size = reader.ReadInt32();
            bid4 = reader.ReadDouble();
            bid4Size = reader.ReadInt32();
            ask4 = reader.ReadDouble();
            ask4Size = reader.ReadInt32();
        }
    }
}

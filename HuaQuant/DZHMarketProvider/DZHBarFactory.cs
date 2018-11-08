using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using SmartQuant.Providers;
using SmartQuant.Instruments;
using SmartQuant.Data;
using SmartQuant.FIX;
using SmartQuant;

namespace HuaQuant.Data.DZH
{
    public class DZHBarFactory:IBarFactory
    {
        private class BarData
        {
            public Bar Bar;
            public long TickCount = 1L;
            public DateTime RealyEndTime;
        }
        private enum BarEventType
        {
            Open,
            New
        }
        private class BarEvent
        {
            public BarEventType BarEventType;
            public Bar Bar;
            public BarEvent(BarEventType barEventType, Bar bar)
            {
                this.BarEventType = barEventType;
                this.Bar = bar;
            }
        }
        
        private bool enabled;
        private BarFactoryInput input;
        private BarFactoryItemList items;
        private IMarketDataProvider provider;
        private ArrayList times;
        private Dictionary<IFIXInstrument, Dictionary<BarType, Dictionary<long, object>>> barStore;

        public DZHBarFactory()
            : this(true,null)
        {
        }
        public DZHBarFactory(bool enabled,IMarketDataProvider provider)
        {
            this.enabled = enabled;
            this.provider = provider;
            this.input = BarFactoryInput.Trade;
            this.items = new BarFactoryItemList();
            this.items.Add(new BarFactoryItem(BarType.Time, 60, true));
            this.barStore = new Dictionary<IFIXInstrument, Dictionary<BarType, Dictionary<long, object>>>();
            this.times = new ArrayList();
        }
        
        
        public bool Enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }  
        public BarFactoryInput Input
        {
            get { return this.input; }
            set { this.input = value; }
        }
        public BarFactoryItemList Items
        {
            get { return this.items; }
        }
        public override string ToString()
        {
            return "DZH Bar Factory";
        }

        public event BarEventHandler NewBar; 
        public event BarEventHandler NewBarOpen;
        public event BarSliceEventHandler NewBarSlice;

        public void EmitNewBar(Bar bar, IFIXInstrument instrument)
        {
            bar.IsComplete = true;
            if (this.NewBar != null)
            {
                this.NewBar(null, new BarEventArgs(bar, instrument, provider));
            }
        }
        public void EmitNewBarOpen(Bar bar, IFIXInstrument instrument)
        {
            if (this.NewBarOpen != null)
            {
                this.NewBarOpen(null, new BarEventArgs(bar, instrument, provider));
            }
        }
        public void EmitNewBarSlice(long barSize)
        {
            if (this.NewBarSlice != null)
            {
                this.NewBarSlice(null, new BarSliceEventArgs(barSize, provider));
            }
        }

       
        public void OnNewQuote(IFIXInstrument instrument, Quote quote)
        {
            //throw new NotImplementedException();//出价喊价不产生bar
        }

        public void OnNewTrade(IFIXInstrument instrument, Trade trade)
        {
            if (!this.enabled)
            {
                return;
            }
            if (this.input == BarFactoryInput.Trade)
            {
                this.OnNewMarketData(instrument, trade.DateTime, trade.Price, trade.Size);
            }
        }

        public void Reset()
        {
            this.barStore.Clear();
            this.times.Clear();
        }

        private void OnNewMarketData(IFIXInstrument instrument, DateTime datetime, double price, int size)
        {
            List<BarEvent> barEventList = new List<BarEvent>();
            Monitor.Enter(this);
            try
            {
                if (!barStore.ContainsKey(instrument)) barStore.Add(instrument, new Dictionary<BarType, Dictionary<long, object>>());
                Dictionary<BarType, Dictionary<long, object>> barCabinet = barStore[instrument];
                foreach (BarFactoryItem barFactoryItem in this.items)
                {
                    if (barFactoryItem.Enabled)
                    {                                             
                        if (!barCabinet.ContainsKey(barFactoryItem.BarType)) barCabinet.Add(barFactoryItem.BarType, new Dictionary<long, object>());
                        Dictionary<long, object> barBox = barCabinet[barFactoryItem.BarType];

                        switch (barFactoryItem.BarType)
                        {
                            //产生以时间度量的bar
                            /**
                             * 这里有个问题就是如果本地机器时间与发过来交易价格中的datetime不同步的话
                             * 靠Clock的Reminder事件来结束一个bar的产生的话，有可能会出错,所以在每次一
                             * 个新Bar的Open事件前，基于Clock的当前时间对产生Bar结束事件的reminder时间
                             * 进行调整计算
                            */
                            case BarType.Time:
                                {
                                    BarData barData;                             
                                    if (barBox.ContainsKey(barFactoryItem.BarSize))
                                    {
                                        barData = (BarData)barBox[barFactoryItem.BarSize];
                                        if (price > barData.Bar.High)
                                        {
                                            barData.Bar.High = price;
                                        }
                                        if (price < barData.Bar.Low)
                                        {
                                            barData.Bar.Low = price;
                                        }
                                        barData.Bar.Close = price;
                                        barData.Bar.Volume += (long)size;                                    
                                    }
                                    else
                                    {
                                        DateTime barDateTime = this.GetBarDateTime(datetime, barFactoryItem.BarSize);
                                        DateTime realyEndTime=Clock.Now.AddSeconds(barFactoryItem.BarSize-(datetime-barDateTime).TotalSeconds);//bar真实结束的本地时间
                                        barData=new BarData{
                                            Bar = new Bar(BarType.Time, barFactoryItem.BarSize, barDateTime, barDateTime.AddSeconds(barFactoryItem.BarSize), price, price, price, price, (long)size, 0L),
                                            RealyEndTime=realyEndTime
                                        };                                        
                                        barBox.Add(barFactoryItem.BarSize, barData);
                                        barEventList.Add(new BarEvent(BarEventType.Open, barData.Bar));
                                        this.AddReminder(barData.RealyEndTime);                                        
                                    }
                                    break;
                                }
                                //产生以tick度量的bar
                            case BarType.Tick:
                                {
                                    BarData barData;
                                    if (barBox.ContainsKey(barFactoryItem.BarSize))
                                    {
                                        barData = (BarData)barBox[barFactoryItem.BarSize];
                                        barData.Bar.EndTime = datetime;
                                        if (price > barData.Bar.High)
                                        {
                                            barData.Bar.High = price;
                                        }
                                        if (price < barData.Bar.Low)
                                        {
                                            barData.Bar.Low = price;
                                        }
                                        barData.Bar.Close = price;
                                        barData.Bar.Volume +=(long) size;
                                        barData.TickCount+=1L;
                                    }
                                    else
                                    {
                                        barData = new BarData
                                        {
                                            Bar = new Bar(BarType.Tick, barFactoryItem.BarSize, datetime, datetime, price, price, price, price, (long)size, 0L)
                                        };
                                        barBox.Add(barFactoryItem.BarSize, barData);
                                        barEventList.Add(new BarEvent(BarEventType.Open, barData.Bar));
                                    }
                                    if (barData.TickCount == barFactoryItem.BarSize)
                                    {
                                        barEventList.Add(new BarEvent(BarEventType.New, barData.Bar));
                                        barBox.Remove(barFactoryItem.BarSize);
                                    }
                                    break;
                                }
                                //产生以成交量度量的bar
                            case BarType.Volume:
                                {
                                    Bar bar2;
                                    if (barBox.ContainsKey(barFactoryItem.BarSize))
                                    {
                                        bar2 = (Bar)barBox[barFactoryItem.BarSize];
                                        bar2.EndTime = datetime;
                                        if (price > bar2.High)
                                        {
                                            bar2.High = price;
                                        }
                                        if (price < bar2.Low)
                                        {
                                            bar2.Low = price;
                                        }
                                        bar2.Close = price;
                                        bar2.Volume += (long)size;
                                    }
                                    else
                                    {
                                        bar2 = new Bar(BarType.Volume, barFactoryItem.BarSize, datetime, datetime, price, price, price, price, (long)size, 0L);
                                        barBox.Add(barFactoryItem.BarSize, bar2);
                                        barEventList.Add(new BarEvent(BarEventType.Open, bar2));
                                    }
                                    if (bar2.Volume >= barFactoryItem.BarSize)
                                    {
                                        barEventList.Add(new BarEvent(BarEventType.New, bar2));
                                        barBox.Remove(barFactoryItem.BarSize);
                                    }
                                    break;
                                }
                            case BarType.Dynamic:
                                break;
                            default:
                                throw new NotSupportedException("The bar factory does not support bar type - " + barFactoryItem.BarType.ToString());
                        }
                        if (barBox.Count == 0)
                        {
                            barCabinet.Remove(barFactoryItem.BarType);
                            if (barCabinet.Count == 0) barStore.Remove(instrument);
                        }
                    }
                }
            }
            finally
            {
                Monitor.Exit(this);
            }
            foreach (BarEvent current in barEventList)
            {
                switch (current.BarEventType)
                {
                    case BarEventType.Open:
                        {
                            Bar bar3 = current.Bar;
                            this.EmitNewBarOpen(bar3, instrument);
                            break;
                        }
                    case BarEventType.New:
                        this.EmitNewBar(current.Bar, instrument);
                        break;
                }
            }
        }
        private DateTime GetBarDateTime(DateTime tradeDateTime, long barSize)
        {
            long num = ((long)tradeDateTime.TimeOfDay.TotalSeconds) / barSize * barSize;
            return tradeDateTime.Date.AddSeconds((double)num);
        }

        private void AddReminder(DateTime datetime)
        {            
            if (!this.times.Contains(datetime)&&datetime>=Clock.Now)
            {
                Clock.AddReminder(new ReminderEventHandler(this.OnReminder), datetime, null);
                this.times.Add(datetime);
            }
        }
        private void OnReminder(ReminderEventArgs args)
        {
            Dictionary<long, Dictionary<IFIXInstrument, Bar>> barFinishedStore = new Dictionary<long, Dictionary<IFIXInstrument, Bar>>();
            Monitor.Enter(this);
            try
            {
                foreach (KeyValuePair<IFIXInstrument, Dictionary<BarType, Dictionary<long, object>>> kvp1 in barStore.ToList())
                {
                    IFIXInstrument instrument = kvp1.Key;
                    Dictionary<BarType, Dictionary<long, object>> barCabinet = barStore[instrument];

                    if (barCabinet.ContainsKey(BarType.Time))
                    {
                        Dictionary<long, object> barBox = barCabinet[BarType.Time];
                        foreach (KeyValuePair<long, object> kvp2 in barBox.ToList())
                        {
                            long num = kvp2.Key;
                            BarData barData = (BarData)kvp2.Value;
                            if (barData.RealyEndTime == args.SignalTime)
                            {
                                if (!barFinishedStore.ContainsKey(num)) barFinishedStore.Add(num, new Dictionary<IFIXInstrument, Bar>());
                                Dictionary<IFIXInstrument, Bar> barFinishedCabinet = barFinishedStore[num];
                                barFinishedCabinet.Add(instrument, barData.Bar);
                                barBox.Remove(num);
                            }
                        }
                        if (barBox.Count == 0) barCabinet.Remove(BarType.Time);
                    }
                    if (barCabinet.Count == 0) barStore.Remove(instrument);
                }
            }
            finally
            {
                Monitor.Exit(this);
            }
            foreach (KeyValuePair<long, Dictionary<IFIXInstrument, Bar>> kvp3 in barFinishedStore)
            {
                long barSize = kvp3.Key;
                Dictionary<IFIXInstrument, Bar> barFinishedCabinet = kvp3.Value;
                foreach (KeyValuePair<IFIXInstrument, Bar> pair in barFinishedCabinet)
                {
                    this.EmitNewBar(pair.Value, pair.Key);
                }
                this.EmitNewBarSlice(barSize);
            }
            this.times.Remove(args.SignalTime);
        }
    }
}

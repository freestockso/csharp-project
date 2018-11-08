using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.ComponentModel;
using SmartQuant;
using SmartQuant.Data;
using SmartQuant.FIX;
using SmartQuant.Instruments;
using SmartQuant.Providers;

namespace HuaQuant.Data.DZH
{
    public partial class DZHMarketProvider:IProvider,IMarketDataProvider,IDisposable
    {
        public DZHMarketProvider()
        {
            this.BarFactory = new DZHBarFactory(true,this);
            ProviderManager.Add(this);
        }
        private string dzhDataPath = "";
       
        [Category("通用设置"), Description("大智慧数据文件夹"), DefaultValue(@"c:\dzh2\data\")]
        public string DZHDataPath
        {
            get { return this.dzhDataPath; }
            set { this.dzhDataPath = value.Trim(); }
        }
        #region IDisposable 
        private bool disposed;
        ~DZHMarketProvider()
        {
            Dispose(false);
        }
        //Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                }
                // Free your own state (unmanaged objects).
                // Set large fields to null.
                if (quoteReader != null) quoteReader.Dispose();
                if (tickTimer != null) tickTimer.Dispose();
                disposed = true;
            }
            //base.Dispose(disposing);
        }
        #endregion
        #region IProvider 成员
        private bool isConnected = false;
        public void Connect(int timeout)
        {
            this.Connect();
            ProviderManager.WaitConnected(this, timeout);
        }

        public void Connect()
        {
            if (System.IO.Directory.Exists(this.dzhDataPath))
            {
                EmitStatusChangedEvent();
                isConnected = true;
                EmitConnectedEvent();
            }
            else
            {
                this.EmitError(-1, -1, "大智慧数据文件夹不存在:" + this.dzhDataPath);
            }
        }

        public void Disconnect()
        {
            EmitStatusChangedEvent();
            isConnected = false;
            EmitDisconnectedEvent();
        }
        [Category("信息")]
        public byte Id
        {
            get { return 251; }
        }
        [Category("信息")]
        public bool IsConnected
        {
            get { return this.isConnected; }
        }
        [Category("信息")]
        public string Name
        {
            get { return "DZHMarketProvider"; }
        }
        public void Shutdown()
        {
            this.BarFactory = null;
            this.Disconnect();
            this.Dispose();
        }
        [Category("信息")]
        public ProviderStatus Status
        {
            get
            {
                if (!IsConnected)
                    return ProviderStatus.Disconnected;
                else
                    return ProviderStatus.Connected;
            }
        }
        [Category("信息")]
        public string Title
        {
            get { return "大智慧实时数据提供者"; }
        }
        [Category("信息")]
        public string URL
        {
            get { return "www.huaquant.com"; }
        }

        /*事件定义与激活方法*/
        public event EventHandler Connected;
        public event EventHandler StatusChanged;
        public event EventHandler Disconnected;
        public event ProviderErrorEventHandler Error;
        private void EmitConnectedEvent()
        {
            if (Connected != null)
            {
                Connected(this, EventArgs.Empty);
            }
        }
        private void EmitStatusChangedEvent()
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, EventArgs.Empty);
            }
        }
        private void EmitDisconnectedEvent()
        {
            if (Disconnected != null)
            {
                Disconnected(this, EventArgs.Empty);
            }
        }
        private void EmitError(int id, int code, string message)
        {
            if (Error != null)
                Error(new ProviderErrorEventArgs(new ProviderError(Clock.Now, this, id, code, message)));
        }
        #endregion
        #region IMarketProvider 
        private DZHQuoteReader quoteReader = null;
        private int refreshTime = 1000;
        private TimeSpan beginTime = new TimeSpan(0, 0, 0);
        private TimeSpan endTime = new TimeSpan(1, 0, 0, 0);
        private IBarFactory factory = null;
        private bool buildDailyBar = false;
        private bool onlyReadInMarket = false;
        public IBarFactory BarFactory
        {
            get
            {
                return this.factory;
            }
            set
            {
                if (this.factory != null)
                {
                    this.factory.NewBar -= new BarEventHandler(this.OnNewBar);
                    this.factory.NewBarOpen -= new BarEventHandler(this.OnNewBarOpen);
                    this.factory.NewBarSlice -= new BarSliceEventHandler(this.OnNewBarSlice);
                }
                this.factory = (IBarFactory)value;
                if (this.factory != null)
                {
                    this.factory.NewBar += new BarEventHandler(this.OnNewBar);
                    this.factory.NewBarOpen += new BarEventHandler(this.OnNewBarOpen);
                    this.factory.NewBarSlice += new BarSliceEventHandler(this.OnNewBarSlice);
                }
            }
        }
        [Category("市场数据提供者设置"), Description("读取数据的刷新时间，单位毫秒"), DefaultValue(1000)]
        public int RefreshTime
        {
            get { return this.refreshTime; }
            set { this.refreshTime = value; }
        }
        [Category("市场数据提供者设置"), Description("市场开市时间"), DefaultValue("09:30")]
        public string BeginTime
        {
            get { return beginTime.ToString(); }
            set { beginTime =TimeSpan.Parse(value); }
        }
        [Category("市场数据提供者设置"), Description("市场闭市时间"), DefaultValue("15:00")]
        public string EndTime
        {
            get { return endTime.ToString(); }
            set { endTime = TimeSpan.Parse(value); }
        }
        [Category("市场数据提供者设置"), Description("是否生成日线"), DefaultValue(false)]
        public bool BuildDailyBar
        {
            get { return buildDailyBar; }
            set { buildDailyBar = value; }
        }
        [Category("市场数据提供者设置"), Description("是否只在开市期间读数据"), DefaultValue(false)]
        public bool OnlyReadInMarket
        {
            get { return onlyReadInMarket; }
            set { onlyReadInMarket = value; }
        }
        public event MarketDataRequestRejectEventHandler MarketDataRequestReject;

        public event MarketDataSnapshotEventHandler MarketDataSnapshot;

        public event BarEventHandler NewBar;

        public event BarEventHandler NewBarOpen;

        public event BarSliceEventHandler NewBarSlice;

        public event CorporateActionEventHandler NewCorporateAction;

        public event FundamentalEventHandler NewFundamental;

        public event BarEventHandler NewMarketBar;

        public event MarketDataEventHandler NewMarketData;

        public event MarketDepthEventHandler NewMarketDepth;

        public event QuoteEventHandler NewQuote;

        public event TradeEventHandler NewTrade;

        private void OnNewBar(object sender, BarEventArgs args)
        {
            if (this.NewBar != null)
            {
                this.NewBar(this, new BarEventArgs(args.Bar, args.Instrument, this));
            }
        }
        private void OnNewBarOpen(object sender, BarEventArgs args)
        {
            if (this.NewBarOpen != null)
            {
                this.NewBarOpen(this, new BarEventArgs(args.Bar, args.Instrument, this));
            }
        }
        private void OnNewBarSlice(object sender, BarSliceEventArgs args)
        {
            if (this.NewBarSlice != null)
            {
                this.NewBarSlice(this, new BarSliceEventArgs(args.BarSize, this));
            }
        }
        private void EmitNewQuote(Quote quote, Instrument instrument)
        {
            if (this.NewQuote != null)
            {
                this.NewQuote(this, new QuoteEventArgs(quote, instrument, this));
            }
            if (this.factory != null)
            {
                this.factory.OnNewQuote(instrument, quote);
            }
        }
        private void EmitNewTrade(Trade trade, Instrument instrument)
        {          
            if (this.NewTrade != null)
            {
                this.NewTrade(this, new TradeEventArgs(trade, instrument, this));
            }
            
            if (this.factory != null)
            {
                this.factory.OnNewTrade(instrument, trade);
            }
        }
        private Timer tickTimer=null;
        private List<string> subscribedSymbols = new List<string>();
        private Dictionary<string, DZHQuote> lastQuotes = new Dictionary<string, DZHQuote>();
        private DateTime lastDateOfQuote = new DateTime(1970, 1, 1);
        private Dictionary<Instrument, Dictionary<DateTime, Daily>> dailyBars = new Dictionary<Instrument, Dictionary<DateTime, Daily>>();
        private void OnReminder(ReminderEventArgs args)
        {
            lock (this)
            {
                foreach(KeyValuePair<Instrument,Dictionary<DateTime,Daily>> kvp1 in dailyBars.ToList()){
                    Instrument instrument = kvp1.Key;
                    Dictionary<DateTime, Daily> dailyBox = kvp1.Value;
                    foreach (KeyValuePair<DateTime, Daily> kvp2 in dailyBox.ToList())
                    {
                        DateTime barDateTime=kvp2.Key;
                        Daily daily=kvp2.Value;
                        DZHQuote lastQuote = lastQuotes[instrument.Symbol];
                        if (lastQuote.Time.Date == barDateTime)
                        {
                            daily.High=lastQuote.High;
                            daily.Low=lastQuote.Low;
                            daily.Close=lastQuote.Price;
                            daily.Volume=(long)lastQuote.TotalVolume;                        
                            ((DZHBarFactory)this.factory).EmitNewBar(daily, instrument);//必须使用barfactory激活事件
                        }
                        dailyBox.Remove(barDateTime);
                    }
                    if (dailyBox.Count == 0) dailyBars.Remove(instrument);
                }
                ((DZHBarFactory)this.factory).EmitNewBarSlice(BarSize.Day);
            }
        }
        /*订阅数据*/
        private void SubscribeSymbol(string symbol)
        {
            if (!subscribedSymbols.Contains(symbol))
                subscribedSymbols.Add(symbol);
            if (tickTimer == null)
            {
                tickTimer = new Timer();
                tickTimer.Interval = refreshTime;
                tickTimer.Elapsed += new ElapsedEventHandler(tickTimer_Elapsed);
                tickTimer.Start();
            }
        }
        /*取消订阅数据*/
        private void UnsubscribeSymbol(string symbol)
        {
            if (subscribedSymbols.Contains(symbol))
                subscribedSymbols.Remove(symbol);
            if (subscribedSymbols.Count == 0 && tickTimer != null)
            {
                tickTimer.Elapsed -= tickTimer_Elapsed;
                tickTimer.Stop();
                tickTimer = null;
            }
        }
        /*定时器执行更新报价*/
        private void tickTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            /*如果只在开市时读数据，在非开市时间直接退出*/
            if (onlyReadInMarket && (DateTime.Now < DateTime.Today.Add(beginTime) || DateTime.Now > DateTime.Today.Add(endTime))) return;
            /*当运行到第二天时，清空报价缓存*/
            if (lastDateOfQuote < DateTime.Today)
            {
                lastDateOfQuote = DateTime.Today;
                lastQuotes.Clear();
            }

            if (quoteReader == null) quoteReader = new DZHQuoteReader(dzhDataPath);
            try
            {
                foreach (string symbol in subscribedSymbols.ToArray())
                {
                    if (!subscribedSymbols.Contains(symbol)) continue;
                    Instrument curInstrument = InstrumentManager.Instruments[symbol];
                    if (curInstrument == null)
                    {
                        this.EmitError(-1, -1, "Symbol " + symbol + " was not found in list of requested symbols.");
                    }
                    else
                    {
                        DZHSymbol dzhSymbol = new DZHSymbol(curInstrument.SecurityExchange, curInstrument.SecurityID);
                        DZHQuote newQuote = quoteReader.RequestQuote(dzhSymbol);
                        bool flag1 = false;
                        bool flag2 = false;
                        bool first = false;//是否是请求的第一笔数据
                        if (this.lastQuotes.ContainsKey(symbol))
                        {
                            DZHQuote oldQuote = this.lastQuotes[symbol];                      
                            /*这里注释掉和下面一样，大智慧发送来的数据存在同一时刻多笔交易的现象
                             * 所以不能用时间来排除
                             */
                            //if (newQuote.Time != oldQuote.Time)
                            //{
                                if ((newQuote.Bid1 != oldQuote.Bid1) || (newQuote.Bid1Vol != oldQuote.Bid1Vol) ||
                                    
                                    (newQuote.Ask1 != oldQuote.Ask1) || (newQuote.Ask1Vol != oldQuote.Ask1Vol))
                                {
                                    flag1 = true;
                                }
                                if ((newQuote.TotalVolume!= oldQuote.TotalVolume) || (newQuote.Price != oldQuote.Price))
                                {
                                    flag2 = true;
                                }
                           // }
                            this.lastQuotes[symbol] = newQuote;
                        }
                        else
                        {
                            first = true;
                            if ((newQuote.Ask1 > 0.0) || (newQuote.Bid1 > 0.0))
                            {
                                flag1 = true;
                            }
                            if ((newQuote.Price > 0.0) || (newQuote.Volume > 0.0))
                            {
                                flag2 = true;
                            }
                            this.lastQuotes.Add(symbol, newQuote);
                        }
                        /*这里注释掉是，不做时间检查，因为不使用市场数据提供者存储数据，市场数据提供者只用于实时交易
                         *而要获取过去的数据存储起来，使用历史数据提供者              
                         */
                        //if ((flag1) && ((curInstrument.GetQuoteArray(newQuote.Time, newQuote.Time)).Count == 0))
                        if (flag1)
                        {
                            this.EmitNewQuote(new Quote(newQuote.Time, newQuote.Bid1, (int)newQuote.Bid1Vol, newQuote.Ask1, (int)newQuote.Ask1Vol), curInstrument);
                        }
                        /*大智慧有可能在同一时刻有多笔交易，QD不允许这样，所以有可能丢失某些笔交易
                         * 如果不作同一时刻的检查，则有可能在数据中保存重复的交易
                         * 这里注释掉是，不做时间检查，因为不使用市场数据提供者存储数据，市场数据提供者只用于实时交易
                         *而要获取过去的数据存储起来，使用历史数据提供者
                         */
                        //if ((flag2) && ((curInstrument.GetTradeArray(newQuote.Time, newQuote.Time)).Count == 0))   
                        if (flag2)
                        {
                            this.EmitNewTrade(new Trade(newQuote.Time, newQuote.Price, (int)newQuote.Volume), curInstrument);
                            /*以下代码是为了产生日线*/
                            if (first&&buildDailyBar)
                            {
                                lock (this)
                                {
                                    DateTime barDateTime = newQuote.Time.Date;
                                    if (!dailyBars.ContainsKey(curInstrument)) dailyBars.Add(curInstrument, new Dictionary<DateTime, Daily>());
                                    Dictionary<DateTime, Daily> dailyBox = dailyBars[curInstrument];
                                    if (!dailyBox.ContainsKey(barDateTime))
                                    {
                                        Daily daily = new Daily(barDateTime, newQuote.Open, newQuote.High, newQuote.Low, newQuote.Price, (long)newQuote.TotalVolume);
                                        dailyBox.Add(barDateTime, daily);                                    
                                        ((DZHBarFactory)this.factory).EmitNewBarOpen(daily, curInstrument);
                                    }
                                    DateTime realyEndTime = Clock.Now.Add(this.endTime-newQuote.Time.TimeOfDay);
                                    Clock.AddReminder(new ReminderEventHandler(OnReminder),realyEndTime,null);
                                }
                               
                            } 
                        }
                    }
                }

            }
            catch (Exception exception)
            {
                this.EmitError(-1, -1, exception.ToString());
            }
        }
        /*发送市场数据请求*/
        public void SendMarketDataRequest(SmartQuant.FIX.FIXMarketDataRequest request)
        {
            lock (this)
            {
                if (!IsConnected)
                {
                    EmitError(-1, -1, "Not connected.");
                    return;
                }

                switch (request.SubscriptionRequestType)
                {
                    case DataManager.MARKET_DATA_SUBSCRIBE:
                        for (int i = 0; i < request.NoRelatedSym; i++)
                        {
                            FIXRelatedSymGroup group = request.GetRelatedSymGroup(i);
                            SubscribeSymbol(group.Symbol);
                        }
                        break;
                    case DataManager.MARKET_DATA_UNSUBSCRIBE:
                        for (int i = 0; i < request.NoRelatedSym; i++)
                        {
                            FIXRelatedSymGroup group = request.GetRelatedSymGroup(i);
                            UnsubscribeSymbol(group.Symbol);
                        }
                        break;
                    default:
                        throw new ArgumentException("Unknown subscription type: " + request.SubscriptionRequestType.ToString());
                }
            }
        }
        #endregion
    }
}

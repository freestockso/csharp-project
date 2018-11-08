using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SmartQuant;
using SmartQuant.Providers;
using SmartQuant.FIX;
using SmartQuant.Data;
using SmartQuant.Instruments;
namespace HuaQuant.Data.DZH
{
    public class DZHHistoricalProvider:IProvider,IHistoricalDataProvider,IDisposable
    {
        public DZHHistoricalProvider()
        {
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
        ~DZHHistoricalProvider()
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
                if (dayReader != null) dayReader.Dispose();
                if (min1Reader != null) min1Reader.Dispose();
                if (min5Reader != null) min5Reader.Dispose();
                if (tickReader != null) tickReader.Dispose();
                if (financeReader != null) financeReader.Dispose();
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
            get { return 252; }
        }
        [Category("信息")]
        public bool IsConnected
        {
            get { return this.isConnected; }
        }
        [Category("信息")]
        public string Name
        {
            get { return "DZHHistoricalProvider"; }
        }
        public void Shutdown()
        {
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
            get { return "大智慧历史数据提供者"; }
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
        private Dictionary<string, HistoricalDataRequest> historicalDataIds = new Dictionary<string, HistoricalDataRequest>();//存放历史数据请求
        private Dictionary<string, HistoricalDataRequest> historicalPendingCancels = new Dictionary<string, HistoricalDataRequest>();//存放等待取消的请求
        private DZHMin1BarReader min1Reader = null;
        private DZHMin5BarReader min5Reader = null;
        private DZHDayBarReader dayReader = null;
        private DZHTickReader tickReader = null;
        private DZHFinanceReader financeReader = null;
        private bool forwardAdjust = false;
        [Category("历史数据提供者设置"), Description("是否前复权"), DefaultValue(false)]
        public bool ForwardAdjust
        {
            get { return forwardAdjust; }
            set { forwardAdjust = value; }
        }
        public int[] BarSizes
        {
            get { return new int[] { 300, 60 }; }//5分线与1分线
        }

        public void CancelHistoricalDataRequest(string requestId)
        {
            HistoricalDataRequest request = null;
            if (this.historicalDataIds.TryGetValue(requestId, out request))
            {
                this.historicalDataIds.Remove(requestId);
                this.historicalPendingCancels.Add(requestId, request);
                this.EmitHistoricalDataRequestCancelled(request.RequestId, request.Instrument);
            }
        }

        public HistoricalDataRange DataRange
        {
            get { return HistoricalDataRange.DateTimeInterval; }
        }

        public HistoricalDataType DataType
        {
            get { return (HistoricalDataType.Bar | HistoricalDataType.Daily | HistoricalDataType.Trade); }
        }

        public event HistoricalDataEventHandler HistoricalDataRequestCancelled;

        public event HistoricalDataEventHandler HistoricalDataRequestCompleted;

        public event HistoricalDataErrorEventHandler HistoricalDataRequestError;

        private void EmitHistoricalDataRequestCancelled(string requestId, IFIXInstrument instrument)
        {
            if (this.HistoricalDataRequestCancelled != null)
            {
                this.HistoricalDataRequestCancelled(this, new HistoricalDataEventArgs(requestId, instrument, this, -1));
            }
        }

        private void EmitHistoricalDataRequestCompleted(string requestId, IFIXInstrument instrument)
        {
            if (this.HistoricalDataRequestCompleted != null)
            {
                this.HistoricalDataRequestCompleted(this, new HistoricalDataEventArgs(requestId, instrument, this, -1));
            }
        }

        private void EmitHistoricalDataRequestError(string requestId, IFIXInstrument instrument, string message)
        {
            if (this.HistoricalDataRequestError != null)
            {
                this.HistoricalDataRequestError(this, new HistoricalDataErrorEventArgs(requestId, instrument, this, -1, message));
            }
        }

        public int MaxConcurrentRequests
        {
            get { return 1; }
        }

        public event HistoricalBarEventHandler NewHistoricalBar;

        public event HistoricalMarketDepthEventHandler NewHistoricalMarketDepth;

        public event HistoricalQuoteEventHandler NewHistoricalQuote;

        public event HistoricalTradeEventHandler NewHistoricalTrade;

        private void EmitHistoricalBar(string requestId, IFIXInstrument instrument, Bar bar)
        {
            if (this.NewHistoricalBar != null)
            {
                this.NewHistoricalBar(this, new HistoricalBarEventArgs(bar, requestId, instrument, this, -1));
            }
        }

        private void EmitHistoricalTrade(string requestID, IFIXInstrument instrument, Trade trade)
        {
            if (this.NewHistoricalTrade != null)
            {
                this.NewHistoricalTrade(this, new HistoricalTradeEventArgs(trade, requestID, instrument, this, -1));
            }
        }

        public void SendHistoricalDataRequest(HistoricalDataRequest request)
        {
            if (!this.IsConnected)
            {
                this.EmitError(1, 1, "DZH historical data provider not connected.");
                return;
            }
            Instrument instrument = (Instrument)request.Instrument;
            this.historicalDataIds.Add(request.RequestId, request);
            List<ISeriesObject> list = this.GetHistoricalData(instrument, request.DataType, (int)request.BarSize, request.BeginDate, request.EndDate);
            bool flag = false;
            if ((list.Count < 1) || (flag = this.historicalPendingCancels.ContainsKey(request.RequestId)))
            {
                if (flag)
                {
                    this.historicalPendingCancels.Remove(request.RequestId);
                    this.EmitHistoricalDataRequestCancelled(request.RequestId, request.Instrument);
                }
                else
                {
                    this.historicalDataIds.Remove(request.RequestId);
                    this.EmitHistoricalDataRequestCompleted(request.RequestId, request.Instrument);
                }
            }
            else
            {
                if (request.DataType == HistoricalDataType.Trade)
                {
                    foreach (ISeriesObject obj2 in list)
                    {
                        this.EmitHistoricalTrade(request.RequestId, request.Instrument, obj2 as Trade);
                        if (this.historicalPendingCancels.ContainsKey(request.RequestId))
                        {
                            this.historicalPendingCancels.Remove(request.RequestId);
                            this.EmitHistoricalDataRequestCancelled(request.RequestId, request.Instrument);
                        }
                    }
                }
                else
                {
                    foreach (ISeriesObject obj3 in list)
                    {
                        this.EmitHistoricalBar(request.RequestId, request.Instrument, obj3 as Bar);
                        if (this.historicalPendingCancels.ContainsKey(request.RequestId))
                        {
                            this.historicalPendingCancels.Remove(request.RequestId);
                            this.EmitHistoricalDataRequestCancelled(request.RequestId, request.Instrument);
                        }
                    }
                }
                this.EmitHistoricalDataRequestCompleted(request.RequestId, request.Instrument);
            }
        }
        public List<ISeriesObject> GetHistoricalData(Instrument instrument, HistoricalDataType dataType, int barSize, DateTime startTime, DateTime endTime)
        {
            List<ISeriesObject> results = new List<ISeriesObject>();
            DZHSymbol symbol = new DZHSymbol(instrument.SecurityExchange, instrument.SecurityID);
            List<DZHBar> bars;
            List<DZHTick> ticks;

            switch (dataType)
            {
                case HistoricalDataType.Bar:
                    if (barSize == 300)
                    {
                        if (min5Reader == null) min5Reader = new DZHMin5BarReader(dzhDataPath);
                        bars = min5Reader.RequestBars(symbol, startTime, endTime);
                    }
                    else if (barSize == 60)
                    {
                        if (min1Reader == null) min1Reader = new DZHMin1BarReader(dzhDataPath);
                        bars = min1Reader.RequestBars(symbol, startTime, endTime);
                    }
                    else throw new ArgumentException("Unknowm bar size:" + barSize.ToString());
                    if ((bars != null) && (bars.Count > 0))
                    {
                        /*前复权调整*/
                        if (forwardAdjust)
                        {
                            if (financeReader == null) financeReader = new DZHFinanceReader(dzhDataPath);
                            List<DZHExDividend> exDivs = financeReader.RequestExDividends(symbol);
                            financeReader.ForwardAdjustedPrice(bars, exDivs);
                        }
                        foreach (DZHBar aBar in bars)
                        {
                            results.Add(new Bar(aBar.Time, aBar.Open, aBar.High, aBar.Low, aBar.Close, (long)aBar.Volume, barSize));
                        }
                    }
                    break;
                case HistoricalDataType.Daily:
                    if (dayReader == null) dayReader = new DZHDayBarReader(dzhDataPath);
                    bars = dayReader.RequestBars(symbol, startTime, endTime);
                    if ((bars != null) && (bars.Count > 0))
                    {
                        /*前复权调整*/
                        if (forwardAdjust)
                        {
                            if (financeReader == null) financeReader = new DZHFinanceReader(dzhDataPath);
                            List<DZHExDividend> exDivs = financeReader.RequestExDividends(symbol);
                            financeReader.ForwardAdjustedPrice(bars, exDivs);
                        }
                        foreach (DZHBar aBar in bars)
                        {
                            results.Add(new Daily(aBar.Time, aBar.Open, aBar.High, aBar.Low, aBar.Close, (long)aBar.Volume));
                        }
                    }
                    break;
                case HistoricalDataType.Trade:
                    if (tickReader == null) tickReader = new DZHTickReader(dzhDataPath);
                    ticks = tickReader.RequestTicks(symbol, startTime, endTime);
                    if ((ticks != null) && (ticks.Count > 0))
                    {
                        int prevVol = 0;
                        foreach (DZHTick aTick in ticks)
                        {
                            results.Add(new Trade(aTick.Time, aTick.Price, (int)aTick.Volume - prevVol));
                            prevVol = (int)aTick.Volume;
                        }
                    }
                    break;
                default:
                    throw new ArgumentException("Unknown data type: " + dataType.ToString());
            }
            return results;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartQuant;
using SmartQuant.FIX;
using SmartQuant.Providers;
using System.ComponentModel;

namespace HuaQuant.Data.DZH
{
    public class DZHInstrumentProvider:IProvider,IInstrumentProvider
    {
         public DZHInstrumentProvider()
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
            get { return 253; }
        }
        [Category("信息")]
        public bool IsConnected
        {
            get { return this.isConnected; }
        }
        [Category("信息")]
        public string Name
        {
            get { return "DZHInstrumentProvider"; }
        }
        public void Shutdown()
        {
            this.Disconnect();
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
            get { return "大智慧证券定义提供者"; }
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
        #region InstrumentProvider
        private string marketFilters = "";
        private string securityTypeFilters = "";
        [Category("证券定义提供者设置"), Description("市场筛选"), DefaultValue(@"SH,SZ")]
        public string MarketFilters
        {
            get { return marketFilters; }
            set { marketFilters = value; }
        }
        [Category("证券定义提供者设置"), Description("证券类型筛选，类型参见DZHSymbol中的定义"), DefaultValue(@"Index,Stock")]
        public string SecurityTypeFilters
        {
            get { return securityTypeFilters; }
            set { securityTypeFilters = value; }
        }
        public event SecurityDefinitionEventHandler SecurityDefinition;

        public void SendSecurityDefinitionRequest(FIXSecurityDefinitionRequest request)
        {
            lock (this)
            {
                if (!this.IsConnected)
                {
                    this.EmitError(1, 1, "Instrument provider  not connected.");
                    return;
                }
                string[] markets = marketFilters.Split(',');
                //是否包含交易市场
                if (request.ContainsField(0xcf) && markets.Contains(request.SecurityExchange))
                {
                    DZHSymbolReader reader = new DZHSymbolReader(dzhDataPath);
                    string[] securityTypes = securityTypeFilters.Split(',');
                    List<DZHSymbol> symbolList = reader.GetSymbols(request.SecurityExchange);
                    reader.Dispose();//显式释放资源
                    if (symbolList != null)
                    {
                        List<DZHSymbol> symbols = new List<DZHSymbol>();
                        foreach (DZHSymbol symbol in symbolList)
                        {
                            if (securityTypes.Contains(symbol.SecurityType.ToString()))
                            {
                                symbols.Add(symbol);
                            }
                        }

                        foreach (DZHSymbol symbol in symbols)
                        {
                            FIXSecurityDefinition definition = new FIXSecurityDefinition(request.SecurityReqID, request.SecurityReqID, 4);
                            definition.SecurityExchange = symbol.Market;
                            definition.SecurityID = symbol.Code;
                            definition.SecurityIDSource = "8";// 8=Exchange Symbol
                            definition.Symbol = symbol.ToString();
                            switch (symbol.SecurityType)
                            {
                                case DZHSymbol.SecurityTypes.Index:
                                    definition.SecurityType = "IDX";
                                    break;
                                case DZHSymbol.SecurityTypes.Stock:
                                    definition.SecurityType = "CS";
                                    break;
                                default:
                                    definition.SecurityType = symbol.SecurityType.ToString().Substring(0, 3);
                                    break;
                            }
                            definition.SecuritySubType = symbol.StockType.ToString();
                            definition.SecurityDesc = symbol.Name;
                            definition.TotNoRelatedSym = symbols.Count;
                            if (this.SecurityDefinition != null)
                            {
                                this.SecurityDefinition(this, new SecurityDefinitionEventArgs(definition));
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}

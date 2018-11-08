using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartQuant.Providers;
using SmartQuant.FIX;
using SmartQuant.Instruments;
using System.ComponentModel;
using System.Threading;

namespace HuaQuant.Data.GSFIX
{
    public class GSFIXProvider:IProvider,IExecutionProvider
    {
        private string fundID;
        private string password;
        private int encryptType;
        private FIXTrade trador;
        private BrokerAccount brokerAccount = null;
        private AutoResetEvent autoResetEvent;

        public GSFIXProvider()
        {
            try
            {
                this.trador = new FIXTrade();
                this.trador.OnConnection += new EventHandler(OnConnection);
                this.trador.OnDisconnection += new EventHandler(OnDisconnection);
                this.trador.ExecutionReport +=new ExecutionReportEventHandler(OnExecutionReport);
                this.trador.OrderCancelReject += new OrderCancelRejectEventHandler(OnOrderCancelReject);
                this.trador.FundStatusReport += new FundStatusReportEventHandler(OnFundStatusReport);
                this.trador.PositionStatusReport+=new PositionStatusReportEventHandler(OnPositionStatusReport);
                autoResetEvent = new AutoResetEvent(false);
            }
            catch (Exception ex)
            {
                EmitError(-1, -1, ex.Message);
            }
            ProviderManager.Add(this);
        }

        private void EmitError(int id, int code, string message)
        {
            if (Error != null)
                Error(new ProviderErrorEventArgs(new ProviderError(DateTime.Now, this, id, code, message)));
        }

        [Category("设置"), Description("资金账号")]
        public string FundID
        {
            get { return this.fundID; }
            set { this.fundID = value; }
        }
        [Category("设置"), Description("密码"),PasswordPropertyText(true)]
        public string Password
        {
            get { return this.password; }
            set { this.password = value; }
        }
        [Category("设置"), Description("加密方式")]
        public int EncryptType
        {
            get { return this.encryptType; }
            set { this.encryptType = value; }
        }


        #region IProvider 成员
        private bool isConnected = false;
        
        public void OnConnection(object sender, EventArgs e)
        {
            this.isConnected = true;
            Console.WriteLine("已登录.");
            if (Connected != null)
            {
                Connected(this, new EventArgs());
            }
        }
        public void OnDisconnection(object sender,EventArgs e)
        {
            this.isConnected = false;
            Console.WriteLine("已登出.");
            if (Disconnected != null)
                Disconnected(this, new EventArgs());
        }
       
        public void Connect(int timeout)
        {
            this.Connect();
            ProviderManager.WaitConnected(this, timeout);
        }

        public void Connect()
        {
            Console.WriteLine("正在登录中,请等待...");
            try
            {
                this.trador.Logon(this.fundID, this.password, this.encryptType);
            }
            catch (Exception ex)
            {
                EmitError(-1, -1, ex.Message);
            }
        }

        public event EventHandler Connected;

        public void Disconnect()
        {
            if (isConnected)
            {
                this.trador.Logout();
            }
        }

        public event EventHandler Disconnected;

        public event ProviderErrorEventHandler Error;

        public byte Id
        {
            get { return 223; }
        }

        public bool IsConnected
        {
            get { return this.isConnected; }
        }

        public string Name
        {
            get { return "GSExecutionProvider"; }
        }

        public void Shutdown()
        {
            this.Disconnect();
            this.trador.Close();
        }

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

        public event EventHandler StatusChanged;

        public string Title
        {
            get { return "国信证券交易执行提供者"; }
        }

        public string URL
        {
            get { return String.Empty; }
        }

        #endregion
        public event SmartQuant.FIX.ExecutionReportEventHandler ExecutionReport;
        public event SmartQuant.FIX.OrderCancelRejectEventHandler OrderCancelReject;

        public void OnExecutionReport(object sender, ExecutionReportEventArgs e)
        {
            if (ExecutionReport != null)
            {
                QuickFix42.ExecutionReport sourceReport=e.ExecutionReport;
                try
                {
                    SmartQuant.FIX.ExecutionReport report = new SmartQuant.FIX.ExecutionReport();
                    if (sourceReport.isSetField(41))//如果已有原始编号OrigClOrdID
                    {
                        report.OrigClOrdID = sourceReport.getOrigClOrdID().getValue();
                    }
                    else if (sourceReport.isSetField(11))//如果有标识委托所对应的ClOrdID
                    {
                        report.ClOrdID = sourceReport.getClOrdID().getValue();
                    }

                    report.ExecID = sourceReport.getExecID().getValue();
                    char execType = sourceReport.getExecType().getValue();
                    //Console.WriteLine(execType);
                    switch (execType)//订单执行结果
                    {
                        case '0':
                            report.ExecType = ExecType.New;
                            break;
                        case '1':
                            report.ExecType = ExecType.PartialFill;
                            break;
                        case '2':
                            report.ExecType = ExecType.Fill;
                            break;
                        case '4':
                            report.ExecType = ExecType.Cancelled;
                            break;
                        case '6':
                            report.ExecType = ExecType.PendingCancel;
                            break;
                        case '8':
                            report.ExecType = ExecType.Rejected;
                            break;
                        default:
                            report.ExecType = ExecType.Undefined;
                            break;
                    }
                    if ((execType == '1') || (execType == '2'))//执行结果是部分成交或是全部成交
                    {
                        report.LastPx = sourceReport.getLastPx().getValue();//本次成交均价
                        report.LastQty = sourceReport.getLastShares().getValue();//本次成交数量
                    }
                    report.OrderQty = sourceReport.getOrderQty().getValue();//订单委托量

                    char orderStatus = sourceReport.getOrdStatus().getValue();
                    if (orderStatus == '1' || orderStatus == '2')//订单状态是部分成交或是全部成交
                    {
                        report.AvgPx = sourceReport.getAvgPx().getValue();//平均成交价
                        report.CumQty = sourceReport.getCumQty().getValue();//累计成交量
                        report.LeavesQty = report.OrderQty - report.CumQty;//未成交量
                    }
                   
                    switch (orderStatus)
                    {
                        case '0':
                            report.OrdStatus = OrdStatus.New;
                            break;
                        case '1':
                            report.OrdStatus = OrdStatus.PartiallyFilled;
                            break;
                        case '2':
                            report.OrdStatus = OrdStatus.Filled;
                            break;
                        case '4':
                            report.OrdStatus = OrdStatus.Cancelled;
                            break;
                        case '6':
                            report.OrdStatus = OrdStatus.PendingCancel;
                            break;
                        case '8':
                            report.OrdStatus = OrdStatus.Rejected;
                            break;
                        default:
                            report.OrdStatus = OrdStatus.Undefined;
                            break;
                    }
                    if (sourceReport.isSetTransactTime())//传送时间
                    {
                        report.TransactTime = sourceReport.getTransactTime().getValue();
                    }
                    if (sourceReport.isSetText())
                    {
                        report.Text = sourceReport.getText().getValue();
                    }
                    ExecutionReport(this, new SmartQuant.FIX.ExecutionReportEventArgs(report));
                }
                catch (Exception ex)
                {
                    EmitError(-1, -1, ex.Message);      
                }
            }         
        }
        
        public void OnOrderCancelReject(object sender, OrderCancelRejectEventArgs e)
        {
            if (OrderCancelReject != null)
            {
                QuickFix42.OrderCancelReject sourceCancelReject = e.OrderCancelReject;
                try
                {
                    SmartQuant.FIX.OrderCancelReject cancelReject = new OrderCancelReject();
                    cancelReject.OrigClOrdID = sourceCancelReject.getOrigClOrdID().getValue();
                    cancelReject.ClOrdID = sourceCancelReject.getClOrdID().getValue();
                    cancelReject.OrderID = sourceCancelReject.getOrderID().getValue();
                    cancelReject.CxlRejReason = CxlRejReason.TooLateToCancel;//因国信没有详细撤单原因说明，暂且设置为这个。
                    cancelReject.CxlRejResponseTo = CxlRejResponseTo.CancelRequest;//因国信不支持订单取消替换，故只有此种错误类型。
                    cancelReject.Text = sourceCancelReject.getText().getValue();
                    OrderCancelReject(this, new SmartQuant.FIX.OrderCancelRejectEventArgs(cancelReject));
                }
                catch (Exception ex)
                {
                    EmitError(-1, -1, ex.Message);
                }
            }
        }

        public void OnFundStatusReport(object sender, FundStatusReportEventArgs e)
        {
            this.brokerAccount = new BrokerAccount("国信-" + this.fundID);
            this.brokerAccount.AddField("Balance", e.Balance.ToString());
            this.brokerAccount.AddField("AvailableBalance", e.AvailableBalance.ToString());
            this.brokerAccount.AddField("TotalAssets", e.TotalAssets.ToString());
            this.brokerAccount.AddField("CapitalAssets", e.CapitalAssets.ToString());
            this.brokerAccount.AddField("MarketValue", e.MarketValue.ToString());
            this.brokerAccount.AddField("BuyFreezed", e.BuyFreezed.ToString());
            this.brokerAccount.BuyingPower = e.AvailableBalance;
        }
        public void OnPositionStatusReport(object sender, PositionStatusReportEventArgs e)
        {
            if (this.brokerAccount != null)
            {
                BrokerPosition brokerPosition = new BrokerPosition();
                brokerPosition.AddCustomField("AccountID", e.AccountID);
                brokerPosition.SecurityExchange = e.SecurityExchange;
                brokerPosition.Symbol = e.SecurityExchange+e.SecurityID;
                brokerPosition.AddCustomField("Balance", e.Balance.ToString());
                brokerPosition.AddCustomField("AvailableBalance", e.AvailableBalance.ToString());
                brokerPosition.Currency = "CNY";
                brokerPosition.LongQty = e.Quantity;
                this.brokerAccount.AddPosition(brokerPosition);
                autoResetEvent.Set();
            }
        }
        public BrokerInfo GetBrokerInfo()
        {
            BrokerInfo brokerInfo = new BrokerInfo();           
            this.trador.QueryFund();
            this.trador.QueryHold();
            autoResetEvent.WaitOne(5000);
            if (this.brokerAccount != null)
            {
                brokerInfo.Accounts.Add(this.brokerAccount);
            }
            return brokerInfo;
        }



        public void SendNewOrderSingle(SmartQuant.FIX.NewOrderSingle order)
        {
            try
            {
                Instrument curInstrument = InstrumentManager.Instruments[order.Symbol];
                char sideID;
                switch (order.Side)
                {
                    case Side.Buy:
                        sideID = '1';
                        break;
                    case Side.Sell:
                        sideID = '2';
                        break;
                    //case Side.SellShort:
                    //    sideID = '3';
                    //    break;
                    default:
                        throw new Exception("不支持的买卖指令。");
                }
                char orderType;
                switch (order.OrdType)
                {
                    case OrdType.Market:
                        orderType = '1';
                        break;
                    case OrdType.Limit:
                        orderType = '2';
                        break;
                    default:
                        throw new Exception("不支持的订单类型");
                }
                this.trador.NewOrder(order.ClOrdID, order.HandlInst, sideID, orderType,
                    curInstrument.SecurityID, curInstrument.SecurityExchange, (float)order.Price, (long)order.OrderQty);
            }
            catch (Exception ex)
            {
                EmitError(-1, -1, ex.Message);
            }
        }

        public void SendOrderCancelReplaceRequest(SmartQuant.FIX.FIXOrderCancelReplaceRequest request)
        {
            throw new NotImplementedException();
        }

        public void SendOrderCancelRequest(SmartQuant.FIX.FIXOrderCancelRequest request)
        {
            try
            {
                Instrument curInstrument = InstrumentManager.Instruments[request.Symbol];
                this.trador.CancelOrder(request.OrigClOrdID, request.ClOrdID, request.Side, curInstrument.SecurityID);
            }
            catch (Exception ex)
            {
                EmitError(-1, -1, ex.Message);
            }
        }

        public void SendOrderStatusRequest(SmartQuant.FIX.FIXOrderStatusRequest request)
        {
            try
            {
                Instrument curInstrument = InstrumentManager.Instruments[request.Symbol];
                this.trador.QueueOrderStatus(request.ClOrdID, request.Side, curInstrument.SecurityID);
            }
            catch (Exception ex)
            {
                EmitError(-1, -1, ex.Message);
            }
        }
    }
}

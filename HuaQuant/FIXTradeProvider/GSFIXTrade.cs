using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickFix;
using System.Runtime.InteropServices;
using System.Collections;
using System.Threading;


namespace HuaQuant.Data.GSFIX
{
    public class FIXTrade : QuickFix.MessageCracker, QuickFix.Application
    {
        [DllImport("gsencrypt.dll")]
        static extern int gsEncrypt(int pi_iMode, string pszPasswordIn, string pszKey, StringBuilder pszPasswordOut, int pi_iSize);

        // FIX初始化变量
        private string settingFile = "initiator.cfg";
        private QuickFix.SessionID ssnid;
        private QuickFix.SocketInitiator socketInitiator;
        private QuickFix.FileStoreFactory messageStoreFactory;
        private QuickFix.SessionSettings settings;
        private QuickFix.FileLogFactory logFactory;
        private QuickFix42.MessageFactory messageFactory;

        // 账号密码
        private string fundID;
        private string passwrod;
        private int encryptType;

        // FIX状态信息
        public string message;          // FIX返回信息
        public bool isLoggedOn;     // FIX连接状态
        private int fClOrdID;       // 唯一序列号


        //public event EventHandler OnError;
        public event EventHandler OnConnection;
        public event EventHandler OnDisconnection;
        public event OrderCancelRejectEventHandler OrderCancelReject;
        public event ExecutionReportEventHandler ExecutionReport;
        public event FundStatusReportEventHandler FundStatusReport;
        public event PositionStatusReportEventHandler PositionStatusReport;

        //private Dictionary<string, OrderInfo> ordersAll = new Dictionary<string, OrderInfo>();

        public FIXTrade()
        {
            Init();
        }

        private void Init()
        {
            this.settings = new QuickFix.SessionSettings(this.settingFile);
            this.messageStoreFactory = new QuickFix.FileStoreFactory(this.settings);
            this.logFactory = new QuickFix.FileLogFactory(this.settings);
            this.messageFactory = new QuickFix42.MessageFactory();
            this.socketInitiator = new QuickFix.SocketInitiator(this, this.messageStoreFactory, this.settings, this.logFactory, this.messageFactory);
        }
        public void Close()
        {
            //this.socketInitiator.stop();
            this.ssnid = null;
            this.socketInitiator = null;
            this.messageStoreFactory = null;
            this.settings = null;
            this.logFactory = null;
            this.messageFactory = null;
        }

        public void Logon(string fundID, string passwrod, int encryptType)
        {
            this.fundID = fundID;
            this.passwrod = passwrod;
            this.encryptType = encryptType;

            this.message = "正在登录...";
            this.socketInitiator.start();
            Session session = Session.lookupSession(ssnid);
            if ((session != null) && !session.isLoggedOn())
            {
                session.logon();
                this.isLoggedOn = session.isLoggedOn();
            }
        }
        public void Logout()
        {
            QuickFix42.Message message = new QuickFix42.Message(new MsgType("5"));
            SendToServer(message);
            Session session = Session.lookupSession(ssnid);
            if ((session != null) && session.isLoggedOn())
            {
                session.logout();
            }            
        }

        public void NewOrder(string clrID, char handinst, char sideID, char orderType, string symbolID, string exchange, float? price, long Quantity)
        {
            ClOrdID clOrdID = new ClOrdID(clrID);
            HandlInst inst = new HandlInst('1');//似乎国信只支持直通私有
            Side side = new Side(sideID);
            OrdType ordType = new OrdType(orderType);
            Symbol symbol = new Symbol(symbolID);
            TransactTime time = new TransactTime();

            QuickFix42.NewOrderSingle message = new QuickFix42.NewOrderSingle(clOrdID, inst, symbol, side, time, ordType);
            message.setString(38, Quantity.ToString());
            if (ordType.getValue() == OrdType.LIMIT)
            {
                message.setString(44, price.Value.ToString());
            }
            message.setString(15, "CNY");
            if (exchange == "SH")
            {
                message.setString(207, "XSHG");
            }
            else if (exchange == "SZ")
            {
                message.setString(207, "XSHE");
            }
            SendToServer(message);
        }
        public void CancelOrder(string oriID,string clrID, char sideID,string symbolID)
        {
            OrigClOrdID origClOrdID = new OrigClOrdID(oriID);
            ClOrdID clOrdID = new ClOrdID(clrID);
            Side side = new Side(sideID);
            Symbol symbol = new Symbol(symbolID);
            TransactTime time = new TransactTime();
            QuickFix42.OrderCancelRequest message = new QuickFix42.OrderCancelRequest(origClOrdID, clOrdID, symbol, side, time);
            message.setInt(38, 0);//委托数量,这里不设置竟无法撤单。
            SendToServer(message);
        }
        public void QueueOrderStatus(string clrID, char sideID, string symbolID)
        {
            ClOrdID clOrdID = new ClOrdID(clrID);
            Side side = new Side(sideID);
            Symbol symbol = new Symbol(symbolID);
            QuickFix42.OrderStatusRequest message = new QuickFix42.OrderStatusRequest(clOrdID, symbol, side);
            SendToServer(message);
        }

        /// <summary>
        /// 查询资金
        /// </summary
        public void QueryFund()
        {
            QuickFix42.Message message = new QuickFix42.Message(new MsgType("UAN"));
            message.setString(710, getNextID());    // 请求ID
            message.setInt(724, 9);                 // 请求类别
            SendToServer(message);
        }

        /// <summary>
        /// 查询股份
        /// </summary>
        public void QueryHold()
        {          
            QuickFix42.Message message = new QuickFix42.Message(new MsgType("UAN"));
            message.setString(710, getNextID());    // 请求ID
            message.setInt(724, 0);                 // 请求类别
            SendToServer(message);
        }
        private string getNextID()
        {
            string str = DateTime.Now.ToString("yyMMddHHmmss-") + fClOrdID;
            Interlocked.Increment(ref fClOrdID);
            return str;
        }
        public void SendToServer(QuickFix42.Message message)//发送信息请求
        {
            if (ssnid != null && Session.lookupSession(ssnid).isLoggedOn())
                Session.sendToTarget(message, ssnid);
            else
                throw new Exception("missing ssnid");
        }
        #region FIX函数
        public override void onMessage(QuickFix42.ExecutionReport report, SessionID sessionID)
        {
            if (ExecutionReport != null)
            {
                ExecutionReport(sessionID, new ExecutionReportEventArgs(report));
            }
        } 
        public override void onMessage(QuickFix42.OrderCancelReject message, SessionID sessionID)
        {
            if (OrderCancelReject != null)
            {
                OrderCancelReject(sessionID, new OrderCancelRejectEventArgs(message));
            }
        }

        void Application.toAdmin(QuickFix.Message message, SessionID sessionID)
        {            
            if (message is QuickFix42.Logon)
            {
                StringBuilder encryptPwd = new StringBuilder(128);
                switch (this.encryptType)
                {
                    case 0:
                        encryptPwd.Insert(0, this.passwrod);
                        break;
                    case 2:
                    case 101:
                        gsEncrypt(this.encryptType, this.passwrod, "GSFIXGW", encryptPwd, 128);
                        break;
                }
                // 调用加密函数
                message.setString(96, "Z:" + this.fundID + ":" + encryptPwd.ToString() + ":");
                message.setInt(98, this.encryptType);
            }
            //Console.WriteLine("to admin:"+message.ToString()); 
        }
        void Application.toApp(QuickFix.Message message, SessionID sessionID) 
        {
            //Console.WriteLine("to app:"+message.ToString()); 
        }
        void Application.fromAdmin(QuickFix.Message message, SessionID sessionID) 
        {
           //Console.WriteLine("from admin:"+message.ToString());
        }
        void Application.fromApp(QuickFix.Message message, SessionID sessionID) 
        {
            //Console.WriteLine("from app:"+message.ToString());  
            // 查询股份和资产不是FIX4.2的标准功能，需要特殊处理
            if (message.getHeader().getString(35) == "UAP")
            {
                PosType posType = new PosType();
                LongQty longQty = new LongQty();
                ShortQty shortQty = new ShortQty();
                NoPositions noPositions = new NoPositions();
                Group group = new Group(noPositions.getField(), posType.getField());

                if ((message.getField(724) == "9") && (FundStatusReport != null))// 资产
                {
                    //string currency = message.getField(15);
                    double balance = 0.0;
                    double availableBalance = 0.0;
                    double totalAsserts = 0.0;
                    double capitalAsserts = 0.0;
                    double marketValue = 0.0;
                    double buyFreezed = 0.0;

                    for (uint i = 1; i <= int.Parse(message.getField(noPositions.getField())); i++)
                    {
                        message.getGroup(i, group);
                        switch (group.getField(posType.getField()))
                        {
                            case "FB":
                                balance = double.Parse(group.getField(longQty.getField()));
                                break;
                            case "FAV":
                                availableBalance = double.Parse(group.getField(longQty.getField()));
                                break;
                            case "MV":
                                totalAsserts = double.Parse(group.getField(longQty.getField()));
                                break;
                            case "F":
                                capitalAsserts = double.Parse(group.getField(longQty.getField()));
                                break;
                            case "SV":
                                marketValue = double.Parse(group.getField(longQty.getField()));
                                break;
                            case "FBF":
                                buyFreezed = double.Parse(group.getField(shortQty.getField()));
                                break;
                        }
                    }
                    FundStatusReport(this, new FundStatusReportEventArgs(balance, availableBalance, totalAsserts,
                        capitalAsserts, marketValue, buyFreezed));
                }
                else if ((message.getField(724) == "0") && (PositionStatusReport != null)) // 股份
                {
                    string accountID = message.getString(1); ;
                    string securityExchange = message.getString(207);
                    switch (securityExchange)
                    {
                        case "XSHG":
                            securityExchange="SH";
                            break;
                        case "XSHE":
                            securityExchange="SZ";
                            break;
                    }
                    
                    string securityID = message.getString(55);
                    double balance = 0.0;
                    double availableBalance = 0.0;
                    double quantity = 0.0;

                    for (uint i = 1; i <= int.Parse(message.getField(noPositions.getField())); i++)
                    {
                        message.getGroup(i, group);
                        switch (group.getField(posType.getField()))
                        {
                            case "SB": balance = double.Parse(group.getField(longQty.getField())); break;
                            case "SAV": availableBalance = double.Parse(group.getField(longQty.getField())); break;
                            case "SQ": quantity = double.Parse(group.getField(longQty.getField())); break;
                        }

                    }
                    PositionStatusReport(this, new PositionStatusReportEventArgs(accountID, securityExchange, securityID,
                        balance, availableBalance, quantity));
                }
            }
            else
            {
                base.crack(message, sessionID); // 调用默认处理方法  
            }    
            message.Dispose();            
        }

        void Application.onCreate(SessionID sessionID)
        {
            this.message = "正在创建连接...";
            this.ssnid = sessionID;
        }
        void Application.onLogon(SessionID sessionID)
        {
            this.isLoggedOn = Session.lookupSession(sessionID).isLoggedOn();
            if (this.isLoggedOn)
            {
                this.message = "登录成功！";
                if (OnConnection != null) OnConnection(this, null);
            }
        }
        void Application.onLogout(SessionID sessionID)
        {
            this.isLoggedOn = Session.lookupSession(sessionID).isLoggedOn();
            if (!this.isLoggedOn)
            {
                this.message = "登出成功！";
                if (OnDisconnection != null) OnDisconnection(this, null);
            }
        }
        #endregion
    }

    public class ExecutionReportEventArgs : EventArgs
    {
        // Fields
        private QuickFix42.ExecutionReport report;

        // Methods
        public ExecutionReportEventArgs(QuickFix42.ExecutionReport report)
        {
            this.report = report;
        }

        // Properties
        public QuickFix42.ExecutionReport ExecutionReport
        {
            get
            {
                return this.report;
            }
        }
    }
    public delegate void ExecutionReportEventHandler(object sender, ExecutionReportEventArgs args);
    public class OrderCancelRejectEventArgs : EventArgs
    {
        // Fields
        private QuickFix42.OrderCancelReject reject;

        // Methods
        public OrderCancelRejectEventArgs(QuickFix42.OrderCancelReject reject)
        {
            this.reject = reject;
        }

        // Properties
        public QuickFix42.OrderCancelReject OrderCancelReject
        {
            get
            {
                return this.reject;
            }
        }
    }
    public delegate void OrderCancelRejectEventHandler(object sender, OrderCancelRejectEventArgs args);
    public class FundStatusReportEventArgs : EventArgs
    {
        private double balance;
        private double availableBalance;
        private double totalAsserts;
        private double capitalAsserts;
        private double marketValue;
        private double buyFreezed;

        public double Balance
        {
            get { return balance; }
        }
        public double AvailableBalance
        {
            get { return availableBalance; }
        }
        public double TotalAssets
        {
            get { return totalAsserts; }
        }
        public double CapitalAssets
        {
            get { return capitalAsserts; }
        }
        public double MarketValue
        {
            get { return marketValue; }
        }
        public double BuyFreezed
        {
            get { return buyFreezed; }
        }
        public FundStatusReportEventArgs(double balance, double availableBalance,double totalAsserts,double capitalAsserts,double marketValue,double buyFreezed)
        {
            this.balance = balance;
            this.availableBalance = availableBalance;
            this.totalAsserts = totalAsserts;
            this.capitalAsserts = capitalAsserts;
            this.marketValue = marketValue;
            this.buyFreezed = buyFreezed;            
        }
    }
    public delegate void FundStatusReportEventHandler(object sender, FundStatusReportEventArgs args);
    public class PositionStatusReportEventArgs : EventArgs
    {
        string accountID;
        string securityExchange;
        string securityID;
        double balance;
        double availableBalance;
        double quantity;

        public string AccountID
        {
            get { return accountID; }
        }
        public string SecurityExchange
        {
            get { return securityExchange; }
        }
        public string SecurityID
        {
            get { return securityID; }
        }
        public double Balance
        {
            get { return balance; }
        }
        public double AvailableBalance
        {
            get { return availableBalance; }
        }
        public double Quantity
        {
            get { return quantity; }
        }
        public PositionStatusReportEventArgs(string accountID,string securityExchange,string securityID,
            double balance,double availableBalance,double quantity)
        {
            this.accountID = accountID;
            this.securityExchange = securityExchange;
            this.securityID = securityID;
            this.balance = balance;
            this.availableBalance = availableBalance;
            this.quantity = quantity;
        }
    }
    public delegate void PositionStatusReportEventHandler(object sender,PositionStatusReportEventArgs args);
}
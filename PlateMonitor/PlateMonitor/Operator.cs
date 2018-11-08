using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GMSDK;
using MongoDB.Bson;
using MongoDB.Driver;

namespace PlateMonitor
{
    class Operator
    {

        private MdApi _md = MdApi.Instance;

        public Operator()
        {
            string username = "";
            string password = "";
            if (Properties.Settings.Default.tbUserName != "") username = Properties.Settings.Default.tbUserName;
            if (Properties.Settings.Default.tbPassword != "") password = Properties.Settings.Default.tbPassword;

            int ret = this._md.Init(username, password, GMSDK.MDMode.MD_MODE_NULL, "", "", "");
            if (ret != 0)
            {
                string msg = this._md.StrError(ret);
                Console.WriteLine(msg);
                return;
            }
        }
        public Dictionary<int, Plate> PlateDict = new Dictionary<int, Plate>();//板块字典
        public Dictionary<string, Security> SecurityDict = new Dictionary<string, Security>();//证券字典
        public List<string> ActiveSymbols = new List<string>();//当天活动证券符号列表
        public int NumOfUpdateBatch = 100;
        public int NumOfPersent = 5;
        public int AllUpLimitCount = 0;
        public int AllNPercentCount = 0;
        public int AllUpCount = 0;
        public int AllSecurityCount = 0;

        public string EastMoneyPath = @"D:\eastmoney\swc8";
        //读取版块和证券信息
        public void ReadPlatesAndSecurities()
        {
            Dictionary<string, string> symbolNameDict = this.getSymbolNameDict();
            FileStream fs = null;
            try
            {
                if (Properties.Settings.Default.tbEasyMoneyPath != "") this.EastMoneyPath = Properties.Settings.Default.tbEasyMoneyPath;
                string filePath = this.EastMoneyPath + @"\bklist_new_xx.dat";
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(fs, Encoding.Default);
                string text;
                string[] markets = new string[2] { "SZSE", "SHSE" };
                while (!reader.EndOfStream)
                {
                    text = reader.ReadLine();
                    int pos = text.LastIndexOf(";");
                    if (pos < 0) continue;
                    string symbolListString = text.Substring(pos + 1);
                    if (symbolListString.Length <= 0) continue;
                    string plateInfoString = text.Substring(0, pos);
                    string[] plateInfoArray = plateInfoString.Split(new char[] { ';' });
                    Plate plate = new Plate();
                    plate.ID = int.Parse(plateInfoArray[0]);
                    plate.Type = int.Parse(plateInfoArray[1]);
                    plate.Name = plateInfoArray[5];
                    string[] symbolArray = symbolListString.Split(new char[] { ':' });
                    foreach (string symbol in symbolArray)
                    {
                        if (symbol.Length < 3) continue;
                        int marketNo = int.Parse(symbol.Substring(0, 1));
                        string newSymbol = markets[marketNo] + "." + symbol.Substring(2);
                        Security curSecurity;
                        if (!this.SecurityDict.TryGetValue(newSymbol, out curSecurity))
                        {
                            curSecurity = new Security();
                            curSecurity.Symbol = newSymbol;
                            symbolNameDict.TryGetValue(newSymbol, out curSecurity.Name);
                            this.SecurityDict.Add(newSymbol, curSecurity);
                        }
                        curSecurity.Plates.Add(plate);
                        plate.Securities.Add(curSecurity);
                    }
                    this.PlateDict.Add(plate.ID, plate);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }
        //获取当天活动证券
        public void GetActiveSymbols()
        {
            List<Instrument> gskInsts = this._md.GetInstruments("SHSE", 1, 1);
            gskInsts.AddRange(this._md.GetInstruments("SZSE", 1, 1));
            foreach (Instrument inst in gskInsts)
            {
                this.ActiveSymbols.Add(inst.symbol);
            }
        }
        private Dictionary<string, string> getSymbolNameDict()
        {
            Dictionary<string, string> symbolNameDict = new Dictionary<string, string>();
            List<Instrument> gskInsts = this._md.GetInstruments("SHSE", 1, 0);
            gskInsts.AddRange(this._md.GetInstruments("SZSE", 1, 0));
            foreach (Instrument inst in gskInsts)
            {
                symbolNameDict.Add(inst.symbol, inst.sec_name);
            }
            return symbolNameDict;
        }
        //更新版块统计
        public void UpdatePlateStatistic()
        {
            //获取当天活动证券的最新行情
            List<Tick> gskTicks = new List<Tick>();
            try
            {
                int i = 0, j = 0;
                string symbolList = "";
                int num = this.ActiveSymbols.Count;
                foreach (string symbol in this.ActiveSymbols)
                {
                    j++;
                    i++;
                    symbolList += symbol + ",";
                    if (i >= this.NumOfUpdateBatch || j >= num)
                    {
                        gskTicks.AddRange(this._md.GetLastTicks(symbolList));
                        i = 0;
                        symbolList = "";
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            //将所有板块的统计归零
            foreach (Plate curPlate in this.PlateDict.Values)
            {
                curPlate.UpLimitCount = 0;
                curPlate.NPercentCount = 0;
                curPlate.UpCount = 0;
            }
            this.AllNPercentCount = 0;
            this.AllSecurityCount = 0;
            this.AllUpCount = 0;
            this.AllUpLimitCount = 0;
            //利用最新行情更新统计
            foreach (Tick gskTick in gskTicks)
            {
                string symbol = gskTick.exchange + "." + gskTick.sec_id;
                Security curSecurity;
                if (this.SecurityDict.TryGetValue(symbol, out curSecurity))
                {
                    curSecurity.Price = gskTick.last_price;
                    curSecurity.IncPercent = (gskTick.last_price / gskTick.pre_close - 1) * 100;
                    bool flag1 = curSecurity.UpLimited = (gskTick.last_price == gskTick.upper_limit);//涨停
                    bool flag2 = (curSecurity.IncPercent >= this.NumOfPersent);//涨幅达到指定的百分比
                    bool flag3 = (curSecurity.IncPercent > 0);
                    //全部统计
                    if (flag1) this.AllUpLimitCount++;
                    if (flag2) this.AllNPercentCount++;
                    if (flag3) this.AllUpCount++;
                    this.AllSecurityCount++;
                    //各版块统计
                    if (flag1 || flag2 || flag3)
                    {
                        foreach (Plate curPlate in curSecurity.Plates)
                        {
                            if (flag1) curPlate.UpLimitCount++;
                            if (flag2) curPlate.NPercentCount++;
                            if (flag3) curPlate.UpCount++;
                        }
                    }
                }
            }
            foreach (Plate curPlate in this.PlateDict.Values)
            {
                curPlate.Weight = (float)(curPlate.UpLimitCount * 0.5 + curPlate.NPercentCount * 0.30 + curPlate.UpCount * 0.20) * 100 / curPlate.Securities.Count;
            }
            this.getIntersectionInHotPlates(17);
        }
        //保存版块统计
        public void SavePlateStatistic()
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            MongoServer server = client.GetServer();
            MongoDatabase database = server.GetDatabase("financeLast");
            MongoCollection<BsonDocument> collection1 = database.GetCollection<BsonDocument>("plateUpLimitStatis");

            DateTime curDate = DateTime.Today;
            string beginTime = curDate.Add(new TimeSpan(9, 25, 0)).ToString("yyyy-MM-dd HH:mm:ss");
            string endTime = curDate.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");

            DateTime startTimeUTC = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            foreach (Security curSecurity in this.SecurityDict.Values)
            {
                if (curSecurity.UpLimited)
                {
                    List<Tick> ticks = this._md.GetTicks(curSecurity.Symbol, beginTime, endTime);
                    Tick firstUpLimitTick = ticks.Find(t => t.last_price == t.upper_limit);
                    if (firstUpLimitTick != null)
                    {
                        curSecurity.FirstUpLimitTime = startTimeUTC.AddSeconds(firstUpLimitTick.utc_time);
                    }
                    int lastNotUpLimitIndex = ticks.FindLastIndex(t => t.last_price < t.upper_limit);
                    lastNotUpLimitIndex++;
                    if (lastNotUpLimitIndex >= 0 && lastNotUpLimitIndex < ticks.Count)
                    {
                        curSecurity.LastUpLimitTime = startTimeUTC.AddSeconds(ticks[lastNotUpLimitIndex].utc_time);
                    }
                }
            }
            BsonElement[] eleArray = new BsonElement[1];
            eleArray[0] = new BsonElement("date", curDate.ToString("yyyy-MM-dd"));
            QueryDocument query = new QueryDocument(eleArray);
            collection1.Remove(query);
            foreach (Plate plate in this.PlateDict.Values)
            {
                if (plate.UpLimitCount <= 0) continue;
                BsonElement[] eleArray1 = new BsonElement[9];
                eleArray1[0] = new BsonElement("date", curDate.ToString("yyyy-MM-dd"));
                eleArray1[1] = new BsonElement("id", plate.ID);
                eleArray1[2] = new BsonElement("name", plate.Name);
                eleArray1[3] = new BsonElement("type", plate.Type);
                eleArray1[4] = new BsonElement("uplimit_count", plate.UpLimitCount);
                eleArray1[5] = new BsonElement("npercent_count", plate.NPercentCount);
                eleArray1[6] = new BsonElement("up_count", plate.UpCount);
                eleArray1[7] = new BsonElement("security_count", plate.Securities.Count);

                BsonArray bsonArray = new BsonArray(plate.UpLimitCount);
                foreach (Security curSecurity in plate.Securities)
                {
                    if (!curSecurity.UpLimited) continue;
                    bsonArray.Add(new BsonDocument(new BsonElement[3] { new BsonElement("symbol",curSecurity.Symbol),
                            new BsonElement("firstUpLimitTime",curSecurity.FirstUpLimitTime.ToString("yyyy-MM-dd HH:mm:ss")),
                            new BsonElement("lastUpLimitTime",curSecurity.LastUpLimitTime.ToString("yyyy-MM-dd HH:mm:ss"))}));
                }
                eleArray1[8] = new BsonElement("securities", bsonArray);
                BsonDocument insert = new BsonDocument(eleArray1);
                collection1.Insert(insert);
            }
        }
        //
        public void DoCalculate()
        {
            foreach (string symbol in this.ActiveSymbols)
            {
                this.calculateMatch(this.SecurityDict[symbol]);
            }
        }
        private void calculateMatch(Security curSecurity)
        {
            //if (curSecurity.Symbol != "SZSE.002864") return;
            curSecurity.Matched = false;
            //daliyBars是反序放置的，最新的日线索引最小
            List<DailyBar> dailyBars = this._md.GetLastNDailyBars(curSecurity.Symbol, 20, DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"));
            if (dailyBars.Count <= 0) return;
            this.AdjustDailys(dailyBars);
            //判断昨日是否有长上影线
            DailyBar lastDaily = dailyBars.First();

            //Console.WriteLine(lastDaily.strtime);
            float up = lastDaily.close;
            if (lastDaily.open > up) up = lastDaily.open;
            float topShadow = lastDaily.high - up;
            float other = up - lastDaily.low;
            if (topShadow > other / 3) return;
            //判断昨日均价是否收阳
            double avgPrice = lastDaily.amount / lastDaily.volume;
            if (avgPrice < lastDaily.open) return;
            //判断最近M天上涨次数是否多于下跌次数
            int m = 10;
            if (dailyBars.Count < m) m = dailyBars.Count;
            int incCount = 0;
            for (int i = 0; i < m; i++)
            {
                if (dailyBars[i].close > dailyBars[i].pre_close) incCount++;
            }
            if (incCount <= m / 2) return;
            curSecurity.Matched = true;
        }
        private void AdjustDailys(List<DailyBar> gmDailys)
        {//进行向前复权
            int num = gmDailys.Count;
            if (num > 1)
            {
                DailyBar lastDaily = gmDailys[0];
                for (int i = 1; i < num; i++)
                {
                    DailyBar curDaily = gmDailys[i];
                    if (curDaily.adj_factor != lastDaily.adj_factor)
                    {
                        curDaily.close = curDaily.close * curDaily.adj_factor / lastDaily.adj_factor;
                        curDaily.high = curDaily.high * curDaily.adj_factor / lastDaily.adj_factor;
                        curDaily.low = curDaily.low * curDaily.adj_factor / lastDaily.adj_factor;
                        curDaily.open = curDaily.open * curDaily.adj_factor / lastDaily.adj_factor;
                        curDaily.pre_close = curDaily.pre_close * curDaily.adj_factor / lastDaily.adj_factor;
                        curDaily.volume = (long)(curDaily.volume * lastDaily.adj_factor / curDaily.adj_factor);
                        curDaily.adj_factor = lastDaily.adj_factor;
                    }
                    lastDaily = curDaily;
                }
            }
        }
        //private List<double> MoveAvg(List<double> dataSeries,int n)
        //{

        //}
        public List<Plate> HotPlates = new List<Plate>();
        public void getIntersectionInHotPlates(float k)
        {
            foreach(Security secu in this.SecurityDict.Values)
            {
                secu.HotPlateCount = 0;
            }
            List<Plate> areaPlates = new List<Plate>();
            List<Plate> industryPlates = new List<Plate>();
            List<Plate> conceptPlates = new List<Plate>();
            foreach (Plate curPlate in this.PlateDict.Values)
            {
                switch (curPlate.Type)
                {
                    case 1:
                        areaPlates.Add(curPlate);
                        break;
                    case 2:
                        industryPlates.Add(curPlate);
                        break;
                    case 3:
                        conceptPlates.Add(curPlate);
                        break;
                }
            }
            areaPlates.Sort((x, y) => { return y.Weight.CompareTo(x.Weight); });
            industryPlates.Sort((x, y) => { return y.Weight.CompareTo(x.Weight); });
            conceptPlates.Sort((x, y) => { return y.Weight.CompareTo(x.Weight); });
            this.HotPlates.Clear();
            this.HotPlates.AddRange(areaPlates.Where(o => { return o.Weight >= k && (o.UpLimitCount>0||o.NPercentCount>0); }).Take(2));
            this.HotPlates.AddRange(industryPlates.Where(o=> { return o.Weight >= k && (o.UpLimitCount > 0 || o.NPercentCount > 0); }).Take(3));
            this.HotPlates.AddRange(conceptPlates.Where(o => { return o.Weight >= k && (o.UpLimitCount > 0 || o.NPercentCount > 0); }).Take(3));
            List<Security> secus = new List<Security>();
            foreach (Plate curPlate in this.HotPlates)
            {
                secus.AddRange(curPlate.Securities);
            }
            secus = secus.Distinct().ToList();
            foreach (Security secu in secus)
            {
                secu.HotPlateCount = secu.Plates.Intersect(this.HotPlates).Count();
            }
        }
    }
}

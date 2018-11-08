using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ReadPlateStock
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Plate> plateList = new List<Plate>();
            string path = @"D:\eastmoney\swc8\bklist_new_xx.dat";
            FileStream fs = null;
            try
            {
                fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(fs,Encoding.Default);
                string text;
                while(!reader.EndOfStream)
                {
                    text = reader.ReadLine();
                    int pos = text.LastIndexOf(";");
                    if (pos < 0) continue;
                    string symbolListString = text.Substring(pos + 1);
                    if (symbolListString.Length <= 0) continue;
                    string plateInfoString = text.Substring(0, pos);
                    string[] plateInfoArray = plateInfoString.Split(new char[] { ';' });
                    Plate plate = new Plate();
                    plate.No = int.Parse(plateInfoArray[0]);
                    plate.Type=int.Parse(plateInfoArray[1]);
                    plate.Name = plateInfoArray[5];
                    string[] symbolArray = symbolListString.Split(new char[] { ':' });
                    foreach(string symbol in symbolArray)
                    {
                        if (symbol.Length < 3) continue;
                        int marketNo = int.Parse(symbol.Substring(0, 1));
                        string[] markets = new string[2] { "SZSE", "SHSE" };
                        string newsymbol = markets[marketNo] + "." + symbol.Substring(2);
                        plate.Symbols.Add(newsymbol);
                    }
                    plateList.Add(plate);
                }
                

            }catch(Exception ex)
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
            var client = new MongoClient("mongodb://localhost:27017");
            var server = client.GetServer();
            var database = server.GetDatabase("finance_new");
            DateTime curDate = DateTime.Now.Date;
            //curDate = new DateTime(2017, 1, 1);
            string monthString = curDate.ToString("yyyy-MM");
            var collection = database.GetCollection<BsonDocument>("PlateSecurities." + monthString);
            collection.RemoveAll();
            foreach (Plate plate in plateList)
            {
                foreach(string symbol in plate.Symbols)
                {
                    BsonDocument insertDocument = new BsonDocument();
                    insertDocument.Add(new BsonElement("plate", plate.Name));
                    insertDocument.Add(new BsonElement("type", plate.Type));
                    insertDocument.Add(new BsonElement("symbol", symbol));
                    collection.Insert(insertDocument);
                }
                
            }
            Console.WriteLine("写入完毕");
            Console.ReadLine();

        }

        public class Plate
        {
            public int No;
            public int Type;
            public string Name;
            public List<string> Symbols=new List<string>();
        }
    }
}

using System;
using System.Net;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace crypto_analyser.model {
    public class ApplicationContext : DbContext {
        public DbSet<Bitcoin> Bitcoins { get; set; } = null!;

        public ApplicationContext() {       // ensure the DB is brand new
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Bitcoins;Trusted_Connection=True;");
        }
    }

    public class ApiController {
        public static BitcoinResponse CreateRequest(string startDate, string endDate) {
            string url = $"https://api.coingecko.com/api/v3/coins/bitcoin/market_chart/range?vs_currency=eur&from={startDate}&to={endDate}";     // the url is created including user's input from the form

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse();

            string response;
            using (System.IO.StreamReader streamReader = new(httpResponse.GetResponseStream())) {      // get json response
                response = streamReader.ReadToEnd();
            }

            BitcoinResponse bitcoinResponse = JsonConvert.DeserializeObject<BitcoinResponse>(response);     // deserialise it
            return bitcoinResponse;
        }
    }

    public class DatabaseController {
        // select rows where the date is the minimum date per each day = closest to midnight
        private readonly static string sqlExpression = "SELECT * FROM Bitcoins WHERE date IN (SELECT MIN(date) FROM Bitcoins GROUP BY CAST(date AS DATE))";

        public static int CalculateLongestDownward(DateTimeOffset startDate, DateTimeOffset endDate) {
            ApplicationContext db = DatabaseController.InsertData(startDate.ToUnixTimeSeconds().ToString(), endDate.ToUnixTimeSeconds().ToString());
            List<Bitcoin> bitcoins = db.Bitcoins.FromSqlRaw(sqlExpression).ToList();

            int counter = 0;
            List<int> counters = new();

            for (int i = 0; i < bitcoins.Count - 1; i++) {          //  if price is higher than the price on the next day, counter++
                if (bitcoins[i].Price > bitcoins[i + 1].Price) { counter++; } else { counters.Add(counter); counter = 0; }        // else save counter and start from the beginning
            }

            counters.Add(counter);      // in case the price never rises
            return counters.Max();      // return the longest sequence
        }

        public static Bitcoin HighestVolume(DateTimeOffset startDate, DateTimeOffset endDate) {
            ApplicationContext db = DatabaseController.InsertData(startDate.ToUnixTimeSeconds().ToString(), endDate.ToUnixTimeSeconds().ToString());
            List<Bitcoin> bitcoins = db.Bitcoins.FromSqlRaw(sqlExpression).ToList();
            
            Bitcoin bitcoin = bitcoins.OrderByDescending(x => x.Total_volume).First();  // select the first record after ordering by descending total volume = select the record with the max total volume

            return bitcoin;
        }

        public static Bitcoin[] CalculateDayForTrade(DateTimeOffset startDate, DateTimeOffset endDate) {
            ApplicationContext db = DatabaseController.InsertData(startDate.ToUnixTimeSeconds().ToString(), endDate.ToUnixTimeSeconds().ToString());
            List<Bitcoin> bitcoins = db.Bitcoins.FromSqlRaw(sqlExpression).ToList();

            Bitcoin sell = bitcoins.OrderByDescending(x => x.Price).First();    // find the date with the highest price to sell
            Bitcoin buy = sell;

            foreach (Bitcoin bitcoin in bitcoins) {          // find the lowest price before that date to buy
                buy = bitcoin.Date < sell.Date && bitcoin.Price < sell.Price ? bitcoin : buy;
            }

            Bitcoin[] _bitcoins = { buy, sell };
            return _bitcoins;
        }

        public static ApplicationContext InsertData(string startDate, string endDate) {
            ApplicationContext db = new();                                      // create new application context
            BitcoinResponse data = ApiController.CreateRequest(startDate, endDate);        // get response data from the API            

            for (int i = 0; i < data.Prices.GetLength(0); i++) {            // get length of the first dimension to loop through
                Bitcoin bitcoin = new() {
                    Date = DateTimeOffset.FromUnixTimeMilliseconds(Int64.Parse(data.Prices[i, 0])).DateTime,        // date is always stored in the first column of the array
                    Price = decimal.Parse(data.Prices[i, 1].Replace('.', ',')),                     // other info is in the second column
                    Market_cap = decimal.Parse(data.Market_caps[i, 1].Replace('.', ',')),
                    Total_volume = decimal.Parse(data.Total_volumes[i, 1].Replace('.', ','))
                };

                db.Bitcoins.Add(bitcoin);       // add new row into the DB
                db.SaveChanges();
            }
            
            return db;     // return applicatoin context       
        }
    }
}

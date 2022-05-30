using System;
using System.Net;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Crypto_analyser.Model {
    public class ApplicationContext : DbContext {
        public DbSet<Bitcoin> Bitcoins { get; set; } = null!;

        public ApplicationContext() {       
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Bitcoins;Trusted_Connection=True;");
        }
    }

    public static class ApiController {
        public static BitcoinJsonResponse GetBitcoinsInRange(string startDate, string endDate) {
            string url = $"https://api.coingecko.com/api/v3/coins/bitcoin/market_chart/range?vs_currency=eur&from={startDate}&to={endDate}";     // the url is created including user's input from the form
            BitcoinJsonResponse bitcoinResponse = new();

            try {
                HttpWebRequest jsonRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse jsonResponse = (HttpWebResponse)jsonRequest.GetResponse();

                string bitcoins;
                using (System.IO.StreamReader JsonResponseReader = new(jsonResponse.GetResponseStream())) {
                    bitcoins = JsonResponseReader.ReadToEnd();
                }

                bitcoinResponse = JsonConvert.DeserializeObject<BitcoinJsonResponse>(bitcoins);
            } catch (Exception e) {
                Console.WriteLine("Error: {0}", e.Message);
            }
            
            return bitcoinResponse;
        }
    }

    public static class DatabaseController {
        private readonly static string sqlExpression = "SELECT * FROM Bitcoins WHERE datetime IN (SELECT MIN(datetime) FROM Bitcoins GROUP BY CAST(datetime AS DATE))";

        public static Bitcoin[] PrepareBitcoinPricesForVisualization(long startDate, long endDate) {
            ApplicationContext db = DatabaseController.PrepareBitcoinsDB(startDate.ToString(), endDate.ToString());
            return db.Bitcoins.FromSqlRaw(sqlExpression).ToArray();
        }

        public static Bitcoin[] CountDaysWithLongestDownwardTrend(long startDate, long endDate) {
            ApplicationContext db = DatabaseController.PrepareBitcoinsDB(startDate.ToString(), endDate.ToString());
            List<Bitcoin> bitcoins = db.Bitcoins.FromSqlRaw(sqlExpression).ToList();
            
            Bitcoin[] startAndEndBitcoins = { };
            int bitcoinsLength = bitcoins.Count;

            int[] sequences = CalculateLongestDownward(bitcoins.Select(x => x.Price).ToArray(), bitcoinsLength);
            int longestDownward = sequences.Max();

            if(!sequences.Any(o => o != sequences[0])) {
                return startAndEndBitcoins;
            }

            Bitcoin startBitcoin = bitcoins[Array.IndexOf(sequences, longestDownward) - longestDownward];
            Bitcoin endBitcoin = bitcoins[Array.IndexOf(sequences, longestDownward)];

            startAndEndBitcoins.Append(startBitcoin);
            startAndEndBitcoins.Append(endBitcoin);

            return startAndEndBitcoins;
        }

        private static int[] CalculateLongestDownward(decimal[] arr, int n) {
            int[] sequences = new int[n];

            for (int i = 0; i < n; i++) {
                sequences[i] = 1;
            }

            for (int i = 1; i < n; i++) {
                if (arr[i] < arr[i - 1]) {
                    sequences[i] = sequences[i - 1] + 1;
                }
            }

            return sequences;
        }

        public static Bitcoin[] FindDaysWithHighestAndLowestTradingVolume(long startDate, long endDate) {
            using ApplicationContext db = DatabaseController.PrepareBitcoinsDB(startDate.ToString(), endDate.ToString());
            List<Bitcoin> bitcoins = db.Bitcoins.FromSqlRaw(sqlExpression).ToList();

            Bitcoin bitcoinWithLowestVolume = bitcoins.OrderBy(x => x.Total_volume).First();
            Bitcoin bitcoinWithHighestVolume = bitcoins.OrderByDescending(x => x.Total_volume).First();

            Bitcoin[] bitcoinsReturn = { bitcoinWithLowestVolume, bitcoinWithHighestVolume };
            return bitcoinsReturn;
        }

        public static Bitcoin[] FindBestDaysToBuyAndSell(long startDate, long endDate) {
            using ApplicationContext db = DatabaseController.PrepareBitcoinsDB(startDate.ToString(), endDate.ToString());
            Bitcoin[] bitcoins = db.Bitcoins.FromSqlRaw(sqlExpression).ToArray();

            decimal maxPriceGap = 0;
            Bitcoin dayToBuy = new();
            Bitcoin dayToSell = new();

            for(int i = 0; i < bitcoins.Length - 2; i++) {
                for(int j = 0; j < bitcoins.Length - 1; j++) {
                    if(bitcoins[j].Price - bitcoins[i].Price > maxPriceGap && bitcoins[i].DateTime < bitcoins[j].DateTime) {
                        maxPriceGap = bitcoins[j].Price - bitcoins[i].Price;
                        dayToBuy = bitcoins[i];
                        dayToSell = bitcoins[j];
                    }
                }
            }

            Bitcoin[] daysToBuyAndToSell = { dayToBuy, dayToSell };
            return daysToBuyAndToSell;
        }

        public static ApplicationContext PrepareBitcoinsDB(string startDate, string endDate) {
            ApplicationContext db = new();
            BitcoinJsonResponse bitcoins = ApiController.GetBitcoinsInRange(startDate, endDate);
            
            for (int i = 0; i < bitcoins.Prices.GetLength(0); i++) {            // get length of the first dimension of an array to loop through
                Bitcoin bitcoin = new() {
                    DateTime = GetDateTimeFromBicoinsDB(bitcoins, i),
                    Price = GetPriceFromBitcoinsDB(bitcoins, i),
                    Market_cap = GetMarketCapFromBitcoinsDB(bitcoins, i),
                    Total_volume = GetTotalVolumeFromBitcoinsDB(bitcoins, i)
                };

                db.Bitcoins.Add(bitcoin);
                db.SaveChanges();
            }

            return db;    
        }

        private static DateTime GetDateTimeFromBicoinsDB(BitcoinJsonResponse bitcoins, int row) {
            return DateTimeOffset.FromUnixTimeMilliseconds(Int64.Parse(bitcoins.Prices[row, 0])).DateTime;
        }

        private static decimal GetPriceFromBitcoinsDB(BitcoinJsonResponse bitcoins, int row) {
            return decimal.Parse(bitcoins.Prices[row, 1].Replace('.', ','));
        }

        private static decimal GetMarketCapFromBitcoinsDB(BitcoinJsonResponse bitcoins, int row) {
            return decimal.Parse(bitcoins.Market_caps[row, 1].Replace('.', ','));
        }

        private static decimal GetTotalVolumeFromBitcoinsDB(BitcoinJsonResponse bitcoins, int row) {
            return decimal.Parse(bitcoins.Total_volumes[row, 1].Replace('.', ','));
        }
    }
}

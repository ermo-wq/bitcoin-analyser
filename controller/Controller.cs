using System;
using crypto_analyser.model;

namespace crypto_analyser.controller {
    class Controller {        
        public static string CalculateLongestDownward(DateTimeOffset startDate, DateTimeOffset endDate) {            
            string result = string.Format("The longest downward trend: {0} days.", DatabaseController.CalculateLongestDownward(startDate, endDate));
            return result;
        }

        public static string HighestVolume(DateTimeOffset startDate, DateTimeOffset endDate) {
            Bitcoin bitcoin = DatabaseController.HighestVolume(startDate, endDate);
            string result = string.Format("Date with the highest trading volume: {0} - {1}", bitcoin.Date.ToShortDateString(), Math.Round(bitcoin.Total_volume, 2));
            return result;
        }

        public static string CalculateDayForTrade(DateTimeOffset startDate, DateTimeOffset endDate) {
            Bitcoin[] bitcoins = DatabaseController.CalculateDayForTrade(startDate, endDate);            
            string result = bitcoins[1] == bitcoins[0] ? "Buying Bitcoin isn't recommended." :      // if the price only decreases = buy variable remained the same then show another result
                string.Format("The best day to buy Bitcoin: {0}. The best date to sell Bitcoin: {1}", bitcoins[0].Date.ToShortDateString(), bitcoins[1].Date.ToShortDateString());            
            return result;
        }
    }
}

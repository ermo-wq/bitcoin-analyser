using System;
using System.Linq;
using Crypto_analyser.Model;

namespace Crypto_analyser.Controller {
    public static class Controller {        
        public static int GetDaysWithLongestDownwardTrend(DateTimeOffset startDate, DateTimeOffset endDate) {
            return DatabaseController.CountDaysWithLongestDownwardTrend(startDate, endDate);
        }

        public static Bitcoin GetBitcoinWithHighestPrice(DateTimeOffset startDate, DateTimeOffset endDate) {
            Bitcoin bitcoin = new();
            bitcoin = startDate <= endDate ? DatabaseController.FindDayWithHighestPrice(startDate, endDate.AddDays(1)) : bitcoin;
            return bitcoin;
        }

        public static Bitcoin[] GetBitcoinWithHighestTradingVolume(DateTimeOffset startDate, DateTimeOffset endDate) {
            Bitcoin[] bitcoins = { };

            startDate = startDate.AddSeconds(startDate.Offset.TotalSeconds);        // convert local time to UTC
            endDate = endDate.AddSeconds(endDate.Offset.TotalSeconds);

            bitcoins = startDate <= endDate ? DatabaseController.FindDayWithHighestAndLowestTradingVolume(startDate.ToUnixTimeSeconds(), endDate.AddDays(1).ToUnixTimeSeconds()) : bitcoins;
            return bitcoins;
        }

        public static Bitcoin[] GetDaysToBuyAndSell(DateTimeOffset startDate, DateTimeOffset endDate) {           
            return DatabaseController.FindBestDaysToBuyAndSell(startDate, endDate);
        }
    }
}

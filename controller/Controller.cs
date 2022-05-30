using System;
using Crypto_analyser.Model;

namespace Crypto_analyser.Controller {
    public static class Controller {   
        public static Bitcoin[] GetBitcoinPrices(DateTimeOffset startDate, DateTimeOffset endDate) {
            Bitcoin[] bitcoins = DatabaseController.PrepareBitcoinPricesForVisualization(ConvertToUTC(startDate).ToUnixTimeSeconds(), ConvertToUTC(endDate).AddDays(1).ToUnixTimeSeconds());
            return bitcoins;
        }

        public static Bitcoin[] GetDaysWithLongestDownwardTrend(DateTimeOffset startDate, DateTimeOffset endDate) {
            Bitcoin[] bitcoins = DatabaseController.CountDaysWithLongestDownwardTrend(ConvertToUTC(startDate).ToUnixTimeSeconds(), ConvertToUTC(endDate).AddDays(1).ToUnixTimeSeconds());
            return bitcoins;
        }

        public static Bitcoin[] GetDaysToBuyAndSell(DateTimeOffset startDate, DateTimeOffset endDate) {
            Bitcoin[] bitcoins = DatabaseController.FindBestDaysToBuyAndSell(ConvertToUTC(startDate).ToUnixTimeSeconds(), ConvertToUTC(endDate).AddDays(1).ToUnixTimeSeconds());
            return bitcoins;
        }

        public static Bitcoin[] GetBitcoinsWithHighestAndLowestVolume(DateTimeOffset startDate, DateTimeOffset endDate) {
            Bitcoin[] bitcoins = DatabaseController.FindDaysWithHighestAndLowestTradingVolume(ConvertToUTC(startDate).ToUnixTimeSeconds(), ConvertToUTC(endDate).AddDays(1).ToUnixTimeSeconds());
            return bitcoins;
        }

        private static DateTimeOffset ConvertToUTC(DateTimeOffset date) {
            return date.AddSeconds(date.Offset.TotalSeconds);
        }

    }
}

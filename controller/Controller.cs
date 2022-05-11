using System;
using Crypto_analyser.Model;

namespace Crypto_analyser.Controller {
    public static class Controller {        
        public static int GetDaysWithLongestDownwardTrend(DateTimeOffset startDate, DateTimeOffset endDate) {                        
            return DatabaseController.CountDaysWithLongestDownwardTrend(startDate, endDate);
        }

        public static Bitcoin GetBitcoinWithHighestTradingVolume(DateTimeOffset startDate, DateTimeOffset endDate) {
            return DatabaseController.FindDayWithHighestTradingVolume(startDate, endDate);
        }

        public static Bitcoin[] GetDaysToBuyAndSell(DateTimeOffset startDate, DateTimeOffset endDate) {           
            return DatabaseController.FindBestDaysToBuyAndSell(startDate, endDate);
        }
    }
}

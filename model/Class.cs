using System;

namespace Crypto_analyser.Model {
    public class Bitcoin {              // DB model
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Price { get; set; }
        public decimal Market_cap { get; set; }
        public decimal Total_volume { get; set; }
    }

    public class BitcoinJsonResponse {          // JSON response model
        public string[,] Prices { get; set; }
        public string[,] Market_caps { get; set; }
        public string[,] Total_volumes { get; set; }
    }
}

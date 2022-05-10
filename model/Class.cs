using System;

namespace crypto_analyser.model {
    public class Bitcoin {              // DB model
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public decimal Market_cap { get; set; }
        public decimal Total_volume { get; set; }
    }

    public class BitcoinResponse {          // JSON response model
        public string[,] Prices { get; set; }
        public string[,] Market_caps { get; set; }
        public string[,] Total_volumes { get; set; }
    }
}

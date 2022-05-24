using System;
using System.Windows.Forms;
using Crypto_analyser.Model;

namespace Crypto_analyser {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            ConfigureDateTimePicker();
        }

        private void PrintAmountOfDaysLongestDownward(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            int amountOfDays = Controller.Controller.GetDaysWithLongestDownwardTrend(startDatePicker.Value, endDatePicker.Value);
            Cursor = Cursors.Default;

            resultLabel.Text = amountOfDays == 0 ? "Price didn't go any lower." : string.Format("The longest downward trend: {0} days.", amountOfDays);            
        }

        private void PrintDateWithHighestPriceAndPrice(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            Bitcoin bitcoin = Controller.Controller.GetBitcoinWithHighestPrice(startDatePicker.Value, endDatePicker.Value);
            Cursor = Cursors.Default;

            resultLabel.Text = bitcoin.Price == 0 ? "No corresponding bitcoin found." : string.Format("Date with the highest price: {0} - {1}", bitcoin.DateTime.ToShortDateString(), Math.Round(bitcoin.Price, 2));
        }

        private void PrintDateWithHighestVolumeAndVolume(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            Bitcoin[] bitcoins = Controller.Controller.GetBitcoinWithHighestTradingVolume(startDatePicker.Value, endDatePicker.Value);
            Cursor = Cursors.Default;

            Bitcoin bitcoinWithLowestVolume = bitcoins[0];
            Bitcoin bitcoinWithHighestVolume = bitcoins[1];

            resultLabel.Text = bitcoinWithHighestVolume.Total_volume == 0 ? "No corresponding bitcoin found." : 
                string.Format("Date with the lowest trading volume: {0} - {1}. Date with the highest trading volume: {2} - {3}.",
                bitcoinWithLowestVolume.DateTime.ToShortDateString(), Math.Round(bitcoinWithLowestVolume.Total_volume, 2),
                bitcoinWithHighestVolume.DateTime.ToShortDateString(), Math.Round(bitcoinWithHighestVolume.Total_volume, 2));
        }

        private void PrintBestDayToBuyAndSell(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            Bitcoin[] bitcoins = Controller.Controller.GetDaysToBuyAndSell(startDatePicker.Value, endDatePicker.Value);
            Cursor = Cursors.Default;
            
            resultLabel.Text = bitcoins[0] == bitcoins[1] ?
                string.Format("The best day to buy Bitcoin: {0}. Selling Bitcoin isn't recommended.", bitcoins[0].DateTime.ToShortDateString()) :
                string.Format("The best day to buy Bitcoin: {0}. The best date to sell Bitcoin: {1}", bitcoins[0].DateTime.ToShortDateString(), bitcoins[1].DateTime.ToShortDateString());
            
            resultLabel.Text = bitcoins[0].DateTime == DateTime.MinValue || bitcoins[1].DateTime == DateTime.MinValue ?
                "Buying Bitcoin isn't recommended. Selling isn't recommended either" : resultLabel.Text;
        }

        private void ConfigureDateTimePicker() {
            startDatePicker.MaxDate = DateTime.Today.AddDays(-1);
            endDatePicker.MaxDate = DateTime.Today;
        }

        private void ExitApplication(object sender, EventArgs e) {
            Application.Exit();
        }
    }
}

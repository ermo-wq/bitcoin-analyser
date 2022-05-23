using System;
using System.Windows.Forms;
using Crypto_analyser.Model;

namespace Crypto_analyser {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void PrintAmountOfDaysLongestDownward(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            int amountOfDays = Controller.Controller.GetDaysWithLongestDownwardTrend(startDatePicker.Value, endDatePicker.Value);
            Cursor = Cursors.Default;

            resultLabel.Text = amountOfDays == 0 ? "Price didn't go any lower." : string.Format("The longest downward trend: {0} days.", amountOfDays);            
        }

        private void PrintDateWithHighestVolumeAndVolume(object sender, EventArgs e) {            
            Cursor = Cursors.WaitCursor;
            Bitcoin bitcoin = Controller.Controller.GetBitcoinWithHighestTradingVolume(startDatePicker.Value, endDatePicker.Value);
            Cursor = Cursors.Default;

            resultLabel.Text = bitcoin.DateTime == DateTime.MinValue ? "No corresponding bitcoin found." : string.Format("Date with the highest trading volume: {0} - {1}", bitcoin.DateTime.ToShortDateString(), Math.Round(bitcoin.Total_volume, 2));
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

        private void ExitApplication(object sender, EventArgs e) {
            Application.Exit();
        }
    }
}

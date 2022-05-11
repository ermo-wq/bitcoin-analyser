using System;
using System.Windows.Forms;
using Crypto_analyser.Model;
using Crypto_analyser.Controller;

namespace Crypto_analyser {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void PrintAmountOfDaysLongestDownward(object sender, EventArgs e) {
            resultLabel.Text = string.Format("The longest downward trend: {0} days.", Controller.Controller.GetDaysWithLongestDownwardTrend(startDatePicker.Value, endDatePicker.Value));
        }

        private void PrintDateWithHighestVolumeAndVolume(object sender, EventArgs e) {
            Bitcoin bitcoin = Controller.Controller.GetBitcoinWithHighestTradingVolume(startDatePicker.Value, endDatePicker.Value);
            resultLabel.Text = string.Format("Date with the highest trading volume: {0} - {1}", bitcoin.DateTime.ToShortDateString(), Math.Round(bitcoin.Total_volume, 2));
        }

        private void PrintBestDayToBuyAndSell(object sender, EventArgs e) {
            Bitcoin[] bitcoins = Controller.Controller.GetDaysToBuyAndSell(startDatePicker.Value, endDatePicker.Value);            
            resultLabel.Text = bitcoins[0] == bitcoins[1] ?
                string.Format("The best day to buy Bitcoin: {0}. Selling Bitcoin isn't recommended.", bitcoins[0].DateTime.ToShortDateString()) :
                string.Format("The best day to buy Bitcoin: {0}. The best date to sell Bitcoin: {1}", bitcoins[0].DateTime.ToShortDateString(), bitcoins[1].DateTime.ToShortDateString());
        }

        private void ExitApplication(object sender, EventArgs e) {
            Application.Exit();
        }
    }
}

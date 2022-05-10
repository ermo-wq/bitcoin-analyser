using System;
using System.Windows.Forms;
using crypto_analyser.controller;

namespace crypto_analyser {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void LongestDownward(object sender, EventArgs e) {  // returns the number of days with the longest downward trend
            resultLabel.Text = Controller.CalculateLongestDownward(startDatePicker.Value, endDatePicker.Value);
        }

        private void HighestVolume(object sender, EventArgs e) {    // returns the date with the highest trading volume and the volume
            resultLabel.Text = Controller.HighestVolume(startDatePicker.Value, endDatePicker.Value);
        }

        private void BestDayForTrade(object sender, EventArgs e) {  // return a pair of days - to buy Bitcoin and to sell it
            resultLabel.Text = Controller.CalculateDayForTrade(startDatePicker.Value, endDatePicker.Value);
        }

        private void ExitApplication(object sender, EventArgs e) {  // exits the application
            Application.Exit();
        }
    }
}

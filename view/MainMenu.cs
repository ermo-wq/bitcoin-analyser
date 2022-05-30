using System;
using System.Linq;
using System.Windows.Forms;
using Crypto_analyser.Model;
using System.Windows.Forms.DataVisualization.Charting;

namespace Crypto_analyser {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            ConfigureDateTimePicker();
        }

        private void ConfigureDateTimePicker() {
            startDatePicker.MaxDate = endDatePicker.MaxDate = DateTime.Today;
        }

        private void PrintAmountOfDaysLongestDownward(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            Bitcoin[] longestDownward = Controller.Controller.GetDaysWithLongestDownwardTrend(startDatePicker.Value, endDatePicker.Value);
            Cursor = Cursors.Default;

            if(!longestDownward.Any(o => o != longestDownward[0])) {
                resultLabel.Text = "Price didn't go any lower.";
                return;
            }

            TimeSpan amountOfDays = longestDownward[1].DateTime - longestDownward[0].DateTime;

            resultLabel.Text = string.Format("The price decreased {0} days in a row from {1} to {2}.", 
                amountOfDays.Days - 1, longestDownward[0].DateTime.ToShortDateString(), longestDownward[1].DateTime.ToShortDateString());            
        }

        private void PrintDateWithHighestAndLowestVolume(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            Bitcoin[] bitcoins = Controller.Controller.GetBitcoinsWithHighestAndLowestVolume(startDatePicker.Value, endDatePicker.Value);
            Cursor = Cursors.Default;

            Bitcoin bitcoinWithLowestVolume = bitcoins[0];
            Bitcoin bitcoinWithHighestVolume = bitcoins[1];

            resultLabel.Text = string.Format("Lowest trading volume: {0} - {1}.\nHighest trading volume: {2} - {3}.",
                bitcoinWithLowestVolume.DateTime.ToShortDateString(), Math.Round(bitcoinWithLowestVolume.Total_volume, 2),
                bitcoinWithHighestVolume.DateTime.ToShortDateString(), Math.Round(bitcoinWithHighestVolume.Total_volume, 2));
        }

        private void PrintBestDayToBuyAndSell(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            Bitcoin[] bitcoins = Controller.Controller.GetDaysToBuyAndSell(startDatePicker.Value, endDatePicker.Value);
            Cursor = Cursors.Default;            

            resultLabel.Text = bitcoins[0].Price <= bitcoins[1].Price ? string.Format("One should neither buy nor sell.") :
                string.Format("The best day to buy Bitcoin: {0}. The best day to sell Bitcoin: {1}", bitcoins[0].DateTime.ToShortDateString(), bitcoins[1].DateTime.ToShortDateString());
        }

        private void VisualiseBitcoinPrices(object sender, EventArgs e) {
            Form dataForm = new();
            dataForm.Width = 800; 
            
            CreateBitcoinPriceChart(dataForm);
            dataForm.ShowDialog();
        }

        private void CreateBitcoinPriceChart(Form dataForm) {
            Chart bitcoinChart = new();
            Bitcoin[] bitcoins = GetBitcoinPrices();

            bitcoinChart.Parent = dataForm;
            bitcoinChart.Dock = DockStyle.Fill;

            bitcoinChart.ChartAreas.Add(new ChartArea("Bitcoin prices"));

            Series priceSeries = new("Prices");
            priceSeries.ChartType = SeriesChartType.Line;
            priceSeries.ChartArea = "Bitcoin prices";

            for (int i = 0; i < bitcoins.Length; i++) {
                priceSeries.Points.AddXY(bitcoins[i].DateTime.Date, bitcoins[i].Price);
            }

            bitcoinChart.Series.Add(priceSeries);
        }

        private Bitcoin[] GetBitcoinPrices() {
            Cursor = Cursors.WaitCursor;
            Bitcoin[] bitcoins = Controller.Controller.GetBitcoinPrices(startDatePicker.Value, endDatePicker.Value);
            Cursor = Cursors.Default;

            return bitcoins;
        }

        private void ExitApplication(object sender, EventArgs e) {
            Application.Exit();
        }
    }
}
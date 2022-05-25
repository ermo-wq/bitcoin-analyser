using System;
using System.Windows.Forms;
using Crypto_analyser.Model;
using System.Windows.Forms.DataVisualization.Charting;

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

        private void PrintDateWithHighestAndLowestVolume(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            Bitcoin[] bitcoins = Controller.Controller.GetBitcoinsWithHighestAndLowestVolume(startDatePicker.Value, endDatePicker.Value);
            Cursor = Cursors.Default;

            Bitcoin bitcoinWithLowestVolume = bitcoins[0];
            Bitcoin bitcoinWithHighestVolume = bitcoins[1];

            resultLabel.Text = string.Format("Date with the lowest trading volume: {0} - {1}.\nDate with the highest trading volume: {2} - {3}.",
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
            startDatePicker.MaxDate = endDatePicker.MaxDate = DateTime.Today;
        }

        private void VisualiseData(object sender, EventArgs e) {
            Form dataForm = new();
            Chart bitcoinChart = new();

            Cursor = Cursors.WaitCursor;
            Bitcoin[] bitcoins = Controller.Controller.GetBitcoinsForVisualization(startDatePicker.Value, endDatePicker.Value);
            Cursor = Cursors.Default;

            bitcoinChart.Parent = dataForm;
            bitcoinChart.Dock = DockStyle.Fill;

            bitcoinChart.ChartAreas.Add(new ChartArea("Bitcoin prices"));

            Series priceSeries = new("Prices");
            priceSeries.ChartType = SeriesChartType.Line;
            priceSeries.ChartArea = "Bitcoin prices";

            for(int i = 0; i < bitcoins.Length; i++) {
                priceSeries.Points.AddXY(bitcoins[i].DateTime.Date, bitcoins[i].Price);
            }

            bitcoinChart.Series.Add(priceSeries);    
            
            dataForm.AutoSize = true;
            dataForm.ShowDialog();
        }

        private void ExitApplication(object sender, EventArgs e) {
            Application.Exit();
        }
    }
}

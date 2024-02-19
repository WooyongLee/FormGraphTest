using System;
using System.Windows;
using System.Windows.Controls;

namespace FormGraph_DotNet6
{
    /// <summary>
    /// BarChartWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BarChartWindow : Window
    {
        public BarChartWindow()
        {
            InitializeComponent();

            this.SizeChanged += BarChartWindow_SizeChanged;
        }

        private void BarChartWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var chartWidth = this.ActualWidth * 2.0 / 3.0;
            var chartHeight = this.ActualHeight * 2.0 / 3.0;

            this.BarGraphChartControl.Width = chartWidth;
            this.BarGraphChartControl.Height = chartHeight;
        }

        private void BarDataAutoGenButton_Click(object sender, RoutedEventArgs e)
        {
            int barCount = int.Parse(BarCountTextBox.Text);

            // Set Bar Count
            this.BarGraphChartControl.BarData = new System.Collections.Generic.List<double>();
            this.BarGraphChartControl.NumOfColumn = barCount;
            for (int i = 0; i < barCount; i++)
            {
                this.BarGraphChartControl.BarData.Add(0);
            }
        }

        private void GenerateSampleButton_Click(object sender, RoutedEventArgs e)
        {
            var stepY = double.Parse(this.StepYTextBox.Text);
            this.BarGraphChartControl.MaxY = double.Parse(this.MaxYTextBox.Text);
            this.BarGraphChartControl.MinY = this.BarGraphChartControl.MaxY - this.BarGraphChartControl.NumOfRow * stepY;

            // Max ~ Min 사이의 Double 값
            int barCount = int.Parse(BarCountTextBox.Text);

            // Set Bar Count
            this.BarGraphChartControl.BarData = new System.Collections.Generic.List<double>();
            this.BarGraphChartControl.BarLegend = new System.Collections.Generic.List<string>();

            Random random = new Random();
            for (int i = 0; i < barCount; i++)
            {
                double randValue = random.NextDouble() * (this.BarGraphChartControl.MaxY - this.BarGraphChartControl.MinY) + this.BarGraphChartControl.MinY;

                this.BarGraphChartControl.BarData.Add(randValue);
            }

            for (int i = 0; i < barCount; i++)
            {
                this.BarGraphChartControl.BarLegend.Add((i + 1).ToString());
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton)
            {
                var rButton = (RadioButton)sender;
                if (rButton.Name.ToLower().Contains("black"))
                {
                    this.BarGraphChartControl.BackgroundTheme = GLGraphLib.ETheme.Black;
                }
                else if (rButton.Name.ToLower().Contains("white"))
                {
                    this.BarGraphChartControl.BackgroundTheme = GLGraphLib.ETheme.White;
                }
            }
        }
    }
}

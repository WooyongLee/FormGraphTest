using System.ComponentModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FormGraph_DotNet6
{
    /// <summary>
    /// ConstellationChartWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ConstellationChartWindow : Window
    {
        public ConstellationChartWindow()
        {
            InitializeComponent();

            this.SizeChanged += ConstellationChartWindow_SizeChanged;
        }

        private void ConstellationChartWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var chartWidth = this.ActualWidth * 2.0 / 3.0;
            var chartHeight = this.ActualHeight * 2.0 / 3.0;

            this.ConstellationChartControl.Width = chartWidth;
            this.ConstellationChartControl.Height = chartHeight;
        }

        private void GenerateSampleButton_Click(object sender, RoutedEventArgs e)
        {
            this.ConstellationChartControl.MaxX = double.Parse(this.MaxXTextBox.Text);
            this.ConstellationChartControl.MinX = -double.Parse(this.MaxXTextBox.Text);

            this.ConstellationChartControl.MaxY = double.Parse(this.MaxYTextBox.Text);
            this.ConstellationChartControl.MinY = -double.Parse(this.MaxYTextBox.Text);

            this.ConstellationChartControl.IsLoadSample = true;

            Random random = new Random();
            Random random2 = new Random();

            // 원점
            this.ConstellationChartControl.CH_X[0, 0] = 0.0;
            this.ConstellationChartControl.CH_Y[0, 0] = 0.0;

            // 각 x, y 축의 만나는 부분의 점
            int row = 8;

            // 4 Channel Test
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j <= row; j++)
                {
                    this.ConstellationChartControl.CH_X[i, j] = j * 0.5 - 2;
                    this.ConstellationChartControl.CH_Y[i, j] = 2 - i * 0.5;
                }
            }
        }

        private void LegendAutoGenButton_Click(object sender, RoutedEventArgs e)
        {
            int countOfLegend = int.Parse(LegendCountTextBox.Text);

            this.ConstellationChartControl.StrLegend = new System.Collections.Generic.List<string>();
            for (int i = 0; i < countOfLegend; i++)
            {
                this.ConstellationChartControl.StrLegend.Add("legend " + (i + 1).ToString());
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton)
            {
                var rButton = (RadioButton)sender;
                if (rButton.Name.ToLower().Contains("black"))
                {
                    this.ConstellationChartControl.BackgroundTheme = GLGraphLib.ETheme.Black;
                }
                else if (rButton.Name.ToLower().Contains("white"))
                {
                    this.ConstellationChartControl.BackgroundTheme = GLGraphLib.ETheme.White;
                }
            }
        }

        private void VisibleLegendCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (VisibleLegendCheckBox.IsChecked != null)
            {
                this.ConstellationChartControl.IsShowLegend = (bool)VisibleLegendCheckBox.IsChecked;
            }
        }
    }
}

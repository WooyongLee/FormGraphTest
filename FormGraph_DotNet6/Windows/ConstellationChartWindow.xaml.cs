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
            this.ConstellationChartControl.IsLoadSample = true;
        }

        private void LegendAutoGenButton_Click(object sender, RoutedEventArgs e)
        {
            // this.ConstellationChartControl.IsShowLegend = !this.ConstellationChartControl.IsShowLegend;
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

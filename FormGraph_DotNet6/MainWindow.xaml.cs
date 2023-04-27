using FormGraphLib_DotNet6;
using GLGraphLib_DotNet6;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace FormGraph_DotNet6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool bToggled = true;

        public MainWindow()
        {
            InitializeComponent();

            GraphComponent.MaxWidth = 800;
            GraphComponent.MaxHeight = 400;
        }

        // Lib Path :: C:\Solutions\Test\FormGraphTest\GLGraphLib_DotNet6\bin\Debug\net6.0-windows

        private void ChangeCHartButton_Click(object sender, RoutedEventArgs e)
        {
            if (bToggled)
            {
                ConstellationChartControl.Visibility = Visibility.Hidden;
                SpectrumChartControl.Visibility = Visibility.Visible;
            }

            else
            {
                ConstellationChartControl.Visibility = Visibility.Visible;
                SpectrumChartControl.Visibility = Visibility.Hidden;
            }
            bToggled = !bToggled;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //GraphChart.Refresh();
        }

        //private void ContextMenuButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        private void TraceButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                var traceNum = ExtractNumber(btn.Name);

                SpectrumChartControl.ShowHideTrace(traceNum - 1);
            }
        }

        private void MarkerButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                var markerNum = ExtractNumber(btn.Name);

                SpectrumChartControl.ShowHideMarker(markerNum - 1);
            }
        }

        private void FixedMarkerButton_Click(object sender, RoutedEventArgs e)
        {
            int fixedMarkerIndex = int.Parse(FixedMarkerTextBox.Text) - 1;

            SpectrumChartControl.SetMarkerFixed(fixedMarkerIndex);
        }

        private void DeltaMarkerButton_Click(object sender, RoutedEventArgs e)
        {
            int sourceIndex = int.Parse(DeltaMarkerSourceTextBox.Text) - 1;
            int targetIndex = int.Parse(DeltaMarkerTargetTextBox.Text) - 1;

            SpectrumChartControl.SetMarkerDelta(sourceIndex, targetIndex);
        }

        public static int ExtractNumber(string input)
        {
            foreach (char c in input)
            {
                if (Char.IsDigit(c))
                {
                    // ascii '0' = 48
                    return c - 48;
                }
            }
            return 0;
        }
    }
}

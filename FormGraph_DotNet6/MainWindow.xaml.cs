using FormGraphLib_DotNet6;
using GLGraphLib;
using System;
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

            MinXTextBox.Text = (SpectrumChartControl.CenterFrequency - SpectrumChartControl.Span / 2).ToString();
            MaxXTextBox.Text = (SpectrumChartControl.CenterFrequency + SpectrumChartControl.Span / 2).ToString();
            MinYTextBox.Text = (SpectrumChartControl.RefLevel - SpectrumChartControl.NumOfColumn * SpectrumChartControl.DivScale).ToString();
            MaxYTextBox.Text = (SpectrumChartControl.RefLevel).ToString();
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

        bool isTwoState = false;
        private void MakeSampleButton_Click(object sender, RoutedEventArgs e)
        {
            double[] data = new double[Trace.TotalDataLength];

            isTwoState = !isTwoState;

            int totalLength = Trace.TotalDataLength - 50;
            if (isTwoState)
            {
                for ( int i = 0; i < totalLength; i++)
                {
                    data[i] = -100 + 0.1 * i;
                }
            }
            else
            {
                for (int i = 0; i < totalLength; i++)
                {
                    data[i] = -0.1 * i;
                }
            }

            for ( int i = totalLength; i < Trace.TotalDataLength; i++)
            {
                if (i % 2 == 0) data[i] = 0 + 0.1 * i;
                else data[i] = -100 - 0.1 * i;
            }

            SpectrumChartControl.MakeTrace(data, 0);
        }

        private void SetMinXYButton_Click(object sender, RoutedEventArgs e)
        {
            SpectrumChartControl.IsSetMinMax = true;
            SpectrumChartControl.MinX = double.Parse(MinXTextBox.Text);
            SpectrumChartControl.MaxX = double.Parse(MaxXTextBox.Text);
            SpectrumChartControl.MinY = double.Parse(MinYTextBox.Text);
            SpectrumChartControl.MaxY = double.Parse(MaxYTextBox.Text);
        }

        private void SpectrumButton_Click(object sender, RoutedEventArgs e)
        {
            Button? btn = sender as Button;

            if (btn != null)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    string btnContent = btn.Content.ToString();

                    SpectrumChartControl.Visibility = Visibility.Hidden;
                    BarGraphChartControl.Visibility = Visibility.Hidden;
                    ConstellationChartControl.Visibility = Visibility.Hidden;

                    if (btnContent.Contains("Spectrum"))
                    {
                        SpectrumChartControl.Visibility = Visibility.Visible;
                    }

                    else if (btnContent.Contains("Constellation"))
                    {
                        ConstellationChartControl.Visibility = Visibility.Visible;
                    }

                    else if (btnContent.Contains("Bar"))
                    {
                        BarGraphChartControl.Visibility = Visibility.Visible;
                    }
                }));
            }
        }
    }
}

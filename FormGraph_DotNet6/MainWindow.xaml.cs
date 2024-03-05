using FormGraphLib_DotNet6;
using GLGraphLib;
using System;
using System.Collections.Generic;
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

            this.SizeChanged += MainWindow_SizeChanged;

            GraphComponent.MaxWidth = 800;
            GraphComponent.MaxHeight = 400;

            MinXTextBox.Text = (SpectrumChartControl.CenterFrequency - SpectrumChartControl.Span / 2).ToString();
            MaxXTextBox.Text = (SpectrumChartControl.CenterFrequency + SpectrumChartControl.Span / 2).ToString();
            MinYTextBox.Text = (SpectrumChartControl.RefLevel - SpectrumChartControl.NumOfColumn * SpectrumChartControl.DivScale).ToString();
            MaxYTextBox.Text = (SpectrumChartControl.RefLevel).ToString();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var chartWidth = this.ActualWidth * 4.0 / 5.0;
            var chartHeight = this.ActualHeight * 2.0 / 3.0;

            this.SpectrumChartControl.Width = chartWidth;
            this.SpectrumChartControl.Height = chartHeight;
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

                SpectrumChartControl.IsVisibleSpectrum[traceNum - 1] = !SpectrumChartControl.IsVisibleSpectrum[traceNum - 1];
            }
        }

        private void MarkerButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                var markerNum = ExtractNumber(btn.Name);

                SpectrumChartControl.ShowHideMarker(markerNum - 1);

                SpectrumChartControl.targetTraceIndex = 0;
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
            isTwoState = !isTwoState;

            if (SpectrumChartControl.ChartMode == ESpectrumChartMode.IQ)
            {
                // 항상 2개 Specturm Visible
                SpectrumChartControl.IsVisibleSpectrum[0] = true;
                SpectrumChartControl.IsVisibleSpectrum[1] = true;

                SpectrumChartControl.MinX = 0;
                SpectrumChartControl.MaxX = 1001;
                SpectrumChartControl.MinY = -33000;
                SpectrumChartControl.MaxY = +33000;

                int totalLength = SpectrumChartControl.TotalDataLength;
                int scale = 20000;
                var data = new List<double>();
                var data2 = new List<double>();
                if (isTwoState)
                {
                    for (int i = 0; i < totalLength; i++)
                    {
                        double angle = -90 + 1 * i; // -90에서 90까지의 각도 (90도 틀어진 값)
                        double radians = angle * Math.PI / 180; // 라디안 값으로 변환
                        data.Add(scale * Math.Cos(radians)); // 코사인 값 추가
                        data2.Add(scale * Math.Sin(radians)); // 사인 값 추가
                    }
                }
                else
                {
                    for (int i = 0; i < totalLength; i++)
                    {
                        double angle = -90 + 1 * i; // -90에서 90까지의 각도 (90도 틀어진 값)
                        double radians = angle * Math.PI / 180; // 라디안 값으로 변환
                        data.Add(scale * Math.Sin(radians)); // 사인 값 추가
                        data2.Add(scale * Math.Cos(radians)); // 코사인 값 추가
                    }
                }
                SpectrumChartControl.TraceData.SetData(0, data);
                SpectrumChartControl.TraceData.SetData(1, data2);
            }

            // Spectrum
            else
            {
                int totalLength = SpectrumChartControl.TotalDataLength - 50;
                var data = new List<double>();
                var data2 = new List<double>();
                if (isTwoState)
                {
                    for (int i = 0; i < totalLength; i++)
                    {
                        data.Add(-100 + 0.1 * i);
                        data2.Add(-0.1 * i);
                    }
                }
                else
                {
                    for (int i = 0; i < totalLength; i++)
                    {
                        data.Add(-0.1 * i);
                    }
                }

                for (int i = totalLength; i < SpectrumChartControl.TotalDataLength; i++)
                {
                    if (i % 2 == 0) data.Add(0 + 0.1 * i);
                    else data.Add(-100 - 0.1 * i);

                    data2.Add(-0.1 * i);
                }

                SpectrumChartControl.TraceData.SetData(0, data);
                SpectrumChartControl.TraceData.SetData(1, data2);
                SpectrumChartControl.IsVisibleSpectrum[0] = true;
            }
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
                    // ConstellationChartControl.Visibility = Visibility.Hidden;

                    if (btnContent.Contains("Spectrum"))
                    {
                        SpectrumChartControl.Visibility = Visibility.Visible;
                    }

                    else if (btnContent.Contains("Constellation"))
                    {
                        // ConstellationChartControl.Visibility = Visibility.Visible;
                        ConstellationChartWindow windows = new ConstellationChartWindow();
                        windows.Show();
                    }

                    else if (btnContent.Contains("Bar"))
                    {
                        // BarGraphChartControl.Visibility = Visibility.Visible;
                        BarChartWindow windows = new BarChartWindow();
                        windows.Show();
                    }
                }));
            }
        }

        bool isInit = false;
        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            if (!isInit)
            {
                isInit = true;
                return;
            }
            ComboBoxItem? item = sender as ComboBoxItem;
            if (item != null)
            {
                if (item.Content.ToString().ToLower().Contains("spectrum"))
                {
                    SpectrumChartControl.ChartMode = ESpectrumChartMode.DefaultSpecturm;
                }

                // I/Q View
                else
                {
                    SpectrumChartControl.ChartMode = ESpectrumChartMode.IQ;
                }
            }
        }

        private void SetFreqButton_Click(object sender, RoutedEventArgs e)
        {
            SpectrumChartControl.CenterFrequency = double.Parse(CenterFreqTextBox.Text); 
            SpectrumChartControl.Span = double.Parse(SpanTextBox.Text); 
        }
    }
}

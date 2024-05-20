using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;

namespace FormGraph_DotNet6
{
    /// <summary>
    /// SmithChartWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SmithChartWindow : Window
    {
        bool isSampleLoad = true;

        public SmithChartWindow()
        {
            InitializeComponent();

            this.SizeChanged += SmithChartWindow_SizeChanged;
        }

        private void SmithChartWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var chartWidth = this.ActualWidth * 2.0 / 3.0;
            var chartHeight = this.ActualHeight * 2.0 / 3.0;

            this.SmithChartControl.Width = chartWidth;
            this.SmithChartControl.Height = chartHeight;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton)
            {
                var rButton = (RadioButton)sender;
                if (rButton.Name.ToLower().Contains("black"))
                {
                    this.SmithChartControl.BackgroundTheme = GLGraphLib.ETheme.Black;
                }
                else if (rButton.Name.ToLower().Contains("white"))
                {
                    this.SmithChartControl.BackgroundTheme = GLGraphLib.ETheme.White;
                }
            }
        }

        private void ChartAxis_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            SmithChartControl.IsPolarAxis = radioButton.Content.ToString().ToUpper().Contains("POLAR");
        }

        private void GenerateSampleButton_Click(object sender, RoutedEventArgs e)
        {
            if (isSampleLoad)
            {
                // List to store the complex numbers for S11 and S21
                List<Complex> s11List = new List<Complex>();
                List<Complex> s21List = new List<Complex>();

                // load sample csv file
                using (var reader = new StreamReader("sample.csv"))
                {
                    // Read the header line
                    var header = reader.ReadLine();
                    if (header == null)
                    {
                        Console.WriteLine("Empty file.");
                        return;
                    }

                    // Read the data lines
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (line != null)
                        {
                            var values = line.Split(',');

                            // Parse the S11 Real and Imag parts
                            double s11Real = double.Parse(values[3], CultureInfo.InvariantCulture);
                            double s11Imag = double.Parse(values[4], CultureInfo.InvariantCulture);
                            Complex s11 = new Complex(s11Real, s11Imag);
                            s11List.Add(s11);

                            // Parse the S21 Real and Imag parts
                            double s21Real = double.Parse(values[5], CultureInfo.InvariantCulture);
                            double s21Imag = double.Parse(values[6], CultureInfo.InvariantCulture);
                            Complex s21 = new Complex(s21Real, s21Imag);
                            s21List.Add(s21);
                        }
                    }
                }

                SmithChartControl.Complexes.SetData(0, s11List);
                SmithChartControl.Complexes.SetData(1, s21List);

                LoadSampleButton.Content = " Clear Sample ";
            } // end if (isSampleLoad)

            else
            {
                SmithChartControl.Complexes.Clear();

                LoadSampleButton.Content = " Load Sample ";
            }

            isSampleLoad = !isSampleLoad;
        }
    }
}

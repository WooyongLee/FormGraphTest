using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FormGraph
{
    /// <summary>
    /// SimpleLineChart.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SimpleLineChart : UserControl
    {
        private bool isConsoleWrite = false;
        private double xmin = 0;
        private double xmax = 6.5;
        private double ymin = -1.1;
        private double ymax = 1.1;
        private Polyline pl;

        public SimpleLineChart()
        {
            InitializeComponent();
            // AddChart();

            Thread painThread = new Thread(new ThreadStart(() =>
            {
                int j = 0;
                while ( true )
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        chartCanvas.Children.Clear();
                        this.AddChart(j);
                    }));

                    Thread.Sleep(20);
                    if (++j > 70)
                    {
                        j = 0;
                    }
                } // end while ( true )
            }));

            // painThread.Start();
        }

        private void AddChart(int j)
        {
            // Draw sine curve:
            if (isConsoleWrite ) Console.WriteLine("Draw Sine Curve");
            pl = new Polyline();
            pl.Stroke = Brushes.Black;
            for (int i = 0;  i < 70; i++)
            {
                double x = i / 5.0;
                double y = Math.Sin(x - j/5.0);
                var pt = NormalizePoint(new Point(x, y));
                pl.Points.Add(pt);
                if (isConsoleWrite) Console.WriteLine(string.Format("x = {0:N3}, y = {1:N3}", pt.X, pt.Y));
            }
            chartCanvas.Children.Add(pl);
            // Draw cosine curve:
            if (isConsoleWrite) Console.WriteLine("Draw Cos Curve");
            pl = new Polyline();
            pl.Stroke = Brushes.Black;
            pl.StrokeDashArray = new DoubleCollection(new double[] { 4, 3 });
            for (int i = 0; i < 70; i++)
            {
                double x = i / 5.0;
                double y = Math.Cos(x - j / 5.0);
                var pt = NormalizePoint(new Point(x, y));
                pl.Points.Add(pt);
                if (isConsoleWrite) Console.WriteLine(string.Format("x = {0:N3}, y = {1:N3}", pt.X, pt.Y));
            }
            chartCanvas.Children.Add(pl);
        }

        private Point NormalizePoint(Point pt)
        {
            Point result = new Point();
            result.X = (pt.X - xmin) * chartCanvas.Width / (xmax - xmin);
            result.Y = chartCanvas.Height - (pt.Y - ymin) *  chartCanvas.Height / (ymax - ymin);
            return result;
        }
    }
}

using FormGraphLib_DotNet6;
using System.Windows;

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

        private void ContextMenuButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

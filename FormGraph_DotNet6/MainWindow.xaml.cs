using FormGraphLib_DotNet6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private void ChangeCHartButton_Click(object sender, RoutedEventArgs e)
        {
            if (bToggled)
            {
                FormGraphChart.Visibility = Visibility.Hidden;
                FormConstellationChart.Visibility = Visibility.Visible;
            }

            else
            {
                FormGraphChart.Visibility = Visibility.Visible;
                FormConstellationChart.Visibility = Visibility.Hidden;
            }
            bToggled = !bToggled;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GraphChart.Refresh();
        }

        private void ContextMenuButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

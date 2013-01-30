using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZicoresTradingPostNotifier.ViewModel;

namespace ZicoresTradingPostNotifier
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindowViewModel m = this.DataContext as MainWindowViewModel;
            m.RequestClose(e.Cancel);
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://notifier.zicore.de/");
            }
            catch (Exception)
            {

            }
        }

        private void Image_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start("http://www.gw2db.com/");
            }
            catch
            {

            }
        }
    }
}

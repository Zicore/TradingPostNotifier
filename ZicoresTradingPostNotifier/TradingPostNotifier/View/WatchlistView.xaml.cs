using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using Scraper.Notifier;
using ZicoresTradingPostNotifier.Helper;
using ZicoresTradingPostNotifier.ViewModel;

namespace ZicoresTradingPostNotifier.View
{
    /// <summary>
    /// Interaktionslogik für NotificationView.xaml
    /// </summary>
    public partial class WatchlistView : UserControl
    {
        ColumnHelper c = new ColumnHelper();
        public WatchlistView()
        {
            InitializeComponent();
            c.Handle(list, "Watchlist");
        }
    }
}

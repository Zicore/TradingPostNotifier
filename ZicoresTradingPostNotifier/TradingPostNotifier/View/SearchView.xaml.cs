using System;
using System.Collections.Generic;
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
using ZicoresTradingPostNotifier.Helper;
using ZicoresTradingPostNotifier.ViewModel;

namespace ZicoresTradingPostNotifier.View
{
    /// <summary>
    /// Interaktionslogik für SearchView.xaml
    /// </summary>
    public partial class SearchView : UserControl
    {
        public SearchView()
        {
            InitializeComponent();
            ColumnHelper c = new ColumnHelper();
            c.Handle(list, "Search");
        }

        private void SortClick(object sender, RoutedEventArgs e)
        {
            //ListView listView = sender as ListView;
            //GridViewColumnHeader header = e.OriginalSource as GridViewColumnHeader;


            ////SearchViewModel vm = DataContext as SearchViewModel;
            ////if (vm != null)
            ////{
            ////    vm.SortClickRedirect(header);
            ////}
        }
    }
}

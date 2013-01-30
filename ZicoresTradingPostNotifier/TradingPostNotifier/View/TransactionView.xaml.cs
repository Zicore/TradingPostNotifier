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

namespace ZicoresTradingPostNotifier.View
{
    /// <summary>
    /// Interaktionslogik für TransactionView.xaml
    /// </summary>
    public partial class TransactionView : UserControl
    {
        public TransactionView()
        {
            InitializeComponent();
            ColumnHelper c = new ColumnHelper();
            c.Handle(list, "TransactionView");
        }
    }
}

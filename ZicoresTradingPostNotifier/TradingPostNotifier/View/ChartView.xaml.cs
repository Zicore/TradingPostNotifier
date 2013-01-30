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
using ZicoresTradingPostNotifier.ViewModel;
using Microsoft.Research.DynamicDataDisplay; // Core functionality
using Microsoft.Research.DynamicDataDisplay.DataSources; // EnumerableDataSource
using Microsoft.Research.DynamicDataDisplay.PointMarkers; // CirclePointMarker

namespace ZicoresTradingPostNotifier.View
{
    /// <summary>
    /// Interaktionslogik für ChartView.xaml
    /// </summary>
    public partial class ChartView : UserControl
    {
        public ChartView()
        {
            InitializeComponent();
            Loaded += ChartView_Loaded;
        }

        void ChartView_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ChartViewModel;
            if (vm != null)
            {
                vm.ChartPlotter = plotter;
                vm.HorizontalAxis = dateAxis;
                vm.Begin();
            }
        }

        //private void Test()
        //{
        //    List<Double> yValues = new List<Double>() { 0, 3, 5, 6, 1, 5, 6 };
        //    List<DateTime> xValues = new List<DateTime>() {
        //            DateTime.Now,
        //            DateTime.Now + new TimeSpan(1, 0, 0, 0),
        //            DateTime.Now + new TimeSpan(2, 0, 0, 0),
        //            DateTime.Now + new TimeSpan(3, 0, 0, 0),
        //            DateTime.Now + new TimeSpan(4, 0, 0, 0),
        //            DateTime.Now + new TimeSpan(5, 0, 0, 0),
        //            DateTime.Now + new TimeSpan(6, 0, 0, 0)
        //        };

        //    EnumerableDataSource<double> yval = new EnumerableDataSource<double>(yValues);
        //    EnumerableDataSource<DateTime> xval = new EnumerableDataSource<DateTime>(xValues);

        //    xval.SetXMapping(x => dateAxis.ConvertToDouble(x));
        //    yval.SetYMapping(y => y);

        //    CompositeDataSource ds = new CompositeDataSource(xval, yval);

        //    plotter.AddLineGraph(ds);
        //    plotter.Viewport.FitToView();

        //    xValues.Add(DateTime.Now + new TimeSpan(10, 0, 0, 0));
        //    yValues.Add(6);

        //    xValues.Add(DateTime.Now + new TimeSpan(11, 0, 0, 0));
        //    yValues.Add(19);

        //    xValues.Add(DateTime.Now + new TimeSpan(12, 0, 0, 0));
        //    yValues.Add(24);
        //}
    }
}

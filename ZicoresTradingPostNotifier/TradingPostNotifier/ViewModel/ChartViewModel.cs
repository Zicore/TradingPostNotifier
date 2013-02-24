using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media; // CirclePointMarker
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.DataSources; // EnumerableDataSource
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using NotifierCore.DataProvider;
using NotifierCore.Notifier;

namespace ZicoresTradingPostNotifier.ViewModel
{
    public class ChartViewModel
    {
        public ChartViewModel(MainWindowViewModel mainViewModel)
        {
            this.MainViewModel = mainViewModel;
        }

        ChartPlotter _chartPlotter;
        public ChartPlotter ChartPlotter
        {
            get { return _chartPlotter; }
            set { _chartPlotter = value; }
        }

        MainWindowViewModel _mainViewModel;
        public MainWindowViewModel MainViewModel
        {
            get { return _mainViewModel; }
            set { _mainViewModel = value; }
        }

        HorizontalDateTimeAxis _horizontalAxis;
        public HorizontalDateTimeAxis HorizontalAxis
        {
            get { return _horizontalAxis; }
            set { _horizontalAxis = value; }
        }

        public void Begin()
        {
            try
            {
                for (int i = 0; i < MainViewModel.HotItemController.Queue.Count; i++)
                {
                    var item = MainViewModel.HotItemController.Queue[i];
                    if (!chartItems.ContainsKey(item.DataId))
                    {
                        if (item.IsSelected) // Chart selection
                        {
                            ObservableDataSource<ItemProxy> items = new ObservableDataSource<ItemProxy>();
                            items.SuspendUpdate();
                            Register(item, items);
                            items.ResumeUpdate();
                        }
                    }
                }
            }
            catch
            {

            }
        }

        Dictionary<int, ObservableDataSource<ItemProxy>> chartItems = new Dictionary<int, ObservableDataSource<ItemProxy>>();

        public void Register(HotItem item, ObservableDataSource<ItemProxy> items)
        {
            items.SetYMapping(y => y.SellPrice);
            items.SetXMapping(x => HorizontalAxis.ConvertToDouble(x.DateTime));

            ChartPlotter.AddLineGraph(items, 2.0, item.Name + " S");

            chartItems.Add(item.DataId, items);
            item.PropertyChanged += item_PropertyChanged;
        }

        void item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            HotItem item = (HotItem)sender;
            if (item != null)
            {
                if (chartItems.ContainsKey(item.DataId))
                {
                    var items = chartItems[item.DataId];
                    switch (e.PropertyName)
                    {
                        case "SellPrice":
                            var proxy = item.CreateProxy();
                            items.AppendAsync(MainWindowViewModel.Dispatcher, proxy);
                            //items.SetYMapping(y => y.SellPrice);
                            //items.SetXMapping(x => HorizontalAxis.ConvertToDouble(x.DateTime));
                            break;
                        case "BuyPrice":

                            break;
                        default:
                            break;
                    }
                }
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

    public class ChartDataHelper
    {
        IEnumerable<DateTime> _dateTimes;
        public IEnumerable<DateTime> DateTimes
        {
            get { return _dateTimes; }
            set { _dateTimes = value; }
        }

        IList _values;
        public IList Values
        {
            get { return _values; }
            set { _values = value; }
        }
    }
}

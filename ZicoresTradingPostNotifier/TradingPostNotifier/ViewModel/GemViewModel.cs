using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NotifierCore.Notifier;

namespace ZicoresTradingPostNotifier.ViewModel
{
    public class GemViewModel
    {
        MainWindowViewModel mainViewModel;
        public MainWindowViewModel MainViewModel
        {
            get { return mainViewModel; }
            set { mainViewModel = value; }
        }

        public GemViewModel(MainWindowViewModel mainViewModel)
        {
            this.MainViewModel = mainViewModel;
        }

        public GemManager GemManager
        {
            get
            {
                return MainViewModel.HotItemController.Gem;
            }
        }
    }
}

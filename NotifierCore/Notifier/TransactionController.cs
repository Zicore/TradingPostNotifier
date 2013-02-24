using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibraryBase.Wpf.ViewModel;

namespace NotifierCore.Notifier
{
    public class TransactionController : BindableBase
    {
        HotItemController _hotItemController;
        public HotItemController HotItemController
        {
            get { return _hotItemController; }
            set { _hotItemController = value; }
        }

        Thread tMainWorkerTransactions;
        private bool _isRunning = true;
        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                _isRunning = value;
                if (!IsRunning)
                {
                    if (tMainWorkerTransactions != null)
                    {
                        tMainWorkerTransactions.Interrupt();
                    }
                }
                OnPropertyChanged("IsRunning");
            }
        }

        ObservableCollection<HotItem> _transactionQueue = new ObservableCollection<HotItem>();
        public ObservableCollection<HotItem> TransactionQueue
        {
            get { return _transactionQueue; }
            private set
            {
                _transactionQueue = value;
                OnPropertyChanged("TransactionQueue");
            }
        }

        public TransactionController(HotItemController hotItemController)
        {
            this.HotItemController = hotItemController;
        }

        public void Start()
        {
            tMainWorkerTransactions = new Thread(new ThreadStart(Run));
            tMainWorkerTransactions.Start();
        }

        private void Run()
        {
            try
            {
                while (IsRunning)
                {
                    if (HotItemController.IsSessionKeyValid || !HotItemController.CurrentApi.IsUnsafe)
                    {
                        for (int i = 0; i < TransactionQueue.Count; i++)
                        {
                            HotItem item = TransactionQueue[i];

                            Task.Factory.StartNew(() =>
                            {
                                item.BuildItem(false); // It checks that the item gets build only once
                                item.Crawl();
                                item.CompareToRules();
                            });
                            Thread.Sleep(150);
                        }
                    }
                    Thread.Sleep(3500);
                }
            }
            catch (ThreadInterruptedException)
            {
                IsRunning = false;
            }
        }
    }
}

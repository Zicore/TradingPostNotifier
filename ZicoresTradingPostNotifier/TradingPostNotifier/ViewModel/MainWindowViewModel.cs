using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scraper.Notifier;
using System.Windows.Input;
using Scraper.Crawler;
using ZicoresTradingPostNotifier.Event;
using System.Collections.ObjectModel;
using ZicoresTradingPostNotifier.View;
using System.Windows;
using System.Diagnostics;
using System.Threading;
using ZicoresTradingPostNotifier.Model;
using System.Windows.Threading;
using LibraryBase.Wpf.Event;
using LibraryBase.Wpf.Commands;
using LibraryBase.Wpf.ViewModel;
using GuildWarsCalculator;
using System.Threading.Tasks;
using Scraper.Notifier.Event;

namespace ZicoresTradingPostNotifier.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        DispatcherTimer _timer;
        public DispatcherTimer Timer
        {
            get { return _timer; }
            set { _timer = value; }
        }

        Thread workerThread;

        RelayCommand _exportWatchlistCommand;
        RelayCommand _importWatchlistCommand;
        RelayCommand _searchCommand;
        RelayCommand _closeCommand;
        RelayCommand _resetIndicesCommand;
        RelayCommand _clearWatchlistCommand;
        RelayCommand _buyOfferCommand;

        private Config _config;

        private static Dispatcher _dispatcher;
        private bool _isRunning = true;

        private TransactionViewModel _soldViewModel;
        private TransactionViewModel _boughtViewModel;
        private TransactionViewModel _buyingViewModel;
        private TransactionViewModel _sellingViewModel;
        private GemViewModel _gemViewModel;
        private ChartViewModel _chartViewModel;
        private SettingsViewModel _settingsViewModel;
        private SearchViewModel _searchViewModel;
        private RecipeViewModel _recipeViewModel;

        private MessageViewModel _messageViewModel;

        FileSaveViewModel _fileSaveProvider;
        FileOpenViewModel _fileOpenProvider;

        public RecipeViewModel RecipeViewModel
        {
            get { return _recipeViewModel; }
            set { _recipeViewModel = value; }
        }

        public MessageViewModel MessageViewModel
        {
            get { return _messageViewModel; }
            set
            {
                _messageViewModel = value;
                OnPropertyChanged("MessageViewModel");
            }
        }

        public SearchViewModel SearchViewModel
        {
            get { return _searchViewModel; }
            set
            {
                _searchViewModel = value;
                OnPropertyChanged("SearchViewModel");
            }
        }

        public ChartViewModel ChartViewModel
        {
            get { return _chartViewModel; }
            set
            {
                _chartViewModel = value;
                OnPropertyChanged("ChartViewModel");
            }
        }

        public SettingsViewModel SettingsViewModel
        {
            get { return _settingsViewModel; }
            set
            {
                _settingsViewModel = value;
                OnPropertyChanged("SettingsViewModel");
            }
        }

        public TransactionViewModel BuyingViewModel
        {
            get { return _buyingViewModel; }
            set { _buyingViewModel = value; }
        }

        public TransactionViewModel SellingViewModel
        {
            get { return _sellingViewModel; }
            set { _sellingViewModel = value; }
        }

        public TransactionViewModel SoldViewModel
        {
            get { return _soldViewModel; }
            set { _soldViewModel = value; }
        }

        public TransactionViewModel BoughtViewModel
        {
            get { return _boughtViewModel; }
            set { _boughtViewModel = value; }
        }

        public GemViewModel GemViewModel
        {
            get { return _gemViewModel; }
            set
            {
                _gemViewModel = value;
                OnPropertyChanged("GemViewModel");
            }
        }

        public MainWindow MainWindow
        {
            get { return _mainWindow; }
            set { _mainWindow = value; }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
            set { _isRunning = value; }
        }

        public static Dispatcher Dispatcher
        {
            get { return _dispatcher; }
            private set { _dispatcher = value; }
        }

        public Visibility TransactionsVisibility
        {
            get { return HotItemController.IsTransactionsSupported ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility MultiLanguageVisibility
        {
            get { return HotItemController.IsMultiLanguageSupported ? Visibility.Visible : Visibility.Collapsed; }
        }

        public String Title
        {
            get
            {
                return HotItemController.IsUnsafe ?
                "Zicore's - Guild Wars 2 - Trading Post Notifier (Trading Post)" : "Zicore's - Guild Wars 2 - Trading Post Notifier (GW2Spidy.com)";
            }
        }
        RelayCommand _viewRecipeCommand;
        public ICommand ViewRecipeCommand
        {
            get
            {
                if (_viewRecipeCommand == null)
                    _viewRecipeCommand = new RelayCommand(param => this.ViewRecipe(param));

                return _viewRecipeCommand;
            }
        }

        ObservableCollection<HotItem> _searchedItems = new ObservableCollection<HotItem>();

        public ObservableCollection<HotItem> Queue
        {
            get
            {
                if (_searchedItems != null && _searchedItems.Count > 0)
                {
                    return _searchedItems;
                }
                return HotItemController.Queue;
            }
        }

        private String _searchQuery = "";
        public String SearchQuery
        {
            get { return _searchQuery; }
            set
            {
                _searchQuery = value;

                if (!String.IsNullOrEmpty(value))
                {
                    new Task(() =>
                        {
                            var items =
                                HotItemController.Queue.Where(x => x.Name.ToLower().Contains(value.ToLower())).ToList();
                            _searchedItems = new ObservableCollection<HotItem>(items);

                            OnPropertyChanged("SearchQuery");
                            OnPropertyChanged("Queue");
                        }).Start();
                }
                else
                {
                    _searchedItems = new ObservableCollection<HotItem>();
                    OnPropertyChanged("SearchQuery");
                    OnPropertyChanged("Queue");
                }
            }
        }


        public void ViewRecipe(object param)
        {
            HotItem item = (HotItem)param;
            if (item != null && item.IsRecipeItem)
            {
                RecipeViewModel.ViewRecipe(item);
            }
        }


        public MainWindowViewModel(MainWindow mainWindow, bool isTradingPostDataProvider)
        {
            this.MainWindow = mainWindow;
            MainWindowViewModel.Dispatcher = mainWindow.Dispatcher;

            HotItem.AddItemRequest += new EventHandler<EventArgs<HotItem>>(HotItemViewModel_AddItemRequest);
            HotItem.RemoveItemRequest += new EventHandler<EventArgs<HotItem>>(HotItemViewModel_RemoveItemRequest);

            _fileSaveProvider = new FileSaveViewModel();
            _fileSaveProvider.Filter = "CSV Files|*.csv";
            _fileSaveProvider.PathSelected += fileSave_PathSelected;

            _fileOpenProvider = new FileOpenViewModel();
            _fileOpenProvider.Filter = "CSV Files|*csv";
            _fileOpenProvider.PathSelected += _fileOpenProvider_PathSelected;

            HotItemController = new HotItemController(isTradingPostDataProvider);

            HotItemController.GuildWars2StatusChanged += new EventHandler<EventArgs<GuildWars2Status>>(HotItemController_GuildWars2StatusChanged);

            NotifiactionViewModel = new NotificationViewModel(this);

            SoldViewModel = new TransactionViewModel(this, TransactionType.Sold);
            SoldViewModel.DisplayName = "Sold Items";

            BoughtViewModel = new TransactionViewModel(this, TransactionType.Bought);
            BoughtViewModel.DisplayName = "Bought Items";

            BuyingViewModel = new TransactionViewModel(this, TransactionType.Buying);
            BuyingViewModel.DisplayName = "Buying Items";

            SellingViewModel = new TransactionViewModel(this, TransactionType.Selling);
            SellingViewModel.DisplayName = "Selling Items";

            MessageViewModel = new ViewModel.MessageViewModel(this);

            SettingsViewModel = new ViewModel.SettingsViewModel(HotItemController);
            SearchViewModel = new ViewModel.SearchViewModel(HotItemController, this);
            RecipeViewModel = new RecipeViewModel(HotItemController);
            ChartViewModel = new ChartViewModel(this);
            GemViewModel = new GemViewModel(this);

            LoadConfig();
            Config.IsTradingPostDataProvider = isTradingPostDataProvider;
            Config.FirstTimeStarted = false;
            HotItemController.Config = this.Config;
            HotItemController.StartWorker();

            workerThread = new Thread(Worker);
            workerThread.Start();

            //Timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, new EventHandler(timerTick), MainWindowViewModel.Dispatcher);
        }

        void timerTick(Object sender, EventArgs e)
        {

        }

        void _fileOpenProvider_PathSelected(object sender, EventArgs<FileOpenViewModel> e)
        {
            HotItemController.Import(e.Value.SelectedPath);
            OnPropertyChanged("Queue");
        }

        void fileSave_PathSelected(object sender, EventArgs<FileSaveViewModel> e)
        {
            HotItemController.Export(e.Value.SelectedPath);
        }

        private HotItemController _hotItemController;
        private int _dataId = 20323; // Unidentified Dye
        private NotificationViewModel _notifiactionViewModel;
        private MainWindow _mainWindow;

        public HotItemController HotItemController
        {
            get { return _hotItemController; }
            set
            {
                _hotItemController = value;
                OnPropertyChanged("HotItemController");
            }
        }

        public int DataId
        {
            get { return _dataId; }
            set
            {
                _dataId = value;
                OnPropertyChanged("DataId");
            }
        }

        public NotificationViewModel NotifiactionViewModel
        {
            get { return _notifiactionViewModel; }
            set { _notifiactionViewModel = value; }
        }

        public Config Config
        {
            get { return _config; }
            set { _config = value; }
        }

        public Visibility ProcessBorderVisibility
        {
            get
            {
                switch (HotItemController.GuildWars2Status)
                {
                    case GuildWars2Status.NotRunning:
                        return Visibility.Visible;
                    case GuildWars2Status.SearchingKey:
                        return Visibility.Visible;
                    case GuildWars2Status.FoundKey:
                        return Visibility.Collapsed;
                    default:
                        return Visibility.Collapsed;
                }
            }
        }
        public string GuildWars2StatusText
        {
            get
            {
                switch (HotItemController.GuildWars2Status)
                {
                    case GuildWars2Status.NotRunning:
                        return "Please start Guild Wars 2";
                    case GuildWars2Status.SearchingKey:
                        return "Please open the trading post now";
                    case GuildWars2Status.FoundKey:
                        return "";
                    default:
                        return "";
                }
            }
        }

        Random random = new Random();

        private void Worker(object state)
        {
            try
            {
                while (IsRunning)
                {
                    RefreshTransactions();

                    for (int i = 0; i < BuyingViewModel.Items.Count; i++)
                    {
                        HotItem item = BuyingViewModel.Items[i];
                        if (item.Notify && DateTime.Now > item.AcceptTime + item.TimeOut)
                        {
                            item.Crawl();
                            if (item.BuyPrice > item.UnitPrice)
                            {
                                MainWindow.Dispatcher.BeginInvoke((Action)delegate
                                {
                                    var model =
                                        new NotificationModel(item,
                                            new NotifierRule(item, RuleType.Higher, item.UnitPrice, ContextType.Other), "Higher", DateTime.Now, NotificationType.Buy);

                                    NotifiactionViewModel.AddBuyNotification(model);
                                    NotifiactionViewModel.ShowOnNotifiaction();
                                });
                            }
                        }
                    }

                    for (int i = 0; i < SellingViewModel.Items.Count; i++)
                    {
                        HotItem item = SellingViewModel.Items[i];
                        if (item.Notify && DateTime.Now > item.AcceptTime + item.TimeOut)
                        {
                            item.Crawl();
                            if (item.SellPrice < item.UnitPrice)
                            {
                                MainWindow.Dispatcher.BeginInvoke((Action)delegate
                                {
                                    var model =
                                        new NotificationModel(item,
                                            new NotifierRule(item, RuleType.Less, item.UnitPrice, ContextType.Other), "Less", DateTime.Now, NotificationType.Sell);
                                    NotifiactionViewModel.AddBuyNotification(model);
                                    NotifiactionViewModel.ShowOnNotifiaction();
                                });
                            }
                        }
                    }
                    Thread.Sleep(random.Next(HotItemController.CurrentApi.WorkerTransactionTimeOut - 500, HotItemController.CurrentApi.WorkerTransactionTimeOut + 500));
                }
            }
            catch (ThreadInterruptedException)
            {
                IsRunning = false;
            }
        }

        private void LoadConfig()
        {
            Config = new Config();
            try
            {
                Config.Load();
            }
            catch
            {
                Config.Save();
            }

            foreach (HotItem item in Config.Items)
            {
                AddItemInternal(item);
                item.BuyContext.Item = item; // XML Loading
                item.SellContext.Item = item;// XML Loading
                foreach (NotifierRule r in item.BuyContext.Rules)
                {
                    r.Item = item;
                    item.BuyContext.ActivateEventHandler(r);

                }
                foreach (NotifierRule r in item.SellContext.Rules)
                {
                    r.Item = item;
                    item.SellContext.ActivateEventHandler(r);
                }
            }

            HotItemController.Gem.LoadConfig(Config);
            SettingsViewModel.ChangeLanguage(Config.LanguageKey);
        }

        private void SaveConfig()
        {
            workerThread.Interrupt();
            HotItemController.Cache.IsRunning = false;
            HotItemController.IsRunning = false;

            Config.Items.Clear();
            foreach (HotItem item in this.HotItemController.Queue)
            {
                Config.Items.Add(item);
            }

            Config.LanguageKey = SettingsViewModel.SelectedLanguage.LanguageKey;

            Config.Save();
        }

        void HotItemController_GuildWars2StatusChanged(object sender, EventArgs<GuildWars2Status> e)
        {
            OnPropertyChanged("GuildWars2StatusText");
            OnPropertyChanged("ProcessBorderVisibility");
            if (e.Value == GuildWars2Status.FoundKey)
            {
                RefreshTransactions();
            }
        }

        private void RefreshTransactions()
        {
            SoldViewModel.RefreshItems();
            BoughtViewModel.RefreshItems();
            SellingViewModel.RefreshItems();
            BuyingViewModel.RefreshItems();
        }

        void HotItemViewModel_AddItemRequest(object sender, EventArgs<HotItem> e)
        {
            var vm = (HotItem)sender;
            AddItemInternal(vm);
        }

        void HotItemViewModel_RemoveItemRequest(object sender, EventArgs<HotItem> e)
        {
            var vm = (HotItem)sender;
            RemoveInternal(vm);
        }

        private void AddItemInternal(HotItem item)
        {
            HotItemController.Add(item);
        }

        private void RemoveInternal(HotItem item)
        {
            HotItemController.Remove(item);
        }

        private void UpdateTransactionList(ObservableCollection<HotItem> list, List<HotItem> items)
        {
            list.Clear();
            foreach (HotItem item in items)
            {
                list.Add(item);
            }
        }

        // ------------------------------------------------------------------------------------------

        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        public event EventHandler<RequestCloseEventArgs> RequestCloseEvent;

        protected virtual void OnRequestClose(bool canceled)
        {
            if (!canceled)
            {
                SaveConfig();
            }

            EventHandler<RequestCloseEventArgs> handler = this.RequestCloseEvent;
            if (handler != null)
                handler(this, new RequestCloseEventArgs(canceled));
        }

        public void RequestClose(bool cancel)
        {
            OnRequestClose(cancel);
        }

        /// <summary>
        /// Returns the command that, when invoked, attempts
        /// to remove this workspace from the user interface.
        /// </summary>
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                    _closeCommand = new RelayCommand(param => this.OnRequestClose(false));

                return _closeCommand;
            }
        }

        public ICommand ExportWatchlistCommand
        {
            get
            {
                if (_exportWatchlistCommand == null)
                    _exportWatchlistCommand = new RelayCommand(param =>
                    {
                        _fileSaveProvider.OpenDialogCommand.Execute(null);
                    });

                return _exportWatchlistCommand;
            }
        }

        public ICommand ImportWatchlistCommand
        {
            get
            {
                if (_importWatchlistCommand == null)
                    _importWatchlistCommand = new RelayCommand(param =>
                    {
                        _fileOpenProvider.OpenDialogCommand.Execute(null);
                    });

                return _importWatchlistCommand;
            }
        }

        public ICommand ResetIndicesCommand
        {
            get
            {
                if (_resetIndicesCommand == null)
                    _resetIndicesCommand = new RelayCommand(param =>
                    {
                        foreach (HotItem item in HotItemController.Queue)
                        {
                            item.SellContext.MovementIndex = 0;
                            item.BuyContext.MovementIndex = 0;
                        }
                    });

                return _resetIndicesCommand;
            }
        }

        public ICommand ClearWatchlistCommand
        {
            get
            {
                if (_clearWatchlistCommand == null)
                    _clearWatchlistCommand = new RelayCommand(param =>
                    {
                        HotItemController.Queue.Clear();
                        OnPropertyChanged("Queue");
                    });

                return _clearWatchlistCommand;
            }
        }
    }
}

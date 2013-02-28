using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NotifierCore.Notifier.Event;
using ZicoresTradingPostNotifier.View;
using System.Windows;
using ZicoresTradingPostNotifier.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;
using LibraryBase.Wpf.Commands;
using LibraryBase.Wpf.ViewModel;
using NotifierCore.Notifier;

namespace ZicoresTradingPostNotifier.ViewModel
{
    public class NotificationViewModel : BindableBase
    {
        RelayCommand _clearRuleCommand;
        RelayCommand _timeoutRuleCommand;

        private MainWindowViewModel _mainViewModel;
        private Notification _notificationView;
        private ObservableCollection<NotificationModel> _buyNotifications = new ObservableCollection<NotificationModel>();
        //private ObservableCollection<NotificationModel> _sellNotifications = new ObservableCollection<NotificationModel>();


        public MainWindowViewModel MainViewModel
        {
            get { return _mainViewModel; }
            set { _mainViewModel = value; }
        }

        public Notification NotificationView
        {
            get { return _notificationView; }
            set { _notificationView = value; }
        }

        public ObservableCollection<NotificationModel> BuyNotifications
        {
            get { return _buyNotifications; }
            set { _buyNotifications = value; }
        }

        //public ObservableCollection<NotificationModel> SellNotifications
        //{
        //    get { return _sellNotifications; }
        //    set { _sellNotifications = value; }
        //}

        public NotificationViewModel(MainWindowViewModel mainViewModel)
        {
            this.MainViewModel = mainViewModel;
            this.MainViewModel.MainWindow.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);

            NotificationView = new Notification();
            NotificationView.DataContext = this;
            //MainViewModel.HotItemController.SellNotification += new EventHandler<NotificationEventArgs>(HotItemController_SellNotification);
            MainViewModel.HotItemController.BuyNotification += new EventHandler<NotificationEventArgs>(HotItemController_BuyNotification);
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NotificationView.Close();
        }

        public void ShowOnNotifiaction()
        {
            try
            {
                if (!NotificationView.IsVisible)
                {
                    if (BuyNotifications.Count > 0)
                    {
                        double x = SystemParameters.PrimaryScreenWidth - NotificationView.Width;
                        double y = SystemParameters.PrimaryScreenHeight - NotificationView.Height - 40;
                        NotificationView.Left = x;
                        NotificationView.Top = y;
                        NotificationView.Show();
                    }
                }
                else
                {
                    //NotificationView.Hide();
                }
            }
            catch { }
        }

        public void ClearRule(NotificationModel item)
        {
            if (item.Item != null && item.Item.Notify)
            {
                item.Item.Notify = false;
            }
            else
            {
                Disable(item);
            }

            BuyNotifications.Remove(item);
            if (BuyNotifications.Count <= 0)
            {
                CloseRequest();
            }
        }

        public ICommand ClearRuleCommand
        {
            get
            {
                if (_clearRuleCommand == null)
                    _clearRuleCommand = new RelayCommand(param => this.ClearRule((NotificationModel)param));

                return _clearRuleCommand;
            }
        }

        public void TimeoutRule(NotificationModel item)
        {
            if (!item.IsGemNotification && item.Item.Notify)
            {
                item.Item.AcceptTime = DateTime.Now;
                item.Item.TimeOut = new TimeSpan(0, item.TimeoutMinutes, 0);
            }
            else
            {
                if (item.Rule != null)
                {
                    item.Rule.IsActive = false;
                    item.Rule.AcceptTime = DateTime.Now;
                    item.Rule.TimeOut = new TimeSpan(0, item.TimeoutMinutes, 0);
                }
                //if (item.Rule != null)
                //{
                //    item.Rule.IsActive = false;
                //    item.Rule.AcceptTime = DateTime.Now;
                //    item.Rule.TimeOut = new TimeSpan(0, item.TimeoutMinutes, 0);
                //}
            }

            BuyNotifications.Remove(item);
            if (BuyNotifications.Count <= 0)
            {
                CloseRequest();
            }
        }

        public ICommand TimeoutRuleCommand
        {
            get
            {
                if (_timeoutRuleCommand == null)
                    _timeoutRuleCommand = new RelayCommand(param => this.TimeoutRule((NotificationModel)param));

                return _timeoutRuleCommand;
            }
        }

        public void AddNotificationMessage(HotItem item, String message)
        {
            NotificationModel m = new NotificationModel(item, null, message, item.TransactionTimeLocal, NotificationType.Undefined);
            m.AsTransaction();
            AddBuyNotification(m);
        }

        public void AddBuyNotification(NotificationModel item)
        {
            AddNotification(BuyNotifications, item);
        }

        //public void AddSellNotification(NotificationModel item)
        //{
        //    AddNotification(SellNotifications, item);
        //}

        private void AddNotification(ObservableCollection<NotificationModel> collection, NotificationModel item)
        {
            NotificationView.Dispatcher.BeginInvoke((Action)delegate()
            {
                try
                {
                    var notificationItem = collection.FirstOrDefault(x => x.DataId == item.DataId);
                    if (notificationItem == null)
                    {
                        //var localItem = collection.FirstOrDefault(x => x.Item.DataId == item.Item.DataId);
                        collection.Add(item);
                    }
                    else
                    {
                        if (item.IsMessageNotification)
                        {
                            collection.Add(item);
                        }
                        else
                        {
                            //notificationItem.Rule.
                            
                        }
                    }
                }
                catch
                {

                }

                ShowOnNotifiaction();
            });
        }

        void HotItemController_BuyNotification(object sender, NotificationEventArgs e)
        {
            if (e.NotificationType == NotificationType.BuyGems || e.NotificationType == NotificationType.BuyGold)
            {
                AddNotification(BuyNotifications,
                                new NotificationModel(HotItemController.Self.Gem, e.GemRuleViewModel, e.Rule, e.Rule.SelectedRuleType.ToString(), DateTime.Now,
                                                      e.NotificationType));
            }
            else
            {
                AddNotification(BuyNotifications,
                                new NotificationModel(e.Item, e.Rule, e.Rule.SelectedRuleType.ToString(), DateTime.Now,
                                                      e.NotificationType));
            }
        }

        //void HotItemController_SellNotification(object sender, NotificationEventArgs e)
        //{
        //    AddNotification(BuyNotifications, new NotificationModel(e.Item, null, e.Rule, e.Rule.SelectedRuleType.ToString(), DateTime.Now)); // Currently we add Sell notifications to the buy list too
        //}

        private void Disable(NotificationModel model)
        {
            if (model.Rule != null)
            {
                model.Rule.Disable();
            }
            //if (model.RuleBuy != null)
            //{
            //    model.RuleBuy.Disable();
            //}
        }

        public void CloseRequest()
        {
            // We Acknowledge every notification otherwise it would bother without end

            for (int i = 0; i < BuyNotifications.Count; i++)
            {
                NotificationModel n = BuyNotifications[i];
                n.TimeoutMinutes = 10;
                TimeoutRule(n);
            }

            //for (int i = 0; i < SellNotifications.Count; i++)
            //{
            //    NotificationModel n = SellNotifications[i];
            //    n.TimeoutMinutes = 5;
            //    TimeoutRule(n);
            //}

            BuyNotifications.Clear();
            //SellNotifications.Clear();

            NotificationView.Hide();
        }
    }
}

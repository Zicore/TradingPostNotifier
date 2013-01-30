using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using LibraryBase.Wpf.Commands;
using LibraryBase.Wpf.ViewModel;
using ZicoresTradingPostNotifier.Model;

namespace ZicoresTradingPostNotifier.ViewModel
{
    public class MessageViewModel : BindableBase
    {
        RelayCommand _ackknowledgeCommand;

        MainWindowViewModel mainViewModel;
        public MainWindowViewModel MainViewModel
        {
            get { return mainViewModel; }
            set { mainViewModel = value; }
        }

        public MessageViewModel(MainWindowViewModel mainViewModel)
        {
            this.MainViewModel = mainViewModel;

            // -------------------------------------------------- //
            // Test
            //AddMessage(Message.CreateInfo("Info", "Item wurde erfolgreich gekauft."));
            //AddMessage(Message.CreateInfo("Info", "Item wurde erfolgreich gekauft."));
            //AddMessage(Message.CreateInfo("Info", "Item wurde erfolgreich gekauft."));
            //AddMessage(Message.CreateInfo("Info", "Item wurde erfolgreich gekauft."));
            //AddMessage(Message.CreateInfo("Info", "Item wurde erfolgreich gekauft."));
            //AddMessage(Message.CreateInfo("Info", "Item wurde erfolgreich gekauft."));
        }

        public RelayCommand AcknowledgeCommand
        {
            get
            {
                if (_ackknowledgeCommand == null)
                    _ackknowledgeCommand = new RelayCommand(x => this.Acknowledge(x));
                return _ackknowledgeCommand;
            }
        }

        public void Acknowledge(object param)
        {
            Message message = param as Message;
            if (message != null)
            {
                RemoveMessage(message);
            }
        }

        ObservableCollection<Message> _messages = new ObservableCollection<Message>();
        public ObservableCollection<Message> Messages
        {
            get { return _messages; }
            set { _messages = value; }
        }

        public void AddMessage(Message message)
        {
            MainWindowViewModel.Dispatcher.BeginInvoke((Action)delegate
            {
                Messages.Add(message);
            });
        }

        public void RemoveMessage(Message message)
        {
            MainWindowViewModel.Dispatcher.BeginInvoke((Action)delegate
            {
                Messages.Remove(message);
            });
        }
    }
}

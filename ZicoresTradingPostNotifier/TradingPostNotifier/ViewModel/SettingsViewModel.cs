﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ZicoresTradingPostNotifier.Model;
using NotifierCore.Notifier;
using LibraryBase.Wpf.ViewModel;
using LibraryBase.Wpf.Commands;

namespace ZicoresTradingPostNotifier.ViewModel
{
    public class SettingsViewModel : BindableBase
    {
        private RelayCommand _resetColumnsCommand;
        private RelayCommand _updateDatabaseCommand;
        private RelayCommand _openDatabaseFolderCommand;

        public RelayCommand ResetColumnsCommand
        {
            get
            {
                if (_resetColumnsCommand == null)
                    _resetColumnsCommand = new RelayCommand(x => ResetColumns(x.ToString()));
                return _resetColumnsCommand;
            }
        }

        public RelayCommand UpdateDatabaseCommand
        {
            get
            {
                if (_updateDatabaseCommand == null)
                    _updateDatabaseCommand = new RelayCommand(x => UpdateDatabase());
                return _updateDatabaseCommand;
            }
        }

        public RelayCommand OpenDatabaseFolderCommand
        {
            get
            {
                if (_openDatabaseFolderCommand == null)
                    _openDatabaseFolderCommand = new RelayCommand(x => OpenDatabaseFolder());
                return _openDatabaseFolderCommand;
            }
        }

        private void OpenDatabaseFolder()
        {
            var filePath = AppDomain.CurrentDomain.BaseDirectory;
            Task.Factory.StartNew(() =>
            {
                filePath = Path.Combine(filePath, "DB");
                Process.Start(filePath);
            });
        }

        private void UpdateDatabase()
        {
            var filePath = AppDomain.CurrentDomain.BaseDirectory;
            Task.Factory.StartNew(() =>
            {
                filePath = Path.Combine(filePath, "DatabaseGenerator.exe");
                Process.Start(filePath);
            });
        }

        public void ResetColumns(String key)
        {
            if (HotItemController.Config.Columns.ContainsKey(key))
            {
                if (!HotItemController.Config.ResetColumns.Contains(key))
                {
                    HotItemController.Config.ResetColumns.Add(key);
                }
            }
        }

        private LanguageModel _selectedLanguage = null;
        public SettingsViewModel(HotItemController controller)
        {
            this._controller = controller;
            this._controller.GuildWars2StatusChanged += _controller_GuildWars2StatusChanged;
            if (SelectedLanguage == null && Languages.Count > 0)
            {
                SelectedLanguage = Languages[0];
            }

        }

        void _controller_GuildWars2StatusChanged(object sender, LibraryBase.Wpf.Event.EventArgs<GuildWars2Status> e)
        {
            if (e.Value == GuildWars2Status.FoundKey && HotItemController.CurrentApi.IsUnsafe)
            {
                OnPropertyChanged("SessionKey");
            }
        }

        HotItemController _controller;

        private ObservableCollection<LanguageModel> _languages = new ObservableCollection<LanguageModel>()
        {
            new LanguageModel("en","English"),
            new LanguageModel("de","German"),
            new LanguageModel("fr","French"),
            new LanguageModel("es","Espain")
        };

        public ObservableCollection<LanguageModel> Languages
        {
            get { return _languages; }
            private set { _languages = value; }
        }

        public LanguageModel SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                _selectedLanguage = value;
                _controller.ChangeLanguage(SelectedLanguage.LanguageKey);
                OnPropertyChanged("SelectedLanguage");
            }
        }

        public void ChangeLanguage(String language)
        {
            try
            {
                var lang = Languages.First(x => x.LanguageKey == language);
                if (lang != null)
                {
                    this.SelectedLanguage = lang;
                }
            }
            catch
            {
                this.SelectedLanguage = Languages.FirstOrDefault();
            }
        }

        public int TransactionLimit
        {
            get { return HotItemController.Config.TransactionLimit; }
            set
            {
                if (HotItemController.Config.TransactionLimit != value)
                {
                    HotItemController.Config.TransactionLimit = value;
                    OnPropertyChanged("TransactionLimit");
                }
            }
        }


        public String SessionKey
        {
            get
            {
                return HotItemController.Config.SessionKey;
            }
        }

        public int TransactionTime
        {
            get { return HotItemController.Config.TimeItemsAreNew; }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 120)
                    value = 120;
                HotItemController.Config.TimeItemsAreNew = value;
                OnPropertyChanged("TransactionTime");
            }
        }

        public bool IsTradingPostDataprovider
        {
            get { return HotItemController.Config.IsTradingPostDataProvider; }
            set
            {
                HotItemController.Config.IsTradingPostDataProvider = value;
                OnPropertyChanged("IsTradingPostDataprovider");
                OnPropertyChanged("IsGW2SpidyDataprovider");
            }
        }

        public bool IsTransactionNotificationEnabled
        {
            get { return HotItemController.Config.IsTransactionNotificationEnabled; }
            set
            {
                HotItemController.Config.IsTransactionNotificationEnabled = value;
                OnPropertyChanged("IsTransactionNotificationEnabled");
            }
        }

        public bool IsGW2SpidyDataprovider
        {
            get { return !HotItemController.Config.IsTradingPostDataProvider; }
            set
            {
                HotItemController.Config.IsTradingPostDataProvider = !value;
                OnPropertyChanged("IsGW2SpidyDataprovider");
                OnPropertyChanged("IsTradingPostDataprovider");
            }
        }

        public bool IsTopMostNotification
        {
            get { return HotItemController.Config.IsTopMostNotification; }
            set
            {
                HotItemController.Config.IsTopMostNotification = value;
                OnPropertyChanged("IsTopMostNotification");
            }
        }
    }
}

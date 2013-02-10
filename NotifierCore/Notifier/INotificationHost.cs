using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GuildWarsCalculator;
using Scraper.Notifier;

namespace NotifierCore.Notifier
{
    public interface INotificationHost
    {
        ObservableCollection<NotifierRule> Rules { get; }
        Money Money { get; }
    }
}

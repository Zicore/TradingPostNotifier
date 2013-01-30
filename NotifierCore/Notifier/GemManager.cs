using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using GuildWarsCalculator;
using LibraryBase.Wpf.ViewModel;
using Newtonsoft.Json.Linq;
using Scraper.Crawler;
using ZicoresTradingPostNotifier.ViewModel;

namespace Scraper.Notifier
{
    public class GemManager : BindableBase
    {
        public GemManager()
        {
            BuyGemGemMoneyInput.ValueChanged += BuyGemGemMoneyInput_ValueChanged;
            BuyGoldGemMoneyInput.ValueChanged += BuyGoldGemMoneyInput_ValueChanged;
        }

        public void LoadConfig(Config config)
        {
            BuyGemsRules.Rules = config.RulesBuyGems;
            BuyGoldRules.Rules = config.RulesBuyGold;

            BuyGemsRules.ActivateEventHandler();
            BuyGoldRules.ActivateEventHandler();
        }

        // ------------------------------------------------------------ //

        //static String uriCoins = "https://exchange-live.ncplatform.net/ws/rates.json?id=undefined&coins={0}";
        //static String uriGems = "https://exchange-live.ncplatform.net/ws/rates.json?id=undefined&gems={0}";
        //static String host = "exchange-live.ncplatform.net";
        //static string referer = "https://exchange-live.ncplatform.net/";

        private bool _isBuyGemActive = false;
        private bool _isBuyGoldActive = false;

        // ------------------------------------------------------------ //

        int _buyGemPrice = 0;
        public int BuyGemPrice
        {
            get { return _buyGemPrice; }
            set
            {
                _buyGemPrice = value;
                OnPropertyChanged("BuyGemPrice");
                OnPropertyChanged("BuyGemPriceMoney");
                OnPropertyChanged("BuyGemGemValueFormat");
                OnPropertyChanged("Margin");
            }
        }

        public Money BuyGemPriceMoney
        {
            get { return new Money(BuyGemPrice * BuyGemPriceQuantity) { Name = "Gems for Gold" }; }
        }

        int _buyGemPriceQuantity = 100;
        public int BuyGemPriceQuantity
        {
            get { return _buyGemPriceQuantity; }
            set
            {
                _buyGemPriceQuantity = value;
                OnPropertyChanged("BuyGemPriceQuantity");
                OnPropertyChanged("BuyGemPriceMoney");
            }
        }

        public double BuyGemGemValue
        {
            get
            {
                if (BuyGemPrice == 0)
                    return 0;
                return (BuyGemGemMoneyInput.TotalCopper / (double)BuyGemPrice);
            }
        }

        public string BuyGemGemValueFormat
        {
            get { return string.Format("{0:0.00}", BuyGemGemValue); }
        }

        Money _buyGemGemMoneyInput = new Money(10000);
        public Money BuyGemGemMoneyInput
        {
            get { return _buyGemGemMoneyInput; }
            set
            {
                _buyGemGemMoneyInput = value;
                OnPropertyChanged("BuyGemGemMoneyInput");
                OnPropertyChanged("BuyGemGemValueFormat");
            }
        }

        int _buyGoldPrice = 0;
        public int BuyGoldPrice
        {
            get { return _buyGoldPrice; }
            set
            {
                _buyGoldPrice = value;
                OnPropertyChanged("BuyGoldPrice");
                OnPropertyChanged("BuyGoldPriceMoney");
                OnPropertyChanged("BuyGoldGemValueFormat");
                OnPropertyChanged("Margin");
            }
        }

        public Money BuyGoldPriceMoney
        {
            get { return new Money(BuyGoldPrice * BuyGoldPriceQuantity) { Name = "Gold for Gems" }; }
        }

        GemRuleViewModel _buyGoldRules = new GemRuleViewModel() { DisplayName = "100 Gems" };
        public GemRuleViewModel BuyGoldRules
        {
            get { return _buyGoldRules; }
            set
            {
                _buyGoldRules = value;
                OnPropertyChanged("BuyGoldRules");
            }
        }

        // ------------------------------------------------------------ //

        int _buyGoldPriceQuantity = 100;
        public int BuyGoldPriceQuantity
        {
            get { return _buyGoldPriceQuantity; }
            set
            {
                _buyGoldPriceQuantity = value;
                OnPropertyChanged("BuyGoldPriceQuantity");
                OnPropertyChanged("BuyGoldPriceMoney");
            }
        }

        public double BuyGoldGemValue
        {
            get
            {
                if (BuyGoldPrice == 0)
                    return 0;
                return (BuyGoldGemMoneyInput.TotalCopper / (double)BuyGoldPrice);
            }
        }

        public string BuyGoldGemValueFormat
        {
            get { return string.Format("{0:0.00}", BuyGoldGemValue); }
        }

        Money _buyGoldGemMoneyInput = new Money(10000);
        public Money BuyGoldGemMoneyInput
        {
            get { return _buyGoldGemMoneyInput; }
            set
            {
                _buyGoldGemMoneyInput = value;
                OnPropertyChanged("BuyGoldGemMoneyInput");
                OnPropertyChanged("BuyGoldGemValueFormat");
            }
        }

        GemRuleViewModel _buyGemsRules = new GemRuleViewModel() { DisplayName = "100 Gems" };
        public GemRuleViewModel BuyGemsRules
        {
            get { return _buyGemsRules; }
            set
            {
                _buyGemsRules = value;
                OnPropertyChanged("BuyGemsRules");
            }
        }

        // ------------------------------------------------------------ //

        public Money Margin
        {
            get { return new Money(BuyGemPrice * MarginQuantity - BuyGoldPrice * MarginQuantity) { Name = "Margin" }; }
        }

        private int _marginQuantity = 100;
        public int MarginQuantity
        {
            get { return _marginQuantity; }
            set
            {
                _marginQuantity = value;
                OnPropertyChanged("MarginQuantity");
                OnPropertyChanged("Margin");
            }
        }

        // ------------------------------------------------------------ //

        void BuyGemGemMoneyInput_ValueChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("BuyGemGemValueFormat");
        }

        void BuyGoldGemMoneyInput_ValueChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("BuyGoldGemValueFormat");
        }

        // ------------------------------------------------------------ //

        public void Update()
        {
            var api = HotItemController.CurrentApi;

            Cookie cookie = null;
            if (!String.IsNullOrEmpty(api.ExchangeHost))
            {
                cookie = new Cookie("s", HotItemController.Config.SessionKey, "/", api.ExchangeHost);
            }
            ScrapeHelper.Get(api.UriBuyGems(10000), api.ExchangeHost, cookie, api.ExchangeReferer, CompletedGemPrice, api.UserAgent);
            ScrapeHelper.Get(api.UriBuyGold(10000), api.ExchangeHost, cookie, api.ExchangeReferer, CompletedGoldPrice, api.UserAgent);

            BuyGemsRules.Money = new Money(BuyGemPrice * 100) { Name = "Gems for Gold" }; ;
            BuyGoldRules.Money = new Money(BuyGoldPrice * 100) { Name = "Gold for Gems" };

            if (_isBuyGemActive)
            {
                foreach (NotifierRule r in BuyGemsRules.Rules)
                {
                    if (r.Compare(BuyGemsRules.Money.TotalCopper))
                    {
                        HotItemController.Self.AddNotification(this,
                                                               new Event.NotificationEventArgs(0, null, r,
                                                                                               Event.NotificationType
                                                                                                    .BuyGems)
                                                                   {
                                                                       GemRuleViewModel
                                                                           =
                                                                           BuyGemsRules
                                                                   });
                    }
                }
            }

            if (_isBuyGoldActive)
            {
                foreach (NotifierRule r in BuyGoldRules.Rules)
                {
                    if (r.Compare(BuyGoldRules.Money.TotalCopper))
                    {
                        HotItemController.Self.AddNotification(this,
                                                               new Event.NotificationEventArgs(0, null, r,
                                                                                               Event.NotificationType
                                                                                                    .BuyGold)
                                                                   {
                                                                       GemRuleViewModel
                                                                           =
                                                                           BuyGoldRules
                                                                   });
                    }
                }
            }
        }

        private void CompletedGemPrice(PostResult rs)
        {
            var json = JObject.Parse(rs.Result);

            BuyGemPrice = Convert.ToInt32(HotItemController.CurrentApi.ParseBuyGemValue(json)); //json["results"]["gems"]["coins_per_gem"].ToObject<int>();

            _isBuyGemActive = true;
        }

        private void CompletedGoldPrice(PostResult rs)
        {
            var json = JObject.Parse(rs.Result);

            BuyGoldPrice = Convert.ToInt32(HotItemController.CurrentApi.ParseBuyGoldValue(json));//json["results"]["coins"]["coins_per_gem"].ToObject<int>();

            _isBuyGoldActive = true;
        }

        // ------------------------------------------------------------ //
    }
}

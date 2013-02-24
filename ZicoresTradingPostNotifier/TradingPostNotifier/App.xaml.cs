using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Globalization;
using ZicoresTradingPostNotifier.Event;
using ZicoresTradingPostNotifier.ViewModel;
using ZicoresTradingPostNotifier.View;
using NotifierCore.Notifier;

namespace ZicoresTradingPostNotifier
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(
              typeof(FrameworkElement),
              new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        MainWindow window;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Config config = new Config();
            try
            {
                config.LoadWithoutEvent();
            }
            catch
            {

            }

            if (config.FirstTimeStarted)
            {
                SplashView splash = new SplashView();
                splash.IsVisibleChanged += splash_IsVisibleChanged;
                splash.Show();
            }
            else
            {
                StartMain(config.IsTradingPostDataProvider);
            }
        }

        void splash_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isVisibleNew = (bool)e.NewValue;
            bool isVisibleOld = (bool)e.OldValue;
            if (!isVisibleNew && isVisibleOld)
            {
                SplashView splash = sender as SplashView;
                if (splash != null)
                {
                    if (splash.SplashScreenDecision == SplashScreenDecision.TradingPost || splash.SplashScreenDecision == SplashScreenDecision.Gw2Spidy)
                    {
                        StartMain(splash.SplashScreenDecision == SplashScreenDecision.TradingPost);
                        splash.Close();
                    }
                }
            }
        }

        private void StartMain(bool isTradingPostDataProvider)
        {
            window = new MainWindow();
            var viewModel = new MainWindowViewModel(window, isTradingPostDataProvider);
            window.DataContext = viewModel;
            window.Show();
        }
    }
}

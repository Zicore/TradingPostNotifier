using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ZicoresTradingPostNotifier.View
{
    public enum SplashScreenDecision
    {
        Gw2Spidy,
        TradingPost,
        Exit
    }

    /// <summary>
    /// Interaktionslogik für SplashView.xaml
    /// </summary>
    public partial class SplashView : Window
    {
        public SplashView()
        {
            InitializeComponent();
        }

        private void Button_GW2Spidy_Click(object sender, RoutedEventArgs e)
        {
            this.SplashScreenDecision = View.SplashScreenDecision.Gw2Spidy;
            Hide();
        }

        private void Button_TradingPost_Click(object sender, RoutedEventArgs e)
        {
            this.SplashScreenDecision = View.SplashScreenDecision.TradingPost;
            Hide();
        }

        private void Button_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.SplashScreenDecision = View.SplashScreenDecision.Exit;
            Close();
        }

        SplashScreenDecision _splashScreenDecision = SplashScreenDecision.Exit;

        public SplashScreenDecision SplashScreenDecision
        {
            get { return _splashScreenDecision; }
            set { _splashScreenDecision = value; }
        }
    }
}

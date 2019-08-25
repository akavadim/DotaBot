using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DotaBot
{
    partial class WindowStatusParse : Window, DotaBot.PS.IViews.IStatusParse    //TODO: Придумать как убрать кнопку "Закрыть" на форме (Крестик)
    {
        private bool _parseEnded;

        public string PagesOfMatchesLoaded
        {
            get => LabelPagesOfMatchesLoaded.Content.ToString();
            set => Dispatcher.Invoke(() => LabelPagesOfMatchesLoaded.Content = value);
        }
        public string MatchesLoaded
        {
            get => LabelMatchesLoaded.Content.ToString();
            set { Dispatcher.Invoke(() => LabelMatchesLoaded.Content = value); }
        }
        public string TotalLoaded
        {
            get => LabelTotalLoaded.Content.ToString();
            set => Dispatcher.Invoke(() => LabelTotalLoaded.Content = value);
        }
        public bool ParseEnded
        {
            get
            {
                return _parseEnded;
            }
            set
            {
                Dispatcher.Invoke(() =>
                {
                    if (value)
                    {
                        ButtonReady.IsEnabled = true;
                        ButtonCancel.IsEnabled = false;
                    }
                    else
                    {
                        ButtonReady.IsEnabled = false;
                        ButtonCancel.IsEnabled = true;
                    }
                    _parseEnded = value;
                });
            }
        }

        public event EventHandler CancelParseClick;

        public WindowStatusParse()
        {
            InitializeComponent();
            PagesOfMatchesLoaded = "";
            MatchesLoaded = "";
            TotalLoaded = "";
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelParseClick?.Invoke(this, EventArgs.Empty);
        }

        private void ButtonReady_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}

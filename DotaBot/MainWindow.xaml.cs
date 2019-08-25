using System.Windows;
using System;
using Microsoft.Win32;

namespace DotaBot
{
    partial class MainWindow : Window, DotaBot.PS.IViews.IMainWindow
    {
        private string _pathNotSortedData;
        private string _pathDatabase;

        public string PathNotSortedData { get => _pathNotSortedData; }
        public string PathDatabase { get => _pathDatabase; }
        public string[] LeftGamersUrls
        {
            get
            {
                string[] gamers = new string[5];
                gamers[0] = TextBoxUrlLeftPlayer1.Text;
                gamers[1] = TextBoxUrlLeftPlayer2.Text;
                gamers[2] = TextBoxUrlLeftPlayer3.Text;
                gamers[3] = TextBoxUrlLeftPlayer4.Text;
                gamers[4] = TextBoxUrlLeftPlayer5.Text;
                return gamers;
            }
        }
        public string[] RightGamersUrls
        {
            get
            {
                string[] gamers = new string[5];
                gamers[0] = TextBoxUrlRightPlayer1.Text;
                gamers[1] = TextBoxUrlRightPlayer2.Text;
                gamers[2] = TextBoxUrlRightPlayer3.Text;
                gamers[3] = TextBoxUrlRightPlayer4.Text;
                gamers[4] = TextBoxUrlRightPlayer5.Text;
                return gamers;
            }
        }
        public string DatabaseStatus
        {
            get =>LabelDatabaseStatus.Content.ToString();
            set => Dispatcher.Invoke(() => LabelDatabaseStatus.Content = value);
        }
        public string NotSortedDataStatus
        {
            get =>LabelNotSortedDataStatus.Content.ToString();
            set => Dispatcher.Invoke(() =>LabelNotSortedDataStatus.Content = value);
        }

        public event DotaBot.PS.IViews.ParseEventHandler ParsePageClick;
        public event DotaBot.PS.IViews.ParseEventHandler ParseClick;
        public event DotaBot.PS.IViews.ParseEventHandler ParseMultyThreadClick;
        public event DotaBot.PS.IViews.ParseEventHandler UpdateNotSortedDataClick;
        public event EventHandler LoadFileNotSortedDataClick;
        public event EventHandler LoadFileDatabaseClick;
        public event EventHandler SaveFileNotSortedDataClick;
        public event EventHandler SaveFileDatabaseClick;
        public event EventHandler AnalizDataClick;
        public event EventHandler ResultMatchClick;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Ретрансляция событий

        private void ButtonAnalizData_Click(object sender, RoutedEventArgs e)
        {
            AnalizDataClick?.Invoke(this, new EventArgs());
        }

        private void ButtonResultMatch_Click(object sender, RoutedEventArgs e)
        {
            ResultMatchClick?.Invoke(this, new EventArgs());
        }

        #endregion

        private void ButtonLoadNotSortedData_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Файлы NotSortedData|*.nsd|Все Файлы|*.*";
            if (ofd.ShowDialog() == true)
            {
                _pathNotSortedData = ofd.FileName;
                LoadFileNotSortedDataClick?.Invoke(this, EventArgs.Empty);
            }
        }

        private void ButtonLoadDatabase_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Файлы Database|*.db|Все Файлы|*.*";
            if (ofd.ShowDialog() == true)
            {
                _pathDatabase = ofd.FileName;
                LoadFileDatabaseClick?.Invoke(this, EventArgs.Empty);
            }
        }

        private void ButtonUpdateNotSortedData_Click(object sender, RoutedEventArgs e)
        {
            WindowStatusParse statusParse = new WindowStatusParse() { Owner = this };
            UpdateNotSortedDataClick?.Invoke(this, statusParse);
            statusParse.ShowDialog();
        }

        private void ButtonParsePage_Click(object sender, RoutedEventArgs e)
        {
            WindowStatusParse statusParse = new WindowStatusParse() { Owner = this };
            ParsePageClick?.Invoke(this, statusParse);
            statusParse.ShowDialog();
        }

        private void ButtonParse_Click(object sender, RoutedEventArgs e)
        {
            WindowStatusParse statusParse = new WindowStatusParse() { Owner = this };
            ParseClick?.Invoke(this, statusParse);
            statusParse.ShowDialog();
        }

        private void ButtonParseMultyThread_Click(object sender, RoutedEventArgs e)
        {
            WindowStatusParse statusParse = new WindowStatusParse() { Owner = this };
            ParseMultyThreadClick?.Invoke(this, statusParse);
            statusParse.ShowDialog();
        }

        private void ButtonSaveNotSorteedData_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Файлы NotSortedData|*.nsd|Все Файлы|*.*";
            if (sfd.ShowDialog() == true)
            {
                _pathNotSortedData = sfd.FileName;
                SaveFileNotSortedDataClick?.Invoke(this, EventArgs.Empty);
            }
        }

        private void ButtonSaveDatabase_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Файлы Database|*.db|Все Файлы|*.*";
            if (sfd.ShowDialog() == true)
            {
                _pathDatabase = sfd.FileName;
                SaveFileDatabaseClick?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}

using System.Windows;

namespace DotaBot
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        [System.STAThread]
        static void Main()
        {
            DotaBot.App app = new DotaBot.App();

            MainWindow mainWindow = new MainWindow();
            MessageService messageService = new MessageService();
            DotaBot.BL.Model model = new DotaBot.BL.Model();
            DotaBot.PS.Presenter presenter = new DotaBot.PS.Presenter(mainWindow, messageService, model);

            app.InitializeComponent();
            app.Run(mainWindow);
        }
    }
}

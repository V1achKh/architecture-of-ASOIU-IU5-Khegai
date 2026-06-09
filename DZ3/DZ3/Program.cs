using ZooApp.Data;
using ZooApp.Forms;

namespace ZooApp;

/// <summary>
/// Точка входа в приложение.
/// </summary>
internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        AppDbContext.InitializeDatabase();
        Application.Run(new MainForm());
    }
}

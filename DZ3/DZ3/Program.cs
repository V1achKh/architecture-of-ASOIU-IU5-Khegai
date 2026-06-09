using DZ3.Data;
using DZ3.Forms;

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

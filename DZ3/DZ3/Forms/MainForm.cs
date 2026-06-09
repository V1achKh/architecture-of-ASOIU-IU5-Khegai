using DZ3.Forms;

namespace DZ3.Forms;

/// <summary>
/// Главное окно приложения с навигацией между разделами.
/// </summary>
public class MainForm : Form
{
    private Button _btnZoos    = null!;
    private Button _btnAnimals = null!;
    private Button _btnReport  = null!;
    private Label  _lblTitle   = null!;

    /// <summary>Инициализирует главное окно.</summary>
    public MainForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text            = "Зоопарки — управление животными";
        Size            = new Size(400, 300);
        StartPosition   = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        BackColor       = Color.FromArgb(240, 248, 255);

        _lblTitle = new Label
        {
            Text      = "🐾 Зоопарки",
            Font      = new Font("Segoe UI", 18f, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 80, 140),
            AutoSize  = false,
            Width     = 360,
            Height    = 50,
            Top       = 20,
            Left      = 20,
            TextAlign = ContentAlignment.MiddleCenter,
        };

        _btnZoos = MakeNavButton("🏛  Зоопарки (справочник)", 90);
        _btnZoos.Click += (_, _) =>
        {
            new ZoosForm().ShowDialog(this);
        };

        _btnAnimals = MakeNavButton("🦁  Животные", 150);
        _btnAnimals.Click += (_, _) =>
        {
            new AnimalsForm().ShowDialog(this);
        };

        _btnReport = MakeNavButton("📊  Отчёты", 210);
        _btnReport.Click += (_, _) =>
        {
            new ReportForm().ShowDialog(this);
        };

        Controls.AddRange(new Control[] { _lblTitle, _btnZoos, _btnAnimals, _btnReport });
    }

    private static Button MakeNavButton(string text, int top)
    {
        return new Button
        {
            Text      = text,
            Font      = new Font("Segoe UI", 11f),
            Width     = 300,
            Height    = 44,
            Left      = 50,
            Top       = top,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(30, 100, 180),
            ForeColor = Color.White,
            Cursor    = Cursors.Hand,
        };
    }
}

using Microsoft.EntityFrameworkCore;
using ZooApp.Data;

namespace ZooApp.Forms;

/// <summary>
/// Форма отчётов.
/// Три раздела, сформированных средствами LINQ:
///   1. Полный список животных с зоопарками
///   2. Количество животных по зоопаркам
///   3. Средняя масса животных по зоопаркам (по убыванию)
/// </summary>
public class ReportForm : Form
{
    private TabControl    _tabs   = null!;
    private DataGridView  _grid1  = null!;
    private DataGridView  _grid2  = null!;
    private DataGridView  _grid3  = null!;
    private Button        _btnRefresh = null!;
    private Button        _btnClose   = null!;

    /// <summary>Инициализирует форму отчётов.</summary>
    public ReportForm()
    {
        InitializeComponent();
        LoadAllReports();
    }

    private void InitializeComponent()
    {
        Text          = "Отчёты";
        Size          = new Size(740, 560);
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize   = new Size(500, 400);

        _tabs = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10f) };

        _grid1 = MakeGrid();
        _grid2 = MakeGrid();
        _grid3 = MakeGrid();

        var page1 = new TabPage("1. Все животные");
        page1.Controls.Add(_grid1);
        var page2 = new TabPage("2. Кол-во по зоопаркам");
        page2.Controls.Add(_grid2);
        var page3 = new TabPage("3. Средняя масса");
        page3.Controls.Add(_grid3);

        _tabs.TabPages.AddRange(new[] { page1, page2, page3 });

        var panel = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(8) };

        _btnRefresh = new Button
        {
            Text      = "Обновить",
            Left      = 8, Top = 8, Width = 110, Height = 34,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(0, 123, 255),
            ForeColor = Color.White,
            Font      = new Font("Segoe UI", 9f),
            Cursor    = Cursors.Hand,
        };
        _btnRefresh.Click += (_, _) => LoadAllReports();

        _btnClose = new Button
        {
            Text      = "Закрыть",
            Left      = 126, Top = 8, Width = 100, Height = 34,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(108, 117, 125),
            ForeColor = Color.White,
            Font      = new Font("Segoe UI", 9f),
            Cursor    = Cursors.Hand,
        };
        _btnClose.Click += (_, _) => Close();

        panel.Controls.AddRange(new Control[] { _btnRefresh, _btnClose });
        Controls.Add(_tabs);
        Controls.Add(panel);
    }

    // ── Загрузка отчётов ─────────────────────────────────────────────────

    /// <summary>Загружает (или обновляет) все три раздела отчёта.</summary>
    private void LoadAllReports()
    {
        using var ctx = new AppDbContext();

        // ── Раздел 1: полный список с названием зоопарка ─────────────
        var report1 = ctx.Animals
            .Include(a => a.Zoo)
            .OrderBy(a => a.Name)
            .Select(a => new
            {
                Животное = a.Name,
                Зоопарк  = a.Zoo!.Name,
                Масса_кг = a.WeightKg,
            })
            .ToList();

        _grid1.DataSource = report1;
        FormatGrid(_grid1);

        // ── Раздел 2: количество животных по зоопаркам ───────────────
        var report2 = ctx.Animals
            .GroupBy(a => a.Zoo!.Name)
            .Select(g => new
            {
                Зоопарк          = g.Key,
                Кол_во_животных  = g.Count(),
            })
            .OrderBy(r => r.Зоопарк)
            .ToList();

        _grid2.DataSource = report2;
        FormatGrid(_grid2);

        // ── Раздел 3: средняя масса по зоопаркам (по убыванию) ───────
        var report3 = ctx.Animals
            .GroupBy(a => a.Zoo!.Name)
            .Select(g => new
            {
                Зоопарк              = g.Key,
                Средняя_масса_кг     = Math.Round(g.Average(a => a.WeightKg), 1),
            })
            .OrderByDescending(r => r.Средняя_масса_кг)
            .ToList();

        _grid3.DataSource = report3;
        FormatGrid(_grid3);
    }

    // ── Вспомогательные методы ────────────────────────────────────────────

    private static DataGridView MakeGrid()
        => new DataGridView
        {
            Dock                  = DockStyle.Fill,
            ReadOnly              = true,
            AllowUserToAddRows    = false,
            AllowUserToDeleteRows = false,
            SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible     = false,
            BackgroundColor       = Color.White,
            Font                  = new Font("Segoe UI", 10f),
        };

    private static void FormatGrid(DataGridView grid)
    {
        grid.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 10f, FontStyle.Bold);
        grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 100, 180);
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.EnableHeadersVisualStyles               = false;
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(235, 245, 255);
    }
}

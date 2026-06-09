using Microsoft.EntityFrameworkCore;
using ZooApp.Data;
using ZooApp.Models;

namespace ZooApp.Forms;

/// <summary>
/// Форма управления справочником зоопарков (CRUD).
/// </summary>
public class ZoosForm : Form
{
    private DataGridView _grid    = null!;
    private Button       _btnAdd  = null!;
    private Button       _btnEdit = null!;
    private Button       _btnDel  = null!;
    private Button       _btnClose = null!;

    /// <summary>Инициализирует форму зоопарков.</summary>
    public ZoosForm()
    {
        InitializeComponent();
        LoadData();
    }

    private void InitializeComponent()
    {
        Text            = "Зоопарки — справочник";
        Size            = new Size(560, 420);
        StartPosition   = FormStartPosition.CenterParent;
        MinimumSize     = new Size(400, 300);

        // ── Таблица ──────────────────────────────────
        _grid = new DataGridView
        {
            Dock                  = DockStyle.Fill,
            ReadOnly              = true,
            AllowUserToAddRows    = false,
            AllowUserToDeleteRows = false,
            SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect           = false,
            AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible     = false,
            BackgroundColor       = Color.White,
            Font                  = new Font("Segoe UI", 10f),
        };

        // ── Панель кнопок ────────────────────────────
        var panel = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(8) };

        _btnAdd  = MakeButton("Добавить",    Color.FromArgb(40, 167, 69));
        _btnEdit = MakeButton("Редактировать", Color.FromArgb(0, 123, 255));
        _btnDel  = MakeButton("Удалить",     Color.FromArgb(220, 53, 69));
        _btnClose = MakeButton("Закрыть",    Color.FromArgb(108, 117, 125));

        _btnAdd.Click   += BtnAdd_Click;
        _btnEdit.Click  += BtnEdit_Click;
        _btnDel.Click   += BtnDel_Click;
        _btnClose.Click += (_, _) => Close();

        int x = 8;
        foreach (var btn in new[] { _btnAdd, _btnEdit, _btnDel, _btnClose })
        {
            btn.Left = x;
            btn.Top  = 8;
            x += btn.Width + 6;
        }

        panel.Controls.AddRange(new Control[] { _btnAdd, _btnEdit, _btnDel, _btnClose });
        Controls.Add(_grid);
        Controls.Add(panel);
    }

    // ── Загрузка данных ───────────────────────────────────────────────────

    /// <summary>Загружает список зоопарков из базы данных.</summary>
    private void LoadData()
    {
        using var ctx = new AppDbContext();
        var zoos = ctx.Zoos
            .OrderBy(z => z.Name)
            .Select(z => new { z.Id, z.Name })
            .ToList();

        _grid.DataSource = zoos;

        if (_grid.Columns.Contains("Id"))
        {
            _grid.Columns["Id"]!.HeaderText  = "ID";
            _grid.Columns["Id"]!.Width       = 60;
            _grid.Columns["Id"]!.FillWeight  = 0;
            _grid.Columns["Id"]!.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
        }
        if (_grid.Columns.Contains("Name"))
            _grid.Columns["Name"]!.HeaderText = "Название зоопарка";
    }

    // ── Выбранный зоопарк ────────────────────────────────────────────────

    private int? SelectedId()
    {
        if (_grid.CurrentRow == null) return null;
        return (int)_grid.CurrentRow.Cells["Id"].Value;
    }

    // ── Добавление ───────────────────────────────────────────────────────

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        using var dlg = new ZooEditDialog("Добавить зоопарк", "");
        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        using var ctx = new AppDbContext();
        ctx.Zoos.Add(new Zoo { Name = dlg.ZooName });
        ctx.SaveChanges();
        LoadData();
    }

    // ── Редактирование ───────────────────────────────────────────────────

    private void BtnEdit_Click(object? sender, EventArgs e)
    {
        int? id = SelectedId();
        if (id == null) { Info("Выберите зоопарк для редактирования."); return; }

        using var ctx = new AppDbContext();
        var zoo = ctx.Zoos.Find(id);
        if (zoo == null) { LoadData(); return; }

        using var dlg = new ZooEditDialog("Редактировать зоопарк", zoo.Name);
        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        zoo.Name = dlg.ZooName;
        ctx.Zoos.Update(zoo);
        ctx.SaveChanges();
        LoadData();
    }

    // ── Удаление ─────────────────────────────────────────────────────────

    private void BtnDel_Click(object? sender, EventArgs e)
    {
        int? id = SelectedId();
        if (id == null) { Info("Выберите зоопарк для удаления."); return; }

        using var ctx = new AppDbContext();
        var zoo = ctx.Zoos.Include(z => z.Animals).FirstOrDefault(z => z.Id == id);
        if (zoo == null) { LoadData(); return; }

        if (zoo.Animals.Count > 0)
        {
            MessageBox.Show(
                $"Невозможно удалить «{zoo.Name}»:\nс этим зоопарком связано {zoo.Animals.Count} животных.\nСначала удалите или переназначьте животных.",
                "Удаление запрещено",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        if (MessageBox.Show(
                $"Удалить зоопарк «{zoo.Name}»?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        ctx.Zoos.Remove(zoo);
        ctx.SaveChanges();
        LoadData();
    }

    private static void Info(string msg)
        => MessageBox.Show(msg, "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);

    private static Button MakeButton(string text, Color color)
        => new Button
        {
            Text      = text,
            Width     = 120,
            Height    = 34,
            FlatStyle = FlatStyle.Flat,
            BackColor = color,
            ForeColor = Color.White,
            Font      = new Font("Segoe UI", 9f),
            Cursor    = Cursors.Hand,
        };
}

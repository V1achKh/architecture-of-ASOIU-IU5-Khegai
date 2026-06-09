using DZ3.Models;
using Microsoft.EntityFrameworkCore;
using DZ3.Data;

namespace DZ3.Forms;

/// <summary>
/// Форма управления животными (CRUD).
/// Показывает список со столбцом «Зоопарк» через Include.
/// </summary>
public class AnimalsForm : Form
{
    private DataGridView _grid     = null!;
    private Button       _btnAdd   = null!;
    private Button       _btnEdit  = null!;
    private Button       _btnDel   = null!;
    private Button       _btnClose = null!;

    /// <summary>Инициализирует форму животных.</summary>
    public AnimalsForm()
    {
        InitializeComponent();
        LoadData();
    }

    private void InitializeComponent()
    {
        Text          = "Животные";
        Size          = new Size(680, 480);
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize   = new Size(500, 350);

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

        var panel = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(8) };

        _btnAdd   = MakeButton("Добавить",      Color.FromArgb(40, 167, 69));
        _btnEdit  = MakeButton("Редактировать", Color.FromArgb(0, 123, 255));
        _btnDel   = MakeButton("Удалить",       Color.FromArgb(220, 53, 69));
        _btnClose = MakeButton("Закрыть",       Color.FromArgb(108, 117, 125));

        _btnAdd.Click   += BtnAdd_Click;
        _btnEdit.Click  += BtnEdit_Click;
        _btnDel.Click   += BtnDel_Click;
        _btnClose.Click += (_, _) => Close();

        int x = 8;
        foreach (var btn in new[] { _btnAdd, _btnEdit, _btnDel, _btnClose })
        {
            btn.Left = x; btn.Top = 8; x += btn.Width + 6;
        }

        panel.Controls.AddRange(new Control[] { _btnAdd, _btnEdit, _btnDel, _btnClose });
        Controls.Add(_grid);
        Controls.Add(panel);
    }

    // ── Загрузка данных ───────────────────────────────────────────────────

    /// <summary>Загружает всех животных с названиями зоопарков.</summary>
    private void LoadData()
    {
        using var ctx = new AppDbContext();
        var animals = ctx.Animals
            .Include(a => a.Zoo)
            .OrderBy(a => a.Name)
            .Select(a => new
            {
                a.Id,
                a.Name,
                Зоопарк    = a.Zoo!.Name,
                a.WeightKg,
            })
            .ToList();

        _grid.DataSource = animals;

        if (_grid.Columns.Contains("Id"))
        {
            _grid.Columns["Id"]!.HeaderText  = "ID";
            _grid.Columns["Id"]!.Width       = 50;
            _grid.Columns["Id"]!.FillWeight  = 0;
            _grid.Columns["Id"]!.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
        }
        if (_grid.Columns.Contains("Name"))
            _grid.Columns["Name"]!.HeaderText = "Название животного";
        if (_grid.Columns.Contains("Зоопарк"))
            _grid.Columns["Зоопарк"]!.HeaderText = "Зоопарк";
        if (_grid.Columns.Contains("WeightKg"))
        {
            _grid.Columns["WeightKg"]!.HeaderText = "Масса (кг)";
            _grid.Columns["WeightKg"]!.Width      = 100;
            _grid.Columns["WeightKg"]!.FillWeight = 0;
            _grid.Columns["WeightKg"]!.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
        }
    }

    private int? SelectedId()
    {
        if (_grid.CurrentRow == null) return null;
        return (int)_grid.CurrentRow.Cells["Id"].Value;
    }

    // ── Добавление ───────────────────────────────────────────────────────

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        List<Zoo> zoos;
        using (var ctx = new AppDbContext())
            zoos = ctx.Zoos.OrderBy(z => z.Name).ToList();

        if (zoos.Count == 0)
        {
            MessageBox.Show("Сначала добавьте хотя бы один зоопарк.", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var dlg = new AnimalEditDialog("Добавить животное", null, zoos);
        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        using var ctx2 = new AppDbContext();
        ctx2.Animals.Add(new Animal
        {
            ZooId    = dlg.SelectedZooId,
            Name     = dlg.AnimalName,
            WeightKg = dlg.WeightKg,
        });
        ctx2.SaveChanges();
        LoadData();
    }

    // ── Редактирование ───────────────────────────────────────────────────

    private void BtnEdit_Click(object? sender, EventArgs e)
    {
        int? id = SelectedId();
        if (id == null) { Info("Выберите животное для редактирования."); return; }

        Animal? animal;
        List<Zoo> zoos;
        using (var ctx = new AppDbContext())
        {
            animal = ctx.Animals.Find(id);
            zoos   = ctx.Zoos.OrderBy(z => z.Name).ToList();
        }
        if (animal == null) { LoadData(); return; }

        using var dlg = new AnimalEditDialog("Редактировать животное", animal, zoos);
        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        using var ctx2 = new AppDbContext();
        var toUpdate = ctx2.Animals.Find(id);
        if (toUpdate == null) { LoadData(); return; }

        toUpdate.ZooId    = dlg.SelectedZooId;
        toUpdate.Name     = dlg.AnimalName;
        toUpdate.WeightKg = dlg.WeightKg;
        ctx2.Animals.Update(toUpdate);
        ctx2.SaveChanges();
        LoadData();
    }

    // ── Удаление ─────────────────────────────────────────────────────────

    private void BtnDel_Click(object? sender, EventArgs e)
    {
        int? id = SelectedId();
        if (id == null) { Info("Выберите животное для удаления."); return; }

        using var ctx = new AppDbContext();
        var animal = ctx.Animals.Find(id);
        if (animal == null) { LoadData(); return; }

        if (MessageBox.Show(
                $"Удалить животное «{animal.Name}»?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        ctx.Animals.Remove(animal);
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

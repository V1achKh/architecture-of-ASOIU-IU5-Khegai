using ZooApp.Models;

namespace ZooApp.Forms;

/// <summary>
/// Диалог добавления / редактирования животного.
/// Содержит поля: название, выпадающий список зоопарков, масса (кг).
/// </summary>
public class AnimalEditDialog : Form
{
    private TextBox  _txtName   = null!;
    private ComboBox _cmbZoo    = null!;
    private TextBox  _txtWeight = null!;
    private Button   _btnOk     = null!;
    private Button   _btnCancel = null!;
    private Label    _lblError  = null!;

    /// <summary>Название животного, введённое пользователем.</summary>
    public string AnimalName     => _txtName.Text.Trim();

    /// <summary>Выбранный Id зоопарка.</summary>
    public int SelectedZooId     => ((Zoo)_cmbZoo.SelectedItem!).Id;

    /// <summary>Масса животного в кг.</summary>
    public double WeightKg       { get; private set; }

    /// <summary>
    /// Создаёт диалог.
    /// </summary>
    /// <param name="title">Заголовок окна.</param>
    /// <param name="animal">Существующее животное для редактирования (null — для добавления).</param>
    /// <param name="zoos">Список зоопарков для выпадающего списка.</param>
    public AnimalEditDialog(string title, Animal? animal, List<Zoo> zoos)
    {
        Text            = title;
        Size            = new Size(380, 250);
        StartPosition   = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        MinimizeBox     = false;

        // ── Поля ────────────────────────────────────────────────────────
        var lblName = MakeLabel("Название животного:", 16);
        _txtName    = new TextBox
        {
            Text  = animal?.Name ?? "",
            Left  = 12, Top = 36, Width = 340,
            Font  = new Font("Segoe UI", 10f),
        };

        var lblZoo = MakeLabel("Зоопарк:", 66);
        _cmbZoo    = new ComboBox
        {
            Left          = 12, Top = 86, Width = 340,
            Font          = new Font("Segoe UI", 10f),
            DropDownStyle = ComboBoxStyle.DropDownList,
            DisplayMember = "Name",
        };
        _cmbZoo.Items.AddRange(zoos.Cast<object>().ToArray());
        if (animal != null)
        {
            var match = zoos.FirstOrDefault(z => z.Id == animal.ZooId);
            _cmbZoo.SelectedItem = match ?? zoos[0];
        }
        else if (zoos.Count > 0)
        {
            _cmbZoo.SelectedIndex = 0;
        }

        var lblWeight = MakeLabel("Масса (кг):", 118);
        _txtWeight    = new TextBox
        {
            Text  = animal?.WeightKg.ToString() ?? "",
            Left  = 12, Top = 138, Width = 200,
            Font  = new Font("Segoe UI", 10f),
        };

        _lblError = new Label
        {
            Text      = "",
            ForeColor = Color.Red,
            Left      = 12, Top = 165, Width = 340,
            Height    = 18,
            Font      = new Font("Segoe UI", 8.5f),
        };

        _btnOk = new Button
        {
            Text         = "ОК",
            DialogResult = DialogResult.OK,
            Left         = 160, Top = 186,
            Width        = 90, Height = 30,
            FlatStyle    = FlatStyle.Flat,
            BackColor    = Color.FromArgb(0, 123, 255),
            ForeColor    = Color.White,
            Font         = new Font("Segoe UI", 9f),
        };
        _btnOk.Click += BtnOk_Click;

        _btnCancel = new Button
        {
            Text         = "Отмена",
            DialogResult = DialogResult.Cancel,
            Left         = 262, Top = 186,
            Width        = 90, Height = 30,
            FlatStyle    = FlatStyle.Flat,
            BackColor    = Color.FromArgb(108, 117, 125),
            ForeColor    = Color.White,
            Font         = new Font("Segoe UI", 9f),
        };

        AcceptButton = _btnOk;
        CancelButton = _btnCancel;

        Controls.AddRange(new Control[]
        {
            lblName, _txtName, lblZoo, _cmbZoo,
            lblWeight, _txtWeight, _lblError, _btnOk, _btnCancel,
        });
    }

    private void BtnOk_Click(object? sender, EventArgs e)
    {
        _lblError.Text = "";

        if (string.IsNullOrWhiteSpace(_txtName.Text))
        {
            _lblError.Text = "Название животного не может быть пустым.";
            DialogResult = DialogResult.None;
            return;
        }

        if (_cmbZoo.SelectedItem == null)
        {
            _lblError.Text = "Выберите зоопарк.";
            DialogResult = DialogResult.None;
            return;
        }

        if (!double.TryParse(_txtWeight.Text.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out double w) || w < 0)
        {
            _lblError.Text = "Масса должна быть неотрицательным числом.";
            DialogResult = DialogResult.None;
            return;
        }

        WeightKg = w;
    }

    private static Label MakeLabel(string text, int top)
        => new Label
        {
            Text     = text,
            Left     = 12, Top = top,
            AutoSize = true,
            Font     = new Font("Segoe UI", 9f),
        };
}

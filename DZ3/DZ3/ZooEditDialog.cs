namespace ZooApp.Forms;

/// <summary>
/// Диалог добавления / редактирования зоопарка.
/// </summary>
public class ZooEditDialog : Form
{
    private TextBox _txtName  = null!;
    private Button  _btnOk    = null!;
    private Button  _btnCancel = null!;

    /// <summary>Название зоопарка, введённое пользователем.</summary>
    public string ZooName => _txtName.Text.Trim();

    /// <summary>
    /// Создаёт диалог.
    /// </summary>
    /// <param name="title">Заголовок окна.</param>
    /// <param name="currentName">Текущее название (для редактирования).</param>
    public ZooEditDialog(string title, string currentName)
    {
        Text            = title;
        Size            = new Size(360, 150);
        StartPosition   = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        MinimizeBox     = false;

        var lbl = new Label
        {
            Text     = "Название зоопарка:",
            Left     = 12, Top = 16,
            AutoSize = true,
            Font     = new Font("Segoe UI", 10f),
        };

        _txtName = new TextBox
        {
            Text   = currentName,
            Left   = 12, Top = 38,
            Width  = 320,
            Font   = new Font("Segoe UI", 10f),
        };

        _btnOk = new Button
        {
            Text         = "ОК",
            DialogResult = DialogResult.OK,
            Left         = 140, Top = 72,
            Width        = 90, Height = 30,
            Font         = new Font("Segoe UI", 9f),
            FlatStyle    = FlatStyle.Flat,
            BackColor    = Color.FromArgb(0, 123, 255),
            ForeColor    = Color.White,
        };
        _btnOk.Click += BtnOk_Click;

        _btnCancel = new Button
        {
            Text         = "Отмена",
            DialogResult = DialogResult.Cancel,
            Left         = 242, Top = 72,
            Width        = 90, Height = 30,
            Font         = new Font("Segoe UI", 9f),
            FlatStyle    = FlatStyle.Flat,
            BackColor    = Color.FromArgb(108, 117, 125),
            ForeColor    = Color.White,
        };

        AcceptButton = _btnOk;
        CancelButton = _btnCancel;

        Controls.AddRange(new Control[] { lbl, _txtName, _btnOk, _btnCancel });
    }

    private void BtnOk_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_txtName.Text))
        {
            MessageBox.Show("Название не может быть пустым.", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            DialogResult = DialogResult.None;
        }
    }
}

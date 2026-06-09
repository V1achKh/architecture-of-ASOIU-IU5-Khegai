using System.Text;

/// <summary>
/// Построитель отчётов с использованием паттерна Fluent Interface.
/// Промежуточные методы возвращают this (цепочка вызовов).
/// Терминальные методы выполняют запрос и форматируют результат.
///
/// Зависит от DatabaseManager (жёсткая связь через конструктор).
/// </summary>
class ReportBuilder
{
    private DatabaseManager _db;

    private string   _sql     = "";
    private string   _title   = "";
    private string[] _headers = Array.Empty<string>();
    private int[]    _widths  = Array.Empty<int>();

    /// <summary>
    /// Конструктор. Принимает DatabaseManager для выполнения запросов.
    /// </summary>
    public ReportBuilder(DatabaseManager db)
    {
        _db = db;
    }


    /// <summary>SQL-запрос для отчёта</summary>
    public ReportBuilder Query(string sql)
    {
        _sql = sql;
        return this;
    }

    /// <summary>Заголовок отчёта</summary>
    public ReportBuilder Title(string title)
    {
        _title = title;
        return this;
    }

    /// <summary>Названия колонок для отображения</summary>
    public ReportBuilder Header(params string[] columns)
    {
        _headers = columns;
        return this;
    }

    /// <summary>Ширина каждой колонки в символах</summary>
    public ReportBuilder ColumnWidths(params int[] widths)
    {
        _widths = widths;
        return this;
    }

    // ──────────── Терминальные методы ────────────

    /// <summary>
    /// Выполняет запрос, форматирует результат через StringBuilder,
    /// возвращает готовую строку отчёта
    /// </summary>
    public string Build()
    {
        var (columns, rows) = _db.ExecuteQuery(_sql);
        var sb = new StringBuilder();

        if (_title.Length > 0)
        {
            sb.AppendLine();
            sb.AppendLine($"=== {_title} ===");
        }

        string[] displayHeaders = _headers.Length > 0 ? _headers : columns;
        int colCount = displayHeaders.Length;

        int[] widths;
        if (_widths.Length >= colCount)
        {
            widths = _widths;
        }
        else
        {
            widths = new int[colCount];
            for (int i = 0; i < colCount; i++)
                widths[i] = 20;
        }

        for (int i = 0; i < colCount; i++)
            sb.Append(displayHeaders[i].PadRight(widths[i]));
        sb.AppendLine();

        int totalWidth = 0;
        for (int i = 0; i < colCount; i++)
            totalWidth += widths[i];
        sb.AppendLine(new string('─', totalWidth));

        foreach (string[] row in rows)
        {
            for (int c = 0; c < row.Length && c < colCount; c++)
                sb.Append(row[c].PadRight(widths[c]));
            sb.AppendLine();
        }

        return sb.ToString();
    }

    /// <summary>Выполняет Build() и выводит результат в консоль</summary>
    public void Print()
    {
        Console.Write(Build());
    }
}

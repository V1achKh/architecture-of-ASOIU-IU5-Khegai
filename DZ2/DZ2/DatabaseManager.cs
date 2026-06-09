using Microsoft.Data.Sqlite;

/// <summary>
/// Управление базой данных SQLite.
/// Инкапсулирует все операции с БД: создание таблиц,
/// импорт CSV, CRUD-операции, выполнение запросов для отчётов.
/// </summary>
class DatabaseManager
{
    private string _connectionString;

    /// <summary>
    /// Конструктор. Принимает путь к файлу БД.
    /// </summary>
    public DatabaseManager(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
    }

    // ──────────── Инициализация ────────────

    /// <summary>
    /// Создаёт таблицы (если не существуют) и загружает CSV при первом запуске
    /// </summary>
    public void InitializeDatabase(string zooCsvPath, string animalCsvPath)
    {
        CreateTables();

        if (GetAllZoos().Count == 0 && File.Exists(zooCsvPath))
        {
            ImportZoosFromCsv(zooCsvPath);
            Console.WriteLine($"[OK] Загружены зоопарки из {zooCsvPath}");
        }

        if (GetAllAnimals().Count == 0 && File.Exists(animalCsvPath))
        {
            ImportAnimalsFromCsv(animalCsvPath);
            Console.WriteLine($"[OK] Загружены животные из {animalCsvPath}");
        }
    }

    /// <summary>Создание таблиц</summary>
    private void CreateTables()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS zoo (
                zoo_id   INTEGER PRIMARY KEY AUTOINCREMENT,
                zoo_name TEXT    NOT NULL
            );
            CREATE TABLE IF NOT EXISTS animal (
                animal_id  INTEGER PRIMARY KEY AUTOINCREMENT,
                zoo_id     INTEGER NOT NULL,
                animal_name TEXT   NOT NULL,
                weight_kg  REAL    NOT NULL,
                FOREIGN KEY (zoo_id) REFERENCES zoo(zoo_id)
            );";
        cmd.ExecuteNonQuery();
    }

    /// <summary>Импорт зоопарков из CSV</summary>
    private void ImportZoosFromCsv(string path)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        string[] lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(';');
            if (parts.Length < 2) continue;
            var cmd = conn.CreateCommand();
            cmd.CommandText =
                "INSERT INTO zoo (zoo_id, zoo_name) VALUES (@id, @name)";
            cmd.Parameters.AddWithValue("@id",   int.Parse(parts[0]));
            cmd.Parameters.AddWithValue("@name", parts[1]);
            cmd.ExecuteNonQuery();
        }
    }

    /// <summary>Импорт животных из CSV</summary>
    private void ImportAnimalsFromCsv(string path)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        string[] lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(';');
            if (parts.Length < 4) continue;
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO animal (animal_id, zoo_id, animal_name, weight_kg)
                VALUES (@id, @zooId, @name, @weight)";
            cmd.Parameters.AddWithValue("@id",     int.Parse(parts[0]));
            cmd.Parameters.AddWithValue("@zooId",  int.Parse(parts[1]));
            cmd.Parameters.AddWithValue("@name",   parts[2]);
            cmd.Parameters.AddWithValue("@weight", double.Parse(parts[3]));
            cmd.ExecuteNonQuery();
        }
    }

    // ──────────── Чтение данных ────────────

    /// <summary>Получить все зоопарки</summary>
    public List<Zoo> GetAllZoos()
    {
        var result = new List<Zoo>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT zoo_id, zoo_name FROM zoo ORDER BY zoo_id";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Zoo(
                reader.GetInt32(0),
                reader.GetString(1)));
        }
        return result;
    }

    /// <summary>Получить всех животных</summary>
    public List<Animal> GetAllAnimals()
    {
        var result = new List<Animal>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText =
            "SELECT animal_id, zoo_id, animal_name, weight_kg FROM animal ORDER BY animal_id";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Animal(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetDouble(3)));
        }
        return result;
    }

    /// <summary>Получить животное по Id</summary>
    public Animal GetAnimalById(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText =
            "SELECT animal_id, zoo_id, animal_name, weight_kg FROM animal WHERE animal_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Animal(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetDouble(3));
        }
        return null;
    }

    // ──────────── Изменение данных ────────────

    /// <summary>Добавить животное (Id генерируется автоматически)</summary>
    public void AddAnimal(Animal animal)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO animal (zoo_id, animal_name, weight_kg)
            VALUES (@zooId, @name, @weight)";
        cmd.Parameters.AddWithValue("@zooId",  animal.ZooId);
        cmd.Parameters.AddWithValue("@name",   animal.Name);
        cmd.Parameters.AddWithValue("@weight", animal.WeightKg);
        cmd.ExecuteNonQuery();
    }

    /// <summary>Обновить данные животного</summary>
    public void UpdateAnimal(Animal animal)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE animal
            SET zoo_id = @zooId, animal_name = @name, weight_kg = @weight
            WHERE animal_id = @id";
        cmd.Parameters.AddWithValue("@id",     animal.Id);
        cmd.Parameters.AddWithValue("@zooId",  animal.ZooId);
        cmd.Parameters.AddWithValue("@name",   animal.Name);
        cmd.Parameters.AddWithValue("@weight", animal.WeightKg);
        cmd.ExecuteNonQuery();
    }

    /// <summary>Удалить животное по Id</summary>
    public void DeleteAnimal(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM animal WHERE animal_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }


    /// <summary>
    /// Выполняет SQL-запрос и возвращает имена столбцов и строки результата.
    /// Используется классом ReportBuilder.
    /// </summary>
    public (string[] columns, List<string[]> rows) ExecuteQuery(string sql)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        using var reader = cmd.ExecuteReader();

        string[] columns = new string[reader.FieldCount];
        for (int i = 0; i < reader.FieldCount; i++)
            columns[i] = reader.GetName(i);

        var rows = new List<string[]>();
        while (reader.Read())
        {
            string[] row = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
                row[i] = reader.GetValue(i)?.ToString() ?? "";
            rows.Add(row);
        }

        return (columns, rows);
    }

    // Фильтр по зоопарку 

    /// <summary>Получить животных конкретного зоопарка</summary>
    public List<Animal> GetAnimalsByZoo(int zooId)
    {
        var result = new List<Animal>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT animal_id, zoo_id, animal_name, weight_kg
            FROM animal WHERE zoo_id = @zooId ORDER BY animal_name";
        cmd.Parameters.AddWithValue("@zooId", zooId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Animal(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetDouble(3)));
        }
        return result;
    }
}

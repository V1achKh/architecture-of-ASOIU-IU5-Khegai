using System.Text;

// ══════════════════════════════════════════════════════════
// Точка входа — консольное меню
// ══════════════════════════════════════════════════════════

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding  = Encoding.UTF8;

string dbPath     = "zoo.db";
string zooCsv     = Path.Combine(AppContext.BaseDirectory, "zoo.csv");
string animalCsv  = Path.Combine(AppContext.BaseDirectory, "animal.csv");

var db = new DatabaseManager(dbPath);
db.InitializeDatabase(zooCsv, animalCsv);

Console.WriteLine();

string choice;
do
{
    Console.WriteLine("╔══════════════════════════════════════╗");
    Console.WriteLine("║      УПРАВЛЕНИЕ ЗООПАРКАМИ           ║");
    Console.WriteLine("╠══════════════════════════════════════╣");
    Console.WriteLine("║ 1 — Показать все зоопарки            ║");
    Console.WriteLine("║ 2 — Показать всех животных           ║");
    Console.WriteLine("║ 3 — Добавить животное                ║");
    Console.WriteLine("║ 4 — Редактировать животное           ║");
    Console.WriteLine("║ 5 — Удалить животное                 ║");
    Console.WriteLine("║ 6 — Отчёты                           ║");
    Console.WriteLine("║ 7 — Фильтр по зоопарку [ГРУППА Г]   ║");
    Console.WriteLine("║ 0 — Выход                            ║");
    Console.WriteLine("╚══════════════════════════════════════╝");
    Console.Write("Ваш выбор: ");
    choice = Console.ReadLine()?.Trim() ?? "";
    Console.WriteLine();

    switch (choice)
    {
        case "1": ShowZoos(db);              break;
        case "2": ShowAnimals(db);           break;
        case "3": AddAnimal(db);             break;
        case "4": EditAnimal(db);            break;
        case "5": DeleteAnimal(db);          break;
        case "6": ReportsMenu(db);           break;
        case "7": FilterByZoo(db);           break; // [ГРУППА Г]
        case "0": Console.WriteLine("До свидания!"); break;
        default:  Console.WriteLine("Неверный пункт меню."); break;
    }

    Console.WriteLine();
}
while (choice != "0");

// ══════════════════════════════════════════════════════════
// Функции пунктов меню
// ══════════════════════════════════════════════════════════

static void ShowZoos(DatabaseManager db)
{
    Console.WriteLine("--- Все зоопарки ---");
    var zoos = db.GetAllZoos();
    foreach (var zoo in zoos)
        Console.WriteLine("  " + zoo);
    Console.WriteLine($"Итого: {zoos.Count}");
}

static void ShowAnimals(DatabaseManager db)
{
    Console.WriteLine("--- Все животные ---");
    var animals = db.GetAllAnimals();
    foreach (var animal in animals)
        Console.WriteLine("  " + animal);
    Console.WriteLine($"Итого: {animals.Count}");
}

static void AddAnimal(DatabaseManager db)
{
    Console.WriteLine("--- Добавление животного ---");

    Console.WriteLine("Доступные зоопарки:");
    var zoos = db.GetAllZoos();
    foreach (var zoo in zoos)
        Console.WriteLine("  " + zoo);

    Console.Write("ID зоопарка: ");
    if (!int.TryParse(Console.ReadLine(), out int zooId))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }

    Console.Write("Название животного: ");
    string name = Console.ReadLine()?.Trim() ?? "";
    if (name.Length == 0)
    {
        Console.WriteLine("Ошибка: название не может быть пустым.");
        return;
    }

    Console.Write("Масса (кг): ");
    if (!double.TryParse(Console.ReadLine(), out double weight))
    {
        Console.WriteLine("Ошибка: введите число.");
        return;
    }

    try
    {
        var animal = new Animal(0, zooId, name, weight);
        db.AddAnimal(animal);
        Console.WriteLine("Животное добавлено.");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }
}

static void EditAnimal(DatabaseManager db)
{
    Console.WriteLine("--- Редактирование животного ---");
    Console.Write("Введите ID животного: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }

    var animal = db.GetAnimalById(id);
    if (animal == null)
    {
        Console.WriteLine($"Животное с ID={id} не найдено.");
        return;
    }

    Console.WriteLine($"Текущие данные: {animal}");
    Console.WriteLine("(нажмите Enter, чтобы оставить значение без изменений)");

    Console.Write($"Название [{animal.Name}]: ");
    string input = Console.ReadLine()?.Trim() ?? "";
    if (input.Length > 0)
        animal.Name = input;

    Console.Write($"ID зоопарка [{animal.ZooId}]: ");
    input = Console.ReadLine()?.Trim() ?? "";
    if (input.Length > 0 && int.TryParse(input, out int newZooId))
        animal.ZooId = newZooId;

    Console.Write($"Масса кг [{animal.WeightKg}]: ");
    input = Console.ReadLine()?.Trim() ?? "";
    if (input.Length > 0 && double.TryParse(input, out double newWeight))
    {
        try
        {
            animal.WeightKg = newWeight; // валидация в set-аксессоре
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            return;
        }
    }

    db.UpdateAnimal(animal);
    Console.WriteLine("Данные обновлены.");
}

static void DeleteAnimal(DatabaseManager db)
{
    Console.WriteLine("--- Удаление животного ---");
    Console.Write("Введите ID животного: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }

    var animal = db.GetAnimalById(id);
    if (animal == null)
    {
        Console.WriteLine($"Животное с ID={id} не найдено.");
        return;
    }

    Console.Write($"Удалить «{animal.Name}»? (да/нет): ");
    string confirm = Console.ReadLine()?.Trim().ToLower() ?? "";
    if (confirm == "да")
    {
        db.DeleteAnimal(id);
        Console.WriteLine("Животное удалено.");
    }
    else
    {
        Console.WriteLine("Удаление отменено.");
    }
}

// ══════════════════════════════════════════════════════════
// Подменю отчётов
// ══════════════════════════════════════════════════════════

static void ReportsMenu(DatabaseManager db)
{
    string choice;
    do
    {
        Console.WriteLine("--- Отчёты ---");
        Console.WriteLine("  1 — Животные с названиями зоопарков");
        Console.WriteLine("  2 — Количество животных в зоопарках");
        Console.WriteLine("  3 — Средняя масса животных по зоопаркам");
        Console.WriteLine("  0 — Назад");
        Console.Write("Ваш выбор: ");
        choice = Console.ReadLine()?.Trim() ?? "";

        switch (choice)
        {
            case "1": Report1_AnimalsWithZoos(db);        break;
            case "2": Report2_CountByZoo(db);             break;
            case "3": Report3_AvgWeightByZoo(db);         break;
            case "0": break;
            default:  Console.WriteLine("Неверный пункт."); break;
        }

        Console.WriteLine();
    }
    while (choice != "0");
}

// ─────── Отчёт 1: Животные с названиями зоопарков (JOIN) ───────
static void Report1_AnimalsWithZoos(DatabaseManager db)
{
    new ReportBuilder(db)
        .Query(@"SELECT a.animal_name, z.zoo_name, a.weight_kg
                   FROM animal a
                   JOIN zoo z ON a.zoo_id = z.zoo_id
                  ORDER BY a.animal_name")
        .Title("Животные по зоопаркам")
        .Header("Животное", "Зоопарк", "Масса (кг)")
        .ColumnWidths(25, 28, 12)
        .Print();
}

// ─────── Отчёт 2: Количество животных по зоопаркам (GROUP BY + COUNT) ───────
static void Report2_CountByZoo(DatabaseManager db)
{
    new ReportBuilder(db)
        .Query(@"SELECT z.zoo_name, COUNT(*) AS cnt
                   FROM animal a
                   JOIN zoo z ON a.zoo_id = z.zoo_id
                  GROUP BY z.zoo_name
                  ORDER BY z.zoo_name")
        .Title("Количество животных по зоопаркам")
        .Header("Зоопарк", "Кол-во")
        .ColumnWidths(28, 10)
        .Print();
}

// ─────── Отчёт 3: Средняя масса по зоопаркам (GROUP BY + AVG) ───────
static void Report3_AvgWeightByZoo(DatabaseManager db)
{
    new ReportBuilder(db)
        .Query(@"SELECT z.zoo_name, ROUND(AVG(a.weight_kg), 1) AS avg_weight
                   FROM animal a
                   JOIN zoo z ON a.zoo_id = z.zoo_id
                  GROUP BY z.zoo_name
                  ORDER BY avg_weight DESC")
        .Title("Средняя масса животных по зоопаркам")
        .Header("Зоопарк", "Средняя масса (кг)")
        .ColumnWidths(28, 20)
        .Print();
}

// ══════════════════════════════════════════════════════════
// [ГРУППА Г] Фильтр по зоопарку
// ══════════════════════════════════════════════════════════

static void FilterByZoo(DatabaseManager db)
{
    Console.WriteLine("--- Фильтр по зоопарку ---");
    Console.WriteLine("Доступные зоопарки:");
    var zoos = db.GetAllZoos();
    foreach (var zoo in zoos)
        Console.WriteLine("  " + zoo);

    Console.Write("Введите ID зоопарка: ");
    if (!int.TryParse(Console.ReadLine(), out int zooId))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }

    var animals = db.GetAnimalsByZoo(zooId);
    if (animals.Count == 0)
    {
        Console.WriteLine("В этом зоопарке нет животных.");
        return;
    }

    Console.WriteLine($"\nЖивотные зоопарка #{zooId}:");
    foreach (var animal in animals)
        Console.WriteLine("  " + animal);
    Console.WriteLine($"Итого: {animals.Count}");
}

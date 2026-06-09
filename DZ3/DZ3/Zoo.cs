namespace ZooApp.Models;

/// <summary>
/// Зоопарк (справочная таблица, сторона «один»)
/// </summary>
public class Zoo
{
    /// <summary>Идентификатор зоопарка (первичный ключ)</summary>
    public int Id { get; set; }

    /// <summary>Название зоопарка</summary>
    public string Name { get; set; } = "";

    /// <summary>Навигационное свойство: животные этого зоопарка</summary>
    public ICollection<Animal> Animals { get; set; } = new List<Animal>();
}

namespace ZooApp.Models;

/// <summary>
/// Животное (основная таблица, сторона «много»)
/// </summary>
public class Animal
{
    /// <summary>Идентификатор животного (первичный ключ)</summary>
    public int Id { get; set; }

    /// <summary>Идентификатор зоопарка (внешний ключ)</summary>
    public int ZooId { get; set; }

    /// <summary>Зоопарк (навигационное свойство)</summary>
    public Zoo? Zoo { get; set; }

    /// <summary>Название животного</summary>
    public string Name { get; set; } = "";

    /// <summary>Масса животного в кг (не может быть отрицательной)</summary>
    public double WeightKg { get; set; }
}

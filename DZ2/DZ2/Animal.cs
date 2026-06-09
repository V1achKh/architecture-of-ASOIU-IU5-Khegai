/// <summary>
/// Животное (основная таблица, сторона «много»)
/// </summary>
class Animal
{
    /// <summary>Идентификатор животного</summary>
    public int Id { get; set; }

    /// <summary>Идентификатор зоопарка (внешний ключ)</summary>
    public int ZooId { get; set; }

    /// <summary>Название животного</summary>
    public string Name { get; set; }

    private double _weightKg;

    /// <summary>
    /// Масса животного в кг (не может быть отрицательной)
    /// </summary>
    public double WeightKg
    {
        get => _weightKg;
        set
        {
            if (value < 0)
                throw new ArgumentException(
                    "Масса животного не может быть отрицательной");
            _weightKg = value;
        }
    }

    /// <summary>Конструктор с параметрами</summary>
    public Animal(int id, int zooId, string name, double weightKg)
    {
        Id = id;
        ZooId = zooId;
        Name = name;
        WeightKg = weightKg; // валидация сработает здесь
    }

    /// <summary>Конструктор по умолчанию</summary>
    public Animal() : this(0, 0, "", 0) { }

    public override string ToString()
        => $"[{Id}] {Name}, зоопарк #{ZooId}, масса: {WeightKg} кг";
}

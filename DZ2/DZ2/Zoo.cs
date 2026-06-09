/// <summary>
/// Зоопарк (справочная таблица, сторона «один»)
/// </summary>
class Zoo
{
    /// <summary>Идентификатор зоопарка</summary>
    public int Id { get; set; }

    /// <summary>Название зоопарка</summary>
    public string Name { get; set; }

    /// <summary>Конструктор с параметрами</summary>
    public Zoo(int id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>Конструктор по умолчанию</summary>
    public Zoo() : this(0, "") { }

    public override string ToString() => $"[{Id}] {Name}";
}

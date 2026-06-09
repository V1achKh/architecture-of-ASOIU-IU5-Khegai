using DZ3.Models;
using Microsoft.EntityFrameworkCore;

namespace DZ3.Data;

/// <summary>
/// Контекст базы данных приложения.
/// Содержит DbSet для зоопарков и животных,
/// обеспечивает создание БД и начальное заполнение данными.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>Набор зоопарков</summary>
    public DbSet<Zoo> Zoos { get; set; }

    /// <summary>Набор животных</summary>
    public DbSet<Animal> Animals { get; set; }

    /// <summary>Настройка подключения к SQLite</summary>
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=zoo.db");

    /// <summary>
    /// Создаёт базу данных при первом запуске и заполняет начальными данными,
    /// если таблицы пусты.
    /// </summary>
    public static void InitializeDatabase()
    {
        using var context = new AppDbContext();
        context.Database.EnsureCreated();

        if (context.Zoos.Any())
            return;

        var zoos = new[]
        {
            new Zoo { Name = "Московский зоопарк" },
            new Zoo { Name = "Ленинградский зоопарк" },
            new Zoo { Name = "Новосибирский зоопарк" },
            new Zoo { Name = "Екатеринбургский зоопарк" },
        };
        context.Zoos.AddRange(zoos);
        context.SaveChanges();

        var animals = new[]
        {
            new Animal { ZooId = zoos[0].Id, Name = "Африканский слон",  WeightKg = 4800 },
            new Animal { ZooId = zoos[0].Id, Name = "Белый медведь",     WeightKg = 450  },
            new Animal { ZooId = zoos[0].Id, Name = "Жираф",             WeightKg = 900  },
            new Animal { ZooId = zoos[0].Id, Name = "Зебра",             WeightKg = 320  },
            new Animal { ZooId = zoos[1].Id, Name = "Амурский тигр",     WeightKg = 180  },
            new Animal { ZooId = zoos[1].Id, Name = "Снежный барс",      WeightKg = 55   },
            new Animal { ZooId = zoos[1].Id, Name = "Северный олень",    WeightKg = 140  },
            new Animal { ZooId = zoos[1].Id, Name = "Горная горилла",    WeightKg = 160  },
            new Animal { ZooId = zoos[2].Id, Name = "Сибирский волк",    WeightKg = 65   },
            new Animal { ZooId = zoos[2].Id, Name = "Рысь",              WeightKg = 22   },
            new Animal { ZooId = zoos[2].Id, Name = "Росомаха",          WeightKg = 18   },
            new Animal { ZooId = zoos[2].Id, Name = "Марал",             WeightKg = 220  },
            new Animal { ZooId = zoos[3].Id, Name = "Бурый медведь",     WeightKg = 280  },
            new Animal { ZooId = zoos[3].Id, Name = "Лось",              WeightKg = 500  },
            new Animal { ZooId = zoos[3].Id, Name = "Кабан",             WeightKg = 120  },
            new Animal { ZooId = zoos[3].Id, Name = "Сокол сапсан",      WeightKg = 1    },
        };
        context.Animals.AddRange(animals);
        context.SaveChanges();
    }
}

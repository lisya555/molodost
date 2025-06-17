List<FurnitureProduct> productsRepo =
[
    new(1, "Ñîâðåìåííûé äèâàí", "Ñîâðåìåííûé", "Äóá", 25000, 1),
    new(2, "Êëàññè÷åñêîå êðåñëî", "Êëàññèêà", "Îðåõ", 18000, 2)
];

List<ProductionWorkshop> workshopsRepo =
[
    new(1, "Öåõ ¹1 (Äåðåâîîáðàáîòêà)"),
    new(2, "Öåõ ¹2 (Ñáîðêà ìåáåëè)")
];

var builder = WebApplication.CreateBuilder();
builder.Services.AddCors();
var app = builder.Build();

app.UseCors(a => a.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

string message = "";

// Ïðîñìîòð ñïèñêà ïðîäóêöèè
app.MapGet("products", (int param = 0) =>
{
    string buffer = message;
    message = "";
    if (param != 0)
        return new { repo = productsRepo.FindAll(x => x.Id == param), message = buffer };
    return new { products = productsRepo, message = buffer };
});

// Äîáàâëåíèå ïðîäóêöèè
app.MapPost("products/create", ([AsParameters] FurnitureProduct dto) =>
    productsRepo.Add(dto));

// Ðåäàêòèðîâàíèå ïðîäóêöèè
app.MapPut("products/update", ([AsParameters] UpdateProductDTO dto) =>
{
    var p = productsRepo.Find(x => x.Id == dto.Id);
    if (p == null) return;

    if (dto.Name != "") p.Name = dto.Name;
    if (dto.Style != "") p.Style = dto.Style;
    if (dto.Material != "") p.Material = dto.Material;
    if (dto.Price != 0) p.Price = dto.Price;
    if (dto.WorkshopId != 0) p.WorkshopId = dto.WorkshopId;
});

// Ïðîñìîòð öåõîâ
app.MapGet("workshops", () => workshopsRepo);

app.Run();

class FurnitureProduct(int id, string name, string style, string material, decimal price, int workshopId)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string Style { get; set; } = style;
    public string Material { get; set; } = material;
    public decimal Price { get; set; } = price;
    public int WorkshopId { get; set; } = workshopId;
}

class ProductionWorkshop(int id, string name)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
}

record class UpdateProductDTO(
    int Id,
    string? Name = "",
    string? Style = "",
    string? Material = "",
    decimal Price = 0,
    int WorkshopId = 0
);










using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

// Контекст базы данных - основной класс для работы с Entity Framework Core
class AppDbContext : DbContext
{
    // DbSet представляет таблицу в базе данных
    public DbSet<FurnitureProduct> Products { get; set; }  // Таблица продуктов мебели
    public DbSet<ProductionWorkshop> Workshops { get; set; } // Таблица производственных цехов

    // Настройка подключения к базе данных SQLite
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=furniture.db"); // Используем файл furniture.db как базу данных
}

// Метод для инициализации базы данных начальными данными
static void InitializeDatabase()
{
    using var db = new AppDbContext();
    // Создаем базу данных, если она еще не существует
    db.Database.EnsureCreated();
    
    // Если таблица цехов пуста, добавляем тестовые данные
    if (!db.Workshops.Any())
    {
        db.Workshops.AddRange(
            new ProductionWorkshop(1, "Цех №1 (Деревообработка)"),
            new ProductionWorkshop(2, "Цех №2 (Сборка мебели)")
        );
        db.SaveChanges(); // Сохраняем изменения в базе данных
    }
    
    // Если таблица продуктов пуста, добавляем тестовые данные
    if (!db.Products.Any())
    {
        db.Products.AddRange(
            new FurnitureProduct(1, "Современный диван", "Современный", "Дуб", 25000, 1),
            new FurnitureProduct(2, "Классическое кресло", "Классика", "Орех", 18000, 2)
        );
        db.SaveChanges(); // Сохраняем изменения в базе данных
    }
}

// Класс, представляющий модель мебельного продукта
class FurnitureProduct
{
    [Key] // Указывает, что это первичный ключ
    public int Id { get; set; }
    
    [Required] // Обязательное поле
    [MaxLength(100)] // Максимальная длина 100 символов
    public string Name { get; set; } // Название продукта
    
    [Required]
    [MaxLength(50)]
    public string Style { get; set; } // Стиль продукта
    
    [Required]
    [MaxLength(50)]
    public string Material { get; set; } // Материал изготовления
    
    [Required]
    [Range(0, double.MaxValue)] // Цена не может быть отрицательной
    public decimal Price { get; set; } // Цена продукта
    
    [Required]
    public int WorkshopId { get; set; } // Внешний ключ для связи с цехом
    
    [ForeignKey("WorkshopId")] // Указывает на свойство, являющееся внешним ключом
    public ProductionWorkshop Workshop { get; set; } // Навигационное свойство к цеху
}

// Класс, представляющий модель производственного цеха
class ProductionWorkshop
{
    [Key] // Первичный ключ
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } // Название цеха
    
    // Навигационное свойство для продуктов, связанных с этим цехом
    public ICollection<FurnitureProduct> Products { get; set; }
}

// Основной код приложения
var builder = WebApplication.CreateBuilder();
// Добавляем сервисы CORS (Cross-Origin Resource Sharing)
builder.Services.AddCors();
// Добавляем контекст базы данных в сервисы
builder.Services.AddDbContext<AppDbContext>();

var app = builder.Build();
// Настраиваем политику CORS
app.UseCors(a => a.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// Инициализация БД при запуске приложения
InitializeDatabase();

// Endpoint для получения списка продуктов
app.MapGet("products", async (AppDbContext db, int param = 0) =>
{
    // Если указан параметр, возвращаем продукт с указанным ID
    if (param != 0)
        return await db.Products
            .Include(p => p.Workshop) // Подгружаем связанные данные о цехе
            .Where(p => p.Id == param)
            .ToListAsync();
    
    // Иначе возвращаем все продукты
    return await db.Products
        .Include(p => p.Workshop) // Подгружаем связанные данные о цехе
        .ToListAsync();
});

// Endpoint для создания нового продукта
app.MapPost("products/create", async (AppDbContext db, [FromBody] FurnitureProduct dto) =>
{
    db.Products.Add(dto); // Добавляем новый продукт
    await db.SaveChangesAsync(); // Сохраняем изменения
    return Results.Created($"/products/{dto.Id}", dto); // Возвращаем созданный продукт
});

// Endpoint для обновления существующего продукта
app.MapPut("products/update", async (AppDbContext db, [FromBody] UpdateProductDTO dto) =>
{
    // Ищем продукт по ID
    var product = await db.Products.FindAsync(dto.Id);
    if (product == null) return Results.NotFound(); // Если не найден - 404
    
    // Обновляем только те поля, которые были переданы
    if (!string.IsNullOrEmpty(dto.Name)) product.Name = dto.Name;
    if (!string.IsNullOrEmpty(dto.Style)) product.Style = dto.Style;
    if (!string.IsNullOrEmpty(dto.Material)) product.Material = dto.Material;
    if (dto.Price != 0) product.Price = dto.Price;
    if (dto.WorkshopId != 0) product.WorkshopId = dto.WorkshopId;
    
    await db.SaveChangesAsync(); // Сохраняем изменения
    return Results.Ok(product); // Возвращаем обновленный продукт
});

// Endpoint для получения списка цехов
app.MapGet("workshops", async (AppDbContext db) => 
    await db.Workshops.ToListAsync()); // Возвращаем все цеха

// Запускаем приложение
app.Run();

// DTO (Data Transfer Object) для обновления продукта
record UpdateProductDTO(
    int Id,                 // ID продукта для обновления
    string? Name = null,    // Новое название (необязательное)
    string? Style = null,   // Новый стиль (необязательное)
    string? Material = null,// Новый материал (необязательное)
    decimal Price = 0,      // Новая цена (необязательное)
    int WorkshopId = 0      // Новый ID цеха (необязательное)
);

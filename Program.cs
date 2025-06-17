
List<PartnerRequest> repo =
[
   
    new(1, 1, new DateOnly(2023, 1, 1), "Продукт A", "Модель X", "Ремонт",
        "Иванов Иван", "79991234567", "Новая"),
];


var builder = WebApplication.CreateBuilder();

// Добавление сервисов CORS (Cross-Origin Resource Sharing)
builder.Services.AddCors();


var app = builder.Build();

// Настройка политики CORS (разрешаем все origins, методы и заголовки)
app.UseCors(a => a.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());


string message = "";

//получение заявок
app.MapGet("requests", (int param = 0) =>
{
    
    string buffer = message;
    message = "";

    // Если указан параметр - возвращаем заявки с указанным номером
    if (param != 0)
        return new { repo = repo.FindAll(x => x.Number == param), message = buffer };

    // Иначе возвращаем все заявки
    return new { repo, message = buffer };
});

// Создание новой заявки
app.MapPost("create", ([AsParameters] PartnerRequest dto) =>
    repo.Add(dto)); 

//обновление заявки
app.MapPut("update", ([AsParameters] UpdateRequestDTO dto) =>
{
    // Ищем заявку по номеру
    var r = repo.Find(x => x.Number == dto.Number);
    if (r == null)
        return; 

   
    if (dto.ProblemType != "")
        r.ProblemType = dto.ProblemType;
    if (dto.Master != "")
        r.Master = dto.Master;
    if (dto.Comment != "")
        r.Comments.Add(dto.Comment);
    if (dto.Status != "")
        r.Status = dto.Status;
});

 //удаление заявки
app.MapDelete("delete/{number}", (int number) =>
{
    
    PartnerRequest? r = repo.FirstOrDefault(u => u.Number == number);

    // Если не найдена - возвращаем 404
    if (r == null) return Results.NotFound(new { message = "Заявка не найдена" });

    // Удаляем заявку из списка
    repo.Remove(r);

    // Возвращаем удаленную заявку
    return Results.Json(r);
});

app.Run();


class PartnerRequest(int number, int partnerId, DateOnly requestDate, string device, string model,
                    string problemType, string partnerName, string phone, string status)
{
    public int Number { get; set; } = number;               
    public int PartnerId { get; set; } = partnerId;         
    public DateOnly RequestDate { get; set; } = requestDate;// Дата создания заявки
    public string Device { get; set; } = device;            // Тип устройства
    public string Model { get; set; } = model;             // Модель устройства
    public string ProblemType { get; set; } = problemType;  // Тип проблемы
    public string PartnerName { get; set; } = partnerName;  // Имя партнера
    public string Phone { get; set; } = phone;             // Контактный телефон
    public string Status { get; set; } = status;           // Статус заявки
    public string? Master { get; set; } = "Не назначено";  // Назначенный мастер
    public List<string> Comments { get; set; } = [];       // Комментарии к заявке
    public decimal TotalAmount { get; set; } = 0;          // Общая сумма (пока не используется)
}

// DTO
record class UpdateRequestDTO(
    int Number,                 // Номер заявки (обязательное поле)
    string? Status = "",        // Новый статус (необязательное)
    string? ProblemType = "",   // Новый тип проблемы (необязательное)
    string? Master = "",        // Новый мастер (необязательное)
    string? Comment = ""        // Новый комментарий (необязательное)
);
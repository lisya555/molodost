var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();

public class Hall  // класс зал
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string OpeningHours { get; set; }
}

public class Customer           //класс клиент
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}

public class Karaoke                                       // класс караоке
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string OpeningHours { get; set; }
   
}

public class Reservation
{
    public int Id { get; set; }
    public int CustomerId { get; set; }                         // Внешний ключ для связи с клиентом
    public DateTime ReservationDate { get; set; }
    public int NumberOfPeople { get; set; }

    public virtual Customer Customer { get; set; }
}




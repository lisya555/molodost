var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();

public class Hall  // ����� ���
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string OpeningHours { get; set; }
}

public class Customer           //����� ������
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}

public class Karaoke                                       // ����� �������
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string OpeningHours { get; set; }
   
}

public class Reservation
{
    public int Id { get; set; }
    public int CustomerId { get; set; }                         // ������� ���� ��� ����� � ��������
    public DateTime ReservationDate { get; set; }
    public int NumberOfPeople { get; set; }

    public virtual Customer Customer { get; set; }
}




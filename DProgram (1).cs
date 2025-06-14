var builder = WebApplication.CreateBuilder();
var app = builder.Build();


List<Order> repo =
[
    new(1, 1, 30, 2030, "������", "���������", "���������", "����", "� ��������","��������"),
    new(2, 2, 15, 2030, "���������", "�� ����������", "�� ��������� �� ������", "�������", "� ������", "����"),
];

// ������ ������������
List<Executor> executors = new List<Executor>
{
    new Executor { Id = 1, Name = "���� ������", Email = "ivan@example.com" },
    new Executor { Id = 2, Name = "������ �������", Email = "sergey@example.com" }
};

// ����� � ��������� ��� ������������ ���������
bool isUpdatedStatus = false;
string massage = "";

// �������� endpoint ��� ��������� ���� ������
app.MapGet("/", () =>
{
    if (isUpdatedStatus)
    {
        string buffer = massage;
        isUpdatedStatus = false;
        massage = "";
        return Results.Json(new OrderUpdateStatusDTO(repo, buffer));
    }
    else
        return Results.Json(repo);
});

// Endpoint ��� �������� ����� ������
app.MapPost("/", (Order o) => repo.Add(o));

// Endpoint ��� ���������� ������
app.MapPut("/{number}", (int number, OrderUpdateDTO dto) =>
{
    Order buffer = repo.Find(o => o.Number == number);
    if (buffer == null)
        return Results.NotFound("�������");

    // ��������� �������
    if (buffer.Status != dto.Status)
    {
        buffer.StatusHistory.Add($"{DateTime.Now}: {dto.Status}");
        isUpdatedStatus = true;
        massage += $"������ ������ #{number} ������� �� '{dto.Status}'\n";

        // ����������� � ����������
        if (dto.Status == "���������" && !string.IsNullOrEmpty(buffer.Master))
        {
            massage += $"����������� {buffer.Master} �������� ������ �� ������ #{number}\n";
        }
    }

    buffer.Status = dto.Status;
    buffer.Description = dto.Description;
    buffer.Master = dto.Master;

    return Results.Json(buffer);
});

// Endpoint ��� ���������� �����������
app.MapPost("/assign", (AssignRequest request) =>
{
    var repairRequest = repo.FirstOrDefault(r => r.Number == request.RequestId);
    if (repairRequest == null)
        return Results.NotFound("������ �� �������");

    var executor = executors.FirstOrDefault(e => e.Id == request.ExecutorId);
    if (executor == null)
        return Results.NotFound("����������� �� ������");

    repairRequest.Master = executor.Name;
    repairRequest.Status = "� ������";

    isUpdatedStatus = true;
    massage += $"����������� {executor.Name} �������� �� ������ #{repairRequest.Number}\n";

    return Results.Ok(repairRequest);
});

// Endpoint ��� ���������� �����������
app.MapPost("/add-comment", (AddCommentRequest request) =>
{
    var repairRequest = repo.FirstOrDefault(r => r.Number == request.RequestId);
    if (repairRequest == null)
        return Results.NotFound("������ �� �������");

    if (string.IsNullOrEmpty(repairRequest.Description))
        repairRequest.Description = $"����������� �� {request.Author}: {request.Comment}";
    else
        repairRequest.Description += $"\n����������� �� {request.Author}: {request.Comment}";

    isUpdatedStatus = true;
    massage += $"�������� ����������� � ������ #{repairRequest.Number} �� {request.Author}\n";

    return Results.Ok(repairRequest);
});

// Endpoint ��� ��������� ������ �� ������
app.MapGet("/{num}", (int num) => repo.Find(o => o.Number == num));

// Endpoint ��� ���������� ������ �� ���������
app.MapGet("/filter/{param}", (string param) => repo.FindAll(o =>
o.Device == param ||
o.Problem == param ||
o.Description == param ||
o.Client == param ||
o.Status == param ||
o.Master == param));

// ����������: ���������� ����������� ������
app.MapGet("/stats/completed-count", () =>
{
    var completedCount = repo.Count(o => o.Status == "���������");
    return Results.Json(new { CompletedRequests = completedCount });
});

// ����������: ������� ����� ���������� ������ (� ����)
app.MapGet("/stats/average-time", () =>
{
    var completedRequests = repo.Where(o => o.Status == "���������").ToList();
    if (!completedRequests.Any())
        return Results.Json(new { AverageTime = "��� ����������� ������" });

    var averageTime = completedRequests
        .Select(o =>
        {
            var createdDate = new DateTime(o.Year, o.Month, o.Day);
            var completedDate = DateTime.Parse(o.StatusHistory.Last().Split(':')[0].Trim());
            return (completedDate - createdDate).TotalDays;
        })
        .Average();

    return Results.Json(new { AverageTimeDays = Math.Round(averageTime, 2) });
});

// ���������� �� ����� ��������������
app.MapGet("/stats/problems-stats", () =>
{
    var problemStats = repo
        .GroupBy(o => o.Problem)
        .Select(g => new
        {
            Problem = g.Key,
            Count = g.Count(),
            Percentage = Math.Round((double)g.Count() / repo.Count * 100, 2)
        })
        .OrderByDescending(x => x.Count)
        .ToList();

    return Results.Json(problemStats);
});

// ������ ���������� ������ ������
app.MapGet("/stats/full-stats", () =>
{
    // ���������� ����������� ������
    var completedCount = repo.Count(o => o.Status == "���������");

    // ������� ����� ����������
    var completedRequests = repo.Where(o => o.Status == "���������").ToList();
    var averageTime = completedRequests.Any() ?
        Math.Round(completedRequests
            .Select(o =>
            {
                var createdDate = new DateTime(o.Year, o.Month, o.Day);
                var completedDate = DateTime.Parse(o.StatusHistory.Last().Split(':')[0].Trim());
                return (completedDate - createdDate).TotalDays;
            })
            .Average(), 2) : 0;

    // ���������� �� ���������
    var problemStats = repo
        .GroupBy(o => o.Problem)
        .Select(g => new
        {
            Problem = g.Key,
            Count = g.Count(),
            Percentage = Math.Round((double)g.Count() / repo.Count * 100, 2)
        })
        .OrderByDescending(x => x.Count)
        .ToList();

    // ���������� �� ������������
    var masterStats = repo
        .Where(o => !string.IsNullOrEmpty(o.Master))
        .GroupBy(o => o.Master)
        .Select(g => new
        {
            Master = g.Key,
            CompletedCount = g.Count(o => o.Status == "���������"),
            TotalCount = g.Count(),
            CompletionPercentage = g.Any() ? Math.Round((double)g.Count(o => o.Status == "���������") / g.Count() * 100, 2) : 0
        })
        .OrderByDescending(x => x.CompletedCount)
        .ToList();

    return Results.Json(new
    {
        TotalRequests = repo.Count,
        CompletedRequests = completedCount,
        AverageCompletionTimeDays = averageTime,
        ProblemStatistics = problemStats,
        MasterStatistics = masterStats
    });
});

app.Run();

// ����� ��� ������� ���������� �����������
public class AssignRequest
{
    public int RequestId { get; set; }
    public int ExecutorId { get; set; }
}

// ����� ��� ������� ���������� �����������
public class AddCommentRequest
{
    public int RequestId { get; set; }
    public string Comment { get; set; }
    public string Author { get; set; }
}

// ����� �����������
public class Executor
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

// DTO ��� ���������� ������
class OrderUpdateDTO
{
    string status;
    string description;
    string master;

    public string Status { get => status; set => status = value; }
    public string Description { get => description; set => description = value; }
    public string Master { get => master; set => master = value; }
}

// DTO ��� �������� ������� ����������
record class OrderUpdateStatusDTO(List<Order> repo, string massage);

// ����� ������
class Order
{
    int number;
    int day;
    int month;
    int year;
    string device;
    string problem;
    string description;
    string client;
    string status;
    string master;
    public List<string> StatusHistory { get; set; } = new List<string>();

    public Order(int number, int day, int month, int year, string device, string problem, string description, string client, string status, string master)
    {
        Number = number;
        Day = day;
        Month = month;
        Year = year;
        Device = device;
        Problem = problem;
        Description = description;
        Client = client;
        Status = status;
        Master = master;
        StatusHistory.Add($"{DateTime.Now}: {status}");
    }

    public int Number { get => number; set => number = value; }
    public int Day { get => day; set => day = value; }
    public int Month { get => month; set => month = value; }
    public int Year { get => year; set => year = value; }
    public string Device { get => device; set => device = value; }
    public string Problem { get => problem; set => problem = value; }
    public string Description { get => description; set => description = value; }
    public string Client { get => client; set => client = value; }
    public string Status { get => status; set => status = value; }
    public string Master { get => master; set => master = value; }
}
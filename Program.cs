
List<PartnerRequest> repo =
[
   
    new(1, 1, new DateOnly(2023, 1, 1), "������� A", "������ X", "������",
        "������ ����", "79991234567", "�����"),
];


var builder = WebApplication.CreateBuilder();

// ���������� �������� CORS (Cross-Origin Resource Sharing)
builder.Services.AddCors();


var app = builder.Build();

// ��������� �������� CORS (��������� ��� origins, ������ � ���������)
app.UseCors(a => a.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());


string message = "";

//��������� ������
app.MapGet("requests", (int param = 0) =>
{
    
    string buffer = message;
    message = "";

    // ���� ������ �������� - ���������� ������ � ��������� �������
    if (param != 0)
        return new { repo = repo.FindAll(x => x.Number == param), message = buffer };

    // ����� ���������� ��� ������
    return new { repo, message = buffer };
});

// �������� ����� ������
app.MapPost("create", ([AsParameters] PartnerRequest dto) =>
    repo.Add(dto)); 

//���������� ������
app.MapPut("update", ([AsParameters] UpdateRequestDTO dto) =>
{
    // ���� ������ �� ������
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

 //�������� ������
app.MapDelete("delete/{number}", (int number) =>
{
    
    PartnerRequest? r = repo.FirstOrDefault(u => u.Number == number);

    // ���� �� ������� - ���������� 404
    if (r == null) return Results.NotFound(new { message = "������ �� �������" });

    // ������� ������ �� ������
    repo.Remove(r);

    // ���������� ��������� ������
    return Results.Json(r);
});

app.Run();


class PartnerRequest(int number, int partnerId, DateOnly requestDate, string device, string model,
                    string problemType, string partnerName, string phone, string status)
{
    public int Number { get; set; } = number;               
    public int PartnerId { get; set; } = partnerId;         
    public DateOnly RequestDate { get; set; } = requestDate;// ���� �������� ������
    public string Device { get; set; } = device;            // ��� ����������
    public string Model { get; set; } = model;             // ������ ����������
    public string ProblemType { get; set; } = problemType;  // ��� ��������
    public string PartnerName { get; set; } = partnerName;  // ��� ��������
    public string Phone { get; set; } = phone;             // ���������� �������
    public string Status { get; set; } = status;           // ������ ������
    public string? Master { get; set; } = "�� ���������";  // ����������� ������
    public List<string> Comments { get; set; } = [];       // ����������� � ������
    public decimal TotalAmount { get; set; } = 0;          // ����� ����� (���� �� ������������)
}

// DTO
record class UpdateRequestDTO(
    int Number,                 // ����� ������ (������������ ����)
    string? Status = "",        // ����� ������ (��������������)
    string? ProblemType = "",   // ����� ��� �������� (��������������)
    string? Master = "",        // ����� ������ (��������������)
    string? Comment = ""        // ����� ����������� (��������������)
);
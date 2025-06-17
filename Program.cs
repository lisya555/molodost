List<FurnitureProduct> productsRepo =
[
    new(1, "����������� �����", "�����������", "���", 25000, 1),
    new(2, "������������ ������", "��������", "����", 18000, 2)
];

List<ProductionWorkshop> workshopsRepo =
[
    new(1, "��� �1 (���������������)"),
    new(2, "��� �2 (������ ������)")
];

var builder = WebApplication.CreateBuilder();
builder.Services.AddCors();
var app = builder.Build();

app.UseCors(a => a.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

string message = "";

// �������� ������ ���������
app.MapGet("products", (int param = 0) =>
{
    string buffer = message;
    message = "";
    if (param != 0)
        return new { repo = productsRepo.FindAll(x => x.Id == param), message = buffer };
    return new { products = productsRepo, message = buffer };
});

// ���������� ���������
app.MapPost("products/create", ([AsParameters] FurnitureProduct dto) =>
    productsRepo.Add(dto));

// �������������� ���������
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

// �������� �����
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
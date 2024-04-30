using MOHService.service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IMohApiService, MohApiService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

app.MapControllers();

app.Run();

using Microsoft.EntityFrameworkCore;
using UserServiceApp.Models.UserService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<ILogger>(provider =>
{
    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
    return loggerFactory.CreateLogger("MyLogger");
});


builder.Services.AddDbContext<AppDbContext>( options=>{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
}, ServiceLifetime.Singleton);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

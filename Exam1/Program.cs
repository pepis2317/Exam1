using Exam1.Services;
using Exam1.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
Log.Logger = new LoggerConfiguration().WriteTo.Console().WriteTo.File($"logs/Log-.txt", rollingInterval: RollingInterval.Day, outputTemplate:"{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message:j}{NewLine}{Exception}").CreateLogger(); 
// Add services to the container.
builder.Host.UseSerilog();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configure sql server
builder.Services.AddEntityFrameworkSqlServer();
builder.Services.AddDbContextPool<AccelokaContext>(options =>
{
    var conString = configuration.GetConnectionString("SQLServerDB");
    options.UseSqlServer(conString);
});
builder.Services.AddTransient<TicketService>();
builder.Services.AddTransient<BookedTicketService>();


var app = builder.Build();
app.UseSerilogRequestLogging();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

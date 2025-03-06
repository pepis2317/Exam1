using Exam1.Services;
using Exam1.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;
using FluentValidation;
using FluentValidation.AspNetCore;
using Exam1.Validators;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System.Reflection;
using Exam1.Validators.Booking;
using Exam1.Validators.User;
using Exam1.Models.Booking;
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;



Log.Logger = new LoggerConfiguration().WriteTo.Console().WriteTo.File($"logs/Log-.txt", rollingInterval: RollingInterval.Day, outputTemplate:"{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message:j}{NewLine}{Exception}").CreateLogger(); 
// Add services to the container.
builder.Host.UseSerilog();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDataProtection();
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
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<CartService>();
builder.Services.AddTransient<CategoryService>();


builder.Services.AddValidatorsFromAssembly(typeof(TicketRequestValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(UserRequestModelValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(UserSignInModelValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(UserEditValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(UserDeleteValidator).Assembly);

//ts pmo icl

//for post validation
builder.Services.AddScoped<IValidator<BookingModel>, BookingPostValidator>();
builder.Services.AddScoped<IValidatorForBookingSinglePost, BookingPostValidator>();
builder.Services.AddScoped<IValidator<UserIdBookingList>, BookingListPostValidator>();
builder.Services.AddScoped<IValidatorForBookingListPost,  BookingListPostValidator>();
//builder.Services.AddValidatorsFromAssembly(typeof(BookingListPostValidator).Assembly);

//for delete validation
builder.Services.AddScoped<IValidator<BookingModel>, BookingDeleteValidator>();
builder.Services.AddScoped<IValidatorForBookingSingleDelete, BookingDeleteValidator>();

//for edit validation
builder.Services.AddScoped<IValidator<BookingModel>, BookingEditValidator>();
builder.Services.AddScoped<IValidatorForBookingSingleEdit, BookingEditValidator>();
builder.Services.AddScoped<IValidator<BookingIdBookingListModel>, BookingListEditValidator>();
builder.Services.AddScoped<IValidatorForBookingListEdit, BookingListEditValidator>();


//builder.Services.AddValidatorsFromAssembly(typeof(BookingEditValidator).Assembly);

//for batch deletion
builder.Services.AddScoped<IValidator<BookingIdBookingListModel>, BookingBatchDeleteValidator>();
builder.Services.AddScoped<IValidatorForBookingBatchDelete, BookingBatchDeleteValidator>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));


//builder.Services.AddValidatorsFromAssembly(typeof(BookingListEditValidator).Assembly);
//builder.Services.AddValidatorsFromAssembly(typeof(BookingEditValidator).Assembly);



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors("AllowAll");
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

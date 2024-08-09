using BookingWebApi.Services;
using MassTransit;
using JwtAuthenticationManagement;
using BookingWebApi.Saga;
using static MassTransit.Logging.OperationName;
using CommonModel.Message;
using BookingWebApi.Consumer;
using BookingWebApi.Common.Data;
using Microsoft.EntityFrameworkCore;
using BookingWebApi.Common.Repository;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<dormitorybookingbookingContext>();
builder.Services.AddSingleton<JwtTokenHandle>();
builder.Services.AddMassTransit(busConfiguration =>
{
    busConfiguration.SetKebabCaseEndpointNameFormatter();
    busConfiguration.AddSagaStateMachine<BookingSaga, BookingSagaData>().InMemoryRepository();
    busConfiguration.AddConsumer<RollBackBookingConsumer>();
    busConfiguration.UsingRabbitMq((context, configuration) =>
    {
        configuration.Host(new Uri(builder.Configuration["MessageBroker:host"]!), h =>
        {
            h.Username(builder.Configuration["MessageBroker:username"]);
            h.Password(builder.Configuration["MessageBroker:password"]);
        });


        configuration.ReceiveEndpoint("payment-success", e =>
        {
            e.ConfigureSaga<BookingSagaData>(context);
        });
        configuration.ReceiveEndpoint("create-room-success", e =>
        {
            e.ConfigureSaga<BookingSagaData>(context);
        });

        configuration.ReceiveEndpoint("create-room-faile", e =>
        {
            e.ConfigureSaga<BookingSagaData>(context);
        });

        configuration.ReceiveEndpoint("roll-back-user-success", e =>
        {
            e.ConfigureSaga<BookingSagaData>(context);
        });

        configuration.ConfigureEndpoints(context);
    });
});
#region Add Repository
var connect = builder.Configuration.GetConnectionString("value");

builder.Services.AddDbContext<dormitorybookingbookingContext>(options =>
{
    options.UseMySql(connect, ServerVersion.AutoDetect(connect));
});
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IRepository<Booking>, Repository<Booking>>();
builder.Services.AddScoped<IRepository<Semester>, Repository<Semester>>();
# endregion

builder.Services.AddTransient<IBookingService, BookingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

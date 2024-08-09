using CommonModel.Message;
using JwtAuthenticationManagement;
using MassTransit;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RoomWebApi.Common.Caching;
using RoomWebApi.Common.Data;
using RoomWebApi.Common.Repository;
using RoomWebApi.Consumer;
using RoomWebApi.Servies;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Add RabbitMq 

builder.Services.AddMassTransit(busConfiguration =>
{
    busConfiguration.SetKebabCaseEndpointNameFormatter();


    busConfiguration.AddConsumer<GetPriceOfRoomConsumer>();
    busConfiguration.AddConsumer<GetRoomConsumer>();
    busConfiguration.AddConsumer<CreateRoomConsumer>();
    busConfiguration.AddConsumer<ResetConsumer>();

    busConfiguration.UsingRabbitMq((context, configuration) =>
    {
        configuration.Host(new Uri(builder.Configuration["MessageBroker:host"]!), h =>
        {
            h.Username(builder.Configuration["MessageBroker:username"]);
            h.Password(builder.Configuration["MessageBroker:password"]);

        });
        configuration.ConfigureEndpoints(context);
    });
});

# endregion

#region Add Module Service
    builder.Services.AddScoped<IRoomService, RoomService>();
    builder.Services.AddScoped<ITypeRoomService, TypeRoomService>();
#endregion

#region Add Database
var connect = builder.Configuration.GetConnectionString("value");

builder.Services.AddDbContext<dormitorybookingroomContext>(options =>
{
    options.UseMySql(connect, ServerVersion.AutoDetect(connect));
});
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddDbContext<dormitorybookingroomContext>();
builder.Services.AddScoped<IRepository<Room>, Repository<Room>>();
builder.Services.AddScoped<IRepository<Roomtype>, Repository<Roomtype>>();
builder.Services.AddScoped<IRepository<Building>, Repository<Building>>();
builder.Services.AddScoped<ICachingDbContext, RedisDbContext>();
# endregion

builder.Services.AddTransient<JwtTokenHandle>();
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

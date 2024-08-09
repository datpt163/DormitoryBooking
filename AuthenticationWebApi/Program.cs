using AuthenticationWebApi.Common.Data;
using AuthenticationWebApi.Common.Repository;
using AuthenticationWebApi.Consumer;
using AuthenticationWebApi.Services;
using CommonModel.Message;
using JwtAuthenticationManagement;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMassTransit(busConfiguration =>
{
    busConfiguration.SetKebabCaseEndpointNameFormatter();


    busConfiguration.AddConsumer<PaymentConsumer>();
    busConfiguration.AddConsumer<RollBackConsumer>();
    busConfiguration.AddConsumer<GetCurrentRoomConsumer>();

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
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IAccountService, AccountService>();
#region Add Repository
var connect = builder.Configuration.GetConnectionString("value");

builder.Services.AddDbContext<dormitorybookinguserContext>(options =>
{
    options.UseMySql(connect, ServerVersion.AutoDetect(connect));
});
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IRepository<Role>, Repository<Role>>();
builder.Services.AddScoped<IRepository<User>, Repository<User>>();
# endregion

builder.Services.AddSingleton<JwtTokenHandle>();
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

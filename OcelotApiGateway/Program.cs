using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Request.Middleware;
using JwtAuthenticationManagement;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Values;
using System.Text;
var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = new ConfigurationBuilder()
                                    .AddJsonFile("ocelot.json")
                                    .Build();
builder.Services.AddOcelot(configuration);
builder.Services.AddJwtAuthen();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
await app.UseOcelot();
app.Run();

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using eShopLite.API.Data;
using eShopLite.API;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<eShopLiteAPIContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("eShopLiteAPIContext") ?? throw new InvalidOperationException("Connection string 'eShopLiteAPIContext' not found.")));

// Add services to the container.
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapProductEndpoints();

app.CreateDbIfNotExists();

app.Run();

using Microsoft.EntityFrameworkCore;
using Products.Data;
using Products.Endpoints;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddSingleton<RandomFailureMiddleware>();


builder.Services.AddDbContext<ProductDataContext>(options =>
	options.UseInMemoryDatabase("inmemproducts"));

// Add services to the container.
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseMiddleware<RandomFailureMiddleware>();

app.MapProductEndpoints();

app.UseStaticFiles();

app.CreateDbIfNotExists();

app.Run();


public class RandomFailureMiddleware : IMiddleware
{
	private Random _rand;

	public RandomFailureMiddleware()
	{
		_rand = new Random();
	}

	public Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		if (_rand.NextDouble() >= 0.5)
		{
			throw new Exception("Computer says no.");
		}
		return next(context);
	}
}

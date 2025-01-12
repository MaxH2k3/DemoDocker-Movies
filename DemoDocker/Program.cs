using DemoDocker.Configuration;
using DemoDocker.Helper;
using DemoDocker.Models;
using DemoDocker.Repositories;
using DemoDocker.Repositories.UnitOfWork;
using DemoDocker.Services;
using DemoDocker.Services.Authentication;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
/*	.AddOData(opt =>
{
	opt.Filter().Select().Expand().OrderBy().Count().SetMaxTop(100)
	.AddRouteComponents("odata", ConfigOdata.GetEdmModel());
});
*/
builder.Services.AddJWT(builder.Configuration);
builder.Services.AddSwagger();

builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<AuthenHelper>();

// Add PostgresSQL
builder.Services.AddDbContext<MoviesContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("SQL"));
	options.EnableSensitiveDataLogging();
	options.EnableDetailedErrors();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
/*builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("Movie API", new OpenApiInfo { Title = "Movie API", Version = "v1" });
});*/

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
	app.UseSwagger();
	app.UseSwaggerUI();
    app.MapScalarApiReference(option =>
	{
		option.OpenApiRoutePattern = "/swagger/v1/swagger.json";
        option.Title = "Movie API";
		option.WithApiKeyAuthentication(new ApiKeyOptions()
		{
			Token = "okok"
		});

    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseODataBatching();

app.MapControllers();

app.Run();

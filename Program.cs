using System.Text.Json.Serialization;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using RUG.WebEng.Transactions.Models;
using RUG.WebEng.Transactions.Repositories;

Env.TraversePath().NoClobber().LoadMulti([".env", "dotnet.env"]);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
    config.EnableAnnotations();
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("default", builder =>
        builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()
    );
});
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddScoped<TransactionRepository>();

var app = builder.Build();



app.UseCors("default");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
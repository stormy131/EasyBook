using Microsoft.EntityFrameworkCore;
using EasyBook.Models;

var builder = WebApplication.CreateBuilder(args);

// Database context
builder.Services.AddDbContext<EasyBookContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("EasyBookContext")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

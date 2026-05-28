using HL7App.ApiService.Models;
using HL7App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddSqliteDbContext<HL7Context>("hl7appdb");

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

// For this simple app we avoid migrations and ensure the database is created,
// in a production app you would want to use proper migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HL7Context>();
    db.Database.EnsureCreated();
}
// Configure the HTTP request pipeline.
app.UseExceptionHandler();

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/", () => "API service is running. Navigate to /weatherforecast to see sample data.");

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/messages", async (HL7Context dbContext) =>
{
    var messages = await dbContext.Messages.ToListAsync();
    return messages;
});

app.MapGet("/messages/{id}", async (int id, HL7Context dbContext) =>
{
    var message = await dbContext.Messages.FindAsync(id);
    return message is not null ? Results.Ok(message) : Results.NotFound();
});

app.MapPost("/messages", async ([FromBody] String message, HL7Context dbContext) =>
{
    var hl7Message = new HL7Message(message);
    dbContext.Messages.Add(hl7Message);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/messages/{hl7Message.Id}", hl7Message);
});


app.MapPatch("/messages/{id}", async (int id, String message, HL7Context dbContext) =>
{
    var hl7Message = await dbContext.Messages.FindAsync(id);
    if (hl7Message is null)
    {
        return Results.NotFound();
    }
    hl7Message.MessageContent = message;    
    await dbContext.SaveChangesAsync();
    return Results.Ok(hl7Message);
});

app.MapDelete("/messages/{id}", async (int id, HL7Context dbContext) =>
{
    var message = await dbContext.Messages.FindAsync(id);
    if (message == null)
    {
        return Results.NotFound();
    }
    dbContext.Messages.Remove(message);
    await dbContext.SaveChangesAsync();
    return Results.Ok();
});
app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

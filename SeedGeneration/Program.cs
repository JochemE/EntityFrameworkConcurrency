using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using SeedGeneration;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.ResponseStatusCode;
    logging.CombineLogs = true;
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseHttpLogging();

app.MapPost("/init/", async () => await InitHandler.Handle())
    .WithOpenApi();

app.MapPost("/entity/", async ([FromServices]ILoggerFactory logger) => await PostEntityHandler.Handle(logger.CreateLogger("PostEntityHandler")))
    .WithOpenApi();

app.Run();

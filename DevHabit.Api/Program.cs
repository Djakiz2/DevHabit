using DevHabit.Api;
using DevHabit.Api.Extensions;
using FluentValidation;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


builder
    .AddApiServices()
    .AddErrorHandling()
    .AddDatabase()
    .AddObservability()
    .AddApplicationServices();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    await app.ApplyMigrationAsync();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.MapControllers();

await app.RunAsync();

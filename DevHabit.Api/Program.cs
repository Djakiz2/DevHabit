using DevHabit.Api;
using DevHabit.Api.Extensions;
using DevHabit.Api.Middleware;
using DevHabit.Api.Settings;
using FluentValidation;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


builder
    .AddApiServices()
    .AddErrorHandling()
    .AddDatabase()
    .AddObservability()
    .AddApplicationServices()
    .AddAuthenticationServices()
    .AddBackgroundJobs()
    .AddCorsPolicy()
    .AddRateLimiting();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    await app.ApplyMigrationAsync();

    await app.SeedInitialDataAsync();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseCors(CorsOptions.PolicyName);

app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.UseMiddleware<ETagMiddleware>();

app.MapControllers();

await app.RunAsync();

public partial class Program;

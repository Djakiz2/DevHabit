using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevHabit.Api.DTOs.Habits;
using DevHabit.IntegrationTests.Infrastructure;
using DevHabit.Api.Entities;
using System.Net.Http.Json;
using System.Net;
using DevHabit.Api.DTOs.Common;

namespace DevHabit.IntegrationTests.Tests;
public class HabitsTests(DevHabitWebAppFactory factory) : IntegrationTestFixture(factory)
{

    [Fact]
    public async Task GetHabits_ShouldReturnEmptyList_WhenNoHabitsExist()
    {
        await CleanupDatabaseAsync();

        HttpClient client = await CreateAuthenticatedClientAsync();

        HttpResponseMessage response = await client.GetAsync(Routes.Habits.GetAll);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        PaginationResult<HabitDto>? result = await response.Content.ReadFromJsonAsync<PaginationResult<HabitDto>>();
        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetHabits_ShouldReturnHabits_WhenHabitsExist()
    {
        // Arrange 
        await CleanupDatabaseAsync();

        HttpClient client = await CreateAuthenticatedClientAsync();

        CreateHabitDto createDto = TestData.Habits.CreateReadingHabit();
        await client.PostAsJsonAsync(Routes.Habits.Create, createDto);

        HttpResponseMessage response = await client.GetAsync(Routes.Habits.GetAll);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        PaginationResult<HabitDto>? result = await response.Content.ReadFromJsonAsync<PaginationResult<HabitDto>>();
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(createDto.Name, result.Items[0].Name);
    }

    [Fact]
    public async Task GetHabits_ShouldSupportFIltering()
    {
        await CleanupDatabaseAsync();

        HttpClient client = await CreateAuthenticatedClientAsync();

        CreateHabitDto measurableHabit = TestData.Habits.CreateReadingHabit();
        CreateHabitDto binaryHabit = TestData.Habits.CreateExerciseHabit();
        binaryHabit = binaryHabit with { Type = HabitType.Binary };

        await client.PostAsJsonAsync(Routes.Habits.Create, measurableHabit);
        await client.PostAsJsonAsync(Routes.Habits.Create, binaryHabit);

        HttpResponseMessage response = await client.GetAsync($"{Routes.Habits.GetAll}?type={(int)HabitType.Measurable}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        PaginationResult<HabitDto>? result = await response.Content.ReadFromJsonAsync<PaginationResult<HabitDto>>();
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(HabitType.Measurable, result.Items[0].Type);
    }


    [Fact]
    public async Task CreateHabit_ShouldSucceed_WithValidParameters()
    {
        // Arrange
        var dto = new CreateHabitDto
        {
            Name = "Read Books",
            Description = "Read technical books to improve skills",
            Type = HabitType.Measurable,
            Frequency = new FrequencyDto
            {
                Type = FrequencyType.Daily,
                TimesPerPeriod = 1
            },
            Target = new TargetDto
            {
                Value = 30,
                Unit = "pages"
            }
        };

        HttpClient client = await CreateAuthenticatedClientAsync();

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(Routes.Habits.Create, dto);

        // Assert

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.NotNull(await response.Content.ReadFromJsonAsync<HabitDto>());
    }
}

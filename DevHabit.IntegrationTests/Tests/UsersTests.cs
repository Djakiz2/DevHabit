using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using DevHabit.Api.DTOs.Users;
using DevHabit.Api.Entities;
using DevHabit.Api.Services;
using DevHabit.IntegrationTests.Infrastructure;

namespace DevHabit.IntegrationTests.Tests;
public sealed class UsersTests(DevHabitWebAppFactory factory) : IntegrationTestFixture(factory)
{
    [Fact]
    public async Task GetCurrentUser_ShouldReturnUser_WhenAuthenticated()
    {
        // Arrange
        const string email = "test@test.com";
        HttpClient client = await CreateAuthenticatedClientAsync(email);

        // Act
        HttpResponseMessage response = await client.GetAsync(Routes.Users.GetCurrentUser);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        UserDto? user = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
    }

    [Fact]
    public async Task GetCurrentUser_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        HttpClient client = CreateClient();

        // Act
        HttpResponseMessage response = await client.GetAsync(Routes.Users.GetCurrentUser);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetCurrentUser_ShouldIncludeHateoasLinks_WhenRequested()
    {
        HttpClient client = await CreateAuthenticatedClientAsync();
        client.DefaultRequestHeaders.Accept.Add(new(CustomMediaTypeNames.Application.HateoasJson));

        HttpResponseMessage response = await client.GetAsync(Routes.Users.GetCurrentUser);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        UserDto? user = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(user);
        Assert.NotNull(user.Links);
        Assert.NotEmpty(user.Links);

        
    }

    [Fact]
    public async Task GetUserById_ShouldReturnForbidden_WhenNotAdmin()
    {
        HttpClient client = await CreateAuthenticatedClientAsync();

        HttpResponseMessage response = await client.GetAsync(Routes.Users.GetById(User.NewId()));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}

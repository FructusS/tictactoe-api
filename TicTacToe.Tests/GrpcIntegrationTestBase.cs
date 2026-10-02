using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace TicTacToe.Tests;

public abstract class GrpcIntegrationTestBase
{
    protected GrpcChannel channel = null!;
    private WebApplicationFactory<Program> factory = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        factory = new WebApplicationFactory<Program>();
        var handler = factory.Server.CreateHandler();

        channel = GrpcChannel.ForAddress(
            "http://localhost",
            new GrpcChannelOptions
            {
                HttpHandler = handler
            });
    }
    
    [SetUp]
    public async Task SetUp()
    {
        await ResetStateAsync();
    }

    protected virtual Task ResetStateAsync()
    {
        return Task.CompletedTask;
    }
    
    [OneTimeTearDown]
    public void TearDown()
    {
        channel.Dispose();
        factory.Dispose();
    }
}
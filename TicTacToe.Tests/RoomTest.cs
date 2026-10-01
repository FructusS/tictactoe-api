using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using NUnit.Framework;
using TicTacToe.Protos;
using RoomService = TicTacToe.Services.RoomService;

namespace TicTacToe.Tests;

[TestFixture]
public class RoomTest
{
    private GrpcChannel channel = null!;
    private Protos.RoomService.RoomServiceClient client = null!;
    private WebApplicationFactory<Program> factory = null!;
    private Guid currentRoom;

    [OneTimeSetUp]
    public void StartApplication()
    {
        factory = new WebApplicationFactory<Program>();
        var handler = factory.Server.CreateHandler();

        channel = GrpcChannel.ForAddress(
            "http://localhost",
            new GrpcChannelOptions
            {
                HttpHandler = handler
            });

        client = new Protos.RoomService.RoomServiceClient(channel);
    }

    [SetUp]
    public async Task SetUp()
    {
        currentRoom = await CreateRoomAsync();
    }
    
    [Test]
    public async Task CreateRoomTest()
    {
        var request = new CreateRoomRequest();
        var response = await client.CreateRoomAsync(request);
        Assert.NotNull(response);


        Assert.DoesNotThrow(() => { _ = new Guid(response.RoomId.ToByteArray()); });
        var id = new Guid(response.RoomId.ToByteArray());

        Assert.That(id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public async Task JoinRoomOnePlayerTest()
    {
        JoinUserReply? player1 = null;
        
        await Assert.DoesNotThrowAsync(async () => { player1 = await client.JoinRoomAsync(new JoinUserRequest() { RoomId = ByteString.CopyFrom(currentRoom.ToByteArray()) }); });

        Assert.NotNull(player1);

        Assert.DoesNotThrow(() => { _ = new Guid(player1.UserId.ToByteArray()); });
        var id = new Guid(player1.UserId.ToByteArray());

        Assert.That(id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public async Task JoinRoomTwoPlayerTest()
    {
        var player1 = await client.JoinRoomAsync(new JoinUserRequest() { RoomId = ByteString.CopyFrom(currentRoom.ToByteArray()) });
        Assert.NotNull(player1);

        Assert.DoesNotThrow(() => { _ = new Guid(player1.UserId.ToByteArray()); });
        var id = new Guid(player1.UserId.ToByteArray());

        Assert.That(id, Is.Not.EqualTo(Guid.Empty));
        var player2 = await client.JoinRoomAsync(new JoinUserRequest() { RoomId = ByteString.CopyFrom(currentRoom.ToByteArray()) });
        
        Assert.NotNull(player2);

        Assert.DoesNotThrow(() => { _ = new Guid(player2.UserId.ToByteArray()); });
        id = new Guid(player2.UserId.ToByteArray());

        Assert.That(id, Is.Not.EqualTo(Guid.Empty));
    }


    [Test]
    public async Task JoinRoomMoreTwoPlayerTest()
    {
        _ = await client.JoinRoomAsync(new JoinUserRequest() { RoomId = ByteString.CopyFrom(currentRoom.ToByteArray()) });
        _ = await client.JoinRoomAsync(new JoinUserRequest() { RoomId = ByteString.CopyFrom(currentRoom.ToByteArray()) });

        await Assert.ThrowsAsync<RpcException>(async () =>
        {
            var player3 = await client.JoinRoomAsync(new JoinUserRequest() { RoomId = ByteString.CopyFrom(currentRoom.ToByteArray()) });
        });
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        channel.Dispose();
        factory.Dispose();
    }

    public async Task<Guid> CreateRoomAsync()
    {
        var request = new CreateRoomRequest();
        var room = await client.CreateRoomAsync(request);
        return new Guid(room.RoomId.ToByteArray());
    }
}
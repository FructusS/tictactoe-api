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
public class RoomTest : GrpcIntegrationTestBase
{
    private Protos.RoomService.RoomServiceClient client = null!;
    private Guid currentRoom;

    protected override async Task ResetStateAsync()
    {
        currentRoom = await CreateRoomAsync();
    }
    
    [OneTimeSetUp]
    public void StartClient()
    {
        client = new Protos.RoomService.RoomServiceClient(channel);
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

    private async Task<Guid> CreateRoomAsync()
    {
        var request = new CreateRoomRequest();
        var room = await client.CreateRoomAsync(request);
        return new Guid(room.RoomId.ToByteArray());
    }
}
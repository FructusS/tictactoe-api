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
        
        var player1 = await client.JoinRoomAsync(new JoinUserRequest() { RoomId = ByteString.CopyFrom(currentRoom.ToByteArray()) });

        Assert.NotNull(player1);

        var id = new Guid(player1.UserId.ToByteArray());

        Assert.That(id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public async Task JoinRoomTwoPlayerTest()
    {
        var player1 = await client.JoinRoomAsync(new JoinUserRequest() { RoomId = ByteString.CopyFrom(currentRoom.ToByteArray()) });

        var id1 = new Guid(player1.UserId.ToByteArray());

        var player2 = await client.JoinRoomAsync(new JoinUserRequest() { RoomId = ByteString.CopyFrom(currentRoom.ToByteArray()) });
        
        var id2 = new Guid(player2.UserId.ToByteArray());

        Assert.Multiple(() =>
        {
            Assert.That(id1, Is.Not.EqualTo(Guid.Empty));
            Assert.That(id2, Is.Not.EqualTo(Guid.Empty));
            Assert.That(id2, Is.Not.EqualTo(id1));
        });
    }


    [Test]
    public async Task JoinRoomMoreTwoPlayerTest()
    {
        await client.JoinRoomAsync(new JoinUserRequest() { RoomId = ByteString.CopyFrom(currentRoom.ToByteArray()) });
        await client.JoinRoomAsync(new JoinUserRequest() { RoomId = ByteString.CopyFrom(currentRoom.ToByteArray()) });

        var ex = await Assert.ThrowsAsync<RpcException>(async () =>
        { 
            await client.JoinRoomAsync(new JoinUserRequest() { RoomId = ByteString.CopyFrom(currentRoom.ToByteArray()) });
        });
        
        Assert.That(
            ex!.StatusCode,
            Is.EqualTo(StatusCode.FailedPrecondition));
    }

    private async Task<Guid> CreateRoomAsync()
    {
        var request = new CreateRoomRequest();
        var room = await client.CreateRoomAsync(request);
        return new Guid(room.RoomId.ToByteArray());
    }
}
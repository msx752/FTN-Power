using Discord.WebSocket;

namespace FTNPower.Model.Interfaces
{
    public interface IContext
    {
        SocketUser User { get; }

        SocketUserMessage Message { get; }
        SocketGuild Guild { get; }
        ISocketMessageChannel Channel { get; }
    }
}
using System;
using System.Linq;
using CommandSystem;
using CustomSpawnerLobby;

namespace CustomLobbySpawner;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
internal class CustomSpawnerLobby : ICommand
{
    public string Command { get; } = "SpawnList";

    public string[] Aliases { get; } = Array.Empty<string>();

    public string Description { get; } = "SpawnLists";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var classd = EventHandlers.ClassDPlayers.Aggregate("", (current, players) => current + (players.Nickname + " "));
        var scp = EventHandlers.ScpPlayers.Aggregate("", (current, players) => current + (players.Nickname + " "));
        var guard = EventHandlers.GuardPlayers.Aggregate("", (current, players) => current + (players.Nickname + " "));
        var nerd = EventHandlers.ScientistPlayers.Aggregate("", (current, players) => current + (players.Nickname + " "));
        response = $"\n ClassDs: {classd} \n SCPs: {scp} \n Guards: {guard} \n Scientists: {nerd}";
        return true;
    }
}
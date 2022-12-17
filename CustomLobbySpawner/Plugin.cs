global using Instance = CustomSpawnerLobbyPlugin;
using System;
using CustomSpawnerLobby;
using Exiled.API.Features;
using HarmonyLib;
using MapEvent = Exiled.Events.Handlers.Map;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;

public class CustomSpawnerLobbyPlugin : Plugin<Config, Translation>
{
    public static CustomSpawnerLobbyPlugin Instance;

    public static bool Disablebroadcast = false;

    public EventHandlers LobbyEventHandlers;
    public override string Author => "Tiliboyy";
    public override string Name => "CustomLobbySpawner";

    public override string Prefix => "CustomLobbySpawner";
    public override Version Version => new(1, 0, 0);
    public override Version RequiredExiledVersion => new(5, 0, 0, 0);


    public override void OnEnabled()
    {
        try
        {
            Instance = this;
            new Harmony("CustomSpawnerLobby.patches").PatchAll();
            LobbyEventHandlers = new EventHandlers();
            Player.ChangingRole += EventHandlers.OnChangingRole;
            Server.WaitingForPlayers += LobbyEventHandlers.WaitingForPlayers;
            Server.RoundStarted += LobbyEventHandlers.OnRoundStart;
            Player.Verified += EventHandlers.VerifiedPlayer;
            Player.Died += EventHandlers.OnDied;
            Player.Spawned += LobbyEventHandlers.OnSpawned;
            Player.DroppingItem += EventHandlers.OnDrop;
            Player.Dying += EventHandlers.OnDying;
            Player.ThrowingItem += EventHandlers.OnThrow;
            MapEvent.PlacingBlood += EventHandlers.OnPlacingBlood;
            Player.SpawningRagdoll += EventHandlers.RagdollSpawning;
            Log.Info($"CustomLobbySpawner v{Version} by Tiliboyy has been loaded!");
        }
        catch (Exception error)
        {
            Log.Error(error);
        }
    }


    public override void OnDisabled()
    {
        new Harmony("CustomSpawnerLobby.patches").UnpatchAll();
        Player.ChangingRole -= EventHandlers.OnChangingRole;
        Player.Died -= EventHandlers.OnDied;
        Player.Dying -= EventHandlers.OnDying;
        Player.Spawned -= LobbyEventHandlers.OnSpawned;
        Player.DroppingItem -= EventHandlers.OnDrop;
        Player.ThrowingItem -= EventHandlers.OnThrow;
        MapEvent.PlacingBlood -= EventHandlers.OnPlacingBlood;
        Player.Verified -= EventHandlers.VerifiedPlayer;
        Server.WaitingForPlayers -= LobbyEventHandlers.WaitingForPlayers;
        Server.RoundStarted -= LobbyEventHandlers.OnRoundStart;
        LobbyEventHandlers = null;
        Instance = null;
    }
}
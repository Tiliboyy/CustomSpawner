using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.CustomItems.API;
using Exiled.Events.EventArgs;
using GameCore;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using UnityEngine;
using MapEvent = Exiled.Events.Handlers.Map;
using static CustomSpawnerLobby.ColliderTriggers;
using Log = Exiled.API.Features.Log;
using Random = System.Random;

namespace CustomSpawnerLobby;

public class EventHandlers : Plugin<Config>
{
    public static CoroutineHandle LobbyTimer;
    public static CoroutineHandle Respawn;
    public static Vector3 SpawnRotation = Instance.Instance.Config.SpawnRotation;
    public static GameObject Scientistspawner;
    public static GameObject Guardspawner;
    public static GameObject Scpspawner;
    public static GameObject Classdspawner;
    public static List<Player> PrioritizedPlayers = new();
    public static List<Player> ScpPlayers = new();
    public static List<Player> ScientistPlayers = new();
    public static List<Player> GuardPlayers = new();
    public static List<Player> ClassDPlayers = new();
    public static List<Player> playersToSpawnAsScp = new();
    public static List<Player> playersToSpawnAsScientist = new();
    public static List<Player> playersToSpawnAsGuard = new();
    public static List<Player> playersToSpawnAsClassD = new();
    public static Random Random = new();
    public SchematicObject Lobby;


    public void WaitingForPlayers()
    {
        GameObject.Find("StartRound").transform.localScale = Vector3.zero;
        LobbyTimer = Timing.RunCoroutine(UnityMethods.LobbyTimer());
        Respawn = Timing.RunCoroutine(UnityMethods.Respawn());

        int lobbynum;
        if (Instance.Instance.Config.LobbySchematics.Count == 0)
        {
            Log.Warn("No Lobby in config players will spawn at spawn-point");
            lobbynum = 0;
        }
        else
        {
            lobbynum = UnityEngine.Random.Range(0, Instance.Instance.Config.LobbySchematics.Count - 1);
        }

        Lobby = ObjectSpawner.SpawnSchematic(Instance.Instance.Config.LobbySchematics[lobbynum],
            CustomSpawnerLobbyPlugin.Instance.Config.SpawnPoint, Quaternion.identity);

        foreach (Transform child in Lobby.transform)
            switch (child.name.ToLower())
            {
                case "scpspawner":
                    Scpspawner = child.gameObject;
                    break;
                case "classdspawner":
                    Classdspawner = child.gameObject;
                    break;
                case "scientistspawner":
                    Scientistspawner = child.gameObject;
                    break;
                case "guardspawner":
                    Guardspawner = child.gameObject;
                    break;
            }

        if (Guardspawner == null)
        {
            Log.Error(
                $"GuardSpawner not found in {Lobby.name.Replace("CustomSchematic-", "")}. Lobby will not be spawned");
            return;
        }

        if (Scientistspawner == null)
        {
            Log.Error(
                $"ScientistSpawner not found in {Lobby.name.Replace("CustomSchematic-", "")}. Lobby will not be spawned");
            return;
        }

        if (Scpspawner == null)
        {
            Log.Error($"ScpSpawner not found in {Lobby.name.Replace("CustomSchematic-", "")}. Lobby will not spawned");
            return;
        }

        if (Classdspawner == null)
        {
            Log.Error(
                $"ClassdSpawner not found in {Lobby.name.Replace("CustomSchematic-", "")}. Lobby will not be spawned");
            return;
        }

        var gameObject1 = new GameObject("ScpSpawner");
        var collider1 = gameObject1.AddComponent<CapsuleCollider>();
        collider1.isTrigger = true;
        collider1.transform.localScale = Scpspawner.gameObject.transform.localScale;
        gameObject1.AddComponent<ScpSpawner>();
        gameObject1.transform.position = Scpspawner.gameObject.transform.position;

        var gameObject2 = new GameObject("ClassDSpawner");
        var collider2 = gameObject2.AddComponent<CapsuleCollider>();
        collider2.isTrigger = true;
        collider2.transform.localScale = Classdspawner.gameObject.transform.localScale;
        gameObject2.AddComponent<ClassDSpawner>();
        gameObject2.transform.position = Classdspawner.gameObject.transform.position;


        var gameObject3 = new GameObject("ScientistSpawner");
        var collider3 = gameObject3.AddComponent<CapsuleCollider>();
        collider3.isTrigger = true;
        collider3.transform.localScale = Scientistspawner.gameObject.transform.localScale;
        gameObject3.transform.position = Scientistspawner.gameObject.transform.position;
        gameObject3.AddComponent<ScientistSpawner>();


        var gameObject4 = new GameObject("GuardSpawner");
        if (gameObject4 == null) throw new ArgumentNullException(nameof(gameObject4));
        var collider4 = gameObject4.AddComponent<CapsuleCollider>();
        collider4.isTrigger = true;
        collider4.transform.localScale = Guardspawner.gameObject.transform.localScale;
        gameObject4.transform.position = Guardspawner.gameObject.transform.position;
        gameObject4.AddComponent<GuardSpawner>();

        Log.Debug(
            $"Spawned {Lobby.name.Replace("CustomSchematic-", "")} at position: {CustomSpawnerLobbyPlugin.Instance.Config.SpawnPoint.ToString().Replace(",", "")}",
            Instance.Instance.Config.IsDebug);
    }

    public static void OnChangingRole(ChangingRoleEventArgs ev)
    {
        if (ev.Reason == SpawnReason.RoundStart) ev.IsAllowed = false;
    }

    public void OnRoundStart()
    {
        var BulkList = Player.List.ToList();

        var SCPsToSpawn = 0;
        var ClassDsToSpawn = 0;
        var ScientistsToSpawn = 0;
        var GuardsToSpawn = 0;
        if (LobbyTimer.IsRunning)
        {
            Timing.KillCoroutines(Respawn);
            Timing.KillCoroutines(LobbyTimer);
        }

        if (Lobby != null) Lobby.Destroy();
        foreach (var player in Player.List)
        {
            player.ClearInventory();
            if (!player.IsOverwatchEnabled) continue;
            BulkList.Remove(player);
            Log.Debug($"Removed {player.Nickname} from the bulk list because they are overwatching",
                Instance.Instance.Config.IsDebug);
        }

        List<char> SpawnSequence = new();
        foreach (var c in Instance.Instance.Config.SpawnSequence) SpawnSequence.Add(c);
        for (var x = 0; x < Player.List.ToList().Count; x++)
            switch (SpawnSequence[x])
            {
                case '4':
                    ClassDsToSpawn += 1;
                    break;
                case '3':
                    ScientistsToSpawn += 1;
                    break;
                case '1':
                    GuardsToSpawn += 1;
                    break;
                case '0':
                    SCPsToSpawn += 1;
                    break;
            }

        if (ClassDsToSpawn != 0)
        {
            if (ClassDPlayers.Count <= ClassDsToSpawn)
            {
                foreach (var ply in ClassDPlayers)
                {
                    playersToSpawnAsClassD.Add(ply);
                    ClassDsToSpawn -= 1;
                    BulkList.Remove(ply);
                }
            }
            else
            {
                for (var x = 0; x < ClassDsToSpawn; x++)
                {
                    var Ply = ClassDPlayers[Random.Next(ClassDPlayers.Count)];
                    playersToSpawnAsClassD.Add(Ply);
                    ClassDPlayers.Remove(Ply);
                    BulkList.Remove(Ply);
                }

                ClassDsToSpawn = 0;
            }
        }


        if (ScientistsToSpawn != 0)
        {
            if (ScientistPlayers.Count <= ScientistsToSpawn)
            {
                foreach (var ply in ScientistPlayers)
                {
                    playersToSpawnAsScientist.Add(ply);
                    ScientistsToSpawn -= 1;
                    BulkList.Remove(ply);
                }
            }
            else
            {
                for (var x = 0; x < ScientistsToSpawn; x++)
                {
                    var Ply = ScientistPlayers[Random.Next(ScientistPlayers.Count)];
                    playersToSpawnAsScientist.Add(Ply);
                    ScientistPlayers.Remove(Ply);
                    BulkList.Remove(Ply);
                }

                ScientistsToSpawn = 0;
            }
        }


        if (GuardsToSpawn != 0)
        {
            if (GuardPlayers.Count <= GuardsToSpawn)
            {
                foreach (var ply in GuardPlayers)
                {
                    playersToSpawnAsGuard.Add(ply);
                    GuardsToSpawn -= 1;
                    BulkList.Remove(ply);
                }
            }
            else
            {
                for (var x = 0; x < GuardsToSpawn; x++)
                {
                    var Ply = GuardPlayers[Random.Next(GuardPlayers.Count)];
                    playersToSpawnAsGuard.Add(Ply);
                    GuardPlayers.Remove(Ply);
                    BulkList.Remove(Ply);
                }

                GuardsToSpawn = 0;
            }
        }

        if (SCPsToSpawn != 0)
        {
            if (ScpPlayers.Count <= SCPsToSpawn)
            {
                foreach (var ply in ScpPlayers)
                {
                    playersToSpawnAsScp.Add(ply);
                    SCPsToSpawn -= 1;
                    BulkList.Remove(ply);
                }
            }
            else
            {
                for (var x = 0; x < SCPsToSpawn; x++)
                {
                    var Ply = ScpPlayers[Random.Next(ScpPlayers.Count)];
                    ScpPlayers.Remove(Ply);
                    playersToSpawnAsScp.Add(Ply);
                    BulkList.Remove(Ply);
                }

                SCPsToSpawn = 0;
            }
        }

        if (ClassDsToSpawn != 0)
            for (var x = 0; x < ClassDsToSpawn; x++)
            {
                var Ply = BulkList[Random.Next(BulkList.Count)];
                playersToSpawnAsClassD.Add(Ply);
                BulkList.Remove(Ply);
            }

        if (SCPsToSpawn != 0)
            for (var x = 0; x < SCPsToSpawn; x++)
                if (BulkList.Count != 0)
                {
                    var Ply = BulkList[Random.Next(BulkList.Count)];
                    playersToSpawnAsScp.Add(Ply);
                    BulkList.Remove(Ply);
                }

        if (ScientistsToSpawn != 0)
            for (var x = 0; x < ScientistsToSpawn; x++)
            {
                var Ply = BulkList[Random.Next(BulkList.Count)];
                playersToSpawnAsScientist.Add(Ply);
                BulkList.Remove(Ply);
            }

        if (GuardsToSpawn != 0)
            for (var x = 0; x < GuardsToSpawn; x++)
            {
                var Ply = BulkList[Random.Next(BulkList.Count)];
                playersToSpawnAsGuard.Add(Ply);
                BulkList.Remove(Ply);
            }

        Log.Debug(
            $"\n BulkList Count: {BulkList.Count}\n SCP: {SCPsToSpawn}\n ClassD: {ClassDsToSpawn}\n Scientist: {ScientistsToSpawn}\n Guard: {GuardsToSpawn}",
            Instance.Instance.Config.IsDebug);
    }

    public static void VerifiedPlayer(VerifiedEventArgs ev)
    {
        if (IsWaitingForPlayers())
            Timing.CallDelayed(Instance.Instance.Config.SpawnDelay, () =>
            {
                if (!IsWaitingForPlayers()) return;
                if (!Instance.Instance.Config.GlobalVoiceChat)
                    ev.Player.SendFakeSyncVar(RoundStart.singleton.netIdentity, typeof(RoundStart), "NetworkTimer",
                        -1);
                ev.Player.Role.Type =
                    Instance.Instance.Config.RolesToChoose[
                        UnityEngine.Random.Range(0, Instance.Instance.Config.RolesToChoose.Count)];
            });
    }

    public void OnSpawned(SpawnedEventArgs ev)
    {
        if (IsWaitingForPlayers())
        {
            ev.Player.ClearInventory();
            ev.Player.Ammo.Clear();
            ev.Player.Position = CustomSpawnerLobbyPlugin.Instance.Config.SpawnPoint + Vector3.up;
            ev.Player.Rotation = new Vector3(SpawnRotation.x, SpawnRotation.y, SpawnRotation.z);


            Timing.CallDelayed(0.3f, () =>
            {
                ev.Player.ResetInventory(Instance.Instance.Config.LobbyItems);

                foreach (var ammo in Config.Ammo) ev.Player.Ammo[ammo.Key.GetItemType()] = ammo.Value;
            });
        }
    }

    public static void OnDied(DiedEventArgs ev)
    {
        if (IsWaitingForPlayers())
            Timing.CallDelayed(2f, () =>
            {
                if (IsWaitingForPlayers())
                {
                    var player = ev.Target;
                    if (!player.IsOverwatchEnabled)
                        ev.Target.Role.Type =
                            Instance.Instance.Config.RolesToChoose[
                                UnityEngine.Random.Range(0, Instance.Instance.Config.RolesToChoose.Count)];
                }
            });
    }

    public static void RagdollSpawning(SpawningRagdollEventArgs ev)
    {
        if (IsWaitingForPlayers()) ev.IsAllowed = false;
    }

    public static bool IsWaitingForPlayers()
    {
        return !Round.IsStarted && RoundStart.singleton.NetworkTimer is > 1 or -2;
    }

    public static void OnDying(DyingEventArgs ev)
    {
        if (IsWaitingForPlayers()) ev.Target.ClearInventory();
    }

    public static void OnThrow(ThrowingItemEventArgs ev)
    {
        if (Instance.Instance.Config.AllowDroppingItem == true) return;
        if (IsWaitingForPlayers())
            ev.IsAllowed = false;
    }

    public static void OnDrop(DroppingItemEventArgs ev)
    {
        if (Instance.Instance.Config.AllowDroppingItem == true) return;
        if (IsWaitingForPlayers())
            ev.IsAllowed = false;
    }

    public static void OnPlacingBlood(PlacingBloodEventArgs ev)
    {
        if (IsWaitingForPlayers()) ev.IsAllowed = false;
    }
}
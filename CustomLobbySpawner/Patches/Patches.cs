using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CustomSpawnerLobby;
using Exiled.API.Features;
using GameCore;
using HarmonyLib;

namespace CustomLobbySpawner;

[HarmonyPatch(typeof(RoundStart), nameof(RoundStart.NetworkTimer), MethodType.Setter)]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
internal static class NetworkTimerPatch
{
    private static bool Prefix(RoundStart __instance, ref short value)
    {
        if (Round.IsStarted &&
            (RoundStart.singleton.NetworkTimer > 1 || RoundStart.singleton.NetworkTimer == -2)) return true;
        if (Instance.Instance.Config.GlobalVoiceChat) return true;
        __instance.Timer = value;
        return false;
    }
}

[HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.SetRandomRoles))]
internal static class RandomRolePatch
{
    [HarmonyPrefix]
    private static bool SetRandomRoles(CharacterClassManager __instance)
    {
        foreach (var player in EventHandlers.playersToSpawnAsClassD)
        {
            player.ReferenceHub.characterClassManager.SetPlayersClass(RoleType.ClassD, player.GameObject,
                CharacterClassManager.SpawnReason.RoundStart);
            var randomPosition = SpawnpointManager.GetRandomPosition(RoleType.ClassD);
            if (randomPosition != null)
            {
                player.ReferenceHub.playerMovementSync.OnPlayerClassChange(randomPosition.transform.position,
                    new PlayerMovementSync.PlayerRotation(0f, randomPosition.transform.rotation.eulerAngles.y));
            }
        }


        foreach (var player in EventHandlers.playersToSpawnAsScientist)
        {
            player.ReferenceHub.characterClassManager.SetPlayersClass(RoleType.Scientist, player.GameObject,
                CharacterClassManager.SpawnReason.RoundStart);
            var randomPosition = SpawnpointManager.GetRandomPosition(RoleType.Scientist);
            if (randomPosition != null)
            {
                player.ReferenceHub.playerMovementSync.OnPlayerClassChange(randomPosition.transform.position,
                    new PlayerMovementSync.PlayerRotation(0f, randomPosition.transform.rotation.eulerAngles.y));
            }
        }


        foreach (var player in EventHandlers.playersToSpawnAsGuard)
        {
            player.ReferenceHub.characterClassManager.SetPlayersClass(RoleType.FacilityGuard, player.GameObject,
                CharacterClassManager.SpawnReason.RoundStart);
            var randomPosition = SpawnpointManager.GetRandomPosition(RoleType.FacilityGuard);
            if (randomPosition != null)
            {
                player.ReferenceHub.playerMovementSync.OnPlayerClassChange(randomPosition.transform.position,
                    new PlayerMovementSync.PlayerRotation(0f, randomPosition.transform.rotation.eulerAngles.y));
            }
        }

        List<RoleType> Roles = new()
        {
            RoleType.Scp049, RoleType.Scp096, RoleType.Scp106, RoleType.Scp173, RoleType.Scp93953, RoleType.Scp93989
        };
        if (EventHandlers.playersToSpawnAsScp.Count > 2)
            Roles.Add(RoleType.Scp079);
        foreach (Player player in EventHandlers.playersToSpawnAsScp)
        {
            var role = Roles[EventHandlers.Random.Next(Roles.Count)];
            Roles.Remove(role);

            player.ReferenceHub.characterClassManager.SetPlayersClass(role, player.GameObject,
                CharacterClassManager.SpawnReason.RoundStart);
            var randomPosition = SpawnpointManager.GetRandomPosition(role);
            if (randomPosition != null)
            {
                player.ReferenceHub.playerMovementSync.OnPlayerClassChange(randomPosition.transform.position,
                    new PlayerMovementSync.PlayerRotation(0f, randomPosition.transform.rotation.eulerAngles.y));
            }
        }

        return false;
    }
}
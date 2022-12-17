using System;
using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Interfaces;
using UnityEngine;

[Serializable]
public class Config : IConfig
{
    [Description("Enables Debug mode")] public bool IsDebug { get; set; } = false;

    [Description("Lobby Stuff")] public bool DisplayWaitMessage { get; set; } = true;

    [Description("Enables global voice chat in the lobby")]
    public bool GlobalVoiceChat { get; set; } = false;

    [Description("The delay it takes to spawn the player in the lobby")]
    public float SpawnDelay { get; set; } = 0.5f;

    [Description("Determines the position of the Hint on the users screen (32 = Top, 0 = Middle, -15 = Below)")]
    public int HintVertPos { get; set; } = 25;

    public bool UseHints { get; set; } = true;

    [Description("Allows players to drop items in the lobby")]

    public bool AllowDroppingItem { get; set; } = false;

    [Description("A List of Schematics that are randomly chosen from")]

    public List<string> LobbySchematics { get; set;  } = new()
    {
        "Lobby Blau"
    };

    [Description("The items a player gets in the Lobby")]

    public List<string> LobbyItems { get; set; } = new()
    {
        "Coin",
        "Flashlight"
    };

    [Description("The Roles that players can spawn as in the lobby")]

    public List<RoleType> RolesToChoose { get; set; } = new()
    {
        RoleType.ChaosMarauder,
        RoleType.Scientist,
        RoleType.NtfCaptain,
        RoleType.Tutorial,
        RoleType.FacilityGuard,
        RoleType.ClassD
    };

    [Description("List of ammo given to a player while in lobby:")]
    public Dictionary<AmmoType, ushort> Ammo { get; set; } = new()
    {
        { AmmoType.Nato556, 0 },
        { AmmoType.Nato762, 0 },
        { AmmoType.Nato9, 0 },
        { AmmoType.Ammo12Gauge, 0 },
        { AmmoType.Ammo44Cal, 0 }
    };

    [Description("Coordinates of where the lobby spawns")]
    public SerializedVector3.SerializedVector3 SpawnPoint { get; set; } = new Vector3(240.1f, 1000, 95.8f);

    [Description("The Rotation of the Player when they spawn")]
    public SerializedVector3.SerializedVector3 SpawnRotation { get; set; } = new Vector3(0, 0, 0);

    [Description(
        "Coordinates of where the spawners are from the spawnpoint of the lobby (use getvector command to get coordinates)")]

    public SerializedVector3.SerializedVector3 ClassDSpawner { get; set; } = new Vector3(-8.5f, 0, 5f);

    public SerializedVector3.SerializedVector3 GuardSpawner { get; set; } = new Vector3(5f, 0, 15f);

    public SerializedVector3.SerializedVector3 ScientistSpawner { get; set; } = new Vector3(-5f, 0, 15f);

    public SerializedVector3.SerializedVector3 ScpSpawner { get; set; } = new Vector3(8.5f, 0, 5f);

    public string SpawnSequence { get; set; } = "4014314031441404134031441404130441014331";

    [Description("Enables the Plugin")] public bool IsEnabled { get; set; } = true;
}


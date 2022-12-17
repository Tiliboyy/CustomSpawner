using System;
using System.ComponentModel;
using Exiled.API.Interfaces;

namespace CustomSpawnerLobby;
[Serializable]
public class Translation : ITranslation
{
    public string NukeCountdown { get; set; } = "<b><color=#ff0509>%sekunden% Sekunen bis zur Detonation.</color></b>";


    [Description("Lobby")]
    public string TopMessage { get; set; } =
        "<size=40><color=yellow><b>Die Runde startet bald, {seconds}</b></color></size>";

    public string BottomMessage { get; set; } = "<size=30><i>{players}</i></size>";

    public string ServerIsPaused { get; set; } = "Server ist Pausiert";

    public string RoundIsBeingStarted { get; set; } = "Die Runde startet";

    public string OneSecondRemain { get; set; } = "Sekunde";

    public string XSecondsRemains { get; set; } = "Sekunden";

    public string OnePlayerConnected { get; set; } = "Spieler ist verbunden";

    public string XPlayersConnected { get; set; } = "Spieler sind verbunden";


    [Description("Diese Nachrichten werden angezeigt, wenn der Spieler auf den Spawn Pads steht")]
    public string Scpmessage { get; set; } = "Du hast <color=#ff0509>SCP</color> gewählt!";

    public string Classdmessge { get; set; } = "Du hast <color=#ff7d05>Klasse-D</color> gewählt!";
    public string Guardmessage { get; set; } = "Du hast <color=#898889>Sicherheitspersonal</color> gewählt!";
    public string Scientistmessage { get; set; } = "Du hast <color=#ffee00>Wissenschaftler</color> gewählt!";
}
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exiled.API.Features;
using GameCore;
using MEC;
using UnityEngine;

namespace CustomSpawnerLobby;

public static class UnityMethods
{
    public static IEnumerator<float> LobbyTimer()
    {
        StringBuilder message = new();
        var x = 0;

        while (EventHandlers.IsWaitingForPlayers())
        {
            message.Clear();


            message.Append(
                $"<size=40><color=yellow><b>{Instance.Instance.Translation.RoundIsBeingStarted}, %seconds</b></color></size>");

            var networkTimer = RoundStart.singleton.NetworkTimer;

            switch (networkTimer)
            {
                case -2:
                    message.Replace("%seconds", Instance.Instance.Translation.ServerIsPaused);
                    break;

                case -1:
                    message.Replace("%seconds", Instance.Instance.Translation.RoundIsBeingStarted);
                    break;

                case 1:
                    message.Replace("%seconds", $"{networkTimer} {Instance.Instance.Translation.XSecondsRemains}");
                    break;

                case 0:
                    message.Replace("%seconds", Instance.Instance.Translation.RoundIsBeingStarted);
                    break;

                default:
                    message.Replace("%seconds", $"{networkTimer} {Instance.Instance.Translation.XSecondsRemains}");
                    break;
            }

            message.Append("\n<size=30><i>%players</i></size>");

            if (Player.List.Count() == 1)
                message.Replace("%players",
                    $"{Player.List.Count()} {Instance.Instance.Translation.OnePlayerConnected}");
            else
                message.Replace("%players", $"{Player.List.Count()} {Instance.Instance.Translation.XPlayersConnected}");

            for (var i = 0; i < Instance.Instance.Config.HintVertPos; i++) message.Append("\n");
            foreach (var ply in Player.List) ply.ShowHint(message.ToString());
            x++;
            yield return Timing.WaitForSeconds(0.5f);
        }
    }

    public static IEnumerator<float> Respawn()
    {
        for (;;)
        {
            foreach (var ply in Player.List)
                if (ply.IsDead)
                    Timing.CallDelayed(1f, () =>
                    {
                        if (!EventHandlers.IsWaitingForPlayers()) return;
                        if (!ply.IsOverwatchEnabled && ply.IsAlive)
                            ply.Role.Type =
                                Instance.Instance.Config.RolesToChoose[
                                    Random.Range(0, Instance.Instance.Config.RolesToChoose.Count)];
                    });
            yield return Timing.WaitForSeconds(2f);
        }
    }
}

public static class ColliderTriggers
{
    public class ClassDSpawner : MonoBehaviour
    {
        public void OnTriggerEnter(Collider other)
        {
            if (Instance.Disablebroadcast)
                return;

            var player = Player.Get(other.gameObject);
            if (player == null) return;
            player = Player.Get(other.gameObject);
            player.Broadcast(300, Instance.Instance.Translation.Classdmessge, Broadcast.BroadcastFlags.Normal, true);
            if (!EventHandlers.ClassDPlayers.Contains(player))
                EventHandlers.ClassDPlayers.Add(player);
        }


        public void OnTriggerExit(Collider other)
        {
            if (Instance.Disablebroadcast)
                return;
            var player = Player.Get(other.gameObject);
            if (player == null) return;
            player.ClearBroadcasts();
            if (EventHandlers.ClassDPlayers.Contains(player))
                EventHandlers.ClassDPlayers.Remove(player);
        }
    }

    public class ScpSpawner : MonoBehaviour
    {
        public void OnTriggerEnter(Collider other)
        {
            if (Instance.Disablebroadcast)
                return;
            var player = Player.Get(other.gameObject);
            if (player == null) return;
            player.Broadcast(300, Instance.Instance.Translation.Scpmessage, Broadcast.BroadcastFlags.Normal, true);
            if (!EventHandlers.ScpPlayers.Contains(player))
                EventHandlers.ScpPlayers.Add(player);
        }

        public void OnTriggerExit(Collider other)
        {
            var player = Player.Get(other.gameObject);
            if (player == null) return;
            player.ClearBroadcasts();
            if (EventHandlers.ScpPlayers.Contains(player))
                EventHandlers.ScpPlayers.Remove(player);
        }
    }

    public class GuardSpawner : MonoBehaviour
    {
        public void OnTriggerEnter(Collider other)
        {
            if (Instance.Disablebroadcast)
                return;
            var player = Player.Get(other.gameObject);
            if (player == null) return;
            player.Broadcast(300, Instance.Instance.Translation.Guardmessage, Broadcast.BroadcastFlags.Normal, true);
            if (!EventHandlers.GuardPlayers.Contains(player))
                EventHandlers.GuardPlayers.Add(player);
        }

        public void OnTriggerExit(Collider other)
        {
            if (Instance.Disablebroadcast)
                return;
            var player = Player.Get(other.gameObject);
            if (player == null) return;
            player.ClearBroadcasts();
            if (EventHandlers.GuardPlayers.Contains(player))
                EventHandlers.GuardPlayers.Remove(player);
        }
    }

    public class ScientistSpawner : MonoBehaviour
    {
        public void OnTriggerEnter(Collider other)
        {
            if (Instance.Disablebroadcast)
                return;
            var player = Player.Get(other.gameObject);
            if (player == null) return;
            player.Broadcast(300, Instance.Instance.Translation.Scientistmessage, Broadcast.BroadcastFlags.Normal,
                true);
            if (!EventHandlers.ScientistPlayers.Contains(player))
                EventHandlers.ScientistPlayers.Add(player);
        }

        public void OnTriggerExit(Collider other)
        {
            if (Instance.Disablebroadcast)
                return;
            var player = Player.Get(other.gameObject);
            if (player == null) return;
            player.ClearBroadcasts();
            if (EventHandlers.ScientistPlayers.Contains(player))
                EventHandlers.ScientistPlayers.Remove(player);
        }
    }
}
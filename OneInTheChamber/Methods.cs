using System.Collections.Generic;
using System.Text;
using Exiled.API.Features;
using MEC;
using UnityEngine;
using LightContainmentZoneDecontamination;
using CustomPlayerEffects;

namespace OneInTheChamber
{
    public class Methods
    {
        private readonly Plugin plugin;
        public Methods(Plugin plugin) => this.plugin = plugin;

        internal void RegisterEvents(bool disable = false)
        {
            switch (disable)
            {
                case true:
                    Exiled.Events.Handlers.Server.RoundStarted -= plugin.EventHandlers.OnRoundStart;
                    Exiled.Events.Handlers.Server.RoundEnded -= plugin.EventHandlers.OnRoundEnd;
                    Exiled.Events.Handlers.Player.PickingUpItem -= plugin.EventHandlers.OnPickingUpItem;
                    Exiled.Events.Handlers.Player.Dying -= plugin.EventHandlers.OnDying;
                    Exiled.Events.Handlers.Player.Hurting -= plugin.EventHandlers.OnHurting;
                    Exiled.Events.Handlers.Player.Escaping -= plugin.EventHandlers.OnEscaping;
                    Exiled.Events.Handlers.Player.Shooting -= plugin.EventHandlers.OnShooting;
                    Exiled.Events.Handlers.Player.Spawning -= plugin.EventHandlers.OnSpawning;
                    Exiled.Events.Handlers.Player.InteractingDoor -= plugin.EventHandlers.OnInteractingDoor;
                    break;
                case false:
                    Exiled.Events.Handlers.Server.RoundStarted += plugin.EventHandlers.OnRoundStart;
                    Exiled.Events.Handlers.Server.RoundEnded += plugin.EventHandlers.OnRoundEnd;
                    Exiled.Events.Handlers.Player.PickingUpItem += plugin.EventHandlers.OnPickingUpItem;
                    Exiled.Events.Handlers.Player.Dying += plugin.EventHandlers.OnDying;
                    Exiled.Events.Handlers.Player.Hurting += plugin.EventHandlers.OnHurting;
                    Exiled.Events.Handlers.Player.Escaping += plugin.EventHandlers.OnEscaping;
                    Exiled.Events.Handlers.Player.Shooting += plugin.EventHandlers.OnShooting;
                    Exiled.Events.Handlers.Player.Spawning += plugin.EventHandlers.OnSpawning;
                    Exiled.Events.Handlers.Player.InteractingDoor += plugin.EventHandlers.OnInteractingDoor;
                    break;
            }
        }

        public void SetupPlayers()
        {
            Round.IsLocked = true;
            plugin.IsRunning = true;

            plugin.Scores = new Dictionary<Player, int>();
            plugin.Lives = new Dictionary<Player, int>();

            Timing.CallDelayed(0.1f, () =>
            {
                foreach(Player p in Player.List)
				{
                    p.SetRole(RoleType.ClassD, true);
                    Timing.CallDelayed(0.1f, () =>
                    {
                        if (plugin.Config.OnlyUseSurface)
                            p.ReferenceHub.playerMovementSync.OverridePosition(GetRandomSurfaceSpawn(), 0, true);
                        else
                            p.ReferenceHub.playerMovementSync.OverridePosition(GetRandomSpawn(p), 0, true);
						p.EnableEffect<Ensnared>(999f);
						p.EnableEffect<Amnesia>(999f);
                        p.IsFriendlyFireEnabled = true;
                    });

                    plugin.Lives.Add(p, plugin.Config.Lives);
                    plugin.Scores.Add(p, 0);
                }
                plugin.Coroutine = Timing.RunCoroutine(StartRound());
            });
        }

        public void SetupMap()
		{
            if (plugin.Config.OnlyUseSurface)
                Warhead.Detonate();
            else if (plugin.Config.DisableDecontamination)
                DecontaminationController.Singleton.disableDecontamination = true;
            else if (plugin.Config.InstantDecontamination)
                Map.StartDecontamination();
		}

        public void EnableGamemode(bool force = false)
        {
            if (!force)
                plugin.IsEnabled = true;
            else
            {
                plugin.IsEnabled = true;
                Timing.CallDelayed(0.1f, () =>
                {
                    SetupMap();
                    SetupPlayers();
                });
            }

            plugin.ShouldDisableNextRound = true;
        }

        public void DisableGamemode(bool force = false)
        {
            if (!force)
                plugin.ShouldDisableNextRound = true;
            else
            {
                List<RoleType> scpRoles = new List<RoleType>
                {
                    RoleType.Scp049,
                    RoleType.Scp079,
                    RoleType.Scp096,
                    RoleType.Scp106,
                    RoleType.Scp173,
                    RoleType.Scp93953,
                    RoleType.Scp93989
                };

                foreach (Player player in Player.List)
                {
                    if (player.Role == RoleType.Scp173)
                    {
                        player.Role = scpRoles[plugin.Rng.Next(scpRoles.Count)];
                    }
                    else
                    {
                        int r = plugin.Rng.Next(6);
                        switch (r)
                        {
                            case 6:
                                player.Role = RoleType.FacilityGuard;
                                break;
                            case 5:
                            case 4:
                                player.Role = RoleType.Scientist;
                                break;
                            default:
                                player.Role = RoleType.ClassD;
                                break;
                        }
                    }
                }

                plugin.IsEnabled = false;
                plugin.IsRunning = false;
                Round.IsLocked = false;
            }
        }

        public Vector3 GetRandomSurfaceSpawn()
		{
            bool useRoleSpawns = plugin.Rng.Next(6) == 0;
            if (useRoleSpawns)
			{
                List<RoleType> roles = new List<RoleType>
                {
                    RoleType.NtfCadet,
                    RoleType.ChaosInsurgency
                };
                return roles[plugin.Rng.Next(roles.Count)].GetRandomSpawnPoint();
            }
            return plugin.SurfaceSpawns[plugin.Rng.Next(plugin.SurfaceSpawns.Count)];
        }

        public Vector3 GetRandomSpawn(Player p)
		{
			bool useRoleSpawns = plugin.Rng.Next(2) == 0;
            if (!useRoleSpawns || Warhead.IsDetonated)
                return GetRandomSurfaceSpawn();
            if (Map.IsLCZDecontaminated)
			{
                List<RoleType> roles = new List<RoleType>
                {
                    RoleType.FacilityGuard,
                    RoleType.Scp049,
                    RoleType.Scp096,
                    RoleType.Scp106,
                    RoleType.Scp93953
                };
                return roles[plugin.Rng.Next(roles.Count)].GetRandomSpawnPoint();
			}
            else
			{
                Role[] classes = p.ReferenceHub.characterClassManager.Classes;
                return classes[plugin.Rng.Next(classes.Length)].roleId.GetRandomSpawnPoint();
            }
		}

        public IEnumerator<float> StartRound()
		{
			for (int i = 5; i > 0; i--)
			{
				yield return Timing.WaitForSeconds(1f);
				Map.Broadcast(1, plugin.Config.StartingBroadcastMessage.Replace("$time", i.ToString()));
			}
			foreach (Player p in Player.List)
			{
				p.DisableAllEffects();
			}
			plugin.Coroutine = Timing.RunCoroutine(RoundTimer());
            yield break;
        }

        public IEnumerator<float> RoundTimer()
        {
            yield return Timing.WaitForSeconds(plugin.Config.MaxDuration);
            int highestScore = -1;
            List<Player> winners = new List<Player>();
            foreach (Player p in Player.List)
            {
                if (plugin.Scores.ContainsKey(p))
				{
                    if (plugin.Scores[p] > highestScore)
                    {
                        winners.Clear();
                        winners.Add(p);
                        highestScore = plugin.Scores[p];
                    }
                    else if (plugin.Scores[p] == highestScore)
                        winners.Add(p);
				}
                p.Kill();
            }
            Round.IsLocked = false;
            StringBuilder winnerNames = new StringBuilder();
            for (int i = 0; i < winners.Count; i++)
            {
                Player p = winners[i];
                winnerNames.Append(p.Nickname);
                if (i != winners.Count - 1) winnerNames.Append(", ");
            }
            Map.Broadcast(10, $"Maximum duration exceeded. {(winners.Count == 1 ? $"{(winnerNames)} has claimed victory!" : $"There was a {winners.Count} way tie between {winnerNames}.")}");
        }
    }
}
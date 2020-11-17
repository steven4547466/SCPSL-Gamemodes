using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using UnityEngine;
using System.Linq;

namespace OneInTheChamber
{
	public class EventHandlers
	{
		private readonly Plugin plugin;
		public EventHandlers(Plugin plugin) => this.plugin = plugin;

        public void OnRoundStart()
        {
            if (!plugin.IsEnabled)
                return;

            Timing.CallDelayed(0.1f, () =>
            {
                plugin.Methods.SetupMap();
                plugin.Methods.SetupPlayers();
            });
        }

        public void OnRoundEnd(RoundEndedEventArgs ev)
        {
            if (!plugin.IsRunning)
                return;

            plugin.Scores = null;
            plugin.Lives = null;
            plugin.IsRunning = false;
            if (plugin.Coroutine != null)
                Timing.KillCoroutines(plugin.Coroutine);
            if (plugin.ShouldDisableNextRound)
                plugin.IsEnabled = false;
        }

        public void OnPickingUpItem(PickingUpItemEventArgs ev)
		{
            if (!plugin.IsRunning)
                return;

            ev.IsAllowed = false;
		}

        public void OnDying(DyingEventArgs ev)
		{
            if (!plugin.IsRunning)
                return;

            ev.Target.ClearInventory();

            if (!plugin.Lives.ContainsKey(ev.Target))
                plugin.Lives.Add(ev.Target, plugin.Config.Lives);
            plugin.Lives[ev.Target]--;

            if (plugin.Lives[ev.Target] <= 0)
            {
                int playersLeft = Player.List.Count(p => p.Role == RoleType.ClassD) - 1;
                if (playersLeft > 1) Map.Broadcast(5, plugin.Config.EliminationBroadcastMessage.Replace("%user", ev.Target.Nickname).Replace("$count", playersLeft.ToString()));
                else
				{
                    Map.Broadcast(10, $"{Player.List.First(p => p.Role == RoleType.ClassD).Nickname} is the last player standing and has claimed victory!");
                    Round.IsLocked = false;
                }
                return;
            }

            Timing.CallDelayed(plugin.Config.RespawnTime, () =>
            {
                ev.Target.SetRole(RoleType.ClassD, true);
                Timing.CallDelayed(0.1f, () => 
                {
                    if (plugin.Config.OnlyUseSurface)
                        ev.Target.Position = plugin.Methods.GetRandomSurfaceSpawn();
                    else
                        ev.Target.Position = plugin.Methods.GetRandomSpawn(ev.Target);
                });
            });
        }

        public void OnSpawning(SpawningEventArgs ev)
		{
            if (!plugin.IsRunning)
                return;

            Timing.CallDelayed(0.1f, () =>
            {
                ev.Player.ClearInventory();
                ev.Player.Inventory.AddNewItem(ItemType.GunCOM15, 1);
                ev.Player.Inventory.AddNewItem(ItemType.GunUSP, 2);
                ev.Player.IsFriendlyFireEnabled = true;
            });
        }

        public void OnHurting(HurtingEventArgs ev)
		{
            if (!plugin.IsRunning)
                return;

            if (ev.DamageType == DamageTypes.Com15 || (ev.DamageType == DamageTypes.Usp && Vector3.Distance(ev.Attacker.Position, ev.Target.Position) <= plugin.Config.UspKillRange))
            {
                ev.Amount = 9999f;
                if (!plugin.Scores.ContainsKey(ev.Attacker))
                    plugin.Scores.Add(ev.Attacker, 0);
                plugin.Scores[ev.Attacker]++;

                if (plugin.Scores[ev.Attacker] >= plugin.Config.ScoreToWin)
                {
                    foreach (Player p in Player.List)
                    {
                        if (p.Id != ev.Attacker.Id) p.Kill();
                    }
                    Map.Broadcast(10, $"{ev.Attacker.Nickname} has reached {plugin.Scores[ev.Attacker]} points and has won the game!");
                    Round.IsLocked = false;
                }
            }
            else if (ev.DamageType == DamageTypes.Usp || ev.DamageType == DamageTypes.Com15)
                ev.IsAllowed = false;
        }

        public void OnEscaping(EscapingEventArgs ev)
		{
            if (!plugin.IsRunning)
                return;

            ev.IsAllowed = false;
        }

        public void OnShooting(ShootingEventArgs ev)
		{
            if (!plugin.IsRunning)
                return;

            Timing.CallDelayed(0.1f, () =>
            {
                if (ev.Shooter.CurrentItem.id == ItemType.GunUSP)
                {
                    ev.Shooter.Inventory.items.ModifyDuration(ev.Shooter.Inventory.GetItemIndex(), 2);
                }

                Player target = Player.Get(ev.Target?.gameObject);

                if (target != null && (ev.Shooter.CurrentItem.id == ItemType.GunCOM15 || ev.Shooter.CurrentItem.id == ItemType.GunUSP))
                {
                    if (ev.Shooter.CurrentItem.id == ItemType.GunCOM15 || (ev.Shooter.CurrentItem.id == ItemType.GunUSP && Vector3.Distance(ev.Shooter.Position, target.Position) <= plugin.Config.UspKillRange))
                    {
                        int index = ev.Shooter.Inventory.items.FindIndex(i => i.id == ItemType.GunCOM15);
                        Inventory.SyncItemInfo gun = ev.Shooter.Inventory.items[index];

                        ev.Shooter.Inventory.items.ModifyDuration(index, gun.durability + 1);
                    }
                }
            });
        }

        public void OnInteractingDoor(InteractingDoorEventArgs ev)
		{
            if (!plugin.IsRunning)
                return;

            ev.IsAllowed = true;
		}
    }
}

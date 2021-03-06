﻿using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;

namespace Juggernaut
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
                    Exiled.Events.Handlers.Player.ThrowingGrenade -= plugin.EventHandlers.OnThrowingGrenade;
                    Exiled.Events.Handlers.Server.RoundStarted -= plugin.EventHandlers.OnRoundStart;
                    Exiled.Events.Handlers.Server.RoundEnded -= plugin.EventHandlers.OnRoundEnd;
                    Exiled.Events.Handlers.Player.Joined -= plugin.EventHandlers.OnPlayerJoin;
                    break;
                case false:
                    Exiled.Events.Handlers.Player.ThrowingGrenade += plugin.EventHandlers.OnThrowingGrenade;
                    Exiled.Events.Handlers.Server.RoundStarted += plugin.EventHandlers.OnRoundStart;
                    Exiled.Events.Handlers.Server.RoundEnded += plugin.EventHandlers.OnRoundEnd;
                    Exiled.Events.Handlers.Player.Joined += plugin.EventHandlers.OnPlayerJoin;
                    break;
            }
        }
        
        public void SetupPlayers()
        {
            int r = plugin.Rng.Next(Player.Dictionary.Count);

            plugin.Juggernaut = Player.List.ElementAt(r);
            
            foreach (Player player in Player.List)
                if (player != plugin.Juggernaut)
                {
                    player.Role = RoleType.NtfCommander;
                    if (plugin.Config.CommandersGetMicro)
                        Timing.CallDelayed(0.5f, () => player.AddItem(ItemType.MicroHID));
                }

            plugin.Juggernaut.Role = RoleType.ChaosInsurgency;
            Timing.CallDelayed(0.75f, () =>
            {
                plugin.Juggernaut.Position = RoleType.Scp93953.GetRandomSpawnPoint();
                plugin.Juggernaut.Health = plugin.Config.JuggernautHealth;
                if (plugin.Config.JuggernautInv.Count > 0)
                    plugin.Juggernaut.ResetInventory(plugin.Config.JuggernautInv);
            });
        }

        public void EnableGamemode(bool force = false)
        {
            if (!force)
                plugin.IsEnabled = true;
            else
            {
                plugin.IsEnabled = true;
                SetupPlayers();
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
            }
        }
    }
}
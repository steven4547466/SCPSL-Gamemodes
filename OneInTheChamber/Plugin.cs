using System;
using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace OneInTheChamber
{
    public class Plugin : Plugin<Config>
    {
        public override string Author { get; } = "Steven4547466";
        public override string Name { get; } = "OneInTheChamber";
        public override string Prefix { get; } = "gamemode_OneInTheChamber";
        public override Version Version { get; } = new Version(1, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(2, 1, 15);

        public static Plugin Singleton;

        public Methods Methods;
        public EventHandlers EventHandlers;
        public bool IsEnabled = false;
        public bool IsRunning = false;
        public bool ShouldDisableNextRound = false;
        public System.Random Rng = new System.Random();
        public string PermissionString = "gamemodes.staff";

        internal Dictionary<Player, int> Scores;
        internal Dictionary<Player, int> Lives;

        internal List<Vector3> SurfaceSpawns = new List<Vector3>
        {
            new Vector3(-20.5f, 1003f, -70f),
            new Vector3(-1.1f, 1003f, -70f),
            new Vector3(15.5f, 1003f, -70f),
            new Vector3(28.2f, 1003f, -70f),
            new Vector3(45f, 1003f, -70f),
            new Vector3(52.5f, 1003f, -62.5f),
            new Vector3(52.5f, 1003f, -52.5f),
            new Vector3(44f, 1003f, -43.2f),
            new Vector3(26.8f, 1003f, -43.2f),
            new Vector3(14.5f, 1000f, -35f),
            new Vector3(18.8f, 1000f, -46.5f),
            new Vector3(21.4f, 995f, -48.3f),
            new Vector3(14f, 1000f, -20f),
            new Vector3(-0.7f, 1003f, -20f),
            new Vector3(-0.07f, 1003f, -36f),
            new Vector3(0.4f, 1003f, -53.5f),
            new Vector3(-12f, 1003f, -43f),
            new Vector3(-54.8f, 985f, -62f),
            new Vector3(-41.5f, 988f, -53f),
            new Vector3(-29f, 990f, -63.8f),
            new Vector3(-27f, 990f, -49.7f),
            new Vector3(-43f, 990f, -49.8f),
            new Vector3(-53.6f, 990f, -49.5f),
            new Vector3(40.5f, 990f, -42.9f),
            new Vector3(47.8f, 990f, -50.8f),
            new Vector3(34.5f, 990f, -62f),
            new Vector3(63.7f, 990f, -61.8f),
            new Vector3(70.6f, 990f, -65.5f),
            new Vector3(69.6f, 990f, -49f),
            new Vector3(80.4f, 990f, -50f),
            new Vector3(87.2f, 990f, -68.6f),
            new Vector3(98f, 990f, -59f),
            new Vector3(115.2f, 990f, -53.5f),
            new Vector3(128f, 992f, -63.5f),
            new Vector3(140.6f, 994f, -53.6f),
            new Vector3(150f, 996f, -71.4f),
            new Vector3(170.2f, 996f, -70f),
            new Vector3(181.7f, 996f, -70f),
            new Vector3(182.5f, 996f, -87.4f),
            new Vector3(192.7f, 996f, -90.8f),
            new Vector3(191.2f, 996f, -47.4f),
            new Vector3(187.4f, 996f, -29.1f),
            new Vector3(13f, 1003f, 0.8f),
            new Vector3(-10.8f, 1003f, 0.7f),
            new Vector3(0.3f, 1003f, 8.6f)
        };
        internal CoroutineHandle Coroutine;

        public override void OnEnabled()
        {
            Singleton = this;
            Methods = new Methods(this);
            EventHandlers = new EventHandlers(this);

            Methods.RegisterEvents();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Methods.RegisterEvents(true);
            Methods = null;
            EventHandlers = null;

            base.OnDisabled();
        }
    }
}
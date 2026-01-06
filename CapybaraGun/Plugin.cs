using System;
using System.Collections.Generic;
using CapybaraGun.Features;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Toys;
using Exiled.CustomItems.API;
using Exiled.Events.EventArgs.Player;
using LabApi.Events;
using UnityEngine;
using events = Exiled.Events.Handlers;

namespace CapybaraGun
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "CapybaraGun";
        public override string Prefix => Name;
        public override string Author => "Morkamo";
        public override Version RequiredExiledVersion => new(9, 12, 1);
        public override Version Version => new(1, 0, 0);

        public static Plugin Instance;
        public static List<GameObject> PermanentObjects { get; set; } = new();
        public static Dictionary<Player, GameObject> AttachedCameras { get; set; } = new();

        public override void OnEnabled()
        {
            Instance = this;
            events.Player.Verified += OnVerifiedPlayer;
            Config.CapybaraGun.Register();
            base.OnEnabled();
        }

        public override void OnDisabled() 
        {
            Config.CapybaraGun.Unregister();
            events.Player.Verified -= OnVerifiedPlayer;
            Instance = null;
            base.OnDisabled();
        }
        
        private void OnVerifiedPlayer(VerifiedEventArgs ev)
        {
            if (ev.Player.ReferenceHub.gameObject.GetComponent<CapybaraGunProperties>() != null)
                return;

            ev.Player.ReferenceHub.gameObject.AddComponent<CapybaraGunProperties>();
        }
    }
}
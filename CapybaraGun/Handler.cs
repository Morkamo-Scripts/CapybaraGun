using System;
using System.Collections.Generic;
using System.Linq;
using AdminToys;
using CapybaraGun.Extensions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using JetBrains.Annotations;
using LabApi.Events.Arguments.PlayerEvents;
using MEC;
using UnityEngine;
using levents = LabApi.Events.Handlers;
using events = Exiled.Events.Handlers;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace CapybaraGun;

public class Handler : CustomItem
{
    public override uint Id { get; set; } = 90;
    public override string Name { get; set; } = "Capybara-FRMG0";
    public override string Description { get; set; } = String.Empty;
    public override float Weight { get; set; } = 1;
    public override ItemType Type { get; set; } = ItemType.GunFRMG0;
    public override SpawnProperties SpawnProperties { get; set; } = null;

    private List<string> _colors = new()
    {
        "red",
        "green",
        "blue",
        "yellow",
        "white",
        "black",
        "grey"
    };

    protected override void SubscribeEvents()
    {
        levents.PlayerEvents.ShootingWeapon += OnShooting;
        events.Player.Hurting += OnHurting;
        events.Player.ReloadingWeapon += OnReloading;
        events.Player.UnloadingWeapon += OnUnloadingWeapon;
        events.Player.Shot += OnShot;
        events.Player.Died += OnDied;
        levents.PlayerEvents.Left += OnLeft;
    }

    protected override void UnsubscribeEvents()
    {
        levents.PlayerEvents.ShootingWeapon -= OnShooting;
        events.Player.Hurting -= OnHurting;
        events.Player.ReloadingWeapon -= OnReloading;
        events.Player.UnloadingWeapon -= OnUnloadingWeapon;
        events.Player.Shot -= OnShot;
        events.Player.Died -= OnDied;
        levents.PlayerEvents.Left -= OnLeft;
    }

    private void OnLeft(PlayerLeftEventArgs ev)
    {
        if (Plugin.AttachedCameras.TryGetValue(ev.Player, out var camera))
        {
            Object.Destroy(camera);
            Plugin.AttachedCameras.Remove(ev.Player);
        }
    }
    
    private void OnDied(DiedEventArgs ev)
    {
        if (Plugin.AttachedCameras.TryGetValue(ev.Player, out var camera))
        {
            Object.Destroy(camera.gameObject);
            Plugin.AttachedCameras.Remove(ev.Player);
        }
    }

    private void OnShooting(PlayerShootingWeaponEventArgs ev)
    {
        if (!Check(Item.Get(ev.FirearmItem.Base)))
            return;

        if (ev.FirearmItem.StoredAmmo < ev.FirearmItem.MaxAmmo)
            ev.FirearmItem.StoredAmmo = ev.FirearmItem.MaxAmmo;
    }

    private void OnHurting(HurtingEventArgs ev)
    {
        if (Check(ev.Attacker?.CurrentItem))
            ev.IsAllowed = false;
    }

    private void OnUnloadingWeapon(UnloadingWeaponEventArgs ev)
    {
        if (Check(ev.Firearm))
            ev.IsAllowed = false;
    }
    
    private void OnReloading(ReloadingWeaponEventArgs ev)
    {
        if (Check(ev.Firearm))
            ev.IsAllowed = false;
    }

    private void OnShot(ShotEventArgs ev)
    {
        if (Check(ev.Firearm))
        {
            var props = ev.Player.CapybaraGunProperties().PlayerProps;

            if (props.IsTextSpawnerEnabled)
            {
                SpawnText(ev.Player);
                return;
            }
            
            if (props.BulletTypeSpecial > -1)
            {
                SpawnPrefab(ev.Player, (PrefabType)props.BulletTypeSpecial);
                return;
            }
            
            if (props.BulletType == 0)
                SpawnCapybara(ev.Player);
            else
            {
                try
                {
                    SpawnItem(ev.Player, Item.Get(((ItemType)props.BulletType).GetItemBase()));
                }
                catch
                {
                    // ignored
                }
            }
        }
    }

    private void SpawnCapybara(Player player)
    {
        if (player == null)
            return;

        var cam = player.CameraTransform;

        Vector3 spawnPos = (cam.position - new Vector3(0, 0.3f, 0)) + cam.forward * 1;
        Quaternion rotation = Quaternion.LookRotation(cam.forward);

        var capybara = PrefabHelper.Spawn(PrefabType.CapybaraToy, spawnPos, rotation);
        capybara.SetWorldScale(new Vector3(0.6f, 0.6f, 0.6f));

        if (player.CapybaraGunProperties().PlayerProps.IsPhysicsEnabled)
        {
            var rb = capybara.AddComponent<Rigidbody>();
            
            if (!player.CapybaraGunProperties().PlayerProps.IsAntiGravityEnabled)
                rb.useGravity = true;
            else 
                rb.useGravity = false;
            
            rb.isKinematic = false;
            rb.mass = 1f;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            float force = player.CapybaraGunProperties().PlayerProps.BulletForce;
            rb.AddForce(cam.forward * force, ForceMode.Impulse);

            Vector3 spinAxis = Random.onUnitSphere;
            float spinStrength = player.CapybaraGunProperties().PlayerProps.AngularSpinVelocity;
            rb.angularVelocity = spinAxis * spinStrength;
        }
        
        if ((int)player.CapybaraGunProperties().PlayerProps.DestroyDelay != 0)
            Timing.CallDelayed(player.CapybaraGunProperties().PlayerProps.DestroyDelay, 
                () => Object.Destroy(capybara));
        else Plugin.PermanentObjects.Add(capybara.gameObject);
    }
    
    private void SpawnItem(Player player, [CanBeNull] Item item)
    {
        if (player == null || item == null || player.CameraTransform == null)
            return;

        var cam = player.CameraTransform;

        Vector3 spawnPos = (cam.position + cam.forward * 1f);
        Quaternion rotation = Quaternion.LookRotation(cam.forward);
        
        var pickup = Pickup.CreateAndSpawn(item.Type, spawnPos, rotation);

        if (player.CapybaraGunProperties().PlayerProps.IsPhysicsEnabled)
        {
            var rb = pickup.Base.GetComponent<Rigidbody>();
            if (rb != null)
            {
                if (!player.CapybaraGunProperties().PlayerProps.IsAntiGravityEnabled)
                    rb.useGravity = true;
                else 
                    rb.useGravity = false;
                
                float force = player.CapybaraGunProperties().PlayerProps.BulletForce;
                rb.AddForce(cam.forward * force, ForceMode.Impulse);
            }
        }

        if ((int)player.CapybaraGunProperties().PlayerProps.DestroyDelay != 0)
            Timing.CallDelayed(player.CapybaraGunProperties().PlayerProps.DestroyDelay, 
                () => pickup.Destroy());
        else Plugin.PermanentObjects.Add(pickup.GameObject);
    }

    private void SpawnPrefab(Player player, PrefabType prefabType)
    {
        if (player == null)
            return;

        var cam = player.CameraTransform;

        Vector3 spawnPos = (cam.position - new Vector3(0, 0.3f, 0)) + cam.forward * 1;
        Quaternion rotation = Quaternion.LookRotation(cam.forward);

        var prefabGo = PrefabHelper.Spawn(prefabType, spawnPos, rotation);
        prefabGo.SetWorldScale(player.CapybaraGunProperties().PlayerProps.BulletScale);

        if (player.CapybaraGunProperties().PlayerProps.IsPhysicsEnabled)
        {
            var rb = prefabGo.AddComponent<Rigidbody>();
            
            if (!player.CapybaraGunProperties().PlayerProps.IsAntiGravityEnabled)
                rb.useGravity = true;
            else 
                rb.useGravity = false;
            
            rb.isKinematic = false;
            rb.mass = 1f;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            float force = player.CapybaraGunProperties().PlayerProps.BulletForce;
            rb.AddForce(cam.forward * force, ForceMode.Impulse);

            Vector3 spinAxis = Random.onUnitSphere;
            float spinStrength = player.CapybaraGunProperties().PlayerProps.AngularSpinVelocity;
            rb.angularVelocity = spinAxis * spinStrength;
        }
        
        if ((int)player.CapybaraGunProperties().PlayerProps.DestroyDelay != 0)
            Timing.CallDelayed(player.CapybaraGunProperties().PlayerProps.DestroyDelay, 
                () => Object.Destroy(prefabGo));
        else Plugin.PermanentObjects.Add(prefabGo.gameObject);
    }
    
    private void SpawnText(Player player)
    {
        if (player == null)
            return;

        var cam = player.CameraTransform;

        Vector3 spawnPos = (cam.position - new Vector3(0, 0.3f, 0)) + cam.forward * 1;
        Quaternion rotation = Quaternion.LookRotation(-cam.forward);

        var prefabGo = PrefabHelper.Spawn<TextToy>(PrefabType.TextToy, spawnPos, rotation);
        var capybaraProps = player.CapybaraGunProperties().PlayerProps;
        var textColor = capybaraProps.IsSymbolsColorRandomazerEnabled 
            ? _colors.GetRandomValue() 
            : "white";

        if (capybaraProps.IsSymbolsRandomazerEnabled)
        {
            string text = capybaraProps.MessageText ?? string.Empty;
            
            var parts = text
                .Split([' '], StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 0)
            {
                int index = Random.Range(0, parts.Length);
                string randomPart = parts[index];

                prefabGo.TextFormat = $"<color={textColor}>{randomPart}</color>";
                prefabGo.Network_textFormat = $"<color={textColor}>{randomPart}</color>";
            }
            else
            {
                prefabGo.TextFormat = ".";
                prefabGo.Network_textFormat = ".";
            }
        }
        else
        {
            prefabGo.TextFormat = $"<color={textColor}>{capybaraProps.MessageText}</color>";
            prefabGo.Network_textFormat = $"<color={textColor}>{capybaraProps.MessageText}</color>";
        }
        
        prefabGo.gameObject.SetWorldScale(capybaraProps.BulletScale);

        if (capybaraProps.IsPhysicsEnabled)
        {
            var rb = prefabGo.gameObject.AddComponent<Rigidbody>();
            
            if (!capybaraProps.IsAntiGravityEnabled)
                rb.useGravity = true;
            else 
                rb.useGravity = false;
            
            rb.isKinematic = false;
            rb.mass = 1f;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            float force = capybaraProps.BulletForce;
            rb.AddForce(cam.forward * force, ForceMode.Impulse);

            Vector3 spinAxis = Random.onUnitSphere;
            float spinStrength = capybaraProps.AngularSpinVelocity;
            rb.angularVelocity = spinAxis * spinStrength;
        }
        
        if ((int)capybaraProps.DestroyDelay != 0)
            Timing.CallDelayed(capybaraProps.DestroyDelay, 
                () => Object.Destroy(prefabGo));
        else Plugin.PermanentObjects.Add(prefabGo.gameObject);
    }
}
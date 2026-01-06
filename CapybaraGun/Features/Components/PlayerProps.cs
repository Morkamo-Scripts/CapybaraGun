using System;
using CapybaraGun.Features.Components.Interfaces;
using UnityEngine;
using Utf8Json.Internal.DoubleConversion;

namespace CapybaraGun.Features.Components;

public class PlayerProps(CapybaraGunProperties capybaraGunProperties) : IPropertyModule
{
    public CapybaraGunProperties CapybaraGunProperties { get; } = capybaraGunProperties;
    public ushort BulletType { get; set; }
    public int BulletTypeSpecial { get; set; } = -1;
    public float BulletForce { get; set; } = 20;
    public Vector3 BulletScale { get; set; } = new (0.6f, 0.6f, 0.6f);
    public bool IsPhysicsEnabled { get; set; } = true;
    public bool IsAntiGravityEnabled { get; set; } = false;
    public float AngularSpinVelocity { get; set; } = 25f;
    public bool IsTextSpawnerEnabled { get; set; } = false;
    public string MessageText { get; set; } = String.Empty;
    public float DestroyDelay { get; set; } = 10f;
    public bool IsSymbolsRandomazerEnabled { get; set; } = false;
    public bool IsSymbolsColorRandomazerEnabled { get; set; } = false;
}
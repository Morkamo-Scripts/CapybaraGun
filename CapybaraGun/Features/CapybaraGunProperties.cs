using CapybaraGun.Features.Components;
using Exiled.API.Features;
using UnityEngine;

namespace CapybaraGun.Features;

public sealed class CapybaraGunProperties() : MonoBehaviour
{
    private void Awake()
    {
        Player = Player.Get(gameObject);
        PlayerProps = new PlayerProps(this);
    }
    
    public Player Player { get; private set; }
    public PlayerProps PlayerProps { get; private set; }
}
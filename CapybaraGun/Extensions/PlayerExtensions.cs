using CapybaraGun.Features;
using CommandSystem;
using Exiled.API.Features;

namespace CapybaraGun.Extensions;

public static class PlayerExtensions
{
    public static Player AsPlayer(this ICommandSender sender)
        => Player.Get(sender);

    public static CapybaraGunProperties CapybaraGunProperties(this Player player)
        => player.ReferenceHub.GetComponent<CapybaraGunProperties>();
}
using System;
using CapybaraGun.Extensions;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Permissions.Extensions;
using RemoteAdmin;
using UnityEngine;

namespace CapybaraGun.Commands;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class BulletAngularSpeedSettingsCommand : ICommand
{
    public string Command { get; } = "cgtags";
    public string[] Aliases { get; } = [];
    public string Description { get; } = "Задаёт коофициент угловой скорость вращения снаряда для CapybaraGun (по умолчанию 25)";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (sender is not PlayerCommandSender || !sender.CheckPermission("ac.ctgscale"))
        {
            string requestPermission = "Требуется разрешение - 'ac.ctgscale'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }

        if (!float.TryParse(arguments.At(0), out var force))
        {
            response = "<color=red>Некорректное значения силы!</color>";
            return false;
        }

        force = Mathf.Clamp(force, 0, 100f);

        sender.AsPlayer().CapybaraGunProperties().PlayerProps.AngularSpinVelocity = force;
        response = $"<color=green>Успешно установлены значения {force:F1}!</color>";
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}
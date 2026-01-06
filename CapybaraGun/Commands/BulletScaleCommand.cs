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
public class BulletScaleCommand : ICommand
{
    public string Command { get; } = "cgtsc";
    public string[] Aliases { get; } = [];
    public string Description { get; } = "Задаёт размер снаряда для CapybaraGun (по умолчанию 0.6 0.6 0.6)";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (sender is not PlayerCommandSender || !sender.CheckPermission("ac.cgtsc"))
        {
            string requestPermission = "Требуется разрешение - 'ac.cgtsc'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }

        if (!float.TryParse(arguments.At(0), out var force1) || 
            !float.TryParse(arguments.At(1), out var force2) ||
            !float.TryParse(arguments.At(2), out var force3))
        {
            response = "<color=red>Некорректное значения силы!</color>";
            return false;
        }

        force1 = Mathf.Clamp(force1, 0.1f, 10f);
        force2 = Mathf.Clamp(force2, 0.1f, 10f);
        force3 = Mathf.Clamp(force3, 0.1f, 10f);
        
        sender.AsPlayer().CapybaraGunProperties().PlayerProps.BulletScale = new Vector3(force1, force2, force3);
        response = $"<color=green>Успешно установлены значения {force1:F1} {force2:F1} {force3:F1}!</color>";
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}
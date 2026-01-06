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
public class BulletForceCommand : ICommand
{
    public string Command { get; } = "cgtforce";
    public string[] Aliases { get; } = [];
    public string Description { get; } = "Задаёт силу снаряда для CapybaraGun (по умолчанию 20)";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (sender is not PlayerCommandSender || !sender.CheckPermission("ac.ctgforce"))
        {
            string requestPermission = "Требуется разрешение - 'ac.ctgforce'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }

        if (!float.TryParse(arguments.At(0), out var force))
        {
            response = "<color=red>Некорректное значение силы!</color>";
            return false;
        }

        if (force < 0 || force > 100)
        {
            force = Mathf.Clamp(force, 0f, 100f);
        }
        
        sender.AsPlayer().CapybaraGunProperties().PlayerProps.BulletForce = force;
        response = $"<color=green>Успешно установлено значения {force:F1}!</color>";
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}
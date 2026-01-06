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
public class BulletPhysicsCommand : ICommand
{
    public string Command { get; } = "cgtphys";
    public string[] Aliases { get; } = [];
    public string Description { get; } = "Управляет физикой снаряда для CapybaraGun (если значение 0 ты выкл)";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (sender is not PlayerCommandSender || !sender.CheckPermission("ac.cgtphys"))
        {
            string requestPermission = "Требуется разрешение - 'ac.cgtphys'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }

        if (!ushort.TryParse(arguments.At(0), out var state))
        {
            response = "<color=red>Некорректное значение!</color>";
            return false;
        }

        bool converted;
        if (state == 0) converted = false;
        else converted = true;
        
        sender.AsPlayer().CapybaraGunProperties().PlayerProps.IsPhysicsEnabled = converted;
        response = $"<color=green>Успешно!</color>";
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}
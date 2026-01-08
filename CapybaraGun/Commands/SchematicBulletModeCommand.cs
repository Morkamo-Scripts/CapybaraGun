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
public class SchematicBulletModeCommand : ICommand
{
    public string Command { get; } = "cgtsbm";
    public string[] Aliases { get; } = [];
    public string Description { get; } = "Выбирает Schematic в качестве снаряда для CapybaraGun (Если строка пустая то выкл)";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (sender is not PlayerCommandSender || !sender.CheckPermission("ac.cgtsbm"))
        {
            string requestPermission = "Требуется разрешение - 'ac.cgtsbm'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }
        
        if (arguments.Count < 1)
            sender.AsPlayer().CapybaraGunProperties().PlayerProps.IsSchematicBulletType = String.Empty;
        else
            sender.AsPlayer().CapybaraGunProperties().PlayerProps.IsSchematicBulletType = arguments.At(0);
        response = $"<color=green>Успешно!</color>";
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}
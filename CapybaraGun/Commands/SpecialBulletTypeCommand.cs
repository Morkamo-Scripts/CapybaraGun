using System;
using CapybaraGun.Extensions;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Permissions.Extensions;
using RemoteAdmin;
using UnityEngine;

namespace CapybaraGun.Commands;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class SpecialBulletTypeCommand : ICommand
{
    public string Command { get; } = "cgts";
    public string[] Aliases { get; } = [];
    public string Description { get; } = "Выбирает специальный тип снарядов тип снарядов для CapybaraGun (-1 отключено)";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (sender is not PlayerCommandSender || !sender.CheckPermission("ac.ctgs"))
        {
            string requestPermission = "Требуется разрешение - 'ac.ctgs'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }

        if (!int.TryParse(arguments.At(0), out var prefabId))
        {
            response = "<color=red>Некорректный ID предмета! (укажите -1 для сброса)</color>";
            return false;
        }

        if (!Enum.IsDefined(typeof(PrefabType), prefabId))
        {
            response = "<color=red>Префаб не найден!</color>";
            return false;   
        }

        sender.AsPlayer().CapybaraGunProperties().PlayerProps.BulletTypeSpecial = prefabId;
        
        response = "<color=green>Успешно!</color>";
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}
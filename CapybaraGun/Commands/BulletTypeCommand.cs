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
public class BulletTypeCommand : ICommand
{
    public string Command { get; } = "cgt";
    public string[] Aliases { get; } = [];
    public string Description { get; } = "Тип снарядов для CapybaraGun";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (sender is not PlayerCommandSender || !sender.CheckPermission("ac.ctg"))
        {
            string requestPermission = "Требуется разрешение - 'ac.ctg'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }

        if (!ushort.TryParse(arguments.At(0), out var itemId))
        {
            response = "<color=red>Некорректный ID предмета! (укажите 0 для сброса)</color>";
            return false;
        }

        if (Item.Get(((ItemType)itemId).GetItemBase()) == null)
        {
            response = "<color=red>Предмет не найден!</color>";
            return false;
        }
        
        sender.AsPlayer().CapybaraGunProperties().PlayerProps.BulletType = itemId;
        
        response = "<color=green>Успешно!</color>";
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}
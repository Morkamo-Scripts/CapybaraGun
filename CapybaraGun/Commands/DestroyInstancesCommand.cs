using System;
using System.Linq;
using CapybaraGun.Extensions;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.CustomItems.API.Features;
using Exiled.Permissions.Extensions;
using RemoteAdmin;
using UnityEngine;

namespace CapybaraGun.Commands;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class DestroyInstancesCommand : ICommand
{
    public string Command { get; } = "cgtdestroy";
    public string[] Aliases { get; } = [];
    public string Description { get; } = "Удаляет все существующие экземпляры CapybaraGun!";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (sender is not PlayerCommandSender || !sender.CheckPermission("ac.cgtdestroy"))
        {
            string requestPermission = "Требуется разрешение - 'ac.cgtdestroy'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }

        var count = 0;

        foreach (var player in Player.List)
        {
            foreach (var item in player.Items.ToList())
            {
                if (CustomItem.Get(InstanceConfig().CapybaraGun.Id)?.Check(item) == true)
                {
                    item.Destroy();
                    count++;
                }
            }
        }

        foreach (var pickup in Pickup.List)
        {
            if (CustomItem.Get(InstanceConfig().CapybaraGun.Id)?.Check(pickup) == true)
            {
                pickup.Destroy();
                count++;
            }
        }
        
        response = $"<color=green>Успешно уничтожено {count}!</color>";
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}
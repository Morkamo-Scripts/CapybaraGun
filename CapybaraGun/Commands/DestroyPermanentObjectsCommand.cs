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
using Mirror;
using RemoteAdmin;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CapybaraGun.Commands;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class DestroyPermanentObjectsCommand : ICommand
{
    public string Command { get; } = "cgtdb";
    public string[] Aliases { get; } = [];
    public string Description { get; } = "Удаляет все постоянные созданные снаряды с помощью CapybaraGun!";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (sender is not PlayerCommandSender || !sender.CheckPermission("ac.cgtdb"))
        {
            string requestPermission = "Требуется разрешение - 'ac.cgtdb'";
            
            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }

        var count = 0;

        foreach (var obj in Plugin.PermanentObjects)
        {
            Object.Destroy(obj);
            count++;
        }
        
        Plugin.PermanentObjects.Clear();
        
        response = $"<color=green>Успешно уничтожено {count}!</color>";
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}
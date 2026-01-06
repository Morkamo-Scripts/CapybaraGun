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
public class BulletDestroyDelayCommand : ICommand
{
    public string Command { get; } = "cgtdd";
    public string[] Aliases { get; } = [];
    public string Description { get; } = "Задаёт время задержки перед удалением снаряда для CapybaraGun (по умолчанию 10)";
    
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

        if (!float.TryParse(arguments.At(0), out var delayInSeconds))
        {
            response = "<color=red>Некорректное значение времени (введите кол-во секунд)!</color>";
            return false;
        }

        if (delayInSeconds < 0 || delayInSeconds > 120)
        {
            delayInSeconds = Mathf.Clamp(delayInSeconds, 0f, 120f);
        }
        
        sender.AsPlayer().CapybaraGunProperties().PlayerProps.BulletForce = delayInSeconds;
        response = $"<color=green>Успешно установлено значение {delayInSeconds:F1}!</color>";
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}
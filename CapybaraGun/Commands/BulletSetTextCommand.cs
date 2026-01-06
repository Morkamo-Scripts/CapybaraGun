using System;
using CapybaraGun.Extensions;
using CommandSystem;
using Exiled.Permissions.Extensions;
using RemoteAdmin;

namespace CapybaraGun.Commands;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class BulletSetTextCommand : ICommand
{
    public string Command { get; } = "cgtst";
    public string[] Aliases { get; } = [];
    public string Description { get; } = "Редактирует строку текста";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (sender is not PlayerCommandSender || !sender.CheckPermission("ac.cgtst"))
        {
            string requestPermission = "Требуется разрешение - 'ac.cgtst'";

            if (InstanceConfig().Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";

            return false;
        }

        sender.AsPlayer().CapybaraGunProperties().PlayerProps
            .MessageText = arguments.Count > 0
            ? string.Join(" ", arguments)
            : string.Empty;

        response = "<color=green>Успешно!</color>";
        return true;
    }
    
    private static Config InstanceConfig()
    {
        return Plugin.Instance.Config;
    }
}
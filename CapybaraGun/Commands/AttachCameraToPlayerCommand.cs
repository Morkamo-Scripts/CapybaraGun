using System;
using System.Linq;
using CapybaraGun.Extensions;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.API.Features.Toys;
using Exiled.Permissions.Extensions;
using PlayerRoles;
using RemoteAdmin;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CapybaraGun.Commands;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class AttachCameraToPlayerCommand : ICommand
{
    public string Command { get; } = "attachCamera";
    public string[] Aliases { get; } = [];
    public string Description { get; } = "Прикрепляет камеру SCP-079 к игроку.";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (sender is not PlayerCommandSender || !sender.CheckPermission("ac.attachCamera"))
        {
            string requestPermission = "Требуется разрешение - 'ac.attachCamera'";
            
            if (Plugin.Instance.Config.Debug)
                response = $"<color=red>Вы не имеете права использовать данную команду!</color>\n" +
                           $"<color=orange>[{requestPermission}]</color>";
            else
                response = "<color=red>Вы не имеете права использовать данную команду!</color>";
            
            return false;
        }

        if (arguments.Count < 2)
        {
            response = "<color=red>Синтаксис: attachCamera [id] [1/0]</color>";
            return false;
        }

        if (!ushort.TryParse(arguments.At(0), out var playerId))
        {
            response = "<color=red>Некорректное значение id игрока!</color>";
            return false;
        }
        
        if (!ushort.TryParse(arguments.At(1), out var state))
        {
            response = "<color=red>Некорректное значение! Если 0 то убрать камеру, если другое число, то закрепить новую.</color>";
            return false;
        }

        if (state == 0)
        {
            var scp079 = Player.List.FirstOrDefault(pl => pl.Role == RoleTypeId.Scp079);
            if (scp079 != null)
                scp079.Role.As<Scp079Role>().LoseSignal(5f);
            
            Plugin.AttachedCameras.TryGetValue(sender.AsPlayer(), out var camera);
            Object.Destroy(camera);
            
            Plugin.AttachedCameras.Remove(sender.AsPlayer());
        }
        else
        {
            if (Plugin.AttachedCameras.ContainsKey(sender.AsPlayer()))
            {
                response = "<color=red>У игрока уже есть камера.</color>";
                return false;
            }

            var player = Player.Get(playerId);
            var cam = player.CameraTransform;

            var cameraToy = PrefabHelper.Spawn(
                PrefabType.LczCameraToy,
                cam.position,
                cam.rotation
            );

            var follow = cameraToy.gameObject.AddComponent<FollowPlayerCamera>();
            follow.target = cam;

            Plugin.AttachedCameras.Add(player, cameraToy);
        }
        
        response = $"<color=green>Успешно!</color>";
        return true;
    }
}

public class FollowPlayerCamera : MonoBehaviour
{
    public Transform target;

    public Vector3 localOffset = new Vector3(0f, 0.5f, -1.2f);

    void LateUpdate()
    {
        if (target == null)
            return;
        
        Quaternion yawRotation = Quaternion.Euler(0f, target.eulerAngles.y, 0f);

        transform.position = target.position + yawRotation * localOffset;
        transform.rotation = yawRotation;
    }
}
namespace Points.Commands
{
    using System;

    using CommandSystem;

    using DataTypes;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    using Internal;

    using UnityEngine;

    internal sealed class Add : ICommand
    {
        private Add()
        {
        }

        public static Add Instance { get; } = new Add();

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (PointEditor.CurrentLoadedPointList == null)
            {
                response = "Load/Create a list first!";
                return false;
            }

            var name = arguments.Count > 0 ? arguments.At(0).ToLowerInvariant() : string.Empty;
            Player player = Player.Get((CommandSender)sender);

            Vector3 forward = player.CameraTransform.forward;
            Vector3 position;
            if (PointEditor.UseCrossHair)
            {
                if (Physics.Raycast(player.CameraTransform.position + forward, forward, out RaycastHit hit, 100f))
                {
                    position = hit.point + Vector3.up * 0.1f;
                }
                else
                {
                    response = "Couldn't find a suitable place to place a point!";
                    return false;
                }
            }
            else
            {
                position = player.Position + Vector3.up * 0.1f;
            }
            
            var rotation = new Vector3(-forward.x, forward.y, -forward.z);
            
            Room closestRoom = player.CurrentRoom;
            RoomType roomName = closestRoom.Type;

            PointEditor.CurrentLoadedPointList.RawPoints.Add(new RawPoint(name, roomName,
                closestRoom.Transform.InverseTransformPoint(position),
                closestRoom.Transform.InverseTransformDirection(rotation)));
            response =
                $"Created point! Room: [{roomName}] ID: [{name}]";
            return true;
        }

        public string Command { get; } = "add";
        public string[] Aliases { get; } = { "a" };
        public string Description { get; } = "Creates a Point. Arg: ID without quotes. (Optional)";
    }
}


using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using System;

namespace ActiveDrop.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class Suicide : ICommand
    {
        public string Command { get; } = "suicide";

        public string[] Aliases { get; } = {"killme"};

        public string Description { get; } = "Simple command to kill your character without special requirements";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if(sender is PlayerCommandSender player)
            {
                response = $"Killing { player.Nickname }";
                Player actualPlayer = Player.Get(player.SenderId);
                actualPlayer.Kill("Suicide");
                return true;
            }

            response = "Cannot execute this command on server!";
            return false;
        }
    }
}

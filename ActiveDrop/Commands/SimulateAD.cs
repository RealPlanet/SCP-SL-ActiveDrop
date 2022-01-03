
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using System;

namespace ActiveDrop.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class SimulateAD : ICommand
    {
        public string Command { get; } = "ActiveDropMe";

        public string[] Aliases { get; } = {"ADMe"};

        public string Description { get; } = "Execute ActiveDrop logic without death, for testing purposes";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if(sender is PlayerCommandSender player)
            {
                response = $"Attempting to drop items";
                Player actualPlayer = Player.Get(player.SenderId);
                var Test = new Handlers.PlayerHandler();
                Test.TestingEntryPoint(actualPlayer);

                return true;
            }

            response = "Cannot execute this command on server!";
            return false;
        }
    }
}

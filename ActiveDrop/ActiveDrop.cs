using Exiled.API.Enums;
using Exiled.API.Features;
using System;

using Player = Exiled.Events.Handlers.Player;

namespace ActiveDrop
{
    public class ActiveDrop : Plugin<Config>
    {
        //private static readonly Lazy<ActiveDrop> LazyInstance = new Lazy<ActiveDrop>( () => new ActiveDrop() );
        #region DefaultData
        public static ActiveDrop Instance { get; private set; } = new ActiveDrop();
        public override string Author => "Planet";
        public override string Name => "Active Drop";
        public override string Prefix => "AD";
        public override Version Version => new Version(2, 0, 1);
        public override Version RequiredExiledVersion => new Version(4, 2, 2);
        public override PluginPriority Priority => PluginPriority.Default;
        #endregion

        private Handlers.PlayerHandler player;
        private ActiveDrop(){}

        public override void OnEnabled()
        {
            player = new Handlers.PlayerHandler();
            RegisterEvents();
        }

        public override void OnDisabled()
        {
            UnregisterEvents();
            player = null;
        }

        public void RegisterEvents()
        {  
            Player.Dying += player.OnPlayerNearDeath;
        }

        public void UnregisterEvents()
        {
            Player.Dying -= player.OnPlayerNearDeath; 
        }
    }
}

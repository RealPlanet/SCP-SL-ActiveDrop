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
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredExiledVersion => new Version(2, 11, 1);
        public override PluginPriority Priority => PluginPriority.Default;
        #endregion

        private Handlers.Player player;
        private ActiveDrop(){}

        public override void OnEnabled()
        {
            player = new Handlers.Player();
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

using Exiled.API.Interfaces;
using System.ComponentModel;

namespace ActiveDrop
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        [Description("Allow multiple items to drop on the same death?")]
        public bool DropMultiple { get; set; } = true;

        [Description("Allow random variations of the grenade fuse timer")]
        public bool RandomVariation { get; set; } = true;

        [Description("Sets the probability for a FragGrenade to automatically drop on holder death")]
        public int GrenadeFragChance { get; set; } = 50;

        [Description("Sets the probability for a FlashGrenade to automatically drop on holder death")]
        public int GrenadeFlashChance { get; set; } = 50;

        [Description("Sets the probability for SCP018 to automatically drop on holder death")]
        public int SCP018Chance { get; set; } = 50;
    }
}

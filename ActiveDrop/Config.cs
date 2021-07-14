using Exiled.API.Interfaces;
using System.ComponentModel;

namespace ActiveDrop
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        [Description("Allow multiple items to drop on the same death?")]
        public bool DropMultiple { get; set; } = true;

        [Description("Sets the probability for a FragGrenade to automatically drop on holder death")]
        public int GrenadeFragChance { get; set; } = 100;

        [Description("Sets the probability for a FlashGrenade to automatically drop on holder death")]
        public int GrenadeFlashChance { get; set; } = 100;

        [Description("Sets the probability for SCP018 to automatically drop on holder death")]
        public int SCP018Chance { get; set; } = 100;

        public float FragFuse { get; set; } = 3f;
        public float FlashFuse { get; set; } = 1f;

        public float SCP018Duration { get; set; } = 5f;
    }
}

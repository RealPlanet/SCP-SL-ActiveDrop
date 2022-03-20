using Exiled.API.Features;
using System.Linq;
using Exiled.Events.EventArgs;
using UnityEngine;
using Mirror;
using Exiled.API.Features.Items;
using Exiled.API.Enums;

namespace ActiveDrop.Handlers
{
    class PlayerHandler
    {
        private bool HasDropped = false;

        public void OnPlayerNearDeath(DyingEventArgs ev) => ExecuteGrenadeDropFor(ev.Target);
        public void TestingEntryPoint(Player Target) => ExecuteGrenadeDropFor(Target);

        private void ExecuteGrenadeDropFor(Exiled.API.Features.Player Target)
        {
            int FlashChance = ActiveDrop.Instance.Config.GrenadeFlashChance;
            int FragChance = ActiveDrop.Instance.Config.GrenadeFragChance;

            int FlashCount = Target.CountItem(ItemType.GrenadeFlash);
            int FragCount = Target.CountItem(ItemType.GrenadeHE);

            HasDropped = false;
            int DiceRoll = RandomGenerator.GetInt16(0, 99);
            if (DiceRoll < FlashChance && FlashCount > 0)
            {
                DropGrenade<FlashGrenade>(Target, FlashCount, ItemType.GrenadeFlash);
            }

            DiceRoll = RandomGenerator.GetInt16(0, 99);
            if (DiceRoll < FragChance && FragCount > 0 && CanDrop())
            {
                DropGrenade<ExplosiveGrenade>(Target, FlashCount, ItemType.GrenadeHE);
            }
        }

        private void DropGrenade<T>(Exiled.API.Features.Player Target, int Count, ItemType ItemType) where T : Item
        {
            T GrenadeItem = Target.Items.FirstOrDefault(x => x.Type == ItemType) as T;

            if (GrenadeItem == null)
            {
                Log.Error($"Attempted to drop flash grenade from dead player, but player had no flash grenade, Count is {Count}");
                return;
            }

            // Spawn the grenade only if removing the item was successful
            if (Target.RemoveItem(GrenadeItem))
            {
                if(GrenadeItem is FlashGrenade)
                {
                    var Explosive = GrenadeItem as FlashGrenade;
                    Explosive.FuseTime = CalculateFuse(ActiveDrop.Instance.Config.FlashFuse);
                    Explosive.SpawnActive(Target.Position + (Vector3.up * ActiveDrop.Instance.Config.GrenadeSpawnVerticalOffset), Target);
                }
                else if(GrenadeItem is ExplosiveGrenade)
                {
                    var Explosive = GrenadeItem as ExplosiveGrenade;
                    Explosive.FuseTime = CalculateFuse(ActiveDrop.Instance.Config.FragFuse);
                    Explosive.SpawnActive(Target.Position + (Vector3.up * ActiveDrop.Instance.Config.GrenadeSpawnVerticalOffset), Target);
                }

                HasDropped = true;
            }
        }

        private bool CanDrop()
        {
            return ActiveDrop.Instance.Config.DropMultiple || !HasDropped;
        }

        private float CalculateFuse(float timer)
        {
            if(ActiveDrop.Instance.Config.RandomVariation)
            {
                float variation = ActiveDrop.Instance.Config.FuseVariation;
                timer += RandomGenerator.GetFloat(-variation, variation);
                if(timer < 0)
                {
                    timer = 0f;
                }
            }
            return timer;
        }
    }
}

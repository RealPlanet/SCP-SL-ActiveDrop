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
                DropFlash(Target, FlashCount);
            }

            DiceRoll = RandomGenerator.GetInt16(0, 99);
            if (DiceRoll < FragChance && FragCount > 0 && CanDrop())
            {
                DropFrag(Target, FragCount);
            }
        }

        private void DropFlash(Exiled.API.Features.Player Target, int Count)
        {
            var GrenadeFlashItem = Target.Items.FirstOrDefault(x => x.Type == ItemType.GrenadeFlash);
            float GrenadeFuse = CalculateFuse(ActiveDrop.Instance.Config.FlashFuse);

            if (GrenadeFlashItem == null)
            {
                Log.Error($"Attempted to drop flash grenade from dead player, but player had no flash grenade, Count is {Count}");
                return;
            }

            // Spawn the grenade only if removing the item was successful
            if (Target.RemoveItem(GrenadeFlashItem))
            {
                var Grenade = new FlashGrenade(ItemType.GrenadeFlash, Target);
                Grenade.FuseTime = GrenadeFuse;
                Grenade.SpawnActive(Target.Position + (Vector3.up * ActiveDrop.Instance.Config.GrenadeSpawnVerticalOffset));
                HasDropped = true;
            }
        }

        private void DropFrag(Exiled.API.Features.Player Target, int Count)
        {
            var GrenadeFragItem = Target.Items.FirstOrDefault(x => x.Type == ItemType.GrenadeHE);
            float GrenadeFuse = CalculateFuse(ActiveDrop.Instance.Config.FragFuse);

            if (GrenadeFragItem == null)
            {
                Log.Error($"Attempted to drop explosive grenade from dead player, but player had no explosive grenade, Count is {Count}");
                return;
            }

            // Spawn the grenade only if removing the item was successful
            if (Target.RemoveItem(GrenadeFragItem))
            {
                var Grenade = new ExplosiveGrenade(ItemType.GrenadeHE, Target);
                Grenade.FuseTime = GrenadeFuse;
                Grenade.SpawnActive(Target.Position + (Vector3.up * 0.25f));
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

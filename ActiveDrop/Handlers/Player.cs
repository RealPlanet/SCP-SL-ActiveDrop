using Exiled.API.Features;
using System.Linq;
using Exiled.Events.EventArgs;
using UnityEngine;
using Mirror;
using Exiled.API.Features.Items;
using Exiled.API.Enums;

namespace ActiveDrop.Handlers
{
    class Player
    {
        private bool hasDropped = false;

        public void OnPlayerNearDeath(DyingEventArgs ev)
        {
            int FlashChance = ActiveDrop.Instance.Config.GrenadeFlashChance;
            int FragChance = ActiveDrop.Instance.Config.GrenadeFragChance;
            int SCP018Chance = ActiveDrop.Instance.Config.SCP018Chance;

            int FlashCount = ev.Target.CountItem(ItemType.GrenadeFlash);
            int FragCount = ev.Target.CountItem(ItemType.GrenadeHE);
            int SCP018Count = ev.Target.CountItem(ItemType.SCP018);

            hasDropped = false;
            int DiceRoll = RandomGenerator.GetInt16(0, 99);
            if (DiceRoll < FlashChance && FlashCount > 0)
            {
                DropFlash(ev);
            }

            DiceRoll = RandomGenerator.GetInt16(0, 99);
            if (DiceRoll < FragChance && FragCount > 0 && CanDrop())
            {
                DropFrag(ev);
            }

            DiceRoll = RandomGenerator.GetInt16(0, 99);
            if (DiceRoll < SCP018Chance && SCP018Count > 0 && CanDrop())
            {
                DropSCP018(ev);
            }
        }

        private void DropFlash(DyingEventArgs ev)
        {
            var grenadeFlashItem = ev.Target.Items.FirstOrDefault(x => x.Type == ItemType.GrenadeFlash);
            float grenadeFuse = CalculateFuse(ActiveDrop.Instance.Config.FlashFuse);

            if (grenadeFlashItem == null)
            {
                Log.Error("Attempted to drop flash grenade from dead player, but player had no flash grenade");
                return;
            }

            ev.Target.RemoveItem(grenadeFlashItem);
            ev.Target.ThrowGrenade(GrenadeType.Flashbang);

            hasDropped = true;
        }

        private void DropFrag(DyingEventArgs ev)
        {
            var grenadeFragItem = ev.Target.Items.FirstOrDefault(x => x.Type == ItemType.GrenadeFlash);
            float grenadeFuse = CalculateFuse(ActiveDrop.Instance.Config.FragFuse);

            if (grenadeFragItem == null)
            {
                Log.Error("Attempted to drop flash grenade from dead player, but player had no flash grenade");
                return;
            }

            ev.Target.RemoveItem(grenadeFragItem);
            ev.Target.ThrowGrenade(GrenadeType.FragGrenade);

            hasDropped = true;
        }

        private void DropSCP018(DyingEventArgs ev)
        {
            var SCPItem = ev.Target.Items.FirstOrDefault(x => x.Type == ItemType.SCP018);
            if (SCPItem == null)
            {
                Log.Error("Attempted to drop flash grenade from dead player, but player had no flash grenade");
                return;
            }

            ev.Target.RemoveItem(SCPItem);
            ev.Target.ThrowGrenade(GrenadeType.Scp018);
            hasDropped = true;
        }

        private bool CanDrop()
        {
            return ActiveDrop.Instance.Config.DropMultiple || !hasDropped;
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

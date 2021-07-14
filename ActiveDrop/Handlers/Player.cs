using Exiled.API.Features;
using System.Linq;
using Exiled.Events.EventArgs;
using Grenades;
using UnityEngine;
using Mirror;

namespace ActiveDrop.Handlers
{
    class Player
    {
        private bool hasDropped = false;

        public GrenadeManager GrenadeManager { get; private set; }

        public void OnPlayerNearDeath(DyingEventArgs ev)
        {
            int FlashChance = ActiveDrop.Instance.Config.GrenadeFlashChance;
            int FragChance = ActiveDrop.Instance.Config.GrenadeFragChance;
            int SCP018Chance = ActiveDrop.Instance.Config.SCP018Chance;

            int FlashCount = ev.Target.CountItem(ItemType.GrenadeFlash);
            int FragCount = ev.Target.CountItem(ItemType.GrenadeFrag);
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
            var grenadeFlashItem = ev.Target.Items.FirstOrDefault(x => x.id == ItemType.GrenadeFlash);
            float grenadeFuse = ActiveDrop.Instance.Config.FlashFuse;

            if (grenadeFlashItem == null)
            {
                Log.Error("Attempted to drop flash grenade from dead player, but player had no flash grenade");
                return;
            }

            ev.Target.RemoveItem(grenadeFlashItem);
            SpawnObjectOnPlayer(grenadeFuse, ItemType.GrenadeFlash, ev.Target);

            hasDropped = true;
        }

        private void DropFrag(DyingEventArgs ev)
        {
            var grenadeFragItem = ev.Target.Items.FirstOrDefault(x => x.id == ItemType.GrenadeFlash);
            float grenadeFuse = ActiveDrop.Instance.Config.FragFuse;
            if (grenadeFragItem == null)
            {
                Log.Error("Attempted to drop flash grenade from dead player, but player had no flash grenade");
                return;
            }

            ev.Target.RemoveItem(grenadeFragItem);
            SpawnObjectOnPlayer(grenadeFuse, ItemType.GrenadeFrag, ev.Target);

            hasDropped = true;
        }

        private void DropSCP018(DyingEventArgs ev)
        {
            var SCPItem = ev.Target.Items.FirstOrDefault(x => x.id == ItemType.SCP018);
            float grenadeFuse = ActiveDrop.Instance.Config.SCP018Duration;

            if (SCPItem == null)
            {
                Log.Error("Attempted to drop flash grenade from dead player, but player had no flash grenade");
                return;
            }

            ev.Target.RemoveItem(SCPItem);
            SpawnObjectOnPlayer(grenadeFuse, ItemType.SCP018, ev.Target);
            hasDropped = true;
        }

        private bool CanDrop()
        {
            return ActiveDrop.Instance.Config.DropMultiple || !hasDropped;
        }

        public Grenade SpawnObjectOnPlayer(float fuseTime, ItemType grenadeType, Exiled.API.Features.Player player)
        {
            GrenadeSettings settings = player.GrenadeManager.availableGrenades.FirstOrDefault(g => g.inventoryID == grenadeType);

            Vector3 position = player.CameraTransform.position;
            Grenade grenade = GameObject.Instantiate(settings.grenadeInstance, position, Quaternion.identity).GetComponent<Grenade>();

            
            grenade.NetworkfuseTime = NetworkTime.time + fuseTime;

            //Tracked.Add(grenade.gameObject);

            Log.Debug($"Spawning grenade at {position} because of ActiveDrop Plugin. GrenadeType is {grenade.name}");
            NetworkServer.Spawn(grenade.gameObject);

            return grenade;
        }
    }
}

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
            float grenadeFuse = CalculateFuse(ActiveDrop.Instance.Config.FlashFuse);

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
            float grenadeFuse = CalculateFuse(ActiveDrop.Instance.Config.FragFuse);

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
            if (SCPItem == null)
            {
                Log.Error("Attempted to drop flash grenade from dead player, but player had no flash grenade");
                return;
            }

            ev.Target.RemoveItem(SCPItem);
            SpawnObjectOnPlayer(0f, ItemType.SCP018, ev.Target);
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
        private Grenade SpawnObjectOnPlayer(float fuseTime, ItemType grenadeType, Exiled.API.Features.Player player)
        {
            GrenadeSettings settings = player.GrenadeManager.availableGrenades.FirstOrDefault(g => g.inventoryID == grenadeType);
            
            Vector3 position = player.CameraTransform.position;

            // Init grenade data
            Grenade grenade = null;
            if (grenadeType != ItemType.SCP018)
            {
                grenade = GameObject.Instantiate(settings.grenadeInstance).GetComponent<Grenade>();  
                grenade.fuseDuration = fuseTime;
                grenade.NetworkfuseTime = NetworkTime.time + fuseTime;
            }
            else
            {
                grenade = GameObject.Instantiate(settings.grenadeInstance).GetComponent<Scp018Grenade>();
                grenade.InitData(player.GrenadeManager, position, Vector3.zero);
            }

            grenade.InitData(player.GrenadeManager, position, Vector3.zero);
            grenade.throwerTeam = player.Team;
            grenade.NetworkthrowerTeam = player.Team;

            Log.Debug($"Spawning grenade at {position} because of ActiveDrop Plugin. GrenadeType is {grenade.name}");
            NetworkServer.Spawn(grenade.gameObject);

            return grenade;
        }
    }
}

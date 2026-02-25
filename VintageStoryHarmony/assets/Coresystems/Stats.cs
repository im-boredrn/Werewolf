using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using VintageStoryHarmony;
using WereWolf.assets.Werewolf.Configuration;

namespace WereWolf.assets.Coresystems
{
    internal class Stats
    {

        public static void ApplyStats(EntityPlayer player, PlayerData.Forms form)
        {

            var stat = player.Stats;

            //save old max before applying
            float oldMax = stat.GetBlended("maxhealthExtraPoints");

            if (form == PlayerData.Forms.WereWolf) // all stats run fine except for maxhealth, its weird rn
            {
                //Movement

                stat.Set("walkspeed", "werewolfmod", WereWolfModSettings.Speed, true);
                stat.Set("jumpHeightMul", "werewolfmod", WereWolfModSettings.Jump, true);


                // Core Stats
                stat.Set("maxhealthExtraPoints", "werewolfmod", WereWolfModSettings.MaxHealth, true);
                stat.Set("healingeffectivness", "werewolfmod", WereWolfModSettings.HealingEffectivness, true);
                stat.Set("rangedWeaponsAcc", "werewolfmod", WereWolfModSettings.RangedAcc, true);



                //Wolf Abilities

                stat.Set("forageDropRate", "werewolfmod", WereWolfModSettings.ForageDropRate, true);
                stat.Set("wildCropDropRate", "werewolfmod", WereWolfModSettings.WildCropDropRate, true);
                stat.Set("animalSeekingRange", "werewolfmod", WereWolfModSettings.AnimalSeekingRange, true);
                stat.Set("animalLootDropRate", "werewolfmod", WereWolfModSettings.AnimalLootDropRate, true);
                stat.Set("animalHarvestingTime", "werewolfmod", WereWolfModSettings.AnimalHarvestingTime, true);
                stat.Set("bowDrawingStrength", "werewolfmod", WereWolfModSettings.BowDrawingStrength, true);


            }
            else if (form == PlayerData.Forms.VulpisHuman)
            {
                //Movement

                stat.Remove("walkspeed", "werewolfmod");
                stat.Remove("jumpHeightMul", "werewolfmod");


                // Core Stats
                stat.Remove("maxhealthExtraPoints", "werewolfmod");
                stat.Remove("healingeffectivness", "werewolfmod");
                stat.Remove("rangedWeaponsAcc", "werewolfmod");



                //Wolf Abilities

                stat.Remove("forageDropRate", "werewolfmod");
                stat.Remove("wildCropDropRate", "werewolfmod");
                stat.Remove("animalSeekingRange", "werewolfmod");
                stat.Remove("animalLootDropRate", "werewolfmod");
                stat.Remove("animalHarvestingTime", "werewolfmod");
                stat.Remove("bowDrawingStrength", "werewolfmod");
            }

            // Health behavior: scale current health based on new max
            var healthBehavior = player.GetBehavior<EntityBehaviorHealth>(); // currerntly not working, or even calling as far as im concened
            if (healthBehavior != null)
            {
                float newMax = stat.GetBlended("maxhealthExtraPoints");
                float oldHealth = healthBehavior.Health;

                float oldPercent = oldMax > 0 ? healthBehavior.Health / oldMax : 1f;
              //  changes health
                healthBehavior.Health = Math.Min(newMax, newMax * oldPercent);

                healthBehavior.MarkDirty();
            }
        }

        // call this per tick to apply regen should be done automatically
        public static void ApplyRegen(EntityPlayer entity, PlayerData.Forms form) // currerntly not working, or even calling as far as im concened
        {
            bool night = WolfTime.isNight(entity);
            var healthBehavior = entity?.GetBehavior<EntityBehaviorHealth>();

            if (healthBehavior == null) return;

            // Only apply regen if WereWolf form
            if (form == PlayerData.Forms.WereWolf)
            {
                float regenAmount = night ? WereWolfModSettings.NightRegen : WereWolfModSettings.DayRegen;
                healthBehavior.Health = Math.Min(healthBehavior.MaxHealth, healthBehavior.Health + regenAmount);
                healthBehavior.MarkDirty();
                entity?.World.Logger.Warning($"Regen check | Form: {form} | Night: {night}");
            }
            entity?.World.Logger.Warning($"Regen check | Form: {form} | Night: {night}");

        }
    }
}

        
            

        
    


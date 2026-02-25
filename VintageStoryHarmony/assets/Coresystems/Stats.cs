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

            var healthBehavior = player.GetBehavior<EntityBehaviorHealth>();

            // Save old max health (total) to scale properly
            float oldMax = healthBehavior?.MaxHealth ?? 20f; // fallback to default health
            float oldHealth = healthBehavior?.Health ?? oldMax;


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


            // Update health to match new max
            if (healthBehavior != null)
            {
                float newMax = healthBehavior.MaxHealth;
                float oldPercent = oldMax > 0 ? oldHealth / oldMax : 1f;
                healthBehavior.Health = Math.Min(newMax, newMax * oldPercent);
                healthBehavior.MarkDirty();
            }
        }

        // Apply regen per tick

        public static float GetRegenAmount(EntityPlayer player, PlayerData.Forms form)
        {
            if (form != PlayerData.Forms.WereWolf) return 0f;

            bool night = WolfTime.isNight(player);
            return night ? WereWolfModSettings.NightRegen : WereWolfModSettings.DayRegen;
        }

        public static void ApplyRegen(EntityPlayer player, float regenAmount)
        {
            if (player == null ||  regenAmount <= 0f) return;

            var healthBehavior = player.GetBehavior<EntityBehaviorHealth>();
            if (healthBehavior == null) return;

            bool night = WolfTime.isNight(player);
            

            healthBehavior.Health = Math.Min(
          healthBehavior.MaxHealth,
          healthBehavior.Health + regenAmount
      );
            healthBehavior.MarkDirty();

            }
        }
    }


        
            

        
    


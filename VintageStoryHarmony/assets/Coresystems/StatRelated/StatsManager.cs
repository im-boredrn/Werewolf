using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Vintagestory;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using VintageStoryHarmony;
using WereWolf.assets.Werewolf.Configuration;
using static WereWolf.assets.Coresystems.PlayerData;

namespace WereWolf.assets.Coresystems.StatRelated
{
    internal class StatsManager
    {
      private  const string StatKey = "StatsForm"; // could later make individual form keys like BearStatsKey
       
      
        public static void ApplyStats(EntityPlayer player, PlayerData.Forms form)
        {
            player.World.Logger.Warning($"[FLOW] ApplyStats CALLED. Form = {PlayerData.GetForm(player)}");
            player.World.Logger.Warning($"[DATA] ApplyStats running on side: {player.World.Side}");
            var stats = player.Stats;
            var currentStoredForm = GetStoredForm(player);
            var healthBehavior = player.GetBehavior<EntityBehaviorHealth>();


            // Save old max health (total) to scale properly
            float oldMax = healthBehavior?.MaxHealth ?? 20f; // fallback to default health
            float oldHealth = healthBehavior?.Health ?? oldMax;
            //  Remove old stats if the form changed
            if (currentStoredForm != form)
            {
                RemoveStats(player);
            }

            switch (form)
            {
                case PlayerData.Forms.WereWolf:
                    stats.Set("walkspeed", StatKey, WereWolfModSettings.Speed, true);
                    stats.Set("jumpHeightMul", StatKey, WereWolfModSettings.Jump, true);
                    stats.Set("maxhealthExtraPoints", StatKey, WereWolfModSettings.MaxHealth, true);
                    stats.Set("healingeffectivness", StatKey, WereWolfModSettings.HealingEffectivness, true);
                    stats.Set("rangedWeaponsAcc", StatKey, WereWolfModSettings.RangedAcc, true);

                    // Wolf abilities
                    stats.Set("forageDropRate", StatKey, WereWolfModSettings.ForageDropRate, true);
                    stats.Set("wildCropDropRate", StatKey, WereWolfModSettings.WildCropDropRate, true);
                    stats.Set("animalSeekingRange", StatKey, WereWolfModSettings.AnimalSeekingRange, true);
                    stats.Set("animalLootDropRate", StatKey, WereWolfModSettings.AnimalLootDropRate, true);
                    stats.Set("animalHarvestingTime", StatKey, WereWolfModSettings.AnimalHarvestingTime, true);
                    stats.Set("bowDrawingStrength", StatKey, WereWolfModSettings.BowDrawingStrength, true);
                    if (healthBehavior != null)
                    {
                        player.World.Logger.Warning($"[FLOW] Updating Health. Form = {PlayerData.GetForm(player)}");

                        float newMax = healthBehavior.MaxHealth;
                        float oldPercent = oldMax > 0 ? oldHealth / oldMax : 1f;
                        healthBehavior.Health = Math.Min(newMax, newMax * oldPercent);
                        healthBehavior.MarkDirty();
                    }
                    break;

                case PlayerData.Forms.VulpisHuman:
                    stats.Set("walkspeed", StatKey, WereWolfModSettings.Speed * 0.5f, true);
                    stats.Set("jumpHeightMul", StatKey, WereWolfModSettings.Jump * 0.5f, true);
                    stats.Set("maxhealthExtraPoints", StatKey, WereWolfModSettings.MaxHealth * 0.5f, true);
                    stats.Set("healingeffectivness", StatKey, WereWolfModSettings.HealingEffectivness * 0.5f, true);
                    stats.Set("rangedWeaponsAcc", StatKey, WereWolfModSettings.RangedAcc * 0.5f, true);

                    // VulpisHuman abilities
                    stats.Set("forageDropRate", StatKey, WereWolfModSettings.ForageDropRate * 0.5f, true);
                    stats.Set("wildCropDropRate", StatKey, WereWolfModSettings.WildCropDropRate * 0.5f, true);
                    stats.Set("animalSeekingRange", StatKey, WereWolfModSettings.AnimalSeekingRange * 0.5f, true);
                    stats.Set("animalLootDropRate", StatKey, WereWolfModSettings.AnimalLootDropRate * 0.5f, true);
                    stats.Set("animalHarvestingTime", StatKey, WereWolfModSettings.AnimalHarvestingTime * 0.5f, true);
                    stats.Set("bowDrawingStrength", StatKey, WereWolfModSettings.BowDrawingStrength * 0.5f, true);

                  if (healthBehavior != null)
                    {
                        player.World.Logger.Warning($"[FLOW] Updating Health. Form = {PlayerData.GetForm(player)}");

                        float newMax = healthBehavior.MaxHealth;
                       float oldPercent = oldMax > 0 ? oldHealth / oldMax : 1f;
                    healthBehavior.Health = Math.Min(newMax, newMax * oldPercent);
                      healthBehavior.MarkDirty();
                    }

                        break;

                case PlayerData.Forms.UnchangedHuman:
                default:
                    RemoveStats(player);
                    break;
            }


            // Save the applied Form in watched Attributes
            SetStoredForm(player, form);
        }

        // Remove any stats applied under this key
        private static void RemoveStats(EntityPlayer player)
        {
            var stat = player.Stats;
            stat.Remove("walkspeed", StatKey);
            stat.Remove("jumpHeightMul", StatKey);


            // Core Stats
            stat.Remove("maxhealthExtraPoints", StatKey);
            stat.Remove("healingeffectivness", StatKey);
            stat.Remove("rangedWeaponsAcc", StatKey);



            //Wolf Abilities

            stat.Remove("forageDropRate", StatKey);
            stat.Remove("wildCropDropRate", StatKey);
            stat.Remove("animalSeekingRange", StatKey);
            stat.Remove("animalLootDropRate", StatKey);
            stat.Remove("animalHarvestingTime", StatKey);
            stat.Remove("bowDrawingStrength", StatKey);
        
            // Update health to match new max
           
            }
        private static PlayerData.Forms GetStoredForm(EntityPlayer player)
        {
            string stored = player.WatchedAttributes.GetString(StatKey, PlayerData.Forms.UnchangedHuman.ToString());
            Enum.TryParse(stored, out PlayerData.Forms form);
            return form;
        }

        private static void SetStoredForm(EntityPlayer player, PlayerData.Forms form)
        {
            player.WatchedAttributes.SetString(StatKey, form.ToString());
            player.WatchedAttributes.MarkPathDirty(StatKey);
        }
    }
}
    





        
            

        
    


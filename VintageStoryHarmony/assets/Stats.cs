using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using WereWolf.assets.Werewolf.Configuration;

namespace VintageStoryHarmony.assets
{
    internal class Stats
    {

        public static void ApplyStats(EntityPlayer player, PlayerData.Forms form )
        {

            var stat = player.Stats;

            //save old max before applying
            float oldMax = stat.GetBlended("maxhealth");

            if (form == PlayerData.Forms.WereWolf)
            {
                //Movement

                stat.Set("walkspeed", "werewolfmod", WereWolfModSettings.Speed, true);
                stat.Set("jumpHeightMul", "werewolfmod", WereWolfModSettings.Jump, true);


                // Core Stats
                stat.Set("maxhealthExtraPoints", "werewolfmod", WereWolfModSettings.MaxHealth, true );
                stat.Set("healingeffectivness", "werewolfmod",WereWolfModSettings.HealingEffectivness , true);
                stat.Set("rangedWeaponsAcc", "werewolfmod",WereWolfModSettings.RangedAcc , true);



                //Wolf Abilities

                stat.Set("forageDropRate", "werewolfmod",WereWolfModSettings.ForageDropRate , true);
                stat.Set("wildCropDropRate", "werewolfmod", WereWolfModSettings.WildCropDropRate , true);
                stat.Set("animalSeekingRange", "werewolfmod", WereWolfModSettings.AnimalSeekingRange, true);
                stat.Set("animalLootDropRate", "werewolfmod", WereWolfModSettings.AnimalLootDropRate, true);
                stat.Set("animalHarvestingTime", "werewolfmod", WereWolfModSettings.AnimalHarvestingTime, true);
                stat.Set("bowDrawingStrength", "werewolfmod", WereWolfModSettings.BowDrawingStrength, true);

                
            }
            else if (form == PlayerData.Forms.Human)
            {
                //Movement

                stat.Remove("walkspeed", "werewolfmod");
                stat.Remove("jumpHeightMul", "werewolfmod" );


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


       
        }

        }
            }

        
    


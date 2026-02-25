using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using WereWolf.assets.Coresystems;
using WereWolf.assets.Werewolf;
using WereWolf.assets.Werewolf.Configuration;
using static WereWolf.assets.Coresystems.PlayerData;

namespace VintageStoryHarmony
{

    [HarmonyPatch(typeof(Entity), "ReceiveDamage")]
    public class DamagePatch
    {
        static void Prefix(Entity __instance, DamageSource damageSource, ref float damage)
        {
            if (__instance == null || damageSource == null) return;

            var attackerEntity = damageSource.SourceEntity;
            var targetEntity = __instance;

            var attackerPlayer = attackerEntity as EntityPlayer;
            var targetPlayer = targetEntity as EntityPlayer;

            // Only get form if player exists
            var attackerForm = PlayerData.Forms.VulpisHuman;
            if (attackerPlayer != null)
            {
                attackerForm = PlayerData.GetForm(attackerPlayer);
            }

            var targetForm = PlayerData.Forms.VulpisHuman;
            if (targetPlayer != null)
            {
                targetForm = PlayerData.GetForm(targetPlayer);
            }

            bool isNight = __instance.World.Calendar.HourOfDay >= 18 || __instance.World.Calendar.HourOfDay < 6;

            // 1. Fall damage only affects Werewolf players
            if (targetForm == PlayerData.Forms.WereWolf &&
                damageSource.Source == EnumDamageSource.Fall &&
                WereWolfModSettings.DisableFallDamage)
            {
                damage = 0f;
                return;
            }

            // 2. Wolf-player immunity
            string targetCode = targetEntity.Code?.ToString() ?? "unknown";
            string attackerCode = attackerEntity?.Code?.ToString() ?? "unknown";

            if ((attackerPlayer != null && targetCode == "wolf") ||
                (attackerCode == "wolf" && targetPlayer != null))
            {
                if (targetForm != Forms.WereWolf) return;
                damage = 0f;
                return;
            }

            // 3. Base multiplier
            float multiplier = 1f;




            if (attackerPlayer == null) // Non-player attacker
            {
                    if (targetForm == Forms.WereWolf) // Player is WereWolf
                    {
                        multiplier *= WereWolfModSettings.DamageReduction;
                    }
            }
            else // Player attacker
            {
                if (attackerForm == PlayerData.Forms.WereWolf)
                    multiplier *= WereWolfModSettings.Damage;
            }

            // 4. Daytime penalty
            if (!isNight) multiplier *= 0.5f;

            // 5. Apply final damage
            damage *= multiplier;

            if (attackerForm == Forms.WereWolf)
            {
           // PlaySound
            }

            // Debug
            if (attackerPlayer != null)
{
                Console.WriteLine($"Form: {attackerForm}, DamageMultiplier: {WereWolfModSettings.Damage}, FinalDamage: {damage}");
}



          

           
        }
    }
}
    


                
                    
                
            

      
        
    
   


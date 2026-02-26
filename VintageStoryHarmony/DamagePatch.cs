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

            // --- Identify target and attacker ---
            Entity targetEntity = __instance;
            Entity attackerEntity = damageSource.SourceEntity;

            // --- Determine if target is a player ---
            EntityPlayer? targetPlayer = targetEntity as EntityPlayer;
            Forms targetForm = targetPlayer != null ? PlayerData.GetForm(targetPlayer) : Forms.UnchangedHuman;

            // --- Determine if attacker is a player ---
            EntityPlayer? attackerPlayer = attackerEntity as EntityPlayer;
            Forms attackerForm = attackerPlayer != null ? PlayerData.GetForm(attackerPlayer) : Forms.UnchangedHuman;

            // --- Determine if attacker is an NPC ---
            bool attackerIsWolf = attackerEntity != null && attackerEntity.Code?.Path.Contains("wolf") == true;
            bool targetIsWolf = targetEntity != null && targetEntity.Code?.Path.Contains("wolf") == true;

            // --- Determine attacker/receiver type ---
            string attackerType = attackerPlayer != null ? "Player" :
                                  attackerIsWolf ? "WolfNPC" :
                                  "Other";

            string targetType = targetPlayer != null ? "Player" :
                                targetIsWolf ? "WolfNPC" :
                                "Other";

            bool isNight = __instance.World.Calendar.HourOfDay >= 18 || __instance.World.Calendar.HourOfDay < 6;

            // 1. Fall damage only affects Werewolf players
            if (targetForm == Forms.WereWolf && damageSource.Source == EnumDamageSource.Fall && WereWolfModSettings.DisableFallDamage)
            {
                damage = 0f;
                return;
            }



            // Wolf vs player immunity
            bool targetIsPlayer = targetPlayer != null;
            bool attackerIsPlayer = attackerPlayer != null;
            bool targetIsWereWolf = targetForm == Forms.WereWolf;
            bool targetIsVulpisHuman = targetForm == Forms.VulpisHuman;
            bool attackerIsWereWolf = attackerForm == Forms.WereWolf;
            bool attackerIsVulpis = attackerForm == Forms.VulpisHuman;


            if ((attackerIsWereWolf || attackerIsVulpis) && targetIsWolf) // If attacker wolf related and target is wolf
            {
                damage = 0f;
            }
            else if (attackerIsWolf && (targetIsVulpisHuman || targetIsWereWolf)) // If attacker is wolf and target is wolf related
            {
                damage = 0f;
            }
           
            

            // 3. Base multiplier
            float multiplier = 1f;




            if (!attackerIsPlayer && targetIsWereWolf) // Non-player attacker
            {
                    if (targetForm == Forms.WereWolf) // Player is WereWolf
                    {
                        multiplier *= WereWolfModSettings.DamageReduction;
                    }
            }
            else if (attackerIsWereWolf)              // WereWolf attacker
            {
             
                    multiplier *= WereWolfModSettings.Damage;
            }

            // 4. Daytime penalty
            if (!isNight && attackerIsWereWolf) multiplier *= 0.5f;

            // 5. Apply final damage
            damage *= multiplier;

            if (attackerIsWereWolf)
            {
           // PlaySound
            }

            // Debug
            if (attackerIsPlayer)
            {
                Console.WriteLine($"Form: {attackerForm} | DamageMultiplier: {WereWolfModSettings.Damage} | FinalDamage: {damage}");
            }
            else if (!attackerIsPlayer)
            {
                Console.WriteLine($"PlayerForm: {targetForm} | Attacker: {attackerType} | DamageReduction: {WereWolfModSettings.DamageReduction} | FinalDamage: {damage}");

            }
        }
    }
}

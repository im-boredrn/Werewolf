using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using VintageStoryHarmony;
using WereWolf.assets.Coresystems.Infections;
using WereWolf.assets.Werewolf.Configuration;
using static WereWolf.assets.Coresystems.PlayerData;

namespace WereWolf.assets.Coresystems
{
    internal class TransformationController
    {
        private static bool debugMode = false;
        public static void TrySetForm(IServerPlayer player, Forms targetForm, TransformationReason reason)
        {

            var entity = player.Entity;
            var infection = entity.GetBehavior<EntityBehaviorInfection>();
            var tree = entity.WatchedAttributes;
            entity.World.Logger.Warning($"TrySetForm called | Player: {player.PlayerName} | TargetForm: {targetForm} | Reason: {reason}");


            // 1️ Infection requirement
            if (infection == null || (infection.CurrentInfection() != EntityBehaviorInfection.Infectionstatus.Infected && reason == TransformationReason.ManualToggle))
            {
                player.SendMessage(GlobalConstants.GeneralChatGroup, "You are not infected.", EnumChatType.Notification);
                return;
            }

            // 2️ Cooldown check (if manual)

            var currentForm = PlayerData.GetForm(entity);
            entity.World.Logger.Warning($"Current form: {currentForm}");
            if (reason == TransformationReason.ManualToggle && !IsCooldownReady(entity)) return;
            
            

            // 3️ If already in form, don't reapply
            if (GetForm(entity) == targetForm) return;

            // 4️ Apply transformation
            SetForm(entity, targetForm);
            if(debugMode)
            {
                entity.World.Logger.Warning($"Form set to {targetForm}");

            }
            SetCooldown(entity);



            // 5 Store state
            tree.SetString("manualForm", targetForm.ToString());
            tree.SetBool("manualFormActive", reason == TransformationReason.ManualToggle);

            entity.MarkTagsDirty();
            entity.World.Logger.Warning($"TrySetForm applied: {targetForm} | ManualActive: {reason == TransformationReason.ManualToggle}");
        }
            
        public static void ProcessTransformation( IServerPlayer player)
        {

            var entity = player.Entity;
            if (entity == null) return;

            // Don't auto-transform if a manual override is active
            bool manualActive = player.Entity?.WatchedAttributes.GetBool("manualFormActive", false) ?? false;
            if (debugMode)
            {
                player.Entity?.World.Logger.Warning($"ProcessTransformation called | ManualActive = {manualActive}");
            }
            if (manualActive) return;



            // Only auto-transform infected players
            var infection = entity.GetBehavior<EntityBehaviorInfection>();
            if (infection == null || infection.CurrentInfection() != EntityBehaviorInfection.Infectionstatus.Infected) return;

            bool isNight = WolfTime.isNight(entity);
            // Determine target form based on night/day
            PlayerData.Forms decidedForm = isNight ? Forms.WereWolf : Forms.VulpisHuman; // if its night you're a werewolf if not you are a vulpishuman I will have t o make more mallable with parameters later I think

            // Apply transformation if not already in the target form
            if (PlayerData.GetForm(entity) != decidedForm)
            {
                // Save to watched attributes (this is save slot)
                TrySetForm(player, decidedForm, TransformationReason.Auto);

                // Apply stats using the decided form

                Stats.ApplyStats(entity, decidedForm);
                Stats.ApplyRegen(entity, decidedForm);
                if (debugMode)
                {
                   entity.World.Logger.Warning($"Stats applied for {decidedForm} | Regen applied for {entity}");
                }
            }

        }
        
             public static bool IsCooldownReady(EntityPlayer player)
        {
            long lastTransformTick = player.WatchedAttributes.GetLong("lastTransformTick", 0);
            long currentTick = player.World.ElapsedMilliseconds;
            return currentTick - lastTransformTick >= WereWolfModSettings.TransformCooldownMS;
        }

        private static void SetCooldown(EntityPlayer player)
        {
            player.WatchedAttributes.SetLong("lastTransformTick", player.World.ElapsedMilliseconds);
            player.WatchedAttributes.MarkPathDirty("lastTransformTick");
        }

            
           

        }


        
        }
    


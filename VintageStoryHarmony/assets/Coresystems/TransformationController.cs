using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Vintagestory;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using VintageStoryHarmony;
using WereWolf.assets.Coresystems.Infections;
using WereWolf.assets.Coresystems.StatRelated;
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
            var healthBehavior = entity.GetBehavior<EntityBehaviorHealth>();
            var tree = entity.WatchedAttributes;
            var currentForm = PlayerData.GetForm(entity);

          //  entity.World.Logger.Warning("[FLOW] TrySetForm reached.");

            //entity.World.Logger.Warning($"[DATA] TrySetForm called | Player: {player.PlayerName} | TargetForm: {targetForm} | Reason: {reason}");


            // 1️ Infection requirement
            if (reason == TransformationReason.ManualToggle)
            {
              //  entity.World.Logger.Warning($"[DATA] Infection behavior null? {infection == null}");
             //   entity.World.Logger.Warning($"[DATA] Current infection state: {infection?.CurrentInfection()}");

                if (infection == null || infection.CurrentInfection() != EntityBehaviorInfection.Infectionstatus.Infected)
                {

                    player.SendMessage(GlobalConstants.GeneralChatGroup, "You are not infected.", EnumChatType.Notification);
                 //   entity.World.Logger.Warning("[DATA] Returning because infection invalid");
                    return;
                }
            }

            // 2️ Cooldown check (if manual)

          //  entity.World.Logger.Warning($"[DATA] Current form: {currentForm}");
          //  entity.World.Logger.Warning($"[DATA] Cooldown check. Ready? {IsCooldownReady(entity)}");
            if (reason == TransformationReason.ManualToggle && targetForm == Forms.WereWolf && !IsCooldownReady(entity))
            {
                //   entity.World.Logger.Warning("[DATA] Returning because cooldown not ready");

                long lastTick = entity.WatchedAttributes.GetLong("lastTransformTick", 0);
                long elapsed = entity.World.ElapsedMilliseconds - lastTick;
                long remainingMS = Math.Max(0, WereWolfModSettings.TransformCooldownMS - elapsed);
                int minutes = (int)(remainingMS / 60000);
                int seconds = (int)((remainingMS % 60000) / 1000);

                player.SendIngameError($"You cannot transform yet! Cooldown remaining: {minutes}m {seconds}s.");
                return;
            }
            // 3️ If already in form, don't reapply
            if (GetForm(entity) == targetForm)
            {
          //      entity.World.Logger.Warning("[DATA] Returning because already in form");
                return;
            }           
        //    entity.World.Logger.Warning($"[DATA] Current infection state: {infection?.CurrentInfection()}");

            // 4️ Apply transformation
           // entity.World.Logger.Warning($"[DATA] About to call SetForm with {targetForm}");
            SetForm(entity, targetForm);
            if(debugMode)
            {
          //      entity.World.Logger.Warning($"[DATA] Form set to {targetForm}");

            }

          //  entity.World.Logger.Warning("[FLOW] About to call ApplyStats");
            StatsManager.ApplyStats(entity, targetForm);
           // entity.World.Logger.Warning($"[DATA] Stats applied for {targetForm} | MaxHealth: {healthBehavior?.MaxHealth}");
            if (targetForm == Forms.WereWolf && reason == TransformationReason.ManualToggle)
            {
                SetCooldown(entity);
            }

            entity.MarkTagsDirty();

            // 5 Store state
         //   tree.SetString("manualForm", targetForm.ToString());
            tree.SetBool("manualFormActive", reason == TransformationReason.ManualToggle);

            entity.MarkTagsDirty();
            entity.World.Logger.Warning($"[DATA] TrySetForm applied: {targetForm} | ManualActive: {reason == TransformationReason.ManualToggle}");
        }

        public static void ProcessTransformation(IServerPlayer player, float dt)
        {
            player.Entity.World.Logger.Warning("[FLOW] ProcessTransformation CALLED"); 
            var entity = player.Entity;
            player.Entity.World.Logger.Warning($"[DATA] ProcessTransformation start | Form: {PlayerData.GetForm(entity)}");

            if (entity == null)
            {
                entity?.World.Logger.Warning("[DATA] entity null returning...");
                return;
            }

            bool manualActive = entity.WatchedAttributes.GetBool("manualFormActive", false);
            if (manualActive) 
            {
                if (!IsCooldownReady(entity))
                    entity.World.Logger.Warning("[DATA] Manual Active — cooldown not ready, skipping auto...");
                return;
            }
            else
            {
                // Cooldown expired > reset manual lock
                entity.WatchedAttributes.SetBool("manualFormActive", false);
                entity.MarkTagsDirty();
                entity.World.Logger.Warning("[DATA] Manual lock expired, resuming auto logic.");
            }

            var infection = entity.GetBehavior<EntityBehaviorInfection>();
            if (infection == null || infection.CurrentInfection() != EntityBehaviorInfection.Infectionstatus.Infected)
            {
                entity.World.Logger.Warning($"[DATA] Infection: {infection?.CurrentInfection() ?? EntityBehaviorInfection.Infectionstatus.None} — returning...");
                return;
            }

            bool isNight = WolfTime.isNight(entity);
            Forms decidedForm = isNight ? Forms.WereWolf : Forms.VulpisHuman;

            Forms currentForm = PlayerData.GetForm(entity);

            // Handle transformation if needed
            if (currentForm != decidedForm)
            {
                TrySetForm(player, decidedForm, TransformationReason.Auto);
              
                currentForm = decidedForm; // update local value
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
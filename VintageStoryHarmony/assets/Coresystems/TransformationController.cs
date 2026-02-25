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

        public static void ProcessTransformation(IServerPlayer player)
        {
            var entity = player.Entity;
            if (entity == null) return;

            bool manualActive = entity.WatchedAttributes.GetBool("manualFormActive", false);
            if (manualActive) return;

            var infection = entity.GetBehavior<EntityBehaviorInfection>();
            if (infection == null || infection.CurrentInfection() != EntityBehaviorInfection.Infectionstatus.Infected)
                return;

            bool isNight = WolfTime.isNight(entity);
            Forms decidedForm = isNight ? Forms.WereWolf : Forms.VulpisHuman;

            Forms currentForm = PlayerData.GetForm(entity);

            // 1️ Handle transformation if needed
            if (currentForm != decidedForm)
            {
                TrySetForm(player, decidedForm, TransformationReason.Auto);
                Stats.ApplyStats(entity, decidedForm);
                currentForm = decidedForm; // update local value
            }

            // 2️ ALWAYS apply regen every tick

            float regen = Stats.GetRegenAmount(entity, currentForm);
            float deltaSeconds = entity.World.ElapsedMilliseconds; // or equivalent server delta
            regen *= deltaSeconds;
            Stats.ApplyRegen(entity, regen);
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
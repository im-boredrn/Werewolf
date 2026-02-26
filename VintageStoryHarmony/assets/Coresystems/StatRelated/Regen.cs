using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.GameContent;
using WereWolf.assets.Werewolf.Configuration;

namespace WereWolf.assets.Coresystems.StatRelated
{
    internal class Regen
    {

        public static float GetRegenAmount(EntityPlayer player, PlayerData.Forms form)
        {
            if (form != PlayerData.Forms.WereWolf) return 0f;

            bool night = WolfTime.isNight(player);
            return night ? WereWolfModSettings.NightRegen : WereWolfModSettings.DayRegen;
        }

        public static void ApplyRegen(EntityPlayer player) // TODO Refacor for scalibility
        {
            player.World.Logger.Warning("ApplyRegen CALLED.");

            //  if (player == null || regenAmount <= 0f) return;


            var healthBehavior = player.GetBehavior<EntityBehaviorHealth>();
            if (healthBehavior == null) return;

            var form = PlayerData.GetForm(player);
            bool night = WolfTime.isNight(player);

            // Only apply regen if WereWolf, Will Be reworkd for scalability
            if (form != PlayerData.Forms.WereWolf) return;

            float regenAmount = night ? WereWolfModSettings.NightRegen : WereWolfModSettings.DayRegen;
            float oldHealth = healthBehavior.Health;

            healthBehavior.Health = Math.Min(healthBehavior.MaxHealth, healthBehavior.Health + regenAmount);
            healthBehavior.MarkDirty();

            // Logging for isolation
            player.World.Logger.Warning($"[Regen] Tick | Form: {form} | Night: {night} | Amount: {regenAmount} | Health: {oldHealth} -> {healthBehavior.Health}");

        }
    }
}
    


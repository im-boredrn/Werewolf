using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using VintageStoryHarmony;
using VintageStoryHarmony.assets;
using WereWolf.assets.Werewolf.Configuration;
using static VintageStoryHarmony.assets.PlayerData;

namespace WereWolf.assets.Keybinds
{
    public class Keybind
    {

        private static long lastPressTime = 0; // store the last time the key was pressed
        private static bool keyPressed = false;

        public static bool OnKeybindPress(KeyCombination comb)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var mod = WereWolfModSystem.Instance;
            if (mod?.Capi == null) return false;

            var player = mod.Capi.World.Player?.Entity as EntityPlayer;
            if (player == null) return false;

            int remaining;
            if (IsOnCooldown(out remaining))
            {
                if (!keyPressed) // only notify the first time, this stops cooldown text spamming
                {
                    mod.Capi.TriggerIngameError(player, "cooldown", $"On Cooldown! Wait: {remaining} minutes.");
                }
                keyPressed = true; // mark key as being held


             
                return false;

            }
            lastPressTime = now;

            keyPressed = false;   // reset if action succeeds

            if (!mod.ManualFormActive)
                {
                    // Enable manual mode
                    mod.ManualFormActive = true;

                    var current = PlayerData.GetForm(player);
                    mod.ManualForm = (current == PlayerData.Forms.Human)
                        ? PlayerData.Forms.WereWolf
                        : PlayerData.Forms.Human;

                    // Apply immediately
                    PlayerData.SetForm(player, mod.ManualForm);
                    Stats.ApplyStats(player, mod.ManualForm);
                }
                else
                {
                    // Disable manual mode
                    mod.ManualFormActive = false;

                    // Do NOT set form here.
                    // Let server tick decide next second.
                }

                return true; // tells the game I handled the keypress
         }

        

        private static bool IsOnCooldown(out int remainingMinutes)
        {
            long cooldownMs = WereWolfModSettings.TransformCooldown * 60000L;
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (now - lastPressTime < cooldownMs) // if on cooldown
            {
                // calculate remaining time
                remainingMinutes = (int)Math.Ceiling((cooldownMs - (now - lastPressTime)) / 60000.0);
                return true;
            }

            // not on cooldown, update last press
            remainingMinutes = 0;
            return false;

        }
    }
}

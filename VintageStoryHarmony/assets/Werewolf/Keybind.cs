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
using static VintageStoryHarmony.assets.PlayerData;

namespace WereWolf.assets.Werewolf
{
    public class Keybind
    {




        public static bool OnKeybindPress(KeyCombination comb)
        {

            var mod = WereWolfModSystem.Instance;
            if (mod?.Capi == null) return false;

            var player = mod.Capi.World.Player?.Entity as EntityPlayer;
            if (player == null) return false;

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

            return true; // tells the game we handled the keypress
        }
    }
}

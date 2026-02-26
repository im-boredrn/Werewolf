using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace WereWolf.assets.Coresystems
{

    public class PlayerData
    {

        public enum Forms
        {
            UnchangedHuman,
            VulpisHuman,
            WereWolf
        }

        private const string FormKey = "werewolfForm";

        public static Forms GetForm(EntityPlayer player)
        {
            string stored = player.WatchedAttributes.GetString(FormKey, Forms.UnchangedHuman.ToString()); // I think this boils down to stored = formkey which is the save slot and unchanged human is the default
            bool success = Enum.TryParse<Forms>(stored, out var form); // try to turn string to enum if success form holds enum if not default enum is unchanged human 
            if (!success) form = Forms.UnchangedHuman;

            return form; // give save slot aka enum
        }

        public static void SetForm(EntityPlayer player, Forms form)
        {
            player.World.Logger.Warning($"[DATA] SetForm called on side: {player.World.Side} | Form: {form}");
            player.WatchedAttributes.SetString(FormKey, form.ToString()); // I have an enum value like Forms.VulpisHuman. turns it into "VulpisHuman" — a string that can be stored in watched attributes.
            player.WatchedAttributes.MarkPathDirty(FormKey); // Saves the string under the key FormKey in the player’s watched attributes. Now the game remembers this form even if you exit or reload.   This is why GetForm can read it back later.
            player.World.Logger.Warning("[FLOW] SetForm finished.");
        }
    }

}


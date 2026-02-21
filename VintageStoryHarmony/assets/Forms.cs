using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace VintageStoryHarmony.assets
{

    public class PlayerData
    {

        public enum Forms
        {
            Human,
            WereWolf
        }

        private const string FormKey = "werewolfForm";

        public static Forms GetForm(EntityPlayer player)
        {
            string stored = player.WatchedAttributes.GetString(FormKey, Forms.Human.ToString());
            return Enum.Parse<Forms>(stored);
        }

        public static void SetForm(EntityPlayer player, Forms form)
        {
            player.WatchedAttributes.SetString(FormKey, form.ToString());
            player.WatchedAttributes.MarkPathDirty(FormKey);
        }
    }

}


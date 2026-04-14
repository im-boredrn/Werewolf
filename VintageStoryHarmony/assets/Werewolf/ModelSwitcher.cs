using PlayerModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using VintageStoryHarmony;
using WereWolf.assets.Coresystems;
using WereWolf.assets.Werewolf.Configuration;

namespace WereWolf.assets.Werewolf
{
    internal class ModelSwitcher
    {
        public static void ModelSwitch(EntityPlayer entity)
        {
         
            if (WereWolfModSettings.ModelSwitcher != true) return;
            
            if (entity?.World?.Api is not ICoreClientAPI capi) return;
            
            var behavior = entity.GetBehavior<PlayerSkinBehavior>();
            if (behavior == null) return;

            if (PlayerData.GetForm(entity) == PlayerData.Forms.WereWolf && WereWolfModSettings.NormalForm == true)
            {
                behavior.SetCurrentModel("wolfplayer:werewolf", 1.4f);
            }
            else if (PlayerData.GetForm(entity) == PlayerData.Forms.VulpisHuman)
            {
                behavior.SetCurrentModel("seraph", 1.1f);
            }

            if (PlayerData.GetForm(entity) == PlayerData.Forms.WereWolf && WereWolfModSettings.LupineForm == true)
            {
                behavior.SetCurrentModel("lupines:lupine", 1.3f);

            }
            else if (PlayerData.GetForm(entity) == PlayerData.Forms.VulpisHuman)
            {
                behavior.SetCurrentModel("seraph", 1.1f);
            }
        }

    }
}

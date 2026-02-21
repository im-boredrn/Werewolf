using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using WereWolf.assets.Werewolf;

namespace VintageStoryHarmony.assets
{
    internal class Transform
    {


        public static void Transformation(Entity entity, EntityPlayer player)
        {
            var mod = WereWolfModSystem.Instance;
            if (mod == null)
            {
                return;
            }
            PlayerData.Forms decidedForm;

            if (mod.ManualFormActive)
            {

                decidedForm = mod.ManualForm;
                 
            }
          else if (WolfTime.isNight(entity))
            {
                decidedForm = PlayerData.Forms.WereWolf;


            }
         else
            {
                decidedForm = PlayerData.Forms.Human;
            }

            // Save to watched attributes (this is persistence)
            PlayerData.SetForm(player, decidedForm);

            // Apply stats using the decided form
            Stats.ApplyStats(player, decidedForm);
        }
    }
}

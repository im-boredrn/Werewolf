using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using VintageStoryHarmony;
using WereWolf.assets.Coresystems;
using WereWolf.assets.Werewolf.Configuration;
using static WereWolf.assets.Coresystems.PlayerData;

namespace WereWolf.assets.Keybinds
{
    public class Keybind
    {


        public static bool OnKeybindPress(KeyCombination comb)
        {
            var mod = WereWolfModSystem.Instance;
            if (mod == null) return false;

            var entity = mod?.Capi?.World.Player.Entity;



            var currentForm = PlayerData.GetForm(entity);
            var targetForm = currentForm == PlayerData.Forms.WereWolf
                             ? PlayerData.Forms.VulpisHuman
                             : PlayerData.Forms.WereWolf;

            var packet = new ToggleFormPacket { TargetForm = targetForm };
            WereWolfModSystem.Instance.Capi.Network.SendPacketClient(packet);

            return true;
        }

       

       
    }
}

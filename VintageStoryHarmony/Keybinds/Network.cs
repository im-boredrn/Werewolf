using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
namespace WereWolf.Keybinds
{
    internal class Network
    {
        public IClientNetworkChannel ClientChannel;
        public IServerNetworkChannel ServerChannel;


        private void RegisterServer(ICoreServerAPI sapi)
        {
            ServerChannel = sapi.Network.RegisterChannel("wolf");
            ServerChannel.RegisterMessageType<>
        }
    }
}

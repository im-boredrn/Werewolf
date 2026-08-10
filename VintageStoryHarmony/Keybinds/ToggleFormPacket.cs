using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using WereWolf.Coresystems;
namespace WereWolf.Keybinds
{

    [ProtoContract]
    public class ToggleFormPacket
    {
        [ProtoMember(1)]
        public PlayerData.Forms TargetForm;
    }

}
    
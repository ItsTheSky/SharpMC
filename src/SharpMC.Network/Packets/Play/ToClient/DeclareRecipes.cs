﻿using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class DeclareRecipes : Packet<DeclareRecipes>, IToClient
    {
        public byte ClientId => 0x66;


        public override void Decode(IMinecraftStream stream)
        {
        }

        public override void Encode(IMinecraftStream stream)
        {
        }
    }
}
using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Login.ToClient
{
    public class Disconnect : Packet<Disconnect>, IToClient
    {
        public byte ClientId => 0x00;

        public string Reason { get; set; }
        public DisconnectState State { get; set; }

        public override int PacketId => (int) State;

        public override void Decode(IMinecraftStream stream)
        {
            Reason = stream.ReadString();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteString(Reason);
        }
        
        public enum DisconnectState
        {
            Login = 0x00,
            Play = 0x1A
        }
    }
}

using SharpMC.API.Components;
using SharpMC.API.Players;
using SharpMC.API.Worlds;
using SharpMC.Plugin.API;

namespace SharpMC.API
{
    public interface IServer
    {
        void Start();

        void Stop();

        IEncryption RsaEncryption { get; }

        IPlayerFactory PlayerFactory { get; }

        IServerInfo Info { get; }

        ILevelManager LevelManager { get; }
        
        IPluginManager PluginManager { get; }
    }
}
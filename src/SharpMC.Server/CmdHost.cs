using System.Threading;
using SharpMC.API;

namespace SharpMC.Server
{
    internal sealed class CmdHost : IHostEnv
    {
        public CmdHost(string contentRoot,
            CancellationTokenSource source)
        {
            ContentRoot = contentRoot;
            Token = source;
        }

        public string ContentRoot { get; }
        public CancellationTokenSource Token { get; }
        public void Shutdown()
        {
            Token.Cancel();
        }
    }
}
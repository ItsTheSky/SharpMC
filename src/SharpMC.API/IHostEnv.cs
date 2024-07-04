namespace SharpMC.API
{
    public interface IHostEnv
    {
        string ContentRoot { get; }
        
        void Shutdown();
    }
}
namespace SharpMC.Network.API.Binary
{
    public interface IBinSerializable<T>
    {
        T ToCompound();

        void ToObject(T tag);
    }
}
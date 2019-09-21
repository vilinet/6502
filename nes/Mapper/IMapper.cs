
namespace NES.Mapper
{
    public interface IMapper
    {
        int Read(ushort address);
        int Write(ushort address);
        int ReadPpu(ushort address);
    }
}

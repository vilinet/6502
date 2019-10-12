namespace NES
{
    public class RomInfo
    {
        public bool HasBattery { get; internal set; }    
        public int ProgramBanks { get; internal set; }
        public int CharBanks { get; internal set; }
        public int MapperId { get; internal set; }
        public Mirroring Mirroring { get; internal set; }
        public bool HasTrainerData { get; internal set; }
        public bool IgnoreMirroringControl { get; internal set; }
        public bool Unisystem { get;  internal set; }
        public bool PlayChoice { get; internal set; }
        public bool NewFormat { get; internal set; }
    }
}
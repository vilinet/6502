using emulator6502;
using NES.Interfaces;

namespace NES
{
    internal class ControllerDevice : IAddressable
    {
        public ushort From { get; }
        public ushort To { get; }

        private readonly IController _controller;
        
        private byte _stateCache = 0;

        private byte _latch = 0;

        public ControllerDevice(ushort port, IController controller)
        {
            From = port;
            To = port;
            _controller = controller;
        }
        
        public void Write(ushort address, byte value)
        {
            if (_controller == null) return;

            if(address == 0x4016)
            {
                if(_latch == 1 && value % 8  == 0)
                {
                    _stateCache = _controller.State;
                }

                _latch = (byte)(value%8);
            }
        }
        
        public byte Read(ushort address)
        {
            var bit = _stateCache & 1;
            _stateCache =  (byte)(_stateCache >> 1);
            return (byte)bit;
        }
    }
}
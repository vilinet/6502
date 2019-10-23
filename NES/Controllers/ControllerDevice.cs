using emulator6502;
using NESInterfaces;

namespace NES
{
    internal class ControllerDevice : IAddressable
    {
        public ushort From { get; }
        public ushort To { get; }

        private readonly IController _controller;
        
        private byte _readCount = 8;
        private byte _stateCache = 0;

        public ControllerDevice(ushort port, IController controller)
        {
            From = port;
            To = port;
            _controller = controller;
        }
        
        public void Write(ushort address, byte value)
        {
     
        }
        
        public byte Read(ushort address)
        {
            if (_controller == null) return 0;
            if (_readCount == 8)
            {
                _readCount = 0;
                _stateCache = _controller.GetState();
            }

            byte v = (byte)(_stateCache & 0b00000001);
            _readCount++;
            _stateCache =  (byte)(_stateCache >> 1);
            return v;
        }
    }
}
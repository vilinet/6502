﻿using NESInterfaces;

namespace NES.Controllers
{
    public class Controller : IController
    {
        private byte _state;

        public byte GetState()
        {
            return _state;
        }

        public void SetButtonState(ControllerButton button, bool pressed)
        {
            if (pressed) _state |= (byte)button;
            else _state &= (byte)~button;
        }
    }
}

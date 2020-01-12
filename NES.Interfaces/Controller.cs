namespace NES.Interfaces
{
    public class Controller : IController
    {
        public byte State { get; private set; }    

        public void SetButtonState(ControllerButton button, bool pressed)
        {
            if (pressed) State |= (byte)button;
            else State &= (byte)~button;
        }
    }
}

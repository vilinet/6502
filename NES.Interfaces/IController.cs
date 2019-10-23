namespace NESInterfaces
{
    public enum ControllerButton : byte
    {
        A = 1,
        B = 2,
        Select = 4,
        Start = 8,
        Up = 16,
        Down = 32,
        Left = 64,
        Right = 128
    }
    public interface IController
    {
        /// <summary>
        /// Bits
        ///  
        /// 0 - A
        ///1 - B
        ///2 - Select
        ///3 - Start
        ///4 - Up
        ///5 - Down
        ///6 - Left
        ///7 - Right
        /// </summary>
        /// <returns></returns>
        byte GetState();

        void SetButtonState(ControllerButton button, bool pressed);
    }
}
namespace NES.Interfaces
{
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
        byte State { get; }

        void SetButtonState(ControllerButton button, bool pressed);
    }
}
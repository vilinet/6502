using NES.Interfaces;
using SFML.Window;

namespace NES.Display.SFML
{
    public class SFMLNesDisplay : SFMLGeneralDisplay
    {
        protected Controller Controller1 { get; }

        public SFMLNesDisplay(string title, uint width, uint height) : base(title, width, height)
        {
            Controller1 = new Controller();
        }

        protected override void HandleEvents()
        {
            while (PollEvent(out Event ev))
            {
                bool? pressed = null;

                switch (ev.Type)
                {
                    case EventType.Closed:
                        Close();
                        break;
                    case EventType.KeyPressed:
                        pressed = true;
                        break;
                    case EventType.KeyReleased:
                        pressed = false;

                        OnKeyRelease(ev.Key.Code);
                        break;
                }

                if (pressed.HasValue)
                {
                    var c = ev.Key.Code;
                    switch (c)
                    {
                        case Keyboard.Key.Left:
                            Controller1.SetButtonState(NESInterfaces.ControllerButton.Left, pressed.Value);
                            break;
                        case Keyboard.Key.Right:
                            Controller1.SetButtonState(NESInterfaces.ControllerButton.Right, pressed.Value);
                            break;
                        case Keyboard.Key.Up:
                            Controller1.SetButtonState(NESInterfaces.ControllerButton.Up, pressed.Value);
                            break;
                        case Keyboard.Key.Down:
                            Controller1.SetButtonState(NESInterfaces.ControllerButton.Down, pressed.Value);
                            break;
                        case Keyboard.Key.Backspace:
                            Controller1.SetButtonState(NESInterfaces.ControllerButton.Select, pressed.Value);
                            break;
                        case Keyboard.Key.Enter:
                            Controller1.SetButtonState(NESInterfaces.ControllerButton.Start, pressed.Value);
                            break;
                        case Keyboard.Key.A:
                            Controller1.SetButtonState(NESInterfaces.ControllerButton.A, pressed.Value);
                            break;
                        case Keyboard.Key.S:
                            Controller1.SetButtonState(NESInterfaces.ControllerButton.B, pressed.Value);
                            break;
                        case Keyboard.Key.Escape:
                            Close();
                            break;
                    }
                }
            }
        }
    }
}
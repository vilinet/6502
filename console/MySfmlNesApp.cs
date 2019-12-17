using emulator6502;
using NES;
using NES.Display.SFML;
using SFML.Window;
using System;
using NES.Interfaces;

namespace console
{
    public class MySfmlNesApp : SFMLNesDisplay
    {
        public Nes Nes { get; }

        public MySfmlNesApp(int x, int y, uint w, uint h, string rom) : base("NES", w, h)
        {
            Nes = new Nes(this, Controller1);
            Nes.LoadRom(rom);
            Nes.RunOnThread();
            Position = new SFML.System.Vector2i(x, y);
        }

        protected override void OnPostDraw()
        {
            DrawText(0, 0, Nes.ActualFps.ToString(), 20);
        }

        protected override void OnKeyRelease(Keyboard.Key key)
        {
            switch (key)
            {
                case Keyboard.Key.Space:
                    if (Nes.State == NesState.Running)
                        Nes.Pause();
                    else Nes.Resume();
                    break;

                case Keyboard.Key.R:
                    Nes.Reset();
                    break;

                case Keyboard.Key.Add:
                    Nes.Speed += 0.1f;
                    break;

                case Keyboard.Key.Subtract:
                    Nes.Speed -= 0.1f;
                    break;

                case Keyboard.Key.Multiply:
                    Nes.Speed = Math.Min(Nes.Speed + 0.25f, 3);
                    break;

                case Keyboard.Key.Divide:
                    Nes.Speed = Math.Max(Nes.Speed - 0.25f, 0);
                    break;

                case Keyboard.Key.PageDown:
                    Nes.Tick();
                    Nes.Tick();
                    Nes.Tick();
                    break;
            }
        }
    }
}

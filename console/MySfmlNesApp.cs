using NES;
using NES.Display.SFML;
using SFML.Graphics;
using SFML.Window;
using System;

namespace console
{
    public class MySfmlNesApp : SFMLNesDisplay
    {
        public Nes Nes { get; }
        private string _superSlow = string.Empty;

        public MySfmlNesApp(int x, int y, uint w, uint h, string rom, uint scale = 1) : base("NES", w, h,scale)
        {
            Nes = new Nes(this, Controller1);
            Nes.LoadRom(rom);
            Nes.RunOnThread();
            Position = new SFML.System.Vector2i(x, y);
        }

        protected override void OnPostDraw()
        {
            DrawText(0, 0, Nes.ActualFps.ToString(), 20);
            if(_superSlow.Length >0)
            {

                DrawText(0, 20, _superSlow,20,  Color.Red);
            }
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
                case Keyboard.Key.B:

                    if (Nes.SuperSlow == 0)
                    {
                        Nes.SuperSlow = 2000;
                        _superSlow = "Slow";
                    }
                    else if (Nes.SuperSlow == 2000)
                    {
                        Nes.SuperSlow = 16000;
                        _superSlow = "Very Slow";
                    }
                    else if (Nes.SuperSlow == 16000)
                    {
                        Nes.SuperSlow = 48000;
                        _superSlow = "Super Slow";
                    }
                    else if (Nes.SuperSlow == 48000)
                    {
                        Nes.SuperSlow = 140000;
                        _superSlow = "Ultra Slow";
                    }
                    else
                    {
                        Nes.SuperSlow = 0;
                        _superSlow = string.Empty;
                    }
                    break;
            }
        }
    }
}

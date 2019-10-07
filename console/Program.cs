using emulator6502;
using NES;
using NES.Display.SDL2;
using SDL2;
using System;

namespace console
{
    public class MyNesDisplay : SDL2NesGameDisplay
    {
        private Nes _nes;
        private readonly bool _debug;

        public MyNesDisplay(bool debug = false) : base("NES", debug?256 * 4:256, debug?240 * 2:240, debug?256 * 2: 256, 240, fontSize: 17)
        {
            _debug = debug;
            _nes = new Nes(this, this, this);
            _nes.LoadPalette("mesen.pal");
            _nes.LoadRom("./smb.nes");
            _nes.RunOnThread();
        }
        public override void OnRenderText(IDrawText renderer)
        {
            renderer.DrawText(0f, 0f, "FPS: " + _nes.ActualFps.ToString(),0xFFFFFF00 , TextAlignment.Default);
        }

        protected override void OnBeforeRender()
        {
            if(_debug == true)
            {
                base.OnBeforeRender();
            }
        }

        protected override void OnKeyDown(SDL.SDL_Keysym e)
        {
            switch (e.sym)
            {
                case SDL.SDL_Keycode.SDLK_SPACE:
                    if (_nes.State == NesState.Running)
                        _nes.Pause();
                    else _nes.Resume();
                    break;
                case SDL.SDL_Keycode.SDLK_r: _nes.Reset(); break;
                case SDL.SDL_Keycode.SDLK_KP_PLUS : _nes.Speed += 0.1f; break;
                case SDL.SDL_Keycode.SDLK_KP_MINUS: _nes.Speed -= 0.1f; break;
                case SDL.SDL_Keycode.SDLK_KP_MULTIPLY:
                    _nes.Speed =Math.Min(_nes.Speed + 0.25f, 3);
                    Console.WriteLine();
                    break;
                case SDL.SDL_Keycode.SDLK_KP_DIVIDE:
                    _nes.Speed =Math.Max(_nes.Speed -0.25f, 0);
                    break;
                default:
                    base.OnKeyDown(e);
                    break;
            }
        }

        void Cpu_BeforeOperationExecuted(Cpu cpu, OpcodeEventArgs e)
        {
            try
            {
                var bb = e.Full.ToString(cpu.GetValue(e.Full)).PadRight(40);
                var str =
                    ($"{bb} A:{cpu.A:X2} X:{cpu.X:X2} Y:{cpu.Y:X2} P:{cpu.Status.Value:X2} SP:{cpu.SP:X2} Cycles: {e.ElapsedCycles}");
                Console.WriteLine(str);
            }
            catch { }
        }
    }

    static class Program
    {
        static void Main(string[] args)
        {
            var display = new MyNesDisplay();

            while (display.IsOpen)
            {
                display.Render();
            }
        }
    }
}
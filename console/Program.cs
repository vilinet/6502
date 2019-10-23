using emulator6502;
using NES;
using NES.Display.SDL2;
using NES.Interfaces;
using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using CodeTerminal;

namespace console
{
    public class MyNesApp : SDL2NesGameDisplay, IDebugDisplay
    {
        private readonly Nes _nes;
        private readonly bool _debug;
        private readonly List<Tuple<int, int, string>> _texts = new List<Tuple<int, int, string>>();

        public MyNesApp(bool debug = false) : base("NES", debug ? 256 * 4 : 256, debug ? 240 * 2 : 240,
            debug ? 256 * 2 : 256, 240, fontSize: 15)
        {
            _debug = debug;
            _nes = new Nes(this, debug ? this : null, this);
            _nes.LoadRom("./smb.nes");
            _nes.RunOnThread();
        }

        public void DrawText(int x, int y, string text)
        {
            _texts.Add(new Tuple<int, int, string>(x, y, text));
        }

        public override void OnRenderText(IDrawText renderer)
        {
            //renderer.DrawText(0f, 0f, "FPS: " + _nes.ActualFps.ToString(), 0xFFFFFF00, TextAlignment.Default);
            foreach (var item in _texts.ToList())
            {
                if (item == null) continue;

                float x = (float) item.Item1 / InternalWidth;
                float y = (float) item.Item2 / InternalHeight;

                renderer.DrawText(x, y, item.Item3, new SDL.SDL_Color()
                {
                    a = 255,
                    r = 255,
                    b = 255,
                    g = 255
                });
            }
        }

        private SDL.SDL_Color GetColor(int x, int y)
        {
            uint color = Pixels[InternalWidth * y + x];

            return new SDL.SDL_Color()
            {
                a = 255,
                b = (byte) (color & 0xFF),
                g = (byte) ((color & 0xFF00) >> 8),
                r = (byte) ((color & 0xFF0000) >> 16)
            };
        }


        private void SetColor(int x, int y, SDL.SDL_Color color)
        {
            uint val = (uint) ((color.r << 16) + (color.g << 8) + color.b);
            DrawPixel(x, y, val);
        }

        protected override void OnEffectApply(int width, int height, uint[] pixels)
        {
            return;
            uint[] original = new uint[pixels.Length];
            pixels.CopyTo(original, 0);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var c = GetColor(i, j);
                    var avg = (byte) ((c.r + c.g + c.b) / 3);
                    SetColor(i, j, new SDL.SDL_Color()
                    {
                        a = 255,
                        r = avg,
                        g = avg,
                        b = avg
                    });
                }
            }
        }

        protected override void OnBeforeRender()
        {
            if (_debug == true) base.OnBeforeRender();
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
                case SDL.SDL_Keycode.SDLK_r:
                    _nes.Reset();
                    break;
                case SDL.SDL_Keycode.SDLK_KP_PLUS:
                    _nes.Speed += 0.1f;
                    break;
                case SDL.SDL_Keycode.SDLK_KP_MINUS:
                    _nes.Speed -= 0.1f;
                    break;
                case SDL.SDL_Keycode.SDLK_KP_MULTIPLY:
                    _nes.Speed = Math.Min(_nes.Speed + 0.25f, 3);
                    Console.WriteLine();
                    break;
                case SDL.SDL_Keycode.SDLK_KP_DIVIDE:
                    _nes.Speed = Math.Max(_nes.Speed - 0.25f, 0);
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
                    ($"{bb} A:{cpu.A:X2} X:{cpu.X:X2} Y:{cpu.Y:X2} P:{cpu.Status.Value:X2} SP:{cpu.SP:X2} Cycles: {e.ElapsedCycles}"
                    );
                Console.WriteLine(str);
            }
            catch
            {
            }
        }
    }

    static class Program
    {
        static void Main(string[] args)
        {
            var terminal = new Terminal();

            terminal.Run();
            /*
            var app = new MyNesApp(true);

            while (app.IsOpen)
            {
                app.Render();
            }*/

        }
    }
}